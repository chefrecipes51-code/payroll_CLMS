/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 14-05-2025
///  IMP NOTE      :-
///                1) IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>
$(document).ready(function () {
    setTimeout(function () {
        var companyId = $('#SessionCompanyId').val();
    
        $.ajax({
            //url: '/PayrollGlobalParameters/FetchPayrollSettings', // Adjust URL as per your controller
            url: '/PayrollGlobalParameters/FetchPayrollSettings?companyId=' + companyId, 
            type: 'GET',
            success: function (response) {
                var hasValidData = false;
                //////////////////////////// Bind 1st Tab:- Start/////////////////////////////
                if (response.success && response.data && response.data.globalParams) {
                    hasValidData = true;
                    var payrollParam = response.data.globalParams;
                    if (payrollParam) {
                        $('#GlobalParamId').val(payrollParam.global_Param_ID);
                        if (payrollParam.salary_Frequency != null) {
                            $('#SalaryFrequency').val(payrollParam.salary_Frequency).trigger('change');
                        }
                        if (payrollParam.monthlySalary_Based_On != null) {
                            $('#MonthlySalaryBasedOn').val(payrollParam.monthlySalary_Based_On).trigger('change');
                        }
                        if (payrollParam.effectivePayroll_start_Mnth != null) {
                            $('#EffectivePayrollStartMonth').val(payrollParam.effectivePayroll_start_Mnth).trigger('change');
                        }
                        if (payrollParam.allow_Adhoc_Components != null) {
                            $('#AllowAdHocComponents').val(payrollParam.allow_Adhoc_Components ? 1 : 0).trigger('change');
                        }
                        if (payrollParam.lOckSalary_Post_Payroll != null) {
                            $('#LockSalary').val(payrollParam.lOckSalary_Post_Payroll ? 1 : 0).trigger('change');
                        }
                        ///Enable Toggle 
                        if (payrollParam.global_Param_ID) {                           
                            $('#toggleContainer').show(); // Show toggle
                            if (payrollParam.isActive) {
                                $('#formulaActiveToggle').prop('checked', true);
                                $('#formulaStatusLabel').text("Active");
                            } else {
                                $('#formulaActiveToggle').prop('checked', false);
                                $('#formulaStatusLabel').text("Inactive");
                            }
                        }
                    }
                }              
                //////////////////////////// Bind 1st Tab:- End/////////////////////////////

                //////////////////////////// Bind 2nd Tab:- Start/////////////////////////////
                //console.log("response.data.compliances");
                //console.log(response.data.compliances);
                if (response.success && response.data && response.data.compliances) {
                    hasValidData = true;
                    const comp = response.data.compliances;
                    $('#Prm_Comlliance_ID').val(comp.prm_Comlliance_ID ?? "");

                    if (comp.pf_Applicable != null) {
                        $('#Pf_Applicable').val(comp.pf_Applicable).trigger('change');
                    }

                    if (comp.pf_Share_Mode_Employer != null) {
                        $('#Pf_Share_Mode_Employer').val(comp.pf_Share_Mode_Employer).trigger('change');
                    }

                    if (comp.epf_Employer_Share_Percentage != null) {
                        $('#Epf_Employer_Share_Percentage').val(comp.epf_Employer_Share_Percentage);
                    }

                    if (comp.eps_Employer_Share_Percentage != null) {
                        $('#Eps_Employer_Share_Percentage').val(comp.eps_Employer_Share_Percentage);
                    }

                    if (comp.vpF_Applicable != null) {
                        $('#VPF_Applicable').val(comp.vpF_Applicable ? 1 : 0).trigger('change');
                    }

                    if (comp.vpF_Mode != null) {
                        $('#VPF_Mode').val(comp.vpF_Mode).trigger('change');
                    }

                    if (comp.esic_Applicable != null) {
                        $('#Esic_Applicable').val(comp.esic_Applicable ? 1 : 0).trigger('change');
                    }

                    if (comp.esic_Salary_Limit != null) {
                        $('#Esic_Salary_Limit').val(comp.esic_Salary_Limit);
                    }

                    if (comp.pT_Applicable != null) {
                        $('#PT_Applicable').val(comp.pT_Applicable ? 1 : 0).trigger('change');
                    }

                    if (comp.pt_Registration_Mode != null) {
                        $('#Pt_Regisdtration_Mode').val(comp.pt_Registration_Mode).trigger('change');
                    }

                    if (comp.lwf_Mode != null) {
                        $('#Lwf_Mode').val(comp.lwf_Mode).trigger('change');
                    }

                    if (comp.lwf_Cycle != null) {
                        $('#Lwf_Cycle').val(comp.lwf_Cycle).trigger('change');
                    }

                    if (comp.lwf_Contribution != null) {
                        $('#Lwf_Contribution').val(comp.lwf_Contribution);
                    }
                    if (comp.tDsDeducted_On_Actual_Date != null) {
                        $('#TDsDeducted_On_Actual_Date').val(comp.tDsDeducted_On_Actual_Date ? 1 : 0).trigger('change');
                    }
                    if (comp.esi_Applicable_Percentage != null) {
                        $('#Esi_Applicable_Percentage').val(comp.esi_Applicable_Percentage);
                    }
                    // Show toggle only if Prm_Comlliance_ID is present and > 0
                    if (comp.prm_Comlliance_ID && comp.prm_Comlliance_ID > 0) {
                        $('#toggleContainerTabSecond').show();

                        // Set checkbox based on isActive value
                        if (comp.isActive === true) {
                            $('#formulaActiveToggleTabSecond').prop('checked', true);
                            $('#formulaStatusLabelTabSecond').text('Active');
                        } else {
                            $('#formulaActiveToggleTabSecond').prop('checked', false);
                            $('#formulaStatusLabelTabSecond').text('Inactive');
                        }
                    } else {
                        $('#toggleContainerTabSecond').hide();
                    }

                }
                //////////////////////////// Bind 2nd Tab:- End/////////////////////////////

                //////////////////////////// Bind 3rd Tab:- Start/////////////////////////////
                //console.log("response.data.settings");
                //console.log(response.data.settings);
                if (response.success && response.data && response.data.settings) {
                    hasValidData = true;
                    var settings = response.data.settings;

                    $('#Payroll_Setin_ID').val(settings.payroll_Setin_ID);
                    $('#Initial_char').val(settings.initial_char || '');

                    // Bind selects with boolean properties as 0/1 and others directly
                    $('#Enable_Pay').val(settings.payslip_Generation ? 1 : 0).trigger('change');  // If Enable_Pay is supposed to be Payslip_Generation? Confirm mapping.

                    $('#Payslip_Generation').val(settings.payslip_Generation ? 1 : 0).trigger('change');

                    $('#Payslip_Format').val(settings.payslip_Format).trigger('change');
                    $('#PayslipNumber_Format').val(settings.payslipNumber_Format || '');

                    $('#PaySlip_Number_Scope').val(settings.paySlip_Number_Scope);

                    $('#Auto_Numbering').val(settings.auto_Numbering ? 1 : 0).trigger('change');

                    $('#IsPayslipNo_Reset').val(settings.isPayslipNo_Reset ? 1 : 0).trigger('change');

                    $('#DigitalSignatur_Requirede').val(settings.digitalSignatur_Requirede ? 1 : 0).trigger('change');

                    $('#PaySlipAutoEmail').val(settings.paySlipAutoEmail ? 1 : 0).trigger('change');
                    if (settings.payroll_Setin_ID && settings.payroll_Setin_ID > 0) {
                        $('#toggleContainerTabThird').show();

                        if (settings.isActive === true) {
                            $('#formulaActiveToggleTabThird').prop('checked', true);
                            $('#formulaStatusLabelTabThird').text('Active');
                        } else {
                            $('#formulaActiveToggleTabThird').prop('checked', false);
                            $('#formulaStatusLabelTabThird').text('Inactive');
                        }
                    } else {
                        $('#toggleContainerTabThird').hide();
                    }
                }
               
                //////////////////////////// Bind 3rd Tab:- End/////////////////////////////

                //////////////////////////// Bind 4th Tab:- Start /////////////////////////////
                //console.log("response.data.thirdPartyParams");
                //console.log(response.data.thirdPartyParams);
                if (response.success && response.data && response.data.thirdPartyParams) {
                    hasValidData = true;
                    var thirdParty = response.data.thirdPartyParams;
            $('#Clms_Param_ID').val(thirdParty.clms_Param_ID);
                    $('#DataSyncType').val(thirdParty.dataSync ? 1 : 0).trigger('change');
                    $('#WorkOrder').val(thirdParty.wo_Sync_Frequency).trigger('change');
                    $('#ContractorMaster').val(thirdParty.workOrder_Sync_Frequency).trigger('change');                 
                    //$('#LabourMaster').val(thirdParty.cl_Sync_Frequency).trigger('change');
                    $('#LabourMaster').val(thirdParty.entity_Sync_Frequency).trigger('change');
                    $('#IsContractLabourPayment').val(thirdParty.contractlabour_payment ? 1 : 0).trigger('change');
                    $('#IsAttendanceProcessed').val(thirdParty.isAttendanceProcessed ? 1 : 0).trigger('change');                   
                    $('#Attendance').val(thirdParty.entity_Sync_Frequency).trigger('change');
                    $('#IntegratedLogin').val(thirdParty.integratedLog_in ? 1 : 0).trigger('change');
                    $('#PayregisterFormatId').val(thirdParty.payregisterFormat_ID).trigger('change');
                    $('#AttendanceProxcessType').val(thirdParty.attendanceProxcessType).trigger('change');
                    if (thirdParty.clms_Param_ID && thirdParty.clms_Param_ID > 0) {
                        $('#toggleContainerTabFourth').show();
                        if (thirdParty.isActive === true) {
                            $('#formulaActiveToggleTabFourth').prop('checked', true);
                            $('#formulaStatusLabelTabFourth').text('Active');
                        } else {
                            $('#formulaActiveToggleTabFourth').prop('checked', false);
                            $('#formulaStatusLabelTabFourth').text('Inactive');
                        }
                    } else {
                        $('#toggleContainerTabFourth').hide();
                    }                 
                    var selectedEntities = (thirdParty.entityparam || "").split(',').map(s => s.trim());
                    // Call dropdown binding with selected values
                    fetchAndBindMultiSelectDropdownForGlobal(
                        '/DropDown/FetchEntityTypeDropdown',
                        '#EntityTypeMigration',
                        'Select Entity Type(s)',
                        null,
                        selectedEntities
                    );
                }
                //////////////////////////// Bind 4th Tab:- End /////////////////////////////


                /////////////////////////// Hide-Show Model Popup:-Start///////////////////////////
                if (hasValidData) {
                    $('#btnCopySettings').show();
                } else {
                    $('#btnCopySettings').hide();  
                }
                ////////////////////////   Hide-Show Model Popup:-End///////////////////////////
            },
            error: function (xhr, status, error) {
                //console.error("Error fetching payroll settings:", error);
            }
        });
    }, 500); 
    bindAllDropdowns();
    function bindAllDropdowns() {
        $('#globalDropdownLoader').show(); // 👈 Show loader before binding starts

        const dropdownPromises = [];

        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchSalaryFrequencyDropdown', '#SalaryFrequency', 'Select Salary Frequency'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchMonthlySalaryDropdown', '#MonthlySalaryBasedOn', 'Select Monthly Salary Type'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchEffectivePayrollStartMonthDropdown', '#EffectivePayrollStartMonth', 'Select Payroll Start Month'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchAllowAdhocComponentsDropdown', '#AllowAdHocComponents', 'Select AdHoc Components'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchLockSalaryEditsPostPayrollDropdown', '#LockSalary', 'Select Lock Salary'));

        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchPFApplicableDropdown', '#Pf_Applicable', 'Select PF Applicability'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchESICBasedOnDropdown', '#Esic_Based_on', 'Select ESIC Based On'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchPFEmployerShareDropdown', '#Pf_Share_Mode_Employer', 'Select PF Share Mode'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchVoluntaryPFDropdown', '#VPF_Applicable', 'Select VPF Applicability'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchVPFModeDropdown', '#VPF_Mode', 'Select VPF Mode'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchESICApplicabilityDropdown', '#Esic_Applicable', 'Select ESIC Applicability'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchProfessionalTaxDropdown', '#PT_Applicable', 'Select PT Applicability'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchPTRegistrationModeDropdown', '#Pt_Regisdtration_Mode', 'Select PT Registration Mode'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchLabourWelfareFundDropdown', '#Lwf_Mode', 'Select LWF Mode'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchLWFCycleDropdown', '#Lwf_Cycle', 'Select LWF Cycle'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchTDSDeductedOnActualDateDropdown', '#TDsDeducted_On_Actual_Date', 'Select TDS Deducted On Actual Date'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchPFBasedOnDropdown', '#Pf_Based_on', 'Select PF Based On'));

        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchEnablePayDropdown', '#Enable_Pay', 'Select Enable Pay'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchSlipGenerationDropdown', '#Payslip_Generation', 'Select Enable Pay Slip Generation'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchPayslipFormatDropdown', '#Payslip_Format', 'Select Payslip Format'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchAutoNumberingDropdown', '#Auto_Numbering', 'Select Auto Numbering'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchIsPayslipNoDropdown', '#IsPayslipNo_Reset', 'Select Reset Payslip Numbering'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchDigitalSignatureDropdown', '#DigitalSignatur_Requirede', 'Select Digital Signature Requirement'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchAutoEmailPayslipsDropdown', '#PaySlipAutoEmail', 'Select Auto Email Payslips'));

        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchDataSyncTypeDropdown', '#DataSyncType', 'Select Data Sync Type'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchSyncFrequencyDropdown', '#ContractorMaster', 'Select Contractor Master'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchSyncFrequencyDropdown', '#LabourMaster', 'Select Labour Master'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchSyncFrequencyDropdown', '#WorkOrder', 'Select Work Details'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchSyncFrequencyDropdown', '#Attendance', 'Select Attendance'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchCommongYesNoDropdown', '#IsAttendanceProcessed', 'Select Is Attendance Processed'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchSyncFrequencyDropdown', '#AttendanceProxcessType', 'Select Attendance Process Type'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchCommongYesNoDropdown', '#IntegratedLogin', 'Select Integrated Login'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchPaymentFormatDropdown', '#PayregisterFormatId', 'Select Pay RegisterFormat'));
        dropdownPromises.push(fetchAndBindDropdownForGlobal('/DropDown/FetchCommongYesNoDropdown', '#IsContractLabourPayment', 'Select Is ContractLabour Payment'));
        dropdownPromises.push(fetchAndBindMultiSelectDropdownForGlobal('/DropDown/FetchEntityTypeDropdown', '#EntityTypeMigration', 'Select Entity Type(s)'));

        dropdownPromises.push(bindCompanyDropdownOnPageLoad()); // If it returns a promise

        // 👇 Hide loader once all bindings are complete
        Promise.all(dropdownPromises).then(() => {
            $('#globalDropdownLoader').hide();
            // ✅ Then show the modal
            const copySettingsModal = new bootstrap.Modal(document.getElementById('copySettingsModal'));
            copySettingsModal.show();
        });
    }


    ///////////////////////////////////////////// BIND DROPDOWN For First Tab Global Setting:- Start////////////////////////
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchSalaryFrequencyDropdown', 
    //    '#SalaryFrequency',
    //    'Select Salary Frequency'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchMonthlySalaryDropdown',
    //    '#MonthlySalaryBasedOn',
    //    'Select Monthly Salary Type'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchEffectivePayrollStartMonthDropdown',
    //    '#EffectivePayrollStartMonth',
    //    'Select Payroll Start Month'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchAllowAdhocComponentsDropdown',
    //    '#AllowAdHocComponents',
    //    'Select AdHoc Components'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchLockSalaryEditsPostPayrollDropdown',
    //    '#LockSalary',
    //    'Select Lock Salary'
    //);
    ///////////////////////////////////////////// BIND DROPDOWN For First Tab Global Setting:- End/////////////////////////

    //////////////////////////////////////////// BIND DROPDOWN For Second Tab Payroll Compliance Settings:- Start ///////////////////////////
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchPFApplicableDropdown', // ✅ Matches FetchPFApplicableDropdown
    //    '#Pf_Applicable',
    //    'Select PF Applicability'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchESICBasedOnDropdown', // ✅ Matches FetchESICBasedOnDropdown
    //    '#Esic_Based_on',
    //    'Select ESIC Based On'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchPFEmployerShareDropdown', // ✅ Matches FetchPFEmployerShareDropdown
    //    '#Pf_Share_Mode_Employer',
    //    'Select PF Share Mode'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchVoluntaryPFDropdown', // ✅ Matches FetchVoluntaryPFDropdown
    //    '#VPF_Applicable',
    //    'Select VPF Applicability'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchVPFModeDropdown', // ✅ Matches FetchVPFModeDropdown
    //    '#VPF_Mode',
    //    'Select VPF Mode'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchESICApplicabilityDropdown', // ✅ Matches FetchESICApplicabilityDropdown
    //    '#Esic_Applicable',
    //    'Select ESIC Applicability'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchProfessionalTaxDropdown', // ✅ Matches FetchProfessionalTaxDropdown
    //    '#PT_Applicable',
    //    'Select PT Applicability'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchPTRegistrationModeDropdown', // ✅ Matches FetchPTRegistrationModeDropdown
    //    '#Pt_Regisdtration_Mode',
    //    'Select PT Registration Mode'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchLabourWelfareFundDropdown', // ✅ Matches FetchLabourWelfareFundDropdown
    //    '#Lwf_Mode',
    //    'Select LWF Mode'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchLWFCycleDropdown', // ✅ Matches FetchLWFCycleDropdown
    //    '#Lwf_Cycle',
    //    'Select LWF Cycle'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchTDSDeductedOnActualDateDropdown', // ✅ Matches FetchLWFCycleDropdown
    //    '#TDsDeducted_On_Actual_Date',
    //    'Select TDS Deducted On Actual Date'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchPFBasedOnDropdown', // ✅ Matches FetchLWFCycleDropdown
    //    '#Pf_Based_on',
    //    'Select PF Based On'
    //);
    /////////////////////////////////////////// BIND DROPDOWN For Second Tab Payroll Compliance Settings:- End ///////////////////////////


    //////////////////////////////////////////// BIND DROPDOWN For Third Tab Payroll Compliance Settings:- Start ///////////////////////////
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchEnablePayDropdown',
    //    '#Enable_Pay',
    //    'Select Enable Pay'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchSlipGenerationDropdown',
    //    '#Payslip_Generation',
    //    'Select Enable Pay Slip Generation'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchPayslipFormatDropdown',
    //    '#Payslip_Format',
    //    'Select Payslip Format'
    //);   
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchAutoNumberingDropdown',
    //    '#Auto_Numbering',
    //    'Select Auto Numbering'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchIsPayslipNoDropdown',
    //    '#IsPayslipNo_Reset',
    //    'Select Reset Payslip Numbering'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchDigitalSignatureDropdown', // You may want to replace this if there's a specific enum for Digital Signature
    //    '#DigitalSignatur_Requirede',
    //    'Select Digital Signature Requirement'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchAutoEmailPayslipsDropdown',
    //    '#PaySlipAutoEmail',
    //    'Select Auto Email Payslips'
    //);
    /////////////////////////////////////////// BIND DROPDOWN For Third Tab Payroll Compliance Settings:- End ///////////////////////////


    //////////////////////////////////////////// BIND DROPDOWN For Fourth Tab Payroll Compliance Settings:- Start ///////////////////////////
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchDataSyncTypeDropdown',
    //    '#DataSyncType',
    //    'Select Data Sync Type'
    //);
   
    //// Bind SyncFrequency dropdowns (all others)
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchSyncFrequencyDropdown',
    //    '#ContractorMaster',
    //    'Select Contractor Master'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchSyncFrequencyDropdown',
    //    '#LabourMaster',
    //    'Select Labour Master'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchSyncFrequencyDropdown',
    //    '#WorkOrder',
    //    'Select Work Details'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchSyncFrequencyDropdown',
    //    '#Attendance',
    //    'Select Attendance '
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchCommongYesNoDropdown',
    //    '#IsAttendanceProcessed',
    //    'Select Is Attendance Processed'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchSyncFrequencyDropdown',
    //    '#AttendanceProxcessType',
    //    'Select Attendance Process Type'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchCommongYesNoDropdown',
    //    '#IntegratedLogin',
    //    'Select Integrated Login '
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchPaymentFormatDropdown',
    //    '#PayregisterFormatId',
    //    'Select Pay RegisterFormat'
    //);
    //fetchAndBindDropdownForGlobal(
    //    '/DropDown/FetchCommongYesNoDropdown',
    //    '#IsContractLabourPayment',
    //    'Select Is ContractLabour Payment'
    //);
    //fetchAndBindMultiSelectDropdownForGlobal(
    //    '/DropDown/FetchEntityTypeDropdown',
    //    '#EntityTypeMigration',
    //    'Select Entity Type(s)'
    //);
    //bindCompanyDropdownOnPageLoad();
    //////////////////////////////////////////// BIND DROPDOWN For Fourth Tab Payroll Compliance Settings:- Start ///////////////////////////
    //$('#copySettingsModal').modal('show');
    $('#resetfirstTab').on('click', function () {
        //const isEditMode = $('#payComponentForm').data('edit-id') !== undefined;

        //if (isEditMode) {
        //    // Re-populate original values from DB
        //    const id = $('#payComponentForm').data('edit-id');
        //    if (id) {
        //        // Call the same AJAX logic to refill
        //        $('.paycomponent[data-id="' + id + '"]').trigger('click');
        //    }
        //} else {
            // Reset all fields for Add mode
            $('#firstTab')[0].reset();
            $(".select2_search_ctm").val(null).trigger("change");
            $(".input_error_msg").text("");           
            //$('#payComponentActiveToggle').prop('checked', false);
            //$('#activeStatusLabel').text('Inactive');
            $('#btnfirstTab').text('Add');
        //}
    });
});
$(document).ready(function () {
    ////////////////////////////////////////////First Tab Insert Data :- Start ///////////////////////////////////////////
    $('#btnfirstTab').click(function () {
        // 1. Read form values
        var salaryFrequency = $('#SalaryFrequency').val();
        var monthlySalaryBasedOn = $('#MonthlySalaryBasedOn').val();
        var roundSalary = $('#RoundSalary').val();
        var effectiveMonth = $('#EffectivePayrollStartMonth').val();
        var allowAdhoc = $('#AllowAdHocComponents').val();
        var lockSalary = $('#LockSalary').val();
        var companyIdForTabOne = $('#SelectedCompanyIdOnPageLoad').val();
        companyIdForTabOne = companyIdForTabOne && !isNaN(companyIdForTabOne) ? parseInt(companyIdForTabOne) : 0;
        // 2. Validate form
        var isValid = true;
        isValid &= validateFormRequired(salaryFrequency, "#SalaryFrequency-error", "Salary Frequency is required.");
        isValid &= validateFormRequired(monthlySalaryBasedOn, "#MonthlySalaryBasedOn-error", "Monthly Salary Based On is required.");
        isValid &= validateFormRequired(roundSalary, "#RoundSalary-error", "Round-off Salary is required.");
        isValid &= validateFormRequired(effectiveMonth, "#EffectivePayrollStartMonth-error", "Effective Payroll Start Month is required.");
        isValid &= validateFormRequired(allowAdhoc, "#AllowAdHocComponents-error", "Please select Allow Ad-Hoc Components.");
        isValid &= validateFormRequired(lockSalary, "#LockSalary-error", "Please select Lock Salary Edits option.");

        if (!isValid) return;

        // 4. Get Global Param ID safely
        var globalParamId = $('#GlobalParamId').val();
        globalParamId = globalParamId && !isNaN(globalParamId) ? parseInt(globalParamId) : 0;
        var isActive = $('#formulaActiveToggle').is(':visible') ? $('#formulaActiveToggle').is(':checked') : false;
        var data = {
            Global_Param_ID: globalParamId,
            Company_ID: companyIdForTabOne, 
            Salary_Frequency: parseInt(salaryFrequency),
            MonthlySalary_Based_On: parseInt(monthlySalaryBasedOn),
            Round_Off_Components: parseFloat(roundSalary),
            EffectivePayroll_start_Mnth: effectiveMonth,
            Allow_Adhoc_Components: allowAdhoc === "1",
            LOckSalary_Post_Payroll: lockSalary === "1",
            IsActive: isActive
        };
        //console.log(data);
        $.ajax({
            url: '/PayrollGlobalParameters/InsertPGlobalParameter', 
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                if (response.success) {
                    showAlert("success", response.message);                 
                    setTimeout(function () {
                        window.location.href = '/PayrollGlobalParameters/Index';
                    }, 5500);
                } else {
                    showAlert("danger", response.message);
                }
            },
            error: function (xhr, status, error) {                
                showAlert("danger", "An unexpected error occurred.");
            }
        });
    });
    ////////////////////////////////////////////First Tab Insert Data :- End ///////////////////////////////////////////

    ////////////////////////////////////////////Second Tab Insert Data :- Start ///////////////////////////////////////////
    $('#btnsecondTab').click(function (e) {
        e.preventDefault();

        // 1. Read form values
        var prmComplianceId = $('#Prm_Comlliance_ID').val();
        var pfApplicable = $('#Pf_Applicable').val();
        var pfShareMode = $('#Pf_Share_Mode_Employer').val();
        var epfShare = $('#Epf_Employer_Share_Percentage').val();
        var epsShare = $('#Eps_Employer_Share_Percentage').val();
        var vpfApplicable = $('#VPF_Applicable').val();
        var vpfMode = $('#VPF_Mode').val();
        var esicApplicable = $('#Esic_Applicable').val();
        var esicSalaryLimit = $('#Esic_Salary_Limit').val();
        var ptApplicable = $('#PT_Applicable').val();
        var ptRegMode = $('#Pt_Regisdtration_Mode').val();
        var lwfMode = $('#Lwf_Mode').val();
        var lwfCycle = $('#Lwf_Cycle').val();
        var lwfContribution = $('#Lwf_Contribution').val();
        var esiBasedOn = $('#Esic_Based_on').val();
        var tdsOnActualDate = $('#TDsDeducted_On_Actual_Date').val();
        var pfApplicablePercentage = $('#Pf_Applicable_Percentage').val();
        var pfBasedOn = $('#Pf_Based_on').val(); // ✅ New
        var esiApplicablePercentage = $('#Esi_Applicable_Percentage').val(); // ✅ New
        var selectedEntityTypes = $('#EntityTypeMigration').val() || [];
        var companyIdForTabTwo = $('#SelectedCompanyIdOnPageLoad').val();
        companyIdForTabTwo = companyIdForTabTwo && !isNaN(companyIdForTabTwo) ? parseInt(companyIdForTabTwo) : 0;
        // 2. Validate form (adjust as per your exact rules)
        var isValid = true;
        isValid &= validateFormRequired(pfApplicable, "#Pf_Applicable-error", "PF Applicability is required.");
        isValid &= validateFormRequired(pfShareMode, "#Pf_Share_Mode_Employer-error", "PF Share Mode is required.");
        isValid &= validateFormRequired(epfShare, "#Epf_Employer_Share_Percentage-error", "EPF % is required.");
        isValid &= validateFormRequired(epsShare, "#Eps_Employer_Share_Percentage-error", "EPS % is required.");
        isValid &= validateFormRequired(esicSalaryLimit, "#Esic_Salary_Limit-error", "ESIC Limit is required.");
        isValid &= validateFormRequired(ptApplicable, "#PT_Applicable-error", "PT Applicability is required.");
        isValid &= validateFormRequired(lwfMode, "#Lwf_Mode-error", "LWF Mode is required.");
        isValid &= validateFormRequired(lwfContribution, "#Lwf_Contribution-error", "LWF Contribution is required.");
        isValid &= validateFormRequired(esiApplicablePercentage, "#Esi_Applicable_Percentage-error", "ESI Applicable % is required.");

        if (!isValid) return;

        // 3. Construct the DTO
        var data = {
            Prm_Comlliance_ID: parseInt(prmComplianceId || 0),
            Company_ID: companyIdForTabTwo, 
            Pf_Applicable: parseInt(pfApplicable),
            Pf_Share_Mode_Employer: parseInt(pfShareMode),
            Epf_Employer_Share_Percentage: parseFloat(epfShare),
            Eps_Employer_Share_Percentage: parseFloat(epsShare),
            VPF_Applicable: vpfApplicable === "1",
            VPF_Mode: parseInt(vpfMode || 0),
            Esic_Applicable: esicApplicable === "1",
            Esic_Salary_Limit: parseFloat(esicSalaryLimit),
            PT_Applicable: ptApplicable === "1",
            Pt_Regisdtration_Mode: parseInt(ptRegMode || 0),
            Lwf_Mode: parseInt(lwfMode),
            Lwf_Cycle: parseInt(lwfCycle || 0),
            Lwf_Contribution: parseFloat(lwfContribution),
            Esi_Based_on: parseInt(esiBasedOn || 0),
            Pf_Based_on: parseInt(pfBasedOn || 0), // ✅ Required by backend
            Esi_Applicable_Percentage: parseFloat(esiApplicablePercentage || 0) // ✅ Required by backend
        };
        data.Pf_Applicable_Percentage = parseFloat(pfApplicablePercentage || 0);
        data.TDsDeducted_On_Actual_Date = tdsOnActualDate === "" ? null : (tdsOnActualDate === "true");
        //console.log(data);

        //// 4. AJAX call to controller
        $.ajax({
            url: '/PayrollGlobalParameters/InsertComplianceSettings',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                if (response.success) {
                    showAlert("success", response.message);
                    setTimeout(function () {
                        window.location.href = '/PayrollGlobalParameters/Index';
                    }, 5500);
                } else {
                    showAlert("danger", response.message);
                }
            },
            error: function (xhr, status, error) {
                showAlert("danger", "An unexpected error occurred.");
            }
        });
    });
    ////////////////////////////////////////////Second Tab Insert Data :- End ///////////////////////////////////////////

    ////////////////////////////////////////////Third Tab Insert Data :- Start ///////////////////////////////////////////
    $('#btnthirdTab').click(function (e) {
        e.preventDefault();

        // 1. Gather form field values
        var pSlipId = parseInt($('#Payroll_Setin_ID').val() || 0);
        //var companyId = parseInt($('#Company_ID').val() || 0);
        var initialChar = $('#Initial_char').val();
      
        var paySlipFormat = parseInt($('#Payslip_Format').val());
        var paySlipIdFormat = $('#PayslipNumber_Format').val();
        var paySlipScope = parseInt($('#PaySlip_Number_Scope').val());
        var paySlipGeneration = $('#Payslip_Generation').val() === "1";
        var autoNumbering = $('#Auto_Numbering').val() === "1";
        var resetNumbering = $('#IsPayslipNo_Reset').val() === "1";
        var digitalSignature = $('#DigitalSignatur_Requirede').val() === "1";
        var autoEmail = $('#PaySlipAutoEmail').val() === "1";
        var companyIdForTabThree = $('#SelectedCompanyIdOnPageLoad').val();
        companyIdForTabThree = companyIdForTabThree && !isNaN(companyIdForTabThree) ? parseInt(companyIdForTabThree) : 0;


        // 2. Construct DTO object
        var data = {
            Payroll_Setin_ID: pSlipId,
            Company_ID: companyIdForTabThree, 
            Initial_char: initialChar,
            Payslip_Generation: paySlipGeneration,
            Payslip_Format: paySlipFormat,
            PayslipNumber_Format: paySlipIdFormat,
            PaySlip_Number_Scope: paySlipScope,
            Auto_Numbering: autoNumbering,
            IsPayslipNo_Reset: resetNumbering,
            DigitalSignatur_Requirede: digitalSignature,
            PaySlipAutoEmail: autoEmail
        };

        //console.log(data); // Optional: debug

        // 3. Make AJAX POST request to controller
        $.ajax({
            url: '/PayrollGlobalParameters/InsertPaySlipSetting',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                if (response.success) {
                    showAlert("success", response.message);
                    setTimeout(function () {
                        window.location.href = '/PayrollGlobalParameters/Index';
                    }, 5500);
                } else {
                    showAlert("danger", response.message);
                }
            },
            error: function (xhr, status, error) {
                showAlert("danger", "An unexpected error occurred.");
            }
        });
    });
    ////////////////////////////////////////////Third Tab Insert Data :- End ///////////////////////////////////////////

    ////////////////////////////////////////////Fourth Tab Insert Data :- Start ///////////////////////////////////////////
    $('#btnfourthTab').click(function (e) {
        e.preventDefault();

        // Gather dropdown values
        //var dataSyncType = parseInt($('#DataSyncType').val() || 0);
        //var contractorMaster = parseInt($('#ContractorMaster').val() || 0);
        //var labourMaster = parseInt($('#LabourMaster').val() || 0);
        //var workOrder = parseInt($('#WorkOrder').val() || 0);
        //var attendance = parseInt($('#Attendance').val() || 0);
        var selectedEntityTypes = $('#EntityTypeMigration').val() || [];
        var companyIdForTabFour = $('#SelectedCompanyIdOnPageLoad').val();
        companyIdForTabFour = companyIdForTabFour && !isNaN(companyIdForTabFour) ? parseInt(companyIdForTabFour) : 0;

        var data = {
            Clms_Param_ID: parseInt($('#Clms_Param_ID').val() || 0), 
            /*Company_Id: parseInt($('#CompanyId').val() || 0),*/
            DataSync: $('#DataSyncType').val() === "1",
            Entityparam: "", 
            Contractlabour_payment: $('#IsContractLabourPayment').val() === "1",
            IsAttendanceProcessed: $('#IsAttendanceProcessed').val() === "1",
            AttendanceProxcessType: parseInt($('#AttendanceProxcessType').val() || 0),
            Wo_Sync_Frequency: parseInt($('#ContractorMaster').val() || 0),
            Entity_Sync_Frequency: parseInt($('#Attendance').val() || 0),
            IntegratedLog_in: $('#IntegratedLogin').val() === "1",
            PayregisterFormat_ID: parseInt($('#PayregisterFormatId').val() || 0),
            WorkOrder_Sync_Frequency: parseInt($('#WorkOrder').val() || 0),
            Cl_Sync_Frequency: parseInt($('#LabourMaster').val() || 0)
        };
        data.Entityparam = selectedEntityTypes.join(',');
        data.Company_ID = companyIdForTabFour;
        //console.log(data); // Debug if needed

        // Make AJAX POST request
        $.ajax({
            url: '/PayrollGlobalParameters/InsertThirdPartyData',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                if (response.success) {
                    showAlert("success", response.message);
                    setTimeout(function () {
                        window.location.href = '/PayrollGlobalParameters/Index';
                    }, 5500);
                } else {
                    showAlert("danger", response.message);
                }
            },
            error: function (xhr, status, error) {
                showAlert("danger", "An unexpected error occurred.");
            }
        });
    });
    ////////////////////////////////////////////Fourth Tab Insert Data :- End ///////////////////////////////////////////
});
function fetchAndBindDropdownForGlobal(url, dropdownSelector, placeholder, data = null) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: url,
            type: 'GET',
            data: data ?? {},
            success: function (response) {
                const $dropdown = $(dropdownSelector);

                if ($.fn.select2 && $dropdown.hasClass("select2-hidden-accessible")) {
                    $dropdown.select2('destroy');
                }

                $dropdown.empty().append(`<option value="">${placeholder}</option>`);

                response.forEach(item => {
                    $dropdown.append(new Option(item.text, item.value));
                });

                if ($.fn.select2) {
                    $dropdown.select2({
                        width: '100%',
                        dropdownAutoWidth: true
                    });
                }

                resolve(); // ✅ done
            },
            error: function (error) {
                // Optional: log or show toast
                reject(error); // ❌ signal failure
            }
        });
    });
}
function fetchAndBindMultiSelectDropdownForGlobal(url, dropdownSelector, placeholder, data = null, selectedValues = []) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: url,
            type: 'GET',
            data: data ?? {},
            success: function (response) {
                const $dropdown = $(dropdownSelector);

                if ($.fn.select2 && $dropdown.hasClass("select2-hidden-accessible")) {
                    $dropdown.select2('destroy');
                }

                $dropdown.empty();

                if (Array.isArray(response)) {
                    response.forEach(item => {
                        const option = new Option(item.text, item.value.trim(), false, false);
                        $dropdown.append(option);
                    });
                }

                $dropdown.attr("multiple", "multiple");

                if ($.fn.select2) {
                    $dropdown.select2({
                        width: '100%',
                        dropdownAutoWidth: true,
                        placeholder: placeholder
                    });
                }

                if (selectedValues.length > 0) {
                    $dropdown.val(selectedValues).trigger('change');
                }

                resolve(); // ✅ done
            },
            error: function (error) {
                reject(error); // ❌
            }
        });
    });
}


//function fetchAndBindDropdownForGlobal(url, dropdownSelector, placeholder, data = null) {
//    $.ajax({
//        url: url,
//        type: 'GET',
//        data: data ?? {},
//        success: function (response) {
//            const $dropdown = $(dropdownSelector);

//            // Destroy select2 if already initialized
//            if ($.fn.select2 && $dropdown.hasClass("select2-hidden-accessible")) {
//                $dropdown.select2('destroy');
//            }

//            // Clear and append placeholder
//            $dropdown.empty().append(`<option value="">${placeholder}</option>`);

//            // Append items
//            response.forEach(function (item) {
//                $dropdown.append(new Option(item.text, item.value));
//            });

//            // Reapply custom select2 style
//            if ($.fn.select2) {
//                $dropdown.select2({
//                    width: '100%', // Ensure it spans correctly
//                    dropdownAutoWidth: true
//                });

//                // Optional: trigger change if needed
//                //$dropdown.trigger('change');
//            }
//        },
//        error: function (error) {
//            //console.error('Dropdown load failed:', error);
//        }
//    });
//}

//function fetchAndBindMultiSelectDropdownForGlobal(url, dropdownSelector, placeholder, data = null, selectedValues = []) {
//    $.ajax({
//        url: url,
//        type: 'GET',
//        data: data ?? {},
//        success: function (response) {
//            //console.log('Dropdown data:', response);

//            const $dropdown = $(dropdownSelector);

//            if ($.fn.select2 && $dropdown.hasClass("select2-hidden-accessible")) {
//                $dropdown.select2('destroy');
//            }

//            $dropdown.empty();

//            if (response && Array.isArray(response)) {
//                response.forEach(function (item) {
//                    const option = new Option(item.text, item.value.trim(), false, false);
//                    $dropdown.append(option);
//                });
//            }

//            $dropdown.attr("multiple", "multiple");

//            if ($.fn.select2) {
//                $dropdown.select2({
//                    width: '100%',
//                    dropdownAutoWidth: true,
//                    placeholder: placeholder
//                });
//            }

//            // ✅ Set selected values if available
//            if (selectedValues.length > 0) {
//                $dropdown.val(selectedValues).trigger('change');
//            }
//        },
//        error: function (error) {
//            //console.error('Multi-select dropdown load failed:', error);
//        }
//    });
//}
//function bindCompanyDropdown(dropdownMenuSelector, excludeSessionCompany = false) {
//    $.ajax({
//        url: '/PayrollMonth/GetCompanyProfilesListJson',
//        type: 'GET',
//        success: function (data) {
//            if (data) {
//                var dropdownMenu = $(dropdownMenuSelector);
//                dropdownMenu.empty();

//                // Add default item
//                dropdownMenu.append(
//                    $('<li>').append(
//                        $('<a>')
//                            .addClass('dropdown-item')
//                            .attr('href', '#')
//                            .attr('data-id', '')
//                            .text('Please Select Value')
//                    )
//                );

//                //var sessionCompanyId = $('#SessionCompanyId').val();
//                var sessionCompanyId = $('#SessionCompanyId').val();
//                var matchedCompanyName = '';
//                console.log(sessionCompanyId);
//                //$.each(data, function (index, company) {
//                //    if (!excludeSessionCompany || company.company_Id != sessionCompanyId) {
//                //        dropdownMenu.append(
//                //            $('<li>').append(
//                //                $('<a>')
//                //                    .addClass('dropdown-item')
//                //                    .attr('href', '#')
//                //                    .attr('data-id', company.company_Id)
//                //                    .text(company.companyName)
//                //            )
//                //        );
//                //    }
//                //});
//                $.each(data, function (index, company) {
//                    if (!excludeSessionCompany || company.company_Id != sessionCompanyId) {
//                        var listItem = $('<a>')
//                            .addClass('dropdown-item')
//                            .attr('href', '#')
//                            .attr('data-id', company.company_Id)
//                            .text(company.companyName);

//                        dropdownMenu.append($('<li>').append(listItem));

//                        if (company.company_Id == sessionCompanyId) {
//                            matchedCompanyName = company.companyName;
//                        }
//                    }
//                });
//                if (!excludeSessionCompany && sessionCompanyId && matchedCompanyName) {
//                    $('#SelectedCompanyIdOnPageLoad').val(sessionCompanyId);
//                    $('#dropdownTextboxOnPageLoad').text(matchedCompanyName);
//                }
//            }
//        },
//        error: function () {
//            //console.error('Failed to load company list.');
//        }
//    });
//}

function bindCompanyDropdownOnPageLoad() {
    var sessionCompanyId = parseInt($('#SessionCompanyId').val());
    console.log("sessionCompanyId", sessionCompanyId);
    $.ajax({
        //url: '/PayrollMonth/GetCompanyProfilesListJson',
        url: '/PayrollPeriod/GetCompanyProfilesListJson',
        type: 'GET',
        success: function (data) {
            if (data) {
                var dropdownMenu = $('#dropdownMenuOnPageLoad');
                dropdownMenu.empty();

                dropdownMenu.append(
                    $('<li>').append(
                        $('<a>')
                            .addClass('dropdown-item')
                            .attr('href', '#')
                            .attr('data-id', '')
                            .text('Please Select Value')
                    )
                );

                var matchedCompanyName = '';

                $.each(data, function (index, company) {
                    var listItem = $('<a>')
                        .addClass('dropdown-item')
                        .attr('href', '#')
                        .attr('data-id', company.company_Id)
                        .text(company.companyName);

                    dropdownMenu.append($('<li>').append(listItem));

                    if (company.company_Id === sessionCompanyId) {
                        matchedCompanyName = company.companyName;
                    }
                });

                if (matchedCompanyName) {
                    $('#dropdownTextboxOnPageLoad').text(matchedCompanyName);
                    $('#SelectedCompanyIdOnPageLoad').val(sessionCompanyId);
                }
            }
        },
        error: function () {
            console.error('Failed to load company list.');
        }
    });
}
function bindCompanyDropdownInModalPopup() {
    var selectedCompanyId = $('#SelectedCompanyIdOnPageLoad').val();
    console.log("MY NEW COMPANY SELECTED",selectedCompanyId);
    $.ajax({
       // url: '/PayrollMonth/GetCompanyProfilesListJson',
        url: '/PayrollPeriod/GetCompanyProfilesListJson',
        type: 'GET',
        success: function (data) {
            if (data) {
                var dropdownMenu = $('#dropdownMenuCompanyOnModelPopUp');
                dropdownMenu.empty();

                dropdownMenu.append(
                    $('<li>').append(
                        $('<a>')
                            .addClass('dropdown-item')
                            .attr('href', '#')
                            .attr('data-id', '')
                            .text('Please Select Value')
                    )
                );

                $.each(data, function (index, company) {
                    if (company.company_Id != selectedCompanyId) {
                        var listItem = $('<a>')
                            .addClass('dropdown-item')
                            .attr('href', '#')
                            .attr('data-id', company.company_Id)
                            .text(company.companyName);

                        dropdownMenu.append($('<li>').append(listItem));
                    }
                });
            }
        },
        error: function () {
            console.error('Failed to load company list.');
        }
    });
}

///////////////////////////////////////////////////////////Copy From Company :- Start////////////////////////////////////////////////////////
$(document).ready(function () {
    // On "Yes" click, hide first modal and show second modal
    $('#yesSettingPermission').on('click', function () {
        $(this).blur(); // Remove focus from the button

        $('#copySettingsModal').modal('hide');
        setTimeout(function () {        
            $('#selectCompanySettingModal').modal('show');
            bindCompanyDropdownInModalPopup();
        }, 500);
    });
    $('#noSettingPermission').on('click', function () {
        $(this).blur(); // Optional: remove focus
        $('#copySettingsModal').modal('hide'); // Hide the modal
    });

    // Handle click on dropdown items to update button text and hidden input

    //$(document).on('click', '#dropdownMenuOnPageLoad a.dropdown-item', function (e) {
    //    e.preventDefault();
    //    var selectedText = $(this).text();
    //    var selectedId = $(this).data('id');
    //    $('#dropdownTextboxOnPageLoad').text(selectedText);
    //    $('#SelectedCompanyIdOnPageLoad').val(selectedId || '');
    //});

    function resetAllPayrollForms() {
        // Reset each form by ID
        $('#firstTab')[0].reset();
        $('#secondTab')[0].reset();
        $('#thirdTab')[0].reset();
        $('#fourthTab')[0].reset();

        // Also reset any select2 dropdowns
        $('.select2_search_ctm').val(null).trigger('change');

        // Reset toggle switches and labels if needed
        $('#formulaActiveToggle').prop('checked', false);
        $('#formulaStatusLabel').text('Inactive');

        $('#formulaActiveToggleTabSecond').prop('checked', false);
        $('#formulaStatusLabelTabSecond').text('Inactive');

        $('#formulaActiveToggleTabThird').prop('checked', false);
        $('#formulaStatusLabelTabThird').text('Inactive');

        $('#formulaActiveToggleTabFourth').prop('checked', false);
        $('#formulaStatusLabelTabFourth').text('Inactive');
        $("#toggleContainer, #toggleContainerTabSecond, #toggleContainerTabThird, #toggleContainerTabFourth").hide();
    }

    function fetchPayrollSettings(companyId) {
        $.ajax({
            url: '/PayrollGlobalParameters/FetchPayrollSettings?companyId=' + companyId,
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    // Bind data as you already do
                    bindPayrollSettings(response); 
                    console.log('Payroll settings:', response.data);
                } else {
                    alert('No data found');
                    // Reset all form tabs
                    resetAllPayrollForms();
                }
            },
            error: function () {
                alert('Error fetching payroll settings.');
            }
        });
    }
    function bindPayrollSettings(response) {
        var hasValidData = false;

        //////////////////////////// Bind 1st Tab:- Start/////////////////////////////
        if (response.success && response.data && response.data.globalParams) {
            hasValidData = true;
            var payrollParam = response.data.globalParams;
            if (payrollParam) {
                $('#GlobalParamId').val(payrollParam.global_Param_ID);
                $('#SalaryFrequency').val(payrollParam.salary_Frequency ?? '').trigger('change');
                $('#MonthlySalaryBasedOn').val(payrollParam.monthlySalary_Based_On ?? '').trigger('change');
                $('#EffectivePayrollStartMonth').val(payrollParam.effectivePayroll_start_Mnth ?? '').trigger('change');
                $('#AllowAdHocComponents').val(payrollParam.allow_Adhoc_Components ? 1 : 0).trigger('change');
                $('#LockSalary').val(payrollParam.lOckSalary_Post_Payroll ? 1 : 0).trigger('change');

                if (payrollParam.global_Param_ID) {
                    $('#toggleContainer').show();
                    $('#formulaActiveToggle').prop('checked', payrollParam.isActive);
                    $('#formulaStatusLabel').text(payrollParam.isActive ? 'Active' : 'Inactive');
                }
            }

        }

        //////////////////////////// Bind 2nd Tab:- Start/////////////////////////////
        if (response.success && response.data && response.data.compliances) {
            hasValidData = true;
            const comp = response.data.compliances;
            $('#Prm_Comlliance_ID').val(comp.prm_Comlliance_ID ?? '');

            $('#Pf_Applicable').val(comp.pf_Applicable ?? '').trigger('change');
            $('#Pf_Share_Mode_Employer').val(comp.pf_Share_Mode_Employer ?? '').trigger('change');
            $('#Epf_Employer_Share_Percentage').val(comp.epf_Employer_Share_Percentage ?? '');
            $('#Eps_Employer_Share_Percentage').val(comp.eps_Employer_Share_Percentage ?? '');
            $('#VPF_Applicable').val(comp.vpF_Applicable ? 1 : 0).trigger('change');
            $('#VPF_Mode').val(comp.vpF_Mode ?? '').trigger('change');
            $('#Esic_Applicable').val(comp.esic_Applicable ? 1 : 0).trigger('change');
            $('#Esic_Salary_Limit').val(comp.esic_Salary_Limit ?? '');
            $('#PT_Applicable').val(comp.pT_Applicable ? 1 : 0).trigger('change');
            $('#Pt_Regisdtration_Mode').val(comp.pt_Registration_Mode ?? '').trigger('change');
            $('#Lwf_Mode').val(comp.lwf_Mode ?? '').trigger('change');
            $('#Lwf_Cycle').val(comp.lwf_Cycle ?? '').trigger('change');
            $('#Lwf_Contribution').val(comp.lwf_Contribution ?? '');
            $('#TDsDeducted_On_Actual_Date').val(comp.tDsDeducted_On_Actual_Date ? 1 : 0).trigger('change');
            $('#Esi_Applicable_Percentage').val(comp.esi_Applicable_Percentage ?? '');

            if (comp.prm_Comlliance_ID && comp.prm_Comlliance_ID > 0) {
                $('#toggleContainerTabSecond').show();
                $('#formulaActiveToggleTabSecond').prop('checked', comp.isActive === true);
                $('#formulaStatusLabelTabSecond').text(comp.isActive ? 'Active' : 'Inactive');
            } else {
                $('#toggleContainerTabSecond').hide();
            }
        }

        //////////////////////////// Bind 3rd Tab:- Start/////////////////////////////
        if (response.success && response.data && response.data.settings) {
            hasValidData = true;
            var settings = response.data.settings;

            $('#Payroll_Setin_ID').val(settings.payroll_Setin_ID ?? '');
            $('#Initial_char').val(settings.initial_char ?? '');

            $('#Enable_Pay').val(settings.payslip_Generation ? 1 : 0).trigger('change');
            $('#Payslip_Generation').val(settings.payslip_Generation ? 1 : 0).trigger('change');
            $('#Payslip_Format').val(settings.payslip_Format ?? '').trigger('change');
            $('#PayslipNumber_Format').val(settings.payslipNumber_Format ?? '');
            $('#PaySlip_Number_Scope').val(settings.paySlip_Number_Scope ?? '');
            $('#Auto_Numbering').val(settings.auto_Numbering ? 1 : 0).trigger('change');
            $('#IsPayslipNo_Reset').val(settings.isPayslipNo_Reset ? 1 : 0).trigger('change');
            $('#DigitalSignatur_Requirede').val(settings.digitalSignatur_Requirede ? 1 : 0).trigger('change');
            $('#PaySlipAutoEmail').val(settings.paySlipAutoEmail ? 1 : 0).trigger('change');

            if (settings.payroll_Setin_ID && settings.payroll_Setin_ID > 0) {
                $('#toggleContainerTabThird').show();
                $('#formulaActiveToggleTabThird').prop('checked', settings.isActive === true);
                $('#formulaStatusLabelTabThird').text(settings.isActive ? 'Active' : 'Inactive');
            } else {
                $('#toggleContainerTabThird').hide();
            }
        }

        //////////////////////////// Bind 4th Tab:- Start /////////////////////////////
        if (response.success && response.data && response.data.thirdPartyParams) {
            hasValidData = true;
            var thirdParty = response.data.thirdPartyParams;

            $('#Clms_Param_ID').val(thirdParty.clms_Param_ID ?? '');
            $('#DataSyncType').val(thirdParty.dataSync ? 1 : 0).trigger('change');
            $('#WorkOrder').val(thirdParty.wo_Sync_Frequency ?? '').trigger('change');
            $('#ContractorMaster').val(thirdParty.workOrder_Sync_Frequency ?? '').trigger('change');
            $('#LabourMaster').val(thirdParty.entity_Sync_Frequency ?? '').trigger('change');
            $('#IsContractLabourPayment').val(thirdParty.contractlabour_payment ? 1 : 0).trigger('change');
            $('#IsAttendanceProcessed').val(thirdParty.isAttendanceProcessed ? 1 : 0).trigger('change');
            $('#Attendance').val(thirdParty.entity_Sync_Frequency ?? '').trigger('change');
            $('#IntegratedLogin').val(thirdParty.integratedLog_in ? 1 : 0).trigger('change');
            $('#PayregisterFormatId').val(thirdParty.payregisterFormat_ID ?? '').trigger('change');
            $('#AttendanceProxcessType').val(thirdParty.attendanceProxcessType ?? '').trigger('change');

            if (thirdParty.clms_Param_ID && thirdParty.clms_Param_ID > 0) {
                $('#toggleContainerTabFourth').show();
                $('#formulaActiveToggleTabFourth').prop('checked', thirdParty.isActive === true);
                $('#formulaStatusLabelTabFourth').text(thirdParty.isActive ? 'Active' : 'Inactive');
            } else {
                $('#toggleContainerTabFourth').hide();
            }

            var selectedEntities = (thirdParty.entityparam || "").split(',').map(s => s.trim());
            fetchAndBindMultiSelectDropdownForGlobal(
                '/DropDown/FetchEntityTypeDropdown',
                '#EntityTypeMigration',
                'Select Entity Type(s)',
                null,
                selectedEntities
            );
        }

        /////////////////////////// Hide/Show Button Based on Data Availability ///////////////////////////
        if (hasValidData) {
            $('#btnCopySettings').show();
        } else {
            $('#btnCopySettings').hide();
        }
    }

    $('#dropdownMenuOnPageLoad').on('click', '.dropdown-item', function (e) {
        e.preventDefault();

        var selectedId = $(this).data('id');
        var selectedText = $(this).text();

        $('#SelectedCompanyIdOnPageLoad').val(selectedId);
        $('#dropdownTextboxOnPageLoad').text(selectedText);

        if (selectedId) {
            fetchPayrollSettings(selectedId);
        }
    });

    $(document).on('click', '#dropdownMenuCompanyOnModelPopUp a.dropdown-item', function (e) {
        e.preventDefault();
        var selectedText = $(this).text();
        var selectedId = $(this).data('id');
        $('#dropdownCompanyOnModelPopUp').text(selectedText);
        $('#SelectedCompanyIdOnModelPopUp').val(selectedId || '');
    });
    $('.modal .btn[data-bs-dismiss="modal"]').on('click', function () {
        $(this).blur();
    });
    $('#btnCopySettings').click(function (e) {
        e.preventDefault(); // prevent default button behavior if any
        $('#selectCompanySettingModal').modal('show');
        // Bind the dropdown inside modal, excluding the selected company
        bindCompanyDropdownInModalPopup();
    });
});

//////////////////////////////////////////////////////////Copy From Company:- End///////////////////////////////////////////////////////////
////////////////////////////////////////////////////////// Bind STATIC Setting Type (Multi-Select):- Start ////////////////////////////////////////////////
$(document).ready(function () {
    var items = [
        { id: 1, text: "Global Settings" },
        { id: 2, text: "Compliance Settings" },
        { id: 3, text: "Payroll Settings" },
        { id: 4, text: "Integration & Mapping Settings" }
    ];

    var dropdown = $('#dropdownMenuSettingsType');
    dropdown.empty();

    // Create list items with checkboxes
    items.forEach(function (item) {
        var li = $('<li/>').append(
            $('<div class="form-check dropdown-item"/>').append(
                $('<input type="checkbox" class="form-check-input settings-type-checkbox"/>')
                    .attr('id', 'settingType_' + item.id)
                    .val(item.id),
                $('<label class="form-check-label"/>')
                    .attr('for', 'settingType_' + item.id)
                    .text(item.text)
            )
        );
        dropdown.append(li);
    });

    // Handle checkbox change
    $(document).on('change', '.settings-type-checkbox', function () {
        var selected = [];
        $('.settings-type-checkbox:checked').each(function () {
            selected.push($(this).val());
        });

        $('#SelectedSettingsTypeId').val(selected.join(','));

        if (selected.length === 0) {
            $('#dropdownSettingsType').text('Select Settings Type');
        } else if (selected.length === 1) {
            $('#dropdownSettingsType').text('Selected: ' + selected[0]);
        } else {
            $('#dropdownSettingsType').text(selected.length + ' Selected');
        }
    });

    $('#btnCopyFromCompany').click(function () {
        var companyId = $('#SelectedCompanyIdOnModelPopUp').val();
        var settingsType = $('#SelectedSettingsTypeId').val(); // comma separated string "1,2,3"

        if (!companyId || !settingsType) {
            alert('Please select both Company and Settings Type.');
            return;
        }

        $.ajax({
            url: '/PayrollGlobalParameters/CopySettingsFromCompany',  // Adjust controller name if different
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                CopyToCompanyID: parseInt(companyId),
                SelectParam: settingsType
            }),
            success: function (response) {
                if (response.success) {
                    $(this).blur(); // Optional: remove focus
                    $('#selectCompanySettingModal').modal('hide'); 
                    showAlert("success", response.message);
                    setTimeout(function () {
                        window.location.href = '/PayrollGlobalParameters/Index';
                    }, 5500);
                } else {
                    alert('Failed: ' + response.message);
                }
            },
            error: function (xhr) {
                alert('Something went wrong.');
                console.error(xhr.responseText);
            }
        });
    });
});
////////////////////////////////////////////////////////// Bind STATIC Setting Type (Multi-Select):- End ////////////////////////////////////////////////
