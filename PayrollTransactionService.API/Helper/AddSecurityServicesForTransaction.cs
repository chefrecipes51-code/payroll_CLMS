using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.APIKeyManagement.Interface;
using Payroll.Common.APIKeyManagement.Service;

namespace PayrollTransactionService.API.Helper
{
    public static class AddSecurityServicesForTransaction
    {
        public static IServiceCollection SecurityServicesForTransaction(this IServiceCollection services)
        {
            // Register security-related services
            services.AddScoped<IApiKeyService, ApiKeyService>();
            services.AddScoped<ApiKeyValidatorHelper>();
            services.AddScoped<ISSOLoginService, SSOLoginService>();
            return services;
        }
    }
}
