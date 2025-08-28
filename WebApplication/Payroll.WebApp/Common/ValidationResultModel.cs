using Microsoft.AspNetCore.Mvc.Rendering;

namespace Payroll.WebApp.Common
{
    /// <summary>
    /// Created By:- Harshida Parmar
    /// Created Date:- 27-11-'24
    /// Note:- Set of rules to upload bulk record. 
    /// </summary>
    public class ValidationResultModel
    {
        public int RowIndex { get; set; }
        public List<string> ColumnValues { get; set; } = new List<string>(); // Initialize to avoid null reference
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public bool HasError { get; set; }
        public List<string> ColumnHeaders { get; set; } = new List<string>(); // Initialize headers
      
    }
    public class PayrollValidationViewModel
    {
        public List<SelectListItem> ServiceDropdown { get; set; } = new List<SelectListItem>();
        public int templateId { get; set; }  // Property to hold the selected Service ID

    }

}
