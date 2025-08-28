using System.Data;
using System.Data.SqlClient;
using System.Text;
using Payroll.Common;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.APIKeyManagement.Interface;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.Middleware;
using Payroll.Common.Middlewares;
using Payroll.Common.Repository.Interface;
using Payroll.Common.Repository.Service;
using PayrollMasterService.API.Helper;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using PayrollMasterService.DAL.Service;

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
builder.Services.AddScoped<IWageGradeMasterRepository, WageGradeMasterServiceRepository>();
builder.Services.AddScoped<IWageConfigDetailRepository, WageConfigDetailServiceRepository>();
builder.Services.AddScoped<IWageRateMasterRepository, WageRateMasterServiceRepository>();
builder.Services.AddScoped<IEarningDeductionMasterRepository, EarningDeductionMasterServiceRepository>();
builder.Services.AddScoped<IAreaMasterRepository, AreaMasterServiceRepository>();
builder.Services.AddScoped<ILocationMasterRepository, LocationMasterServiceRepository>();
builder.Services.AddScoped<ICompanyCurrencyMasterRepository, CompanyCurrencyMasterServiceRepository>();
builder.Services.AddScoped<ICompanyMasterRepository, CompanyMasterServiceRepository>();
builder.Services.AddScoped<IDepartmentMasterRepository, DepartmentMasterServiceRepositroy>();
builder.Services.AddScoped<IMapDepartmentLocationRepository, MapDepartmentLocationServiceRepository>();
builder.Services.AddScoped<ICachingServiceRepository, CachingServiceRepository>();
builder.Services.AddScoped<IYearlyItTableMasterRepository, YearlyItTableMasterServiceRepository>();
builder.Services.AddScoped<IDetailYearlyItTableMasterRepository, DetailYearlyItTableMasterServiceRepository>();
builder.Services.AddScoped<IRoleMasterRepository, RoleMasterServiceRepository>();
builder.Services.AddScoped<IMapUserLocationRepository, MapUserLocationServiceRepository>();
builder.Services.AddScoped<IMapDepartmentRoleRepository, MapDepartmentRoleServiceRepository>();
builder.Services.AddScoped<IEventMasterRepository, EventMasterServiceRepository>();
builder.Services.AddScoped<IRoleMenuMappingRepository, RoleMenuMappingRepository>();
builder.Services.AddScoped<IEventAuthMappingRepository, EventAuthMappingRepository>();
builder.Services.AddScoped<ICompanyLocationRepository, CompanyLocationServiceRepository>();
builder.Services.AddScoped<IEntityTaxStatutoryRepository, EntityTaxStatutoryServiceRepository>();
builder.Services.AddScoped<IFormulaMasterRepository, FormulaMasterServiceRepository>();
builder.Services.AddScoped<IApprovalSetUpRepository, ApprovalSetUpRepository>();
builder.Services.AddScoped<ISalaryStructureRepository, SalaryStructureServiceRepository>();
builder.Services.AddScoped<IReportRepository, ReportServiceRepository>();
builder.Services.AddScoped<ISalaryStructureRepository, SalaryStructureServiceRepository>();
builder.Services.AddScoped<IGeneralAccountingHeadRepository, GeneralAccountingHeadRepository>();
builder.Services.AddScoped<IGeographicalLocationRepository, GeographicalLocationServiceRepository>();
builder.Services.AddScoped<IGLGroupRepository, GLGroupRepository>();
builder.Services.AddScoped<ErrorLogServiceRepository>();
builder.Services.AddHostedService<RabbitMQConsumerService>();

//builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
//builder.Services.AddScoped<ApiKeyValidatorHelper>();
//builder.Services.AddScoped<ISSOLoginService, SSOLoginService>();
//builder.Services.AddScoped<ApiKeyValidatorHelper>();
builder.Services.SecurityServicesForPayrollMaster(); //FOR X-API-KEY we Required These line


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