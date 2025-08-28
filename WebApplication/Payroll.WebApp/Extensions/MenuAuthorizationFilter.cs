using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Payroll.WebApp.Helpers;
using Microsoft.Extensions.Options;
using Payroll.WebApp.Models.DTOs;
using Payroll.Common.ViewModels;

namespace Payroll.WebApp.Extensions
{
    public class MenuAuthorizationFilter : IAsyncActionFilter
    {
      
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly RestApiUserServiceHelper _userServiceHelper;
     

        public MenuAuthorizationFilter(RestApiUserServiceHelper userServiceHelper, IHttpContextAccessor httpContextAccessor, IOptions<ApiSettings> apiSettings, IMapper mapper)
        {
            _apiSettings = apiSettings.Value;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userServiceHelper = userServiceHelper;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userMenus = await MenuHelper.GetUserMenus(httpContext, _userServiceHelper, _mapper, _apiSettings);

            if (userMenus == null || !userMenus.Any())
            {
                context.Result = new RedirectToActionResult("Access_Denied", "Account", null);
                return;
            }

            // Extract controller and action names from the request
            string controllerName = context.RouteData.Values["controller"]?.ToString().ToLower();
            string actionName = context.RouteData.Values["action"]?.ToString().ToLower();
            string httpMethod = httpContext.Request.Method; // GET, POST, PUT, DELETE

            // Find the matching menu item
            var matchedMenu = userMenus.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));

            if (matchedMenu == null)
            {
                context.Result = new RedirectToActionResult("Access_Denied", "Account", null);
                return;
            }

            // Validate HTTP method access based on role permissions
            bool isAuthorized = (httpMethod == "GET" && matchedMenu.GrantView) ||
                                (httpMethod == "POST" && matchedMenu.GrantAdd ) ||
                                (httpMethod == "PUT" && matchedMenu.GrantEdit);

            if (!isAuthorized)
            {
                context.Result = new RedirectToActionResult("Access_Denied", "Account", null);
                return;
            }

            // Continue to the next action
            await next();
        }

        public async Task OnActionExecutionAsyncOLD(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            //var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(httpContext, "UserSessionData");

            //if (sessionData == null)
            //{
            //    // If session data is null, redirect to login
            //    context.Result = new RedirectToActionResult("Login", "Account", null);
            //    return;
            //}

            var menuItems = await MenuHelper.GetUserMenus(httpContext, _userServiceHelper, _mapper, _apiSettings);


            // If menu items are still null or empty, redirect to unauthorized page
            if (menuItems == null || !menuItems.Any())
            {
                context.Result = new RedirectToActionResult("Access_Denied", "Account", null);
                return;

                //context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                //return;
            }

            // Check if the current URL is allowed
           // string currentUrl = httpContext.Request.Path.Value.ToLower();
            string currentUrl = httpContext.Request.Path.Value.TrimStart('/').ToLower();
            bool hasAccess = menuItems.Any(m => m.ActionUrl.ToLower() == currentUrl);

            if (!hasAccess)
            {
                //context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                //return;

                context.Result = new RedirectToActionResult("Access_Denied", "Account", null);
                return;
            }

            // Continue execution
            await next();
        }
    }
}
