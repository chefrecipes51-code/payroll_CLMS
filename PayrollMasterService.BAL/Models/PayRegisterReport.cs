using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payrollmasterservice.BAL.Models
{
    public class PayRegisterReport : BaseModel
    {
        public string EntityName { get; set; }
        public string EntityCode { get; set; }
        public string SkillName { get; set; }
        public Nullable<int> TotalWorkingDays { get; set; }
        public Nullable<int> PresentDays { get; set; }
        public Nullable<Decimal> BasicWages { get; set; }
        public Nullable<Decimal> DA { get; set; }
        public Nullable<Decimal> TotalOTHours { get; set; }
        public Nullable<Decimal> TotalEarnings { get; set; }
        public Nullable<Decimal> TotalDeductions { get; set; }
        public decimal? Wage_Per_Day { get; set; }
        public decimal? otherearning { get; set; }
        public Nullable<Decimal> NetPay { get; set; }
        //public Nullable<Decimal> Net_Salary { get; set; }


        //public Nullable<Decimal> GrossPay { get; set; }



        //public Nullable<Decimal> TotalEmpContribution { get; set; }
        //public Nullable<Decimal> TotalEmplrContribution { get; set; }


    }

    public class PayRegisterFilter
    {
        public int CompanyID { get; set; }
        public string CompanyLocationIDs { get; set; }
        public string ContractorIDs { get; set; }
        public string EntityIDs { get; set; }
        public int PayrollMonth { get; set; }
        public int PayrollYear { get; set; }
        public Nullable<DateTime> ProcessedDate { get; set; }
        public Nullable<DateTime> @FinancialYearStart { get; set; }

    }

    //public class PayRegisterReportDTO : BaseModel
    //{
    //    public string EntityName { get; set; }
    //    public string EntityCode { get; set; }
    //    public string Designation { get; set; }
    //    public Nullable<int> TotalWorkingDays { get; set; }
    //    public Nullable<int> PresentDays { get; set; }
    //    public Nullable<Decimal> BasicWages { get; set; }
    //    public Nullable<Decimal> DA { get; set; }
    //    public Nullable<Decimal> TotalOTHours { get; set; }
    //    public Nullable<Decimal> TotalEarnings { get; set; }
    //    public Nullable<Decimal> TotalDeductions { get; set; }
    //    public Nullable<Decimal> Net_Salary { get; set; }

    //}
    //public class PayRegisterReportGrid : BaseModel
    //{
    //    public string EntityName { get; set; }
    //    public string EntityCode { get; set; }
    //    public string Designation { get; set; }
    //    public Nullable<int> TotalWorkingDays { get; set; }
    //    public Nullable<int> PresentDays { get; set; }
    //    public Nullable<Decimal> BasicWages { get; set; }
    //    public Nullable<Decimal> DA { get; set; }
    //    public Nullable<Decimal> TotalOTHours { get; set; }
    //    public Nullable<Decimal> TotalEarnings { get; set; }
    //    public Nullable<Decimal> TotalDeductions { get; set; }
    //    public Nullable<Decimal> Net_Salary { get; set; }


    //}
}

