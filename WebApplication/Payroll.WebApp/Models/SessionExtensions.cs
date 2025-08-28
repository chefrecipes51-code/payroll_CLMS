using Newtonsoft.Json;
using Payroll.Common.CommonDto;

namespace Payroll.WebApp.Models
{
    public static class SessionExtensions
    {
        private const string AuthConfigSessionKey = "AuthConfig";

        // Set object to session
        public static void SetAuthConfig(this ISession session, AuthConfigModel authConfig)
        {
            var jsonData = JsonConvert.SerializeObject(authConfig);
            session.SetString(AuthConfigSessionKey, jsonData);
        }

        // Get object from session
        public static AuthConfigModel GetAuthConfig(this ISession session)
        {
            var jsonData = session.GetString(AuthConfigSessionKey);
            return jsonData == null ? null : JsonConvert.DeserializeObject<AuthConfigModel>(jsonData);
        }
    }
}
