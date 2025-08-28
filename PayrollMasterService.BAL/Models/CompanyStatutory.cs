using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class CompanyStatutory : BaseModel
    {
        public int Statutory_Type_Id { get; set; }
        public byte Company_Id { get; set; }
        public string StatutoryType_Name { get; set; }
    }
}
