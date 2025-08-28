using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PayrollTransactionService.BAL.Models;

namespace Payroll.WebApp.SignalRHubs
{
    public class PayrollPollingService : BackgroundService
    {
        private readonly NotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        private PayrollProcessStatus _lastStatus;

        public PayrollPollingService(NotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
            _lastStatus = new PayrollProcessStatus();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //var current = await _notificationService.GetActiveNotificationsAsync();

                    //if (current != null && HasChanged(current, _lastStatus))
                    //{
                    //    _lastStatus = current;

                    //    // Push update to all clients
                    //    await _hubContext.Clients.All.SendAsync("ReceiveNotificationCount1", current);

                    //    Console.WriteLine("🔥 SignalR Broadcast Sent: " + JsonConvert.SerializeObject(current));
                    //}
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Polling failed: {ex.Message}");
                }

                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken); // Poll every 3 seconds
            }
        }

        private bool HasChanged(PayrollProcessStatus current, PayrollProcessStatus previous)
        {
            return current.Total != previous.Total ||
                   current.Completed != previous.Completed ||
                   current.Remaining != previous.Remaining ||
                   current.Status != previous.Status;
        }
    }
}
