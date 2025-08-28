using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.ApplicationModel
{
    public class BaseModel
    {
        public bool IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } // value manage from database side.
        public int? UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; } // value manage from database side.
        public bool IsDeleted { get; set; }
        public string StatusMessage { get; set; }
        public int MessageType { get; set; }
        public string ApplicationMessage { get; set; }
       

    }
}
