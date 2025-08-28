using System.Data;
using System.Data.SqlClient;
using System.Text;
using Payroll.Common;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.Middleware;
using Payroll.Common.Middlewares;
using Payroll.Common.Repository.Interface;
using Payroll.Common.Repository.Service;
using PayrollTransactionService.API.Helper;
using PayrollTransactionService.DAL.Interface;
using PayrollTransactionService.DAL.Service;
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
builder.Services.AddScoped<IUserActivateDeactivateStatusRepository, UserActivateDeactivateStatusRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IErrorLogRepository, ErrorLogServiceRepository>();
builder.Services.AddScoped<IAuditTrailRepository, AuditTrailRepository>();
builder.Services.AddScoped<IPendingApprovalRequestRepository, PendingApprovalRequestRepository>();
builder.Services.AddScoped<IPayPeriodRepository, PayPeriodRepository>();
builder.Services.AddScoped<IAttendancePayPeriodRepository, AttendancePayPeriodRepository>();
builder.Services.AddScoped<IPayComponentMasterRepository, PayComponentMasterServiceRepository>();
builder.Services.AddScoped<IPayGradeMasterRepository, PayGradeMasterServiceRepository>();
builder.Services.AddScoped<ITaxSlabMasterRepository, TaxSlabMasterServiceRepository>();
builder.Services.AddScoped<IMapEntityTaxRegimeRepository, MapEntityTaxRegimeServiceRepository>();
builder.Services.AddScoped<IEntityMasterRepository, EntityMasterServiceRepository>();
builder.Services.AddScoped<IContractordetailsRepository, ContractorDetailsServiceRepository>();
builder.Services.AddScoped<IContractorValidationRepository, ContractorValidationServiceRepositor>();

builder.Services.AddScoped<IPTaxSlabRepository, PTaxSlabRepository>();
builder.Services.AddScoped<IParameterRepository, ParameterRepository>();
builder.Services.AddScoped<IPayrollProcessRepository, PayrollProcessRepositoryService>();
builder.Services.AddScoped<IPayrollTransactionStagingRepository, PayrollTransactionStagingRepository>();

//builder.Services.AddScoped<ApiKeyValidatorHelper>();
builder.Services.SecurityServicesForTransaction();//FOR X-API-KEY we Required These line

//builder.Services.AddScoped<ErrorLogServiceRepository>();


//builder.Services.AddHostedService<RabbitMQConsumerService>();
#endregion
// Background service
//builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
//builder.Services.AddHostedService<SalaryProcessingWorker>();
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
// Add controllers
builder.Services.AddControllers();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
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
app.UseSession();// Use session before adding custom middlewares
app.UseMiddleware<GlobalExceptionMiddleware>();// Global Exception MiddleWare
app.MapControllers();

app.Run();