using PayrollMasterService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Interface
{
    public interface IMapDepartmentLocationRepository : IGenericRepository<MapDepartmentLocation>
    {
        Task<IEnumerable<FloorMaster>> GetAllFloorAsync(string procedureName);
        Task<IEnumerable<FloorMaster>> GetFloorByIdAsync(string procedureName, object parameters);
    }
}
