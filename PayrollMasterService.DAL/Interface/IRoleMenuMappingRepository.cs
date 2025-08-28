using PayrollMasterService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Interface
{
    public interface IRoleMenuMappingRepository : IGenericRepository<RoleMenuMappingRequest>
    {
        //Task<RoleMenuMappingRequest> AddNewAsync(string storedProcedure, RoleMenuMappingRequest roleMenuMapping);
        Task<IEnumerable<UserRoleMenu>> GetRoleMenuByIdAsync(string procedureName, object parameters);
    }
}
