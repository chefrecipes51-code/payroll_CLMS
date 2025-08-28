using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class ContractorValidationRequest
    {
        public int Contractor_Id { get; set; }
        public string ContractorCode { get; set; }
        public string BusinessAreaCode { get; set; }
        public string CompanyCode { get; set; }
        public string ContractorName { get; set; }
        public string ContactNo { get; set; }
        public string Email_Id { get; set; }
        public string Address { get; set; }
        public string LicenseNo { get; set; }
        public int? MaxLabourCount { get; set; }
        public int? Assigned_labour_Count { get; set; }
        public int? ContractorMapping { get; set; }
        public string LIN_No { get; set; }
        public string PANNo { get; set; }
        public string TANNo { get; set; }
        public string EPF_No { get; set; }
        public string ESIC_No { get; set; }
        public bool? Is_SubContractor { get; set; }
        public string Parent_Contractor_Code { get; set; }
        public bool? IsActive { get; set; }
        public string WorkOrderNo { get; set; }
        public decimal? WorkOrderValue { get; set; }
        public string? HasWorkOrderValue { get; set; }
        public string? SubContractorName { get; set; }

        // Missing field indicators
        public bool IsMissing_BusinessAreaCode { get; set; }
        public bool IsMissing_CompanyCode { get; set; }
        public bool IsMissing_ContactNo { get; set; }
        public bool IsMissing_LicenseNo { get; set; }
        public bool IsMissing_MaxLabourCount { get; set; }
        public bool IsMissing_PANNo { get; set; }
        public bool IsMissing_TANNo { get; set; }
        public bool IsMissing_WorkOrderNo { get; set; }
        public bool IsMissing_WorkOrderValue { get; set; }
    }
    public class ContractorValidationRequestModel
    {
        public byte CompanyId { get; set; }
        public List<int>? LocationIds { get; set; } = new();
        public List<int>? WorkOrderIds { get; set; } = new();
        public int Month_Id { get; set; }
        public int Year { get; set; }
    }

}
