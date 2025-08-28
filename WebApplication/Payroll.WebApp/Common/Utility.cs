using System.Data;
using System.Reflection;

namespace Payroll.WebApp.Common
{
    /// <summary>
    /// Prepared By:- Harshida Parmar 
    /// Date:- 15-11-'24
    /// Note:- Convert List into Database to we can use this class later when our SP use
    ///         Parent-Child Record entry using @UserDefineTable 
    /// </summary>
    public static class Utility
    {
        #region NotEmptyNotNA
        /// <summary>
        /// Created By:- Harshida Parmar
        /// Created Date:- 21-11-'24        
        /// Updated By:-
        /// Updated Date:-....
        /// Updated Note:- 
        /// </summary>
        /// <param name="inStr"></param>
        /// <returns></returns>
        public static bool NotEmptyNotNA(string inStr)
        {
            return !string.IsNullOrEmpty(inStr)
                   && !string.IsNullOrWhiteSpace(inStr)
                   && !inStr.Equals("NA", StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}
