using AutoMapper;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;

public class UserPermissionFilter : IAsyncActionFilter
{
    private readonly RestApiUserServiceHelper _userServiceHelper;
    private readonly IMapper _mapper;
    private readonly ApiSettings _apiSettings;

    public UserPermissionFilter(
        RestApiUserServiceHelper userServiceHelper,
        IMapper mapper,
        ApiSettings apiSettings)
    {
        _userServiceHelper = userServiceHelper;
        _mapper = mapper;
        _apiSettings = apiSettings;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var menuItems = await MenuHelper.GetUserMenus(httpContext, _userServiceHelper, _mapper, _apiSettings);
        string controllerName = context.RouteData.Values["controller"]?.ToString().ToLower();
        var matchedMenu = menuItems.FirstOrDefault(m => m.ActionUrl.ToLower().Contains(controllerName));

        if (context.Controller is Controller controller)
        {
            controller.ViewBag.UserPermissions = matchedMenu ?? new UserRoleBasedMenuDTO();
        }

        await next(); // Continue execution
    }
}