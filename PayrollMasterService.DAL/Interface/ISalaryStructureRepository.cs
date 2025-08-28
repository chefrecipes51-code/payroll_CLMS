using PayrollMasterService.BAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Interface
{
    public interface ISalaryStructureRepository: IGenericRepository<SalaryStructureDTO>
    {
        Task<IEnumerable<SalaryStructureGrid>> GetAllByIdAsync(string procedureName, object parameters);
        Task<SalaryStructureDTO> AddUpdateSalaryStructure(string procedureName, SalaryStructureDTO objSalaryStructure);
        Task<SalarySimulatorDTO> CalculateSalaryStructure(string procedureName, SalarySimulatorDTO objSalaryStructure);

    }
}
