using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocietyBill.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "society_bill");

            migrationBuilder.CreateTable(
                name: "Societies",
                schema: "society_bill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Societies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Flats",
                schema: "society_bill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FlatNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OwnerName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Auth0UserId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SocietyId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flats_Societies_SocietyId",
                        column: x => x.SocietyId,
                        principalSchema: "society_bill",
                        principalTable: "Societies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                schema: "society_bill",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FlatId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bills_Flats_FlatId",
                        column: x => x.FlatId,
                        principalSchema: "society_bill",
                        principalTable: "Flats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bills_FlatId",
                schema: "society_bill",
                table: "Bills",
                column: "FlatId");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_Year",
                schema: "society_bill",
                table: "Bills",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_Flats_Auth0UserId",
                schema: "society_bill",
                table: "Flats",
                column: "Auth0UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Flats_SocietyId",
                schema: "society_bill",
                table: "Flats",
                column: "SocietyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bills",
                schema: "society_bill");

            migrationBuilder.DropTable(
                name: "Flats",
                schema: "society_bill");

            migrationBuilder.DropTable(
                name: "Societies",
                schema: "society_bill");
        }
    }
}
