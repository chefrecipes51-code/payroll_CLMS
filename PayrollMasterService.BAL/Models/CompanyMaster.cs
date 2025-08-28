using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollMasterService.BAL.Models
{
    public class CompanyMaster : BaseModel
    {
        public byte Company_Id { get; set; }
        public byte CompanyType_ID { get; set; }
        public string Company_Code { get; set; }
        public int Group_Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPrintName { get; set; }
        public bool IsParent { get; set; }
        public string CompanyShortName { get; set; }
        //public byte ParentCompanyId { get; set; }
        public byte ParentCompany_Id { get; set; }
        public byte CompanyLevel { get; set; }
        public int Location_ID { get; set; }
        public bool Has_Subsidary { get; set; }

        // Relationships
        public CompanyCorrespondance CompanyCorrespondance { get; set; }
        public CompanyStatutory CompanyStatutory { get; set; }
        public int Currency_ID { get; set; }// Added By Harshida 14-02-25
        public DateTime? StartDate { get; set; } // Added By Harshida 21-02-25
        public DateTime? EndDate { get; set; }   // Added By Harshida 21-02-25
    }

    #region New Class because of List CompanyCorrespondance and CompanyStatutory
    public class CompanyFinYear : BaseModel
    {
        public byte Company_Id { get; set; }     
        public string CompanyName { get; set; }   
        public DateTime? StartDate { get; set; } // Added By Harshida 14-04-25
        public DateTime? EndDate { get; set; }   // Added By Harshida 14-04-25
    }
    public class CompanyProfile : BaseModel
    {
        public byte Company_Id { get; set; }
        public byte CompanyType_ID { get; set; }
        public string Company_Code { get; set; }
        public int Group_Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPrintName { get; set; }
        public bool IsParent { get; set; }
        public string CompanyShortName { get; set; }
        //public byte ParentCompanyId { get; set; }
        public byte ParentCompany_Id { get; set; }
        public byte CompanyLevel { get; set; }
        public int Location_ID { get; set; }
        public bool Has_Subsidary { get; set; }

        // Relationships
        public List<CompanyCorrespondance> CompanyCorrespondances { get; set; } = new();
        public List<CompanyStatutory> CompanyStatutories { get; set; } = new();
		public List<SubsidiaryMaster> SubsidiaryMasteries { get; set; } = new();
		public int Currency_ID { get; set; }
        public DateTime? StartDate { get; set; } // Added By Harshida 21-02-25
        public DateTime? EndDate { get; set; }   // Added By Harshida 21-02-25
    }
    public class CompanyDemographicDetails : BaseModel
    {
        public byte Company_Id { get; set; }
        public byte CompanyType_ID { get; set; }
        public string Company_Code { get; set; }        
        public string CompanyName { get; set; }
        public string CompanyPrintName { get; set; }
        public bool IsParent { get; set; }
        public string CompanyShortName { get; set; }       
        public byte ParentCompany_Id { get; set; }        
        public bool Has_Subsidary { get; set; }
        //public int Group_Id { get; set; } // 06-02-25 AS PER the input from Purojit Sir We will do it later
        //public byte CompanyLevel { get; set; } // 06-02-25 AS PER the input from Purojit Sir We will do it later
        //public int Location_ID { get; set; } // 06-02-25 AS PER the input from Purojit Sir We will do it later
    }
    #endregion
}
