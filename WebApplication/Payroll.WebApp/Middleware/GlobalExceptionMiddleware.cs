/****************************************************************************************************
 *  Date  : 04-mar-2025                                                                             *
 *  Payroll-500 Copied this middleware from common library project to handle                        *
 *              custom session expired exceptation .                                                *
 ****************************************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Payroll.Common.ApplicationModel;
using Payroll.Common.CommonDto;
using Payroll.Common.Repository.Interface;
using Payroll.Common.Repository.Service;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

using Payroll.WebApp.Middleware;
using static Payroll.WebApp.Helpers.SessionHelper;

namespace Payroll.WebApp.Middleware
{
    public class GlobalExceptionWebMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionWebMiddleware> _logger;
        private readonly IServiceProvider _serviceProvider;
        public GlobalExceptionWebMiddleware(RequestDelegate next, ILogger<GlobalExceptionWebMiddleware> logger, IServiceProvider serviceProvider)
        {
            _next = next;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (SessionExpiredException ex)
            {
                _logger.LogError($"Session expired: {ex.Message}");
                await HandleExceptionAsync(httpContext, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            using var scope = _serviceProvider.CreateScope();

            // Example of logging with additional details
            _logger.LogError(exception, "An unhandled exception occurred while processing request from {RequestPath}", context.Request.Path);

            // Log to database if required using repository patterns
            var service = scope.ServiceProvider.GetRequiredService<IErrorLogRepository>();


            // Log the error details to the error log database.
            await LogErrorToDatabase(context, exception, service);

            context.Response.ContentType = "application/json";

            // Determine the appropriate status code based on the exception type
            context.Response.StatusCode = exception switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                ValidationException => StatusCodes.Status400BadRequest,
                SessionExpiredException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            var errorDetails = new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                Message = exception is UnauthorizedAccessException ? "Unauthorized Access" :
                          exception is ValidationException ? "Validation Error Occurred" :
                           exception is SessionExpiredException ? "Session Expired" :
                          "Internal Server Error. Please try again later123.",
                Detail = exception.Message // Optionally include exception details for debugging (can be omitted in production)
            };
            var errorjson = JsonSerializer.Serialize(errorDetails?.ToString());
            // Return the structured error response as JSON

            if (exception is SessionExpiredException)
            {
                context.Response.Clear();
                //context.Response.Redirect("/Account/LoginPage");
                if (context.Request.Cookies.TryGetValue("CLMSLogin", out var clmsLoginValue))
                {
                    if (clmsLoginValue == "1")
                    {
                        context.Response.Redirect("/CLMSLandingPage/Index");
                    }
                    else
                    {
                        context.Response.Redirect("/Account/LoginPage");

                    }
                }
                else
                {

                    var isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

                    if (isAjax)
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var json = JsonSerializer.Serialize(new
                        {
                            success = false,
                            isSessionExpired = true,
                            message = "Session expired. Please login again."
                        });

                        await context.Response.WriteAsync(json);
                    }
                    else
                    {
                        context.Response.Redirect("/Account/LoginPage");
                    }
                    return;

                }
                return;
            }

            await context.Response.WriteAsync(errorjson);
        }

        //public class SessionExpiredException : Exception
        //{
        //    public SessionExpiredException(string message) : base(message) { }
        //}
        // this method can be used to log the error to the database
        private async Task LogErrorToDatabase(HttpContext context, Exception exception, IErrorLogRepository services)
        {
            await services.ErrorLogAsync(exception, context);
        }
    }
}
