/// <summary>
/// Developer: Harshida Dilipbhai Parmar  
/// Date: 04-Apr-2025
/// 
/// IMPORTANT NOTE:
/// Reason for creating a new base controller instead of using the existing "BaseController":
/// The existing "BaseController" depends on "ICompositeViewEngine", which is currently not 
/// required in this application. However, some of its methods are still needed, particularly 
/// in the AccountController.
/// 
/// To avoid affecting the existing BaseController and its dependencies, a new controller 
/// has been created with only the required functionalities.
/// 
/// Approved By: Purojit Bhar  
/// Approval Date: 04-Apr-2025
/// </summary>

using Microsoft.AspNetCore.Mvc;
using Payroll.Common.Helpers;
using Payroll.Common.ViewModels;
using Payroll.WebApp.Helpers;

namespace Payroll.WebApp.Controllers
{
    public class SharedUtilityController : Controller
    {
        [HttpGet]
        public IActionResult EncryptId(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("Invalid ID");

            string encryptedId = SingleEncryptionHelper.Encrypt(id);
            return Ok(encryptedId);
        }

        public int SessionUserId
        {
            get
            {
                var sessionData = SessionHelper.GetSessionObject<SessionViewModel>(HttpContext, "UserSessionData");
                return int.TryParse(sessionData?.UserId, out var parsedUserId) ? parsedUserId : 0;
            }
        }


    }
}
