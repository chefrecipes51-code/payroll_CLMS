using PayrollTransactionService.BAL.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IPTaxSlabRepository : IGenericRepository<PtaxSlabRequest>
    {
        Task<IEnumerable<TaxParamRequest>> GetTaxParamAsync(string procedureName, int taxType, int? stateId);
        Task<IEnumerable<ProfessionalTaxSlabViewModel>> GetAllAsyncWithId(string procedureName, object parameters);
    }
}
