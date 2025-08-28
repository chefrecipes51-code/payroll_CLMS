using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace Payroll.WebApp.Controllers
{
    /// <summary>
    /// Created By:- Harshida D. Parmar
    /// Created Date:- 28-11-'24
    /// Note:- This controller contains the code in which you can get the idea
    ///         Either you can download individual File (Index.cshtml) 
    ///         OR download all files and make RAR file (DownloadAllFiles.cshtml).
    /// </summary>
    public class SampleDownloadMoreFilesAtOneGoController : Controller
    {

        #region Download Individual File at One Go
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DownloadCsvFile()
        {
            // Path to the CSV file
            string csvFilePath = Path.Combine("wwwroot", "files", "sample.csv");
            var fileBytes = System.IO.File.ReadAllBytes(csvFilePath);
            return File(fileBytes, "text/csv", "sample.csv");
        }

        [HttpGet]
        public IActionResult DownloadTextFile()
        {
            // Path to the text file
            string textFilePath = Path.Combine("wwwroot", "files", "sample.txt");
            var fileBytes = System.IO.File.ReadAllBytes(textFilePath);
            return File(fileBytes, "text/plain", "sample.txt");
        }
        #endregion
        #region Download All file in on go RAR file 
        public IActionResult DownloadAllFiles()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DownloadFilesUsingRAR()
        {
            // Paths to your files (replace with actual file paths)
            string csvFilePath = Path.Combine("wwwroot", "files", "sample.csv");
            string textFilePath = Path.Combine("wwwroot", "files", "sample.txt");

            // Create a memory stream to hold the ZIP
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // Add CSV file to the ZIP
                    var csvFile = archive.CreateEntry("sample.csv", CompressionLevel.Fastest);
                    using (var entryStream = csvFile.Open())
                    using (var fileStream = System.IO.File.OpenRead(csvFilePath))
                    {
                        fileStream.CopyTo(entryStream);
                    }

                    // Add Text file to the ZIP
                    var textFile = archive.CreateEntry("sample.txt", CompressionLevel.Fastest);
                    using (var entryStream = textFile.Open())
                    using (var fileStream = System.IO.File.OpenRead(textFilePath))
                    {
                        fileStream.CopyTo(entryStream);
                    }
                }

                // Return the ZIP file to the browser
                return File(memoryStream.ToArray(), "application/zip", "FilesBundle.zip");
            }
        }
        #endregion
    }
}
