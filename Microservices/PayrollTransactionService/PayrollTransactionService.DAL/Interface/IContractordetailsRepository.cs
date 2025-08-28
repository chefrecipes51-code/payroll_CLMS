using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IContractordetailsRepository 
    {
        Task<IEnumerable<ContractorDetails>> GetAllContractorAsync(string procedureName, object parameters);
        Task<IEnumerable<ContractorDetails>> GetContractorDetailsByCompanyIdAsync(string procedureName, object parameters);
        Task<IEnumerable<Contractorprofile>> GetContractorProfileByCompanyIdAsync(string procedureName, object parameters);
    }
}
