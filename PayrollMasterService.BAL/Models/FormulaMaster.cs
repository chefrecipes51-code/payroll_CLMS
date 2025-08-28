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
    public class FormulaMaster : BaseModel
    {       
        public int Formula_Id { get; set; }
        public string FormulaName { get; set; }
        public string Formula_Computation { get; set; }
        public int? Cmp_Id { get; set; }
        public string Formula_Code { get; set; }
    }
}