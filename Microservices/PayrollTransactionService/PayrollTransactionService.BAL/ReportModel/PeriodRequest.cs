using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class PeriodRequest : BaseModel
    { 
        public int Period_Id { get; set; } 
        public string? Period_Code { get; set; }
        public string? Period_Name { get; set; }
        public string? Company_Id { get; set; }
        public int Month_Id { get; set; }
        public int Period_Type { get; set; }
        public DateTime PeriodFrom_Date { get; set; }
        public DateTime PeriodTo_Date { get; set; }
        public string? PeriodType { get; set; }         
		public string? FYearDate { get; set; }
        public byte? CustomGroup_Id { get; set; }
        public int Contractor_Id { get; set; }
        public string? CustomGroupName { get; set; }
        public int? Days { get; set; }
        public string Contractor_Code { get; set; }
        public string WorkOrder_Code { get; set; }

    }

    public class PayrollRequest
    {
        public int? CompanyId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? SelectedDate { get; set; }
        public string WorkOrder_Code { get; set; }
        public string WorkOrder_No { get; set; }
        public string Contractor_Code { get; set; }
    }
    public class PeriodResponseWrapper
    {
        public List<PeriodRequest> Periods { get; set; }

        public PeriodResponseWrapper()
        {
            Periods = new List<PeriodRequest>();
        }
    }
}
