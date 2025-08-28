using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class PayGradeMaster : BaseModel
    {
        public int PayGrade_Id { get; set; }
        public int? Cmp_Id { get; set; }
        public string PayGradeCode { get; set; }
        public string PayGradeName { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public string PayGradeDesc { get; set; }
    } 
    
}
