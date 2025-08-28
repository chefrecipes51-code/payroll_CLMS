using Dapper;
using Payroll.Common.APIKeyManagement.Interface;
using System.Collections.Concurrent;
using System.Data;
using System.Security.Cryptography;

namespace Payroll.Common.APIKeyManagement.Service
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly ConcurrentDictionary<string, ApiKey> _apiKeys = new();

        private readonly IDbConnection _dbConnection;
        public ApiKeyService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<bool> ValidateApiKeyAsync(string apiKey)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ApiKey", apiKey, DbType.String);
                parameters.Add("@RETURN_VALUE", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                // Execute stored procedure
                await _dbConnection.ExecuteAsync(
                    "SP_ValidateDeltaApiKey",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // Fetch the return value
                var returnValue = parameters.Get<int>("@RETURN_VALUE");

                // Return true if return value is 1
                return returnValue == 1;
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating API key.", ex);
            }
        }
        public async Task<bool> ValidateUserCredentialsAsync(string userId, string password)
        {
            try
            {
                // Prepare parameters for the stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId, DbType.String);
                parameters.Add("@Password", password, DbType.String);

                // Define the return value parameter
                parameters.Add("@RETURN_VALUE", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

                // Execute stored procedure
                await _dbConnection.ExecuteAsync(
                    "SP_ValidateDeltaAPIUser",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // Fetch the return value
                var returnValue = parameters.Get<int>("@RETURN_VALUE");

                // Return true if return value is 1
                return returnValue == 1;
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                throw new Exception("Error validating user credentials.", ex);
            }
        }


        public async Task<string> GenerateApiKeyAsync(string userId, TimeSpan validityPeriod, int maxUsage)
        {
            try
            {
                // Generate a unique API key
                var apiKeyValue = GenerateApiKey();
                var createdDateTime = DateTime.UtcNow;
                var expiryDateTime = createdDateTime.Add(validityPeriod);

                // Save the API key details to the database
                var parameters = new DynamicParameters();
                parameters.Add("@ApiKey", apiKeyValue, DbType.String);
                parameters.Add("@UserId", userId, DbType.String);
                parameters.Add("@CreatedDateTime", createdDateTime, DbType.DateTime);
                parameters.Add("@ExpiryDateTime", expiryDateTime, DbType.DateTime);

                await _dbConnection.ExecuteAsync(
                    "SP_SaveDeltaApiKey", // This is the stored procedure created earlier
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // Cache the API key details in memory
                var apiKey = new ApiKey
                {
                    ApiKeyValue = apiKeyValue,
                    UserId = userId,
                    ExpiryDate = expiryDateTime,
                    UsageCount = 0,
                    MaxUsageLimit = maxUsage
                };

                _apiKeys[apiKeyValue] = apiKey;

                return apiKeyValue; // Return the generated API key
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating and saving API key.", ex);
            }
        }

        public async Task MarkApiKeyAsValidatedAsync(string apiKey)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ApiKey", apiKey, DbType.String);
                parameters.Add("@UpdatedDateTime", DateTime.UtcNow, DbType.DateTime);

                await _dbConnection.ExecuteAsync(
                    "SP_UpdateApiKeyValidationStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating API key validation status.", ex);
            }
        }

        public async Task MarkApiKeyAsConsumedAsync(string apiKey)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ApiKey", apiKey, DbType.String);
                parameters.Add("@UpdatedDateTime", DateTime.UtcNow, DbType.DateTime);

                await _dbConnection.ExecuteAsync(
                    "SP_UpdateApiKeyConsumeStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating API key validation status.", ex);
            }
        }

        private ApiKey GetKey(string Api)
        {
            return _apiKeys.TryGetValue(Api, out var apiKeyModel) ? apiKeyModel : null;

        }
        private static string GenerateApiKey()
        {
            using var hmac = new HMACSHA256();
            return Convert.ToBase64String(hmac.Key);
        }

    }

    public class ApiKey
    {
        public string ApiKeyValue { get; set; }
        public string UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UsageCount { get; set; }
        public int MaxUsageLimit { get; set; }
    }
}
