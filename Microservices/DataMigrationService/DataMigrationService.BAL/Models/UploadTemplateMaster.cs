using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationService.BAL.Models
{
    public class UploadTemplateMaster : BaseModel
    {
        public int Template_Id { get; set; }
        public int Service_ID { get; set;}    
        public string TemplateName { get; set;}
        public string HelpFileName { get; set;}
        public string FileExtension { get; set; }
        public string TemplateFilePath { get; set; }
        public string HelpFilePath { get; set; }
    }
}
