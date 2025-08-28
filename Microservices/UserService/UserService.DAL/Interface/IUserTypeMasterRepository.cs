using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.BAL.Models;

namespace UserService.DAL.Interface
{
    public interface IUserTypeMasterRepository
    {
        Task<IEnumerable<UserTypeMaster>> GetAllAsync(string procedureName);
    }
}
