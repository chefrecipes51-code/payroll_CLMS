using Dapper;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.DAL.Interface
{
    public interface ICompanyMasterRepository : IGenericRepository<CompanyMaster>
    {
        Task AddCompanyAsync(string procedureName, object companyMaster, string entityType);
        Task UpdateCompanyAsync(string procedureName, object companyMaster, string entityType);
        Task DeleteCompanyAsync(string procedureName, object companyMaster, string entityType);
        Task<IEnumerable<CompanyCorrespondance>> GetAllCompanyCorrespondanceAsync(string procedureName);
        Task<CompanyCorrespondance> GetCompanyCorrespondanceByIdAsync(string procedureName, object parameters);
        Task<IEnumerable<CompanyStatutory>> GetAllCompanyStatutoryAsync(string procedureName);
        Task<CompanyStatutory> GetCompanyStatutoryByIdAsync(string procedureName, object parameters);

        #region Added By Harshida 23-01-25
        Task<IEnumerable<CompanyCorrespondance>> GetAllCompanyCorrespondancesByCompanyIdAsync(string procedureName, object parameters);
        Task<IEnumerable<CompanyStatutory>> GetAllCompanyStatutoriesByCompanyIdAsync(string procedureName, object parameters);
        Task UpdateCompanyCorrespondanceAsync(string procedureName, object companyMaster, string entityType);
        Task AddCompanyCorrespondanceAsync(string procedureName, object companyCorrespondanceMaster, string entityType);
        Task<IEnumerable<CompanyTypeRequest>> GetAllCompanyTypeAsync(string procedureName);
        Task<IEnumerable<CurrencyRequest>> GetAllCurrencyAsync(string procedureName, DynamicParameters parameters);
        Task AddCompanyConfigurationAsync(string procedureName, object companyCorrespondanceMaster);
        #endregion
        #region added by Krunali 05-02-2025 payroll - 431
        Task<IEnumerable<SubsidiaryMaster>> GetAllSubsidiaryMaster(string procedureName);

        Task<SubsidiaryMaster> GetAllSubsidiaryMasterById(string procedureName, object parameters);
        Task<SubsidiaryMaster> AddSubsidiaryMasterAsync(string procedureName, SubsidiaryMaster subsidiaryMaster);

        Task<SubsidiaryMaster> UpdateSubsidiaryMasterAsync(string procedureName, SubsidiaryMaster subsidiaryMaster);

        Task<SubsidiaryMaster> DeleteSubsidiaryMasterAsync(string procedureName, SubsidiaryMaster subsidiaryMaster);

        #endregion
    }
}
