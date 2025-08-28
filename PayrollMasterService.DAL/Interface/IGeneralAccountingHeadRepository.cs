using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Interface
{
    public interface IGeneralAccountingHeadRepository : IGenericRepository<AccountingHeadRequest>
    {
        Task<IEnumerable<AccountType>> GetAllAccountTypesAsync(string procedureName, object parameters);

    }
}
