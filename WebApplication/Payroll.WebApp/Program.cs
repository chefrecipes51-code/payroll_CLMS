using Payroll.Common.Middlewares;
using Payroll.Common.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Payroll.WebApp.CommonService;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.SignalRHubs;
using Payroll.Common;
using Payroll.Common.FtpUtility;
using Payroll.Common.Repository.Interface;
using Payroll.Common.Repository.Service;
using Payroll.WebApp.Middleware;
using DataMigrationService.BAL.Models;
using Payroll.WebApp.Extensions;
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettingsNew"));

// Register the IDbConnection as SqlConnection
var keyLocation = builder.Configuration["DataProtection:KeyLocation"];
#region ADD CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAllOrigins",
//        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
//});

/////////////////////// As per Sir Input When Ocelot introduce we will ADD CORS :- STARt ///////////////////////
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: MyAllowSpecificOrigins,
//    policy =>
//    {
//        policy.WithOrigins("https://localhost:7230","https://localhost:7093")
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});
/////////////////////// As per Sir Input When Ocelot introduce we will ADD CORS :- END ///////////////////////
#endregion
/// <summary>
///  Developer Name :- Harshida Parmar
///  Created Date   :- 16-Sep-2024
///  Message detail :- AddDataProtection() 
///     1.: - It is part of ASP.NET Core's Data Protection API.
///     2.: - API ensures that data can be encrypted and safely stored or transmitted.
///     3.: - The keys are typically stored in C:\Users\<UserName>\AppData\Local\ASP.NET\DataProtection-Keys
/// </summary>
if (!Directory.Exists(keyLocation))
{
    Directory.CreateDirectory(keyLocation);
}
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keyLocation));

builder.Services.AddSingleton<UrlEncryptionService>(); // Register your custom service
var configuration = builder.Configuration;

builder.Services.AddScoped<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
// Register ErrorLogServiceRepository as a scoped service
builder.Services.AddScoped<ErrorLogServiceRepository>(); 
// Add services to the container.
builder.Services.ServiceRegister();
//builder.Services.AddMemoryCache(); //Modified by Priyanshi 06-02-25
// Register IHttpContextAccessor
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<MenuAuthorizationFilter>(); // Register the filter
//builder.Services.AddScoped<ICachingServiceRepository, CachingServiceRepository>(); //Modified by Priyanshi 06-02-25
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
 {
     options.RequireHttpsMetadata = false;
     options.SaveToken = true;
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuerSigningKey = true,
         ValidateIssuer = false,
         ValidateAudience = false,
         ClockSkew = TimeSpan.Zero, // Prevent time delay for token expiration
         ValidateLifetime = true, // Ensure token hasn't expired
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
     };
 });

// Role-based authorization
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("user"));
//});

//builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
    options.Cookie.HttpOnly = true; // Secure the cookie
    options.Cookie.IsEssential = true; // Make the session cookie essential
});
//User Web Services Details
// Register HttpClient for Data Migration Service (7256)
builder.Services.AddHttpClient<RestApiDataMigrationServiceHelper>(client =>
{
    //client.BaseAddress = new Uri("http://192.168.7.213:7256/api/");
    client.BaseAddress = new Uri("https://localhost:7256/api/");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

//User Web Services Details
// Register Named HttpClient with Timeout & SSL Validation
builder.Services.AddHttpClient<RestApiTransactionServiceHelper>(client =>
{
    // client.BaseAddress = new Uri("http://192.168.7.213:7230/api/");
    client.BaseAddress = new Uri("https://localhost:7230/api/");
   
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});


builder.Services.AddHttpClient<RestApiUserServiceHelper>(client =>
{
   // client.BaseAddress = new Uri("http://192.168.7.213:7037/api/");
    client.BaseAddress = new Uri("https://localhost:7037/api/");
    client.Timeout = TimeSpan.FromMinutes(12); // ✅ Set timeout here
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});
// Register HttpClient for Master Service (7074)
builder.Services.AddHttpClient<RestApiMasterServiceHelper>(client =>
{
   // client.BaseAddress = new Uri("http://192.168.7.213:7074/api/");
    client.BaseAddress = new Uri("https://localhost:7074/api/");
    client.Timeout = TimeSpan.FromMinutes(12);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

builder.Services.AddHttpClient<NotificationService>();
builder.Services.AddSignalR();
//builder.Services.AddHostedService<PayrollPollingService>(); // ✅ Polling
//builder.Services.AddSingleton<NotificationHub>();
//builder.Services.AddSingleton<SubscribeTableDependency>();
builder.Services.AddTransient<FtpService>();
builder.Services.AddSingleton<ICachingServiceRepository,CachingServiceRepository>(); // or services.AddScoped<CommonHelper>();


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient(); // Register HttpClient
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseXContentTypeOptions();
app.UseXfo(options => options.Deny());
app.UseCsp(options => options.FrameAncestors(directive => directive.Self()));
app.UseXDownloadOptions();
app.UseXfo(options => options.SameOrigin());

app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "-1";
    await next();
});


app.UseMiddleware<EncryptedUrlMiddleware>();
app.UseRouting();


app.UseSession();

var connectionString = app.Configuration.GetConnectionString("DefaultConnection");

app.UseMiddleware<GlobalExceptionWebMiddleware>();
app.UseAuthentication();
app.UseCors(MyAllowSpecificOrigins);
app.MapHub<NotificationHub>("/notificationHub");
//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=LoginPage}/{id?}");

app.Lifetime.ApplicationStopping.Register(() =>
{
    SqlDependency.Stop(connectionString);
});
app.Run();