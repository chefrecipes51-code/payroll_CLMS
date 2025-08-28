using Microsoft.AspNetCore.Mvc.Rendering;
using EntityService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityService.DAL.Interface
{
    public interface IEntityMasterTranRepository
    {
        Task<EntityMasterAssignWage> AssignWageToEntityAsync(string procedureName, EntityMasterAssignWage model);
        List<SelectListItem> GetWageGradesForDropdown();
    }
}
