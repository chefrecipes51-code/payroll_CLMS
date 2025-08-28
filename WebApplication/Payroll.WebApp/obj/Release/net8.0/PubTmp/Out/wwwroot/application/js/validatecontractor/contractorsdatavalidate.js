/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 10-June-2025
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>
//function loadContractorValidationData() {
//    return $.ajax({
//        url: '/ContractorValidation/GetContractorValidationData',
//        type: 'GET',
//        contentType: 'application/json',
//        success: function (response) {
//            if (response.success) {
//                //console.log(response);
//                var rows = "";
//                var hasAnyMissing = false;
//                $.each(response.data, function (i, row) {

//                    const isMissing = row.isMissing_BusinessAreaCode ||
//                        row.isMissing_CompanyCode ||
//                        row.isMissing_ContactNo ||
//                        row.isMissing_LicenseNo ||
//                        row.isMissing_MaxLabourCount ||
//                        row.isMissing_PANNo ||
//                        row.isMissing_TANNo ||
//                        row.isMissing_WorkOrderNo ||
//                        row.isMissing_WorkOrderValue;

//                    if (!hasAnyMissing) {
//                        isMissing = row.isMissing_BusinessAreaCode ||
//                            row.isMissing_CompanyCode ||
//                            row.isMissing_ContactNo ||
//                            row.isMissing_LicenseNo ||
//                            row.isMissing_MaxLabourCount ||
//                            row.isMissing_PANNo ||
//                            row.isMissing_TANNo ||
//                            row.isMissing_WorkOrderNo ||
//                            row.isMissing_WorkOrderValue;

//                        if (isMissing) {
//                            hasAnyMissing = true; // 🛑 Stop checking further
//                        }
//                    }

//                    const checkboxDisabledAttr = isMissing ? 'disabled' : '';
//                    const statusText = row.isActive ? 'Active' : 'Inactive';


//                    rows +=
//                    `<tr>
//                        <td class="sticky_cell checkbox">
//                            <div class="input-group">
//                                <input type="checkbox" class="form-check-input mt-0 row-checkbox"
//                                    data-row-index="${i}" data-contractor-id="${row.contractor_Id}" ${checkboxDisabledAttr} />
//                            </div>
//                        </td>
//                        <td>${row.contractorName ?? ''}</td>
//                           <td>${row.subContractorName ?? ''}</td>
//                       <td>
//                            <div class="chip ${row.isActive ? 'success' : 'danger'}">
//                                ${row.isActive ? 'Active' : 'Inactive'}
//                            </div>
//                        </td>
//                      <td class="p-0">
//                        ${row.isMissing_ContactNo
//                                            ? `<div class="editable-cell">
//                                    <div class="d-flex align-items-center justify-content-between">
//                                        <p>${row.contactNo ?? 'Null'}</p>

//                                    </div>
//                               </div>`
//                                            : row.contactNo ?? ''}
//                    </td>

//                       <td class="p-0">
//                            ${row.isMissing_LicenseNo
//                                                ? `<div class="editable-cell">
//                                        <div class="d-flex align-items-center justify-content-between">
//                                            <p>${row.licenseNo ?? 'Null'}</p>

//                                        </div>
//                                   </div>`
//                                                : row.licenseNo ?? ''}
//                        </td>

//                      <td class="p-0">
//                        ${row.isMissing_MaxLabourCount
//                                            ? `<div class="editable-cell">
//                                    <div class="d-flex align-items-center justify-content-between">
//                                        <p>${row.maxLabourCount ?? 'Null'}</p>

//                                    </div>
//                               </div>`
//                                            : row.maxLabourCount ?? ''}
//                    </td>

//                       <td>${row.assigned_labour_Count ?? ''}</td>
//                      <td class="p-0">
//                        ${row.isMissing_PANNo
//                                            ? `<div class="editable-cell">
//                                    <div class="d-flex align-items-center justify-content-between">
//                                        <p>${row.panNo ?? 'Null'}</p>

//                                    </div>
//                               </div>`
//                                            : row.panNo ?? ''}
//                        </td>

//                       <td class="p-0">
//                            ${row.isMissing_TANNo
//                                                ? `<div class="editable-cell">
//                                        <div class="d-flex align-items-center justify-content-between">
//                                            <p>${row.tanNo ?? 'Null'}</p>

//                                        </div>
//                                   </div>`
//                                                : row.tanNo ?? ''}
//                        </td>


//                        <td>${row.email_Id ?? ''}</td>
//                        <td>${row.address ?? ''}</td>

//                        <td>${row.liN_No ?? ''}</td>
//                        <td>${row.epF_No ?? ''}</td>
//                        <td>${row.esiC_No ?? ''}</td>
//                        <td>${row.workOrderNo ?? ''}</td>
//                        <td>
//                            ${row.hasWorkOrderValue ?? ''}
//                        </td>
//                    </tr>`;
//                });

//                $("#contractor-body").html(rows);
//               // makeDataTable("validatecontractors-list");
//            } else {
//                $("#contractor-body").html(`<tr><td colspan="22" class="text-danger">${response.message}</td></tr>`);
//            }
//        },
//        error: function () {
//            $("#contractor-body").html(`<tr><td colspan="22" class="text-danger">Error loading data.</td></tr>`);
//        }
//    });
//}

function loadContractorValidationData() {
    return $.ajax({
        url: '/ContractorValidation/GetContractorValidationData',
        type: 'GET',
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                var rows = "";
                var hasAnyMissing = false;

                $.each(response.data, function (i, row) {
                    // ✅ Check missing status only once
                    const isMissing = row.isMissing_BusinessAreaCode ||
                        row.isMissing_CompanyCode ||
                        row.isMissing_ContactNo ||
                        row.isMissing_LicenseNo ||
                        row.isMissing_MaxLabourCount ||
                        row.isMissing_PANNo ||
                        row.isMissing_TANNo ||
                        row.isMissing_WorkOrderNo ||
                        row.isMissing_WorkOrderValue;

                    if (isMissing && !hasAnyMissing) {
                        hasAnyMissing = true; // ✅ Set once
                    }

                    const checkboxDisabledAttr = isMissing ? 'disabled' : '';
                    const statusText = row.isActive ? 'Active' : 'Inactive';

                    rows += `
                    <tr>
                        <td class="sticky_cell checkbox">
                            <div class="input-group">
                                <input type="checkbox" class="form-check-input mt-0 row-checkbox" 
                                    data-row-index="${i}" data-contractor-id="${row.contractor_Id}" ${checkboxDisabledAttr} />
                            </div>
                        </td>
                        <td>${row.contractorName ?? ''}</td>
                        <td>${row.subContractorName ?? ''}</td>
                        <td>
                            <div class="chip ${row.isActive ? 'success' : 'danger'}">
                                ${statusText}
                            </div>
                        </td>
                        <td class="p-0">
                            ${row.isMissing_ContactNo ? `
                                <div class="editable-cell">
                                    <div class="d-flex align-items-center justify-content-between">
                                        <p>${row.contactNo ?? 'Null'}</p>
                                    </div>
                                </div>` : (row.contactNo ?? '')}
                        </td>
                        <td class="p-0">
                            ${row.isMissing_LicenseNo ? `
                                <div class="editable-cell">
                                    <div class="d-flex align-items-center justify-content-between">
                                        <p>${row.licenseNo ?? 'Null'}</p>
                                    </div>
                                </div>` : (row.licenseNo ?? '')}
                        </td>
                        <td class="p-0">
                            ${row.isMissing_MaxLabourCount ? `
                                <div class="editable-cell">
                                    <div class="d-flex align-items-center justify-content-between">
                                        <p>${row.maxLabourCount ?? 'Null'}</p>
                                    </div>
                                </div>` : (row.maxLabourCount ?? '')}
                        </td>
                        <td>${row.assigned_labour_Count ?? ''}</td>
                        <td class="p-0">
                            ${row.isMissing_PANNo ? `
                                <div class="editable-cell">
                                    <div class="d-flex align-items-center justify-content-between">
                                        <p>${row.panNo ?? 'Null'}</p>
                                    </div>
                                </div>` : (row.panNo ?? '')}
                        </td>
                        <td class="p-0">
                            ${row.isMissing_TANNo ? `
                                <div class="editable-cell">
                                    <div class="d-flex align-items-center justify-content-between">
                                        <p>${row.tanNo ?? 'Null'}</p>
                                    </div>
                                </div>` : (row.tanNo ?? '')}
                        </td>
                        <td>${row.email_Id ?? ''}</td>
                        <td>${row.address ?? ''}</td>
                        <td>${row.liN_No ?? ''}</td>
                        <td>${row.epF_No ?? ''}</td>
                        <td>${row.esiC_No ?? ''}</td>
                        <td>${row.workOrderNo ?? ''}</td>
                        <td>${row.hasWorkOrderValue ?? ''}</td>
                    </tr>`;
                });

                $("#contractor-body").html(rows);

                // ✅ Fire alert once if any missing data found
                if (hasAnyMissing) {
                    showAlert("warning", "Please contact the administrator for data correction.");
                }
            } else {
                $("#contractor-body").html(`<tr><td colspan="22" class="text-danger">${response.message}</td></tr>`);
            }
        },
        error: function () {
            $("#contractor-body").html(`<tr><td colspan="22" class="text-danger">Error loading data.</td></tr>`);
        }
    });
}

function formatDate(dateStr) {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-GB'); // e.g., "10/06/2025"
}
$(document).ready(function () {

    ///////////////////////////////////////Load Contractor AT PAGE LOAD Details:- Start/////////////////////////////////////////////
    loadContractorValidationData().done(function () {
        bindSessionPreviousMonthYear();
    });
    ///////////////////////////////////////Load Contractor AT PAGE LOAD Details:- End//////////////////////////////////////////////

    //////////////////////////////////////Checkbox Check And UnChecked :- Start ///////////////////////////////////////
    //////////////////////////Contractor SELECT ALL
    $(document).on('change', '#selectAllContractor', function () {
        const isChecked = $(this).is(':checked');
        $('.row-checkbox:not(:disabled)').prop('checked', isChecked);
    });

    $(document).on('change', '.row-checkbox', function () {
        const total = $('.row-checkbox:not(:disabled)').length;
        const checked = $('.row-checkbox:checked:not(:disabled)').length;
        $('#selectAllContractor').prop('checked', total === checked);
    });
    //////////////////////////Entity SELECT ALL
    $(document).on('change', '#selectAllEntity', function () {
        const isChecked = $(this).is(':checked');
        $('#validateentity-list .entity-checkbox:not(:disabled)').prop('checked', isChecked);
    });
    $(document).on('change', '#validateentity-list .entity-checkbox', function () {
        const $all = $('#validateentity-list .entity-checkbox:not(:disabled)');
        const $checked = $('#validateentity-list .entity-checkbox:checked:not(:disabled)');
        $('#selectAllEntity').prop('checked', $all.length === $checked.length);
    });
    /////////////////////////PAY TAB SELECT ALL
    $(document).on('change', '#selectAllPay', function () {
        const isChecked = $(this).is(':checked');
        $('#validatepay-list .pay-checkbox:not(:disabled)').prop('checked', isChecked);
    });
    $(document).on('change', '#validatepay-list .pay-checkbox', function () {
        const $all = $('#validatepay-list .pay-checkbox:not(:disabled)');
        const $checked = $('#validatepay-list .pay-checkbox:checked:not(:disabled)');
        $('#selectAllPay').prop('checked', $all.length === $checked.length);
    });

    /////////////////////////////// Checkbox Check And UnChecked :- End///////////////////////////////////////////////


    ////////////////////////////////////// PASS SELECTED CHECKBOX ID:- Start//////////////////////////////////////////
    $(document).on('click', '#btnfirsttab', function () {
        var selectedIds = [];

        $('.row-checkbox:checked:not(:disabled)').each(function () {
            selectedIds.push($(this).data('contractor-id'));
        });

        if (selectedIds.length === 0) {
            alert("Please select at least one valid contractor.");
            return;
        }
        //console.log("Execute SP", selectedIds);
        $.ajax({
            url: '/ContractorValidation/SubmitSelectedContractors',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(selectedIds),
            success: function (response) {
                if (response.success) {
                    $.ajax({
                        url: '/ContractorValidation/GetEntityValidationData',
                        type: 'POST',
                        contentType: 'application/json',
                        data: JSON.stringify({ contractorIds: selectedIds }),
                        success: function (res) {
                            if (res.success && res.data) {
                                //console.log('EntityExecute', res.data);
                                fetchGradeDropdownOptionsOnce().done(function (dropdownRes) {
                                    if (dropdownRes.success && dropdownRes.result) {
                                        gradeConfigOptions = dropdownRes.result;
                                        bindEntityValidationData(res.data);
                                        makeDataTable("validateentity-list");
                                        activateTab('entity-status-tab', 'entity-status-pane');
                                    }
                                    else
                                    {
                                        alert(dropdownRes.message || "Unable to fetch grade config.");
                                    }
                                }).fail(function () {
                                    alert("Failed to fetch grade dropdown.");
                                });
                            } else {
                                alert(res.message);
                            }
                        },
                        error: function () {
                            alert("Error occurred while calling GetEntityValidation.");
                        }
                    });
                }
            },
            error: function () {
                alert("Error submitting data.");
            }
        });
    });

    ////////////////////////////////////// PASS SELECTED CHECKBOX ID:- End//////////////////////////////////////////
});
////////////////////////////////////////////////////TAB SELECTION LOGIC:-start///////////////////////////////////////
function activateTab(tabId, paneId) {

    $('.nav-link').removeClass('active');
    $('.tab-pane').removeClass('show active');
    $('.nav-link').attr('aria-selected', 'false');

    $('#' + tabId).addClass('active').attr('aria-selected', 'true');
    $('#' + paneId).addClass('show active');
}
function activateTabManually(tabId, paneId) {
    $('.nav-link').removeClass('active');
    $('.tab-pane').removeClass('show active');
    $('.nav-link').attr('aria-selected', 'false');

    $(`#${tabId}`).addClass('active').attr('aria-selected', 'true');
    $(`#${paneId}`).addClass('show active');
}
$(document).ready(function () {
    //function switchTab(targetSelector) {
    //    // Remove 'active' class from all tab buttons
    //    $('.nav-link').removeClass('active');

    //    // Add 'active' class to the tab button with matching data-target
    //    $('.nav-link[data-target="' + targetSelector + '"]').addClass('active');

    //    // Hide all tab content sections
    //    $('.tab-section').addClass('d-none');

    //    // Show the selected tab section
    //    $(targetSelector).removeClass('d-none');
    //}

    //// ✅ On page load - show Approval tab and section
    //switchTab('#approval-tab-section');

    //// ✅ On manual tab click (optional)
    //$('.nav-link').on('click', function () {
    //    const target = $(this).data('target');
    //    switchTab(target);
    //});
});
////////////////////////////////////////////////////TAB SELECTION LOGIC:-end///////////////////////////////////////


////////////////////////////////////////////////////ENTITY TAB EDIT CLICK:-start///////////////////////////////////////
function bindEntityValidationData(data) {
    var $tbody = $('#entiry-body');
    $tbody.empty();

    data.forEach(function (entity, i) { 
        var hasWorkOrderValue = entity.hasWorkOrderValue ?? '';
        var isNo = hasWorkOrderValue.toLowerCase() === 'no';
        var row = `
            <tr> 
                <td class="sticky_cell checkbox">
                    <div class="input-group">
                        <input type="checkbox"
                               class="form-check-input mt-0 entity-checkbox"
                               data-entity-id="${entity.entity_ID}"
                               ${entity.isMissing_GradeConfigName ? 'disabled' : ''} />
                    </div>
                </td>
                <td>${entity.entityCode ?? ''}</td>
                <td>${entity.entityName ?? ''}</td>             
                <td>${entity.dateOfDeployment ? formatDate(entity.dateOfDeployment) : ''}</td>
                <td>${entity.validityDate ? formatDate(entity.validityDate) : ''}</td>
                <td>${entity.aadharNo ?? ''}</td>
                <td>${entity.companyName ?? ''}</td>
                <td>${entity.skillCategory ?? ''}</td>                    
                <td class="p-0">
                    ${entity.isMissing_GradeConfigName
                                ? `<div class="editable-cell" data-col="GradeConfigName" data-row="${i}" data-entity-id="${entity.entity_ID}">
                                <div class="d-flex align-items-center justify-content-between">
                                    <p class="cell-value" data-selected-id="${entity.gradeConfigId ?? ''}">
                                        ${entity.gradeConfigName ?? 'Null'}
                                    </p>
                                    <button type="button" class="btn edit-icon">
                                        <span class="sprite-icons edit-secondary-lg"></span>
                                    </button>
                               </div>
                           </div>`
                    : entity.gradeConfigName ?? ''}
                </td>
                <td>${entity.entityTrade ?? ''}</td>
                <td>${entity.entityType ?? ''}</td>
                <td>${entity.contractorName ?? ''}</td>
                <td>${entity.workorderNo ?? ''}</td>
                <td>${entity.workOrderStart ? formatDate(entity.workOrderStart) : ''}</td>
                <td>${entity.workOrderEnd ? formatDate(entity.workOrderEnd) : ''}</td>
            </tr>
        `;
        $tbody.append(row);
    });
}
var gradeConfigOptions = []; // global scoped
function fetchGradeDropdownOptionsOnce() {
    return $.ajax({
        url: '/ContractorValidation/GetGradeConfigDropdown',
        type: 'GET'
    });
}

    $(document).ready(function () {
        $('#validateentity-list').on('click', '.edit-icon', function () {
            const $cell = $(this).closest('.editable-cell');
            const col = $cell.data('col');
            const rowIndex = $cell.data('row');
            const entityId = $cell.data('entity-id');
            const selectedValue = $cell.find('.cell-value').data('selected-id') ?? '';

            const selectId = `${col}_${rowIndex}`;
            var dropdownHtml = `
            <select id="${selectId}" class="form-select form-select-sm grade-dropdown" 
                    data-row="${rowIndex}" data-entity-id="${entityId}">
                <option disabled ${selectedValue === '' ? 'selected' : ''} value="">Select</option>`;

            gradeConfigOptions.forEach(option => {
                const selected = option.id == selectedValue ? 'selected' : '';
                dropdownHtml += `<option value="${option.id}" ${selected}>${option.name}</option>`;
            });
            dropdownHtml += `</select>`;
            $cell.html(dropdownHtml);
        });
        $('#validateentity-list').on('change', '.grade-dropdown', function () {
            const $dropdown = $(this);
            const selectedVal = $dropdown.val();
            const $row = $dropdown.closest('tr');
            const $checkbox = $row.find('.entity-checkbox');
            if (selectedVal && selectedVal !== '') {
                $checkbox.prop('disabled', false);
            } else {
                $checkbox.prop('disabled', true);
            }
        });
    });


$(document).on('click', '#btnsecondtab', function () {
    var selectedEntities = [];
    $('#validateentity-list tbody tr').each(function () {
        var $row = $(this);
        var $checkbox = $row.find('.entity-checkbox');

        if ($checkbox.prop('checked')) {
            var entityId = $checkbox.data('entity-id');
            var gradeConfigId = null;

            // If dropdown exists (editable), get value from <select>
            var $dropdown = $row.find('select.grade-dropdown');
            if ($dropdown.length > 0) {
                gradeConfigId = parseInt($dropdown.val());
            } else {
              
                var $span = $row.find('.cell-value');
                var selectedId = $span.data('selected-id');
                if (selectedId !== undefined && selectedId !== null && selectedId !== '') {
                    gradeConfigId = parseInt(selectedId);
                }
            }

            if (!isNaN(entityId)) {
                selectedEntities.push({
                    Entity_ID: entityId,
                    GradeConfigName: !isNaN(gradeConfigId) ? gradeConfigId : null
                });
            }
        }
    });

    // Step 1: Validate at least one selected
    if (selectedEntities.length === 0) {
        alert("Please select at least one entity to proceed.");
        return;
    }

    // Step 2: Prepare final payload
    var payload = {      
        EntityUpdateList: selectedEntities
    };
   //console.log("payload", payload);
    // Step 3: Send AJAX POST
    $.ajax({
        url: '/ContractorValidation/SubmitSelectedEntities',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (response) {
            if (response.success) {
                //alert("Entities updated successfully.");
                // Optionally go to next tab here
                var entityIds = selectedEntities.map(e => e.Entity_ID);
               // console.log("entityIds For Pay", entityIds);
                fetchPayValidationData(entityIds);
            } else {
                alert(response.message || "Update failed.");
            }
        },
        error: function () {
            alert("An error occurred while saving entity data.");
        }
    });
});

////////////////////////////////////////////////////ENTITY TAB EDIT CLICK:-End///////////////////////////////////////


///////////////////////////////////////////////////// PAY CALCULATION TAB:- Start////////////////////////////////////////
var salaryStructureOptions = []; // global
function fetchSalaryDropdownOptionsOnce() {
    return $.ajax({
        url: '/ContractorValidation/GetSalaryConfigDropdown',
        type: 'GET'
    });
}
function fetchPayValidationData(entityIds) {
    if (!Array.isArray(entityIds) || entityIds.length === 0) {
        alert("No entities provided for pay validation.");
        return;
    }

    $.ajax({
        url: '/ContractorValidation/SelectedEntitiesForPayValidation',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(entityIds),
        success: function (res) {
            console.log("FETCH PAY Success:---", res);
            if (res.success) {    
                fetchSalaryDropdownOptionsOnce()
                    .done(function (dropdownRes) {
                        if (dropdownRes.success && dropdownRes.result) {
                            console.log("FETCH PAY:---", res.result);
                            salaryStructureOptions = dropdownRes.result;
                            populatePayTab(res.data);    
                            if ($.fn.DataTable.isDataTable('#validatepay-list')) {
                                $('#validatepay-list').DataTable().destroy();
                            }
                            makeDataTable("validatepay-list");
                            activateTab('pay-tab', 'pay-tab-pane');

                        } else {
                            alert(dropdownRes.message || "Unable to fetch grade config.");
                        }
                    })
                    .fail(function () {
                        alert("Failed to fetch grade dropdown.");
                    });
                ////////////////////
            } else {
                alert(res.message || "Pay validation failed.");
            }
        },
        error: function () {
            alert("Error fetching pay validation data.");
        }
    });
}
function populatePayTab(data) {
    var $tbody = $('#pay-body');
    $tbody.empty(); // Clear previous content

    data.forEach(function (item, i) {
        var row = `
            <tr>
                <td class="sticky_cell checkbox">
                    <div class="input-group">
                        <input type="checkbox"
                               class="form-check-input mt-0 pay-checkbox"
                               data-entity-id="${item.entity_ID}"
                               ${item.isMissing_SalaryStructureName ? 'disabled' : ''} />
                    </div>
                </td>              
                <td>${item.entityCode || ''}</td>
                <td>${item.entityNamed || ''}</td>
                <td>${item.gender || ''}</td>
                <td>${formatDate(item.dateOfDeployment)}</td>
                <td>${formatDate(item.validityDate)}</td>
                <td>${item.aadharNo || ''}</td>
                <td>${item.companyName || ''}</td>
                <td>${item.skillCategory || ''}</td>
                <td>${item.entityType || ''}</td>
                <td>${item.gradeConfigName || ''}</td>
                <td>${item.contractorName || ''}</td>
                <td>${item.workorderNo || ''}</td>
                <td>${formatDate(item.workOrderStart)}</td>
                <td>${formatDate(item.workOrderEnd)}</td>              
                <td class="p-0">
                    ${item.isMissing_SalaryStructureName
                                ? `<div class="editable-cell" data-col="SalaryStructure" data-row="${i}" data-entity-id="${item.entity_ID}">
                                <div class="d-flex align-items-center justify-content-between">
                                    <p class="cell-value" data-selected-id="${item.salaryStructureId ?? ''}">
                                        ${item.salaryStructureName ?? 'Null'}
                                    </p>
                                    <button type="button" class="btn edit-icon">
                                        <span class="sprite-icons edit-secondary-lg"></span>
                                    </button>
                                </div>
                            </div>`
                                : item.salaryStructureName ?? '-'}
                </td>
                

            </tr>`;
        $tbody.append(row);
    });
}
$(document).on('click', '#validatepay-list .edit-icon', function () {
   
    const $cell = $(this).closest('.editable-cell');
    const col = $cell.data('col');
    const rowIndex = $cell.data('row');
    const selectedValue = $cell.find('.cell-value').data('selected-id') ?? '';
    const selectId = `${col}_${rowIndex}`;

    // Build dropdown
    var dropdownHtml = `
        <select id="${selectId}" class="form-select form-select-sm salary-dropdown" data-row="${rowIndex}">
            <option ${selectedValue === '' ? 'selected' : ''} disabled value="">Select Salary Structure</option>`;

    salaryStructureOptions.forEach(option => {
        const selected = option.id == selectedValue ? 'selected' : '';
        dropdownHtml += `<option value="${option.id}" ${selected}>${option.name}</option>`;
    });

    dropdownHtml += '</select>';

    // Replace the editable cell with dropdown
    $cell.closest('td').html(dropdownHtml);
});
$(document).on('change', '.salary-dropdown', function () {
    const $dropdown = $(this);
    const selectedVal = $dropdown.val();
    const $row = $dropdown.closest('tr');
    const $checkbox = $row.find('.pay-checkbox');

    if (selectedVal && selectedVal !== '') {
        $checkbox.prop('disabled', false);
    } else {
        $checkbox.prop('disabled', true);
    }
});

$(document).on('click', '#btnthirdtab', function () {
    var selectedPayEntities = [];
    $('#validatepay-list tbody tr').each(function () {
        var $row = $(this);
        var $checkbox = $row.find('.pay-checkbox');

        if ($checkbox.prop('checked')) {
            var entityId = $checkbox.data('entity-id');
            var salaryStructureId = null;

            // Editable mode with <select>
            //var $dropdown = $row.find('select.salary-structure-dropdown');
            var $dropdown = $row.find('select.salary-dropdown');
            if ($dropdown.length > 0) {
                salaryStructureId = parseInt($dropdown.val());
            } else {
                // Read-only mode with <span>
                var $span = $row.find('.cell-value');
                var selectedId = $span.data('selected-id');
                if (selectedId !== undefined && selectedId !== null && selectedId !== '') {
                    salaryStructureId = parseInt(selectedId);
                }
            }

            if (!isNaN(entityId)) {
                selectedPayEntities.push({
                    Entity_ID: entityId,
                    SalaryStructureId: !isNaN(salaryStructureId) ? salaryStructureId : null
                });
            }
        }
    });

    // Validation
    if (selectedPayEntities.length === 0) {
        alert("Please select at least one entity with a salary structure.");
        return;
    }

    // Final payload
    var payPayload = {
        EntityStructureUpdateList: selectedPayEntities
    };
    console.log("btnthirdtab",payPayload);
    // Submit to server
    $.ajax({
        url: '/ContractorValidation/SubmitSelectedPayCal',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payPayload),
        success: function (response) {
            if (response.success) {
                var entityIds = selectedPayEntities.map(e => e.Entity_ID);
                fetchComplianceData(entityIds);
                

            } else {
                alert(response.message || "Failed to save pay data.");
            }
        },
        error: function () {
            alert("An error occurred while saving pay data.");
        }
    });
});
///////////////////////////////////////////////////// PAY CALCULATION TAB:- End////////////////////////////////////////


///////////////////////////////////////////////////// Compliance TAB:- Start////////////////////////////////////////
function populateComplianceTab(data) {
    console.log("populateCompliance", data);
    const $tbody = $('#compliance-body');
    $tbody.empty();

    data.forEach((item, i) => {
        const missingMap = {
            PFNo: item.isMissing_PFNo,
            PFAmount: item.isMissing_PFAmount,
            PFPercent: item.isMissing_PFPercent,
            VPFValue: item.isMissing_VPFValue,
            VPFPercent: item.isMissing_VPFPercent,
            UANNo: item.isMissing_UANNo,
            ESICNo: item.isMissing_ESICNo
            //PTAmount: item.isMissing_PTAmount
        };

        const isAnyMissing = Object.values(missingMap).some(x => x === true);

        function renderEditableCell(value, isMissing, field, rowIndex) {
            if (isMissing) {
                return `
                    <div class="editable-cell bg-light-danger" data-col="${field}" data-row="${rowIndex}" data-entity-id="${item.entity_ID}">
                        <span class="cell-value" data-selected-id="${value ?? ''}">${value ?? 'Null'}</span>
                        <i class="fas fa-pen edit-icon ms-2" title="Edit ${field}"></i>
                    </div>`;
            } else {
                return value ?? '-';
            }
        }

        const rowHtml = `
           <tr data-entity-id="${item.entity_ID}" data-missing='${JSON.stringify(missingMap)}'>
                <td class="sticky_cell checkbox">
                    <div class="input-group">
                        <input type="checkbox"
                               class="form-check-input mt-0 compliance-checkbox"
                               data-entity-id="${item.entity_ID}"
                               ${isAnyMissing ? 'disabled' : ''} />
                    </div>
                </td>
                <td>${item.entityCode || ''}</td>
                <td>${item.entityName || ''}</td>
                <td>${renderEditableCell(item.pfNo, item.isMissing_PFNo, 'PFNo', i)}</td>
                <td>${renderEditableCell(item.uanNo, item.isMissing_UANNo, 'UANNo', i)}</td>
                <td>${renderEditableCell(item.esicNo, item.isMissing_ESICNo, 'ESICNo', i)}</td>
                <td>${item.policyNo || ''}</td>                
                <td>${renderEditableCell(item.pfAmount, item.isMissing_PFAmount, 'PFAmount', i)}</td>
                <td>${renderEditableCell(item.pfPercent, item.isMissing_PFPercent, 'PFPercent', i)}</td>
                <td>${renderEditableCell(item.vpfValue, item.isMissing_VPFValue, 'VPFValue', i)}</td>
                <td>${renderEditableCell(item.vpfPercent, item.isMissing_VPFPercent, 'VPFPercent', i)}</td>               
                <td>${item.ptAmount ?? ''}</td>                
                <td>${item.policyAmount ?? ''}</td>
            </tr>
        `;
        $tbody.append(rowHtml);
    });
}

// Show input on edit icon click
$(document).on('click', '#validatecompliance-list .edit-icon', function () {
    const $cell = $(this).closest('.editable-cell');
    const col = $cell.data('col');
    const rowIndex = $cell.data('row');
    const entityId = $cell.data('entity-id');
    const currentValue = $cell.find('.cell-value').data('selected-id') ?? '';

    const inputId = `${col}_${rowIndex}`;
    const inputHtml = `
    <input type="text" id="${inputId}" class="form-control form-control-sm compliance-input"
           data-col="${col}" data-row="${rowIndex}" data-entity-id="${entityId}" value="${currentValue}" />
        `;

    $cell.html(inputHtml);
});

const decimalFields = ['PFAmount', 'PFPercent', 'VPFValue', 'VPFPercent', 'PTAmount'];
const digitOnlyFields = ['UANNo'];

function isValidPFNo(val) {
    const regex = /^[A-Z]{2}[A-Z]{3}[0-9]{7}[0-9]{0,3}[0-9]+$/;
    return regex.test(val) && val.length >= 12 && val.length <= 22;
}

function isValidUAN(val) {
    return /^[1-9][0-9]{11}$/.test(val);
}

function isValidESIC(val) {
    return /^\d{2}-\d{7}-\d{1}$/.test(val);
}

$(document).on('input', '.compliance-input', function () {
    const $input = $(this);
    const field = $input.data('col');
    let val = $input.val()?.trim();

    // Apply field-specific formatting
    if (decimalFields.includes(field)) {
        val = val.replace(/[^0-9.]/g, '');
        const parts = val.split('.');
        if (parts.length > 2) val = parts[0] + '.' + parts[1];
        if (parts.length === 2) val = parts[0] + '.' + parts[1].substring(0, 2);
        $input.val(val);
    } else if (field === 'UANNo') {
        val = val.replace(/\D/g, '').substring(0, 12);
        $input.val(val);
    } else if (field === 'ESICNo') {
        // Allow only digits
        val = val.replace(/\D/g, '').substring(0, 10);

        // Format: XX-XXXXXXX-X
        let formatted = '';
        if (val.length > 0) {
            formatted += val.substring(0, 2);
        }
        if (val.length >= 3) {
            formatted += '-' + val.substring(2, 9);
        }
        if (val.length === 10) {
            formatted += '-' + val.substring(9);
        }

        $input.val(formatted);
    }

    // Validation check for the entire row
    const rowIndex = $input.data('row');
    const $row = $('#compliance-body tr').eq(rowIndex);
    const $inputs = $row.find('.compliance-input');

    let allValid = true;

    $inputs.each(function () {
        const $fieldInput = $(this);
        const fieldName = $fieldInput.data('col');
        const value = $fieldInput.val()?.trim();

        if (!value) {
            allValid = false;
            return false;
        }

        // Field-specific validations
        if (fieldName === 'PFNo' && !isValidPFNo(value)) {
            allValid = false;
            return false;
        }
        if (fieldName === 'UANNo' && !isValidUAN(value)) {
            allValid = false;
            return false;
        }
        if (fieldName === 'ESICNo' && !isValidESIC(value)) {
            allValid = false;
            return false;
        }
        if (decimalFields.includes(fieldName) && isNaN(value)) {
            allValid = false;
            return false;
        }
    });

    // Enable checkbox only if all editable inputs in the row are valid
    $row.find('.compliance-checkbox').prop('disabled', !allValid);
});



// Auto-format decimal fields on blur (not for UAN/ESIC)
$(document).on('blur', '.compliance-input', function () {
    const field = $(this).data('col');
    var val = $(this).val();
    if (decimalFields.includes(field) && val && !isNaN(val)) {
        $(this).val(parseFloat(val).toFixed(2));
    }
});
function fetchComplianceData(entityIds) {
    if (!Array.isArray(entityIds) || entityIds.length === 0) {
        alert("No entities provided for pay compliance.");
        return;
    }

    $.ajax({
        url: '/ContractorValidation/SelectedEntitiesForComplianceValidation',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(entityIds),
        success: function (res) {
            if (res.success && res.data && res.data.length > 0) {
                populateComplianceTab(res.data);
                if ($.fn.DataTable.isDataTable('#validatecompliance-list')) {
                    $('#validatecompliance-list').DataTable().destroy();
                }
                makeDataTable("validatecompliance-list");
                activateTab('compliance-tab', 'compliance-tab-pane');

            } else {
                alert(res.message || "No compliance data found.");
            }
        },
        error: function () {
            alert("Error fetching pay validation data.");
        }
    });
}

$(document).on('click', '#btnfourthtab', function () {
    var selectedComplianceEntities = [];

    $('#validatecompliance-list tbody tr').each(function () {
        var $row = $(this);
        var $checkbox = $row.find('.compliance-checkbox');

        if ($checkbox.prop('checked')) {
            var entityId = $checkbox.data('entity-id');
            if (!entityId) return;

            var updatedData = {
                Entity_ID: entityId
            };

            // Loop over expected compliance fields
            ['PFNo', 'PFAmount', 'PFPercent', 'VPFValue', 'VPFPercent', 'UANNo', 'ESICNo'].forEach(function (field) {
                var $cell = $row.find(`.editable-cell[data-col="${field}"]`);
                if ($cell.length > 0) {
                    var $input = $cell.find('input');
                    if ($input.length > 0) {
                        var value = $input.val();
                        if (value !== '') {
                            // If the value should be numeric, convert it
                            if (['PFAmount', 'PFPercent', 'VPFValue', 'VPFPercent'].includes(field)) {
                                updatedData[field] = parseFloat(value);
                            } else {
                                updatedData[field] = value;
                            }
                        }
                    } else {
                        // fallback to span
                        var $span = $cell.find('.cell-value');
                        updatedData[field] = $span.text().trim();
                    }
                }
            });
            selectedComplianceEntities.push(updatedData);
        }
    });

    // Validation
    if (selectedComplianceEntities.length === 0) {
        alert("Please select at least one entity with valid compliance data.");
        return;
    }

    // Final payload
    var compliancePayload = {
        EntityComplianceUpdateList: selectedComplianceEntities
    };
    // Submit to controller
    $.ajax({
        url: '/ContractorValidation/SubmitSelectedCompliance',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(compliancePayload),
        success: function (response) {
            if (response.success) {
                //alert("Compliance data saved successfully.");   
                const entityIds = selectedComplianceEntities.map(e => e.Entity_ID);
                if (entityIds.length > 0) {
                    fetchAttendanceData(entityIds);
                    //if ($.fn.DataTable.isDataTable('validateattendance-list')) {
                    //    $('validateattendance-list').DataTable().destroy();
                    //}
                    //makeDataTable("validateattendance-list");
                    activateTab('attendance-tab', 'attendance-tab-pane');
                }
            } else {
                alert(response.message || "Failed to save compliance data.");
            }
        },
        error: function () {
            alert("An error occurred while saving compliance data.");
        }
    });
});

///////////////////////////////////////////////////// Compliance TAB:- End////////////////////////////////////////


///////////////////////////////////////////////////// Attendance TAB:- Start////////////////////////////////////////
function fetchAttendanceData(entityIds) {
    if (!Array.isArray(entityIds) || entityIds.length === 0) {
        alert("No entities provided for attendance.");
        return;
    }

    $.ajax({
        url: '/ContractorValidation/SelectedEntitiesForAttendanceValidation',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(entityIds),
        success: function (res) {
            if (res.success && res.data && res.data.length > 0) {
                populateAttendanceTab(res.data);
                if ($.fn.DataTable.isDataTable('validateattendance-list')) {
                    $('validateattendance-list').DataTable().destroy();
                }
                makeDataTable("validateattendance-list");
                $('#compliance-tab-section').addClass('d-none');
                $('#attendance-tab-section').removeClass('d-none');
                $('#attendance-tab').addClass('active');
                $('.nav-link').not('#attendance-tab').removeClass('active');
            } else {
                alert(res.message || "No attendance data found.");
            }
        },
        error: function () {
            alert("Error fetching attendance data.");
        }
    });
}

function populateAttendanceTab(data) {
    const $tbody = $('#attendance-body');
    $tbody.empty();
    //console.log("Attendance", data);

    data.forEach(item => {
        const isMissing = item.datasequence === 1;
        const rowClass = isMissing ? 'bg-light-red' : ''; // Add custom class for styling

        const rowHtml = `
            <tr class="${rowClass}">               
                <td>${item.entity_Name || '-'}</td>
                <td>${item.contractor_Name || '-'}</td>
                <td>${item.payMonthPeriod_StartDt ? formatDate(item.payMonthPeriod_StartDt) : '-'}</td>
                <td>${item.payMonthPeriod_EndDt ? formatDate(item.payMonthPeriod_EndDt) : '-'}</td>
                <td>${item.total_Days ?? '-'}</td>
                <td>${item.present_Days ?? '-'}</td>
                <td>${item.absent_Days ?? '-'}</td>
                <td>${item.week_Off_Days ?? '-'}</td>
                <td>${item.holiday_Days ?? '-'}</td>
                <td>${item.leave_Days ?? '-'}</td>
                <td>${item.comp_Off_Days ?? '-'}</td>
                <td>${item.work_On_Holiday_Days ?? '-'}</td>
                <td>${item.work_on_Week_Of ?? '-'}</td>
                <td>${item.total_OT_Hours ?? '-'}</td>
                <td>${item.ot_Shift_Code || '-'}</td>
                <td>${item.isActive ? 'Active' : 'Inactive'}</td>
                <td class="d-none contractor-id-td">${item.contractor_Id}</td>
                <td class="d-none entity-id-td">${item.entity_ID}</td>
                <td class="d-none datasequence-td">${item.datasequence}</td> <!-- Hidden field -->
            </tr>
        `;
        $tbody.append(rowHtml);
    });
}

$(document).on('click', '#btnfifthtab', function () {
    var attendanceSaveList = [];

    $('#validateattendance-list tbody tr').each(function () {
        var $row = $(this);
        var contractorId = parseInt($row.find('.contractor-id-td').text());
        var entityId = parseInt($row.find('.entity-id-td').text());
        var datasequence = parseInt($row.find('.datasequence-td').text());
        if (datasequence === 2 && !isNaN(contractorId) && !isNaN(entityId)) {
            attendanceSaveList.push({
                ContractorId: contractorId,
                EntityId: entityId
            });
        }
    });

    if (attendanceSaveList.length === 0) {
        alert("No attendance records found to process.");
        return;
    }

    // 🔁 First API call: SubmitSelectedAttendance (if still needed)
    $.ajax({
        url: '/ContractorValidation/SubmitSelectedAttendance',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            CompanyId: 0, // will be set server-side
            EntityAttendanceIds: attendanceSaveList.map(x => x.EntityId) // If this API still expects only EntityIds
        }),
        success: function (response) {
            if (response.success) {
                //alert("Attendance data submitted successfully.");

                // 🔁 Second API call: SaveAttendanceValidation
                $.ajax({
                    url: '/ContractorValidation/SaveAttendanceValidation',
                    type: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(attendanceSaveList),
                    success: function (saveResponse) {
                        if (saveResponse.success) {
                            showAlert('success', saveResponse.message);                            
                        } else {
                            showAlert('danger', saveResponse.message);                           
                        }
                    },
                    error: function () {
                        alert("Error while saving attendance.");
                    }
                });

            } else {
                alert(response.message || "Failed to submit attendance.");
            }
        },
        error: function () {
            alert("Error while submitting attendance.");
        }
    });
});

///////////////////////////////////////////////////// Attendance TAB:- End////////////////////////////////////////

////////////////////////////////////////////////////MODEL POPUP IN FIRST SCREEN FETCHING DROPDOWN VALUE:-START///////////////////////////////////////
function loadContractorWithFilterValidationData() {
    $.ajax({
        url: '/ContractorValidation/GetContractorValidationData',
        type: 'GET',
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                var rows = "";
                $.each(response.data, function (i, row) {
                    const isMissing = row.isMissing_BusinessAreaCode ||
                        row.isMissing_CompanyCode ||
                        row.isMissing_ContactNo ||
                        row.isMissing_LicenseNo ||
                        row.isMissing_MaxLabourCount ||
                        row.isMissing_PANNo ||
                        row.isMissing_TANNo ||
                        row.isMissing_WorkOrderNo ||
                        row.isMissing_WorkOrderValue;

                    const checkboxDisabledAttr = isMissing ? 'disabled' : '';
                    const statusText = row.isActive ? 'Active' : 'Inactive';
                    rows +=
                        `<tr>
                        <td class="sticky_cell checkbox">
                            <div class="input-group">
                                <input type="checkbox" class="form-check-input mt-0 row-checkbox" 
                                    data-row-index="${i}" data-contractor-id="${row.contractor_Id}" ${checkboxDisabledAttr} />
                            </div>
                        </td>                    
                        <td>${row.contractorName ?? ''}</td>
                           <td>${row.subContractorName ?? ''}</td>
                       <td>
                            <div class="chip ${row.isActive ? 'success' : 'danger'}">
                                ${row.isActive ? 'Active' : 'Inactive'}
                            </div>
                        </td>
                      <td class="p-0">
                        ${row.isMissing_ContactNo
                            ? `<div class="editable-cell">
                                    <div class="d-flex align-items-center justify-content-between">
                                        <p>${row.contactNo ?? 'Null'}</p>
                                        <button class="btn">
                                            <span class="sprite-icons edit-secondary-lg"></span>
                                        </button>
                                    </div>
                               </div>`
                            : row.contactNo ?? ''}
                    </td>

                       <td class="p-0">
                            ${row.isMissing_LicenseNo
                            ? `<div class="editable-cell">
                                        <div class="d-flex align-items-center justify-content-between">
                                            <p>${row.licenseNo ?? 'Null'}</p>
                                            <button class="btn">
                                                <span class="sprite-icons edit-secondary-lg"></span>
                                            </button>
                                        </div>
                                   </div>`
                            : row.licenseNo ?? ''}
                        </td>

                      <td class="p-0">
                        ${row.isMissing_MaxLabourCount
                            ? `<div class="editable-cell">
                                    <div class="d-flex align-items-center justify-content-between">
                                        <p>${row.maxLabourCount ?? 'Null'}</p>
                                        <button class="btn">
                                            <span class="sprite-icons edit-secondary-lg"></span>
                                        </button>
                                    </div>
                               </div>`
                            : row.maxLabourCount ?? ''}
                    </td>

                       <td>${row.assigned_labour_Count ?? ''}</td>
                      <td class="p-0">
                        ${row.isMissing_PANNo
                            ? `<div class="editable-cell">
                                    <div class="d-flex align-items-center justify-content-between">
                                        <p>${row.panNo ?? 'Null'}</p>
                                        <button class="btn">
                                            <span class="sprite-icons edit-secondary-lg"></span>
                                        </button>
                                    </div>
                               </div>`
                            : row.panNo ?? ''}
                        </td>

                       <td class="p-0">
                            ${row.isMissing_TANNo
                            ? `<div class="editable-cell">
                                        <div class="d-flex align-items-center justify-content-between">
                                            <p>${row.tanNo ?? 'Null'}</p>
                                            <button class="btn">
                                                <span class="sprite-icons edit-secondary-lg"></span>
                                            </button>
                                        </div>
                                   </div>`
                            : row.tanNo ?? ''}
                        </td>


                        <td>${row.email_Id ?? ''}</td>
                        <td>${row.address ?? ''}</td>
                       
                        <td>${row.liN_No ?? ''}</td>
                        <td>${row.epF_No ?? ''}</td>
                        <td>${row.esiC_No ?? ''}</td>   
                        <td>${row.workOrderNo ?? ''}</td> 
                        <td>
                            ${row.hasWorkOrderValue ?? ''}
                        </td>
                    </tr>`;
                });
                $("#contractor-body").html(rows);
                if ($.fn.DataTable.isDataTable('validatecontractors-list')) {
                    $('validatecontractors-list').DataTable().destroy();
                }
              
                makeDataTable("validatecontractors-list");
            } else {
                $("#contractor-body").html(`<tr><td colspan="20" class="text-danger">${response.message}</td></tr>`);
            }
        },
        error: function () {
            $("#contractor-body").html(`<tr><td colspan="20" class="text-danger">Error loading data.</td></tr>`);
        }
    });
}
/*$("#btnfirstopenmodelpopup").on("click", function () {*/
$(document).ready(function () {
    // Intercept the button click and show the correct modal
    $(document).on('click', '#btn2', function (e) {
        e.preventDefault(); // prevent default modal trigger
        fetchValidationContractorDropdownData(
            '/DropDown/FetchCompaniesDropdown',
            {},
            '#contractorcompany',
            'Select company'
        ).done(function () {
            const selectedCompanyId = $('#contractorcompany').val();
            //if (selectedCompanyId) {
            //    fetchContractors(selectedCompanyId).done(function () {
            //        fetchLocations(selectedCompanyId);
            //    });
            //}
            if (selectedCompanyId) {
                fetchPreviousMonthYear(selectedCompanyId).done(function () {
                    fetchContractors(selectedCompanyId).done(function () {
                        fetchLocations(selectedCompanyId);
                    });
                });
            }
            const filterModal = new bootstrap.Modal(document.getElementById('filterModal'));
            filterModal.show();
        });
        //$('#filterModal').modal('show'); // manually trigger the correct modal
    });
});

$("#btnfirstopenmodelpopup").on("click", function () {
  
    // Load companies
    fetchValidationContractorDropdownData(
        '/DropDown/FetchCompaniesDropdown',
        {},
        '#contractorcompany',
        'Select company'
    ).done(function () {
        const selectedCompanyId = $('#contractorcompany').val();        
        if (selectedCompanyId) {
            fetchPreviousMonthYear(selectedCompanyId).done(function () {
                fetchContractors(selectedCompanyId).done(function () {
                    fetchLocations(selectedCompanyId);
                });
            });
        }
        const filterModal = new bootstrap.Modal(document.getElementById('filterModal'));
        filterModal.show();
    });
});
$('#filterModal').on('hidden.bs.modal', function () {
    $('#btn2').focus();
});

$('#resetFilters').on('click', function () {
    $('#filterModal select').val('');
});
$('#applyFilters').on('click', function () {
    const companyId = $('#contractorcompany').val();
    const locationIds = $('#locationName').val() || [];
    const workOrderIds = $('#contractorName').val() || [];
    const monthId = parseInt($('#hiddenMonthId').val()) || null;
    const year = parseInt($('#year').val()) || null;
    const filterModal = bootstrap.Modal.getInstance(document.getElementById('filterModal'));

    const requestModel = {
        companyId: parseInt(companyId),
        locationIds: locationIds.map(id => parseInt(id)),
        workOrderIds: workOrderIds.map(id => parseInt(id)),
        month_Id: monthId,
        year: year
    };
    $.ajax({
        url: '/ContractorValidation/ContractorFilter',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(requestModel),
        success: function (response) {

            if (response.success) {
                if (filterModal) {
                    filterModal.hide();
                }

                // ✅ Destroy old DataTable if needed
                if ($.fn.DataTable.isDataTable('#validatecontractors-list')) {
                    $('#validatecontractors-list').DataTable().clear().destroy();
                }

                // ✅ Bind new filtered data
                buildContractorRows(response.data);
                if ($.fn.DataTable.isDataTable('validatecontractors-list')) {
                    $('validatecontractors-list').DataTable().destroy();
                }
                makeDataTable("validatecontractors-list");
            } else {
                filterModal.hide();
                ['#validatecontractors-list', '#validateentity-list', '#validatepay-list', '#validatecompliance-list', '#validateattendance-list']
                    .forEach(function (tableId) {
                        if ($.fn.DataTable.isDataTable(tableId)) {
                            $(tableId).DataTable().clear().draw();
                        } else {
                            $(tableId + ' tbody').empty(); // fallback if DataTable is not initialized
                        }
                    });
                showAlert('danger', response.message);      
            }
        },
        error: function (xhr) {
            console.error("Filter application failed:", xhr.responseText);
        }
    });
});
function buildContractorRows(data) {
    var rows = "";

    $.each(data, function (i, row) {

        const isMissing = row.isMissing_BusinessAreaCode ||
            row.isMissing_CompanyCode ||
            row.isMissing_ContactNo ||
            row.isMissing_LicenseNo ||
            row.isMissing_MaxLabourCount ||
            row.isMissing_PANNo ||
            row.isMissing_TANNo ||
            row.isMissing_WorkOrderNo ||
            row.isMissing_WorkOrderValue;

        const checkboxDisabledAttr = isMissing ? 'disabled' : '';
        const statusText = row.isActive ? 'Active' : 'Inactive';

        rows += `<tr>
                        <td class="sticky_cell checkbox">
                            <div class="input-group">
                                <input type="checkbox" class="form-check-input mt-0 row-checkbox" 
                                    data-row-index="${i}" data-contractor-id="${row.contractor_Id}" ${checkboxDisabledAttr} />
                            </div>
                        </td>                    
                        <td>${row.contractorName ?? ''}</td>
                           <td>${row.subContractorName ?? ''}</td>
                       <td>
                            <div class="chip ${row.isActive ? 'success' : 'danger'}">
                                ${row.isActive ? 'Active' : 'Inactive'}
                            </div>
                        </td>
                          <td>
                            ${row.isMissing_ContactNo
                ? `<div class="editable-cell bg-light-danger" data-col="ContactNo" data-row="${i}">
                                    <span class="cell-value">${row.contactNo ?? 'Null'}</span>
                                   </div>`
                : row.contactNo ?? ''}
                        </td>
                        <td>
                            ${row.isMissing_LicenseNo
                ? `<div class="editable-cell bg-light-danger" data-col="LicenseNo" data-row="${i}">
                                    <span class="cell-value">${row.licenseNo ?? 'Null'}</span>
                                   </div>`
                : row.licenseNo ?? ''}
                        </td>
                        <td>
                            ${row.isMissing_MaxLabourCount
                ? `<div class="editable-cell bg-light-danger" data-col="MaxLabourCount" data-row="${i}">
                                    <span class="cell-value">${row.maxLabourCount ?? 'Null'}</span>
                                   </div>`
                : row.maxLabourCount ?? ''}
                        </td>
                         <td>${row.assigned_labour_Count ?? ''}</td>
                        <td>
                            ${row.isMissing_PANNo
                ? `<div class="editable-cell bg-light-danger" data-col="PANNo" data-row="${i}">
                                    <span class="cell-value">${row.panNo ?? 'Null'}</span>
                                   </div>`
                : row.panNo ?? ''}
                        </td>

                        <td>
                            ${row.isMissing_TANNo
                ? `<div class="editable-cell bg-light-danger" data-col="TANNo" data-row="${i}">
                                    <span class="cell-value">${row.tanNo ?? 'Null'}</span>
                                   </div>`
                : row.tanNo ?? ''}
                        </td>
                        <td>${row.email_Id ?? ''}</td>
                        <td>${row.address ?? ''}</td>
                       
                        <td>${row.liN_No ?? ''}</td>
                        <td>${row.epF_No ?? ''}</td>
                        <td>${row.esiC_No ?? ''}</td>   
                        <td>${row.workOrderNo ?? ''}</td> 
                        <td>
                            ${row.hasWorkOrderValue ?? ''}
                        </td>
                    </tr>`;
    });

    $("#contractor-body").html(rows);
}
function fetchValidationContractorDropdownData(url, data, dropdownId, placeholderText) {
    const deferred = $.Deferred();
    $.ajax({
        url: url,
        type: 'GET',
        data: data,
        async: false,
        success: function (response) {
            populateValidationContractorDropdown(dropdownId, response, placeholderText);
            deferred.resolve();
        },
        error: function () {
            console.error(`Failed to fetch data for ${dropdownId}`);
            deferred.reject();
        }
    });
    return deferred.promise();
}
function populateValidationContractorDropdown(dropdownId, items, placeholderText) {
    const dropdown = $(dropdownId);
    dropdown.empty().append(new Option(placeholderText, ''));

    const sessionCompanyId = parseInt($("#hiddenSessionCompanyId").val());

    items.forEach(item => {
        const option = new Option(item.text, item.value);
        if (parseInt(item.value) === sessionCompanyId) {
            option.selected = true;
        }
        dropdown.append(option);
    });
}
function fetchContractors(companyId) {
    const deferred = $.Deferred();

    $.ajax({
        url: `/DropDown/FetchContractorWorkOrderDropdown?company_ID=${companyId}`,
        type: 'GET',
        success: function (data) {
            populateDropdownData('#contractorName', data, 'value', 'text');
            deferred.resolve(); // resolve the promise
        },
        error: function () {
            console.error("Error fetching contractors");
            deferred.reject(); // reject the promise
        }
    });

    return deferred.promise(); // this is important
}
function fetchLocations(companyId) {
    $.ajax({
        url: `/DropDown/FetchDistinctLocationDropdown?companyId=${companyId}`,
        type: 'GET',
        success: function (data) {
            populateDropdownData('#locationName', data, 'value', 'text');
        },
        error: function () {
            console.error("Error fetching locations");
        }
    });
}
function fetchPreviousMonthYear(companyId) {
    return $.ajax({
        url: '/ContractorValidation/GetPreviousMonthYearPeriod',
        type: 'GET',
        data: { companyId: companyId }
    }).done(function (response) {
        //console.log(response);
        if (response.isSuccess && response.data) {
            $('#hiddenMonthId').val(response.data.month_Id);
            $('#year').val(response.data.year);
            $('#monthName').val(response.data.monthName);
        } else {
            alert(response.message || "Unable to fetch pay period.");
        }
    }).fail(function () {
        alert("Error fetching previous month/year.");
    });
}
function bindSessionPreviousMonthYear() {
    return $.ajax({
        url: '/ContractorValidation/BindPreviousMonthYearAtPageLoad',
        type: 'GET'       
    }).done(function (response) {
      
        if (response.isSuccess && response.data) {
            $('#hiddenMonthId').val(response.data.month_Id);
            $('#year').val(response.data.year);
            $('#monthName').val(response.data.monthName);
        } else {
            alert(response.message || "Unable to fetch pay period.");
        }
    }).fail(function () {
        alert("Error fetching previous month/year.");
    });
}
function populateDropdownData(selector, data, valueField, textField, selectedValue = "") {
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

    //dropdown.select2('destroy').select2({
    //    placeholder: dropdown.data('placeholder') || 'Select',
    //    allowClear: true,
    //    width: '100%',
    //    dropdownAutoWidth: true
    //});

    if (selectedValue) {
        dropdown.val(selectedValue).trigger('change.select2');
    }
}
////////////////////////////////////////////////////MODEL POPUP IN FIRST SCREEN FETCHING DROPDOWN VALUE:-End///////////////////////////////////////
