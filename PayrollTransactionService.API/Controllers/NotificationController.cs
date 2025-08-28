using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.DAL.Interface;
using PayrollTransactionService.DAL.Service;

namespace PayrollTransactionService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _repository;

        public NotificationController(INotificationRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [HttpGet("GetActiveNotifications")]
        public async Task<IEnumerable<PayrollProcessusingSignalR>> GetActiveNotifications()
        {
            return await _repository.GetActiveNotificationsAsync();
        }
    }
}
