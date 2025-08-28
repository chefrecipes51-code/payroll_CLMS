using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class YearlyItTableMaster : BaseModel
    {
        public int YearlyItTable_Id { get; set; }
        public byte Company_Id { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Remarks { get; set;}
        public bool NewRegime {  get; set; }
        public bool OldRegime { get; set; }
        public int Currency_Id { get; set; }
    }
}
