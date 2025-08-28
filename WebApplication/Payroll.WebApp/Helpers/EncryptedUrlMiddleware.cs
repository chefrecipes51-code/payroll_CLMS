using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace Payroll.WebApp.Helpers
{
    /// <summary>
    /// Developer Name :- Harshida Parmar
    /// Created Date   :- 16-Sep-2024
    /// Message detail :- Middleware to handle encrypted URLs by decrypting and routing to the correct controller and action.
    /// </summary>
    public class EncryptedUrlMiddleware
    {
        #region Constructor
        private readonly RequestDelegate _next;
        private readonly IDataProtector _protector;
        private readonly ILogger<EncryptedUrlMiddleware> _logger;

        /// <summary>  
        /// Developer Name :- Harshida Parmar
        /// Created Date   :- 16-Sep-2024
        /// Initializes a new instance of the <see cref="EncryptedUrlMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the HTTP request pipeline.</param>
        /// <param name="dataProtectionProvider">The data protection provider used for URL encryption and decryption.</param>
        /// <param name="logger">Logger for logging information during middleware execution.</param>

        public EncryptedUrlMiddleware(RequestDelegate next, IDataProtectionProvider dataProtectionProvider, ILogger<EncryptedUrlMiddleware> logger)
        {
            _next = next;
            _protector = dataProtectionProvider.CreateProtector("UrlEncryptionService");
            _logger = logger;
        }
        #endregion
        #region Middleware Logic
        /// <summary>
        /// Developer Name :- Harshida Parmar
        /// Created Date   :- 16-Sep-2024
        /// Invoked during the request pipeline to check for encrypted URLs, decrypt them, and reroute the request.
        /// </summary>
        /// <param name="context">The current HTTP request context.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value.Trim('/');

            if (!string.IsNullOrEmpty(path) && path.Contains('-'))
            {
                try
                {
                    _logger.LogInformation($"Encrypted path received: {path}");

                    // Decrypt the path
                    var decryptedPath = _protector.Unprotect(path);
                    _logger.LogInformation($"Decrypted path: {decryptedPath}");

                    // Create a new URI from the decrypted path  7093
                    var decryptedUri = new Uri($"https://localhost:7093{decryptedPath}", UriKind.Absolute);
                    //var decryptedUri = new Uri($"https://localhost:7110{decryptedPath}", UriKind.Absolute);

                    // Extract route values from the decrypted URI
                    var routeValues = new Dictionary<string, object>();
                    var query = QueryHelpers.ParseQuery(decryptedUri.Query);

                    // Extract controller and action from the path
                    var segments = decryptedUri.AbsolutePath.Trim('/').Split('/');
                    if (segments.Length < 2)
                    {
                        _logger.LogWarning("Invalid URL format. Path does not contain controller and action.");
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                        await context.Response.WriteAsync("Invalid URL format.");
                        return;
                    }

                    var controller = segments[0];
                    var action = segments[1];

                    routeValues["controller"] = controller;
                    routeValues["action"] = action;

                    // Add query parameters to route values
                    foreach (var queryParam in query)
                    {
                        routeValues[queryParam.Key] = queryParam.Value.ToString();
                    }

                    _logger.LogInformation($"Redirecting to controller: {controller}, action: {action}, with parameters: {string.Join(", ", routeValues.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");

                    // Set the request path and query string
                    context.Request.Path = new PathString($"/{controller}/{action}");
                    context.Request.QueryString = QueryString.Create(query);

                    // Call the next middleware in the pipeline
                    await _next(context);
                    return;
                }
                catch (Exception ex)
                {
                    // Handle decryption or URI errors
                    _logger.LogError($"Invalid URL: {ex.Message}");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync($"Invalid URL: {ex.Message}");
                    return;
                }
            }

            // Call the next middleware in the pipeline if no encryption in path
            await _next(context);
        }
        #endregion
    }
}
