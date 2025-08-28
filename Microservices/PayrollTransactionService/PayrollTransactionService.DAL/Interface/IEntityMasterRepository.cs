using PayrollTransactionService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.DAL.Interface
{
    public interface IEntityMasterRepository 
    {
        Task<IEnumerable<ContractorMaster>> GetAllContratcorAsync(string procedureName, object parameters);
        Task<IEnumerable<WorkOrderMaster>> GetAllWorkorderAsync(string procedureName, object parameters);
        Task<IEnumerable<ContractorWorkOrderRequest>> GetContractorWorkorderAsync(string procedureName, object parameters);
        Task<IEnumerable<EntityMaster>> GetAllEntityAsync(string procedureName, object parameters);
        Task<MapEntityGradeMaster> AddAsync(string procedureName, MapEntityGradeMaster mapGradeEntity);
        Task<EntityCompliance> EntityComplianceAsync(string procedureName, EntityCompliance entityCompliance);
        Task<IEnumerable<EntityDataValidation>> GetAllEntityDataValidationAsync(string procedureName, object parameters);

    }
}
