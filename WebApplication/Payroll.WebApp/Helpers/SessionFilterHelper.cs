using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Payroll.Common.ViewModels;


namespace Payroll.WebApp.Helpers
{

    public class SessionFilterHelper : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Retrieve SessionViewModel from the session
            var sessionModel = SessionHelper.GetSessionObject<SessionViewModel>(context.HttpContext, "UserSessionData");

            // Check if the user is logged in
            if (sessionModel == null || !sessionModel.IsLoggedIn)
            {
                // Redirect to Login Page
                context.Result = new RedirectToActionResult("LoginPage", "Login", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // No post-execution logic needed
        }
    }


}
