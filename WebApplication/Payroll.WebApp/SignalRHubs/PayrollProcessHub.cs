using Microsoft.AspNetCore.SignalR;
using Payroll.WebApp.Models.DTOs;
using PayrollTransactionService.BAL.Models;

namespace Payroll.WebApp.SignalRHubs
{
    public class PayrollProcessHub : Hub
    {
        public async Task SendProgress(string connectionId, PayrollProgressUpdate progress)
        {
            await Clients.Client(connectionId).SendAsync("ReceivePayrollProcessData", progress);
        }

        public async Task SendError(string connectionId, string error)
        {
            await Clients.Client(connectionId).SendAsync("ReceivePayrollProcessError", new { error });
        }
       

    }
}
