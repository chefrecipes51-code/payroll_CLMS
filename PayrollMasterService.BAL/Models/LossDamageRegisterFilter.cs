using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class LossDamageRegisterFilter
    {
        public int CompanyID { get; set; }
        public string? CompanyLocationIDs { get; set; }
        public string? ContractorIDs { get; set; }
        public string? LocationIDs { get; set; }
        public string? EntityIDs { get; set; }
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime FinancialYearStart { get; set; }
    }
    public class LossDamageRegisterReport
    {
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
        public string Father_Name { get; set; }
        public string Skillcategory_Name { get; set; }
        public string Type { get; set; }
        public DateTime Damage_Date { get; set; }
        public string Name_Of_WitNess { get; set; }
        public decimal Total_Amount { get; set; }
        public int No_Of_Installment { get; set; }
        public string RecoveryDate { get; set; }
        public string Remarks { get; set; }
        public decimal EMI_Amount { get; set; }
    }
}
