/***************************************************************************************************
 *                                                                                                 
 *  Project                 : Payroll Management System                                                        
 *  File                    : DepartmentController.cs                                                   
 *  Description             : Display Department data into list.                                            
 *                                                                                                
 *  Author                  : Harshida Parmar                                                                  
 *  Date                    : December 18, 2024                                                                 
 *  Jira Task By Harshida   : 281(First time testing Layout with Grid).
 *  
 *  © 2024 Harshida Parmar. All Rights Reserved.                                                  
 *                                                                                           
 **************************************************************************************************/

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payroll.Common.FtpUtility;
using Payroll.WebApp.Helpers;
using Payroll.WebApp.Models.DTOs;
using PayrollMasterService.BAL.Models;

namespace Payroll.WebApp.Controllers
{

    public class DepartmentMasterController : Controller
    {
        #region CTOR
        private readonly IMapper _mapper;
        private readonly ApiSettings _apiSettings;
        private readonly FtpService _ftpService;
        int DPId = 0;
        private readonly RestApiMasterServiceHelper _masterServiceHelper;
        public DepartmentMasterController(RestApiMasterServiceHelper masterServiceHelper, FtpService ftpService, IOptions<ApiSettings> apiSettings, IMapper mapper)
        {
            _ftpService = ftpService;
            _apiSettings = apiSettings.Value;
            _mapper = mapper;
            _masterServiceHelper = masterServiceHelper;
        }
        #endregion
        #region Department Record Display
        public async Task<IActionResult> Index(int? department_Id = null, bool? isActive = null)
        {
            var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllDepartmentMasterUrl;
            //var apiResponse = await RestApiMasterServiceHelper.Instance.GetAllRecordsAsync<DepartmentMaster>(apiUrl);
            var apiResponse = await _masterServiceHelper.GetAllRecordsAsync<DepartmentMaster>(apiUrl);
            if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
            {
                IEnumerable<DepartmentMasterDTO> departmentMappings = _mapper.Map<IEnumerable<DepartmentMasterDTO>>(apiResponse.Result);
                return View(departmentMappings);
            }
            else
            {
                return View(new List<DepartmentMasterDTO>());
            }
        }

        public async Task<IActionResult> TestDepartment(int? department_Id = null, bool? isActive = null)
        {
            var apiUrl = _apiSettings.PayrollMasterServiceEndpoints.GetAllDepartmentMasterUrl;
            // var apiResponse = await RestApiMasterServiceHelper.Instance.GetAllRecordsAsync<DepartmentMaster>(apiUrl);
            var apiResponse = await _masterServiceHelper.GetAllRecordsAsync<DepartmentMaster>(apiUrl);
            if (apiResponse?.IsSuccess == true && apiResponse.Result != null)
            {
                IEnumerable<DepartmentMasterDTO> departmentMappings = _mapper.Map<IEnumerable<DepartmentMasterDTO>>(apiResponse.Result);
                return View(departmentMappings);
            }
            else
            {
                return View(new List<DepartmentMasterDTO>());
            }
        }
        #endregion
    }
}
