$(document).ready(function () {
    const path = window.location.pathname;

    $('.nav-link').removeClass('active');

    if (path.includes('/EntityMaster/GradeEntityMapping')) {
        $('#gradeEntityMappingTab').addClass('active');
    } else if (path.includes('/EntityMaster/EntityList')) {
        $('#entityMasterTab').addClass('active');
    }
    const sessionCompanyId = $("#sessionCompanyId").val();
    const sessionRoleId = $("#sessionRoleId").val();
    const tableId = "entity-master-list";

    if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
        $(`#${tableId}`).DataTable().clear().destroy();
    }
    makeDataTableNew(tableId);
    // Optionally replace with empty message (like in AJAX success)
    $(`#${tableId} tbody`).html(
        '<tr><td colspan="6" class="text-center">Select a filters to display the grid data.</td></tr>'
    );


    // Load partial view and setup Select2
    $.ajax({
        url: '/EntityMaster/MapEntityGradeResponse',
        type: 'GET',
        success: function (html) {
            $('#mapEntityGradeContainer').html(html);
            $('#standardFilterModal').modal('show');
            // ✅ Move this inside here
            $('#gradeDropdown, #salaryStructureDropdown').hide(); // hide dropdowns
            $('#gradeText, #salaryStructureText').show(); // show text
            // Load and initialize Select2
            $.getScript('/assets/src/js/select2.min.js', function () {
                $.getScript('/assets/src/custom-js/select2.js', function () {
                    //initializeSelect2('.select2_search_ctm');
                });
            });

            if (sessionRoleId == 1) {
                // Admin or multi-company access
                $('#mapCompanyDropdown').prop('disabled', false); // Enable dropdown
                fetchCompanies(sessionCompanyId, function () {
                    resetAllFilters(); // ⬅️ Reset after dropdowns populated
                });
            } else {
                // Limited access user
                $('#mapCompanyDropdown').prop('disabled', true); // Disable dropdown
                fetchCompanies(sessionCompanyId, function () {
                    // Set the company dropdown value manually and trigger change
                    $('#mapCompanyDropdown').val(sessionCompanyId).trigger('change');

                    // Reset other filters
                    $('#mapContractorDropdown').val('').trigger('change');
                    $('#mapLocationDropdown').val('').trigger('change');
                    $('#mapTradeDropdown').val('').trigger('change');
                    $('#mapSkillDropdown').val('').trigger('change');
                    $('#mapGradeDropdown').val('').trigger('change');
                    $(".input_error_msg").text('');

                    // Manually fetch contractors since dropdown is disabled (user can’t trigger it)
                    fetchContractors(sessionCompanyId);
                });
            }
            // ✅ Independent
            fetchActiveGrade();   // ✅ Load grade once at start if independent
        },
        error: function () {
            alert('Failed to load filters.');
        }
    });

    // Reusable Select2 initializer
    function initializeSelect2(selector) {
        $(selector).select2({
            placeholder: function () {
                return $(this).data('placeholder') || 'Select';
            },
            allowClear: true,
            width: '100%',
            dropdownAutoWidth: true
        });
    }

    // Populate dropdown with Select2 re-init
    function populateDropdown(selector, data, valueField, textField, selectedValue = "") {
        const dropdown = $(selector);
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

        dropdown.select2('destroy').select2({
            placeholder: dropdown.data('placeholder') || 'Select',
            allowClear: true,
            width: '100%',
            dropdownAutoWidth: true
        });

        if (selectedValue) {
            dropdown.val(selectedValue); // Don't trigger change here
            //dropdown.val(selectedValue).trigger('change.select2');
        }
    }

    // Cascading change events
    $(document).on('change', '#mapCompanyDropdown', function () {
        const companyId = $(this).val();
        resetAfterCompany();
        if (companyId) {
            fetchContractors(companyId);
        }
    });

    $(document).on('change', '#mapContractorDropdown', function () {
        const companyId = $('#mapCompanyDropdown').val();
        resetAfterCompany(); // If needed for this level
        if (companyId) {
            fetchLocations(companyId);
        }
    });

    $(document).on('change', '#mapLocationDropdown', function () {
        const locationId = $(this).val();
        resetAfterLocation();
        if (locationId) {
            fetchTrade(locationId);
        }
    });

    $(document).on('change', '#mapTradeDropdown', function () {
        const tradeId = $(this).val();
        const locationId = $('#mapLocationDropdown').val();
        if (locationId && tradeId) {
            fetchSkillCategory(locationId, tradeId);
        }
    });

    $(document).on('change', '#mapSkillDropdown', function () {
        resetAfterSkill();
        // ✅ Removed fetchActiveGrade() from here
    });

    // Reset methods
    function resetAfterCompany() {
        populateDropdown('#mapLocationDropdown', [], 'value', 'text');
        populateDropdown('#mapLocationDropdown', [], 'value', 'text');
        populateDropdown('#mapTradeDropdown', [], 'value', 'text');
        populateDropdown('#mapSkillDropdown', [], 'value', 'text');
        // Don't reset grade if independent
    }

    function resetAfterLocation() {
        populateDropdown('#mapTradeDropdown', [], 'value', 'text');
        populateDropdown('#mapSkillDropdown', [], 'value', 'text');
        // Don't reset grade if independent
    }

    function resetAfterSkill() {
        // No reset for grade if independent
    }

    // AJAX fetch methods
    function fetchCompanies(selectedValue = '', callback = null) {
        $.ajax({
            url: '/DropDown/FetchCompaniesDropdown',
            type: 'GET',
            success: function (data) {
                populateDropdown('#mapCompanyDropdown', data, 'value', 'text', selectedValue);
                // Defer callback until Select2 has completed setup
                setTimeout(function () {
                    if (callback && typeof callback === "function") {
                        callback();
                    }
                }, 200); // Give select2 enough time to render
            },
            error: function () {
                console.error("Error fetching companies");
            }
        });
    }

    function fetchContractors(companyId) {
        $.ajax({
            url: `/DropDown/FetchContractorDropdown?contractor_ID=0&company_ID=${companyId}`,
            type: 'GET',
            success: function (data) {
                populateDropdown('#mapContractorDropdown', data, 'value', 'text');
            },
            error: function () {
                console.error("Error fetching contractors");
            }
        });
    }

    function fetchLocations(companyId) {
        $.ajax({
            url: `/DropDown/FetchDistinctLocationDropdown?companyId=${companyId}`,
            type: 'GET',
            success: function (data) {
                populateDropdown('#mapLocationDropdown', data, 'value', 'text');
            },
            error: function () {
                console.error("Error fetching locations");
            }
        });
    }

    function fetchTrade(locationId) {
        $.ajax({
            url: `/DropDown/FetchTradeTypeDropdown?companyLocationID=${locationId}&isActive=true`,
            type: 'GET',
            success: function (data) {
                populateDropdown('#mapTradeDropdown', data, 'value', 'text');
            },
            error: function () {
                console.error("Error fetching trade types");
            }
        });
    }

    function fetchSkillCategory(locationId, tradeId) {
        $.ajax({
            url: `/DropDown/FetchSkillCategoryDropdown?correspondance_ID=${locationId}&trade_ID=${tradeId}&isActive=true`,
            type: 'GET',
            success: function (data) {
                populateDropdown('#mapSkillDropdown', data, 'value', 'text');
            },
            error: function () {
                console.error("Error fetching skill categories");
            }
        });
    }

    function fetchActiveGrade() {
        $.ajax({
            url: `/DropDown/FetchActivePayGradeTypeDropdown`,
            type: 'GET',
            success: function (data) {
                populateDropdown('#mapGradeDropdown', data, 'value', 'text');
            },
            error: function () {
                console.error("Error fetching pay grades");
            }
        });
    }
    function validateFilterForm() {
        let isValid = true;
        $(".input_error_msg").text("");

        const company = $('#mapCompanyDropdown').val();
        const contractor = $('#mapContractorDropdown').val();
        const mapgrade = $('#mapGradeDropdown').val();

        if (!company) {
            $('#mapCompanyDropdown-error').text('Please select company.');
            isValid = false;
        }
        if (!contractor) {
            $('#mapContractorDropdown-error').text('Please select contractor.');
            isValid = false;
        }
        //if (!mapgrade) {
        //    $('#mapGradeDropdown-error').text('Please select grade.');
        //    isValid = false;
        //}
        return isValid;
    }

    $(document).on('click', '#resetEntityFilterBtn', function () {
        const totalCount = 0;
        const activeCount = 0;
        const inactiveCount = 0;

        // Update the counts in UI
        $('#totalEntityCount span').text(totalCount.toString().padStart(2, '0'));
        $('#totalActiveCount span').text(activeCount.toString().padStart(2, '0'));
        $('#totalInactiveCount span').text(inactiveCount.toString().padStart(2, '0'));

        // Reset dropdown values (with Select2 or similar)
        $('#mapCompanyDropdown').val(sessionCompanyId).trigger('change');
        $('#mapContractorDropdown').val('').trigger('change');
        $('#mapLocationDropdown').val('').trigger('change');
        $('#mapTradeDropdown').val('').trigger('change');
        $('#mapSkillDropdown').val('').trigger('change');
        $('#mapGradeDropdown').val('').trigger('change');
        // Clear error messages
        $(".input_error_msg").text('');
        // Clear the DataTable
        const tableId = "entity-master-list";

        if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
            $(`#${tableId}`).DataTable().clear().destroy();
        }
        makeDataTableNew(tableId);
        // Optionally replace with empty message (like in AJAX success)
        $(`#${tableId} tbody`).html(
            '<tr><td colspan="6" class="text-center">Select a filters to display the grid data.</td></tr>'
        );
    });
    $(document).on("click", '#applyEntityFilterBtn', () => loadEntityData(1));
    $(document).on("change", '#pageSizeDropdown', () => loadEntityData(1));

    function loadEntityData(pageNumber = 1) {
        if (!validateFilterForm()) return;

        const tableId = "#entity-master-list";
        const pageSize = parseInt($('#pageSizeDropdown').val()) || 10;

        const filterData = {
            SelectType: 'ET',
            CompanyId: $('#mapCompanyDropdown').val(),
            CorrespondanceId: $('#mapLocationDropdown').val() || 0,
            ContractorId: $('#mapContractorDropdown').val(),
            EntityId: 0,
            EntityCode: "",
            EntityName: "",
            SelectFilter: "",
            Regime_Id: 0,
            FinYear_ID: 0,
            Trade_Id: $('#mapTradeDropdown').val() || 0,
            Skill_ID: $('#mapSkillDropdown').val() || 0,
            Grade_ID: $('#mapGradeDropdown').val() || 0,
            PageNumber: pageNumber,
            PageSize: pageSize
        };

        $.ajax({
            url: '/EntityMaster/MapEntityGradeResponse',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(filterData),
            success(response) {
                const entities = response.gradeMapEntities || [];
                const totalRecords = response.totalRecords || 0;
                const totalPages = Math.ceil(totalRecords / pageSize);
                console.log(entities.count);
                if ($.fn.DataTable.isDataTable(tableId)) {
                    $(tableId).DataTable().clear().destroy();
                }

                bindTable(entities);

                const start = (pageNumber - 1) * pageSize + 1;
                const end = start + entities.length - 1;

                $('#pagination-info').text(`${start} – ${end} of ${totalRecords} records`);
                renderPagination(totalPages, pageNumber);

                makeDataTableNew(tableId);
                var modalEl = document.getElementById('standardFilterModal');
                var modal = bootstrap.Modal.getInstance(modalEl); // or new bootstrap.Modal(modalEl)
                modal.hide();
                // Count logic
                const totalCount = entities.length;
                const activeCount = entities.filter(e => e.isActive === true).length;
                const inactiveCount = entities.filter(e => e.isActive === false).length;

                // Update the counts in UI
                $('#totalEntityCount span').text(totalCount.toString().padStart(2, '0'));
                $('#totalActiveCount span').text(activeCount.toString().padStart(2, '0'));
                $('#totalInactiveCount span').text(inactiveCount.toString().padStart(2, '0'));

            },
            error() {
                showAlert('danger', response.message);
            }
        });
    }




    $(document).on("click", ".page-btn", function () {
        const page = parseInt($(this).data('page'));
        loadEntityData(page);
    });



    $(document).on('click', '.redirectToEntityProfile', function () {
        var entityId = $(this).data('entityid');
        console.log(entityId);
        if (entityId) {
            fetch('/EntityMaster/EncryptId?id=' + encodeURIComponent(entityId))
                .then(response => response.text())
                .then(encryptedId => {
                    window.location.href = "/EntityMaster/EntityProfile?id=" + encodeURIComponent(encryptedId);
                    // 👇 Moved here: hide dropdowns after HTML is loaded
                    $('#gradeDropdown, #salaryStructureDropdown').addClass('d-none');
                    $('#gradeText, #salaryStructureText').removeClass('d-none');
                    console.log("gradeDropdown exists:", $('#gradeDropdown').length > 0);
                    console.log("gradeDropdown class BEFORE:", $('#gradeDropdown').attr('class'));

                })
                .catch(error => console.error('Encryption error:', error));
        }
    });

    function resetAllFilters() {
        $('#mapCompanyDropdown').val(sessionCompanyId).trigger('change');
        $('#mapContractorDropdown').val('').trigger('change');
        $('#mapLocationDropdown').val('').trigger('change');
        $('#mapTradeDropdown').val('').trigger('change');
        $('#mapSkillDropdown').val('').trigger('change');
        $('#mapGradeDropdown').val('').trigger('change');
        $(".input_error_msg").text('');
    }

});
$(document).ready(function () {
    entityDataValidation();
});

function bindTable(data) {
    let tbody = '';
    data.forEach((item, index) => {
        const isChecked = item.isActive ? 'checked' : '';
        const statusText = item.isActive ? 'Active' : 'Inactive';
        const uniqueId = `status-${index}`; // ensure unique ID for label/input
        tbody += `
            <tr>
                <td hidden>${item.contractor_ID ?? ''}</td>
                <td hidden>${item.entity_ID ?? ''}</td>
                <td hidden>${item.contractor_Mast_Code ?? ''}</td>
                <td>${item.entity_Code ?? ''}</td>
                <td>${item.entity_Name ?? ''}</td>
                <td>${item.companyName ?? ''}</td>
                <td>${item.contractor_Name ?? ''}</td>
                <td>${item.locationName ?? ''}</td>
                <td>${item.departmentName ?? ''}</td>
                <td class="sticky_cell">
                    <div class="form-check form-switch">
                        <input class="form-check-input" disabled type="checkbox" role="switch" id="${uniqueId}" ${isChecked}>
                        <label class="form-check-label" for="${uniqueId}">
                            ${statusText}
                        </label>
                    </div>
                </td>

                <td class="sticky_cell">
                    <button type="button" data-entityid="${item.entity_ID}" class="btn btn-primary-light-icon-md redirectToEntityProfile" >
                        <span class="sprite-icons eye-primary-dark"></span>
                    </button>
                </td>
            </tr>
        `;
    });
    $('#entity-master-list tbody').html(tbody);
}
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString(); // Customize if needed
}

function entityDataValidation() {
    $.ajax({
        url: `/EntityMaster/EntityDataValidation`, // API that returns list of fields to validate
        type: 'GET',
        success: function (response) {
            console.log("Validation Response:", response);

            if (!response.success || !Array.isArray(response.data)) return;

            var controlNames = response.data.map(function (item) {
                return item.validation_Control_Name;
            });

            controlNames.forEach(function (controlId) {
                var $ctrl = $('#' + controlId);

                if ($ctrl.length > 0) {
                    let val;

                    // Check if input/select/textarea
                    if ($ctrl.is('input, select, textarea')) {
                        val = $ctrl.val();
                    } else {
                        val = $.trim($ctrl.text());
                    }
                    let normalizedVal = ($.trim(val) + '').toLowerCase();

                    if (
                        normalizedVal === '' ||
                        normalizedVal === '0' ||
                        normalizedVal === '0' ||
                        normalizedVal === '0.00' ||
                        normalizedVal === 'no' ||
                        normalizedVal === '-' ||
                        normalizedVal === 'select' ||
                        normalizedVal === 'undefined' ||
                        normalizedVal === 'null' ||
                        parseFloat(normalizedVal) === 0
                    ) {
                        $ctrl.css('background-color', '#D3BBF1');
                    }
                    //// Normalize and check invalid values
                    //if (val === '' || val === null || val === undefined || val === '-' || val === '0' || val == 0 || val === 'No' || val === 0.00   ) {
                    //    $ctrl.css('background-color', '#D3BBF1'); // Highlight
                    //}
                } else {
                    console.warn("Control not found:", controlId);
                }
            });
        },
        error: function () {
            console.error("Error fetching validation data.");
        }
    });
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

$(document).ready(function () {
    const gradeId = $('#gradeText').data('grade-id');
    const salaryId = $('#salaryStructureText').data('salary-id');

    // Hide the static text elements entirely (optional)
    $('#gradeText, #salaryStructureText').hide();

    // Load and populate the dropdowns on page load (disabled)
    $.ajax({
        url: '/DropDown/FetchActivePayGradeTypeDropdown',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            const $dropdown = $('#gradeDropdown');
            $dropdown.empty().append('<option value="">Select Pay Grade</option>');
            $.each(data, function (index, item) {
                const selectedAttr = item.value == gradeId ? 'selected' : '';
                $dropdown.append(`<option value="${item.value}" ${selectedAttr}>${item.text}</option>`);
            });

            $dropdown.prop('disabled', true); // Disable on load
            $dropdown.select2({
                placeholder: 'Select Pay Grade',
                width: '100%',
                dropdownParent: $dropdown.parent()
            }).addClass('select2_search_ctm');
        }
    });

    $.ajax({
        url: `/DropDown/FetchSalaryStructureDropdown?salaryId=${salaryId}`,
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            const $dropdown = $('#salaryStructureDropdown');
            $dropdown.empty().append('<option value="">Select Salary Structure</option>');
            $.each(data, function (index, item) {
                const selectedAttr = item.value == salaryId ? 'selected' : '';
                $dropdown.append(`<option value="${item.value}" ${selectedAttr}>${item.text}</option>`);
            });

            $dropdown.prop('disabled', true); // Disable on load
            $dropdown.select2({
                placeholder: 'Select Salary Structure',
                width: '100%',
                dropdownParent: $dropdown.parent()
            }).addClass('select2_search_ctm');
        }
    });

    // Edit button: enable the dropdowns
    $('#editBasicEntity').on('click', function () {
        $('#gradeDropdown, #salaryStructureDropdown').prop('disabled', false);
        $('#editBasicEntity').addClass('d-none');
        $('#cancelbasicentityinfobtn').removeClass('d-none');
    });

    // Cancel button behavior
    $('#cancelbasicentityinfobtn').click(function () {
        $('#gradeDropdown, #salaryStructureDropdown').prop('disabled', true); // Re-disable
        const id = parseInt($('#Entity_ID').val());
        fetch('/EntityMaster/EncryptId?id=' + encodeURIComponent(id))
            .then(response => response.text())
            .then(encryptedId => {
                window.location.href = "/EntityMaster/EntityProfile?id=" + encodeURIComponent(encryptedId);
            })
            .catch(error => console.error('Encryption error:', error));
    });
});





$(document).ready(function () {
    const countryId = 99;
    const selectedStateId = $('#ptStateSelectedId').val();

    // Load dropdown and keep it disabled with selected value shown
    if (countryId) {
        $.ajax({
            url: `/DropDown/FetchStateDropdown/${countryId}`,
            type: 'GET',
            success: function (data) {
                const $dropdown = $('#ptStateInput');
                $dropdown.empty().append('<option value="">Select PT State</option>');

                $.each(data, function (index, item) {
                    const selectedAttr = item.value == selectedStateId ? 'selected' : '';
                    $dropdown.append(`<option value="${item.value}" ${selectedAttr}>${item.text}</option>`);
                });

                // Initialize Select2
                $dropdown.select2({
                    placeholder: 'Select PT State',
                    width: '100%',
                    dropdownParent: $dropdown.parent()
                });

                // Keep it disabled until edit
                $dropdown.prop('disabled', true);
            },
            error: function () {
                console.error('Failed to fetch state dropdown');
            }
        });
    }

    $('#editComplainceIdentification').on('click', function () {
        $('#ptStateInput').prop('disabled', false); // Enable dropdown

        const editableFields = [
            'pfApplicable',
            'vpfValue',
            'LwfApplicable',
            'gyApplicable',
            'premimumDeduction',
            'pfamount',
            'ptApplicable',
            'dedAmount',
            'VpfApplicable',
            'policyNo',
            'vpftype'
        ];

        editableFields.forEach(function (field) {
            $(`#${field}Text`).addClass('d-none');
            $(`#${field}Input`).removeClass('d-none');
        });

        // Handle PT State
        $('#ptStateText').addClass('d-none');          // Hide static text
        $('#ptStateLabel').removeClass('d-none');      // Show label
        $('#ptStateInput').prop('disabled', false);    // Enable dropdown
        
        // Set input/select values from text
        $('#pfApplicableInput').val($('#pfApplicableText').text().trim() === 'Yes' ? 'Yes' : 'No');
        $('#vpfValueInput').val($('#vpfValueText').text().trim());
        $('#LwfApplicableInput').val($('#LwfApplicableText').text().trim() === 'Yes' ? '1' : '0');
        $('#gyApplicableInput').val($('#gyApplicableText').text().trim() === 'Yes' ? '1' : '0');
        $('#premimumDeductionInput').val($('#premimumDeductionText').text().trim() === 'Yes' ? '1' : '0');
        $('#pfamountInput').val($('#pfamountText').text().trim());
        $('#ptApplicableInput').val($('#ptApplicableText').text().trim() === 'Yes' ? '1' : '0');
        $('#dedAmountInput').val($('#dedAmountText').text().trim());
        $('#VpfApplicableInput').val($('#VpfApplicableText').text().trim() === 'Yes' ? 'Yes' : 'No');
        $('#policyNoInput').val($('#policyNoText').text().trim());
        $('#vpfCalculationType').val($('#vpftypeText').text().trim());

        const readonlyFields = ['pfNo', 'esicNo', 'uan', 'esicExitDate'];
        readonlyFields.forEach(function (field) {
            $(`#${field}Text`).removeClass('d-none');
            $(`#${field}Input`).addClass('d-none');
        });

        $('#editComplainceIdentification').addClass('d-none');
        $('#saveComplainceIdentification, #cancelComplainceIdentification').removeClass('d-none');

        // Trigger change to show/hide related containers
        $('#pfApplicableInput').trigger('change');
        $('#VpfApplicableInput').trigger('change');
    });

    $('#cancelComplainceIdentification').click(function () {
        $('#ptStateInput').prop('disabled', true);
        const id = parseInt($('#Entity_ID').val());
        fetch('/EntityMaster/EncryptId?id=' + encodeURIComponent(id))
            .then(response => response.text())
            .then(encryptedId => {
                window.location.href = "/EntityMaster/EntityProfile?id=" + encodeURIComponent(encryptedId);
            })
            .catch(error => console.error('Encryption error:', error));
    });


    $('#pfApplicableInput').on('change', function () {
        if ($(this).val() === '1') { // 1 means Yes
            $('#pfCalculationTypeContainer').show();
        } else {
            $('#pfCalculationTypeContainer, #pfPercentContainer, #pfAmountContainer').hide();
        }
    });

    $('#pfCalculationType').on('change', function () {
        const value = $(this).val();
        if (value === 'Percent') {
            $('#pfPercentContainer').show();
            $('#pfAmountContainer').hide();
        } else if (value === 'Amount') {
            $('#pfAmountContainer').show();
            $('#pfPercentContainer').hide();
        } else {
            $('#pfPercentContainer, #pfAmountContainer').hide();
        }
    });

    $('#VpfApplicableInput').on('change', function () {
        if ($(this).val() === '1') { // 1 means Yes
            $('#vpfCalculationTypeContainer').show();
        } else {
            $('#vpfCalculationTypeContainer, #vpfPercentContainer, #vpfAmountContainer').hide();
        }
    });

    $('#vpfCalculationType').on('change', function () {
        const value = $(this).val();
        if (value === 'Percent') {
            $('#vpfPercentContainer').show();
            $('#vpfAmountContainer').hide();
        } else if (value === 'Amount') {
            $('#vpfAmountContainer').show();
            $('#vpfPercentContainer').hide();
        } else {
            $('#vpfPercentContainer, #vpfAmountContainer').hide();
        }
    });

    // Input validations
    $('#pfAmountInput, #dedAmountInput, #vpftypeInput, #pfPercentInput, #vpfPercentInput, #vpfAmountInput').on('input', function () {
        const fieldId = `#${this.id}`;
        const errorId = `#${this.id}-error`;
        validateNumericField(fieldId, errorId);
    });

    function validateNumericField(selector, errorSelector) {
        const val = $(selector).val().trim();
        let error = '';
        if (val) {
            const regex = /^\d+(\.\d{1,2})?$/;
            if (!regex.test(val)) {
                error = 'Invalid number format';
            }
        }
        $(errorSelector).text(error);
        return error === '';
    }

    function validateComplainceForm() {
        let isValid = true;

        // PF Amount
        if (!validateNumericField('#pfAmountInput', '#pfAmountText-error')) isValid = false;

        // Deduction Amount
        if (!validateNumericField('#dedAmountInput', '#dedAmountText-error')) isValid = false;

        // VPF Input
        if (!validateNumericField('#vpftypeInput', '#vpftypeInput-error')) isValid = false;

        // PF Logic - only validate further if "Yes" is selected
        if ($('#pfApplicableInput').val() === 'Yes') {
            const pfType = $('#pfCalculationType').val();

            if (!pfType) {
                /* alert("Please select PF Calculation Type");*/
                $("#pfCalculationType-error").text("Please select PF Calculation Type");
                isValid = false;
            } else if (pfType === 'Percent') {
                if (!validateNumericField('#pfPercentInput', '#pfPercent-error')) isValid = false;
            } else if (pfType === 'Amount') {
                if (!validateNumericField('#pfAmountInput', '#pfAmount-error')) isValid = false;
            }
        }

        // VPF Logic - only validate further if "Yes" is selected
        if ($('#VpfApplicableInput').val() === 'Yes') {
            const vpfType = $('#vpfCalculationType').val();

            if (!vpfType) {
                //alert("Please select VPF Calculation Type");
                $("#vpfCalculationType-error").text("Please select VPF Calculation Type");
                isValid = false;
            } else if (vpfType === 'Percent') {
                if (!validateNumericField('#vpfPercentInput', '#vpfPercent-error')) isValid = false;
            } else if (vpfType === 'Amount') {
                if (!validateNumericField('#vpfAmountInput', '#vpfAmount-error')) isValid = false;
            }
        }

        return isValid;
    }

    // PF change handlers
    $('#pfApplicableInput').change(function () {
        const value = $(this).val();
        if (value === 'Yes') {
            $('#pfCalculationTypeContainer').show();
        } else {
            $('#pfCalculationTypeContainer, #pfPercentContainer, #pfAmountContainer').hide();
        }
    });

    $('#pfCalculationType').change(function () {
        const type = $(this).val();
        $('#pfPercentContainer').toggle(type === 'Percent');
        $('#pfAmountContainer').toggle(type === 'Amount');
    });

    // VPF change handlers
    $('#VpfApplicableInput').change(function () {
        const value = $(this).val();
        if (value === 'Yes') {
            $('#vpfCalculationTypeContainer, #vpfValueContainer').show();
        } else {
            $('#vpfCalculationTypeContainer, #vpfPercentContainer, #vpfAmountContainer, #vpfValueContainer').hide();
        }
    });

    $('#vpfCalculationType').change(function () {
        const type = $(this).val();
        $('#vpfPercentContainer').toggle(type === 'Percent');
        $('#vpfAmountContainer').toggle(type === 'Amount');
    });

    // Submit
    $(document).on('click', '#saveComplainceIdentification', function () {
        if (!validateComplainceForm()) return;

        const entityComplianceDTO = {
            Entity_ID: parseInt($('#Entity_ID').val()),
            Pf_No: $('#pfNoInput').val(),
            Uan_No: $('#uanInput').val(),
            EsicNo: $('#esicNoInput').val(),
            Pf_Applicable: $('#pfApplicableInput').val() === "Yes" ? 1 : 0,
            Vpf_Applicable: $('#VpfApplicableInput').val() === "Yes" ? 1 : 0,
            Vpf_percent: parseFloat($('#vpfPercentInput').val()) || 0,
            Vpf_Value: parseFloat($('#vpfAmountInput').val()) || 0,
            Pt_Applicable: parseInt($('#ptApplicableInput').val()) || 0,
            Pt_State_ID: parseInt($('#ptStateInput').val()) || 0,
            Lwf_Applicable: parseInt($('#LwfApplicableInput').val()) || 0,
            Esi_Exit_Date: $('#esicExitDateInput').val() || null,
            Pay_Grade_Id: parseInt($('#gradeDropdown').val()) || $('#gradeText').data('grade-id'),
            Policy_No: $('#policyNoInput').val(),
            PolicyAmt: parseFloat($('#dedAmountInput').val()) || 0,
            GratuityApplicable: parseInt($('#gyApplicableInput').val()) || 0,
            Pf_Amount: parseFloat($('#pfAmountInput').val()) || 0,
            Pf_Percent: parseFloat($('#pfPercentInput').val()) || 0,
            UpdatedBy: 0
        };
        $.ajax({
            url: '/EntityMaster/UpdateEntityCompliance',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(entityComplianceDTO),
            success: function (response) {
                if (response.success) {
                    setTimeout(function () {
                        showAlert("success", response.message);
                    }, 2000);
                    window.location.href = `/EntityMaster/EntityList`;
                } else {
                    showAlert("danger", response.message);
                }
            },
            error: function (xhr, status, error) {
                alert("An error occurred while processing: " + error);
            }
        });
    });
});

