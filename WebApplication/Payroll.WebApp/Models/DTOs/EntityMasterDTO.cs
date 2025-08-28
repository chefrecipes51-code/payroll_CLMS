using Payroll.Common.ApplicationModel;

namespace Payroll.WebApp.Models.DTOs
{
    public class EntityMasterDTO : BaseModel
    {
        public int Entity_ID { get; set; }
        public string Entity_Code { get; set; }
        public string Entity_Old_Code { get; set; } = "na";
        public string Entity_Name { get; set; }

        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }

        public string Dateof_Deployment { get; set; } // formatted dd/MM/yyyy
        public string Validity_Date { get; set; }      // formatted dd/MM/yyyy

        public decimal? Net_Salary { get; set; }

        public string Salary_Basis { get; set; } // "Monthly", "Bi weakly", "Weakly"

        public string LocationName { get; set; }
        public string Contractor_Name { get; set; }
        public string Trade_Name { get; set; }
        public string Skillcategory_Name { get; set; }
        public string Subsidiary_Name { get; set; } // plant name

        public string WorkOrder_No { get; set; }

        public string Wo_Start_Date { get; set; } // formatted dd/MM/yyyy
        public string Wo_End_Date { get; set; }   // formatted dd/MM/yyyy

        public string Wk_Off1 { get; set; } // Day name, e.g., "Sunday"
        public string Wk_Off2 { get; set; }

        public int? Pf_Applicable { get; set; }
        public int? Vpf_Applicable { get; set; }
        public decimal? VPF_Percent { get; set; }
        public decimal? VPF_Value { get; set; }
        public int? PT_Applicable { get; set; }
        public int? PT_State_Id { get; set; }
        public decimal? Pt_Amount { get; set; }

        public string Pf_No { get; set; }
        public decimal? Pf_Amount { get; set; }
        public decimal? Pf_Percent { get; set; }
        public string Uan_No { get; set; }
        public string Policy_No { get; set; }

        public int? Lwf_Applicable { get; set; }
        public string? Esic_No { get; set; }
        public DateTime Esic_Exit_Date { get; set; }
        public int Pay_Grade_Id { get; set; }
        public int? GratuityApplicable { get; set; }
        public int? Deduction_Amount { get; set; }
        public int? VPF_Type { get; set; }
        public decimal? PolicyAmt { get; set; }
        public int Salarystructure_ID { get; set; }
    }
}
