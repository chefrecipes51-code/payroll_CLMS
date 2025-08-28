namespace Payroll.Common.APIKeyManagement.Interface
{
    public interface IApiKeyService
    {
        Task<bool> ValidateApiKeyAsync(string apiKey);
        Task<string> GenerateApiKeyAsync(string userId, TimeSpan validityPeriod, int maxUsage);
        Task<bool> ValidateUserCredentialsAsync(string userId, string Password);
        Task MarkApiKeyAsValidatedAsync(string apiKey);
        Task MarkApiKeyAsConsumedAsync(string apiKey);
    }
}
