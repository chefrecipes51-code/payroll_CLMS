using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Payroll.Common.ApplicationModel;
using Payroll.Common.Repository.Interface;
using Payroll.Common.Repository.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.Middlewares
{
    /// <summary>
    ///  Developer Name :- Abhishek Yadav
    ///  Message detail :- Role Class for Role wise Endpoint authoization mapping functionality, which can map Role with API Endpoint. 
    ///  Created Date   :- 12-Oct-2024
    ///  Last Modified  :- 12-Oct-2024
    ///  Modification   :- None
    /// </summary>
    /// <returns></returns>
    public class RoleMiddlware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RoleMiddlware> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RoleMiddlware(RequestDelegate next, ILogger<RoleMiddlware> logger, IServiceProvider serviceProvider)
        {
            _next = next;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            
            // Check if the user is authenticated
            if (context.User.Identity.IsAuthenticated)
            {
                // Get the requested API path
                var requestedUrl = context.Request.Path.Value;

                // Split the URL and get the first segment (base segment)
                var segments = requestedUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                string baseSegment = segments.Length > 0 ? segments[0] : string.Empty;

                // Log the base segment to the console
                Console.WriteLine($"Requested Base API Segment: {baseSegment}");

                // Alternatively, log using the ILogger
                _logger.LogInformation($"Requested Base API Segment: {baseSegment}");

                // Get the HTTP method (GET, POST, PUT, DELETE, PATCH)
                var httpMethod = context.Request.Method;

                // Query the database to check if the role has access to this endpoint and method
                var userRole = context.User.FindFirst("TypeOfRole")?.Value;
               
                var permission = await GetPermissionasync(userRole, baseSegment);
       


                if (permission != null)
                {
                    // Check based on the HTTP method
                    bool hasAccess = httpMethod switch
                    {
                        "GET" => permission.Get,
                        "POST" => permission.Post,
                        "PUT" => permission.Put,
                        "DELETE" => permission.Delete,
                        "PATCH" => permission.Patch,
                        _ => false
                    };

                    if (!hasAccess)
                    {
                        // If no access, return 403 Forbidden
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        await context.Response.WriteAsync("Forbidden: You do not have access to this API.");
                        return;
                    }
                }
                else
                {
                    // If no permission found, return 403 Forbidden
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden: No permission found for your role.");
                    return;
                }
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }

     private async Task<RoleApiPermissionModel> GetPermissionasync(string RoleName, string ApiEndpoint)
        {
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IRolePermmison>();
            return await service.RolePermissionMappingAsync(RoleName,ApiEndpoint);
        }
    }
}
