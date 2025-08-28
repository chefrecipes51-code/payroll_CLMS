$(document).ready(function () {
    var path = window.location.pathname.toLowerCase();
        $('.nav-link').removeClass('active');
  
    if (path.includes('/taxslab/assignincometaxslab')) {
        $('#assignTaxTab').addClass('active');
    } else if (path.includes('/taxslab/taxslab')) {
        $('#incomeTaxTab').addClass('active');
    }
    const tableId = "assign-tax-regime-list";

    if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
        $(`#${tableId}`).DataTable().clear().destroy();
    }
    makeDataTable(tableId);
    // Optionally replace with empty message (like in AJAX success)
    $(`#${tableId} tbody`).html(
        '<tr><td colspan="6" class="text-center">Select a filters to display the grid data.</td></tr>'
    );

    // Step 1: Load Partial View on Page Load (GET method)
    $.ajax({
        url: '/TaxSlab/EntityFilterResponseList',
        type: 'GET',
        success: function (html) {
            $('#entityFilterContainer').html(html);
            // Wait for modal to finish rendering and THEN apply select2
            $('#standardFilterModal').modal('show').on('shown.bs.modal', function () {
                // Reinitialize all select2s inside modal once visible
                $('#contractorDropdown, #financialYearDropdown, #taxregimeDropdown, #entityCodeDropdown, #entityNameDropdown').each(function () {
                    const $dropdown = $(this);
                    if ($dropdown.hasClass("select2-hidden-accessible")) {
                        $dropdown.select2('destroy');
                    }
                    $dropdown.select2({
                        dropdownParent: $('#standardFilterModal'),
                        placeholder: $dropdown.data('placeholder') || 'Select',
                        allowClear: true,
                        width: '100%'
                    });
                });
            });

            // Manually load scripts from the partial view
            $.getScript('/assets/src/js/select2.min.js', function () {
                $.getScript('/assets/src/custom-js/select2.js', function () {
                    // Reinitialize all select2s inside modal once visible
                    $('#contractorDropdown, #financialYearDropdown, #taxregimeDropdown, #entityCodeDropdown, #entityNameDropdown').each(function () {
                        const $dropdown = $(this);
                        if ($dropdown.hasClass("select2-hidden-accessible")) {
                            $dropdown.select2('destroy');
                        }
                        $dropdown.select2({
                            dropdownParent: $('#standardFilterModal'),
                            placeholder: $dropdown.data('placeholder') || 'Select',
                            allowClear: true,
                            width: '100%'
                        });
                    });
                });
            });

            $('#contractorDropdown, #taxregimeDropdown, #financialYearDropdown').on('change', function () {
                const fieldId = $(this).attr('id');
                $(`#${fieldId}-error`).text(''); // Clear the error message
            });
            // Step 2: Initialize dropdowns after partial is loaded
            var sessionYear = $('#hdnSessionFinancialYear').val();
            console.log(sessionYear);
            BindFinancialYear(sessionYear).done(function () {
                GetContractors().done(function () {
                    BindTaxRegimeDropdown();
                });
            });


        },
        error: function () {
            alert('Failed to load filters.');
        }
    });
    $('#standardFilterModal').modal('show').on('shown.bs.modal', function () {
        InitializeSelect2('#contractorDropdown', 'Select Contractor');
        InitializeSelect2('#taxregimeDropdown', 'Select Tax Regime');
        InitializeSelect2('#financialYearDropdown', 'Select Financial Year');
        InitializeSelect2('#entityCodeDropdown', 'Select Entity Code');
        InitializeSelect2('#entityNameDropdown', 'Select Entity Name');
    });

    // Step 3: Handle contractor dropdown selection
    $(document).on('change', '#contractorDropdown', function () {
        const contractorId = $(this).val();
        GetEntities(contractorId);
    });

    $(document).on('change', '#entityNameDropdown', function () {
        $('#entityCodeDropdown').val($(this).val()).trigger('change.select2');
    });

    $(document).on('change', '#entityCodeDropdown', function () {
        $('#entityNameDropdown').val($(this).val()).trigger('change.select2');
    });

    // Disable assign button on page load
    $(document).ready(function () {
        $('#assigntaxregime').prop('disabled', true);
    });

    // When any checkbox inside table body is changed
    $(document).on('change', '#assign-tax-regime-list tbody input[type="checkbox"]', function () {
        // Check if any checkbox is checked
        const anyChecked = $('#assign-tax-regime-list tbody input[type="checkbox"]:checked').length > 0;

        // Enable or disable the assign button based on checkbox state
        $('#assigntaxregime').prop('disabled', !anyChecked);
    });


    var selectedOption = null;

    $(document).ready(function () {
        // Disable button on page load
        $('#assigntaxregime').prop('disabled', true);
    });

    // Apply Selection Logic
    $(document).on('click', '#applySelection', function () {
        const table = $('#assign-tax-regime-list').DataTable();
        if ($('#selectPage').is(':checked')) {
            selectedOption = 'page';
            table.rows({ page: 'current' }).every(function () {
                $(this.node()).find('.row-checkbox').prop('checked', true);
            });
        } else if ($('#selectAll').is(':checked')) {
            selectedOption = 'all';
            table.rows().every(function () {
                $(this.node()).find('.row-checkbox').prop('checked', true);
            });
        }

        toggleAssignButton();
        // ✅ Hide the dropdown modal
        //$('#dropdownModal').hide(); 
        $('#dropdownModal').fadeOut(); // Smooth transition
    });

    // Reset Selection
    $(document).on('click', '#resetSelection', function () {
        const table = $('#assign-tax-regime-list').DataTable();
        selectedOption = null;

        table.rows().every(function () {
            $(this.node()).find('.row-checkbox').prop('checked', false);
        });

        $('input[name="recordSelect"]').prop('checked', false);
        toggleAssignButton();
        // ✅ Hide the dropdown modal
        //$('#dropdownModal').hide(); 
        $('#dropdownModal').fadeOut(); // Smooth transition
    });

    // Reapply selection mode on table redraw (pagination, search, etc.)
    $('#assign-tax-regime-list').on('draw.dt', function () {
        const table = $('#assign-tax-regime-list').DataTable();

        if (selectedOption === 'page') {
            table.rows({ page: 'current' }).every(function () {
                $(this.node()).find('.row-checkbox').prop('checked', true);
            });
        } else if (selectedOption === 'all') {
            table.rows().every(function () {
                $(this.node()).find('.row-checkbox').prop('checked', true);
            });
        }
        toggleAssignButton();
    });

    // Toggle assign button when any checkbox is clicked manually
    $(document).on('change', '.row-checkbox', function () {
        toggleAssignButton();
        const anyChecked = $('.row-checkbox:checked').length > 0;
        $('#deselectAllBtn').prop('disabled', !anyChecked);
    });

    // Function to enable/disable the Assign button
    function toggleAssignButton() {
        const anyChecked = $('.row-checkbox:checked').length > 0;
        $('#assigntaxregime').prop('disabled', !anyChecked);
        $('#deselectAllBtn').prop('disabled', !anyChecked);
    }

    // Deselect all checkboxes on button click
    $(document).on('click', '#deselectAllBtn', function () {
        deselectedRowandButton();
    });
    function deselectedRowandButton() {
        const table = $('#assign-tax-regime-list').DataTable();

        // Uncheck all checkboxes inside the table rows
        table.rows().every(function () {
            $(this.node()).find('.row-checkbox').prop('checked', false);
        });

        // Clear selectedOption so it doesn't reapply on redraw
        selectedOption = null;

        // Disable Assign and Deselect All buttons
        $('#assigntaxregime').prop('disabled', true);
        $('#deselectAllBtn').prop('disabled', true);
    }
    function validateFilterForm() {
        let isValid = true;
        $(".input_error_msg").text("");

        const contractor = $('#contractorDropdown').val();
        const taxregime = $('#taxregimeDropdown').val();
        const financial = $('#financialYearDropdown').val();

        if (!contractor) {
            $('#contractorDropdown-error').text('Please select contractor.');
            isValid = false;
        }
        if (!taxregime) {
            $('#taxregimeDropdown-error').text('Please select tax regime.');
            isValid = false;
        }
        if (!financial) {
            $('#financialYearDropdown-error').text('Please select financial year.');
            isValid = false;
        }
        return isValid;
    }

    $(document).on('click', '#resetFilterBtn', function () {
  
        var sessionYearId = $('#hdnSessionFinancialYearValue').val();

        var sessionCompany = $('#hdnSessionFinancialYear').val();
        console.log($('#hdnSessionFinancialYear').val()); // Should print 2025-2026
        console.log($('#hdnSessionFinancialYearValue').val()); // Should print 2025-2026
        // Reset dropdown values (with Select2 or similar)
        $('#contractorDropdown').val('').trigger('change');
        $('#taxregimeDropdown').val('').trigger('change');
        //$('#financialYearDropdown').val(sessionYearId).trigger('change');
        $('#entityCodeDropdown').val('').trigger('change');
        $('#entityNameDropdown').val('').trigger('change');
        // Re-bind Financial Year dropdown and select sessionYearId after binding
        BindFinancialYear(sessionCompany).done(function () {
            $('#financialYearDropdown').val(sessionYearId).trigger('change');
        });
        // Clear error messages
        $(".input_error_msg").text('');
        // Clear the DataTable
        const tableId = "assign-tax-regime-list";

        if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
            $(`#${tableId}`).DataTable().clear().destroy();
        }
        makeDataTable(tableId);
        // Optionally replace with empty message (like in AJAX success)
        $(`#${tableId} tbody`).html(
            '<tr><td colspan="6" class="text-center">Select a filters to display the grid data.</td></tr>'
        );
    });
    $(document).on("click", '#applyFilterBtn', function () {
        if (!validateFilterForm()) return;
        const filterData = {
            SelectType: 'CO', // 👈 Required filter type
            CompanyId: 0,
            CorrespondanceId: 0,
            ContractorId: $('#contractorDropdown').val(),
            EntityId: $('#entityCodeDropdown').val() || 0, // ID remains value
            EntityCode: $('#entityCodeDropdown').find('option:selected').val() ? $('#entityCodeDropdown').find('option:selected').text() : "",
            EntityName: $('#entityNameDropdown').find('option:selected').val() ? $('#entityNameDropdown').find('option:selected').text() : "",
            SelectFilter: "",
            Regime_Id: $('#taxregimeDropdown').val(),
            FinYear_ID: $('#financialYearDropdown').val(),
            trade_Id: 0,
            skill_ID: 0,
            grade_ID: 0
        };
        $.ajax({
            url: '/TaxSlab/EntityFilterResponseList',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(filterData),
            success: function (response) {
                const tableId = "assign-tax-regime-list";
                const hasData = response?.contractorEntities?.length > 0;
                // Destroy existing DataTable before binding new data
                if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
                    $(`#${tableId}`).DataTable().clear().destroy();
                }
                // Bind new table data
                if (hasData) {
                    bindTable(response.contractorEntities);
                    // Re-initialize DataTable
                    makeDataTable(tableId);
                } else {
                    $('#assign-tax-regime-list tbody').html(
                        '<tr><td colspan="9" class="text-center">No records found</td></tr>'
                    );
                }
                var modalEl = document.getElementById('standardFilterModal');
                var modal = bootstrap.Modal.getInstance(modalEl); // or new bootstrap.Modal(modalEl)
                modal.hide();
             
            },
            error: function () {
                alert('Error loading contractor entity data.');
            }
        });
    });
    $(document).on("click", '#assigntaxregime', function () {
        // Call to bind dropdown on page load
        BindAssignTaxRegimeDropdown();

        // Click handler for dropdown item selection
        $(document).on('click', '#dropdownMenu .dropdown-item', function () {
            const selectedText = $(this).text();
            const selectedValue = $(this).data('value');

            $('#dropdownTextbox').text(selectedText);
            $('#dropdownTextbox').attr('data-selected-value', selectedValue);
            // Remove the error message if exists
            $('#dropdownTextbox-error').text('');
        });
    });
    $(document).on("click", "#bulkInsertAssignTaxBtn", function () {
        const selectedRows = [];
        const selectedRegimeId = $('#dropdownTextbox').attr('data-selected-value');
        const selectedFinYearId = $('#financialYearDropdown').attr('data-selected-value');

        if (!selectedRegimeId) {
            $('#dropdownTextbox-error').text('Please select a Tax Regime from the dropdown.');
            return false;
        }

        return true;

        $('#assign-tax-regime-list tbody tr').each(function () {
            const $row = $(this);
            const isChecked = $row.find('input[type="checkbox"]').is(':checked');
            if (isChecked) {
                const entityID = $row.find('td').eq(2).text().trim(); // Not required, just showing reference
                const contractorID = $row.find('td').eq(3).text().trim(); // Not required, just showing reference
                const contractorMastCode = $row.find('td').eq(4).text().trim();
                const entityName = $row.find('td').eq(5).text().trim();
                const entityCode = $row.find('td').eq(6).text().trim();

                selectedRows.push({
                    Contractor_Id: parseInt(contractorID), // You must supply correct data here
                    Contractor_Code: contractorMastCode?.trim().charAt(0) || 'X', // Default or read dynamically
                    Entity_ID: parseInt(entityID), // Replace with actual entity ID if available
                    Entity_Code: entityCode,
                    Regime_Id: parseInt(selectedRegimeId),
                    FinYear_ID: 9, // You can fetch it from elsewhere if needed
                    IsActive: true
                });
            }
        });

        if (selectedRows.length === 0) {
            alert("Please select at least one row.");
            return;
        }
        const requestData = {
            EntityTaxRegime: selectedRows // ✅ Must match DTO property name
        };
        $.ajax({
            url: '/TaxSlab/BulkAssignTaxRegime', // update path as needed
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(requestData),
            success: function (response) {
                if (response.success) {
                    $('#assignRegime').modal('hide');
                    setTimeout(function () {
                        showAlert("success", response.message);
                    }, 1000);
                    deselectedRowandButton();
                    // Optionally refresh data/grid
                } else {
                    showAlert("danger", response.message);
                }
            },
            error: function (xhr, status, error) {
                alert("An error occurred while processing: " + error);
            }
        });
    });

    // Step 4 and 5: Removed invalid entityCodeDropdown and entityNameDropdown click events
    // They were using <li><a> which is not valid in <select> dropdowns
});

function BindAssignTaxRegimeDropdown() {
    $.ajax({
        url: '/DropDown/FetchTaxRegimeDropdown',
        type: 'GET',
        success: function (data) {
            const $dropdownMenu = $('#dropdownMenu');
            $dropdownMenu.empty(); // Clear any existing options

            // Populate from API
            if (Array.isArray(data)) {
                data.forEach(function (item) {
                    $dropdownMenu.append(`
                            <li>
                                <a class="dropdown-item" data-value="${item.value}">${item.text}</a>
                            </li>
                        `);
                });
            }

            // Append static option
            $dropdownMenu.append(`
                    <li>
                        <a class="dropdown-item" data-value="99">Same As Previous Year</a>
                    </li>
                `);
        },
        error: function () {
            console.error('Failed to load Tax Regime dropdown');
        }
    });
}

// Usage: To get the selected value later (e.g., in form submission)
function GetSelectedRegimeId() {
    return $('#dropdownTextbox').attr('data-selected-value') || null;
}
function BindFinancialYear(sessionYear) {
    return $.ajax({
        url: `/DropDown/FetchFinancialYearDropdown?companyId=${sessionYear}`,
        type: 'GET',
        success: function (data) {
            console.log(data);
            if (data && data.length > 0) {
                const $dropdown = $('#financialYearDropdown').empty();
                $dropdown.append('<option value="">Select Tax Regime</option>');

                data.forEach(c => {
                    const isDisabled = c.disabled ? ' disabled' : '';
                    $dropdown.append(`<option value="${c.value}"${isDisabled}>${c.text}</option>`);
                });
                InitializeSelect2('#financialYearDropdown', 'Select Financial Year');

                // Auto-select the first non-placeholder and non-disabled option
                const $options = $dropdown.find('option:not(:disabled):not([value=""])');
                if ($options.length > 0) {
                    $options.first().prop('selected', true);
                }

                $('#financialYearDropdown').trigger('change');
            }
        },
        error: function () {
            console.error("Error fetching tax regimes");
        }
    });

}

function GetContractors() {
    const requestData = {
        SelectType: "C",
        CompanyId: 0,
        CorrespondanceId: 0,
        ContractorId: 0,
        EntityId: 0,
        EntityCode: "",
        EntityName: "",
        SelectFilter: "",
    };

    return $.ajax({
        url: '/TaxSlab/EntityFilterResponseList',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        success: function (data) {
            const $dropdown = $('#contractorDropdown');

            if (data.contractors && data.contractors.length > 0) {
                $dropdown.empty().append('<option value="">Select Contractor</option>');

                data.contractors.forEach(c => {
                    $dropdown.append(
                        $('<option>', {
                            value: c.contractor_ID,
                            text: c.contractor_Name
                        })
                    );
                });

                InitializeSelect2('#contractorDropdown', 'Select Contractor');


                // ✅ Optionally auto-select the first contractor
                //const firstValidValue = data.contractors[0]?.contractor_ID || '';
                //$dropdown.val(firstValidValue).trigger('change.select2');

                //console.log("Contractor selected:", firstValidValue);
            } else {
                $dropdown.empty().append('<option value="">No contractors found</option>');
                $dropdown.select2();
            }
        },
        error: function () {
            console.error('Error fetching contractors');
        }
    });
}

function GetEntities(contractorId) {
    const requestData = {
        SelectType: "ED",
        CompanyId: 0,
        CorrespondanceId: 0,
        ContractorId: contractorId,
        EntityId: 0,
        EntityCode: "",
        EntityName: "",
        SelectFilter: "",
    };

    return $.ajax({
        url: '/TaxSlab/EntityFilterResponseList',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        success: function (data) {
            console.log(data);
            const entityCodes = data?.entityCodes ?? [];
            
            if (!Array.isArray(data.entityCodes) || data.entityCodes.length === 0) {
                $('#entityCodeDropdown').html('<option>No entities found</option>');
                $('#entityNameDropdown').html('<option>No entities found</option>');
            }
            if (Array.isArray(entityCodes) && entityCodes.length > 0) {

                // Bind entity code dropdown
                BindEntityDropdown('#entityCodeDropdown', data.entityCodes, 'entity_ID', 'entity_Code');

                // Bind entity name dropdown
                BindEntityDropdown('#entityNameDropdown', data.entityCodes, 'entity_ID', 'entity_Name');
            }
            else {
                $('#entityCodeDropdown').empty().append('<option value="">No entities found</option>');
                $('#entityNameDropdown').empty().append('<option value="">No entities found</option>');
            }
        },
        error: function () {
            console.error('Error fetching entity data');
        }
    });
}

function BindEntityDropdown(dropdownId, data, valueField, textField) {
    const $dropdown = $(dropdownId).empty();
    $dropdown.append('<option value="">Select</option>');
    if (data && data.length > 0) {
        data.forEach(item => {
            $dropdown.append(`<option value="${item[valueField]}">${item[textField]}</option>`);
        });
        InitializeSelect2(dropdownId, 'Select');

        // Reinitialize Select2 after populating the dropdown
        $(dropdownId).trigger('change');
    }
}

function BindTaxRegimeDropdown() {
    return $.ajax({
        url: '/DropDown/FetchTaxRegimeDropdown',
        type: 'GET',
        success: function (data) {
            if (data && data.length > 0) {
                const $dropdown = $('#taxregimeDropdown').empty();
                $dropdown.append('<option value="">Select Tax Regime</option>');

                data.forEach(c => {
                    const isSelected = c.selected ? ' selected' : '';
                    const isDisabled = c.disabled ? ' disabled' : '';
                    $dropdown.append(`<option value="${c.value}"${isSelected}${isDisabled}>${c.text}</option>`);
                });
                InitializeSelect2('#taxregimeDropdown', 'Select Tax Regime');

                $('#taxregimeDropdown').trigger('change');

            }
        },
        error: function () {
            console.error("Error fetching tax regimes");
        }
    });
}
function InitializeSelect2(dropdownSelector, placeholder = 'Select') {
    const $dropdown = $(dropdownSelector);

    // Destroy if already initialized
    if ($dropdown.hasClass("select2-hidden-accessible")) {
        $dropdown.select2('destroy');
    }

    $dropdown.select2({
        placeholder: placeholder,
        allowClear: true,
        width: '100%',
        dropdownParent: $('#standardFilterModal') // VERY IMPORTANT if inside a modal
    });
}

function bindTable(data) {
    let tbody = '';
    data.forEach((item, index) => {
        tbody += `
            <tr>
                <td><input type="checkbox" class="form-check-input row-checkbox"></td>
                 <td hidden>${item.regime_Id ?? ''}</td>
                <td hidden>${item.entity_ID ?? ''}</td>
                <td hidden>${item.contractor_ID ?? ''}</td>
                <td hidden>${item.contractor_Mast_Code ?? ''}</td>
                <td>${item.entity_Name ?? ''}</td>
                <td>${item.entity_Code ?? ''}</td>
                <td>${item.regimename ?? ''}</td>
                <td>${item.dateof_Deployment ? formatDate(item.dateof_Deployment) : ''}</td>
            </tr>
        `;
    });
    $('#assign-tax-regime-list tbody').html(tbody);
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString(); // Customize if needed
}

