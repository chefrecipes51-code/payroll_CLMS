/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-494                                                                 *
 *  Description:   HasPermissionAttribute                                                          *
     *      These classes help to maintain the authorize menu level                                *                                   
 *                                                                                                 *
 *  Author: Chirag Gurjar                                                                          *
 *  Date  : 20-feb-2025                                                                            *
 *                                                                                                 *
 ***************************************************************************************************/
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;

namespace Payroll.WebApp.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HasPermissionAttribute : TypeFilterAttribute
    {
        public HasPermissionAttribute(PermissionIds permissions)
            : base(typeof(HasPermissionFilter))
        {
            Arguments = new object[] { permissions };
        }
    }

    public class HasPermissionFilter : IAuthorizationFilter
    {
        public HasPermissionFilter(PermissionIds permissions)
        {
            this.Permissions = permissions;
        }

        public PermissionIds Permissions { get; set; }

        /// <param name="context"></param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                //context.Result = new UnauthorizedResult();
                //return;
            }

            //UserInfo userInfo = Helpers.Utility.UserInfo(context.HttpContext);
           
            UserInfo userInfo = new UserInfo
            {
                UserId = 79,
                UserName = "ankita",
                Permissions = new List<int> { 3, 2 } // ✅ Initialize and add values in one line
            };



            if (userInfo == null)
            {
                context.Result = new BadRequestObjectResult(new ApiResponse(440, "Session Expire"));
                return;
            }
         

            if (!userInfo.Permissions.Contains((int)this.Permissions))
            {
                context.Result = new BadRequestObjectResult(new ApiResponse(StatusCodes.Status410Gone, "You are not authorized to access this content."));
            }
        }
    }
}
