/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-294                                                                  *
 *  Description:   UserAuthorize                                                                    *
     *      These classes help to maintain the session                                              *                                   
 *                                                                                                  *
 *  Author: Harshida Parmar                                                                         *
 *  Date  : 24-Dec-'24                                                                              *
 *                                                                                                  *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;

namespace Payroll.WebApp.Extensions
{
    public class UserAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                var sessionModel = SessionHelper.GetSessionObject<SessionViewModel>(filterContext.HttpContext, "UserSessionData");

                if (sessionModel == null || !sessionModel.IsLoggedIn)
                {
                    // Clear session if user is not logged in or session data is missing
                    SessionHelper.ClearSession(filterContext.HttpContext);

                    // Set cache headers to prevent browser caching
                    filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
                    filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";
                    filterContext.HttpContext.Response.Headers["Expires"] = "-1";


                    // Redirect to the login page
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "action", "LoginPage" },
                        { "controller", "Account" }
                    });

                    bool isAjaxCall = filterContext.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";

                    if (isAjaxCall)
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                        {
                            { "action", "Logout" },
                            { "controller", "Account" }
                        });
                    }

                    return;
                }
            }
            catch (Exception)
            {
                // If an exception occurs, redirect to Login
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "action", "LoginPage" },
                { "controller", "Account" }
            });
            }
        }
    }


}
