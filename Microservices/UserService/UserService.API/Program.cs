using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Payroll.Common;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.APIKeyManagement.Interface;
using Payroll.Common.APIKeyManagement.Service;
using Payroll.Common.Middlewares;
using Payroll.Common.Repository.Interface;
using Payroll.Common.Repository.Service;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using UserService.API.Helper;
using UserService.DAL.Interface;
using UserService.DAL.Service;

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
//// Add JWT authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = configuration["Jwt:Issuer"],
//            ValidAudience = configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
//        };
//    });

//builder.Services.AddAuthorization();
// Register repository service
builder.Services.AddScoped<IUserRepository, UserServiceRepository>();
builder.Services.AddScoped<IMappingUserCompanyRepository, MappingUserCompanyRepository>();
builder.Services.AddScoped<IMappingUserRoleRepository, MappingUserRoleRepository>();
builder.Services.AddScoped<ICountryMasterRepository, CountryMasterServiceRepository>();
builder.Services.AddScoped<IUserTypeMasterRepository, UserTypeMasterServiceRepository>();
builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();

builder.Services.AddScoped<IModuleMasterRepository, ModuleMasterServiceRepository>();
builder.Services.AddScoped<IServiceMasterRepository, ServiceMasterServiceRepository>();

//builder.Services.AddScoped<IApiKeyService, ApiKeyService>();
//builder.Services.AddScoped<ApiKeyValidatorHelper>();
//builder.Services.AddScoped<ISSOLoginService, SSOLoginService>();
builder.Services.SecurityServicesForUserService();//FOR X-API-KEY we Required These line

// Register repository service
//builder.Services.AddScoped<UserServiceRepository>();

// Register ErrorLogServiceRepository as a scoped service
//builder.Services.AddScoped<ErrorLogServiceRepository>();
builder.Services.ServiceRegister();
// Add session services
builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory cache
//builder.Services.AddMemoryCache();
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