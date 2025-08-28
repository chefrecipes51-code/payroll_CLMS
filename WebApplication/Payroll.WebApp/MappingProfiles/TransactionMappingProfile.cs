using AutoMapper;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollTransactionService.BAL.Models;
using PayrollTransactionService.BAL.ReportModel;
using UserService.BAL.Models;

namespace Payroll.WebApp.MappingProfiles
{
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<PayComponentMaster, PayComponentDTO>();
            CreateMap<PayComponentDTO, PayComponentMaster>();
            CreateMap<PayGradeMaster, PayGradeMasterDTO>();
            CreateMap<PayGradeMasterDTO, PayGradeMaster>();
            CreateMap<PayGradeConfigMaster, PayGradeConfigDTO>();
            CreateMap<PayGradeConfigDTO, PayGradeConfigMaster>();
            CreateMap<TaxSlabMasterDTO, TaxSlabMaster>();
            CreateMap<TaxSlabMaster, TaxSlabMasterDTO>();

            CreateMap<MapEntityTaxRegime, MapEntityTaxRegimeDTO>();
            CreateMap<MapEntityTaxRegimeDTO, MapEntityTaxRegime>();
            CreateMap<EntityTaxRegime, EntityTaxRegimeDTO>();
            CreateMap<EntityTaxRegimeDTO, EntityTaxRegime>();

            CreateMap<EntityMaster, EntityMasterDTO>();
            CreateMap<EntityMasterDTO, EntityMaster>();

            CreateMap<MapEntityGradeMaster, MapEntityGradeMasterDTO>();
            CreateMap<MapEntityGradeMasterDTO, MapEntityGradeMaster>();  
            
            CreateMap<MapEntityGrade, MapEntityGradeDTO>();
            CreateMap<MapEntityGradeDTO, MapEntityGrade>();

            CreateMap<PayComponentActivationRequest, PayComponentActivationDTO>();
            CreateMap<PayComponentActivationDTO, PayComponentActivationRequest>();

            CreateMap<PtaxSlabRequest, PtaxSlabDTO>();
            CreateMap<PtaxSlabDTO, PtaxSlabRequest>();

            CreateMap<TaxParamRequest, TaxParamDTO>();
            CreateMap<TaxParamDTO, TaxParamRequest>();

            CreateMap<PayrollGlobalParamRequest, PayrollGlobalParamDTO>();
            CreateMap<PayrollGlobalParamDTO, PayrollGlobalParamRequest>();

            CreateMap<PayrollComplianceRequest, PayrollComplianceDTO>();
            CreateMap<PayrollComplianceDTO, PayrollComplianceRequest>();

            CreateMap<PayrollSettingRequest, PayrollSettingDTO>();
            CreateMap<PayrollSettingDTO, PayrollSettingRequest>();

            CreateMap<ThirdPartyParameterRequest, ThirdPartyParameterDTO>();
            CreateMap<ThirdPartyParameterDTO, ThirdPartyParameterRequest>();

            CreateMap<EntityTypeRequest, EntityTypeDTO>();
            CreateMap<EntityTypeDTO, EntityTypeRequest>(); 
            
            CreateMap<EntityCompliance, EntityComplianceDTO>();
            CreateMap<EntityComplianceDTO, EntityCompliance>();

            CreateMap<CopySettingsRequest, CopySettingsDTO>();
            CreateMap<CopySettingsDTO, CopySettingsRequest>();

            CreateMap<ContractorDetails, ContractorDetailsDTO>();
            CreateMap<ContractorDetailsDTO, ContractorDetails>();

            CreateMap<Contractorprofile, ContractorProfileDTO>();
            CreateMap<ContractorProfileDTO, Contractorprofile>();

            CreateMap<PayrollTransDataForProcessDTO, PayrollTransDataForProcess>();
            CreateMap<PayrollTransDataForProcess,PayrollTransDataForProcessDTO>();

            CreateMap<StartPayrollProcessDTO, StartPayrollProcess>();
            CreateMap<StartPayrollProcess, StartPayrollProcessDTO>();

        }
    }
}
