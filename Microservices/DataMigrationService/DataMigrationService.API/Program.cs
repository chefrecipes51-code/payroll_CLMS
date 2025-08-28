using DataMigrationService.DAL.Interface;
using DataMigrationService.DAL.Service;
using Microsoft.Extensions.Configuration;
using Payroll.Common.Middlewares;
using Payroll.Common.Repository.Service;
using System.Data.SqlClient;
using System.Data;
using DataMigrationService.API.Controllers;

var builder = WebApplication.CreateBuilder(args);
// Access configuration
var configuration = builder.Configuration;
// Register database connection with correct configuration reference
builder.Services.AddSingleton<IDbConnection>(sp =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new SqlConnection(connectionString);
});

builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register repository service
#region repository

builder.Services.AddScoped<IServiceImportMasterRepository, ServiceImportMasterServiceRepository>();


builder.Services.AddScoped<ErrorLogServiceRepository>();
#endregion
var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication(); // Add this line to use authentication
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleware>();// Global Exception MiddleWare
app.MapControllers();

app.Run();
