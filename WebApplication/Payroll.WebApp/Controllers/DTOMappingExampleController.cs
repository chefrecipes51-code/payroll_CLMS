using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payroll.Common.ApplicationConstant;
using Payroll.Common.ApplicationModel;
using Payroll.Common.EnumUtility;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models;
using PayrollMasterService.BAL.Models;
using System.Net.Http;
using System.Text;
using UserService.BAL.Models;

namespace Payroll.WebApp.Controllers
{
    /// <summary>
    /// Developed By:- Harshida Parmar
    /// Created Date:- 14-11-'24
    /// Note:- This is the SAMPLE FOR PARENT CHILD INSERT AT ONE GO.
    /// </summary>
    public class DTOMappingExampleController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly HttpClient _httpClient;

        public DTOMappingExampleController(IOptions<ApiSettings> apiSettings, IMapper mapper, HttpClient httpClient)
        {
            _apiSettings = apiSettings.Value;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            #region THIS IS SAMPLE DATA WORKS FINE FOR TESTING
            //var responseModel = new ApiResponseModel<RoleMenuMappingRequestDTO>
            //{
            //    IsSuccess = false
            //};

            //var roleMenuRequestDto = new RoleMenuMappingRequestDTO
            //{
            //    Header = new RoleMenuMappingHeaderDTO
            //    {
            //        Role_Menu_Hdr_Id = 0,
            //        Role_Id = 1,  // Corrected RoleId
            //        //Company_Id = 1,
            //        EffectiveFrom = DateTime.Now,
            //        CreatedBy = 1
            //    },
            //    Details = new List<RoleMenuDetailDTO>
            //    {
            //        new RoleMenuDetailDTO
            //        {
            //            Menu_Id = 11,
            //            Company_Id = 3
            //        },
            //        new RoleMenuDetailDTO
            //        {
            //            Menu_Id = 22,
            //            Company_Id = 3
            //        }
            //    },
            //    MessageType = (int)EnumUtility.ApplicationMessageTypeEnum.Information,
            //    MessageMode = (int)EnumUtility.ApplicationMessageModeEnum.Insert,
            //    ModuleId = (int)EnumUtility.ModuleEnum.RoleMenuMapping
            //};

            //var roleMenuRequestEntity = _mapper.Map<RoleMenuMappingRequest>(roleMenuRequestDto);

            //try
            //{
            //    var roleMenuDetailApiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostRoleMenuDetailUrl;
            //    var jsonContent = JsonConvert.SerializeObject(roleMenuRequestEntity);
            //    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            //    var response = await _httpClient.PostAsync(roleMenuDetailApiUrl, content);
            //    var apiResponse = await response.Content.ReadAsStringAsync();

            //    var result = JsonConvert.DeserializeObject<ApiResponseModel<RoleMenuMappingRequestDTO>>(apiResponse);

            //    if (result?.IsSuccess == true)
            //    {
            //        responseModel.IsSuccess = true;
            //        responseModel.RedirectUrl = RouteUrlConstants.LoginPageUrl;
            //        responseModel.Message = MessageConstants.PasswordChangeSuccess;
            //        return Json(responseModel);
            //    }
            //    else
            //    {
            //        responseModel.Message = result?.Message ?? "An error occurred.";
            //        return Json(responseModel);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    responseModel.Message = "An error occurred while processing your request.";
            //    return Json(responseModel);
            //}
            #endregion
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] RoleMenuMappingRequestDTO roleMenuRequestDto)
        {
            var responseModel = new ApiResponseModel<RoleMenuMappingRequestDTO>
            {
                IsSuccess = false
            };

            var roleMenuRequestEntity = _mapper.Map<RoleMenuMappingRequest>(roleMenuRequestDto);

            try
            {
                var roleMenuDetailApiUrl = _apiSettings.PayrollMasterServiceEndpoints.PostRoleMenuDetailUrl;
                var jsonContent = JsonConvert.SerializeObject(roleMenuRequestEntity);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(roleMenuDetailApiUrl, content);
                var apiResponse = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<ApiResponseModel<RoleMenuMappingRequestDTO>>(apiResponse);

                if (result?.IsSuccess == true)
                {
                    responseModel.IsSuccess = true;
                    responseModel.RedirectUrl = RouteUrlConstants.LoginPageUrl;
                    responseModel.Message = MessageConstants.PasswordChangeSuccess;
                    return Json(responseModel);
                }
                else
                {
                    responseModel.Message = result?.Message ?? "An error occurred.";
                    return Json(responseModel);
                }
            }
            catch (Exception ex)
            {
                responseModel.Message = "An error occurred while processing your request.";
                return Json(responseModel);
            }
        }

    }
}
