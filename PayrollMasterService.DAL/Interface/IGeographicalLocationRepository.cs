using PayrollMasterService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Interface
{
    public interface IGeographicalLocationRepository
    {
        Task<IEnumerable<StateMaster>> GetAllStateAsync(string procedureName, object parameters);
        Task<IEnumerable<CityMaster>> GetAllCityAsync(string procedureName, object parameters);
        Task<IEnumerable<LocationMaster>> GetAllCityWiseLocationAsync(string procedureName, object parameters);
    }
}
