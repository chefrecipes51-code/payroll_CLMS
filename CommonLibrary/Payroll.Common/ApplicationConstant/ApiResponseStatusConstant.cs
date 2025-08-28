using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.ApplicationConstant
{
    public class ApiResponseStatusConstant
    {
        public const int Ok = 200; // HTTP Status for OK code.
        public const int NotFound = 404;  // HTTP Status for Not Found code.
        public const int BadRequest = 400; // HTTP Status for Bad Request code.
        public const int Created = 201; // HTTP Status for Created code.
        public const int NoContent = 204; // HTTP Status for No Content code.
        public const int InternalServerError = 500;  // HTTP Status for Internal Server Error code.
        public const int Unauthorized = 401;  // Unauthorized.
    }
}
