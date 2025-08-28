using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class ApiRquestForm
    {
        public string userId { get; set; }
        public string Password { get; set; }
        public int validityMinutes { get; set; } = 20;
        public int maxUsage { get; set; } = 10;
    }
}
