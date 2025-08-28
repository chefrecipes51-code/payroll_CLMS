using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.CommonDto
{
    public class ResponseModel
    {
        public string ApplicationMessage { get; set; }
        public int ApplicationMessageType { get; set; }
        public int IsSuccessType { get; set; }
    }
}
