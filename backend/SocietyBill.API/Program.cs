using Microsoft.EntityFrameworkCore;
using SocietyBill.API.Extensions;

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add Auth0 authentication
builder.Services.AddAuth0Authentication(builder.Configuration);

// Add DI for services, repositories, DbContext, etc.
builder.Services.AddSocietyBillServices(builder.Configuration);

// Register application services
builder.Services.AddHttpClient<SocietyBill.Application.Interfaces.Services.IAuth0ManagementService, SocietyBill.Infrastructure.Services.Auth0ManagementService>();
builder.Services.AddScoped<SocietyBill.Application.Interfaces.Services.IBillService, SocietyBill.Application.Services.BillService>();
builder.Services.AddScoped<SocietyBill.Application.Interfaces.Services.IFlatService, SocietyBill.Application.Services.FlatService>();
builder.Services.AddScoped<SocietyBill.Application.Interfaces.Services.IPdfService, SocietyBill.Application.Services.PdfService>();
builder.Services.AddScoped<SocietyBill.Application.Interfaces.Services.IInvitationService, SocietyBill.Application.Services.InvitationService>();

// Register repositories
builder.Services.AddScoped<SocietyBill.Application.Interfaces.Repositories.IBillRepository, SocietyBill.Infrastructure.Repositories.BillRepository>();
builder.Services.AddScoped<SocietyBill.Application.Interfaces.Repositories.IFlatRepository, SocietyBill.Infrastructure.Repositories.FlatRepository>();
builder.Services.AddScoped<SocietyBill.Application.Interfaces.Repositories.ISocietyRepository, SocietyBill.Infrastructure.Repositories.SocietyRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Use global exception handler
app.UseMiddleware<SocietyBill.API.Middlewares.ExceptionHandlingMiddleware>();
// Removed TenantIdentifierMiddleware as it blocks CORS preflight and is redundant with TenantProvider.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply migrations or create database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SocietyBill.Infrastructure.Data.AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
