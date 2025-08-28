using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class UserTypeMaster : BaseModel
    {
        public int UserType_Id { get; set; }
        public string UserTypeName { get; set; }
    }
}
