using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class PayrollProcessusingSignalR
    {
        public int Total { get; set; }
        public int Remaining { get; set; }    
        public int Completed { get; set; } 
        public int Status { get; set; }
    }
}
