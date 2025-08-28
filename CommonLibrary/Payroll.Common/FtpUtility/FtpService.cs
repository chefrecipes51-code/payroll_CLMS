using System.Net;

namespace Payroll.Common.FtpUtility
{
    public class FtpService
    {
        //public async Task UploadFileAsync(string localFilePath, string ftpFileName)
        //{
        //    string ftpUri = $"{FtpSettings.Host}/{ftpFileName}";

        //    var request = (FtpWebRequest)WebRequest.Create(ftpUri);
        //    request.Method = WebRequestMethods.Ftp.UploadFile;
        //    request.Credentials = new NetworkCredential(FtpSettings.Username, FtpSettings.Password);

        //    using (var fileStream = new FileStream(localFilePath, FileMode.Open))
        //    using (var requestStream = await request.GetRequestStreamAsync())
        //    {
        //        await fileStream.CopyToAsync(requestStream);
        //    }
        //}
        public async Task<FileUploadResponse> UploadFileAsync(string localFilePath, string ftpFileName)
        {
            //string ftpUri = $"{FtpSettings.Host}/{ftpFileName}";
            string ftpUri = $"{FtpSettings.Host}{ftpFileName}";

            try
            {
                var request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(FtpSettings.Username, FtpSettings.Password);

                using (var fileStream = new FileStream(localFilePath, FileMode.Open))
                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    await fileStream.CopyToAsync(requestStream);
                }
                // If everything succeeds, return true
                return new FileUploadResponse(true, ftpUri);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error uploading file to FTP: {ex.Message}");
                // Return false in case of failure
                return new FileUploadResponse(false, null);
            }
        }

    }
    public class FileUploadResponse
    {
        public bool IsSuccess { get; set; }
        public string FtpFilePath { get; set; }

        // Constructor for easy initialization
        public FileUploadResponse(bool isSuccess, string ftpFilePath)
        {
            IsSuccess = isSuccess;
            FtpFilePath = ftpFilePath;
        }
    }

}
