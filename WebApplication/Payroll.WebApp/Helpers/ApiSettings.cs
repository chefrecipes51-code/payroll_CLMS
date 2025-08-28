/***************************************************************************************************
 *                                                                                                 
 *  Project    : Payroll Management System                                                        
 *  File       : ApiSettings.cs                                                   
 *  Description: Setting up the url in combine with baseurl through Appsettings.cs file.                                               
 *                                                                                                
 *  Author     : Harshida Parmar                                                                  
 *  Date       : Nov 13, 2024                                                                 
 *                                                                                                
 *  © 2024 Harshida Parmar. All Rights Reserved.                                                  
 *                                                                                                
 **************************************************************************************************/
namespace Payroll.WebApp.Helpers
{
    /// <summary>
    /// Prepared By:- Harshida Parmar
    /// Created Date:- 13-11-'24
    /// Note:- Setting up the url in combine with baseurl through Appsetting.cs file
    /// Updated By:- ...
    /// Updated Date:- ...
    /// </summary>
    public class ApiSettings
    {
        public string BaseUrl { get; set; }
        public string BaseUrlUserAuthService { get; set; }
        public string BaseUrlPayrollMasterService { get; set; }
        public string BaseUrlPayrollUserService { get; set; }
        public string BaseUrlPayrollDataMigrationService { get; set; }
        public string BaseUrlPayrollTransactionService { get; set; }
        public ApiEndpoints UserAuthServiceEndpoints => new ApiEndpoints(BaseUrlUserAuthService);
        public ApiEndpoints PayrollMasterServiceEndpoints => new ApiEndpoints(BaseUrlPayrollMasterService);
        public ApiEndpoints PayrollTransactionEndpoints => new ApiEndpoints(BaseUrlPayrollTransactionService);
        public ApiEndpoints BaseUrlPayrollUserServiceEndpoints => new ApiEndpoints(BaseUrlPayrollUserService);
        public ApiEndpoints BaseUrlPayrollDataMigrationServiceEndpoints => new ApiEndpoints(BaseUrlPayrollDataMigrationService);

        public class ApiEndpoints
        {
            private readonly string _baseUrl;

            public ApiEndpoints(string baseUrl)
            {
                _baseUrl = baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/";
            }
            #region Transaction Service 
            public string PostPayrollMonthDetailUrl => _baseUrl + "postpaymaster";
            public string PostAttendancePayrollMonthDetailUrl => _baseUrl + "postattendancepaymaster";
            public string PostPayComponentsUrl => _baseUrl + "postpaycomponentmaster";
            public string PutPayComponentsUrl => _baseUrl + "updatepaycomponentmaster";
            public string GetByIdPayComponentsUrl => _baseUrl + "getpaycomponentmasterbyid";
            public string GetAllPayComponentsUrl => _baseUrl + "getallpaycomponentmaster";
            public string GetPayComponentsChildUrl => _baseUrl + "getpaycomponentchildmaster";

            
            public string GetAllActivePayComponentsUrl => _baseUrl + "getallactivepaycomponentmaster";//29-04-25
            public string GetActiveInActivePayComponentsUrl => _baseUrl + "activateallpaycomponent";
            public string PutPayComponentActivationUrl => _baseUrl + "updatepaycomponentfromformula";//29-04-25
            public string DeletePayComponentsUrl => _baseUrl + "deletepaycomponentmaster";
            public string GetPayrollMonthGroupUrl => _baseUrl + "getallpaymonth";
            public string GetAttendancePayrollMonthGroupUrl => _baseUrl + "GetAllAttendancePayMonth";
            public string GetPayrollMonthBySdateUrl => _baseUrl + "getallpaymonthwithsdate";
            public string GetAttendancePayrollMonthBySdateUrl => _baseUrl + "getallattendancepaymonthwithsdate";

            public string GetAllPayGradeUrl => _baseUrl + "getallpaygrademaster";
            public string PostPayGradesUrl => _baseUrl + "postpaygrademaster";
            public string PutPayGradesUrl => _baseUrl + "updatepaygrademaster";
            public string GetByIdPayGradesUrl => _baseUrl + "getpaygrademasterbyid";
            public string DeletePayGreadsUrl => _baseUrl + "deletepaygrademaster";
            public string GetAllActivePayGreadsUrl => _baseUrl + "getallactivepaygrademaster";
            public string GetDistinctLocationUrl => _baseUrl + "getdistinctlocationbyid";
            public string GetSkillCategoryUrl => _baseUrl + "getallskillcategory";
            public string GetTradeMasterUrl => _baseUrl + "getalltrademaster";
            public string GetAllPayGradeConfigUrl => _baseUrl + "getallpaygradeconfigmaster";
            public string GetPreviousMonthYearCompanyUrl => _baseUrl + "get-previous-period";
            public string PostPayGradesConfigUrl => _baseUrl + "postpaygradeconfigmaster";
            public string PutPayGradesConfigUrl => _baseUrl + "updatepaygradeconfigmaster";
            public string GetByIdPayGradeConfigUrl => _baseUrl + "getpaygradeconfigmasterbyid";
            public string DeletePayGreadConfigUrl => _baseUrl + "deletepaygradeconfigmaster";
            public string GetAllTaxSlabUrl => _baseUrl + "getalltaxslabmaster";
            public string GetAllTaxregimeUrl => _baseUrl + "getalltaxregimemaster";
            public string PostTaxSlabUrl => _baseUrl + "posttaxslabmaster";
            public string PutTaxSlabUrl => _baseUrl + "updatetaxslabmaster";
            public string GetByIdTaxSlabUrl => _baseUrl + "gettaxslabmasterbyid";
            public string DeleteTaxSlabUrl => _baseUrl + "deletetaxslabMaster";           
            public string GetAllEntityFilterRequestUrl => _baseUrl + "getallentityfilterrequest";
            public string GetAllFinancialYearRequestUrl => _baseUrl + "getallfinancialyear";
            public string PostAssignTaxRegimeRequestUrl => _baseUrl + "postmapentitytaxregime";
            public string GetAllPTaxSlabUrl => _baseUrl + "getallptakslab";
            public string GetTaxParamUrl => _baseUrl + "getselecttaxparam";
            public string GetByIdPTaxSlabUrl => _baseUrl + "getallptakslabbyid";
            public string PostPTaxSlabUrl => _baseUrl + "postptaxslab";
            public string PutPTaxSlabUrl => _baseUrl + "updateptaxslab";
            public string DeletePTaxSlabUrl => _baseUrl + "deleteptaxslab";
            public string GetAllEntityTypeUrl => _baseUrl + "get-all-entity-type";
            public string GetAllPayrollGlobalUrl => _baseUrl + "get-global-payroll-parameters";
            public string PostPayrollGlobalParameterUrl => _baseUrl + "postpayrollglobal";
            public string PostCopyFromOneToAnotherCompanyUrl => _baseUrl + "copy-payroll-parameter";
            public string PutPayrollGlobalParameterUrl => _baseUrl + "updateglobalparam";
            public string PostPayrollComplianceUrl => _baseUrl + "postpayrollcompliance";
            public string PutPayrollComplianceUrl => _baseUrl + "updatepayrollcompliance";

            public string PostPayrollSlipUrl => _baseUrl + "postpayrollsetting";
            public string PutPayrollSlipUrl => _baseUrl + "updatepayrollsetting";

            public string GetSalaryStructureGridUrl => _baseUrl + "getallsalarystructure";
            public string GetSalaryStructureByIdUrl => _baseUrl + "getsalarystructurebyid";
            public string PostSalaryStructureUrl => _baseUrl + "postsalarystructure";
            public string CalculateSalaryStructureUrl => _baseUrl + "calculatesalarystructure";
            public string PutSalaryStructureUrl => _baseUrl + "putsalarystructure";
            public string DeleteSalaryStructureUrl => _baseUrl + "deletesalarystructure";
            public string GetAllContractorUrl => _baseUrl + "getContractorMasterbyid";
            public string GetProfileContractorUrl => _baseUrl + "GetContractorProfileByCompanyIdAsync";

            public string GetAllContractorDetailUrl => _baseUrl + "getallcontractormaster";
            
            public string GetAllWorkOrderDetailUrl => _baseUrl + "getWorkOrderDataByContractor";
            public string GetContractorWithWorkOrderUrl => _baseUrl + "getContractorWithWorkOrder";
            public string PostThirdPartyUrl => _baseUrl + "postthirdpartyparameter";
            public string PutThirdPartyUrl => _baseUrl + "updatethirdpartyparameter";
            public string PutMapGradeEntityUrl => _baseUrl + "updatemapgradeentity";
            public string GetAllEntityDetailUrl => _baseUrl + "getallentitymaster";
            public string UpdateEntityComplianceDetailUrl => _baseUrl + "updateentitycompliance";
            public string GetAllEntityDataValidationDetailUrl => _baseUrl + "getallentitydatavalidation";
            public string GetAllCompanyPayrollValidationDetailUrl => _baseUrl + "getallcompanypayrollvalidation";
            public string GetAllCompanyLocationPayrollValidationDetailUrl => _baseUrl + "getallcompanylocationpayrollvalidation";
            public string GetAllWorkOrderPayrollValidationDetailUrl => _baseUrl + "getallworkorderpayrollvalidation";
            public string GetAllContractorPayrollValidationDetailUrl => _baseUrl + "getallcontractorpayrollvalidation";
            public string GetAllPreviousMonthYearPeriod_ByCmpIdDetailUrl => _baseUrl + "getallpreviousmonthyearperiodbycmpId";
            public string PostPayrollTransDataForProcessDetailUrl => _baseUrl + "postpayrolltransdataforprocess";
            public string PostStartPayrollProcessDetailUrl => _baseUrl + "poststartpayrollprocess";
            public string GetAllPayrollProcessusingSignalRDetail => _baseUrl + "getallpayrollprocessusingsignalR";
            public string GetPhaseWiseComponentPayrollProcessDetail => _baseUrl + "getphasewisecomponentpayrollprocess";
            public string GetContractorsUrl => _baseUrl + "getcontractorvalidation";
            public string GetContractorEntityUrl => _baseUrl + "getentityvalidation";
            public string GetPayCalculationUrl => _baseUrl + "getentitypayvalidation";
            public string GetComplianceUrl => _baseUrl + "getentitycompliance";
            public string GetAttendanceUrl => _baseUrl + "getentityattendance";
            public string UpdateValidateEntityUrl => _baseUrl + "validate-entities";
            public string UpdateValidateContractorsUrl => _baseUrl + "validate-contractors";
            public string UpdateValidatePayCalUrl => _baseUrl + "validate-pay-structure";
            public string UpdateValidatecomplianceUrl => _baseUrl + "validate-entity-compliance";
            public string UpdateValidateAttendanceUrl => _baseUrl + "validate-entity-attendance";
            public string AddPayrollTranStgDataUrl => _baseUrl + "save-payroll-staging";
           
            #endregion

            #region UserAuth Micro Service
            public string AuthUrl => _baseUrl + "auth/token";

            //public string GetAllUserUrl => _baseUrl + "UserApi/getalluser";
            public string GetBreadCrumbsUrl => _baseUrl + "getbreadcrumbbymenuid";
            public string GetAllUserUrl => _baseUrl + "UserApi/getusers"; // Modify By Harshida From getalluser to getusers (30-12-'24)
            public string GetUserListUrl => _baseUrl + "UserApi/getuserslist"; // chirag
            public string GetLocationwiseUserListUrl => _baseUrl + "UserApi/getlocationwiseuserslist"; // chirag
            public string PostUserUrl => _baseUrl + "UserApi/adduser";
            public string GetById => _baseUrl + "UserApi/getbyid";
            public string GetByIdAuth => _baseUrl + "UserApi/getbyidauth";
            public string UpdateLoginActivity => _baseUrl + "UserApi/updateloginactivity";
            public string UpdateUserPassword => _baseUrl + "UserApi/updateuserpwd";
            public string GetUserMapDetailsById => _baseUrl + "UserApi/getusermapdetailsbyid";
            public string GetEditUserMapDetailsById => _baseUrl + "UserApi/geteditusermapdetailsbyid";
            public string PutUserUrl => _baseUrl + "UserApi/updateuser";
            public string DeactivateUserUrl => _baseUrl + "UserApi/deactivateuser";
            public string DeleteUserUrl => _baseUrl + "UserApi/deleteuser";
            public string DeleteUserByIdUrl => _baseUrl + "UserApi/deleteUserById";
            public string PostVerificationForResetPasswordUrl => _baseUrl + "UserApi/verifyresetpasswordlink";
            public string UpdateUserRoleStatusMaster => _baseUrl + "UserApi/updateuserrolestatusmaster";
            public string UpdateUserLocationStatusMaster => _baseUrl + "UserApi/updateuserlocationstatusmaster";
            



            public string GetAllWageGradeDetailUrl => _baseUrl + "getallwagegradedetail";
            public string VerifyOTPUrl => _baseUrl + "UserApiService/verifyotp";
            public string AuthConfigUrl => _baseUrl + "UserApiService/authconfig";
            public string ChangePasswordUrl => _baseUrl + "UserApi/updateuserpassword";
            public string PostOrPutUserRoleMenuUrl => _baseUrl + "UserApi/AddOrUpdateUserRoleMenu"; //Added By Harshida 17-01-'25
            public string PostLoginHistoryUrl => _baseUrl + "UserApi/postloginhistory"; // Added by Harshida 27-03-'25
            public string PutLoginHistoryUrl => _baseUrl + "UserApi/putloginhistory";  // Added by Harshida 27-03-'25

            #endregion

            #region PayrollMaster Service
            public string GetPayrollDataUrl => _baseUrl + "payroll/data";
            public string GetWageGradeDetailsUrl => _baseUrl + "payroll/wagegrade";
            public string PostRoleMenuDetailUrl => _baseUrl + "postrolemenudetails";
            public string GetAllCompanyMasterDetailUrl => _baseUrl + "getallcompanymasterapi";
            public string GetAllCityWiseLocationMasterDetailUrl => _baseUrl + "getallcitywiselocationmaster";
            public string GetAllLocationMasterDetailUrl => _baseUrl + "getalllocationmaster";
            public string GetAllStateMasterDetailUrl => _baseUrl + "getallstatemaster";
            public string GetAllCityMasterDetailUrl => _baseUrl + "getallcitymaster";
            public string GetAllRoleMasterDetailUrl => _baseUrl + "getallrolemaster";
            public string GetCompanylocationmapDetailUrl => _baseUrl + "companylocationmap";
            public string GetCompanyDetailsByIdUrl => _baseUrl + "getcompanymasterbyid"; //Added By Hashida [24-01-'25][These method exist in Payroll Master Web Service]
            public string PostCompanyMasterDetailsUrl => _baseUrl + "postcompanymaster";
            public string PutCompanyMasterDetailsUrl => _baseUrl + "updatecompanymaster";  //Added By Hashida [07-02-'25][These method exist in Payroll Master Web Service]
            public string GetCompanyDemographicDetailsUrl => _baseUrl + "getcompanydemographicmasterbyid";  //Added By Hashida [07-02-'25][These method exist in Payroll Master Web Service]
            public string GetUserRoleMenuByroleIdUrl => _baseUrl + "getuserrolemenubyroleId";
            public string GetAllAreaMasterUrl => _baseUrl + "getallareamaster";
            public string PostAreaMasterDetailUrl => _baseUrl + "postareamaster";
            public string GetAreaMasterDetailByIdUrl => _baseUrl + "getareamasterbyid";
            public string PutAreaMasterDetailUrl => _baseUrl + "updateareamaster";
            public string DeleteAreaMasterDetailUrl => _baseUrl + "deleteareamaster";
            public string GetAreaLocationMasterDetailByIdUrl => _baseUrl + "getarealocationmasterbyid";

            public string GetAllLocationMasterUrl => _baseUrl + "getalllocationmaster";
            public string PostLocationMasterDetailUrl => _baseUrl + "postlocationmaster";
            public string GetLocationMasterDetailByIdUrl => _baseUrl + "getlocationmasterbyid";
            public string PutLocationMasterDetailUrl => _baseUrl + "updatelocationmaster";
            public string DeleteLocationMasterDetailUrl => _baseUrl + "deletelocationmaster";
            public string GetCompanyCorrespondanceByIdUrl => _baseUrl+"getcompanycorrespondancebyid";
            public string GetCorrespondanceByCompanyIdUrl => _baseUrl + "getcompanycorrespondancebyCompanyid";

            public string PutCompanyCorrespondanceMasterUrl => _baseUrl + "putcompanycorrespondancemaster";
            public string PostCompanyConfigurationUrl => _baseUrl + "postcompanyconfiguration";//Added By Harshida 17-02-25
            public string PostCompanyCorrespondanceMasterUrl => _baseUrl + "postcompanycorrespondancemaster";
            public string GetAllCompanyTypeUrl => _baseUrl + "getallcompanytype"; //Added By Harshida 06-02-25
            public string GetAllCompanyCurrencyUrl => _baseUrl + "getallcurrency"; //Added By Harshida 14-02-25
            public string GetAllCompanyListUrl => _baseUrl + "getallcompany"; //Added By Harshida 17-02-25
            public string GetAllDepartmentMasterUrl => _baseUrl + "getalldepartmentmaster";
            public string PostDepartmentMasterDetailUrl => _baseUrl + "postdepartmentmaster";
            public string GetDepartmentMasterDetailByIdUrl => _baseUrl + "getdepartmentmasterbyid";
            public string PutDepartmentMasterDetailUrl => _baseUrl + "updatedepartmentmaster";

            public string GetAllMapDepartmentLocationUrl => _baseUrl + "getallmapdepartmentlocation";
            public string GetMapDepartmentLocationByIdUrl => _baseUrl + "getmapdepartmentlocationbyid";
            public string PostMapDepartmentLocationDetailUrl => _baseUrl + "postmapdepartmentlocation";
            public string PutMapDepartmentLocationDetailUrl => _baseUrl + "updatemapDepartmentLocation";
            public string DeleteDepartmentMasterDetailUrl => _baseUrl + "deletedepartmentmaster";
            public string DeleteMapDepartmentMasterDetailUrl => _baseUrl + "deletemapDepartmentLocation";
            public string GetAllFloorMasterUrl => _baseUrl + "getallfloormaster";
            public string GetFloorMasterByIdUrl => _baseUrl + "getfloormasterbyid";
            public string PostMapUserLocationDetail => _baseUrl + "postmapuserlocation";

            public string PostAccountingHeadDetailUrl => _baseUrl + "postgeneralaccountinghead";
            public string GetAccountingHeadDetailUrl => _baseUrl + "getallgeneralaccountinghead";
            public string GetAccountingHeadDetailByIdUrl => _baseUrl + "getgeneralaccountingheadbyid";
            public string PutAccountingHeadDetailUrl => _baseUrl + "updategeneralaccountinghead";
            public string DeleteAccountingHeadDetailUrl => _baseUrl + "deletegeneralaccountinghead";
            public string GetAllAccountingTypeUrl => _baseUrl + "getgeneralaccounttypes";

            public string PostGLGroupUrl => _baseUrl + "postglgroup";
            public string GetGLGroupUrl => _baseUrl + "getallglgroups";
            public string GetGLGroupByIdUrl => _baseUrl + "getglgroupbyid";
            public string PutGLGroupUrl => _baseUrl + "updateglgroup";
            public string DeleteGLGroupUrl => _baseUrl + "deleteglgroup";
            public string GetParentGroupUrl => _baseUrl + "getparentsubglgroups";


            public string PostSubsidiaryMasterUrl => _baseUrl + "postsubsidiarymaster"; // added by krunali gohil 06-02-2025
			public string GetSubsidiaryMasterUrl => _baseUrl + "getallsubsidiarymaster"; // added by krunali gohil 07-02-2025
			public string DeleteSubsidiaryMasterUrl => _baseUrl + "deletesubsidiarymaster"; // added by krunali gohil 07-02-2025

			public string GetSubsidiaryMasterByIdUrl => _baseUrl + "getsubsidiarymasterbyid"; // added by krunali gohil 07-02-2025
			public string PutSubsidiaryMasterUrl => _baseUrl + "updataesubsidiarymaster"; // added by krunali gohil 07-02-2025
			public string GetAllSalaryStructureByIdUrl => _baseUrl + "getallsalarystructure"; // added by krunali gohil 07-02-2025
            
            #endregion

            #region User Service
            public string PostGetGenerateAPIUrl => _baseUrl + "generate";
            public string PostUserRoleMappingUrl => _baseUrl + "postuserrolemaster";
            public string GetUserRoleMappingsUrl => _baseUrl + "getalluserrole";
            public string GetBreadcrumbByMenuIdUrl => _baseUrl + "getbreadcrumbbymenuid";
            public string GetUserRoleMenuUrl => _baseUrl + "getalluserrolemenu";
            public string GetUserRoleMenuEditUrl => _baseUrl + "getalluserrolemenuedit";
            public string GetUserLocationWiseRoleEditUrl => _baseUrl + "UserApi/getedituserlocationwiserole";
            public string UpdateUserRoleMappingUrl => _baseUrl + "updateuserrolemaster";
            public string DeleteUserRoleMappingUrl => _baseUrl + "updateuserrolemaster";
            public string GetAllCountriesUrl => _baseUrl + "getallcountries";
            public string GetAllUserTypeUrl => _baseUrl + "getallusertypemaster";
            public string UpdateUserRoleLocation => _baseUrl + "updateuserrolelocation";
            public string CheckUserEmail => _baseUrl + "checkemailexist";//Added By Harshida 22-01-'25
            public string GetUserRoleLocation => _baseUrl + "getuserrolelocation";

            public string GetAllModuleUrl => _baseUrl + "getallmodules";
            public string GetAllServicesByModuleIdUrl => _baseUrl + "getservicesbymoduleid";
            public string GetServiceByIdUrl => _baseUrl + "getservicebyid";
            
            #endregion

            #region BulkUpload Service
            public string PostDataMigrationDepartmentUrl => _baseUrl + "postdepartmentstage";
            public string GetBulkUploadServiceNameAndTemplateUrl => _baseUrl + "getserviceimportmasterbyid";
            public string PostDataMigrationContractorDocumentUrl => _baseUrl + "postcontractordocumentmaster";
            public string UpdateContractorDocumentFTPPathUrl => _baseUrl + "updatecontractordocumentpathmaster";
            #endregion

            #region Services Master  and Service wise Data Get and Post
            public string GetServiceWisePendingApproverUrl => _baseUrl + "GetSesrviceWiseList";
            public string GetServiceNameforApprovalProcess => _baseUrl + "getservicemasterbyModuleId";
            public string GetReturnedData => _baseUrl + "GetReturnedData";
            public string PostApprovalDataStgingtofinal => _baseUrl + "postapprovaldatastagingtoFinal";
            public string PutApprovalDataReturnStatus => _baseUrl + "putapprovaldatareturnstatus";
            public string DeleteApprovalMatrix => _baseUrl + "deleteapprovalmatrix";

            #endregion

            #region Wage
            public string PutWageFormulaUrl => _baseUrl + "updateformulamaster";
            public string PostWageFormulaUrl => _baseUrl + "postformulamaster";
            public string GetWageFormulaByIdUrl => _baseUrl + "getformulamasterbyid";
            public string GetAllEarningDeductionMasterUrl => _baseUrl + "getallearningdeductiomaster";
            public string GetAllFormulaUrl => _baseUrl + "getallformulamaster";
            public string DeleteFormulaByIdUrl => _baseUrl + "deleteformulamaster";
            public string FormulaSuggestionsUrl => _baseUrl + "getformulasuggestions";
            #endregion

            #region Report 
            public string GetPayRegisterReportUrl => _baseUrl + "getpayregisterreport";
            public string GetFineRegisterReportUrl => _baseUrl + "getfineregisterreport";
            public string GetComplianceReportUrl => _baseUrl + "getcompliancereport";
            public string GetSalarySlipReportUrl => _baseUrl + "getsalaryslipreport";
            public string GetOvertimeReportUrl => _baseUrl + "getovertimereport";
            public string GetLossDamageReportUrl => _baseUrl + "getlossdamageregisterreport";
            public string GetLoanandAdvanceReportUrl => _baseUrl + "getloanandadvancereport";
            public string GetTaxDeductionReportUrl => _baseUrl + "gettaxdeductionreport";
            public string GetContractorPaymentRegisterUrl => _baseUrl + "getcontractorpaymentregister";
            #endregion
            #region
            public string GetApprovalConfigGridUrl => _baseUrl + "getapprovalconfiggrid"; //chirag 12-03-2025
            public string GetApprovalConfigUrl => _baseUrl + "getapprovalconfig"; //chirag 12-03-2025
            public string PostApprovalSetUpUrl => _baseUrl + "postapprovalsetup"; //chirag 12-03-2025


            public string GetApprovalSetUpUrl => _baseUrl + "getapprovalsetupbyserviceid"; //chirag 12-03-2025
            public string GetApprovalUrl => _baseUrl + "get-approvals"; //Harshida 29-05-25
            public string UpdateApprovalUrl => _baseUrl + "update-approval"; //Harshida 02-06-25
            #endregion
        }
    }
}
