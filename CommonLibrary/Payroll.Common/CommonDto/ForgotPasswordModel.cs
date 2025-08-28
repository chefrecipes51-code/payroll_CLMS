using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.CommonDto
{
    public class SendEmailModel
    {
        public string Email { get; set; }
        public int CompanyId {  get; set; }
        public int TemplateType { get; set; }
        public int OTP { get; set; }
        public string NewPassword { get; set; }
    }
}
