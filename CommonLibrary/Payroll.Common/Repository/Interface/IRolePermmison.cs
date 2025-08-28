using Payroll.Common.ApplicationModel;

namespace Payroll.Common.Repository.Interface
{
    public interface IRolePermmison
    {
        Task<RoleApiPermissionModel> RolePermissionMappingAsync(string RoleName, string ApiEndpoint);
    }
}
