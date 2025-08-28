using System.ComponentModel.DataAnnotations;

namespace Payroll.Common.CommonDto
{
    public class ErrorLogMaster
    {
        [Key]
        public long LogId { get; set; }

        public byte LogType { get; set; }

        public string LogMessage { get; set; }

        public byte SeverityLevel { get; set; }

        public int ErrorCode { get; set; }
        public int ErrorLine { get; set; }

        public string Source { get; set; }

        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }

        public string StackTrace { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string UserAgent { get; set; }
    }
}
