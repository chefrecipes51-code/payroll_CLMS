using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class DetailYearlyItTableMaster : BaseModel
    {
        public int YearlyItTableDetail_Id { get; set; }  
        public int YearlyItTable_Id { get; set; }        
        public byte Company_Id { get; set; }             
        public decimal Income_From { get; set; }         
        public decimal Income_To { get; set; }           
        public int TaxPaybleInPercentage { get; set; } 
        public decimal TaxPaybleInAmount { get; set; } 
    }
}
