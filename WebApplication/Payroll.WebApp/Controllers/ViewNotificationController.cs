using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Payroll.WebApp.Models.DTOs;
using Payroll.WebApp.SignalRHubs;

namespace Payroll.WebApp.Controllers
{
    public class ViewNotificationController : Controller
    {
       

        private readonly NotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ViewNotificationController(NotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        //[NonAction]
        public async Task<IActionResult> Index([FromBody] PayrollProcessusingSignalRRequest payrollProcessusingSignalRRequest)
        {
            var notificationResult = await _notificationService.GetActiveNotificationsAsync(payrollProcessusingSignalRRequest);

            ViewBag.Total = notificationResult?.Total ?? 0;
            ViewBag.Completed = notificationResult?.Completed ?? 0;
            ViewBag.Remaining = notificationResult?.Remaining ?? 0;
            ViewBag.Status = notificationResult?.Status ?? 0;

            return View();
        }
        public async Task<IActionResult> TriggerSignalRUpdate([FromBody] PayrollProcessusingSignalRRequest payrollProcessusingSignalRRequest)
        {
            var data = await _notificationService.GetActiveNotificationsAsync(payrollProcessusingSignalRRequest);

            // Push to SignalR clients
            await _hubContext.Clients.All.SendAsync("ReceiveNotificationCount1", data);

            // Return as JSON for polling check
            return Json(data);
        }

    }
}
