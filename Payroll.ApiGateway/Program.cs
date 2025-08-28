using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Payroll.Common;
using Payroll.Common.Middlewares;
using Payroll.Common.Repository.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load configuration files including ocelot.json
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// JWT settings from appsettings.json

// Configure Authentication with JWT Bearer
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });
builder.Services.ServiceRegister();

// Add Authorization services (make sure roles are checked)

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);
var app = builder.Build();

// Add Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseMiddleware<RoleMiddlware>();
app.UseAuthorization();   // Authorization middleware

// Enable Ocelot middleware
await app.UseOcelot();

app.Run();
