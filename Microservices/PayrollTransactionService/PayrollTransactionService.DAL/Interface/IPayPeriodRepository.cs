///
/// Developer Name:- Harshida Parmar
/// Date:- 08-04-25
/// Note:- "GetPayPeriodsByCompanyIdAsync" method pass the Company ID and based on that Fetch PeriodRequest Data
/// So IGenericRepository some common method which I have implement it no matter used or not so that is why did
/// not used the IGenericRepository  here.
///
using PayrollTransactionService.BAL.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    //public interface IPayPeriodRepository : IGenericRepository<PeriodRequest>
    public interface IPayPeriodRepository //WHY DID NOT USED IGenericRepository ??? Read Above Note
    {
        Task<IEnumerable<PeriodRequest>> GetAllAsync(string procedureName);
        Task<IEnumerable<PeriodRequest>> GetPayPeriodsByCompanyIdAsync(string procedureName, object parameters);
        Task<PeriodRequest> UpdateAsync(string procedureName, PeriodRequest model);
        Task<PeriodRequest> AddAsync(string procedureName, PeriodRequest model);
        Task<PeriodRequest> DeleteAsync(string procedureName, object parameters);
        // Task<IEnumerable<PeriodRequest>> GetPayPeriodsByCompanyIdAndSDateAsync(string procedureName, object parameters);
        Task<(IEnumerable<PeriodRequest> Periods, string ErrorMessage)> GetPayPeriodsByCompanyIdAndSDateAsync(string procedureName, object parameters);
    }
}
