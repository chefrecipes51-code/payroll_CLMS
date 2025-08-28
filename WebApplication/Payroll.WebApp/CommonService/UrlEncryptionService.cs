using Microsoft.AspNetCore.DataProtection;

namespace Payroll.WebApp.CommonService
{
    /// <summary>
    /// Developer Name :- Harshida Parmar
    /// Created Date   :- 16-Sep-2024
    /// Message detail :- UrlEncryptionService class
    /// </summary>
    public class UrlEncryptionService
    {
        #region Constructor
        /// <summary>
        /// Developer Name :- Harshida Parmar
        /// Created Date   :- 16-Sep-2024
        /// Message detail :- IDataProtector         
        /// It is scoped to a particular purpose.
        /// If two parts of your app have access to the same keys, they won't be able to decrypt each
        /// other’s data unless they use the same purpose.
        /// </summary>

        private readonly IDataProtector _protector;

        public UrlEncryptionService(IDataProtectionProvider dataProtectionProvider)
        {
            //  CreateProtector("UrlEncryptionService") 
            //      Purpose string : - Ensuring that data is only decrypted by the
            //                         same part of the application that encrypted it.
            _protector = dataProtectionProvider.CreateProtector("UrlEncryptionService");
        }
        #endregion
        #region Protect URL
        #region URL Encryption
        /// <summary>
        /// Developer Name :- Harshida Parmar
        /// Created Date   :- 16-Sep-2024
        /// Message detail :- Encrypt     
        /// Encrypts the specified controller, action, and route parameters into a secure URL format.
        /// </summary>
        /// <param name="controller">Name of the Controller</param>
        /// <param name="action">The action method name to be invoked.</param>
        /// <param name="parameters">A dictionary of route parameters to include in the URL (e.g., employee ID).</param>
        /// <returns>A cryptographically secure, encrypted string representing the URL with 
        /// the specified controller, action, and parameters.
        /// Example
        /// https://localhost:7093/CfDJ8FtQCxe4Vt5PkAmgr4OVGc04lesy-LUDL_4DF7OxceACLD9v22GpK_TAaVcwzUVrqCnDeneeZuQdbSVMskn6pF50e6apbnxGpiypF6aO-lGBgh26EyZb5duAgwwHV8yxgif3nQqT-VDumVgSldN-I4A
        /// </returns>

        public string Encrypt(string controller, string action, IDictionary<string, object> parameters)
        {
            var baseUrl = $"/{controller}/{action}";

            // Convert parameters to a query string
            //var queryString = new QueryString(
            //    string.Join("&", parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value?.ToString() ?? string.Empty)}"))
            //);
            var queryString = new QueryString(
                            "?" + string.Join("&", parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value?.ToString() ?? string.Empty)}"))
                            );

            var fullUrl = baseUrl + queryString;

            return _protector.Protect(fullUrl);
        }
        #endregion
        #region URL Decryption
        /// <summary>
        /// Developer Name :- Harshida Parmar
        /// Created Date   :- 16-Sep-2024
        /// Message detail :- Decrypts the given encrypted URL string back into its original readable form.
        /// </summary>
        /// <param name="encryptedUrl">The encrypted URL string to be decrypted.</param>
        /// <returns>The decrypted URL string with the original controller, action, and route parameters.
        /// </returns>
        public string Decrypt(string encryptedUrl)
        {
            return _protector.Unprotect(encryptedUrl);
        }
        #endregion
        #endregion
    }
}
