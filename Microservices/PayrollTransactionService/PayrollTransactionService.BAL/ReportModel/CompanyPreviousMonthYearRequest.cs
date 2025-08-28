using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class CompanyPreviousMonthYearRequest
    {
        public int Month_Id { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; } = string.Empty;
    }
    public class CompanyIdRequest
    {
        public int CompanyId { get; set; }
    }
}
