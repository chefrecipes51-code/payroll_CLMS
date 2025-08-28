using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Payroll.Common.EnumUtility
{
    public class EnumUtility
    {
        public enum ApplicationMessageTypeEnum
        {
            // Add your enum values here
            Information = 1,
            Warning = 2,
            Error = 3,
            // ...
        }
        public enum SubsidiaryTypeEnum
        {
            Plant = 1,
            WareHouse = 2,
            Port = 3,
            Others = 4

        }
        public enum PTax
        {
            Yearly = 1,
            Monthly = 2,
            HalfYearly = 3,
            Quartly = 4,
            ParticularMonth = 5
        }
        public enum Gender
        {
            Male = 1,
            Female = 2,
            Transgender = 3
        }
        public enum ApplicationMessageModeEnum
        {
            // Add your enum values here
            Insert = 1,
            Update = 2,
            Delete = 3,
            Fetch = 4,
            Match = 5,
            Send = 6,
            Exist = 7,
            Deactivated = 8,
            ImportData = 9,
            ImportVerified = 10,
            DocumentUploadStaging = 11,
            DoesNotExist = 12
            // ...
        }
        public enum ModuleEnum
        {
            // Add your enum values here
            PayGradeMaster = 1,
            WageRateMaster = 2,
            EarningDeductionMaster = 3,
            LocationMaster = 4,
            AreaMaster = 5,
            CompanyCurrencyMaster = 6,
            CompanyMaster = 7,
            CompanyCorrespondanceMaster = 8,
            CompanyStatutoryMaster = 9,
            CompnayStatutoryDocumentType = 10,
            DepartmentMaster = 11,
            MapDepartmentLocation = 12,
            EmailMaster = 13,
            AuthMaster = 14,
            MappingUserCompany = 15,
            MappingUserRole = 16,
            MapUserLocation = 17,
            UserMaster = 18,
            MapDepartmentRole = 20,
            YearlyItTableMaster = 21,
            DetailYearlyItTableMaster = 22,
            RoleMaster = 23,
            UserActivateDeactivateStatus = 24,
            EventMaster = 26,
            RoleMenuMapping = 27,
            EventAuthSetUp = 28,
            DataMigrationImport = 29,
            WageConfigMaster = 31,
            EntityTaxStatutory = 32,
            SubsidiaryMaster = 33,
            SubsidiaryCompanyMaster = 33,
            FormulaMaster = 34,
            CompanyConfiguration = 35,
            ApprovalSetUp = 36,
            UserTransationHistory = 37,
            PayPeriod = 38,
            PTaxSlab = 42,            
            MapTaxRegime = 44,
            SalaryStructureConfiguration = 43,
            GlobalParameterSetup=45,
            PayrollSettingParameter=46, 
            ComplianceParameter=47,
            ThirdPartyParameter=48,
            MapGradeEntity =50,
            EntityCompliance = 51,           
            ApprovalStatus=52,
            PayrollTranSTGData=53,
            AccountingHead= 55,
            GLGroup=56
            // ...
        }
        public enum BulkUploadStatus
        {
            [Description("Records have been Successfully Imported.")]
            DepartmentUpload = 1,

            [Description("Records have been Successfully Imported.")]
            ContractorDocumentUpload = 2,

            [Description("Error occurred while importing records.")]
            ImportError = 3
        }

        /// Represents a set of Salutation(titles) with associated IDs for use in the application.
        /// <summary>
        /// <remarks>
        /// Developed by: Abhishek  
        /// Created Date   :- 18-Dec-2024
        /// </remarks>
        /// </summary>
        public enum SalutationEnum
        {
            Mr,
            Mrs,
            Miss,
            Dr,
            Ms,
            Prof,
            Rev,
            Lady,
            Sir,
            Capt,
            Major,
            LtCol,
            Col,
            LtCmdr,
            TheHon
            // ...
        }
        public enum CorrespondanceTypeEnum
        {
            Permanent = 1,
            Registered = 2,
            Correspondence = 3
        }
        public enum CalculationType
        {
            Variable = 1,
            Fixed = 2,
            Calculate = 3,
            Rate = 4
        }
        public enum PayrollHeads
        {
            Earning = 1,
            Deduction = 2,
        }
        public enum SalaryFrequencyEnum
        {
            PAnum = 1,
            Weekly = 2,
            BiWeekly = 3
        }
        public enum MonthlySalary
        {
            MonthDay = 1,
            PresentDay = 2,
            ExcludingWeekends = 3
        }
        public enum SalaryBasicsEnum
        {
            CTC = 1
            //Gross = 2,
            //Net = 3
        }
        public enum ComponentValueTypeEnum
        {
           // Percentage = 1,
            Amount = 2,
            Formula = 3
        }
        public enum YesNoEnum
        {
            Yes = 1,
            No = 0
        }

      
        public enum LockSalaryEditsPostPayroll
        {
            Yes = 1,
            No = 0
        }
        public enum EffectivePayrollStartMonth
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12
        }
        public enum PFEmployerShare
        {
            [Display(Name = "EPF + EPS")]
            EPF_EPS = 1,

            [Display(Name = "Full in EPF")]
            FullInEPF = 0
        }
        public enum PFBasedOn
        {
            Gross = 1,
            Others = 0
        }
        public enum ESICBasedOn
        {
            Gross = 1,
            NET = 2
        }
        public enum VoluntaryPF
        {
            Yes = 1,
            No = 0
        }
        public enum VPFMode
        {
            Percentageofbasic = 1,
            Fixed = 2,
            Amount =3
        }
        public enum ESICApplicability
        {
            Yes = 1,
            No = 0
        }
        public enum ProfessionalTax
        {            
            Not = 0,
            StateWise=1
        }
        public enum PTRegistrationMode
        {
            Single = 1,
            OrgWise=2,
            PerState=3
        }
        public enum LabourWelfareFund
        {
            Applicable = 1,
            NotApplicable = 2,
            StateWise = 3
        }
        public enum LWFCycle
        {
            Monthly = 1,
            HalfYearly = 2
        }
        public enum TDSDeductedOnActualDate
        {
            Yes = 1,
            No = 0
        }
        public enum CommonToggleYesNo
        {
            Yes = 1,
            No = 0
        }        
        public enum PayslipFormat
        {
            Simple = 1,
            Detailed = 2,
            ComponentWise=3
        }
        public enum DataSyncType
        {         
            Automatically = 1,
            Manually = 2
        }

        public enum SyncFrequency
        {         
            Sync = 1,
            Daily = 2,
            Weekly = 3,
            Monthly = 4
        }
        public enum PaymentFormat
        {
            DDL = 1,
            JSON = 2,
            CSV = 3,
            DB = 4
        }
        public enum TranType
        {
            Dr = 1,
            Cr = 2
        }
    }
}
