using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IPayComponentMasterRepository: IGenericRepository<PayComponentMaster>
    {
        Task<IEnumerable<PayComponentMaster>> GetAllByIdAsync(string procedureName, object parameters);
        Task<PayComponentActivationRequest> UpdatePayComponentStatusAsync(string procedureName, PayComponentActivationRequest earningDeductionMaster);

    }
}
