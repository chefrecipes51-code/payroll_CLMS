using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class WageGradeMaster : BaseModel
    {
        //public int Wage_Id { get; set; }
        //public int Cmp_id { get; set; }
        //public int Tenant_id { get; set; }
        //public string WageGradeCode { get; set; }
        //public string WageGradeName { get; set; }
        //public decimal WageGradeBasic { get; set; }
        //public int PaymentModeId { get; set; }
        //public bool IsHRAapplicable { get; set; }
        //public bool HRAallownceType { get; set; }
        //public bool NotInUse { get; set; }

        //payroll-413 --changes in table format
        public int Wage_Id { get; set; }
        public int? Cmp_id { get; set; }
        public string WageGradeCode { get; set; }
        public string WageGradeName { get; set; }
        public DateTime Effective_StartDate { get; set; }
        public DateTime Effective_EndDate { get; set; }
      

    }
}
