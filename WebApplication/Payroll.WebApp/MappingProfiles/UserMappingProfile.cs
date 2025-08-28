using AutoMapper;
using DataMigrationService.BAL.Models;
using Payroll.Common.CommonDto;
using Payroll.Common.CommonRequest;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;
using PayrollMasterService.BAL.Requests;
using UserService.BAL.Models;
using UserService.BAL.Requests;
using UserMapLocation = PayrollMasterService.BAL.Models.UserMapLocation;

namespace Payroll.WebApp.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<LoginDTO, LoginRequest>();
            CreateMap<LoginRequest, LoginDTO>();

            CreateMap<SendEmailDTO, SendEmailModel>();
            CreateMap<SendEmailModel, SendEmailDTO>();

            CreateMap<UserDTO, UserRequest>();
            CreateMap<UserRequest, UserDTO>();

            CreateMap<UserMapDetailModel, UserMapDetailsDTO>().ReverseMap();
            CreateMap<LocationDetailModel, LocationDetailDTO>().ReverseMap();

            // If `UserRoles` is a collection inside UserMapDetailModel, map it explicitly
            CreateMap<UserRole, UserRoleDTO>().ReverseMap();
            CreateMap<UpdateUserRoleStatusModel, UpdateUserRoleStatusDTO>().ReverseMap();
            CreateMap<UpdateUserLocationStatusModel, UpdateUserLocationStatusDTO>().ReverseMap();
            CreateMap<MapUserLocation, MapUserLocationDTO>().ReverseMap();
            CreateMap<UserMapLocation, UserMapLocationDTO>().ReverseMap();

            #region Added By Priyanshi 02-01-25
            CreateMap<CompanyMasterDTO, CompanyMaster>();
            CreateMap<CompanyMaster, CompanyMasterDTO>();
            
            CreateMap<CompanyCorrespondanceDTO, PayrollMasterService.BAL.Models.CompanyCorrespondance>();
            CreateMap<PayrollMasterService.BAL.Models.CompanyCorrespondance, CompanyCorrespondanceDTO>();

            CreateMap<CompanyStatutoryDTO, CompanyStatutory>();
            CreateMap<CompanyStatutory, CompanyStatutoryDTO>();

            CreateMap<EntityTaxStatutoryDTO, EntityTaxStatutory>();
            CreateMap<EntityTaxStatutory, EntityTaxStatutoryDTO>();

            CreateMap<AreaDTO, AreaMaster>();
            CreateMap<AreaMaster, AreaDTO>(); 
            
            CreateMap<LocationMasterDTO, LocationMaster>();
            CreateMap<LocationMaster, LocationMasterDTO>();

            CreateMap<DepartmentDTO, DepartmentMaster>();
            CreateMap<DepartmentMaster, DepartmentDTO>();
            #endregion

            #region Added By Harshida 30-12-'24
            CreateMap<UserInfoDTO, UserRequest>();
            CreateMap<UserRequest, UserInfoDTO>();
            CreateMap<UserCompanyDetailsDTO, UserCompanyDetails>();
            CreateMap<UserCompanyDetails, UserCompanyDetailsDTO>();
            CreateMap<UserLocationDetailsDTO, UserLocationDetails>();
            CreateMap<UserLocationDetails, UserLocationDetailsDTO>();
            CreateMap<UserRoleDetailsDTO, UserRoleDetails>();
            CreateMap<UserRoleDetails, UserRoleDetailsDTO>();
            #endregion

            #region Added By Harshida 30-12-'24
            CreateMap<UserInfoDTO, UserRequest>();
            CreateMap<UserRequest, UserInfoDTO>();

            CreateMap<UserCompanyDetailsDTO, UserCompanyDetails>();
            CreateMap<UserCompanyDetails, UserCompanyDetailsDTO>();

            CreateMap<UserLocationDetailsDTO, UserLocationDetails>();
            CreateMap<UserLocationDetails, UserLocationDetailsDTO>();

            CreateMap<UserRoleDetailsDTO, UserRoleDetails>();
            CreateMap<UserRoleDetails, UserRoleDetailsDTO>();

            CreateMap<RoleOrLocationDTO, RoleOrLocationRequest>();
            CreateMap<RoleOrLocationRequest, RoleOrLocationDTO>();

            CreateMap<UserRoleMenuDTO, UserRoleMenu>();
            CreateMap<UserRoleMenu, UserRoleMenuDTO>();

            CreateMap<CompanyTypeDTO, CompanyTypeRequest>(); //Added By Harshida 07-02-25
            CreateMap<CompanyTypeRequest, CompanyTypeDTO>();//Added By Harshida 07-02-25
            #endregion

            CreateMap<UserChangepwdDTO, UserRequest>();
            CreateMap<UserRequest, UserChangepwdDTO>();

            CreateMap<SubsidiaryMasterDTO, SubsidiaryMaster>(); //Added By Krunali
            CreateMap<SubsidiaryMaster, SubsidiaryMasterDTO>(); //Added By Krunali

            CreateMap<DeactivateUserDTO, DeactivateUser>();
            CreateMap<DeactivateUser, DeactivateUserDTO>();

            CreateMap<FormulaMasterDTO, PayrollMasterService.BAL.Models.FormulaMaster>();
        }
    }
}
