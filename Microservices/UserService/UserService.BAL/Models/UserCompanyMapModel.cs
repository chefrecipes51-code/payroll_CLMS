using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class UserCompanyMapModel : BaseModel
    {
        public int UserMapCompany_Id { get; set; } 
        public int User_Id { get; set; }            
        public int Company_Id { get; set; }         
        public DateTime? Effective_From_Dt { get; set; }       
    }
}
