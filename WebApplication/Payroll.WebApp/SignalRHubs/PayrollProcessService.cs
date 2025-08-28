using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollTransactionService.BAL.Models;

namespace Payroll.WebApp.SignalRHubs
{
    public class PayrollProcessService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiTransactionServiceHelper _transactionServiceHelper;

        public PayrollProcessService(HttpClient httpClient, IOptions<ApiSettings> apiSettings, RestApiTransactionServiceHelper transactionServiceHelper)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettings.Value;
            _transactionServiceHelper = transactionServiceHelper;
        }
        public async Task<IEnumerable<PayrollProcessusingSignalRDTO>> GetPayrollProcessDataAsync(int cmpId, int monthId, int yearId)
        {
            try
            {
                var apiUrl = $"{_apiSettings.PayrollTransactionEndpoints.GetAllPayrollProcessusingSignalRDetail}?cmp_Id={cmpId}&month_Id={monthId}&year_Id={yearId}";

                var response = await _transactionServiceHelper.GetCommonAsync<List<PayrollProcessusingSignalRDTO>>(apiUrl);

                var responseList = response?.Result;

                return responseList ?? Enumerable.Empty<PayrollProcessusingSignalRDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service Exception: " + ex.Message);
                return Enumerable.Empty<PayrollProcessusingSignalRDTO>();
            }
        }



    }
}
