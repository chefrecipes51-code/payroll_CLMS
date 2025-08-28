using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Payroll.WebApp.Models.DTOs;
using PayrollTransactionService.BAL.Models;
using static Payroll.WebApp.SignalRHubs.NotificationService;

namespace Payroll.WebApp.SignalRHubs
{
    public class NotificationHub : Hub
    {
        private readonly string _connectionString;
        private readonly NotificationService _notificationService;
        public NotificationHub(IConfiguration configuration, NotificationService notificationService)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _notificationService = notificationService;

        }
        public async Task SendNotification([FromBody] PayrollProcessusingSignalRRequest payrollProcessusingSignalRRequest)
        {
            var products = _notificationService.GetActiveNotificationsAsync(payrollProcessusingSignalRRequest);
            await Clients.All.SendAsync("ReceiveNotification", "New notification available");


        }
        public async Task SendNotiFicationCount([FromBody] PayrollProcessusingSignalRRequest payrollProcessusingSignalRRequest)
        {
            PayrollProcessusingSignalRDTO Notifications = await _notificationService.GetActiveNotificationsAsync(payrollProcessusingSignalRRequest);
            await Clients.All.SendAsync("ReceiveNotificationCount1", Notifications);
        }
        public async Task SendNotificationCount1()
        {
            // You can call _repository or service here if needed
            await Clients.All.SendAsync("ReceiveNotificationCount", new { count = 5 }); // Example
        }
    }
}
