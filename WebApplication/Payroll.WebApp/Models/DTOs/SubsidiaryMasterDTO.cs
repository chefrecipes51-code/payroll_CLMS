using Payroll.Common.ApplicationModel;
using static Payroll.Common.EnumUtility.EnumUtility;
namespace Payroll.WebApp.Models.DTOs
{
	/// <summary>
	/// Create By:- Krunali
	/// </summary>
	public class SubsidiaryMasterDTO : BaseModel
	{
		//[Required]
		public int Subsidiary_Id { get; set; }
		public int SubsidiaryType_Id { get; set; }
		public string Subsidiary_Code { get; set; }
		public string Subsidiary_Name { get; set; }

		public string buttonTextMessage { get; set; } = "Add";
		public int Company_Id { get; set; }
		//public List<string> Companies { get; set; }

		public string CompanyName { get; set; }
		public string LocationName { get; set; }
		// public string? Company_Code { get; set; }

		public int CountryId { get; set; }
		public int Location_ID { get; set; }

		public SubsidiaryTypeEnum subsidiaryTypeEnum { get; set; }
		//public List<UserMapLocation> UserMapLocation { get; set; } = new List<UserMapLocation>(); // For `@UserMapLocation_Id`

		//  public List<Company> companyMaster { get; set; }
		//    public List<int> Department { get; set; }
		// public List<string> Role { get; set; }
		//  public string Country { get; set; }
		// public List<CountryMaster> countryMaster { get; set; }
		// public string State { get; set; }

		// public int Country_Id { get; set; }
		public int State_Id { get; set; }
		//public string Branch { get; set; }
		public int cityid { get; set; }

		public int Area_id { get; set; }
		// public string Externalunique_Id { get; set; }
		// public int Log_Id { get; set; }
	}
}
