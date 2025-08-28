namespace Payroll.WebApp.Models
{
    #region BULK UPLOAD FOR Contractor 
    public class BulkUploadResponseContractorDocument
    {
        public List<ValidationGroupContractorDocument> VALIDATE { get; set; }
    }
    public class ValidationGroupContractorDocument
    {
        public List<ValidationResultContractorDocument> ValidationResults { get; set; }
        public List<CountResultContractorDocument> CountResults { get; set; }
    }
    public class ValidationResultContractorDocument
    {
        public int Contractor_Doc_Id { get; set; }
        public int Contractor_Id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public int IsError { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class CountResultContractorDocument
    {
        public int TotalValidatedRecords { get; set; }
        public int TotalUnvalidatedRecords { get; set; }
        public string ApplicationMessage { get; set; }
        public int ApplicationMessageType { get; set; }
    }
    #endregion
}
