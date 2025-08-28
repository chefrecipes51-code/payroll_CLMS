using Payroll.Common.APIKeyManagement.Requests;

namespace Payroll.Common.APIKeyManagement.Interface
{
    public interface ISSOLoginService
    {
        Task<CLMSLoginResponse> ValidateCLMSUserAsync(CLMSLoginRequest request);
    }
}
