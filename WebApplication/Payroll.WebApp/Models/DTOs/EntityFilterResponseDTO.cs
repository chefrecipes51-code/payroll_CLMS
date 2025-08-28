namespace Payroll.WebApp.Models.DTOs
{
    public class EntityFilterResponseDTO
    {
        public List<ContractorResponseDTO> Contractors { get; set; }
        public List<EntityCodeResponseDTO> EntityCodes { get; set; }
        public List<EntityNameResponseDTO> EntityNames { get; set; }
        public List<ContractorEntityResponseDTO> ContractorEntities { get; set; }
        public List<GradeMapEntityResponseDTO> GradeMapEntities { get; set; }
        public int TotalCount { get; set; }
    }
    public class EntityFilterRequestDTO
    {
        public string SelectType { get; set; } // e.g., "C", "ED", "EM", "CO"
        public int? CompanyId { get; set; }
        public int? CorrespondanceId { get; set; }
        public int? ContractorId { get; set; }
        public int? EntityId { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string SelectFilter { get; set; }
        public int? Regime_Id { get; set; }
        public int? FinYear_ID { get; set; }
        public int? Trade_Id { get; set; }
        public int? Skill_ID { get; set; }
        public int? Grade_ID { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class ContractorResponseDTO
    {
        public int Contractor_ID { get; set; }
        public string Contractor_Name { get; set; }
    }
    public class EntityCodeResponseDTO
    {
        public int Entity_ID { get; set; }
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
    }
    public class EntityNameResponseDTO
    {
        public int Entity_ID { get; set; }
        public string Entity_Name { get; set; }
    }
    public class ContractorEntityResponseDTO
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
    public class GradeMapEntityResponseDTO
    {
        public int Entity_ID { get; set; }
        public int Contractor_ID { get; set; }
        public int Contractor_Mast_Code { get; set; }
        public string Contractor_Name { get; set; }
        public string Entity_Code { get; set; }
        public string Entity_Name { get; set; }
        public int Regime_Id { get; set; }
        public string Regimename { get; set; } // string since it's converted to VARCHAR in SQL
        public string PayGradeName { get; set; } // string since it's converted to VARCHAR in SQL
        public string Dateof_Deployment { get; set; } // string since it's converted to VARCHAR in SQL
        public string Trade_Name { get; set; } // string since it's converted to VARCHAR in SQL
        public string LocationName { get; set; } // string since it's converted to VARCHAR in SQL
        public string Skillcategory_Name { get; set; } // string since it's converted to VARCHAR in SQL
        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
    }
}
