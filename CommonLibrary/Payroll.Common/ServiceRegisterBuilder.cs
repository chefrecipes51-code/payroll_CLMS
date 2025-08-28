using Payroll.Common.Repository.Interface;
using Payroll.Common.Repository.Service;

namespace Payroll.Common
{
    public static class ServiceRegisterBuilder
    {
        public static void ServiceRegister(this IServiceCollection services)
        {

            //services.AddSingleton<DapperContext.DapperContext>();
            services.AddScoped<IErrorLogRepository, ErrorLogServiceRepository>();
            services.AddScoped<IRolePermmison,RolePermisionRepository>();
            
        }
    }
}
