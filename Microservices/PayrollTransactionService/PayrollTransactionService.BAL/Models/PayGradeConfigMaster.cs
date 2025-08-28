using Payroll.Common.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollTransactionService.BAL.Models
{
    public class PayGradeConfigMaster : BaseModel
    {
        public int PayGradeConfig_Id { get; set; }
        public int Cmp_Id { get; set; }
        public int Correspondance_ID { get; set; }
        public string GradeConfigName { get; set; }
        public int PayGrade_Id { get; set; }
        public int Trade_Id { get; set; }
        public int SkillType_Id { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
        public string PayGradeName { get; set; }
        public string Skillcategory_Name { get; set; }
        public string Trade_Name { get; set; }

    }
}
