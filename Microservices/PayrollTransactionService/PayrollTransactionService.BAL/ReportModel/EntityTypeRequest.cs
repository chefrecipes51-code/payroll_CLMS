using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.ReportModel
{
    public class EntityTypeRequest : BaseModel
    {
        public int EntityType_Id { get; set; }
        public string? ExternalUnique_Id { get; set; }
        public string EntityType_Name { get; set; } = string.Empty;
        public bool Ot_Applicable { get; set; }
        public bool Comp_off_Applicable { get; set; }
        public int Max_Ot { get; set; }
        public int Ot_Start_After { get; set; }
        public int Ot_Start_Before { get; set; }
        public bool? Isimporterd { get; set; }
        public int? ExportLogId { get; set; }        
    }
}
