using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class CompanyLocationMapDto
    {
        public List<CountryDto> Countries { get; set; } = new List<CountryDto>();
        public List<StateDto> States { get; set; } = new List<StateDto>();
        public List<CityDto> Cities { get; set; } = new List<CityDto>();
        public List<LocationDto> Locations { get; set; } = new List<LocationDto>();
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
        public List<AreaLocationDto> AreaLocations { get; set; } = new List<AreaLocationDto>();
        public List<AreaGeographicDto> Areas { get; set; } = new List<AreaGeographicDto>();
    }
    public class CountryDto
    {
        public int Country_Id { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Stdcode { get; set; }
    }
    public class StateDto
    {
        public int CountryId { get; set; }
        public int State_Id { get; set; }
        public string StateName { get; set; }
    }
    public class CityDto
    {
        public int State_Id { get; set; }
        public int City_ID { get; set; }
        public string City_Name { get; set; }
    }
    public class LocationDto
    {
        public int CityId { get; set; }
        public int Correspondance_ID { get; set; }
        public string LocationName { get; set; }
    }
    public class RoleDto
    {
        public int? Role_Menu_Hdr_Id { get; set;}
        public int? Role_Id { get; set; }
        public string RoleName { get; set;}
    }
    public class AreaLocationDto
    {
        public int City_ID { get; set; }
        public int Location_Id { get; set; }
        public string LocationName { get; set; }
    }
    public class AreaGeographicDto
    {
        public int Area_Id { get; set; }
        public int Location_Id { get; set; }
        public string AreaName { get; set; }
    }

}
