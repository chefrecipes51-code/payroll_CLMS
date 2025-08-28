using Newtonsoft.Json;
using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class EntityMaster : BaseModel
    {
        // File information
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }

        // List of data (UDT equivalent)
        public List<EntityDataUDT> EntityDataUDTList { get; set; }


        // public string Department_Code { get; set; }
        // public int CreatedBy { get; set; }
        public int Module_Id { get; set; }
        // public int CompanyId { get; set; }
        public int Log_Id { get; set; }
    }

    public class EntityMasterVerified : BaseModel
    {
        // File information
        public string TemplateFileName { get; set; }
        public string UploadFileName { get; set; }
        public string UploadFilePath { get; set; }

        // List of data (UDT equivalent)
        public List<EntityDataValidateUDT> EntityDataValidateUDT { get; set; }

        public int Module_Id { get; set; }
        // public int CompanyId { get; set; }
        public int Log_Id { get; set; }
    }

    public class EntityDataValidateUDT
    {
        public string PayrollNo { get; set; }
        public bool IsVerified { get; set; }
    }
    public class EntityDataUDT
    {
        public int? Company_Id { get; set; }
        public string Company_Code { get; set; }
        public int EmploymentType { get; set; }
        public int? Contractor_id { get; set; }
        public string PayrollNo { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfJoining { get; set; }
        public int? HOD { get; set; }
        public int? Superior { get; set; }
        public int? EmpCategory { get; set; }
        public int? Religion { get; set; }
        public int? Nationality { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? MaritalStatus { get; set; }
        public int? Children { get; set; }
        public int? UserRole { get; set; }
        public string BloodGroup { get; set; }
        public string Emergency_Contact_Name { get; set; }
        public bool? Police_verification { get; set; }
        public int WorkOrder_Id { get; set; }
        public int WeekOff_Type { get; set; }
        public int WeekOff_Day { get; set; }
        public string Job_Description { get; set; }
        public bool Contract_Applicable { get; set; }
        public DateTime Contract_Start_date { get; set; }
        public DateTime Contract_End_date { get; set; }
        public bool OT_Applicable { get; set; }
        public int Shift_Id { get; set; }
        public bool Is_Auto_Renewal { get; set; }
        public int Location_Id { get; set; }
        public int? Department_Id { get; set; }
        public int? Sub_Department { get; set; }
        public int Designation { get; set; }
        public int? Position_Id { get; set; }
        public int? Organization_Unit { get; set; }
        public int EmailType_Id { get; set; }
        public string EmailAddress { get; set; }
        public bool Is_Default { get; set; }
        public int EmailType_Id2 { get; set; }
        public string EmailAddress2 { get; set; }
        public bool Is_Default2 { get; set; }
        public int PhoneType_Id { get; set; }
        public string PhoneNo { get; set; }
        public string Phone_Exchange { get; set; }
        public bool Is_Default_C { get; set; }
        public int PhoneType_Id2 { get; set; }
        public string PhoneNo2 { get; set; }
        public string Phone_Exchange2 { get; set; }
        public bool Is_Default2_C { get; set; }
        public int Log_id { get; set; }
        public string ExternalUnique_Id { get; set; }
        public bool? IsError { get; set; }
    }

    public class EntityUDT
    {
        public int? Company_Id { get; set; }
        public string Company_Code { get; set; }
        public int EmploymentType { get; set; }
        public int? Contractor_id { get; set; }
        public string PayrollNo { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfJoining { get; set; }
        public int? HOD { get; set; }
        public int? Superior { get; set; }
        public int? EmpCategory { get; set; }
        public int? Religion { get; set; }
        public int? Nationality { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public int? MaritalStatus { get; set; }
        public int? Children { get; set; }
        public int? UserRole { get; set; }
        public string BloodGroup { get; set; }
        public string Emergency_Contact_Name { get; set; }
        public bool? Police_verification { get; set; }
        public int Log_id { get; set; }
        public string ExternalUnique_Id { get; set; }
        public bool? IsError { get; set; }


    }

    public class EntityJobProfileUDT
    {
        public int? Company_Id { get; set; }
        public string Company_Code { get; set; }
        public int Emp_Id { get; set; }
        public string PayrollNo { get; set; }
        public int Contractor_Id { get; set; }
        public int WorkOrder_Id { get; set; }
        public int WeekOff_Type { get; set; }
        public int WeekOff_Day { get; set; }
        public string Job_Description { get; set; }
        public bool Contract_Applicable { get; set; }
        public DateTime Contract_Start_date { get; set; }
        public DateTime Contract_End_date { get; set; }
        public bool OT_Applicable { get; set; }
        public int Shift_Id { get; set; }
        public bool Is_Auto_Renewal { get; set; }
        public string ExternalUnique_Id { get; set; }
        public int Log_Id { get; set; }
        public bool? IsError { get; set; }
    }

    public class EntityPlacementUDT
    {
        public int? Company_Id { get; set; }
        public string Company_Code { get; set; }
        public int Emp_Id { get; set; }
        public string PayrollNo { get; set; }
        public int Location_Id { get; set; }
        public int? Department_Id { get; set; }
        public int? Sub_Department { get; set; }
        public int Designation { get; set; }
        public int? Position_Id { get; set; }
        public int? Organization_Unit { get; set; }
        public int Log_Id { get; set; }
        public string ExternalUnique_Id { get; set; }
        public bool? IsError { get; set; }
    }

    public class EntityEmailUDT
    {
        public int Emp_Id { get; set; }
        public int Company_Id { get; set; }
        public string Company_Code { get; set; }
        public string PayrollNo { get; set; }
        public int EmailType_Id { get; set; }
        public string EmailAddress { get; set; }
        public bool Is_Default { get; set; }
        public int Log_id { get; set; }
        public string ExternalUnique_Id { get; set; }
        public bool? IsError { get; set; }
    }

    public class EntityContactUDT
    {
        public int Emp_Id { get; set; }
        public int Company_Id { get; set; }
        public string Company_Code { get; set; }
        public string PayrollNo { get; set; }
        public int PhoneType_Id { get; set; }
        public string PhoneNo { get; set; }
        public string Phone_Exchange { get; set; }
        public bool Is_Default { get; set; }
        public int Log_id { get; set; }
        public string ExternalUnique_Id { get; set; }
        public bool? IsError { get; set; }
    }
}
