using PayrollTransactionService.BAL.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IPayrollTransactionStagingRepository : IGenericRepository<PayrollTranStgDataRequest>
    {
        //Task<T> InsertPayrollStagingDataAsync(string procedureName, PayrollTranStgDataRequest request);
        Task<PayrollStgData> SavePayrollStagingDataAsync(
    string procedureName,
    SavePayrollStagingRequestModel request);

    }
}
