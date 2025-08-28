using Payroll.Common.Repository.Interface;
using PayrollTransactionService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IPayGradeMasterRepository : IGenericRepository<PayGradeMaster>
    {
        Task<IEnumerable<PayGradeMaster>> GetAllActiveAsync(string procedureName, object parameters);
        Task<IEnumerable<PayGradeConfigMaster>> GetAllPayGradeConfigAsync(string procedureName,object parameters);
        Task<PayGradeConfigMaster> GetPayGradeConfigByIdAsync(string procedureName, object parameters);
        Task<PayGradeConfigMaster> AddPayGradeConfigAsync(string procedureName, PayGradeConfigMaster model);
        Task<PayGradeConfigMaster> UpdatePayGradeConfigAsync(string procedureName, PayGradeConfigMaster model);
        Task<PayGradeConfigMaster> DeletePayGradeConfigAsync(string procedureName, object parameters);
        Task<IEnumerable<TradeMaster>> GetAllTradeAsync(string procedureName, object parameters);
        Task<IEnumerable<SkillCategory>> GetAllSkillCategoryAsync(string procedureName, object parameters);
        Task<IEnumerable<DistinctLocation>> GetAllDistinctLocationAsync(string procedureName, object parameters);

    }
}
