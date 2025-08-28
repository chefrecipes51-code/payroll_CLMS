using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class SkillCategory : BaseModel
    {
        public int Skillcategory_Id { get; set; }
        public char ExternalUniqueID { get; set; }
        public string Skillcategory_Name { get; set; }
        public int Company_Id { get; set; }
        public int Correspondance_ID { get; set; }
    }
}
