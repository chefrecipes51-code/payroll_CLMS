using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;

namespace PayrollMasterService.DAL.Interface
{
    public interface IApprovalSetUpRepository : IGenericRepository<ApprovalConfigGrid> 
    {
        Task<ApprovalSetUp> AddNewAsync(string storedProcedure, ApprovalSetUp approvalSetUp);
        Task<ApprovalConfigCommon> AddApprovalConfigAsync(string storedProcedure, ApprovalConfigCommon approvalConfigCommon);
        Task<IEnumerable<ApprovalSetUpFilter>> GetByFilterAttributesAsync(string procedureName, object parameters);
        Task<(ApprovalConfig, IEnumerable<ApprovalLevel>, IEnumerable<ApprovalDetail>)> GetApprovalConfigDetailsAsync(string procedureName, int configId);
        Task<IEnumerable<ApprovalConfigGrid>> GetApprovalConfigGridAsync(string procedureName, int? companyId, int? locationId, int? serviceId);
        //Task<IEnumerable<ApprovalDetailRequest>> GetListAsync(string procedureName, object parameters); //29-05-25
        Task<ApprovalListViewModel> GetApprovalWithSummaryAsync(string procedureName, object parameters);//02-06-25
        Task<ApprovalDetailRequest> UpdateApprovalAsync(string procedureName, ApprovalDetailRequest model); //29-05-25

    }
}