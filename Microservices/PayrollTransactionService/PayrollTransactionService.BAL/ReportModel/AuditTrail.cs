using Dapper;
using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class AuditTrail : BaseModel
    {
        static AuditTrail()
        {
            // Set up custom mappings between SQL columns and class properties
            SqlMapper.SetTypeMap(typeof(AuditTrail), new CustomPropertyTypeMap(
                typeof(AuditTrail),
                (type, columnName) => type.GetProperty(columnName.Replace(" ", string.Empty))
            ));
        }
        public string? CompanyName { get; set; }
        public string? TableName { get; set; }
        public string? Operation { get; set; }
        //public DateTime ChangeDate { get; set; }
        //public string ChangeTime => ChangeDate.ToString("HH:mm:ss"); // Calculated property for time only
        public string? ChangedDate { get; set; }  // Store date part only
        public string? ChangedTime { get; set; }  // Store time part only
        public string? UserName { get; set; }

        #region Passing below Field as Parameter To SP "dbo.Sp_Select_Audit_Trail" 
        public int? CompanyId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        #endregion
    }
}
