$(document).ready(function () {
    var originalPayGradeConfigData = null; // Keep original edit data
    var isEditMode = false;
    const sessionCompanyId = $("#sessionCompanyId").val();
    const sessionRoleId = $("#sessionRoleId").val();
    console.log("Session CompanyId:", sessionCompanyId, "Session RoleId:", sessionRoleId);

    if (sessionRoleId == 1) {
        // Admin or multi-company access
        $('#allCompanyDropdown').prop('disabled', false); // Enable dropdown
        fetchAllCompanies(sessionCompanyId);
    } else {
        // Limited access user
        $('#allCompanyDropdown').prop('disabled', true); // Disable dropdown

        fetchAllCompanies(sessionCompanyId);
        console.log(sessionCompanyId);
        // Directly load grid with session company id
        LoadPayGradeConfig(sessionCompanyId);
    }
    $(document).on('change', '#allCompanyDropdown', function () {
        const selectedCompanyId = $(this).val();
        if (selectedCompanyId) {
            LoadPayGradeConfig(selectedCompanyId);
        } else {
            $('#payGradeConfigContainer').empty();
            $('#payGradeCount').text('0');
        }
    });

    // Initialize Select2
    $('.select2_search_ctm').select2({
        placeholder: function () {
            return $(this).data('placeholder');
        },
        allowClear: true,
        width: '100%'
    });

    // Live validation for input
    $('.form-control').on('input change', function () {
        const fieldId = $(this).attr('id');
        $(`#${fieldId}-error`).text('');
    });
    function validatePayGradeForm() {
        var isValid = true;
        $(".input_error_msg").text("");

        const paygradeconfigname = $("#paygradeconfigname").val().trim() || "";
        const company = $("#DistinctCompanyDropdown").val();
        const location = $("#DistictLocationDropdown").val();
        const payGrade = $('#payGradeType').val();
        const trade = $('#DistinctTradeType').val();
        const skill = $('#skillTypeDropdown').val();
        const effectiveFromDate = $("#effectiveFromDate").val();
        const effectiveToDate = $("#effectiveToDate").val();

        if (!paygradeconfigname) {
            //$("#paygradeconfigname-error").text("Please enter pay grade configuration name.");
            isValid = true;
        } else {
            const error = validateTextField(paygradeconfigname);
            if (error) {
                $('#paygradeconfigname-error').text(error);
                isValid = false;
            }
        }

        if (!company) {
            $('#DistinctCompanyDropdown-error').text('Please select a company.');
            isValid = false;
        }

        if (!location) {
            $('#DistictLocationDropdown-error').text('Please select a location.');
            isValid = false;
        }

        if (!payGrade) {
            $('#payGradeType-error').text('Please select the pay grade type.');
            isValid = false;
        }

        if (!trade) {
            $('#DistinctTradeType-error').text('Please select the trade type.');
            isValid = false;
        }

        if (!skill) {
            $('#skillTypeDropdown-error').text('Please select the skill type.');
            isValid = false;
        }

        if (!effectiveFromDate) {
            $('#effectiveFromDate-error').text('Please select Effective From date.');
            isValid = false;
        }

        if (!effectiveToDate) {
            $('#effectiveToDate-error').text('Please select Effective To date.');
            isValid = false;
        }

        // Date order check
        const parseDate = (dateStr) => {
            const parts = dateStr.split('/');
            return parts.length === 3 ? new Date(`${parts[2]}-${parts[1]}-${parts[0]}`) : null;
        };

        const from = parseDate(effectiveFromDate);
        const to = parseDate(effectiveToDate);

        if (from && to && from > to) {
            $('#effectiveFromDate-error').text("Effective From date cannot be later than Effective To date.");
            isValid = false;
        }
        return isValid;
    }

    // Save handler
    $(document).on('click', '#btnSavePayGradeConfig', function () {

        if (!validatePayGradeForm()) return;

        const editId = $('#payGradeMappingForm').attr('data-edit-id');
        const isEdit = editId !== undefined && editId !== '';

        const effectiveFromDateStr = $("#effectiveFromDate").val();
        const effectiveToDateStr = $("#effectiveToDate").val();

        if (!effectiveFromDateStr || !effectiveToDateStr) {
            showAlert("warning", "Both Effective From and Effective To dates must be provided.");
            return;
        }

        // Parse date format from dd/MM/yyyy to yyyy-MM-dd
        const parseDate = (dateStr) => {
            const parts = dateStr.split('/');
            if (parts.length === 3) {
                return `${parts[2]}-${parts[1]}-${parts[0]}`; // yyyy-MM-dd
            }
            return null;
        };

        const formattedEffectiveFromDate = parseDate(effectiveFromDateStr);
        const formattedEffectiveToDate = parseDate(effectiveToDateStr);

        if (!formattedEffectiveFromDate || !formattedEffectiveToDate) {
            showAlert("warning", "Invalid date format. Please use dd/MM/yyyy.");
            return;
        }

        if (new Date(formattedEffectiveFromDate) > new Date(formattedEffectiveToDate)) {
            $('#effectiveFromDate-error').text("Effective From date cannot be later than Effective To date.");
            showAlert("warning", "Effective From date cannot be later than Effective To date.");
            return;
        }

        const dataDto = {
            PayGradeConfig_Id: isEdit ? parseInt(editId) : 0,
            Cmp_Id: parseInt($("#DistinctCompanyDropdown").val()),
            Correspondance_ID: parseInt($("#DistictLocationDropdown").val()),
            GradeConfigName: $("#paygradeconfigname").val().trim(),
            PayGrade_Id: parseInt($('#payGradeType').val()),
            Trade_Id: parseInt($('#DistinctTradeType').val()),
            SkillType_Id: parseInt($('#skillTypeDropdown').val()),
            EffectiveFrom: formattedEffectiveFromDate,
            EffectiveTo: formattedEffectiveToDate,
            isActive: $('#payGradeConfigActiveToggle').prop('checked'),
            PayGradeName: "",
            Skillcategory_Name: "",
            Trade_Name: "",
            EffectiveFromStr: "",
            EffectiveToStr: "",
        }


        const url = isEdit ? '/PayConfiguration/UpdatePayGradeMapping' : '/PayConfiguration/AddPayGradeMapping';

        $.ajax({
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataDto),
            success: function (response) {
                if (response.success) {
                    $('#paygradeconfigadd').modal('hide');
                    setTimeout(function () {
                        showAlert("success", response.message);
                    }, 1000);
                    LoadPayGradeConfig();
                } else {
                    showAlert("danger", response.message);
                }
            },
            error: function () {
                toastr.error("Something went wrong while saving the data.");
            }
        });
    });

    // Reset form
    $(document).on('click', '#btnResetPayGradeConfig', function () {
        if (isEditMode && originalPayGradeConfigData) {
            // If editing -> Reset to original data
            setPayGradeForm(originalPayGradeConfigData);
        } else {
            // If adding -> Clear all fields
            clearPayGradeForm();
        }
    });
    $(document).on('click', '#editpayGradeConfig', function () {
        var id = $(this).data('id');
        $.ajax({
            url: `/PayConfiguration/GetPayGradeMappingDetailsById/${id}`,
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    var data = response.data;
                    // Save Original Data
                    originalPayGradeConfigData = data;
                    isEditMode = true;
                    setPayGradeForm(data);

                    // Update the hidden field to know it's an edit operation
                    // Set Edit ID properly
                    $('#payGradeMappingForm').attr('data-edit-id', data.payGradeConfig_Id);

                    // Change button text
                    $('#btnSavePayGradeConfig').text('Update');
                    $('#formTitle').text('Update Pay Grade Mapping'); // change title

                    // **OPEN MODAL**
                    $('#paygradeconfigadd').modal('show');
                } else {
                    showAlert('danger', response.message);
                }
            },
            error: function () {
                toastr.error("Error fetching data for edit.");
            }
        });
    });
    $('#payGradeConfigActiveToggle').on('change', function () {
        $('#activeStatusLabel').text(this.checked ? 'Active' : 'Inactive');
    });
    // Set Form with Data
    function setPayGradeForm(data) {
        $(".input_error_msg").text('');

        $('#paygradeconfigname').val(data.gradeConfigName);

        // Format and set dates
        const formatDate = (dateStr) => {
            const date = new Date(dateStr);
            return date.toLocaleDateString('en-GB');
        };
        $('#effectiveFromDate').val(formatDate(data.effectiveFrom));
        $('#effectiveToDate').val(formatDate(data.effectiveTo));

        $('#payGradeConfigActiveToggle').prop('checked', data.isActive ?? false);
        $('#activeStatusLabel').text(data.isActive ? 'Active' : 'Inactive');

        if (isEditMode) {
            $('#togglePayGradeConfigContainer').show();
        } else {
            $('#togglePayGradeConfigContainer').hide();
        }

        // Start cascading population
        $('#DistinctCompanyDropdown').val(data.cmp_Id).trigger('change');
        fetchCompanies(data.cmp_Id); // Set company first

        fetchLocations(data.cmp_Id, function () {
            $('#DistictLocationDropdown').val(data.correspondance_ID).trigger('change');

            fetchTrade(data.correspondance_ID, function () {
                $('#DistinctTradeType').val(data.trade_Id).trigger('change');

                fetchSkillCategory(data.correspondance_ID, function () {
                    $('#skillTypeDropdown').val(data.skillType_Id).trigger('change');

                    // Only after skillTypeDropdown is populated, fetch and set pay grade
                    fetchActiveGrade(data.skillType_Id, function () {
                        console.log("data.payGrade_Id", data.payGrade_Id);
                        $('#payGradeType').val(data.payGrade_Id).trigger('change');
                    });
                });
            });
        });
    }
    // Clear Form
    function clearPayGradeForm() {
        $('#paygradeconfigname').val('');
        $('#payGradeTypeDropdown').val('');
        $('#DistinctTradeType').val('');
        $('#skillTypeDropdown').val('');
        $('#effectiveFromDate').val('');
        $('#effectiveToDate').val('');
        $('#payGradeConfigActiveToggle').prop('checked', false);
        $('#activeStatusLabel').text('Inactive');
        $(".input_error_msg").text('');
    }
    function populateCascadingDropdown(selector, data, valueField, textField, selectedValue = "") {
        const dropdown = $(selector);
        // Destroy select2 first to avoid conflict
        if (dropdown.hasClass('select2-hidden-accessible')) {
            dropdown.select2('destroy');
        }

        dropdown.empty().append('<option value="">Select</option>');

        if (Array.isArray(data) && data.length > 0) {
            $.each(data, function (index, item) {
                dropdown.append(
                    $('<option></option>')
                        .attr("value", item[valueField])
                        .text(item[textField])
                        .prop("selected", item[valueField] == selectedValue)
                );
            });
        }
        // Re-initialize select2
        dropdown.select2({
            dropdownParent: $('#paygradeconfigadd'),
            placeholder: dropdown.data('placeholder') || 'Select',
            allowClear: true,
            width: '100%'
        });
        if (selectedValue) {
            dropdown.val(selectedValue).trigger('change.select2');
        }
        dropdown.val(selectedValue);
        dropdown.trigger('change.select2');
    }

    // =============== Event Handlers ===============

    // When company changes
    $(document).on('change', '#DistinctCompanyDropdown', function () {
        const companyId = $(this).val();
        resetDropdownsAfterCompany();

        if (companyId) {
            fetchLocations(companyId);
        }
    });

    // When location changes
    $(document).on('change', '#DistictLocationDropdown', function () {
        const locationId = $(this).val();
        resetDropdownsAfterLocation();

        if (locationId) {
            fetchTrade(locationId);
            //fetchSkillCategory(locationId);
        }
    });
    $(document).on('change', '#DistinctTradeType', function () {
        const tradeId = $(this).val();
        const locationId = $('#DistictLocationDropdown').val();

        if (locationId && tradeId) {
            fetchSkillCategory(locationId, tradeId); // ✅ Skill fetched based on Location + Trade
        }
    });
    $(document).on('change', '#skillTypeDropdown', function () {
        resetDropdownsAfterSkill(); // Reset pay grade dropdown
        fetchActiveGrade(); // Fetch Active Pay Grade dropdown
    });


    // =============== Reset Helpers ===============
    function resetDropdownsAfterCompany() {
        populateCascadingDropdown('#DistictLocationDropdown', [], 'value', 'text');
        populateCascadingDropdown('#DistinctTradeType', [], 'value', 'text');
        populateCascadingDropdown('#skillTypeDropdown', [], 'value', 'text');
        populateCascadingDropdown('#payGradeType', [], 'value', 'text');

    }

    function resetDropdownsAfterLocation() {
        populateCascadingDropdown('#DistinctTradeType', [], 'value', 'text');
        populateCascadingDropdown('#skillTypeDropdown', [], 'value', 'text');
        populateCascadingDropdown('#payGradeType', [], 'value', 'text');
    }
    // =============== Reset Helpers ===============

    function resetDropdownsAfterSkill() {
        populateCascadingDropdown('#payGradeType', [], 'value', 'text');
    }

    // =============== Fetch Methods ===============

    function fetchAllCompanies(selectedValue = '') {
        $.ajax({
            url: '/DropDown/FetchCompaniesDropdown',
            type: 'GET',
            success: function (data) {
                if (data && data.length > 0) {
                    const $dropdown = $('#allCompanyDropdown');
                    $dropdown.empty().append('<option value="">Select Company</option>');

                    data.forEach(item => {
                        $dropdown.append(`<option value="${item.value}">${item.text}</option>`);
                    });

                    // Initialize Select2
                    if ($.fn.select2) {
                        $dropdown.select2({
                            placeholder: $dropdown.data('placeholder'),
                            width: '100%',
                            allowClear: true,
                            dropdownAutoWidth: true
                        });
                    } else {
                        console.warn("Select2 plugin not loaded.");
                    }

                    // ✅ Set selected value AFTER options and Select2 are ready
                    if (selectedValue && $dropdown.find(`option[value="${selectedValue}"]`).length > 0) {
                        $dropdown.val(selectedValue).trigger('change'); // or 'change.select2'
                    }
                }
            },
            error: function () {
                console.error("Error fetching companies");
            }
        });
    }

    function fetchCompanies(selectedValue = '') {
        $.ajax({
            url: '/DropDown/FetchCompaniesDropdown',
            type: 'GET',
            success: function (data) {
                populateCascadingDropdown('#DistinctCompanyDropdown', data, 'value', 'text', selectedValue);
            },
            error: function () {
                console.error("Error fetching companies");
            }
        });
    }
    function fetchLocations(companyId, callback) {
        $.ajax({
            url: `/DropDown/FetchDistinctLocationDropdown?companyId=${companyId}`,
            type: 'GET',
            success: function (data) {
                populateCascadingDropdown('#DistictLocationDropdown', data, 'value', 'text');
                if (callback) callback();
            },
            error: function () {
                console.error("Error fetching locations");
            }
        });
    }
    function fetchTrade(locationId, callback) {
        $.ajax({
            url: `/DropDown/FetchTradeTypeDropdown?companyLocationID=${locationId}&isActive=true`,
            type: 'GET',
            success: function (data) {
                populateCascadingDropdown('#DistinctTradeType', data, 'value', 'text');
                if (callback) callback();
            },
            error: function () {
                console.error("Error fetching trade types");
            }
        });
    }
    function fetchSkillCategory(locationId, callback) {
        $.ajax({
            url: `/DropDown/FetchSkillCategoryDropdown?correspondance_ID=${locationId}&isActive=true`,
            type: 'GET',
            success: function (data) {
                populateCascadingDropdown('#skillTypeDropdown', data, 'value', 'text');
                if (callback) callback();
            },
            error: function () {
                console.error("Error fetching skill categories");
            }
        });
    }
    function fetchActiveGrade(skillTypeId, callback) {
        $.ajax({
            url: `/DropDown/FetchActivePayGradeTypeDropdown`,
            type: 'GET',
            success: function (data) {
                console.log(data);
                populateCascadingDropdown('#payGradeType', data, 'value', 'text');
                if (typeof callback === 'function') callback();
            },
            error: function () {
                console.error("Error fetching pay Grade Type");
            }
        });
    }

    $(document).on('click', '#btnAddPayGradeConfig', function () {
        isEditMode = false;
        originalPayGradeData = null;
        clearPayGradeForm();
        $('#payGradeMappingForm').removeAttr('data-edit-id');
        $('#btnSavePayGradeConfig').text('Save');
        $('#formTitle').text('Add Pay Grade Mapping');
        $('#togglePayGradeConfigContainer').hide();  // Hide the status toggle in add mode
        $('#paygradeconfigadd').modal('show');
        // Clear all dropdowns
        resetDropdownsAfterCompany();
        populateCascadingDropdown('#DistinctCompanyDropdown', [], 'value', 'text');

        fetchCompanies();
    });
    // Initialize datepicker for 'Effective From Date'
    $('#effectiveFromDate').datepicker({
        format: 'dd/mm/yyyy',     // Display date format
        startDate: '0d',          // Disable past dates in calendar
        autoclose: true,          // Close calendar automatically after selection
        todayHighlight: true,     // Highlight today's date
        beforeShowDay: function (date) {
            if (date < new Date().setHours(0, 0, 0, 0)) {
                return { enabled: false, classes: 'disabled-date' }; // Disable past dates
            }
            return { enabled: true };
        }
    }).datepicker('setDate', new Date())   // Set current date by default
        .on('changeDate', function (e) {
            // Clear the date if a past date is somehow selected
            if (new Date(e.date) < new Date().setHours(0, 0, 0, 0)) {
                $('#effectiveFromDate').val('');
                showAlert("warning", "Past dates are not allowed.");
            }
        });

    // Prevent manual typing of past dates
    $('#effectiveFromDate').on('keypress keydown paste', function (e) {
        e.preventDefault();  // Block manual typing
    });

    // Initialize datepicker for 'Effective To Date'
    $('#effectiveToDate').datepicker({
        format: 'dd/mm/yyyy',     // Display date format
        startDate: '0d',          // Disable past dates in calendar
        autoclose: true,          // Close calendar automatically after selection
        todayHighlight: true,     // Highlight today's date
        beforeShowDay: function (date) {
            if (date < new Date().setHours(0, 0, 0, 0)) {
                return { enabled: false, classes: 'disabled-date' }; // Disable past dates
            }
            return { enabled: true };
        }
    }).datepicker('setDate', new Date())   // Set current date by default
        .on('changeDate', function (e) {
            // Clear the date if a past date is somehow selected
            if (new Date(e.date) < new Date().setHours(0, 0, 0, 0)) {
                $('#effectiveToDate').val('');
                showAlert("warning", "Past dates are not allowed.");
            }
        });

    // Prevent manual typing of past dates
    $('#effectiveToDate').on('keypress keydown paste', function (e) {
        e.preventDefault();  // Block manual typing
    });
});

$(document).ready(function () {
    // Handle dropdown change
   
});
function LoadPayGradeConfig(companyId) {
    $.ajax({
        url: '/PayConfiguration/PayGradeMappingList',
        type: 'GET',
        data: { id: companyId }, // pass companyId here
        success: function (result) {
            $('#payGradeConfigContainer').html(result.html);
            $('#payGradeCount').text(result.count);
            var tableId = "pay-grade-config-list";
            if ($.fn.DataTable && $.fn.dataTable.isDataTable(`#${tableId}`)) {
                $(`#${tableId}`).DataTable().destroy();
            }
            if (result.count > 0) {
                makeDataTable(tableId);
            } else {
                // Initialize the variable before using it
                var $table = $(`#${tableId}`);
                // Manually add "no data" message if no rows
                $table.find('tbody').html(
                    '<tr><td colspan="10" class="text-center">No data available.</td></tr>'
                );
            }
        },
        error: function () {
            $('#payGradeConfigContainer').html('<div class="text-danger text-center">Failed to load data.</div>');
        }
    });
}

var selectedButton = null;
var isRequestInProgress = false;

$(document).on('click', '.btn-danger-light-icon[data-bs-target="#devarePayGradeConfig"]', function () {
    selectedButton = $(this);
});
//$('#confirmAreaDevare').on('click', function () {
$(document).on('click', '#confirmPayGradeConfigDevare', function () {
    if (isRequestInProgress) return; // Prevent multiple requests
    isRequestInProgress = true;

    if (!selectedButton) {
        console.error("Error: No button was selected.");
        isRequestInProgress = false;
        return;
    }

    var payGradeConfigId = selectedButton.data('paygradeconfig-id');

    if (!payGradeConfigId) {
        showAlert("danger", "Invalid ID.");
        isRequestInProgress = false;
        return;
    }

    var rowId = `row-${payGradeConfigId}`; // Construct the row ID
    var rowData = {
        PayGradeConfig_Id: payGradeConfigId,
        Cmp_Id: 0,
        Correspondance_ID: 0,
        GradeConfigName: "", // Ensure proper integer or NaN
        PayGrade_Id: 0, // Ensure proper integer or NaN
        Trade_Id: 0,
        SkillType_Id: 0,
        EffectiveFrom: Date, // Set to null
        EffectiveTo: Date,    // Set to null
        PayGradeName: "",
        Skillcategory_Name: "",
        Trade_Name: "",
        EffectiveFromStr: "",
        EffectiveToStr: "",
    };
    console.log(rowData);
    $.ajax({
        url: '/PayConfiguration/DevarePayGradeMapping',
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(rowData),
        success: function (response) {
            if (response && response.success) {
                $(`#${rowId}`).fadeOut(500, function () {
                    $(this).remove();
                });
                showAlert("success", response.message);
                LoadPayGradeConfig();
                //setTimeout(function () {
                //    // Redirect to the list page
                //    window.location.href = "/PayConfiguration/PayGradeMapping";
                //}, 1000);
            } else {
                showAlert("danger", response.message || "Failed to devare area. Please try again.");
            }
            $('#devarePayGradeConfig').modal('hide');
        },
        error: function () {
            showAlert("danger", "An error occurred. Please try again.");
            $('#devarePayGradeConfig').modal('hide');
        },
        compvare: function () {
            isRequestInProgress = false;
            // Ensure modal cleanup to prevent UI glitches
            setTimeout(() => {
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }, 300);
        }
    });
});