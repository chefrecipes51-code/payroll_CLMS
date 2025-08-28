using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.ApplicationConstant
{

    //
    public class RouteUrlConstants
    {
        #region Common

        public const string Master = "Master";

        /* ----- Action Name ----- */
        public const string Index = "Index";
        public const string SaveCreate = "SaveCreate";
        public const string SaveEdit = "SaveEdit";

        #endregion

        #region Login

        public const string Login = "LoginPage";

        /* ----- Url ----- */
        public const string LoginPageUrl = "https://localhost:7093/"; /* "/Login/LoginPage" */

        #endregion

        #region Home

        public const string Home = "Home";

        /* ----- Url ----- */
        public const string HomePageUrl = "/Home/Index";

        #endregion

        #region User

        public const string User = "User";

        /* ----- Action Name ----- */
        public const string ManageUser = "Manage User";

        /* ----- Url ----- */
        public const string UserPageUrl = "/User/Index";

        #endregion
    }
}
