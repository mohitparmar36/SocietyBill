using SocietyBill.Application.Interfaces.Repositories;
using SocietyBill.Application.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Services
{
    public class PdfService : IPdfService
    {
        private readonly IBillRepository _billRepository;
        private readonly IFlatRepository _flatRepository;

        public PdfService(IBillRepository billRepository, IFlatRepository flatRepository)
        {
            _billRepository = billRepository;
            _flatRepository = flatRepository;
        }

        public async Task<byte[]> GenerateBillPdfAsync(Guid billId, CancellationToken cancellationToken = default)
        {
            var bill = await _billRepository.GetByIdAsync(billId, cancellationToken);
            if (bill == null) throw new Exception("Bill not found");
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                    page.Header().Element(compose => ComposeHeader(compose, "INVOICE", $"Flat {bill.Flat.FlatNumber}\n{bill.Flat.OwnerName}"));

                    page.Content().Element(compose =>
                    {
                        compose.PaddingVertical(1, Unit.Centimetre).Column(column =>
                        {
                            column.Item().Text($"Bill Amount: ₹{bill.Amount:0.00}").FontSize(16).SemiBold();
                            column.Item().Text($"Due Date: {bill.DueDate:MMM dd, yyyy}");
                            column.Item().Text($"Status: {(bill.IsPaid ? "Paid" : "Unpaid")}").FontColor(bill.IsPaid ? Colors.Green.Medium : Colors.Red.Medium);
                        });
                    });

                    page.Footer().Element(ComposeFooter);
                });
            });
            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateYearlyStatementPdfAsync(string auth0UserId, int year, CancellationToken cancellationToken = default)
        {
            var flat = await _flatRepository.GetByAuth0UserIdAsync(auth0UserId, cancellationToken);
            if (flat == null) throw new Exception("Flat not found");
            var bills = await _billRepository.GetByFlatIdAsync(flat.Id, year, cancellationToken);
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                    page.Header().Element(compose => ComposeHeader(compose, $"YEARLY STATEMENT {year}", $"Flat {flat.FlatNumber}\n{flat.OwnerName}"));

                    page.Content().Element(compose =>
                    {
                        compose.PaddingVertical(1, Unit.Centimetre).Column(column =>
                        {
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Month");
                                    header.Cell().Element(CellStyle).Text("Amount");
                                    header.Cell().Element(CellStyle).Text("Due Date");
                                    header.Cell().Element(CellStyle).Text("Generated On");
                                    header.Cell().Element(CellStyle).Text("Status");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                    }
                                });

                                foreach (var b in bills.OrderBy(x => x.Month))
                                {
                                    table.Cell().Element(CellStyle).Text(new DateTime(year, b.Month, 1).ToString("MMMM"));
                                    table.Cell().Element(CellStyle).Text($"₹{b.Amount:0.00}");
                                    table.Cell().Element(CellStyle).Text($"{b.DueDate:dd MMM yyyy}");
                                    table.Cell().Element(CellStyle).Text($"{b.GeneratedAt:dd MMM yyyy}");
                                    table.Cell().Element(CellStyle).Text(b.IsPaid ? "Paid" : "Unpaid").FontColor(b.IsPaid ? Colors.Green.Medium : Colors.Red.Medium);

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                                    }
                                }
                            });

                            column.Item().PaddingTop(25).AlignRight().Text($"Total Paid: ₹{bills.Where(x => x.IsPaid).Sum(x => x.Amount):0.00}").FontSize(14).SemiBold();
                            column.Item().AlignRight().Text($"Total Outstanding: ₹{bills.Where(x => !x.IsPaid).Sum(x => x.Amount):0.00}").FontSize(14).SemiBold().FontColor(Colors.Red.Medium);
                        });
                    });

                    page.Footer().Element(ComposeFooter);
                });
            });
            return document.GeneratePdf();
        }

        private void ComposeHeader(IContainer container, string title, string subtitle)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text(title).FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text(subtitle).FontSize(14).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(150).AlignRight().Column(column =>
                {
                    column.Item().Text("SocietyBill").FontSize(20).SemiBold().FontColor(Colors.Grey.Darken3);
                    column.Item().Text("Billing Platform").FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(x =>
            {
                x.Span("Page ");
                x.CurrentPageNumber();
                x.Span(" of ");
                x.TotalPages();
            });
        }
    }
}