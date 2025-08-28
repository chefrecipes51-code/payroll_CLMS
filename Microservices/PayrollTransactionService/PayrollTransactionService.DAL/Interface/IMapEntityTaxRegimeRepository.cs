using PayrollTransactionService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IMapEntityTaxRegimeRepository : IGenericRepository<MapEntityTaxRegime>
    {
        Task<EntityFilterResponse> GetAllEntityFiltersAsync(string procedureName, EntityFilterRequest request);
        Task<IEnumerable<FinancialYearMaster>> GetAllFinancialYearAsync(string procedureName, object parameters);
    }
}
