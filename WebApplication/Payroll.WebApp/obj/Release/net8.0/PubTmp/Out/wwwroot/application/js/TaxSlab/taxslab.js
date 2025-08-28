$(document).ready(function () {
    var originalTaxSlabConfigData = null; // Keep original edit data
    var isEditMode = false;

    var path = window.location.pathname.toLowerCase();

    $('.nav-link').removeClass('active');

    if (path.includes('/taxslab/assignincometaxslab')) {
        $('#assignTaxTab').addClass('active');
    } else if (path.includes('/taxslab/taxslab')) {
        $('#incomeTaxTab').addClass('active');
    }
    const sessionCompanyId = $("#sessionCompanyId").val();
    const sessionRoleId = $("#sessionRoleId").val();
    // Check if RoleId is 1 (Admin)
    if (sessionRoleId == 1) {
        // Admin or multi-company access
        $('#allCompanyDropdown').prop('disabled', false); // Enable dropdown
        fetchAllCompanies(sessionCompanyId);
    } else {
        // Non-admin - disable dropdown and show only sessionCompanyId
        $('#allCompanyDropdown').prop('disabled', true); // Disable dropdown

        fetchAllCompanies(sessionCompanyId);
        console.log(sessionCompanyId);
        // Directly load grid with session company id
        LoadTaxSlab(sessionCompanyId);
    }
    $(document).on('change', '#allCompanyDropdown', function () {
        const selectedCompanyId = $(this).val();
        if (selectedCompanyId) {
            LoadTaxSlab(selectedCompanyId);
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

    // Clear errors as user types or selects
    $('#slabname, #incomefrom, #incometo, #taxpayable').on('input', function () {
        const fieldId = $(this).attr('id');
        $(`#${fieldId}-error`).text('');
    });

    $('#regimeDropdown').on('change', function () {
        $('#regimeDropdown-error').text('');
    });

    $('#incomefrom, #incometo').on('input', function () {
        let val = $(this).val();
        // Allow only valid decimal input
        val = val.replace(/[^0-9.]/g, '')
            .replace(/(\..*)\./g, '$1')
            .replace(/^(\d*\.\d{0,2}).*$/, '$1');
        $(this).val(val);

        const fieldId = $(this).attr('id');
        const value = val.trim();
        if (!value || /^\d+(\.\d{1,2})?$/.test(value)) {
            $(`#${fieldId}-error`).text('');
        }
    });

    // Format salary fields on blur
    $('#incomefrom, #incometo').on('blur', function () {
        const val = parseFloat($(this).val());
        if (!isNaN(val)) {
            $(this).val(val.toFixed(2));
        }
    });
    function validateTaxSlabForm() {
        let isValid = true;
        $(".input_error_msg").text("");

        const slabname = $("#slabname").val().trim();
        const regime = $("#regimeDropdown").val();
        const incomefrom = $("#incomefrom").val().trim();
        const incometo = $('#incometo').val().trim();
        const taxpayable = $('#taxpayable').val().trim();
        const decimalRegex = /^\d+(\.\d{1,2})?$/;

        if (!slabname) {
            $("#slabname-error").text("Please enter slab name.");
            isValid = false; 
        } else {
            const error = validateTextField(slabname);
            if (error) {
                $('#slabname-error').text(error);
                isValid = false;
            }
        }

        if (!regime) {
            $('#regimeDropdown-error').text('Please select the tax regime.');
            isValid = false;
        }

        if (!incomefrom) {
            $("#incomefrom-error").text("Please enter income from.");
            isValid = false;
        } else if (!decimalRegex.test(incomefrom) || parseFloat(incomefrom) < 0) {
            $("#incomefrom-error").text("Income From must be a valid non-negative decimal number.");
            isValid = false;
        }

        if (!incometo) {
            $("#incometo-error").text("Please enter income to.");
            isValid = false;
        } else if (!decimalRegex.test(incometo) || parseFloat(incometo) < 0) {
            $("#incometo-error").text("Income To must be a valid non-negative decimal number.");
            isValid = false;
        }

        const from = parseFloat(incomefrom);
        const to = parseFloat(incometo);
        if (!isNaN(from) && !isNaN(to) && to < from) {
            $("#incometo-error").text("Income To must be greater than or equal to Income From.");
            isValid = false;
        }

        if (!taxpayable) {
            $("#taxpayable-error").text("Please enter payable tax in %.");
            isValid = false;
        }

        return isValid;
    }

    // Save handler
    $(document).on('click', '#btnSavetaxSlab', function () {

        if (!validateTaxSlabForm()) return;

        const editId = $('#taxSlabForm').attr('data-edit-id');
        const isEdit = editId !== undefined && editId !== '';
        const dataDto = {
            YearlyItTableDetail_Id: isEdit ? parseInt(editId) : 0,
            YearlyItTable_Id: parseInt($("#regimeDropdown").val()),
            Company_Id: $('#companyId').val() || 0,
            SlabName: $("#slabname").val().trim(),
            Income_From: parseFloat($('#incomefrom').val()) || 0,
            Income_To: parseFloat($('#incometo').val()) || 0,
            TaxPaybleInPercentage: parseInt($('#taxpayable').val()),
            TaxPaybleInAmount: 0,
            isActive: $('#TaxSlabActiveToggle').prop('checked'),
        }
        const url = isEdit ? '/TaxSlab/UpdateTaxSlab' : '/TaxSlab/AddTaxSlab';
        $.ajax({
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dataDto),
            success: function (response) {
                if (response.success) {
                    $('#taxslabadd').modal('hide');
                    setTimeout(function () {
                        showAlert("success", response.message);
                    }, 1000);
                    LoadTaxSlab();
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
    $(document).on('click', '#btnResettaxSlab', function () {
        if (isEditMode && originalTaxSlabConfigData) {
            // If editing -> Reset to original data
            setTaxSlabForm(originalTaxSlabConfigData);
        } else {
            // If adding -> Clear all fields
            clearTaxSlabForm();
        }
    });
    $(document).on('click', '#editTaxSlab', function () {
        let id = $(this).data('id');
        $.ajax({
            url: `/TaxSlab/GetTaxSlabDetailsById/${id}`,
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    let data = response.data;
                    console.log(data);
                    // Save Original Data
                    originalTaxSlabConfigData = data;
                    isEditMode = true;
                    setTaxSlabForm(data);

                    // Update the hidden field to know it's an edit operation
                    // Set Edit ID properly
                    $('#taxSlabForm').attr('data-edit-id', data.yearlyItTableDetail_Id);

                    // Change button text
                    $('#btnSavetaxSlab').text('Update');
                    $('#formTitle').text('Update Income Tax Slab'); // change title

                    // **OPEN MODAL**
                    $('#taxslabadd').modal('show');
                } else {
                    showAlert('danger', response.message);
                }
            },
            error: function () {
                toastr.error("Error fetching data for edit.");
            }
        });
    });
    $('#TaxSlabActiveToggle').on('change', function () {
        $('#activeStatusLabel').text(this.checked ? 'Active' : 'Inactive');
    });
    // Set Form with Data
    function setTaxSlabForm(data) {
        $(".input_error_msg").text('');
        console.log(data);
        $('#companyId').val(data.company_Id);
        $('#slabname').val(data.slabName).prop('readonly', true).addClass('disabled-input');
        $('#incomefrom').val(data.income_From);
        $('#incometo').val(data.income_To);
        $('#taxpayable').val(data.taxPaybleInPercentage);

        $('#TaxSlabActiveToggle').prop('checked', data.isActive ?? false);
        $('#activeStatusLabel').text(data.isActive ? 'Active' : 'Inactive');

        if (isEditMode) {
            $('#toggleTaxSlabContainer').show();
        } else {
            $('#toggleTaxSlabContainer').hide();
        }

        // Start cascading population
        $('#regimeDropdown').val(data.yearlyItTable_Id).trigger('change');
        loadTaxRegimeDropdown(data.yearlyItTable_Id);
    }
    // Clear Form
    function clearTaxSlabForm() {
        $('#slabname').val('').prop('readonly', false).removeClass('disabled-input');
        $('#incomefrom').val('');
        $('#incometo').val('');
        $('#taxpayable').val('');
        $('#regimeDropdown').val('');
        $('#TaxSlabActiveToggle').prop('checked', false);
        $('#activeStatusLabel').text('Inactive');
        $(".input_error_msg").text('');
    }

    $(document).on('click', '#btnAddTaxSlab', function () {
        isEditMode = false;
        originalTaxSlabConfigData = null;
        clearTaxSlabForm();
      
        populateCascadingDropdown('#regimeDropdown', [], 'value', 'text');
        loadTaxRegimeDropdown();
        $('#taxSlabForm').removeAttr('data-edit-id');
        $('#btnSavetaxSlab').text('Save');
        $('#formTitle').text('Add Income Tax Slab');
        $('#toggleTaxSlabContainer').hide();  // Hide the status toggle in add mode
        $('#taxslabadd').modal('show');
    });
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
            dropdownParent: $('#taxslabadd'),
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

    function loadTaxRegimeDropdown(selectedValue = '') {
        $.ajax({
            url: '/DropDown/FetchTaxRegimeDropdown',
            type: 'GET',
            success: function (data) {
                populateCascadingDropdown('#regimeDropdown', data, 'value', 'text', selectedValue);
            },
            error: function () {
                console.error("Error fetching companies");
            }
        });
    }
});

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
function LoadTaxSlab(companyId) {
    $.ajax({
        url: '/TaxSlab/TaxSlabList',
        type: 'GET',
        data: { id: companyId }, 
        success: function (result) {
            console.log(result.count);
            $('#taxSlabContainer').html(result.html);
            $('#taxSlabCount').text(result.count);
            let tableId = "tax-slab-list";
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
            $('#taxSlabContainer').html('<div class="text-danger text-center">Failed to load data.</div>');
        }
    });
}

var selectedButton = null;
var isRequestInProgress = false;

$(document).on('click', '.btn-danger-light-icon[data-bs-target="#deleteTaxSlab"]', function () {
    selectedButton = $(this);
});
$(document).on('click', '#confirmTaxSlabDelete', function () {
    if (isRequestInProgress) return; // Prevent multiple requests
    isRequestInProgress = true;

    if (!selectedButton) {
        console.error("Error: No button was selected.");
        isRequestInProgress = false;
        return;
    }

    var yearlyItTableDetailId = selectedButton.data('yearlyittabledetail-id');
    console.log(yearlyItTableDetailId);
    if (!yearlyItTableDetailId) {
        showAlert("danger", "Invalid ID.");
        isRequestInProgress = false;
        return;
    }

    var rowId = `row-${yearlyItTableDetailId}`; // Construct the row ID
    var rowData = {
        YearlyItTableDetail_Id: yearlyItTableDetailId,
        YearlyItTable_Id: 0,
        Company_Id: 0,
        SlabName: "",
        Income_From: 0,
        Income_To: 0,
        TaxPaybleInPercentage: 0,
        TaxPaybleInAmount: 0,
    };
    console.log(rowData);
    $.ajax({
        url: '/TaxSlab/DeleteTaxSlab',
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
                LoadTaxSlab();
            } else {
                showAlert("danger", response.message || "Failed to delete tax. Please try again.");
            }
            $('#deleteTaxSlab').modal('hide');
        },
        error: function () {
            showAlert("danger", "An error occurred. Please try again.");
            $('#deleteTaxSlab').modal('hide');
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