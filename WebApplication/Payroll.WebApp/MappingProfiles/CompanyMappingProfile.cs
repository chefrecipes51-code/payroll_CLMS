using AutoMapper;
using Payroll.WebApp.Models.DTOs;
using Payroll.WebApp.Models;
using PayrollMasterService.BAL.Models;
using UserService.BAL.Models;
using UserService.BAL.Requests;
using PayrollMasterService.BAL.Requests;

namespace Payroll.WebApp.MappingProfiles
{
    public class CompanyMappingProfile : Profile
    {
        public CompanyMappingProfile()
        {
            CreateMap<CompanyConfigurationDTO, CompanyConfigurationRequest>(); //Added By Harshida 16-02-'25
            CreateMap<CompanyConfigurationRequest, CompanyConfigurationDTO>(); //Added By Harshida 16-02--'25
        }
    }
}
