using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class TaxSlabMaster : BaseModel
    {
        public int YearlyItTableDetail_Id { get; set; }
        public int YearlyItTable_Id { get; set; }   
        public int Company_Id { get; set; }   
        public string SlabName { get; set; }   
        public decimal Income_From { get; set; }   
        public decimal Income_To { get; set; }   
        public int TaxPaybleInPercentage { get; set; }   
        public decimal TaxPaybleInAmount { get; set; }   
    }
}
