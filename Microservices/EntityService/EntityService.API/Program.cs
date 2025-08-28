using EntityService.DAL.Interface;
using EntityService.DAL.Service;
using Payroll.Common.Repository.Interface;
using Payroll.Common.Repository.Service;
using Payroll.Common;
using System.Data.SqlClient;
using System.Data;

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

#region repository
// Register repository service
builder.Services.AddScoped<IErrorLogRepository, ErrorLogServiceRepository>();
builder.Services.AddScoped<IEntityMasterTranRepository, EntityMasterTranRepository>();
//builder.Services.AddScoped<ErrorLogServiceRepository>();


//builder.Services.AddHostedService<RabbitMQConsumerService>();
#endregion

builder.Services.ServiceRegister();
// Register ErrorLogServiceRepository as a scoped service
//builder.Services.AddScoped<ErrorLogServiceRepository>();
// Add session services
builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory cache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout duration
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Required to keep session active even if user rejects cookies
});
// Add Data Protection service (required for session)
builder.Services.AddDataProtection();



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
