using System.Text.RegularExpressions;

namespace Payroll.WebApp.Common
{ 
    
      /// <summary>
      /// Created By:- Harshida Parmar
      /// Created Date:- 27-11-'24
      /// Note:- Individual class to define rules. 
      /// </summary>
    public class PayrollColumnValidation
    {
    }
    public static class DepartmentColumnValidation
    {
        public const int DepartmentTemplateId = 1;
        public const int DepartmentColumnCount = 3;
        public const string DepartmentTable = "tbl_mst_department";

        public static readonly List<string> DepartmentColumnNames = new List<string>
        {
            "DepartmentCode",
            "DepartmentName",
            "ExternalUnique_Id"
        };

        public static readonly List<Func<string, bool>> DepartmentColumnValidations = new List<Func<string, bool>>
        {
            // First column: DepartmentCode (non-empty, maximum length 10, not a date or number)
            value => !string.IsNullOrEmpty(value) &&
                     value.Length <= 10 &&
                     !DateTime.TryParse(value, out _) &&
                     !decimal.TryParse(value, out _),

            // Second column: DepartmentName (non-empty, maximum length 200, not a date or number)
            value => !string.IsNullOrEmpty(value) &&
                     value.Length <= 200 &&
                     !DateTime.TryParse(value, out _) &&
                     !decimal.TryParse(value, out _),

            // Third column: ExternalUnique_Id (nullable, maximum length 10)
            value => string.IsNullOrEmpty(value) || value.Length <= 10
        };
    }

    public static class ContractorDocumentColumnValidation
    {
        public const int ContractorDocumentId = 4;
        public const int ContractorDocumentColumnCount = 7;
        public const string ContractorDocumentTable = "tbl_mst_Contractor_Document";
        public static readonly List<string> ContractorDocumentColumnNames = new List<string>
            {
                "Company_ID","Contractor_ID","Company_Code","Document_Type","Document_Name","Document_Path", "ExternalUnique_Id" 
            };
        public static readonly List<Func<string, bool>> ContractorDocumentColumnValidations = new List<Func<string, bool>>
            {
                // 1. Company_id: Must be a valid positive integer or nullable
                value => string.IsNullOrEmpty(value) || (int.TryParse(value, out var companyId) && companyId > 0),

                // 2. Contractor_ID: Must be a valid positive integer (Required)
                value => int.TryParse(value, out var contractorId) && contractorId > 0,
                
                // 3. CompanyCode: Must be non-empty and have a maximum length of 10
               value => !string.IsNullOrEmpty(value) && value.Length <= 10 && Regex.IsMatch(value, "^[A-Za-z0-9]+$"),

                // 4. Document_Type: Must be a valid positive integer (Required)
                value => int.TryParse(value, out var documentTypeId) && documentTypeId > 0,

                // 5. Document_Name: Must be non-empty and have a maximum length of 100
                value => !string.IsNullOrEmpty(value) && value.Length <= 100,

                // 6. Document_Path: Must be non-empty and have a maximum length of 255
                value => !string.IsNullOrEmpty(value) && value.Length <= 255,
                
                // 7: ExternalUnique_Id (nullable, maximum length 10)
                value => string.IsNullOrEmpty(value) || value.Length <= 10

            };

    }

    public static class SubsidiaryColumnValidation
    {
        public const int SubsidiaryTemplateId = 5; // A unique template ID for subsidiary data
        public const int SubsidiaryColumnCount = 5; // Number of columns in the CSV
        public const string SubsidiaryTable = "tbl_stg_subsidiary";

        public static readonly List<string> SubsidiaryColumnNames = new List<string>
        {
            "SubsidiaryType_Id", // Tinyint
            "Subsidiary_Code",   // Char(6)
            "Subsidiary_Name",   // NVarchar(100)
            "Company_Code",      // NVarchar(10)
            "ExternalUnique_Id"  // NVarchar(10)
        };

        public static readonly List<Func<string, bool>> SubsidiaryColumnValidations = new List<Func<string, bool>>
        {
            // First column: SubsidiaryType_Id (nullable, must be a valid number between 0-255 for tinyint)
            value => string.IsNullOrEmpty(value) || (byte.TryParse(value, out var num) && num >= 0),

            // Second column: Subsidiary_Code (non-empty, exactly 6 characters, alphanumeric)
            value => !string.IsNullOrEmpty(value) &&
                     value.Length == 6 &&
                     !DateTime.TryParse(value, out _) &&
                     !decimal.TryParse(value, out _),

            // Third column: Subsidiary_Name (non-empty, maximum length 100)
            value => !string.IsNullOrEmpty(value) && value.Length <= 100,

            // Fourth column: Company_Code (nullable, maximum length 10)
            value => string.IsNullOrEmpty(value) || value.Length <= 10,

            // Fifth column: ExternalUnique_Id (nullable, maximum length 10)
            value => string.IsNullOrEmpty(value) || value.Length <= 10
        };
    }

    public static class LocationColumnValidation
    {
        public const int LocationTemplateId = 3; // Assign an ID for the location template
        public const int LocationColumnCount = 4; // Total columns required for the location table
        public const string LocationTable = "tbl_mst_location";

        public static readonly List<string> LocationColumnNames = new List<string>
        {
            "CityId", "LocationName", "IsActive", "CreatedBy"
        };

        public static readonly List<Func<string, bool>> LocationColumnValidations = new List<Func<string, bool>>
        {
            // First column: CityId (must be a valid integer and greater than 0)
            value => int.TryParse(value, out int cityId) && cityId > 0,

            // Second column: LocationName (non-empty string, max length 100)
            value => !string.IsNullOrEmpty(value) && value.Length <= 100,

            // Third column: IsActive (boolean represented as "1" or "0")
            value => value == "1" || value == "0",

            // Fourth column: CreatedBy (must be a valid integer and greater than 0)
            value => int.TryParse(value, out int createdBy) && createdBy > 0,
        };
    }

}
