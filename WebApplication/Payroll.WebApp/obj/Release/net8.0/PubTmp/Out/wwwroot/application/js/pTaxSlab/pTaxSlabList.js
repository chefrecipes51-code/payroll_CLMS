/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 08-05-2025
///  IMP NOTE      :-
///                1) IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>
////////////////////////////////////////////Binding Dropdown Down On Model POPUP:=Start//////////////////////////////////////////

function fetchAndBindDropdown(url, dropdownSelector, placeholder, data = null) {
    $.ajax({
        url: url,
        type: 'GET',
        data: data ?? {}, // Only pass data if provided
        success: function (response) {
            if (response && response.length > 0) {
                // Destroy select2 if already initialized
                if ($.fn.select2 && $(dropdownSelector).hasClass("select2-hidden-accessible")) {
                    $(dropdownSelector).select2('destroy');
                }

                // Clear and repopulate dropdown
                $(dropdownSelector).empty().append(`<option value="">${placeholder}</option>`);
                response.forEach(function (item) {
                    $(dropdownSelector).append(new Option(item.text, item.value));
                });

                // Reinitialize select2
                $(dropdownSelector).select2({
                    dropdownParent: $('#addProfessionalTaxSlab')
                });
            }
        },
        error: function (error) {
            //console.log('Error fetching data:', error);
        }
    });
}
////////////////////////////////////////////Binding Dropdown Down On Model POPUP:=End//////////////////////////////////////////
function resetPTaxSlabForm() {
    $('#professionalTaxSlabForm')[0].reset();
    $('.input_error_msg').text('');
    resetAndRebindDropdown('#state');
    resetAndRebindDropdown('#frequency');
    resetAndRebindDropdown('#gender');
    hideTaxFields();
    $('#isYearEndAdjustmentWrapper').hide(); // hide checkbox section
    $('#statusToggleWrapper').hide();        // hide status toggle
    $('#pTaxSlabActiveToggle').prop('checked', true); // reset toggle
    $('#pTaxSlabStatusLabel').text('Active');         // reset label text
    // Reset checkbox
    $('#isYearEndAdjustment').prop('checked', false);
    // Reset hidden Id
    $('#ptaxSlabId').val('');
    $('#professionalTaxSlabTitle').text('Add Professional Tax Slab');
    $('#btnSavePTaxSlab').text('Add');
}
//function resetAndRebindDropdown(selector) {
//    if ($.fn.select2 && $(selector).hasClass("select2-hidden-accessible")) {
//        $(selector).val('').trigger('change');
//        $(selector).select2('destroy');
//    }
//    if (selector === '#frequency') {
//        fetchAndBindDropdown('/DropDown/FetchPayTaxFrequencyDropdown', selector, 'Select Frequency');
//    } else if (selector === '#gender') {
//        fetchAndBindDropdown('/DropDown/FetchGenderDropdown', selector, 'Select Gender');
//    } else if (selector === '#state') {
//        fetchAndBindDropdown('/DropDown/FetchCommonStateDropdown', selector, 'Select State', { Country_Id: 99 });
//    }
//}
function resetAndRebindDropdown(selector) {
    const $el = $(selector);

    // Always destroy first if already initialized
    if ($.fn.select2 && $el.hasClass("select2-hidden-accessible")) {
        try {
            $el.select2('destroy');
        } catch (e) {
            //console.warn("Select2 destroy failed:", e);
        }
    }

    // Clear old options and add placeholder
    $el.empty().append('<option value="">Loading...</option>');

    // Determine dropdown source
    let url = '';
    let placeholder = '';
    let data = {};

    switch (selector) {
        case '#frequency':
            url = '/DropDown/FetchPayTaxFrequencyDropdown';
            placeholder = 'Select Frequency';
            break;
        case '#gender':
            url = '/DropDown/FetchGenderDropdown';
            placeholder = 'Select Gender';
            break;
        case '#state':
            url = '/DropDown/FetchCommonStateDropdown';
            placeholder = 'Select State';
            data = { Country_Id: 99 };
            break;
    }

    if (!url) return;

    // Fetch data
    $.ajax({
        url: url,
        type: 'GET',
        data: data,
        success: function (response) {
            $el.empty().append(`<option value="">${placeholder}</option>`);
            $.each(response, function (i, item) {
                $el.append(new Option(item.text, item.value));
            });

            // Reinitialize select2 after DOM update
            setTimeout(function () {
                $el.select2({
                    dropdownParent: $('#addProfessionalTaxSlab'),
                    width: '100%' // ensure full width
                });
            }, 10); // slight delay to ensure DOM updated
        },
        error: function () {
            toastr.error("Failed to load dropdown data.");
        }
    });
}

function hideTaxFields() {
    $('#gender').closest('.col-lg-6').hide();
    $('#srCitizenAge').closest('.col-lg-6').hide();
    $('#specialPTaxAmt').closest('.col-lg-6').hide();
    $('#age').closest('.col-lg-6').hide();
}
function showTaxFields() {
    $('#gender').prop('disabled', false).closest('.col-lg-6').show();
    $('#srCitizenAge').prop('disabled', false).closest('.col-lg-6').show();
    $('#specialPTaxAmt').prop('disabled', false).closest('.col-lg-6').show();
    $('#age').prop('disabled', false).closest('.col-lg-6').show();
}
function formatDateForApi(dateStr) {
    if (!dateStr) return null;
    const parts = dateStr.split('/');
    if (parts.length === 3) {
        // Assuming your input is in MM/DD/YYYY format
        const [month, day, year] = parts;
        return `${year}-${month.padStart(2, '0')}-${day.padStart(2, '0')}`;
    }
    return dateStr; // fallback
}
$(document).ready(function () {
    // Handle Add button click
    $('.btn-add-hide').on('click', function () {
        resetPTaxSlabForm();
    });   
    $('#state').on('change', function () {
        var stateId = $(this).val(); // Get selected state id

        if (stateId) {
            $.ajax({
                url: `/ProfessionalTaxSlab/getTaxParamsByState?stateId=${stateId}`,
                type: 'GET',  // Using GET method for fetching data
                success: function (data) {
                    //console.log('Tax Parameters:', data);
                    if (data.isSuccess && data.result && data.result.length > 0) {
                        var taxParam = data.result[0];      
                        //console.log(taxParam);
                        showTaxFields();
                        switch (taxParam.gender) {
                            case 'M':
                                $('#gender').val('1').trigger('change');
                                break;
                            case 'F':
                                $('#gender').val('2').trigger('change');
                                break;
                            case 'T':
                                $('#gender').val('3').trigger('change');
                                break;
                        }
                        $('#SrCitizenAge').val(taxParam.srCitizenExemption); // Fixing typo
                        $('#specialPTaxAmt').val(taxParam.specialAmt);
                        $('#specialDeductionMonth').val(taxParam.monthSpecific);
                    } else {
                        hideTaxFields();
                    }
                },
                error: function (xhr, status, error) {
                    hideTaxFields();
                }
            });
        }
    });
});
$(document).ready(function () {
  
    $('#professionalTaxSlabForm').on('submit', function (e) {
        e.preventDefault(); // Stop native form submission
        var isValid = validateForm();
        if (isValid) {
            var formData = {
                Ptax_Slab_Id: parseInt($('#ptaxSlabId').val()) || 0,           
                State_ID: $('#state').val(),
                Min_Income: parseFloat($('#salaryFrom').val()) || 0,
                Max_Income: parseFloat($('#salaryTo').val()) || 0,
                Frequency: parseInt($('#frequency').val()) || 0,
                SpecialDeductionMonth: $('#specialDeductionMonth').val() ? parseInt($('#specialDeductionMonth').val()) : null,
                PTaxAmt: parseFloat($('#ptAmount').val()) || 0,
                SpecialPTaxAmt: parseFloat($('#specialPTaxAmt').val()) || 0,
                Gender: $('#gender').val() ? parseInt($('#gender').val()) : null,
                SrCitizenAge: $('#srCitizenAge').val() ? parseInt($('#srCitizenAge').val()) : null,
                Is_YearEnd_Adjustment: $('#isYearEndAdjustment').length ? $('#isYearEndAdjustment').prop('checked') : false,
                Effective_From: formatDateForApi($('#effectiveDate').val()),
                Effective_To: formatDateForApi($('#effectiveTo').val())
            };
            $.ajax({
                url: '/ProfessionalTaxSlab/InsertPtaxSlab',  // Adjust URL as needed
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(formData),
                success: function (response) {
                    if (response.success) {
                        $('#addProfessionalTaxSlab').modal('hide');
                        $.ajax({
                            url: '/ProfessionalTaxSlab/GetUpdatedPTaxSlabTableRows',
                            method: 'GET',
                            success: function (data) {                               
                                showAlert("success", response.message);
                                $('#ptaxTableBody').html(data);
                                var countMyRow = $('#ptaxTableBody tr').length;
                                $('#showMyCount span').text(countMyRow); 
                            }
                        }); 
                        setTimeout(() => {
                            window.location.href = "/ProfessionalTaxSlab/Index";
                        }, 1000);
                    } else {
                        $('#addProfessionalTaxSlab').modal('hide');
                        showAlert("danger", response.message);
                        //setTimeout(() => {
                        //    window.location.href = "/ProfessionalTaxSlab/Index";
                        //}, 1000);
                    }
                },
                error: function (error) {                   
                    alert('An error occurred while saving the data.');
                }
            });
        } else {
            // If validation failed, prevent submission and show a message
           // alert('Please fill in all required fields.');
        }
    });
    //////////////////////////////////////////////////DELETE :- Start/////////////////////////////////////////////
    var selectedPtaxSlabId = 0;
    $(document).on('click', '.pTaxSlabDelete', function () {
        selectedPtaxSlabId = $(this).data('ptaxslabid');
    });
    $('#confirmPTaxSlabDelete').on('click', function () {
        if (!selectedPtaxSlabId) return;

        const ptaxSlabDTO = {
            Ptax_Slab_Id: selectedPtaxSlabId
        };

        $.ajax({
            url: '/ProfessionalTaxSlab/DeletePTaxSlab',
            type: 'POST',
            data: JSON.stringify(ptaxSlabDTO),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                if (response.success) {
                    $('#deletePTaxSlab').modal('hide');
                    showAlert("success", response.message);

                    $.ajax({
                        url: '/ProfessionalTaxSlab/GetUpdatedPTaxSlabTableRows',
                        method: 'GET',
                        success: function (data) {
                            $('#ptaxTableBody').html(data); 
                            let tableId = "ptaxslab-list";
                            if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
                                $(`#${tableId}`).DataTable().destroy();
                            }
                            makeDataTable(tableId);
                        },
                        error: function () {
                            showAlert("danger", "Failed to refresh table rows.");
                        }
                    });                    
                    setTimeout(() => {
                         window.location.href = "/ProfessionalTaxSlab/Index";
                    }, 1000);
                } else {
                    showAlert("danger", response.message);
                }
            },
            error: function () {
                showAlert("danger", "An unexpected error occurred.");
            }
        });
    });
    /////////////////////////////////////////////////DELETE :- END////////////////////////////////////////////////
    /////////////////////////////////////////////////Reset :- Start ///////////////////////////////////////////
    $('#btnResetPTaxSlab').on('click', function () {
        const ptaxSlabId = $('#ptaxSlabId').val();
        console.log("ResetAttheTimeofEdit", originalPTaxSlabData);
        if (!ptaxSlabId || parseInt(ptaxSlabId) === 0) {
            // Delay to allow modal and DOM to be fully rendered
            setTimeout(function () {
                resetPTaxSlabForm();
            }, 50); // slight delay
        } else {
            if (originalPTaxSlabData) {
                console.log("DuringEditReset");
                loadSlabDataForEdit(originalPTaxSlabData);
            }
        }
    });
    /////////////////////////////////////////////////Reset :- End /////////////////////////////////////////////
});
function validateForm() {
    var isPTaxSlabValid = true;

    // Validate State
    var state = $("#state").val();
    if (!validateFormRequired(state, "#state-error", "State is required")) {
        isPTaxSlabValid = false;
    }

    // Validate Salary From
    var salaryFrom = $("#salaryFrom").val();
    if (!validateFormRequired(salaryFrom, "#salaryFrom-error", "Salary range from is required")) {
        isPTaxSlabValid = false;
    } else if (!validateFormMaxLength(salaryFrom, 10, "#salaryFrom-error", "Salary range from is too long")) {
        isPTaxSlabValid = false;
    }


    // Salary To (Max_Income)
    var salaryTo = $("#salaryTo").val();
    if (!validateFormRequired(salaryTo, "#salaryTo-error", "Salary range to is required")) {
        isPTaxSlabValid = false;
    } else if (!validateFormMaxLength(salaryTo, 10, "#salaryTo-error", "Salary range to is too long")) {
        isPTaxSlabValid = false;
    }
    // Validate range
    if (!validateNumericRange(salaryFrom, salaryTo, "#salaryTo-error", "Salary range to must be greater than or equal to salary range from")) {
        isPTaxSlabValid = false;
    }

    var frequency = $("#frequency").val();
    if (!validateFormRequired(state, "#frequency-error", "frequency is required")) {
        isPTaxSlabValid = false;
    }

    // Validate PT Amount
    var ptAmount = $("#ptAmount").val();
    if (!validateFormRequired(ptAmount, "#ptAmount-error", "PT Amount is required")) {
        isPTaxSlabValid = false;
    }

    // Validate Effective Date
    let effectiveDate = $("#effectiveDate").val();
    if (!validateFormRequired(effectiveDate, "#effectiveDate-error", "Effective date is required")) {
        isPTaxSlabValid = false;
    }

    // Validate Effective To
    var effectiveTo = $("#effectiveTo").val();
    if (!validateFormRequired(effectiveTo, "#effectiveTo-error", "Effective to date is required")) {
        isPTaxSlabValid = false;
    }
    // Validate date range
    if (!validateDateRange(effectiveDate, effectiveTo, "#effectiveTo-error", "Effective to date must be later than or equal to effective from date")) {
        isPTaxSlabValid = false;
    }
    // Age (optional but validated if present)
    var age = $("#srCitizenAge").val();
    if (age && !validateNonNegative(age, "#srCitizenAge-error", "Age cannot be negative")) {
        isPTaxSlabValid = false;
    }
    var specialMonth = $("#specialDeductionMonth").val();
    if (!validateMonthRange(specialMonth, "#number-error", "Month must be between 1 and 12")) {
        isPTaxSlabValid = false;
    }
    var specialPTaxAmt = $("#specialPTaxAmt").val();
    if (!validateNonNegativeIfNotNull(specialPTaxAmt, "#specialPTaxAmt-error", "Special PTAX Amount cannot be negative")) {
        isPTaxSlabValid = false;
    }

    return isPTaxSlabValid;
}

//function fetchAndBindDropdownForEdit(url, selector, placeholder, selectedValue, data = null) {
//    $.ajax({
//        url: url,
//        type: 'GET',
//        data: data ?? {},
//        success: function (response) {
//            if (response && response.length > 0) {
//                if ($.fn.select2 && $(selector).hasClass("select2-hidden-accessible")) {
//                    $(selector).select2('destroy');
//                }
//                $(selector).empty().append(`<option value="">${placeholder}</option>`);
//                response.forEach(function (item) {
//                    $(selector).append(new Option(item.text, item.value));
//                });
//                $(selector).select2({
//                    dropdownParent: $('#addProfessionalTaxSlab')
//                });
//                setTimeout(() => {
//                    $(selector).val(selectedValue?.toString()).trigger('change');
//                }, 200);
//            }
//        },
//        error: function (error) {
//            //console.log(`Error loading ${selector} for edit:`, error);
//        }
//    });
//}



function fetchAndBindDropdownForEdit(url, selector, placeholder, selectedValue, data = null, onComplete = null) {
    $.ajax({
        url: url,
        type: 'GET',
        data: data ?? {},
        success: function (response) {
            if (response && response.length > 0) {
                if ($.fn.select2 && $(selector).hasClass("select2-hidden-accessible")) {
                    $(selector).select2('destroy');
                }

                $(selector).empty().append(`<option value="">${placeholder}</option>`);
                response.forEach(function (item) {
                    $(selector).append(new Option(item.text, item.value));
                });

                $(selector).select2({
                    dropdownParent: $('#addProfessionalTaxSlab'),
                    width: '100%' // ensure styling
                });

                // Set selected value AFTER select2 initialized
                $(selector).val(selectedValue?.toString()).trigger('change');

                // ✅ Callback to notify it’s done
                if (typeof onComplete === 'function') {
                    onComplete();
                }
            }
        },
        error: function () {
            console.log(`Error loading ${selector}`);
        }
    });
}

//function loadSlabDataForEdit(data) {
//   // resetPTaxSlabForm();
//    $("#professionalTaxSlabTitle").text("Edit Professional Tax Slab");
//    $('#btnSavePTaxSlab').text('Update');
//    $("#ptaxSlabId").val(data.ptax_Slab_Id);
//    fetchAndBindDropdownForEdit('/DropDown/FetchCommonStateDropdown', '#state', 'Select State', data.state_ID, { Country_Id: 99 });
//    fetchAndBindDropdownForEdit('/DropDown/FetchPayTaxFrequencyDropdown', '#frequency', 'Select Frequency', data.frequency);
//    fetchAndBindDropdownForEdit('/DropDown/FetchGenderDropdown', '#gender', 'Select Gender', data.gender);
//    $("#salaryFrom").val(data.min_Income);
//    $("#salaryTo").val(data.max_Income);
//    $("#specialDeductionMonth").val(data.specialDeductionMonth);
//    $("#ptAmount").val(data.pTaxAmt);
//    $("#specialPTaxAmt").val(data.specialPTaxAmt);
//    $("#srCitizenAge").val(data.srCitizenAge);
//    $("#age").val(data.age); // this key not in object; remove or keep if you expect to calculate/populate it
//    $("#effectiveDate").val(formatDate(data.effective_From));
//    $("#effectiveTo").val(formatDate(data.effective_To));

//    // Toggle display based on is_YearEnd_Adjustment
//    if (data.is_YearEnd_Adjustment !== undefined) {
//        $("#isYearEndAdjustment").prop('checked', data.is_YearEnd_Adjustment);
//        $("#isYearEndAdjustmentWrapper").show();
//    }
//    if (data.isActive !== undefined) {
//        $("#pTaxSlabActiveToggle").prop('checked', data.isActive);
//        $("#statusToggleWrapper").show();
//        $("#pTaxSlabStatusLabel").text(data.isActive ? "Active" : "Inactive");
//    }
//    $(".input_error_msg").text("");
//}

function loadSlabDataForEdit(data) {
    $("#professionalTaxSlabTitle").text("Edit Professional Tax Slab");
    $('#btnSavePTaxSlab').text('Update');
    $("#ptaxSlabId").val(data.ptax_Slab_Id);

    fetchAndBindDropdownForEdit('/DropDown/FetchCommonStateDropdown', '#state', 'Select State', data.state_ID, { Country_Id: 99 });
    fetchAndBindDropdownForEdit('/DropDown/FetchPayTaxFrequencyDropdown', '#frequency', 'Select Frequency', data.frequency);
    fetchAndBindDropdownForEdit('/DropDown/FetchGenderDropdown', '#gender', 'Select Gender', data.gender);

    // Delay non-dropdown fields slightly to allow dropdowns to load first
    setTimeout(() => {
        $("#salaryFrom").val(data.min_Income);
        $("#salaryTo").val(data.max_Income);
        $("#specialDeductionMonth").val(data.specialDeductionMonth);
        $("#ptAmount").val(data.pTaxAmt);
        $("#specialPTaxAmt").val(data.specialPTaxAmt);
        $("#srCitizenAge").val(data.srCitizenAge);
        $("#age").val(data.age);
        $("#effectiveDate").val(formatDate(data.effective_From));
        $("#effectiveTo").val(formatDate(data.effective_To));

        if (data.is_YearEnd_Adjustment !== undefined) {
            $("#isYearEndAdjustment").prop('checked', data.is_YearEnd_Adjustment);
            $("#isYearEndAdjustmentWrapper").show();
        }
        if (data.isActive !== undefined) {
            $("#pTaxSlabActiveToggle").prop('checked', data.isActive);
            $("#statusToggleWrapper").show();
            $("#pTaxSlabStatusLabel").text(data.isActive ? "Active" : "Inactive");
        }

        $(".input_error_msg").text("");
    }, 300); // enough delay to ensure dropdowns settle
}

function formatDate(dateStr) {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`; // Always YYYY-MM-DD
}
$(document).on("click", ".redirectFromPTaxSlabList", function () {
    var id = $(this).data("ptaxslabid");
    $.ajax({
        url: '/ProfessionalTaxSlab/GetPTaxSlabDetail',
        type: 'GET',
        data: { id: id },
        success: function (response) {
            if (response.isSuccess) {
                originalPTaxSlabData = response.data;
                //console.log("AssignvalueForEditReset");
                //console.log(originalPTaxSlabData);
                loadSlabDataForEdit(response.data); 
                $('#addProfessionalTaxSlab').modal('show');
            } else {
                //toastr.error(response.message || "Failed to fetch tax slab data.");
            }
        },
        error: function () {
            //toastr.error("Error occurred while fetching data.");
        }
    });
});