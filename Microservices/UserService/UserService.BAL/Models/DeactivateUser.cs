using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class DeactivateUser : BaseModel
    {
        public int Country_Id {  get; set; }
        public int Company_Id { get; set;}
        public int UserId { get; set;}
        public int Deactivation_Type { get; set;}
        public string Deactivation_Reason { get; set;}
        public int Approve_Reject_Level { get; set;}
        public DateTime Effective_On { get; set;}
    }   
}
