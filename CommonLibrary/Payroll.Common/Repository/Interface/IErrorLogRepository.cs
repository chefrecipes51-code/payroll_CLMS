using Payroll.Common.ApplicationModel;
using Payroll.Common.CommonDto;

namespace Payroll.Common.Repository.Interface
{
    public interface IErrorLogRepository
    {
        public Task ErrorLogAsync(Exception exception, HttpContext context);
    }
}
