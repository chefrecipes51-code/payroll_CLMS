/****************************************************************************************************
 *  Description:                                                                                    *
 *  This controller use to retrive different reports using stored procedure                        *
 *                                                                                                  *
 *  Author: Chirag Gurjar                                                                          *
 *  Date  : 08-July-2025                                                                            *
 *                                                                                                  *
 ****************************************************************************************************/

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.APIKeyManagement.Common;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using payrollmasterservice.BAL.Models;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.DAL.Interface;
using System.Net;

namespace PayrollMasterService.API.Controllers
{
    [ApiController]
    [Route("api/")]
    public class ReportsApiController : ControllerBase
    {
        private readonly IReportRepository _repository;
        private readonly ApiKeyValidatorHelper _apiKeyValidatorHelper;
        public ReportsApiController(IReportRepository repository, ApiKeyValidatorHelper apiKeyValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiKeyValidatorHelper = apiKeyValidator;
        }


        /// <summary>
        /// Developer Name: Chirag Gurjar
        /// Message Detail: API to retrieve report pay register
        /// Created Date: 08-July-2025             
        [HttpPost("getpayregisterreport")]
        public async Task<IActionResult> GetPayRegisterReport([FromBody] PayRegisterFilter payRegisterFilter)
        {
            ApiResponseModel<IEnumerable<PayRegisterReport>> apiResponse = new ApiResponseModel<IEnumerable<PayRegisterReport>>();
            //var isValid = _apiKeyValidatorHelper.Validate(apiKey);
            //if (!isValid)
            //{
            //    apiResponse.IsSuccess = false;
            //    apiResponse.Message = "Invalid API Key.";
            //    apiResponse.StatusCode = ApiResponseStatusConstant.Unauthorized;
            //    return Unauthorized(apiResponse);
            //}
            var objPayRegisterReport = await _repository.GetPayRegisterReportAsync(DbConstants.GetPayRegisterReport,
                    new
                    {
                        CompanyID = payRegisterFilter.CompanyID,
                        CompanyLocationIDs = payRegisterFilter.CompanyLocationIDs,
                        ContractorIDs = payRegisterFilter.ContractorIDs,
                        EntityIDs = payRegisterFilter.EntityIDs,
                        PayrollMonth = payRegisterFilter.PayrollMonth,
                        PayrollYear = payRegisterFilter.PayrollYear,
                        ProcessedDate = payRegisterFilter.ProcessedDate,
                        FinancialYearStart = payRegisterFilter.FinancialYearStart

                    });
            if (objPayRegisterReport == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = objPayRegisterReport;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        /// <summary>
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve report fine register 
        /// Created Date: 09-July-2025             
        [HttpPost("getfineregisterreport")]
        public async Task<IActionResult> GetFineRegisterReport([FromBody] FineRegisterFilter fineRegisterReport)
        {
            ApiResponseModel<IEnumerable<FineRegisterReport>> apiResponse = new ApiResponseModel<IEnumerable<FineRegisterReport>>();

            var objFineRegisterReport = await _repository.GetFineRegisterReportAsync(DbConstants.GetFineRegisterReport,
                    new
                    {
                        CompanyID = fineRegisterReport.CompanyID,
                        CompanyLocationIDs = fineRegisterReport.CompanyLocationIDs,
                        ContractorIDs = fineRegisterReport.ContractorIDs,
                        EntityIDs = fineRegisterReport.EntityIDs,
                        PayrollMonth = fineRegisterReport.PayrollMonth,
                        PayrollYear = fineRegisterReport.PayrollYear,
                        ProcessedDate = fineRegisterReport.ProcessedDate,
                        FinancialYearStart = fineRegisterReport.FinancialYearStart

                    });
            if (objFineRegisterReport == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = objFineRegisterReport;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        [HttpPost("getcompliancereport")]
        public async Task<IActionResult> GetComplianceReport([FromBody] ComplianceFilter complianceReport)
        {
            ApiResponseModel<IEnumerable<ComplianceReport>> apiResponse = new ApiResponseModel<IEnumerable<ComplianceReport>>();

            var reportData = await _repository.GetComplianceReportAsync(DbConstants.GetComplianceReport, new
            {
                CompanyID = complianceReport.CompanyID,
                CompanyLocationIDs = complianceReport.CompanyLocationIDs,
                ContractorIDs = complianceReport.ContractorIDs,
                EntityIDs = complianceReport.EntityIDs,
                PayrollMonth = complianceReport.PayrollMonth,
                PayrollYear = complianceReport.PayrollYear,

            });

            if (reportData == null || !reportData.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = reportData;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        #region LOSS AND DAMAGE
        /// <summary>
        /// Developer Name: Harshida PArmar
        /// Message Detail: API to retrieve loss/damage register report
        /// Created Date: 10-July-2025
        /// </summary>
        [HttpPost("getlossdamageregisterreport")]
        public async Task<IActionResult> GetLossDamageRegisterReport([FromBody] LossDamageRegisterFilter filter)
        {
            ApiResponseModel<IEnumerable<LossDamageRegisterReport>> apiResponse = new ApiResponseModel<IEnumerable<LossDamageRegisterReport>>();

            var reportData = await _repository.GetLossDamageRegisterReportAsync(DbConstants.GetLossDamageReport, new
            {
                CompanyID = filter.CompanyID,
                CompanyLocationIDs = filter.CompanyLocationIDs,
                ContractorIDs = filter.ContractorIDs,
                LocationIDs = filter.LocationIDs,
                EntityIDs = filter.EntityIDs,
                PayrollMonth = filter.PayrollMonth,
                PayrollYear = filter.PayrollYear,
                ProcessedDate = filter.ProcessedDate,
                FinancialYearStart = filter.FinancialYearStart
            });
            if (reportData == null || !reportData.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = reportData;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }
        #endregion

        #region TDS
        /// <summary>
        /// Developer: Harshida Parmar
        /// Purpose: API to retrieve tax deduction report
        /// Created Date: 11-July-2025
        /// </summary>
        [HttpPost("gettaxdeductionreport")]
        public async Task<IActionResult> GetTaxDeductionReport([FromBody] TaxDeductionReportFilter filter)
        {
            ApiResponseModel<IEnumerable<TaxDeductionReport>> apiResponse = new ApiResponseModel<IEnumerable<TaxDeductionReport>>();

            var reportData = await _repository.GetTaxDeductionReportAsync(DbConstants.GetTDSReport, new
            {
                CompanyID = filter.CompanyID,
                CompanyLocationIDs = filter.CompanyLocationIDs,
                ContractorIDs = filter.ContractorIDs,
                EntityIDs = filter.EntityIDs,
                PayrollMonth = filter.PayrollMonth,
                PayrollYear = filter.PayrollYear,
                ProcessedDate = filter.ProcessedDate,
                FinancialYearStart = filter.FinancialYearStart
            });

            if (reportData == null || !reportData.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = reportData;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        #endregion

        #region Contractor Payment
        /// <summary>
        /// Developer: Harshida Parmar
        /// Purpose: API to retrieve contractor payment register report
        /// Created Date: 11-July-2025
        /// </summary>
        [HttpPost("getcontractorpaymentregister")]
        public async Task<IActionResult> GetContractorPaymentRegister([FromBody] ContractorPaymentRegisterFilter filter)
        {
            ApiResponseModel<IEnumerable<ContractorPaymentRegisterReport>> apiResponse = new ApiResponseModel<IEnumerable<ContractorPaymentRegisterReport>>();

            var reportData = await _repository.GetContractorPaymentRegisterDataAsync(DbConstants.GetContractorPaymentRegister, new
            {
                CompanyID = filter.CompanyID,
                CompanyLocationIDs = filter.CompanyLocationIDs,
                ContractorIDs = filter.ContractorIDs,
                EntityIDs = filter.EntityIDs,
                PayrollMonth = filter.PayrollMonth,
                PayrollYear = filter.PayrollYear,
                ProcessedDate = filter.ProcessedDate,
                FinancialYearStart = filter.FinancialYearStart
            });

            if (reportData == null || !reportData.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = reportData;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        #endregion

        /// <summary>
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve report Salary Slip Data 
        /// Created Date: 10-July-2025             
        [HttpPost("getsalaryslipreport")]
        public async Task<IActionResult> GetSalarySlipReport([FromBody] SalarySlipFilter salarySlipFilter)
        {
            ApiResponseModel<IEnumerable<SalarySlipReport>> apiResponse = new ApiResponseModel<IEnumerable<SalarySlipReport>>();

            var objSalarySlipReport = await _repository.GetSalarySlipReportAsync(DbConstants.GetSalarySlipReport,
                    new
                    {
                        CompanyID = salarySlipFilter.CompanyID,
                        CompanyLocationIDs = salarySlipFilter.CompanyLocationIDs,
                        ContractorIDs = salarySlipFilter.ContractorIDs,
                        EntityIDs = salarySlipFilter.EntityIDs,
                        PayrollMonth = salarySlipFilter.PayrollMonth,
                        PayrollYear = salarySlipFilter.PayrollYear,
                    });
            if (objSalarySlipReport == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = objSalarySlipReport;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve report Overtime Data 
        /// Created Date: 10-July-2025             
        [HttpPost("getovertimereport")]
        public async Task<IActionResult> GetOvertimeReport([FromBody] OvertimeFilter overtimeFilter)
        {
            ApiResponseModel<IEnumerable<OvertimeReport>> apiResponse = new ApiResponseModel<IEnumerable<OvertimeReport>>();

            var objSalarySlipReport = await _repository.GetOvertimeReportAsync(DbConstants.GetOvertimeReport,
                    new
                    {
                        CompanyID = overtimeFilter.CompanyID,
                        CompanyLocationIDs = overtimeFilter.CompanyLocationIDs,
                        ContractorIDs = overtimeFilter.ContractorIDs,
                        EntityIDs = overtimeFilter.EntityIDs,
                        PayrollMonth = overtimeFilter.PayrollMonth,
                        PayrollYear = overtimeFilter.PayrollYear,
                        ProcessedDate = overtimeFilter.ProcessedDate,
                        FinancialYearStart = overtimeFilter.FinancialYearStart
                    });
            if (objSalarySlipReport == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = objSalarySlipReport;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

        /// <summary>
        /// Developer Name: Priyanshi Jain
        /// Message Detail: API to retrieve report Loan & Advance Data 
        /// Created Date: 11-July-2025             
        [HttpPost("getloanandadvancereport")]
        public async Task<IActionResult> GetLoanandAdvanceReport([FromBody] LoanandAdvanceFilter loanandAdvanceFilter)
        {
            ApiResponseModel<IEnumerable<LoanandAdvanceReport>> apiResponse = new ApiResponseModel<IEnumerable<LoanandAdvanceReport>>();

            var objLoanandAdvanceReport = await _repository.GetLoanandAdvanceReportAsync(DbConstants.GetLoanandAdvanceReport,
                    new
                    {
                        CompanyID = loanandAdvanceFilter.CompanyID,
                        CompanyLocationIDs = loanandAdvanceFilter.CompanyLocationIDs,
                        ContractorIDs = loanandAdvanceFilter.ContractorIDs,
                        EntityIDs = loanandAdvanceFilter.EntityIDs,
                        PayrollMonth = loanandAdvanceFilter.PayrollMonth,
                        PayrollYear = loanandAdvanceFilter.PayrollYear,
                        ProcessedDate = loanandAdvanceFilter.ProcessedDate,
                        FinancialYearStart = loanandAdvanceFilter.FinancialYearStart
                    });
            if (objLoanandAdvanceReport == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = ApiResponseMessageConstant.RecordNotFound;
                apiResponse.StatusCode = ApiResponseStatusConstant.NotFound;
                return NotFound(apiResponse);
            }

            apiResponse.IsSuccess = true;
            apiResponse.Result = objLoanandAdvanceReport;
            apiResponse.Message = ApiResponseMessageConstant.GetRecord;
            apiResponse.StatusCode = ApiResponseStatusConstant.Ok;
            return Ok(apiResponse);
        }

    }
}
   
