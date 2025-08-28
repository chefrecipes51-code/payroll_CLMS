/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-323                                                                  *
 *  Description:   HeaderOffcanvasViewComponent                                                     *
     *      These classes help to bind Header Offcanvas                                             *                                   
 *                                                                                                  *
 *  Author: Harshida Parmar                                                                         *
 *  Date  : 24-Dec-'24                                                                              *
 *                                                                                                  *
 ****************************************************************************************************/
using Microsoft.AspNetCore.Mvc;
using Payroll.Common.Helpers;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;
using UserService.BAL.Requests;


namespace Payroll.WebApp.Extensions
{
    public class HeaderOffcanvasViewModel
    {
        public SessionViewModel SessionData { get; set; }
        public UserSessionViewModel SessionUserData { get; set; }
    }
    public class HeaderOffcanvasViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
            var sessionUserData = SessionHelper.GetSessionObject<UserSessionViewModel>(HttpContext, "UserDetailData");

            if (sessionData == null)
            {
                // If no session data is found, return an empty SessionViewModel
                return await Task.FromResult((IViewComponentResult)View("HeaderOffcanvasViewComponent", new UserSessionViewModel()));
            }
            //string encryptedId = SingleEncryptionHelper.Encrypt(id);
            //sessionData.Username = sessionData.Username;
            //// sessionData.IsLoggedIn = sessionData.IsLoggedIn;
            //sessionUserData.CompanyDetails = sessionUserData.CompanyDetails;
            //sessionUserData.LocationDetails = sessionUserData.LocationDetails;
            //sessionUserData.RoleDetails = sessionUserData.RoleDetails;
            //return await Task.FromResult((IViewComponentResult)View("HeaderOffcanvasViewComponent", sessionData));
            var model = new HeaderOffcanvasViewModel
            {
                SessionData = sessionData,
                SessionUserData = sessionUserData
            };
            if (int.TryParse(sessionData.UserId, out int userId) && userId > 0)
            {
                string encryptedId = SingleEncryptionHelper.Encrypt(userId.ToString());
                string profileUrl = Url.Action("UserProfile", "User", new { userId = encryptedId, fromProfile = false });
                sessionUserData.ProfilePath = profileUrl;
            }


            return await Task.FromResult((IViewComponentResult)View("HeaderOffcanvasViewComponent", model));
        }
    }
}

