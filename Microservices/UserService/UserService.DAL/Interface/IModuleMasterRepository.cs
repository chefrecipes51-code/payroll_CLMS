using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BAL.Models;

namespace UserService.DAL.Interface
{
    public interface IModuleMasterRepository 
    {
        Task<IEnumerable<ModuleMaster>> GetAllAsync(string procedureName);
    }
}
