/****************************************************************************************************
 *                                                                                                  *
 *  Description:                                                                                    *
 *  This middleware handles global exception logging and error response generation for HTTP         *
 *  requests in the application. It ensures that unhandled exceptions are logged appropriately and  *
 *  meaningful error messages are returned to the client.                                           *
 *                                                                                                  *
 *  Key Features:                                                                                   *
 *  - Captures all unhandled exceptions during request processing.                                  *
 *  - Logs detailed error information, including request path, to a centralized logger.             *
 *  - Supports logging errors to a database using repository patterns (IErrorLogRepository).        *
 *  - Returns structured error responses in JSON format with appropriate HTTP status codes:         *
 *      - 401 for UnauthorizedAccessException                                                       *
 *      - 400 for ValidationException                                                               *
 *      - 500 for other unhandled exceptions                                                        *
 *  - Provides customizable error messages for client debugging or user-friendly messages.          *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - InvokeAsync         : Processes the HTTP request and handles exceptions.                      *
 *  - HandleExceptionAsync: Logs the exception and returns a structured JSON error response.        *
 *  - LogErrorToDatabase  : Logs the exception details to a database via IErrorLogRepository.        *
 *                                                                                                  *
 *  Author: Priyanshu Jain                                                                             *
 *  Date  : 08-Sep-2024                                                                                 *
 *                                                                                                  *
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

namespace Payroll.Common.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IServiceProvider _serviceProvider;
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IServiceProvider serviceProvider)
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
                _ => StatusCodes.Status500InternalServerError
            };

            var errorDetails = new ErrorDetails
            {
                StatusCode = context.Response.StatusCode,
                Message = exception is UnauthorizedAccessException ? "Unauthorized Access" :
                          exception is ValidationException ? "Validation Error Occurred" :
                          "Internal Server Error. Please try again later.",
                Detail = exception.Message // Optionally include exception details for debugging (can be omitted in production)
            };
            var errorjson = JsonSerializer.Serialize(errorDetails?.ToString());
            // Return the structured error response as JSON
            await context.Response.WriteAsync(errorjson);
        }
        // this method can be used to log the error to the database
        private async Task LogErrorToDatabase(HttpContext context, Exception exception, IErrorLogRepository services)
        {
            await services.ErrorLogAsync(exception, context);
        }
    }
}
