using Microsoft.AspNetCore.Mvc.Rendering;

namespace Payroll.WebApp.Models
{
    public class DepartmentViewModel
    {  
        // Dropdown list for departments
        public List<SelectListItem> DepartmentDropdown { get; set; }

        // Default constructor
        public DepartmentViewModel()
        {
            DepartmentDropdown = new List<SelectListItem>();
        }
    }

    public class CountriesViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> CountriesDropdown { get; set; }

        // Default constructor
        public CountriesViewModel()
        {
            CountriesDropdown = new List<SelectListItem>();
        }
    }
    public class CityViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> CitysDropdown { get; set; }

        // Default constructor
        public CityViewModel()
        {
            CitysDropdown = new List<SelectListItem>();
        }
    }
    public class StateViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> StatesDropdown { get; set; }

        // Default constructor
        public StateViewModel()
        {
            StatesDropdown = new List<SelectListItem>();
        }
    }
    public class UserTypeViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> UserTypeDropdown { get; set; }

        // Default constructor
        public UserTypeViewModel()
        {
            UserTypeDropdown = new List<SelectListItem>();
        }
    }

    public class UserListViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> UserDropdown { get; set; }

        // Default constructor
        public UserListViewModel()
        {
            UserDropdown = new List<SelectListItem>();
        }
    }

    public class CompanyViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> CompanyDropdown { get; set; }

        // Default constructor
        public CompanyViewModel()
        {
            CompanyDropdown = new List<SelectListItem>();
        }
    }
    public class CompanyCountryViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> CompanyCountryDropdown { get; set; }

        // Default constructor
        public CompanyCountryViewModel()
        {
            CompanyCountryDropdown = new List<SelectListItem>();
        }
    }
    public class LocationViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> LocationDropdown { get; set; }

        // Default constructor
        public LocationViewModel()
        {
            LocationDropdown = new List<SelectListItem>();
        }
    }
    public class CompanyLocationMapViewModel
    {
        public List<DropdownItem> Countries { get; set; }
        public List<DropdownItem> States { get; set; }
        public List<DropdownItem> Cities { get; set; }
        public List<DropdownItem> Locations { get; set; }
    }
    public class DropdownItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class RoleViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> RoleDropdown { get; set; }

        // Default constructor
        public RoleViewModel()
        {
            RoleDropdown = new List<SelectListItem>();
        }
    }
    public class SalutationViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> SalutationModel { get; set; }
    }

    public class UserRoleMenuViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> UserRoleMenuDropdown { get; set; }

        // Default constructor
        public UserRoleMenuViewModel()
        {
            UserRoleMenuDropdown = new List<SelectListItem>();
        }
    }
    #region Added By Harshida 
    public class EntityTypeViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> EntityTypeDropdown { get; set; }

        // Default constructor
        public EntityTypeViewModel()
        {
            EntityTypeDropdown = new List<SelectListItem>();
        }
    }
    public class CompanyTypeViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> CompanyTypeDropdown { get; set; }

        // Default constructor
        public CompanyTypeViewModel()
        {
            CompanyTypeDropdown = new List<SelectListItem>();
        }
    }
    public class CompanyCurrencyViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> CompanyCurrencyDropdown { get; set; }

        // Default constructor
        public CompanyCurrencyViewModel()
        {
            CompanyCurrencyDropdown = new List<SelectListItem>();
        }
    }
    public class AccountHeadViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> AccountHeadDropdown { get; set; }

        // Default constructor
        public AccountHeadViewModel()
        {
            AccountHeadDropdown = new List<SelectListItem>();
        }
    }
    public class GLViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> GLDropdown { get; set; }

        // Default constructor
        public GLViewModel()
        {
            GLDropdown = new List<SelectListItem>();
        }
    }
    public class FormulaViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> FormulaDropdown { get; set; }

        // Default constructor
        public FormulaViewModel()
        {
            FormulaDropdown = new List<SelectListItem>();
        }
    }

    #endregion
    public class AreaViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> AreasDropdown { get; set; }

        // Default constructor
        public AreaViewModel()
        {
            AreasDropdown = new List<SelectListItem>();
        }
    }
    public class FloorViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> FloorsDropdown { get; set; }

        // Default constructor
        public FloorViewModel()
        {
            FloorsDropdown = new List<SelectListItem>();
        }
    }

    public class ModulesViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> ModulesDropdown { get; set; }

        // Default constructor
        public ModulesViewModel()
        {
            ModulesDropdown = new List<SelectListItem>();
        }
    }

    public class ServicesViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> ServicesDropdown { get; set; }

        // Default constructor
        public ServicesViewModel()
        {
            ServicesDropdown = new List<SelectListItem>();
        }
    }

    public class PayGradeViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> PayGradeDropdown { get; set; }

        // Default constructor
        public PayGradeViewModel()
        {
            PayGradeDropdown = new List<SelectListItem>();
        }
    }
    public class DistinctLocationViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> DistinctLocationDropdown { get; set; }

        // Default constructor
        public DistinctLocationViewModel()
        {
            DistinctLocationDropdown = new List<SelectListItem>();
        }
    }
    public class IsParentComponentViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> ParentComponentDropdown { get; set; }

        // Default constructor
        public IsParentComponentViewModel()
        {
            ParentComponentDropdown = new List<SelectListItem>();
        }
    }
    public class SkillCategoryViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> SkillCategoryDropdown { get; set; }

        // Default constructor
        public SkillCategoryViewModel()
        {
            SkillCategoryDropdown = new List<SelectListItem>();
        }
    }
    public class TradeViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> TradeDropdown { get; set; }

        // Default constructor
        public TradeViewModel()
        {
            TradeDropdown = new List<SelectListItem>();
        }
    }
    public class TaxRegimeViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> TaxRegimeDropdown { get; set; }

        // Default constructor
        public TaxRegimeViewModel()
        {
            TaxRegimeDropdown = new List<SelectListItem>();
        }
    }
    public class ContractorViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> ContractorDropdown { get; set; }

        // Default constructor
        public ContractorViewModel()
        {
            ContractorDropdown = new List<SelectListItem>();
        }
    }
    public class ContractorWorkOrderViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> ContractorWorkOrderDropdown { get; set; }

        // Default constructor
        public ContractorWorkOrderViewModel()
        {
            ContractorWorkOrderDropdown = new List<SelectListItem>();
        }
    }
    public class WorkOrderViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> WorkOrderDropdown { get; set; }

        // Default constructor
        public WorkOrderViewModel()
        {
            WorkOrderDropdown = new List<SelectListItem>();
        }
    }
    public class FinYearViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> FinYearDropdown { get; set; }

        // Default constructor
        public FinYearViewModel()
        {
            FinYearDropdown = new List<SelectListItem>();
        }
    }

    public class PayComponentViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> PayComponentDropdown { get; set; }

        // Default constructor
        public PayComponentViewModel()
        {
            PayComponentDropdown = new List<SelectListItem>();
        }
    }
    public class SalaryStractureViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> SalaryStructureDropdown { get; set; }

        // Default constructor
        public SalaryStractureViewModel()
        {
            SalaryStructureDropdown = new List<SelectListItem>();
        }
    }
    public class CompanyPayrollValidationViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> CompanyPayrollValidationDropdown { get; set; }

        // Default constructor
        public CompanyPayrollValidationViewModel()
        {
            CompanyPayrollValidationDropdown = new List<SelectListItem>();
        }
    }
    public class CompanyLocationPayrollValidationViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> CompanyLocationPayrollValidationDropdown { get; set; }

        // Default constructor
        public CompanyLocationPayrollValidationViewModel()
        {
            CompanyLocationPayrollValidationDropdown = new List<SelectListItem>();
        }
    }
    public class ContractorPayrollValidationViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> ContractorPayrollValidationDropdown { get; set; }

        // Default constructor
        public ContractorPayrollValidationViewModel()
        {
            ContractorPayrollValidationDropdown = new List<SelectListItem>();
        }
    }
    public class WorkOrderPayrollValidationViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> WorkOrderPayrollValidationDropdown { get; set; }

        // Default constructor
        public WorkOrderPayrollValidationViewModel()
        {
            WorkOrderPayrollValidationDropdown = new List<SelectListItem>();
        }
    }
    public class PreviousMonthYearPeriodPayrollValidationViewModel
    {
        // Dropdown list for departments
        public List<SelectListItem> PreviousMonthYearPeriodPayrollValidationDropdown { get; set; }

        // Default constructor
        public PreviousMonthYearPeriodPayrollValidationViewModel()
        {
            PreviousMonthYearPeriodPayrollValidationDropdown = new List<SelectListItem>();
        }
    }
}
