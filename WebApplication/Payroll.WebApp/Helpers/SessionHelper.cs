using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace Payroll.WebApp.Helpers
{
    public static class SessionHelper
    {
        public static void SetSessionObject<T>(HttpContext context, string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            context.Session.SetString(key, json);
        }

        public class SessionExpiredException : Exception
        {
            public SessionExpiredException(string message) : base(message) { }
        }

        public static T GetSessionObject<T>(HttpContext context, string key) where T : class
        {
            var json = context.Session.GetString(key);

            //Payroll-500 chirag gurjar 4-mar-2025 Handle custom exceptation to redirect login page.
            if (string.IsNullOrEmpty(json) && key == "UserSessionData")
            {
                throw new SessionExpiredException("Session has expired.");
            }

            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<T>(json);
        }


        public static void ClearSession(HttpContext context)
        {
            context.Session.Clear();
        }
    }

}
