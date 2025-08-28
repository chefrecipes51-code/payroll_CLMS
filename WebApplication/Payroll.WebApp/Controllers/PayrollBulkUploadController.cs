/***************************************************************************************************
 *                                                                                                 
 *  Project                 : Payroll Management System                                                        
 *  File                    : PayrollBulkUploadController.cs                                                   
 *  Description             : Controller for handling bulk upload operations, including file validation,       
 *                              data staging, and file management.                                               
 *                                                                                                
 *  Author                  : Harshida Parmar                                                                  
 *  Date                    : December 3, 2024                                                                 
 *  Jira Task By Harshida   : 246,265                                                                                                  
 *  © 2024 Harshida Parmar. All Rights Reserved.                                                  
 *                                                                                           
 **************************************************************************************************/

using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using Payroll.WebApp.Common;
using System.Globalization;
using DataMigrationService.BAL.Models;
using Payroll.WebApp.Helpers;
using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payroll.Common.FtpUtility;
using static Payroll.Common.EnumUtility.EnumUtility;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text.RegularExpressions;
using Payroll.Common.CommonDto;
using Payroll.WebApp.Models;
using static System.Net.WebRequestMethods;
using System.Diagnostics.Metrics;
using Payroll.Common.ViewModels;

namespace Payroll.WebApp.Controllers
{
    public class PayrollBulkUploadController : Controller
    {
        #region CTOR
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly FtpService _ftpService;
        private readonly RestApiDataMigrationServiceHelper _migrationServiceHelper;
        int DPId = 0;
        public PayrollBulkUploadController(RestApiDataMigrationServiceHelper datamigrationServiceHelper, FtpService ftpService, IOptions<ApiSettings> apiSettings, IMapper mapper)
        {
            _ftpService = ftpService;
            _apiSettings = apiSettings.Value;
            _mapper = mapper;
            _migrationServiceHelper = datamigrationServiceHelper;
        }
        #endregion        
        #region Upload And Verify file Data
        public async Task<IActionResult> Index()
        {
            //DPId = 3; //DepartmentBulk
            DPId = 2; //CLSM 
            var model = await BindServiceNameAsync(); // Get the model populated with service data
            return View(model); // Pass the populated model to the view
        }
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file, int templateId)
        {
            if (templateId == 0)
            {
                ModelState.AddModelError("templateId", "Please select a valid service.");
            }

            var model = await BindServiceNameAsync(); // Get the model with the service list
            model.templateId = templateId; // Set the selected service to the model


            if (file == null || file.Length == 0)
            {
                return Json(new List<PayrollValidationViewModel>());
            }
            List<string> expectedColumnNames;
            int expectedColumnCount;
            List<Func<string, bool>> columnValidations;
            List<ValidationResultModel> validationResults = new List<ValidationResultModel>();
            switch (templateId)
            {
                case DepartmentColumnValidation.DepartmentTemplateId:
                    expectedColumnNames = DepartmentColumnValidation.DepartmentColumnNames;
                    expectedColumnCount = DepartmentColumnValidation.DepartmentColumnCount;
                    columnValidations = DepartmentColumnValidation.DepartmentColumnValidations;
                    break;
                case ContractorDocumentColumnValidation.ContractorDocumentId:
                    // Set the expected columns and validations for the case
                    expectedColumnNames = ContractorDocumentColumnValidation.ContractorDocumentColumnNames;
                    expectedColumnCount = ContractorDocumentColumnValidation.ContractorDocumentColumnCount;
                    columnValidations = ContractorDocumentColumnValidation.ContractorDocumentColumnValidations;

                    #region Check Unique CompanyID ONLY
                    // Additional validation for "Company_ID"
                    using (var reader = new StreamReader(file.OpenReadStream()))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        // Read all records from the CSV
                        var records = csv.GetRecords<dynamic>().ToList();

                        // Find the index of the "Company_ID" column
                        var companyIdColumnIndex = expectedColumnNames.IndexOf("Company_ID");
                        if (companyIdColumnIndex != -1)
                        {
                            // Extract all values from the "Company_ID" column
                            var companyIdValues = records
                                .Select(record => ((IDictionary<string, object>)record)
                                .Values
                                .ElementAt(companyIdColumnIndex)
                                ?.ToString())
                                .Where(value => !string.IsNullOrEmpty(value))
                                .ToList();

                            if (companyIdValues.Distinct().Count() > 1)
                            {
                                return Json(new ValidationResultModel
                                {
                                    RowIndex = 0,
                                    HasError = true,
                                    ErrorMessages = new List<string> { "Only a unique value is allowed in the 'Company_ID' column." }
                                });
                            }
                        }
                    }
                    #endregion

                    break;

                default:
                    return BadRequest("Invalid template ID.");
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Read the CSV records dynamically
                var records = csv.GetRecords<dynamic>();

                int rowIndex = 2; // Start at 2 because the first row is headers

                foreach (var record in records)
                {
                    var columns = ((IDictionary<string, object>)record).Values.ToList();
                    var validationResult = new ValidationResultModel
                    {
                        RowIndex = rowIndex,
                        ColumnValues = columns.Select(c => c.ToString()).ToList(), // Convert columns to string values
                        ColumnHeaders = expectedColumnNames, // Use expectedColumnNames dynamically
                        HasError = false,
                        ErrorMessages = new List<string>()
                    };

                    // Validate the row
                    if (columns.Count != expectedColumnCount)
                    {
                        validationResult.HasError = true;
                        validationResult.ErrorMessages.Add($"Row {rowIndex} does not have the correct number of columns.");
                    }
                    else
                    {
                        for (int i = 0; i < columns.Count; i++)
                        {
                            bool isValid = columnValidations[i](columns[i].ToString());

                            if (!isValid)
                            {
                                validationResult.HasError = true;
                                validationResult.ErrorMessages.Add($"Error in Column '{expectedColumnNames[i]}': '{columns[i]}' is not valid.");
                            }
                        }
                        #region Check File Extension for ContractorDocument
                        if (templateId == ContractorDocumentColumnValidation.ContractorDocumentId)
                        {
                            // Find the index of the "Document_Path" column
                            var documentPathColumnIndex = expectedColumnNames.IndexOf("Document_Path");

                            if (documentPathColumnIndex != -1)
                            {
                                var documentPath = columns[documentPathColumnIndex].ToString();

                                // Regular expression to check if the file path has an extension like .pdf, .docx, .txt, etc.
                                string fileExtensionPattern = @"\.[a-zA-Z0-9]+$"; // Matches file extensions like .pdf, .jpg, etc.

                                if (!Regex.IsMatch(documentPath, fileExtensionPattern))
                                {
                                    validationResult.HasError = true;
                                    validationResult.ErrorMessages.Add($"Error in Column 'Document_Path': '{documentPath}' must contain a valid file extension.");
                                }
                            }
                        }
                        #endregion
                    }
                    validationResults.Add(validationResult);
                    rowIndex++;
                }
            }
            return Json(validationResults);
        }
        #endregion
        #region Bulk Upload Main Method
        [HttpPost]
        public async Task<IActionResult> SaveDataInStaging([FromForm] IFormFile file, [FromForm] int templateId, [FromForm] string tableData)
        {
            if (string.IsNullOrEmpty(tableData))
            {
                return BadRequest(new { message = "No valid records provided." });
            }

            // Deserialize tableData JSON into List<SaveDataRequest>
            List<SaveDataRequest> validRecords = JsonConvert.DeserializeObject<List<SaveDataRequest>>(tableData);

            // Handle invalid records if needed (optional)
            if (validRecords == null || validRecords.Count == 0)
            {
                return BadRequest(new { message = "No valid records to process." });
            }
            // Handle the file if it is provided
            string filePath = null;
            if (file != null && file.Length > 0)
            {
                #region Upload Files into FTP
                string newFileName = null;
                if (file != null && file.Length > 0)
                {
                    newFileName = await HandleFileRenaming(file);
                }
                #endregion

                // templateId = 1;
                // Process the data based on the templateId
                #region Insert Data into Staging Table Switch Case Start
                switch (templateId)
                {
                    case DepartmentColumnValidation.DepartmentTemplateId:
                        return await BulkUploadDepartmentStaging(validRecords, newFileName, file, "Department");

                    case ContractorDocumentColumnValidation.ContractorDocumentId:
                        return await BulkUploadContractorDepartmentStaging(validRecords, newFileName, file, "ContractorDocument");

                    case SubsidiaryColumnValidation.SubsidiaryTemplateId:
                        return await BulkUploadSubsidiaryStaging(validRecords, newFileName, file, "Subsidiary");
                    default:
                        return BadRequest("Invalid template ID.");
                }
                #endregion
            }
            // Return a success response
            return Ok(new { message = "Data saved successfully!" });
        }


        #endregion
        #region CommonMethodForBindDropDown
        private async Task<PayrollValidationViewModel> BindServiceNameAsync()
        {
            PayrollValidationViewModel obj = new PayrollValidationViewModel();
            #region Bind ServiceName
            string serviceNameApiUrl = _apiSettings.BaseUrlPayrollDataMigrationServiceEndpoints.GetBulkUploadServiceNameAndTemplateUrl;
            string queryParams = $"?ServiceType={DPId}&Service_Id=0&IsActive=true";

            var serviceList = await _migrationServiceHelper.GetServiceNameAsync($"{serviceNameApiUrl}{queryParams}");

            if (serviceList != null && serviceList.Any())
            {
                // Populate the ServiceDropdown list in the model
                obj.ServiceDropdown = serviceList.Select(item => new SelectListItem
                {
                    Value = item.Service_Id.ToString(),
                    Text = item.ServiceName
                }).ToList();
            }
            else
            {
                // Handle the case where no services are returned
                obj.ServiceDropdown = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "No Services Available" }
                };
            }
            #endregion

            return obj;
        }
        #endregion

        #region Get Template And HelpFile Name
        [HttpGet]
        public async Task<JsonResult> DownLoadFileNamePath(int templateId)
        {
            PayrollValidationViewModel obj = new PayrollValidationViewModel();
            string serviceNameApiUrl = _apiSettings.BaseUrlPayrollDataMigrationServiceEndpoints.GetBulkUploadServiceNameAndTemplateUrl;
            string queryParams = $"?ServiceType=1&Service_Id={templateId}&IsActive=true";

            var serviceList = await _migrationServiceHelper.GetSampleAndHelpFileAsync($"{serviceNameApiUrl}{queryParams}");
            foreach (var service in serviceList)
            {
                service.HelpFileName = $"{service.HelpFileName}.{service.FileExtension.ToLower()}";
                service.TemplateName = $"{service.TemplateName}.{service.FileExtension.ToLower()}";
            }
            return Json(serviceList);
        }
        #endregion
        #region Download Physical File
        [HttpGet]
        public async Task<IActionResult> DownloadFile(string fileName, string filePath)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(filePath))
            {
                return BadRequest("File name and path are required.");
            }

            string ftpFilePath = $"{filePath}{fileName}";
            string tempFilePath = Path.GetTempFileName(); // Temporary location for download

            var request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(FtpSettings.Username, FtpSettings.Password);

            using (var response = (FtpWebResponse)await request.GetResponseAsync())
            using (var responseStream = response.GetResponseStream())
            using (var fileStream = new FileStream(tempFilePath, FileMode.Create))
            {
                await responseStream.CopyToAsync(fileStream);
            }

            byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);
            System.IO.File.Delete(tempFilePath); // Clean up temporary file

            // Return as downloadable file with Save As dialog
            return File(fileBytes, "application/octet-stream", fileName);
        }

        #endregion


        #region Handle File Renaming Process To Pass During inserting Data
        private async Task<string> HandleFileRenaming(IFormFile file)
        {
            var allowedExtensions = new[] { ".xls", ".xlsx", ".csv", ".txt" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Only Excel and CSV files are allowed.");
            }

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var newFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{timestamp}{fileExtension}";
            return newFileName;
        }

        #endregion

        #region Bulk Upload Individual Validation

        #region Bulk Upload For Departnment
        private async Task<IActionResult> BulkUploadDepartmentStaging(List<SaveDataRequest> validRecords, string newFileName, IFormFile file, string DS)
        {
            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
            List<Department> departments = validRecords.Select(record => new Department
            {
                DepartmentCode = record.ColumnValues[record.ColumnHeaders.IndexOf("DepartmentCode")],
                DepartmentName = record.ColumnValues[record.ColumnHeaders.IndexOf("DepartmentName")],
                ExternalUnique_Id = record.ColumnValues[record.ColumnHeaders.IndexOf("ExternalUnique_Id")],
                IsError = record.HasError,
                ErrorRemarks = string.Join(", ", record.ErrorMessages)
            }).ToList();

            var departmentStage = new DepartmentStage
            {
                TemplateFileName = newFileName,
                UploadFileName = newFileName,
                UploadFilePath = null, // Adjust if needed
                CreatedBy = userId,  //SESSION value will be inserted here
                Departments = departments
            };

            var apiUrl = _apiSettings.BaseUrlPayrollDataMigrationServiceEndpoints.PostDataMigrationDepartmentUrl;
            var responseModel = await _migrationServiceHelper.PostDepartmentStageAsync(apiUrl, departmentStage);
            #region Upload file into FTP
            // if (responseModel.Message == "Records have been Successfully Imported.") {
            if (responseModel.Message == BulkUploadStatus.DepartmentUpload.GetDescription())
            {
                var tempPath = Path.GetTempFileName();
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var uploadSuccess = await _ftpService.UploadFileAsync(tempPath, newFileName);
                System.IO.File.Delete(tempPath);
            }
            #endregion
            return Json(responseModel);
        }
        #endregion

        #region Bulk Upload For Subsidiary Stage
        private async Task<IActionResult> BulkUploadSubsidiaryStaging(List<SaveDataRequest> validRecords, string newFileName, IFormFile file, string DS)
        {
            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
            List<Department> departments = validRecords.Select(record => new Department
            {
                DepartmentCode = record.ColumnValues[record.ColumnHeaders.IndexOf("DepartmentCode")],
                DepartmentName = record.ColumnValues[record.ColumnHeaders.IndexOf("DepartmentName")],
                ExternalUnique_Id = record.ColumnValues[record.ColumnHeaders.IndexOf("ExternalUnique_Id")],
                IsError = record.HasError,
                ErrorRemarks = string.Join(", ", record.ErrorMessages)
            }).ToList();

            var departmentStage = new DepartmentStage
            {
                TemplateFileName = newFileName,
                UploadFileName = newFileName,
                UploadFilePath = null, // Adjust if needed
                CreatedBy = userId,  //SESSION value will be inserted here
                Departments = departments
            };

            var apiUrl = _apiSettings.BaseUrlPayrollDataMigrationServiceEndpoints.PostDataMigrationDepartmentUrl;
            var responseModel = await _migrationServiceHelper.PostDepartmentStageAsync(apiUrl, departmentStage);
            #region Upload file into FTP
            // if (responseModel.Message == "Records have been Successfully Imported.") {
            if (responseModel.Message == BulkUploadStatus.DepartmentUpload.GetDescription())
            {
                var tempPath = Path.GetTempFileName();
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var uploadSuccess = await _ftpService.UploadFileAsync(tempPath, newFileName);
                System.IO.File.Delete(tempPath);
            }
            #endregion
            return Json(responseModel);
        }
        #endregion

        #region Bulk Upload For ContractorDocument
        private async Task<IActionResult> BulkUploadContractorDepartmentStaging(List<SaveDataRequest> validRecords, string newFileName, IFormFile file, string CD)
        {
            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            int userId = int.TryParse(sessionData.UserId, out var parsedUserId) ? parsedUserId : 0;
            var companyIds = validRecords
               .Select(record => int.TryParse(record.ColumnValues[record.ColumnHeaders.IndexOf("Company_ID")], out var companyId) ? companyId : 0)
               .Distinct()
               .ToList();

            // Ensure only one unique Company_ID exists
            if (companyIds.Count != 1)
            {
                // Handle error: either no Company_ID or more than one unique Company_ID
                return Json(new { success = false, message = "Multiple or no unique Company_ID found in the uploaded data." });
            }

            var companyId = companyIds.First();

            List<ContractDocument> contractorDocuments = validRecords.Select(record => new ContractDocument
            {
                Contractor_Id = int.TryParse(record.ColumnValues[record.ColumnHeaders.IndexOf("Contractor_ID")], out var contractorId) ? contractorId : 0,
                Document_Type_Id = int.TryParse(record.ColumnValues[record.ColumnHeaders.IndexOf("Document_Type")], out var documentType) ? documentType : 0,
                Company_Code = record.ColumnValues[record.ColumnHeaders.IndexOf("Company_Code")],
                DocumentName = record.ColumnValues[record.ColumnHeaders.IndexOf("Document_Name")],
                DocumentPath = record.ColumnValues[record.ColumnHeaders.IndexOf("Document_Path")],
                IsError = record.HasError,
                ErrorRemarks = string.Join(", ", record.ErrorMessages)
            }).ToList();


            var contractorDocumentStage = new ContractorDocumentMaster
            {
                TemplateFileName = newFileName,
                UploadFileName = newFileName,
                UploadFilePath = null, // Adjust if needed
                CreatedBy = userId, // SESSION value will be inserted here
                Company_Id = companyId,
                ContractDocumentTypes = contractorDocuments
            };

            // Post the data to the API endpoint
            var apiUrl = _apiSettings.BaseUrlPayrollDataMigrationServiceEndpoints.PostDataMigrationContractorDocumentUrl;
            var responseModel = await _migrationServiceHelper.PostContractorDepartmentStageAsync(apiUrl, contractorDocumentStage);

            #region Upload file into FTP
            //// If the API response indicates a successful import, upload the file to FTP

            if (responseModel.Message == BulkUploadStatus.ContractorDocumentUpload.GetDescription())
            {
                // Deserialize the JSON response to get document paths
                var bulkUploadResponse = JsonConvert.DeserializeObject<BulkUploadResponseContractorDocument>(responseModel.JsonResponse?.ToString());

                if (bulkUploadResponse != null && bulkUploadResponse.VALIDATE != null)
                {
                    // Loop through each validation group
                    foreach (var validate in bulkUploadResponse.VALIDATE)
                    {
                        // Loop through each validation result
                        foreach (var validationResult in validate.ValidationResults)
                        {
                            if (validationResult.IsError == 1) // Check for error documents
                            {
                                var documentPath = validationResult.DocumentPath; // Extract the DocumentPath

                                // Check if the file exists on the local path
                                #region Validating File Exist And Upload in FTP
                                if (System.IO.File.Exists(documentPath))
                                {
                                    try
                                    {
                                        // Create a temporary file for uploading
                                        var tempPath = System.IO.Path.GetTempFileName();

                                        // Copy the file from the local path to the temp path
                                        System.IO.File.Copy(documentPath, tempPath, true);

                                        // Define a unique file name for FTP upload
                                        var formFile = new FormFile(System.IO.File.OpenRead(documentPath), 0, new System.IO.FileInfo(documentPath).Length, null, System.IO.Path.GetFileName(documentPath));
                                        var uniqueFileName = await HandleFileRenaming(formFile);

                                        // Upload the file to FTP
                                        var uploadSuccess = await _ftpService.UploadFileAsync(tempPath, uniqueFileName);
                                        if (uploadSuccess.IsSuccess)
                                        {
                                            // Update the DocumentPath with the FTP path
                                            validationResult.DocumentPath = uploadSuccess.FtpFilePath;
                                            Console.WriteLine($"File uploaded successfully. FTP Path: {uploadSuccess.FtpFilePath}");
                                            #region Update the responseModel's corresponding DocumentPath
                                            var correspondingDocument = contractorDocuments
                                                .FirstOrDefault(doc => doc.Contractor_Id == validationResult.Contractor_Id && doc.DocumentName == validationResult.DocumentName);

                                            if (correspondingDocument != null)
                                            {
                                                correspondingDocument.DocumentPath = uploadSuccess.FtpFilePath;
                                            }
                                            #endregion
                                            #region UpdatePathAfterUploadingFileToFTP
                                            var apiUrlFtpPath = _apiSettings.BaseUrlPayrollDataMigrationServiceEndpoints.UpdateContractorDocumentFTPPathUrl + "/" + validationResult.Contractor_Doc_Id;
                                            ContractDocumentFTP obj = new ContractDocumentFTP
                                            {
                                                DocumentPath = uploadSuccess.FtpFilePath,
                                                Contractor_Doc_Id = validationResult.Contractor_Doc_Id,
                                                Contractor_Id = validationResult.Contractor_Id
                                            };
                                            var responseFtpPathModel = await _migrationServiceHelper.UpdateContractorDocumentAsync(apiUrlFtpPath, obj);
                                            #endregion
                                        }
                                        else
                                        {
                                            Console.WriteLine("Failed to upload the file.");
                                        }
                                        // Clean up the temporary file
                                        System.IO.File.Delete(tempPath);

                                        Console.WriteLine($"File uploaded successfully: {uniqueFileName}");
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error uploading file: {documentPath}. Details: {ex.Message}");
                                    }
                                }
                                else
                                {
                                    // Handle case where the file does not exist
                                    Console.WriteLine($"File not found at {documentPath}");
                                }
                                #endregion
                            }

                        }
                    }
                }
                else
                {
                    Console.WriteLine("No valid data found in the response.");
                }
                // Update the responseModel with the updated document paths(after FTP upload)
                responseModel.JsonResponse = JsonConvert.SerializeObject(bulkUploadResponse);

            }
            #endregion
            // Return the API response as JSON
            return Json(responseModel);
        }
        #endregion

        #endregion
        #region Testing
        public IActionResult DisplayGridSample()
        {
            return View();
        }
        #endregion

    }

    public class SaveDataInStagingRequest
    {
        public int TemplateId { get; set; }
        public List<SaveDataRequest> TableData { get; set; }
    }
    public class SaveDataRequest
    {
        public int RowIndex { get; set; }
        public List<string> ColumnValues { get; set; }
        public List<string> ErrorMessages { get; set; }
        public bool HasError { get; set; }
        public List<string> ColumnHeaders { get; set; }
    }


}
