using Payroll.WebApp.Middleware;

namespace Payroll.WebApp.Middleware
{
    public class TokenSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Add("Authorization", "Bearer " + token);
            }
            await _next(context);
        }
    }
}
