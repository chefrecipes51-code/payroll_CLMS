using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class Notification
    {
        public long Notification_Id { get; set; }        
        public string NotificationTitle { get; set; }  
        public string NotificationDesc { get; set; }     
        public int? UserId { get; set; }                 
        public int? RoleId { get; set; }                  
        public byte CompanyId { get; set; }              
        public bool? IsRead { get; set; }                
        public bool? IsCommon { get; set; }              
        public bool IsActive { get; set; }                
        public int CreatedBy { get; set; }                
        public DateTime CreatedDate { get; set; }       
        public int? UpdatedBy { get; set; }              
        public DateTime? UpdatedDate { get; set; }      
        public bool IsDeleted { get; set; }               
    }
    public class PayrollProcessStatus
    {
        public int Total { get; set; }
        public int Remaining { get; set; }
        public int Completed { get; set; }
        public int Status { get; set; }
    }
    public class NotificationResult
    {
        public int Count { get; set; }
        public IEnumerable<PayrollProcessStatus> Notifications { get; set; }
    }

}
