using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models;
using Payroll.WebApp.Models.DTOs;
using PayrollTransactionService.BAL.Models;

namespace Payroll.WebApp.SignalRHubs
{
    public class NotificationService 
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiTransactionServiceHelper _transactionServiceHelper;

        public NotificationService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, RestApiTransactionServiceHelper transactionServiceHelper)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _transactionServiceHelper = transactionServiceHelper;
        }
        public async Task<PayrollProcessusingSignalRDTO> GetActiveNotificationsAsync([FromBody] PayrollProcessusingSignalRRequest payrollProcessusingSignalRRequest)
        {
            // Use the injected _httpClient (which handles DNS, reuse, etc.)
            //var response = await _httpClient.GetAsync("https://localhost:7230/api/Notification");
            var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayrollProcessusingSignalRDetail}?cmp_Id={payrollProcessusingSignalRRequest.Cmp_Id}&month_Id={payrollProcessusingSignalRRequest.Month_Id}&year_Id={payrollProcessusingSignalRRequest.Year_Id}";
            var response = await _transactionServiceHelper.GetCommonAsync<List<PayrollProcessusingSignalRDTO>>(
            apiUrl
            );
            var responseList = response?.Result.FirstOrDefault();

            return responseList;
        }
        public class NotificationResult
        {
            public int Count { get; set; }
            public IEnumerable<Notification> Notifications { get; set; }
        }
    }
}
