using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class UpdateUserRoleStatusModel: BaseModel
    {
        public int User_Id { get; set; }
        public int Role_User_Id { get; set;}    
        public bool ActualActivestatus { get; set;}

    }
}
