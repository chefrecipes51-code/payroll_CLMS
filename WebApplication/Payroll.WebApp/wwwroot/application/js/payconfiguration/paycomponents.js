$(document).ready(function () {

    $('.select2_search_ctm').select2({
        placeholder: function () {
            return $(this).data('placeholder'); // Set dynamic placeholder from data-placeholder attribute
        },
        allowClear: true,  // Allows clearing the selection (if needed)
        multiple: false,   // Ensure it's a single select dropdown
        dropdownAutoWidth: true,  // Auto adjust dropdown width
        width: '100%'      // Ensures the dropdown takes up full width of its container
    });
    // Clear error when user starts typing or selecting valid input
    $('#payrollHeadDropdown').on('change', function () {
        if ($(this).val()) {
            $('#payrollHeadDropdown-error').text('');
        }
    });

    $('#payComponent').on('input', function () {
        if ($(this).val().trim()) {
            $('#payComponentDropdown-error').text('');
        }
    });
    // Disable button on page load
    $('#Parent').prop('disabled', true);
    $('#calculationTypeDropdown').on('change', function () {
        //////////////////////////Added For Formula:-start///////////////////////////
        var isEditFormulaBTN = $('#payComponentForm').data('edit-id') !== undefined;
        //console.log("isEditFormulaBTN", isEditFormulaBTN);
        var selectedText = $("#calculationTypeDropdown option:selected").text().trim();
        if (selectedText === "Calculate") {
            $('#formulaSection').show();
            if (!isEditFormulaBTN) {
                $('#formulaSectionbutton').show();
            } else {
                $('#formulaSectionbutton').hide();
            }
            loadDropdown('#formulaDropdown', '/DropDown/FetchFormulaTypeDropdown');
        } else {
            $('#formulaSection').hide();
            $('#formulaSectionbutton').hide();
        }
        if (selectedText === "Fixed") {
            $('#amountSection').show();
        } else {
            $('#amountSection').hide();
        }
        //////////////////////////Added For Formula:-End///////////////////////////
        if ($(this).val()) {
            $('#calculationTypeDropdown-error').text('');
        }
    });

    // Trigger change event on page load
    $('#isChlidDropdown').on('change', function () {
        var isChildValue = $("#isChlidDropdown option:selected").text().trim();
        console.log('Child dropdown changed to:', isChildValue);
        
        if (isChildValue === 'Yes') {
            $('#isParentDropdown').prop('disabled', false);
            $('#isParentDropdown').attr('required', true);
            $('#isParentDropdown-error').show(); // Show error by default if no selection
            
            // ✅ IMPROVED: Clear existing options and load fresh data
            $('#isParentDropdown').empty().append($('<option>', {
                value: '',
                text: '-- Select --'
            }));
            
            loadDropdown('#isParentDropdown', '/DropDown/FetchIsParentPaycomponentDropdown');
        } else {
            $('#isParentDropdown').prop('disabled', true);
            $('#isParentDropdown').empty().append($('<option>', {
                value: '',
                text: '-- Select --'
            }));
            $('#isParentDropdown').removeAttr('required');
            $('#isParentDropdown-error').hide();
        }
        if ($(this).val()) {
            $('#isChlidDropdown-error').text('');
        }
    });
    $('#isParentDropdown').on('change', function () {
        if ($(this).val()) {
            $('#isParentDropdown-error').text('');
        }
    });

    $('#minval').on('input', function () {
        const val = $(this).val().trim();
        const regex = /^\d+(\.\d{1,2})?$/;
        if (!val || regex.test(val)) {
            $('#minval-error').text('');
        }
    });

    $('#maxval').on('input', function () {
        const val = $(this).val().trim();
        const regex = /^\d+(\.\d{1,2})?$/;
        if (!val || regex.test(val)) {
            $('#maxval-error').text('');
        }
    });

    $('#amountval').on('input', function () {
        const val = $(this).val().trim();
        const regex = /^\d+(\.\d{1,2})?$/;
        if (val && regex.test(val)) {
            $('#amountval-error').text('');
        }
    });

    function validateForm() {
        var isValid = true;

        // Clear previous messages
        $('.input_error_msg').text('');

        // Required field validations
        const payrollHead = $('#payrollHeadDropdown').val();
        const payComponent = $('#payComponent').val().trim();
        const calculationType = $('#calculationTypeDropdown').val();
        const minVal = $('#minval').val().trim();
        const maxVal = $('#maxval').val().trim();
        const amountVal = $('#amountval').val().trim();
        const isChildValue = $("#isChlidDropdown").val();
        const isChild = $("#isChlidDropdown option:selected").text().trim();
        var parentValue = $('#isParentDropdown').val();
        const decimalRegex = /^\d+(\.\d{1,2})?$/;
        // Payroll Head
        if (!payrollHead) {
            $('#payrollHeadDropdown-error').text('Please select the Pay Head.');
            isValid = false;
        }

        // Pay Component validation
        if (!payComponent) {
            $('#payComponentDropdown-error').text('Please enter the Pay Component Name.');
            isValid = false;
        } else {
            const error = validateTextField(payComponent);
            if (error) {
                $('#payComponentDropdown-error').text(error);
                isValid = false;
            }
        }

        // Calculation Type
        if (!calculationType) {
            $('#calculationTypeDropdown-error').text('Please select the Calculate Type.');
            isValid = false;
        }

        //// Decimal format regex
        // Min value validation (always visible)
        let min = parseFloat(minVal);
        if (minVal) {
            if (!decimalRegex.test(minVal) || min < 0) {
                $('#minval-error').text('Minimum Amount must be a valid non-negative decimal number.');
                isValid = false;
            }
        }

        // Max value validation (always visible)
        let max = parseFloat(maxVal);
        if (maxVal) {
            if (!decimalRegex.test(maxVal) || max < 0) {
                $('#maxval-error').text('Maximum Amount must be a valid non-negative decimal number.');
                isValid = false;
            }
        }

        // Min should not be greater than Max
        if (!isNaN(min) && !isNaN(max) && min > max) {
            $('#minval-error').text('Minimum Amount cannot be greater than Maximum Amount.');
            isValid = false;
        }

        // Calculation Type
        if (!isChildValue) {
            $('#isChlidDropdown-error').text('Please select the Is Child.');
            isValid = false;
        }

        if (isChild === 'Yes' && (!parentValue || parentValue === "")) {
            $('#isParentDropdown-error').text("Please select a parent.").show();
            isValid = false;
        } else {
            $('#isParentDropdown-error').hide();
        }


        if ($('#amountSection').is(':visible')) {
            $('#amountval-error').text('');
            let amount = parseFloat(amountVal);

            if (!amountVal) {
                $('#amountval-error').text('Please enter the Amount.');
                isValid = false;
            } else if (!decimalRegex.test(amountVal) || amount < 0) {
                $('#amountval-error').text('Amount must be a valid non-negative decimal number.');
                isValid = false;
            }

            // min ≤ amount ≤ max
            if (!isNaN(min) && !isNaN(amount) && amount < min) {
                $('#amountval-error').text('Amount should be greater than or equal to Minimum Amount.');
                isValid = false;
            }

            if (!isNaN(max) && !isNaN(amount) && amount > max) {
                $('#amountval-error').text('Amount should be less than or equal to Maximum Amount.');
                isValid = false;
            }
        } else {
            $('#amountval-error').text('');
        }

        return isValid;
    }

    // Allow only valid decimal input while typing
    $('#minval, #maxval, #amountval').on('input', function () {
        var val = $(this).val();
        // Remove invalid characters and ensure only 1 decimal point and max 2 decimal places
        val = val.replace(/[^0-9.]/g, '')
            .replace(/(\..*)\./g, '$1') // prevent multiple dots
            .replace(/^(\d*\.\d{0,2}).*$/, '$1'); // max 2 decimal places
        $(this).val(val);
    });

    // Format on blur to enforce 2 decimal places (e.g. 10 -> 10.00)
    $('#minval, #maxval, #amountval').on('blur', function () {
        const val = parseFloat($(this).val());
        if (!isNaN(val)) {
            $(this).val(val.toFixed(2));
        }
    });

    // Function to populate dropdown from URL
    function loadDropdown(selector, url, onComplete) {
        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                const $dropdown = $(selector);
                $dropdown.empty().append($('<option>', {
                    value: '',
                    text: '-- Select --'
                }));

                $.each(data, function (i, item) {
                    $dropdown.append($('<option>', {
                        value: item.value,
                        text: item.text
                    }));
                });
                if (onComplete) onComplete();  // Call callback after loading
                $dropdown.trigger('change'); // for select2 or any events
            },
            error: function () {
                console.error('Failed to load dropdown data for:', selector);
            }
        });
    }

    // Call this function on page load
    loadDropdown('#payrollHeadDropdown', '/DropDown/FetchPayrollHeadsDropdown');
    loadDropdown('#calculationTypeDropdown', '/DropDown/FetchCalculationTypeDropdown');
    loadDropdown('#isChlidDropdown', '/DropDown/FetchIsChildPayComponentDropdown', function () {
        // Auto-select first valid option after options are loaded
        var $childDropdown = $('#isChlidDropdown');
        var firstValidValue = $childDropdown.find('option').filter(function () {
            return $(this).val() !== '' && !$(this).is(':disabled');
        }).first().val();

        console.log('Auto-selecting:', firstValidValue);

        if (firstValidValue) {
            $childDropdown.val(firstValidValue).trigger('change');
        }
    });


    $(document).on('click', '.paycomponent', function () {
        var id = $(this).data('id');

        $.ajax({
            url: `/PayConfiguration/GetPayComponentDetailsById/${id}`,
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    var data = response.data;
                    // Populate fields
                    $('#payComponent').val(data.earningDeductionName);
                    $('#calculationTypeDropdown').val(data.calculationType).trigger('change');

                    ///////////////////////////For Formula Check:- Start
                    if (data.calculationTypeName?.toLowerCase() === 'calculate' || data.calculationType === 3) {
                        $('#formulaSection').show();
                        $('#formulaSectionbutton').hide();
                        loadDropdownWithSelectedValue('#formulaDropdown', '/DropDown/FetchFormulaTypeDropdown', function () {
                            setSelectedValueInDropdown('#formulaDropdown', data.formula_Id);
                        });
                    }
                    //////////////////////////For Formula Check:- End
                    $('#payrollHeadDropdown').val(data.earningDeductionType).trigger('change');
                    $('#minval').val(data.minimumUnit_value);
                    $('#maxval').val(data.maximumUnit_value);
                    if (data.calculationTypeName?.toLowerCase() === 'fixed' || data.calculationType === 2) {
                        $('#amountSection').show();
                        $('#amountval').val(data.amount);
                    }
                    
                    // ✅ FIXED: Proper sequence for child/parent dropdown handling
                    loadDropdownWithSelectedValue('#isChlidDropdown', '/DropDown/FetchIsChildPayComponentDropdown', function () {
                        const isChildValue = data.is_Child ? "1" : "0";
                        setSelectedValueInDropdown('#isChlidDropdown', isChildValue);

                        // ✅ CRITICAL: Use setTimeout to ensure the change event completes first
                        setTimeout(function() {
                            // If is_Child is true, load parent dropdown AFTER the change event has processed
                            if (data.is_Child) {
                                // ✅ Wait for parent dropdown to be enabled and loaded
                                loadDropdownWithSelectedValue('#isParentDropdown', '/DropDown/FetchIsParentPaycomponentDropdown', function () {
                                    // ✅ Add additional delay to ensure dropdown is fully rendered
                                    setTimeout(function() {
                                        setSelectedValueInDropdown('#isParentDropdown', data.parent_EarningDeduction_Id);
                                        console.log('Parent dropdown value set:', data.parent_EarningDeduction_Id);
                                    }, 100);
                                });
                            }
                        }, 200); // Allow change event to complete
                    });
                    
                    $('#formula').val(data.formula || '');
                    // Status toggle
                    const isActive = data.isActive ?? false; // use the correct field name
                    $('#payComponentActiveToggle').prop('checked', isActive);
                    $('#activeStatusLabel').text(isActive ? 'Active' : 'Inactive');
                    $('#togglePayComponentContainer').show();
                    // Update the hidden field to know it's an edit operation
                    $('#payComponentForm').data('edit-id', data.earningDeduction_Id);

                    // Change the button text to "Update"
                    $('#paySaveButton').text('Update');
                    $('#payComponentFormTitle').text('Update Pay Components');
                    $('#payComponentTitile').show(); // <-- Show title on edit
                    $('#totalPayComponent').show(); // <-- Show title on edit
                    $('#cancelComponentFormbtn').show();
                    // Show the form if hidden
                    $('#pay-component-card').slideDown();
                } else {
                    showAlert('danger', response.message);
                }
            },
            error: function () {
                toastr.error("Error fetching data for edit.");
            }
        });
    });
    $('#payComponentActiveToggle').on('change', function () {
        $('#activeStatusLabel').text(this.checked ? 'Active' : 'Inactive');
    });

    // Bind to form submit
    $("#paySaveButton").on('click', function (e) {
        if (!validateForm()) {
            e.preventDefault();
            return;
        }

        var isEdit = $('#payComponentForm').data('edit-id') !== undefined;
        var earningDeductionId = isEdit ? $('#payComponentForm').data('edit-id') : 0;

        var earningDeductionName = $('#payComponent').val().trim().split(' ').map(function (word) {
            return word.charAt(0).toUpperCase() + word.slice(1).toLowerCase();
        }).join(' ');

        var dto = {
            earningDeductionName: earningDeductionName,
            calculationType: parseInt($('#calculationTypeDropdown').val()),
            earningDeductionType: parseInt($('#payrollHeadDropdown').val()),
            minimumUnit_value: parseFloat($('#minval').val()) || 0,
            maximumUnit_value: parseFloat($('#maxval').val()) || 0,
            amount: parseFloat($('#amountval').val()) || 0,
            is_Child: $('#isChlidDropdown').val() === "1",
            parent_EarningDeduction_Id: parseInt($('#isParentDropdown').val()) || 0,
            formula: $('#formula').val() || "",
            formula_Id: parseInt($('#formulaDropdown').val()) || 0,
            company_Id: 0,
            earningDeduction_Id: earningDeductionId,
            earningDeductionTypeName: "",
            calculationTypeName: "",
            parentEarningdeductionName: "",
            isActive: $('#payComponentActiveToggle').prop('checked')
        };
        ////// MODEL POP UP CODE :- Start
        var calculationTypeVal = parseInt($('#calculationTypeDropdown').val());
        if (calculationTypeVal === 3 && !$('#formulaDropdown').val()) {
            $('#formulapaycomponent').modal('show');

            $('#btnFormulaYes').off('click').on('click', function () {
                $('#formulapaycomponent').modal('hide');
                saveData(dto, isEdit);
            });

            $('#btnFormulaNo').off('click').on('click', function () {
                $('#formulapaycomponent').modal('hide');
                setTimeout(function () {
                    $('#payComponent').focus();
                }, 300);
            });
            return;
        }
        ////// MODEL POP UP CODE :- End
        //if (!$('#formulaDropdown').val()) {
        //    $('#formulapaycomponent').modal('show');

        //    // Handle "Yes" button click - Save anyway
        //    $('#btnFormulaYes').off('click').on('click', function () {
        //        $('#formulapaycomponent').modal('hide');
        //        saveData(dto, isEdit);
        //    });

        //    // Handle "No" button click - Refocus input
        //    $('#btnFormulaNo').off('click').on('click', function () {
        //        $('#formulapaycomponent').modal('hide');
        //        setTimeout(function () {
        //            $('#payComponent').focus();
        //        }, 300);
        //    });
        //    return;
        //}
        saveData(dto, isEdit);
    });
    function saveData(dto, isEdit) {
        var url = isEdit ? '/PayConfiguration/UpdatePayComponents' : '/PayConfiguration/AddPayComponents';

        $.ajax({
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (response) {
                if (response.success) {
                    $('#pay-component-card').slideToggle();
                    $('#payComponentForm')[0].reset();
                    $('.select2_search_ctm').val(null).trigger('change');
                    $('#payComponentForm').removeData('edit-id');
                    $('#paySaveButton').text('Add');
                    $('#togglePayComponentContainer').hide();
                    setTimeout(function () {
                        showAlert("success", response.message);
                    }, 1000);
                    LoadPayComponents();
                } else {
                    showAlert("danger", response.message);
                }
            },
            error: function () {
                toastr.error("Something went wrong while saving the data.");
            }
        });
    }


    // Reset form behavior
    $('#resetPayComponentFormBtn').on('click', function () {
        const isEditMode = $('#payComponentForm').data('edit-id') !== undefined;

        if (isEditMode) {
            // Re-populate original values from DB
            const id = $('#payComponentForm').data('edit-id');
            if (id) {
                // Call the same AJAX logic to refill
                $('.paycomponent[data-id="' + id + '"]').trigger('click');
            }
        } else {
            // Reset all fields for Add mode
            $('#payComponentForm')[0].reset();
            $(".select2_search_ctm").val(null).trigger("change");
            $(".input_error_msg").text("");
            $('#payComponentForm').removeData('edit-id');
            $('#payComponentActiveToggle').prop('checked', false);
            $('#activeStatusLabel').text('Inactive');
            $('#paySaveButton').text('Add');
        }
    });


});
$(document).ready(function () {
    LoadPayComponents();
});
function LoadPayComponents() {
    $.ajax({
        url: '/PayConfiguration/PayComponentList',
        //url: '@Url.Action("PayComponentList", "PayConfiguration")', // Change to your controller name
        type: 'GET',
        success: function (result) {
            $('#payComponentContainer').html(result.html);
            $('#payComponentCount').text(result.count);

            var tableId = "pay-component-list";
            if ($.fn.DataTable && $.fn.dataTable.isDataTable(`#${tableId}`)) {
                $(`#${tableId}`).DataTable().clear().destroy();
            }
            if (result.count > 0) {
                makeDataTable(tableId);
                // Get the initialized DataTable instance
                var table = $(`#${tableId}`).DataTable();

                // Disable buttons initially
                disableNonEditableButtons(tableId);

                // Bind disable function on draw event
                table.on('draw.dt', function () {
                    disableNonEditableButtons(tableId);
                });
            } else {
                // Initialize the variable before using it
                var $table = $(`#${tableId}`);
                // Manually add "no data" message if no rows
                $table.find('tbody').html(
                    '<tr><td colspan="11" class="text-center">No data available.</td></tr>'
                );
            }
        },
        error: function () {
            $('#payComponentContainer').html('<div class="text-danger text-center">Failed to load data.</div>');
        }
    });
}

// Function to disable buttons where data-iseditable="false"
function disableNonEditableButtons(tableId) {
    $(`#${tableId} .edit-btn, #${tableId} .delete-btn`).each(function () {
        const isEditable = $(this).attr('data-iseditable');
        if (isEditable === "false") {
            $(this).prop('disabled', true).addClass('disabled');
        }
    });
}

var selectedButton = null;
var isRequestInProgress = false;
$(document).on('click', '.btn-danger-light-icon[data-bs-target="#deletePayComponent"]', function () {
    selectedButton = $(this);
});
//$('#confirmAreaDevare').on('click', function () {
$(document).on('click', '#confirmPayComponentDelete', function () {
    if (isRequestInProgress) return; // Prevent multiple requests
    isRequestInProgress = true;

    if (!selectedButton) {
        console.error("Error: No button was selected.");
        isRequestInProgress = false;
        return;
    }

    var payComponentId = selectedButton.data('paycomponent-id');

    if (!payComponentId) {
        showAlert("danger", "Invalid area ID.");
        isRequestInProgress = false;
        return;
    }

    var rowId = `row-${payComponentId}`; // Construct the row ID

    var rowData = {
        EarningDeduction_Id: payComponentId,
        earningDeductionName: "",
        calculationType: 0, // Ensure proper integer or NaN
        earningDeductionType: 0, // Ensure proper integer or NaN
        minimumUnit_value: 0,
        maximumUnit_value: 0,
        amount: 0,
        formula: "", // Optional if you have this input
        formula_Id: 0, // Optional if you have this input
        company_Id: 0,//$('#CompanyId').val() || 0, // Set this appropriately
        EarningDeductionTypeName: "",
        CalculationTypeName: "",
        is_Child: false,
        parent_EarningDeduction_Id: 0,
        parentEarningdeductionName: "",

    };

    $.ajax({
        url: '/PayConfiguration/DeletePayComponent',
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
                LoadPayComponents();

            } else {
                showAlert("danger", response.message || "Failed to delete  area. Please try again.");
            }
            $('#deletePayComponent').modal('hide');
        },
        error: function () {
            showAlert("danger", "An error occurred. Please try again.");
            $('#deletePayComponent').modal('hide');
        },
        complete: function () {
            isRequestInProgress = false;
            // Ensure modal cleanup to prevent UI glitches
            setTimeout(() => {
                $('body').removeClass('modal-open');
                $('.modal-backdrop').remove();
            }, 300);
        }
    });
});
$(document).on('click', '#cancelComponentFormbtn', function () {
    const formContainer = $('#pay-component-card');
    if (formContainer.is(':visible')) {
        formContainer.slideUp();
        $('#payComponentForm')[0].reset(); // Reset the form
        $(".select2_search_ctm").val(null).trigger("change"); // Reset Select2 dropdowns
        $(".input_error_msg").text(""); // Clear error messages
        $('#payComponentForm').removeData('edit-id');

        // Reset status toggle and label
        $('#payComponentActiveToggle').prop('checked', false);
        $('#activeStatusLabel').text('Inactive');

        // Hide any additional toggle container
        $('#togglePayComponentContainer').hide();

        // Reset button text to "Add"
        $('#paySaveButton').text('Add');
        $('#payComponentTitile').hide();
        $('#totalPayComponent').show();
        // Hide the cancel button again
        $('#cancelComponentFormbtn').hide();
        // Reset the title to "Add" when closing
    }
});


function TogglePayComponentForm() {
    //$('#pay-component-card').slideToggle(); // This handles both slideDown and slideUp
    const formContainer = $('#pay-component-card');
    const formTitle = $('#payComponentFormTitle');
    const titleContainer = $('#payComponentTitile');
    if (formContainer.is(':visible')) {
        //formContainer.slideUp();
        //$('#payComponentForm')[0].reset(); // reset the form if hiding
        //$(".select2_search_ctm").val(null).trigger("change"); // Reset Select2 dropdowns
        //$(".input_error_msg").text(""); // Clear error messages
        //$('#payComponentForm').removeData('edit-id');

        //// Reset status toggle and hide it
        //$('#payComponentActiveToggle').prop('checked', false);
        //$('#activeStatusLabel').text('Inactive');
        //$('#togglePayComponentContainer').hide();
        //// Reset the button text
        //$('#paySaveButton').text('Add');
        //// Reset the title to "Add" when closing
        //formTitle.text('Add Pay Components');
        //// Hide the title container
        //titleContainer.hide();
        $('#totalPayComponent').show();
        //$('#cancelComponentFormbtn').hide();

    } else {
        // When opening the form for "Add", ensure toggle is hidden
        $('#togglePayComponentContainer').hide();
        formContainer.slideDown();
        // Set title based on whether it's Add or Edit mode
        const isEdit = $('#payComponentForm').data('edit-id') !== undefined;
        formTitle.text(isEdit ? 'Update Pay Components' : 'Add Pay Components');
        // Show the title container
        titleContainer.show();
        $('#totalPayComponent').show();
        $('#cancelComponentFormbtn').show();
    }
}
////////////////////////////////////Implementing Formula Code:-Start//////////////////////////////
function setSelectedValueInDropdown(selector, value) {
    const $dropdown = $(selector);

    // ✅ IMPROVED: Add retry mechanism for better reliability
    function attemptSelection(retries = 3) {
        // Check if the value exists in the dropdown
        if ($dropdown.find(`option[value='${value}']`).length > 0) {
            console.log(`✅ Setting value '${value}' in dropdown ${selector}`);
            $dropdown.val(value).trigger('change'); // Trigger 'change' to handle select2 events
            return true;
        } else if (retries > 0) {
            console.log(`⏳ Value '${value}' not found in ${selector}, retrying... (${retries} attempts left)`);
            setTimeout(() => attemptSelection(retries - 1), 50);
            return false;
        } else {
            console.warn(`❌ Value '${value}' not found in dropdown: ${selector} after multiple attempts`);
            return false;
        }
    }
    
    return attemptSelection();
}

function loadDropdownWithSelectedValue(selector, url, callback) {
    $.ajax({
        url: url,
        type: 'GET',
        success: function (data) {
            const $dropdown = $(selector);
            $dropdown.empty().append($('<option>', {
                value: '',
                text: '-- Select --'
            }));

            $.each(data, function (i, item) {
                $dropdown.append($('<option>', {
                    value: item.value,
                    text: item.text
                }));
            });
            $dropdown.trigger('change');
            if (callback) {
                callback();
            }
        },
        error: function () {
            console.error('Failed to load dropdown data for:', selector);
        }
    });
}


////////////////////////////////////Implementing Formula Code:-End//////////////////////////////