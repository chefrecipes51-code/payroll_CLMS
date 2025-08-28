using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Common.ApplicationConstant
{
    public static class DbConstants
    {
        #region Store Procedure

        #region Store Procedure / Login
        public const string AuthUserAccount = "SP_AuthUserAccount";
        #endregion

        #region Store Procedure / User Master  

        //  Auth user account details
        public const string GetUserByIdForAuth = "SP_AuthByIdUserMasterDetail";
        public const string GetUserAdditionalDetailsByIdForAuth = "SP_SelectUserInfoDtl"; //Added By Harshida 23-12-'24
        public const string GetUserUserRoleBasedMenu = "SP_SelectUserRoleBasedMenu"; //Added By Harshida 10-01-'25
        public const string GetUserUserCompanyRoleLocation = "SP_SelectUserLocationwiseRole";
        public const string GetBreadcrumbByMenuId = "SP_Select_BreadcrumbByMenuId";
        public const string GetUserUserRoleBasedMenuEdit = "SP_SelectUserRoleBasedMenu_Edit";
        public const string AddUserUserRoleBasedMenu = "SP_AddUpdateUserRoleMenuMap"; //Added By Harshida 17-01-'25
        public const string AddUserTransactionHistory = "SP_Trn_Loginhiostory"; //Added By Harshida 27-03-'25
        public const string UpdateUserTransactionHistory = "SP_Trn_Loginhiostory"; //Added By Harshida 27-03-'25
        public const string SelectUserTransactionHistory = "SP_Trn_Select_Loginhiostory"; //Added By Harshida 27-03-'25
        // User core related funtionality sp names
        public const string GetUsers = "SP_SelectUsersMasterDetails";
        public const string GetUserById = "SP_SelectByIdUserMasterDetail";
        public const string GetUserMapDetailsById = "SP_SelectUserMapDetailByID";
        public const string GetEditUserMapDetailsById = "SP_SelectUserdDtlByID";
        public const string GetEditUserLocationWiseRole = "SP_SelectUserLocationwiseRole";
        public const string AddEditUser = "SP_AddUpdateUserMasterDetail";
        public const string ChangeUserPassword = "SP_ChangeUserPassword";
        public const string DeleteUserDetail = "SP_DeleteUserMaster";
        public const string UpdateUserRoleStatus = "SP_Update_User_Role_Status";
        public const string UpdateUserLocationStatus = "SP_Update_User_Locatione_Status";
        public const string GetUsersList = "SP_SelectUserList";
        public const string GetLocationwiseUsersList = "SP_SelectLocationwiseUserList";
        public const string UpdateDeactivateUsers = "SP_Deactivate_User";


        // User Role & Company Mapping SP
        public const string GetUserRoleMappingDetail = "SP_GetUserRoleMappingByUserId";
        public const string GetRoleById = "SP_GetByIdRoleMasterDetail";

        // Forgot Password
        public const string SendEmail = "SP_SendEmailSMSProcess";

        public const string CheckOTPIsValid = "SP_CheckOTPIsValid";
        public const string AuthConfigDetails = "SP_GetAuthConfigDetails";
        public const string UpdateUserPassword = "SP_UpdateUserPassword";
        public const string UpdateUserAccountStatus = "Sp_Deactivate_User";
        public const string ApproveOrRejectUpdateUserAccountStatus = "sp_update_user_active_deactive_status";

        // Update user password
        public const string UpdateUserLoginActivity = "SP_UpdateUserLoginActivityDetail";
        public const string UpdateUserActiveDeactiveStatus = "sp_update_user_active_deactive_status";

        // Update User Default Role And Location
        public const string UpdateUserDefaultRoleLocation = "SP_UpdateDefaultRoleOrLocation"; //Added By Harshida 27-12-'24

        // Email Already Exist
        public const string CheckEmailExist = "SP_SelectUserExist"; //Added By Harshida 22-01-'25

        //Company statuatory
        public const string GetAllEntityTaxStatutory = "SP_SelectEntityTaxStatutory";
        public const string GetEntityTaxStatutoryById = "SP_SelectEntityTaxStatutory";
        public const string AddEditEntityTaxStatutory = "SP_AddUpdateEntityTaxStatutory";
        public const string DeleteEntityTaxStatutory = "SP_DeleteEntityTaxStatutory";
        #endregion

        #region Store Procedure / Error

        public const string AddErrorMaster = "SP_AddErrorLogMaster";
        public const string SelectRoleApiPermission = "SP_SelectRoleEndpointPermission";

        #endregion

        #region Store Procedure / Wage Grade Master  


        public const string GetWageGradeMaster = "SP_SelectWageGradeMaster";
        public const string AddEditWageGradeMaster = "SP_AddUpdateWageGradeMaster";
        public const string GetWageGradeMasterById = "SP_SelectWageGradeMaster";
        public const string DeleteWageGradeMaster = "SP_DeleteWageGradeMaster";
        public const string ChangeWageGradeMasterStatus = "";
        #endregion

        #region Store Procedure / Pay Grade Master  
        public const string GetPayGradeMaster = "SP_SelectPayGradeMaster";
        public const string GetTradeMaster = "[dbo].[SP_SelectTrademst]";
        public const string GetSkillCategory = "[dbo].[SP_SelectSkillCategory]";
        public const string GetDistinctLocation = "[dbo].[SP_SelectDistinctLocation]";
        public const string AddEditPayGradeMaster = "SP_AddUpdatePayGradeMaster";
        public const string GetPayGradeMasterById = "SP_SelectPayGradeMaster";
        public const string DeletePayGradeMaster = "SP_DeletePayGradeMaster";
        public const string ChangePayGradeMasterStatus = "";
        #endregion

        #region Store Procedure / Pay Grade Config Master  
        public const string GetPayGradeConfigMaster = "SP_SelectPayGradeConfig";
        public const string AddEditPayGradeConfigMaster = "SP_AddUpdatePayGradeConfig";
        public const string GetPayGradeConfigMasterById = "SP_SelectPayGradeConfig";
        public const string DeletePayGradeConfigMaster = "SP_DeletePayGradeConfig";
        public const string ChangePayGradeConfigMasterStatus = "";
        #endregion

        #region Store Procedure / Wage Config Master  
        public const string GetWageConfigDetails = "SP_SelectWageConfigDetails";
        public const string AddEditWageConfigDetails = "SP_AddUpdateWageConfigDetail";
        public const string GetWageConfigDetailById = "SP_SelectWageConfigDetails";
        public const string DeleteWageConfigDetail = "SP_DeleteWageConfig";
        public const string ChangeWageConfigDetailStatus = "";
        #endregion 

        #region Store Procedure / Wage Rate Master 
        public const string GetWageRateMaster = "SP_SelectWageRateMaster";
        public const string AddEditWageRateMaster = "SP_AddUpdateWageRateMaster";
        public const string GetWageRateMasterById = "SP_SelectWageRateMaster";
        public const string DeleteWageRateMaster = "SP_DeleteWageRateMaster";
        #endregion

        #region Store Procedure / PTax Slab
        public const string GetPtaxSlab = "SP_SelectPtaxSlabConfig";
        public const string GetTaxParam = "SP_SelectTaxParam";
        public const string AddEditPtaxSlab = "SP_AddUpdatePtaxSlab";
        public const string GetPtaxSlabById = "SP_SelectPtaxSlabConfig";
        public const string DeletePtaxSlab = "SP_DeletePtaxSlabConfig";
        #endregion

        #region Store Procedure / Entity Master
        public const string GetAllContractorDetail = "SP_Select_Contractor";
        public const string GetAllEntityDataValidation = "SP_Select_DataValidation";
        #endregion 
        #region Store Procedure / WorkOrder Master
        public const string GetAllWorkdOrderDetail = "SP_SelectWorkOrderByContractor";
        public const string GetContractorWorkdOrder = "SP_BindContractorWithWorkOrder";
        #endregion

        #endregion

        #region Store Procedure / PayrollSetting
        public const string GetPayrollSetting = "";
        public const string AddEditPayrollSetting = "SP_AddUpdate_PayrollSetting";
        public const string GetPayrollSettingById = "";
        public const string DeletePayrollSetting = "";
        #endregion

        #region Store Procedure / PayrollCompliance
        public const string GetPayrollCompliance = "";
        public const string AddEditPayrollCompliance = "SP_AddUpdate_PayrollCompliance";
        public const string GetPayrollComplianceById = "";
        public const string DeletePayrollCompliance = "";
        #endregion

        #region Store Procedure / PayrollGlobal
        public const string PostOneCompanyToAnotherCompany = "SP_AddUpdate_CopyParam";
        public const string GetAllEntityType = "SP_Select_Entity_Type";
        public const string GetPayrollGlobal = "SP_SelectGlobal_Payroll_Param";
        public const string AddEditPayrollGlobal = "SP_AddUpdate_PayrollGlobal";
        public const string GetPayrollGlobalById = "";
        public const string DeletePayrollGlobal = "";
        #endregion

        #region Store Procedure / ThirdPartyData
        public const string GetThirdPartyData = "";
        public const string AddEditThirdPartyData = "SP_AddUpdate_ThirdPartyData";
        public const string GetThirdPartyDatalById = "";
        public const string DeleteThirdPartyData = "";
        #endregion

        #region Store Procedure / Earning Deduction Master  
        public const string GetEarningDeductionMaster = "SP_SelectEarningDeductionMaster";
        public const string AddEditEarningDeductionMaster = "SP_AddUpdateEarningDeductionMaster";
        public const string PayComponentActivationMaster = "SP_ActivateComponent"; // 29-04-25

        public const string GetEarningDeductionMasterById = "SP_SelectEarningDeductionMaster";
        public const string DeleteEarningDeductionMaster = "SP_DeleteEarningDeductionMaster";
        public const string ChangeEarningDeductionMasterStatus = "";
        #endregion

        #region Store Procedure / Country Master
        public const string GetAllCountries = "SP_SelectcOUNTRYMaster";
        #endregion

        #region Store Procedure / User Type Master
        public const string GetAllUserTypeMaster = "SP_SelectUserTypeMaster";
        #endregion

        #region Store Procedure / Location Master  
        public const string GetLocationMaster = "SP_SelectLocationMaster";
        public const string AddEditLocationMaster = "SP_AddUpdateLocationMaster";
        public const string GetLocationMasterById = "SP_SelectLocationMaster";
        public const string DeleteLocationMaster = "SP_DeleteLocationMaster";
        public const string ChangeLocationMasterStatus = "";
        #endregion

        #region Store Procedure / General Account Head 
        public const string GetGeneralAccountMaster = "SP_Select_Accounting_Head ";
        public const string GetGeneralAccountType = "SP_Select_Accounting_Type";
        public const string AddEditGeneralAccountMaster = "Sp_AddUpdate_Accounting_Head";
        public const string GetGeneralAccountMasterById = "SP_Select_Accounting_Head ";
        public const string DeleteGeneralAccountMaster = "SP_Delete_Accounting_Head ";
        #endregion

        #region Store Procedure / G L Group
        public const string GetGLGroupMaster = "Sp_Select_GL_Group";
        public const string GetGLGroupMasterById = "Sp_Select_GL_Group";
        public const string AddEditGLGroup = "Sp_AddUpdate_GL_Group";
        public const string DeleteGLGroup = "Sp_Delete_GL_Group";
        #endregion

        #region Store Procedure / Area Master  
        public const string GetAreaMaster = "SP_SelectAreaMaster";
        public const string AddEditAreaMaster = "SP_AddUpdateAreaMaster";
        public const string GetAreaMasterById = "SP_SelectAreaMaster";
        public const string DeleteAreaMaster = "SP_DeleteAreaMaster";
        public const string ChangeAreaMasterStatus = "";
        #endregion

        #region Store Procedure / Company Currency Master  
        public const string GetCompanyCurrencyMaster = "SP_SelectCompanyCurrencyMaster";
        public const string AddEditCompanyCurrencyMaster = "SP_AddUpdateCompanyCurrencyMaster";
        public const string GetCompanyCurrencyMasterById = "SP_SelectCompanyCurrencyMaster";
        public const string DeleteCompanyCurrencyMaster = "SP_DeleteCompanyCurrencyMaster";
        public const string ChangeCompanyCurrencyMasterStatus = "";
        #endregion

        #region Store Procedure / Company Location Dto Master  
        public const string GetCompanyLocationDtoMaster = "SP_SelectCompanyLocationMap";
        #endregion

        #region Store Procedure / Company Master  
        public const string GetCompanyMaster = "SP_SelectCompanyMaster";
        public const string AddEditCompanyMaster = "SP_AddUpdateCompanyMaster";
        public const string GetCompanyMasterById = "SP_SelectCompanyMaster";
        public const string DeleteCompanyMaster = "SP_DeleteCompanyMaster";
        public const string ChangeCompanyMasterStatus = "";
        public const string AddStgCompanyMaster = "SP_AddStgCompany";
        public const string GetStgCompanyMasterById = "SP_SelectCommpany_Staging";
        public const string UpdateStgCompanyMaster = "SP_ValidateCompanyStaging";
        public const string GetCompanyType = "SP_SelectCompanyType "; //Added By Harshida 06-02-25
        #endregion

        #region Store Procedure / SubsidiaryMaster [Added By Krunali]

        // Added by Krunali gohil payroll-431
        public const string GetCompanySubsidiaryMaster = "SP_SelectSubSidiary";
        public const string AddCompanySubsidiaryMaster = "SP_AddUpdateSubsidiary";
        public const string UpdateCompanySubsidiaryMaster = "SP_AddUpdateSubsidiary";
        public const string GetCompanySubsidiaryMasterById = "SP_SelectSubsidiary";
        public const string DeleteCompanySubsidiaryMaster = "SP_DeleteSubsidiary";

        #endregion

        #region Store Procedure / Currency Master //Added By Harshida
        public const string GetCurrencyMaster = "SP_SelectCurrency";
        public const string AddEditCurrencyMaster = "";
        public const string GetCurrencyMasterById = "SP_SelectCurrency";
        public const string DeleteCurrencyMaster = "";
        #endregion

        #region Store Procedure / Company Correspondance Master  
        public const string GetCompanyCorrespondance = "SP_SelectCompanyCorrespondance";
        public const string AddEditCompanyCorrespondance = "SP_AddUpdateCompanyCorrespondance";
        public const string GetCompanyCorrespondanceById = "SP_SelectCompanyCorrespondance";
        public const string DeleteCompanyCorrespondance = "SP_DeleteCompanyCorrespondanceMaster";
        public const string ChangeCompanyCorrespondanceStatus = "";
        public const string AddStageCompanyCorrespondance = "SP_AddStgCompanycorrespondance";
        public const string GetStageCompanyCorrespondance = "SP_SelectStgCompanycorrespondance_staging";
        public const string ValidateStageCompanyCorrespondance = "SP_ValidateStgCompanycorrespondance";
        #endregion

        #region Store Procedure / Company Configuration [Added By Harshida 14-02-25]
        public const string AddCompanyConfiguration = "SP_AddCompanyConfiguration";
        #endregion

        #region Store Procedure / Company Statutory Master  
        public const string GetCompanyStatutoryMaster = "SP_SelectCompanyStatutoryMaster";
        public const string AddEditCompanyStatutoryMaster = "SP_AddUpdateCompanyStatutoryMaster";
        public const string GetCompanyStatutoryMasterById = "SP_SelectCompanyStatutoryMaster";
        public const string DeleteCompanyStatutoryMaster = "SP_DeleteCompanyStatutoryMaster";
        public const string ChangeCompanyStatutoryMasterStatus = "";
        #endregion

        #region Store Procedure / Department Master  
        public const string GetDepartmentMaster = "SP_SelectDepartmentMaster";
        public const string AddEditDepartmentMaster = "SP_AddUpdateDepartmentMaster";
        public const string GetDepartmentMasterById = "SP_SelectDepartmentMaster";
        public const string DeleteDepartmentMaster = "SP_DeleteDepartmentMaster";
        public const string ChangeDepartmentMasterStatus = "";
        #endregion

        #region Store Procedure / Map Department Location   
        public const string GetMapDepartmentLocation = "SP_SelectDepartmentMapLocationMaster";
        public const string AddEditMapDepartmentLocation = "SP_AddUpdateDepartmentMapLocationMaster";
        public const string GetMapDepartmentLocationById = "SP_SelectDepartmentMapLocationMaster";
        public const string DeleteMapDepartmentLocation = "SP_DeleteDepartmentMapLocationMaster";
        public const string ChangeMapDepartmentLocationStatus = "";
        public const string GetAllFloorMaster = "SP_SelectFloorMaster";
        public const string GetFloorMasterById = "SP_SelectFloorMaster";
        #endregion

        #region Store Procedure / Map User Company
        public const string GetMapUserCompanyMaster = "SP_SelectMapUserCompany";
        public const string AddEditMapUserCompanyMaster = "SP_AddUpdateMapUserCompany";
        public const string GetMapUserCompanyMasterById = "SP_SelectMapUserCompany";
        public const string DeleteMapUserCompanyMaster = "SP_DeleteMapUserCompany";
        public const string ChangeMapUserCompanyMasterStatus = "";
        #endregion

        #region Store Procedure / Map User Role
        public const string GetMapUserRoleMaster = "SP_SelectUserRoleMap";
        public const string AddEditMapUserRoleMaster = "SP_AddUpdateUserRoleMap";
        public const string GetMapUserRoleMasterById = "SP_SelectUserRoleMap";
        public const string DeleteMapUserRoleMaster = "SP_DeleteUserRoleMap";
        public const string ChangeMapUserRoleMasterStatus = "";
        #endregion

        #region Store Procedure / PayrollTransactionService
        public const string AddEditPayrollTransactionService = "SP_Srv_Appr_Rej_Update";
        public const string GetPayrollTransactionServiceById = "SP_SelectServiceApproveReject";
        #endregion

        #region Store Procedure / Yearly It Table 
        public const string GetYearlyITTable = "SP_selectYearlyITTable";
        public const string AddEditYearlyITTable = "SP_AddUpdateYearlyITTable";
        public const string GetYearlyITTableById = "SP_selectYearlyITTable";
        public const string DeleteYearlyITTable = "SP_DeleteYearlyITTable";
        public const string ChangeYearlyITTableStatus = "";
        #endregion

        #region Store Procedure / Deatil Yearly It Table 
        public const string GetDeatilYearlyITTable = "SP_SelectYearlyITTableDetail";
        public const string AddEditDeatilYearlyITTable = "SP_AddUpdateYearlyITTableDetail";
        public const string GetDeatilYearlyITTableById = "SP_SelectYearlyITTableDetail";
        public const string DeleteDeatilYearlyITTable = "SP_DeleteYearlyITTableDetail";
        public const string ChangeDeatilYearlyITTableStatus = "";
        #endregion

        #region Store Procedure / Tax Slab Master  
        public const string GetTaxSlabTable = "SP_SelectTaxConfig";
        public const string GetTaxRegimeTable = "SP_SelectTaxRegime";
        public const string AddEditTaxSlabTable = "SP_AddUpdateTaxConfig";
        public const string GetTaxSlabTableById = "SP_SelectTaxConfig";
        public const string DeleteTaxSlabTable = "Sp_deletetaxconfig ";
        public const string ChangeTaxSlabTableStatus = "";
        #endregion

        #region Store Procedure / AuditTrailService
        public const string GetAuditTrailByCompanyIDAndDate = "Sp_Select_Audit_Trail";
        public const string GetAllAuditTrails = "";
        public const string GetAuditTrailById = "";
        public const string AddAuditTrail = "";
        public const string DeleteAuditTrail = "";
        public const string UpdateAuditTrail = "";
        #endregion

        #region Store Procedure / PendingApprovalRequest
        public const string GetPendingApprovalRequest = "SP_Select_SRV_Appr_Rej";
        #endregion

        #region Store Procedure / Role Master Table 
        public const string GetRoleMaster = "SP_SelectRoleMaster";
        public const string AddEditRoleMaster = "SP_AddUpdateRoleMaster";
        public const string GetRoleMasterById = "SP_SelectRoleMaster";
        public const string DeleteRoleMaster = "SP_DeleteRoleMaster";
        public const string ChangeRoleMasterStatus = "";
        #endregion

        #region Store Procedure / Map User Location 
        public const string GetMapUserLocation = "SP_SelectUserLocationMap";
        public const string AddEditMapUserLocation = "SP_AddUpdateUserLocationMap";
        public const string GetMapUserLocationById = "SP_SelectUserLocationMap";
        public const string DeleteMapUserLocation = "SP_DeleteUserLocationMap";
        public const string ChangeMapUserLocationStatus = "";
        #endregion

        #region Store Procedure / Map Department Role   
        public const string GetMapDepartmentRole = "dbo.SP_SelectDepartmentRoleMap";
        public const string AddEditMapDepartmentRole = "dbo.SP_AddUpdateDepartmentRoleMap";
        public const string GetMapDepartmentRoleById = "dbo.SP_SelectDepartmentRoleMap";
        public const string DeleteMapDepartmentRole = "dbo.SP_DeleteDepartmentRoleMap";
        public const string ChangeMapDepartmentRoleStatus = "";
        #endregion

        #region Store Procedure / Event Master  
        public const string GetEventMaster = "dbo.SP_SelectEventMaster";
        public const string AddEditEventMaster = "dbo.SP_AddUpdateEventMaster";
        public const string GetEventMasterById = "dbo.SP_SelectEventMaster";
        public const string DeleteEventMaster = "dbo.SP_DeleteEventMaster";
        public const string ChangeEventMasterStatus = "";
        #endregion

        #region Store Procedure / Role Menu  
        public const string GetRoleMenu = "";
        public const string AddEditRoleMenu = "SP_AddUpdateRoleMenuMap";
        public const string GetWageRoleMenuById = "";
        public const string DeleteRoleMenu = "SP_DeleteMapRoleMenuMap";
        public const string GetUserRoleMenuByRoleId = "SP_SelectUserRoleMenu";
        #endregion

        #region Store Procedure / Event Auth SetUp  
        public const string AddEditEventAuthSetUp = "dbo.SP_AddUpdate_trn_eventauth_setup";
        public const string GetEventAuthSetUp = "dbo.SP_Selecttrn_eventauth_setup";
        #endregion

        #region Store Procedure / Approval Setup  
        public const string GetApprovalConfigGrid = "dbo.SP_GetApprovalConfigAll";
        public const string GetApprovalConfig = "dbo.SP_GetApprovalConfigByID";
        public const string AddEditApprovalConfig = "dbo.SP_AddUpdate_approval_config";
        public const string DeleteApprovalMatrix = "dbo.SP_DeleteApprovalConfig";
        public const string GetAllApproval = "dbo.SP_Select_Approval";//29-05-25
        public const string UpdateApprovalStatus = "dbo.SP_UpdateApproval";//29-05-25

        public const string GetApprovalSetUp = "dbo.SP_GetApprovalSetUpByServiceID";   //remove      
        //public const string AddEditApprovalSetUp = "dbo.SP_AddUpdate_approval_setup";
        #endregion

        #region Store Procedure / Department Stage  
        public const string GetDepartmentStage = "SP_SelectDepartmentStaging";
        public const string AddDepartmentStage = "SP_AddDepartmentStaging";
        public const string UpdateDepartmentStage = "SP_ValidateDepartmentStaging";
        public const string GetDepartmentStageById = "SP_SelectDepartmentStaging";
        public const string DeleteDepartmentStage = "";
        #endregion

        #region Store Procedure / Contract Type
        public const string GetContractTypeMaster = "";
        public const string AddContractTypeMaster = "SP_AddContractType";
        public const string UpdateContractTypeMaster = "";
        public const string GetContractTypeMasterById = "SP_SelectContracttype";
        public const string DeleteContractTypeMaster = "";
        #endregion

        #region Store Procedure / Service Import/ Upload Template Master
        public const string GetServiceImportMasterById = "SP_Selectserviceimport";
        #endregion

        #region Store Procedure / Service Master
        public const string GetServiceMaster = "SP_SelectserviceMaster";
        #endregion

        #region Store Procedure / Generic Service Master
        public const string GetGenericServiceData = "SP_GenericServiceDataFetch";
        #endregion

        #region Store Procedure / Department location Stag  
        public const string GetDepartmentLocationStage = "";
        public const string AddDepartmentLocationStage = "SP_AddDepartmentLocationStaging";
        public const string GetDepartmentLocationStageById = "SP_SelectDepartmentLocationStaging";
        public const string DeleteLocationDepartmentStage = "";
        public const string UpdateDepartmentLocationStage = "SP_ValidateDepartmentLocationStaging";
        #endregion

        #region Store Procedure / Final Data Approval Migration CLMS -- Added By Abhishek 01-04-2025  
        public const string ValidateApprovalMigrationToFinal = "SP_ValidateMigrateDataStagingToFinal";
        public const string GetReturnedMigrationData = "SP_SelectGenericReturnedDataFetch";
        #endregion

        #region Store Procedure / Contractor Document -- Added By Harshida 06-12-'24
        public const string GetContractorDocumentMaster = "";
        public const string AddContractorDocumentMaster = "SP_AddContractorDocument";
        public const string UpdateContractorDocumentMaster = "SP_Update_Contractor_Document";
        public const string GetContractorDocumentMasterById = "SP_Select_Contractor_Document";
        public const string DeleteContractorDocumentMaster = "";
        #endregion

        #region Store Procedure / Contract Master
        public const string GetContractMaster = "";
        public const string AddContractMaster = "SP_AddContract";
        public const string UpdateContractMaster = "";
        public const string GetContractMasterById = "SP_SelectContract";
        public const string DeleteContractMaster = "";
        #endregion

        #region Store Procedure / Work Order Master   
        public const string GetWorkOrderMaster = "";
        public const string AddWorkOrderMaster = "SP_Add_WorkOrder_Staging";
        public const string UpdateWorkOrderMaster = "SP_ValidateWorkorderStaging";
        public const string GetWorkOrderMasterById = "SP_SelectWorkorderStaging";
        public const string DeleteWorkOrderMaster = "";
        #endregion

        #region Store Procedure / Contractor Stag
        public const string GetContractorStage = "";
        public const string AddContractorStage = "SP_AddContractorStaging";
        public const string GetContractorStageByLogId = "SP_SelectContractorStaging";
        public const string DeleteContractorStage = "";
        public const string UpdateContractorStage = "SP_ValidateContractorStaging";
        #endregion

        #region Store Procedure / Map Work Order Contractor   
        public const string GetMapWorkOrderContractor = "";
        public const string AddMapWorkOrderContractor = "SP_AddWOContractor_Map_Staging";
        public const string UpdateMapWorkOrderContractor = "SP_ValidateMapWoContractorStaging";
        public const string GetMapWorkOrderContractorById = "SP_SelectWOContractor_Map_Staging";
        public const string DeleteMapWorkOrderContractor = "";
        #endregion

        #region Store Procedure / Map Entity Tax Regime   
        public const string GetMapEntityTaxRegime = "";
        public const string AddMapEntityTaxRegime = "SP_AddUpdateEntityTaxRegime";
        public const string UpdateMapEntityTaxRegime = "";
        public const string GetMapEntityTaxRegimeById = "";
        public const string DeleteMapEntityTaxRegime = "";
        public const string GetEntityFilterProcedure = "SP_Select_Entity_Filters";
        public const string GetAllFinancialYearProcedure = "SP_Select_Allfinyr";
        #endregion

        #region Store Procedure / Map Grade Entity   
        public const string UpdateMapGradeEntity = "Sp_Update_Entity_PayGrade";
        public const string GetAllEntityDetailUrl = "SP_Select_Entity_ByID";
        public const string UpdateEntityComplianceDetailUrl = "SP_AddUpdate_EntityCompliance";
        #endregion

        #region Store Procedure / Subsidiary Stage -- Added By Harshida 12-12-'24
        public const string GetSubsidiaryStage = "";
        public const string AddSubsidiaryStage = "SP_AddSubsidiary_Staging";
        public const string UpdateSubsidiaryStage = "SP_Update_Contractor_Document";
        public const string GetSubsidiaryStageByLogId = "SP_SelectStgSubsidiaryEntity";
        public const string DeleteSubsidiaryStage = "";
        #endregion

        #region Store Procedure / Subsidiary Master -- Added By Harshida 16-12-'24
        public const string GetSubsidiaryMaster = "";
        public const string AddSubsidiaryMaster = "SP_ValidateStgCompanySubsidiary";
        public const string UpdateSubsidiaryMaster = "SP_Update_Contractor_Document";
        public const string GetSubsidiaryMastereByLogId = "SP_SelectStgSubsidiaryEntity";
        public const string DeleteSubsidiaryMaster = "";
        #endregion

        #region Store Procedure / Pay Month
        public const string GetPayMonthMaster = "SP_SelectMonthlyPayPeriod";
        public const string AddEditPayMonth = "SP_AddMonthlyPayrollPeriods_ByCompany";
        public const string GetPayMonthById = "SP_SelectMonthlyPayPeriod";
        public const string DeletePayMonth = "";
        #endregion

        #region Store Procedure /Attendance Pay Month
        public const string GetAttendancePayMonthMaster = "SP_SelectMonthlyAttendancePayPeriod";
        public const string AddEditAttendancePayMonth = "SP_AddMonthlyAttendancePayrollPeriods_ByCompany";
        public const string GetAttendancePayMonthById = "SP_SelectMonthlyAttendancePayPeriod";
        public const string DeleteAttendancePayMonth = "";
        #endregion

        #region Store Procedure / Entity Master -- Added By Chirag 19-12-'24
        public const string GetEntityMaster = "";
        public const string AddEntityMaster = "SP_AddEntityData";
        public const string UpdateEntityMaster = "SP_ValidateEntityDataStaging";
        public const string GetEntityMastereByLogId = "SP_SelectEntityStaging";
        public const string DeleteEntityMaster = "";
        public const string UpdateEntityMasterAssignWage = "SP_EntityAssignWage";

        public const string AddEntityMasterMapSubsidiary = "SP_Map_Entity_SubsidiaryData";
        public const string GetEntityMasterMapSubsidiaryById = "SP_Select_Map_Entity_SubsidiaryData";

        public const string AddEntityBankFinance = "SP_Add_Entity_BankFinance";
        public const string GetEntityBankFinanceById = "SP_Select_Entity_BankFinance";
        public const string UpdateEntityBankFinance = "";

        public const string AddEntityDocuments = "SP_Add_Entity_Document";
        public const string GetEntityDocumentsById = "SP_Select_Entity_Document";
        public const string UpdateEntityDocuments = "";

        public const string AddEntityInsurance = "SP_Add_Entity_Insurance";
        public const string GetEntityInsuranceById = "SP_Select_Entity_Insurance";
        public const string UpdateEntityInsurance = "";
        #endregion

        #region Store Procedure / Geographical Location  
        public const string GetAllStateMaster = "SP_SelectState";
        public const string GetAllCityMaster = "SP_SelectCity";
        public const string GetAllCityWiseLocationMaster = "SP_Selectcity_wise_Location";
        #endregion

        #region Store Procedure / formula masterr
        public const string GetFormulaMaster = "SP_SelectFormula";
        public const string GetFormulaMasterById = "SP_SelectFormula";
        public const string AddEditFormulaMaster = "SP_AddUpdateFormula";
        public const string DeleteFormulaMaster = "SP_DeleteFormula";
        public const string FormulaSuggestionsMaster = "sp_SelectFormulaSuggestions";
        #endregion

        #region Store Procedure / Data Processing "Validation" 
        public const string GetValidateMissingContractorData = "SP_ValidateMissingPara_Contractor";
        public const string UpdateValidateMissingContractor = "SP_UpdateValidateMissingPara_Contractor";

        public const string GetValidateMissingEntityData = "SP_ValidateMissingPara_Entity";
        public const string UpdateMissingEntity = "SP_UpdateValidateMissingPara_Entity";

        public const string GetValidateMissingPayData = "SP_ValidateMissingPara_EntityPayStructure";
        public const string UpdateMissingPayCal = "SP_UpdateValidateMissingPara_EntityPayStructure";

        public const string GetValidateMissingComplianceData = "SP_ValidateMissingPara_EntityCompliance";
        public const string UpdateValidateMissingComplianceData = "SP_UpdateValidateMissingPara_EntityCompliance";

        public const string GetValidateMissingAttendanceData = "SP_ValidateMissingPara_EntityAttendance";
        public const string UpdateValidateMissingAttendanceData = "SP_UpdateValidateMissingPara_EntityAttendance";

        public const string AddPayrollTransactionStagingData = "SP_AddPayrollTranSTGData";

        public const string GetPreviousMonthYearCompany = "SP_GetPreviousMonthYearPeriod_ByCmpId";
        public const string AddPayrollTranStgData = "Sp_InsertPayroll_Tran_Stg_Data";
        #endregion

        #region Store Procedure / Module Master
        public const string GetAllModules = "SP_SelectModuleMaster";
        #endregion

        #region Store Procedure / Services Master
        public const string GetAllServices = "SP_SelectServices";
        #endregion

        #region Store Procedure / SalaryStructure  
        public const string GetSalaryStructureAll = "SP_SelectSalarystructure";// "dbo.GetSalaryStructureAll";
        public const string GetSalaryStructureById = "SP_SelectSalarystructureById";// "dbo.SP_SalaryStructureById";
        public const string AddEditSalaryStructure = "dbo.SP_AddUpdateSalaryStructure";
        public const string CalculateSalaryStructure = "dbo.SP_CalculateSalarySimulator";
        public const string DeleteSalaryStructure = "dbo.SP_DeleteSalaryStructure";

        public const string AddSalaryStructure = "dbo.SP_AddSalaryStructure";

        #endregion

        #region Store Procedure / Contractor Details [Added By Foram Patel 22-05-25]
        public const string GetallContractorDetails = "SP_Select_Contractor";
        #endregion

        #region Store Procedure / Payroll Process 
        public const string GetallContractorPayrollValidationDetails = "SP_Select_Contractor_onpayrollvalidation";
        public const string GetallCompanyPayrollValidationDetails = "SP_Select_Company_onpayrollvalidation";
        public const string GetallCompanyLocationPayrollValidationDetails = "SP_Select_CompanyLocation_onpayrollvalidation";
        public const string GetallWorkOrderPayrollValidationDetails = "SP_Select_workorder_onpayrollvalidation";
        public const string GetallPreviousMonthYearPeriodDetails = "SP_GetPreviousMonthYearPeriod_ByCmpId";
        public const string UpdatePayrollTranStgDataForProcessDetails = "Sp_UpdatePayroll_Tran_Stg_Data_For_Process";
        public const string UpdateStartPayrollProcessDetails = "SP_StartPayrollProcess";
        public const string PostProcessPayrollEmployeesDetails = "sp_ProcessPayrollEmployees";
        public const string GetAllPayrollProcessusingSignalRDetails = "SP_SelectPayrollProcessusingSignalR";
        public const string GetPhaseWiseComponentPayrollProcessDetails = "sp_SelerctComponent_dtl_Payroll_Process";
        #endregion

        #region Store Procedure - Reports  
        public const string GetPayRegisterReport = "dbo.sp_GetPayRegisterData";
        public const string GetFineRegisterReport = "dbo.sp_GetFineRegisterData";
        public const string GetComplianceReport = "dbo.sp_GetComplianceReport";
        public const string GetSalarySlipReport = "dbo.sp_GetSalarySlipData";
        public const string GetOvertimeReport = "dbo.sp_GetOTRegisterData";
        public const string GetLossDamageReport = "sp_GetLossDamageRegisterData";
        public const string GetLoanandAdvanceReport = "sp_GetLoan_Advance_RegisterData";
        public const string GetTDSReport = "SP_GetTaxDeductionReport";
        public const string GetContractorPaymentRegister = "sp_GetContractor_Payment_RegisterData";
        #endregion

        #region User-Defined Table (UDT)

        #region User-Defined Table (UDT)  / Department Stage 
        public const string UDTDepartmentStage = "dbo.udt_department_data";
        #endregion

        #region User-Defined Table (UDT)  / Contract Type Master
        public const string UDTContractTypeMaster = "dbo.udt_contracttype_data";
        #endregion

        #region User-Defined Table (UDT)  / Contractor Document Master -- Added By Harshida 06-12-'24
        public const string UDTContractorDocumentMaster = "dbo.udt_ContractorDocument_data";
        #endregion

        #region User-Defined Table (UDT)  / Subsidiary Stage -- Added By Harshida 12-12-'24
        public const string UDTSubsidiaryStage = "dbo.udt_stgsubsidiary_data";
        #endregion

        #region User-Defined Table (UDT)  / Contract Master
        public const string UDTContractMaster = "dbo.udt_Contract_data";
        #endregion

        #region User-Defined Table (UDT)  / Work Order Master
        public const string UDTWorkOrderMaster = "dbo.udt_workorder_data";
        #endregion

        #region User-Defined Table (UDT)  / Map Work Order Master
        public const string UDTMapWorkOrderMaster = "dbo.udt_mapwocontractor_data";
        #endregion

        #region User-Defined Table (UDT)  / Company Master
        public const string UDTCompanyMaster = "dbo.udt_stgcompany_data";
        #endregion

        #region User-Defined Table (UDT)  / Company Correspondance Master
        public const string UDTCompanyCorrespondanceMaster = "dbo.udt_stg_companycorrespondance_data";
        #endregion

        #region User-Defined Table (UDT)  / Map Entity Tax Regime 
        public const string UDTMapEntityTaxRegime = "dbo.UDT_Entity_TaxRegime";
        #endregion

        #region User-Defined Table (UDT)  / Map Entity Grade  
        public const string UDTMapEntityGrade = "dbo.UDT_Entity_PayGrade";
        #endregion

        #region User-Defined Table (UDT)  / Contractor Document Master -- Added By Harshida 06-12-'24
        public const string UDTUserRoleMenuDetails = "dbo.Udt_User_RoleMenuDetailType";
        #endregion

        #endregion
     
    }

}

