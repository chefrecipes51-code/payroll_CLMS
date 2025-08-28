using Payroll.WebApp.Models;
using AutoMapper;
using PayrollMasterService.BAL.Models;
using UserService.BAL.Models;
using Payroll.WebApp.Models.DTOs;
using UserService.BAL.Requests;

namespace Payroll.WebApp.MappingProfiles
{
    /// <summary>
    /// Prepared By :- Harshida Parmar
    /// Date: - 14-11-'24
    /// Note:- Map your class here.
    /// </summary>
    public class RoleMenuMappingProfile : Profile
    {
        public RoleMenuMappingProfile()
        {
           
            CreateMap<RoleMenuMappingHeaderDTO, RoleMenuMappingHeader>();
            CreateMap<RoleMenuMappingHeader, RoleMenuMappingHeaderDTO>();
         
            CreateMap<RoleMenuDetailDTO, RoleMenuDetail>();
            CreateMap<RoleMenuDetail, RoleMenuDetailDTO>();    
        
            CreateMap<RoleMenuMappingRequestDTO, RoleMenuMappingRequest>();
            CreateMap<RoleMenuMappingRequest, RoleMenuMappingRequestDTO>();

            CreateMap<UserRoleMappingDTO, UserRoleMapping>();
            CreateMap<UserRoleMapping, UserRoleMappingDTO>();

            CreateMap<UserRoleMapDTO, UserRoleMappingRequest>();
            CreateMap<UserRoleMappingRequest, UserRoleMapDTO>();
                      
            #region PayrollMaster Class Mapping
            CreateMap<DepartmentMasterDTO, DepartmentMaster>();
            CreateMap<DepartmentMaster, DepartmentMasterDTO>();
            #endregion

            CreateMap<UserRoleBasedMenuDTO, UserRoleBasedMenuRequest>(); //Added By Harshida 10-01-'25
            CreateMap<UserRoleBasedMenuRequest, UserRoleBasedMenuDTO>(); //Added By Harshida 10-01-'25
        }
    }
}
