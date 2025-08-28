using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.APIKeyManagement.Common
{
    public class ApiKeyValidatorHelper
    {
        private readonly Payroll.Common.APIKeyManagement.Interface.IApiKeyService _apiKeyService;

        public ApiKeyValidatorHelper(Payroll.Common.APIKeyManagement.Interface.IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        public bool Validate(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return false;
            }

            var isValid = _apiKeyService.ValidateApiKeyAsync(apiKey).GetAwaiter().GetResult();
            _apiKeyService.MarkApiKeyAsValidatedAsync(apiKey).GetAwaiter().GetResult();
            _apiKeyService.MarkApiKeyAsConsumedAsync(apiKey).GetAwaiter().GetResult();

            return isValid;
        }
    }
}
