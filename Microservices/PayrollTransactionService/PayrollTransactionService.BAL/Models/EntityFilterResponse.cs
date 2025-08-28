using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class EntityFilterResponse
    {
        public List<ContractorResponse> Contractors { get; set; }
        public List<EntityCodeResponse> EntityCodes { get; set; }
        public List<EntityNameResponse> EntityNames { get; set; }
        public List<ContractorEntityResponse> ContractorEntities { get; set; }
        public List<GradeMapEntityResponse> GradeMapEntities { get; set; }
        public int TotalCount { get; set; }
    }
    public class EntityFilterRequest
    {
        public string SelectType { get; set; } // e.g., "C", "ED", "EM", "CO" ,"ET"
        public int? CompanyId { get; set; }
        public int? CorrespondanceId { get; set; }
        public int? ContractorId { get; set; }
        public int? EntityId { get; set; }
        public string EntityCode { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string SelectFilter { get; set; } = string.Empty;
        public int? Regime_Id { get; set; }
        public int? FinYear_ID { get; set; }
        public int? Trade_Id { get; set; }
        public int? Skill_ID { get; set; }
        public int? Grade_ID { get; set; }
        public int PageNumber { get; set; } 
        public int PageSize { get; set; } 
    }
    public class ContractorResponse
    {
        public int Contractor_ID { get; set; }
        public string Contractor_Name { get; set; }
    }
    public class EntityCodeResponse
    {
        public int Entity_ID { get; set; }
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
    }
    public class EntityNameResponse
    {
        public int Entity_ID { get; set; }
        public string Entity_Name { get; set; }
    }
    public class ContractorEntityResponse
    {
        public int Entity_ID { get; set; }
        public int Contractor_ID { get; set; }
        public int Contractor_Mast_Code { get; set; }
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
        public int Regime_Id { get; set; }
        public string Regimename { get; set; }
        public string Dateof_Deployment { get; set; } // string since it's converted to VARCHAR in SQL
    }
    public class GradeMapEntityResponse
    {
        public int Entity_ID { get; set; }
        public int Contractor_ID { get; set; }
        public int Contractor_Mast_Code { get; set; }
        public string Contractor_Name { get; set; }
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
        public int Regime_Id { get; set; }
        public string Regimename { get; set; }
        public string PayGradeName { get; set; }
        public string Dateof_Deployment { get; set; }
        public string Trade_Name { get; set; }
        public string LocationName { get; set; } // string since it's converted to VARCHAR in SQL
        public string Skillcategory_Name { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }

    }

}
