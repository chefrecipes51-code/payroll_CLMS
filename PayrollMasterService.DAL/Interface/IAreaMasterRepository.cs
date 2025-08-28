using PayrollMasterService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Interface
{
    public interface IAreaMasterRepository : IGenericRepository<AreaMaster>
    {
        Task<IEnumerable<AreaMaster>> GetAllByIdAsync(string procedureName, object parameters);
        Task<IEnumerable<AreaMaster>> GetAllAreaByLocationIdAsync(string procedureName, object parameters);
    }
}
