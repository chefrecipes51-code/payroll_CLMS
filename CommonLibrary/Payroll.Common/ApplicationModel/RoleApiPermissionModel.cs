using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Payroll.Common.ApplicationModel
{
    public class RoleApiPermissionModel
    {
        public string RoleName { get; set; }

        public string ApiEndpoint { get; set; }

        public bool Get { get; set; } = false;

        public bool Post { get; set; } = false;

        public bool Put { get; set; } = false;

        public bool Delete { get; set; } = false;

        public bool Patch { get; set; } = false;

    }
}
