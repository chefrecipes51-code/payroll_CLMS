using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class EntityCompliance : BaseModel
    {
        public int Entity_ID { get; set; }
        public string Pf_No { get; set; }
        public string Uan_No { get; set; }
        public string EsicNo { get; set; }
        public int Pf_Applicable { get; set; }  
        public int Vpf_Applicable { get; set; }   
        public decimal Vpf_Value { get; set; }   
        public decimal Vpf_percent { get; set; }   
        public int Pt_Applicable { get; set; }   
        public int Pt_State_ID { get; set; }   
        public int Lwf_Applicable { get; set; }   
        public DateTime? Esi_Exit_Date { get; set; }   
        public int Pay_Grade_Id { get; set; }   
        public string Policy_No { get; set; }   
        public decimal Pf_Percent { get; set; }   
        public decimal Pf_Amount { get; set; }   
        public int GratuityApplicable { get; set; }   
        public decimal PolicyAmt { get; set; }   
    }
}
