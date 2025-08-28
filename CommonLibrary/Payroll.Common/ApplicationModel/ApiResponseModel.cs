using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Payroll.Common.ApplicationModel
{

    public class ApiResponseModel<T> 
    {
        public bool IsSuccess { get; set; }
        public T Result { get; set; }
        public string Message { get; set; }
        public int MessageType { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string RedirectUrl { get; set; }
        public object Data { get; set; }
        public object JsonResponse { get; set; }
        public int StatusCode { get; set; }
        public int? returnCount { get; set; }           // added by Abhishek
        public string AuthCode { get; set; }
        public string VerifyUserCode { get; set; }
        public string TemplateType { get; set; }
    }

   
}
