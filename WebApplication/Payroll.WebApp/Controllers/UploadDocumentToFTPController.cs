using Microsoft.AspNetCore.Mvc;
using Payroll.Common.FtpUtility;
using System.Net;

namespace Payroll.WebApp.Controllers
{
    public class UploadDocumentToFTPController : Controller
    {
        private readonly FtpService _ftpService;
        public UploadDocumentToFTPController(FtpService ftpService)
        {
            _ftpService = ftpService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Please select a file to upload.");
            }

            // Check file extension to allow only Excel and CSV files
            var allowedExtensions = new[] { ".xls", ".xlsx", ".csv" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Only Excel and CSV files are allowed.");
            }

            // Generate the new filename with the current timestamp
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var newFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{timestamp}{fileExtension}";

            // Save the file to a temporary location
            var tempPath = Path.GetTempFileName();
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Upload the file to FTP with the new filename
            var uploadSuccess = await _ftpService.UploadFileAsync(tempPath, newFileName);

            // Delete the temporary file after upload
            System.IO.File.Delete(tempPath);

            return Ok("File uploaded successfully to FTP server.");
        }
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest("File name is required.");
            }
            string ftpFilePath = $"{FtpSettings.Host}{fileName}";
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
        //public async Task<IActionResult> UploadFile(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("Please select a file to upload.");
        //    }

        //    // Save the file to a temporary location
        //    var tempPath = Path.GetTempFileName();
        //    using (var stream = new FileStream(tempPath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }


        //    // Upload the file to FTP
        //    string ftpFileName = file.FileName; // Use the original file name for FTP
        //    await _ftpService.UploadFileAsync(tempPath, ftpFileName);

        //    // Delete the temporary file after upload
        //    System.IO.File.Delete(tempPath);

        //    return Ok("File uploaded successfully to FTP server.");
        //}
    }
    public class FtpSettings_
    {
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
