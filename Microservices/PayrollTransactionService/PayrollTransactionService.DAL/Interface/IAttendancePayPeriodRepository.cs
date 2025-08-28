///
/// Developer Name:- Abhishek Yadav
/// Date:- 26-05-25
/// Note:- "GetAttendancePayPeriodsByCompanyIdAsync" method pass the Company ID and based on that Fetch PeriodRequest Data
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
    public interface IAttendancePayPeriodRepository //WHY DID NOT USED IGenericRepository ??? Read Above Note
    {
        Task<IEnumerable<PeriodRequest>> GetAllAsync(string procedureName);
        Task<IEnumerable<PeriodRequest>> GetAttendancePayPeriodsByCompanyIdAsync(string procedureName, object parameters);
        Task<PeriodRequest> UpdateAsync(string procedureName, PeriodRequest model);
        Task<PeriodRequest> AddAsync(string procedureName, PeriodRequest model);
        Task<PeriodRequest> DeleteAsync(string procedureName, object parameters);
        // Task<IEnumerable<PeriodRequest>> GetPayPeriodsByCompanyIdAndSDateAsync(string procedureName, object parameters);
        Task<(IEnumerable<PeriodRequest> Periods, string ErrorMessage)> GetAttendancePayPeriodsByCompanyIdAndSDateAsync(string procedureName, object parameters);
    }
}
