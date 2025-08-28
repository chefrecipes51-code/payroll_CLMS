using PayrollMasterService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Interface
{
    public interface IEarningDeductionMasterRepository : IGenericRepository<EarningDeductionMaster>
    {
        Task<IEnumerable<EarningDeductionMaster>> GetAllByIdAsync(string procedureName, object parameters);
    }
}
