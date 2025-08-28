using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.BAL.Models
{
    public class UpdateUserLocationStatusModel : BaseModel
    {
        public int User_Id { get; set; }
        public int Correspondance_ID { get; set; }
        public bool ActualActivestatus { get; set; }
    }
}
