using PayrollTransactionService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface ITaxSlabMasterRepository : IGenericRepository<TaxSlabMaster>
    {
        public Task<IEnumerable<TaxRegimeMaster>> GetAllTaxRegimeAsync(string procedureName);
       Task<IEnumerable<TaxSlabMaster>> GetAllTaxRegimeByCompanyAsync(string procedureName,object parameter);
    }
}
