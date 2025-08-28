/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-77                                                                   *
 *  Description:                                                                                    *
 *  This controller handles message publishing functionality using RabbitMQ.                        *
 *  It includes APIs to send messages to RabbitMQ queues for message-driven architecture.           *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - SendMessage: Sends a test message ("Hello RabbitMQ!") to the RabbitMQ queue.                  *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 06-Nov-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/

using Microsoft.AspNetCore.Mvc;
using Payroll.Common.Middleware;

namespace PayrollMasterService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class MessageController : ControllerBase
    {
        private readonly RabbitMQService _rabbitMQService;
        public MessageController(RabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpGet("message")]
        public IActionResult SendMessage()
        {
            string message = "Hello RabbitMQ!";
            _rabbitMQService.SendMessage(message);
            return Ok("Message sent to RabbitMQ");
        }
    }
}
