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
    const tableId = "grade-entity-mapping-list";

    if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
        $(`#${tableId}`).DataTable().clear().destroy();
    }
    makeDataTable(tableId);
    // Optionally replace with empty message (like in AJAX success)
    $(`#${tableId} tbody`).html(
        '<tr><td colspan="6" class="text-center">Select a filters to display the grid data.</td></tr>'
    );


    // Load partial view and setup Select2
    $.ajax({
        url: '/EntityMaster/MapEntityGradeResponse',
        type: 'GET',
        success: function (html) {
            $('#mapGradeContainer').html(html);
            $('#standardFilterModal').modal('show');
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
    function resetAllFilters() {
        $('#mapCompanyDropdown').val(sessionCompanyId).trigger('change');
        $('#mapContractorDropdown').val('').trigger('change');
        $('#mapLocationDropdown').val('').trigger('change');
        $('#mapTradeDropdown').val('').trigger('change');
        $('#mapSkillDropdown').val('').trigger('change');
        $('#mapGradeDropdown').val('').trigger('change');
        $(".input_error_msg").text('');
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
                console.log(data);
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

    // Disable assign button on page load
    $(document).ready(function () {
        $('#mapgradeentity').prop('disabled', true);
    });

    // When any checkbox inside table body is changed
    $(document).on('change', '#grade-entity-mapping-list tbody input[type="checkbox"]', function () {
        // Check if any checkbox is checked
        const anyChecked = $('#grade-entity-mapping-list tbody input[type="checkbox"]:checked').length > 0;

        // Enable or disable the assign button based on checkbox state
        $('#mapgradeentity').prop('disabled', !anyChecked);
    });


    var selectedOption = null;

    $(document).ready(function () {
        // Disable button on page load
        $('#mapgradeentity').prop('disabled', true);
    });

    // Apply Selection Logic
    $(document).on('click', '#applySelection', function () {
        const table = $('#grade-entity-mapping-list').DataTable();
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
        const table = $('#grade-entity-mapping-list').DataTable();
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
    $('#grade-entity-mapping-list').on('draw.dt', function () {
        const table = $('#grade-entity-mapping-list').DataTable();

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
        $('#mapgradeentity').prop('disabled', !anyChecked);
        $('#deselectAllBtn').prop('disabled', !anyChecked);
    }

    // Deselect all checkboxes on button click
    $(document).on('click', '#deselectAllBtn', function () {
        deselectedRowandButton();
    });
    function deselectedRowandButton() {
        const table = $('#grade-entity-mapping-list').DataTable();

        // Uncheck all checkboxes inside the table rows
        table.rows().every(function () {
            $(this.node()).find('.row-checkbox').prop('checked', false);
        });

        // Clear selectedOption so it doesn't reapply on redraw
        selectedOption = null;

        // Disable Assign and Deselect All buttons
        $('#mapgradeentity').prop('disabled', true);
        $('#deselectAllBtn').prop('disabled', true);
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
        const tableId = "grade-entity-mapping-list";

        if ($.fn.DataTable.isDataTable(`#${tableId}`)) {
            $(`#${tableId}`).DataTable().clear().destroy();
        }
        makeDataTable(tableId);
        // Optionally replace with empty message (like in AJAX success)
        $(`#${tableId} tbody`).html(
            '<tr><td colspan="6" class="text-center">Select a filters to display the grid data.</td></tr>'
        );
    });
    
    $(document).on("click", '#applyEntityFilterBtn', () => loadMapEntityData(1));

    $(document).on("change", '#pageSizeDropdown', () => loadMapEntityData(1));

    function loadMapEntityData(pageNumber = 1) {
        if (!validateFilterForm()) return;

        const tableId = "grade-entity-mapping-list";
        const pageSize = parseInt($('#pageSizeDropdown').val()) || 10;

        const filterData = {
            SelectType: 'ET', // 👈 Required filter type
            CompanyId: $('#mapCompanyDropdown').val(),
            CorrespondanceId: $('#mapLocationDropdown').val() || 0,
            ContractorId: $('#mapContractorDropdown').val(),
            EntityId: 0, // ID remains value
            EntityCode: "", // ✅ Get selected text
            EntityName: "", // ✅ Get selected text
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
            success: function (response) {
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
               
            },
            error() {
                showAlert('danger', "No record found.");
            }
        });
    }

    $(document).on("click", ".page-btn", function () {
        const page = parseInt($(this).data('page'));
        loadMapEntityData(page);
    });

    //$(document).on("click", '#filterClosebtn', function () {
    //    console.log("Cancel");
    //    var modalEl = document.getElementById('standardFilterModal');
    //    var modal = bootstrap.Modal.getInstance(modalEl); // or new bootstrap.Modal(modalEl)
    //    modal.hide();
    //});
    $(document).on("click", '#mapgradeentity', function () {
        // Call to bind dropdown on page load
        BindmapgradeentityDropdown();

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
    // Change event to remove error message
    //$(document).on('change', '#dropdownTextbox', function () {
    //    // Remove error message
    //    $('#dropdownTextbox-error').text('');

    //    // Update the value and trigger select2 change
    //    $('#dropdownTextbox').val($(this).val()).trigger('change.select2');
    //});
    $(document).on("click", "#bulkInsertMapGradeEntityBtn", function () {
        const selectedRows = [];
        const selectedPayGradeId = $('#dropdownTextbox').attr('data-selected-value');

        if (!selectedPayGradeId) {
            $('#dropdownTextbox-error').text('Please select a Grade from the dropdown.');
            return false;
        }
        return true;
        $('#grade-entity-mapping-list tbody tr').each(function () {
            const $row = $(this);
            const isChecked = $row.find('input[type="checkbox"]').is(':checked');
            if (isChecked) {
                const entityID = $row.find('td').eq(2).text().trim(); // Not required, just showing reference

                selectedRows.push({
                    Entity_ID: parseInt(entityID), // You must supply correct data here
                    Pay_Grade_ID: selectedPayGradeId,
                });
            }
        });

        if (selectedRows.length === 0) {
            showAlert("warning", "Please select at least one row.");
            return;
        }
        const requestData = {
            MapEntityGrade: selectedRows // ✅ Must match DTO property name
        };
        $.ajax({
            url: '/EntityMaster/BulkAssignMapGrdaeEntity', // update path as needed
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(requestData),
            success: function (response) {
                if (response.success) {
                    $('#mapgradeentitymodel').modal('hide');
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
});
function BindmapgradeentityDropdown() {
    $.ajax({
        url: '/DropDown/FetchActivePayGradeTypeDropdown',
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

            //// Append static option
            //$dropdownMenu.append(`
            //        <li>
            //            <a class="dropdown-item" data-value="99">Same As Previous Year</a>
            //        </li>
            //    `);
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
                <td>${item.contractor_Name ?? ''}</td>
                <td>${item.entity_Code ?? ''}</td>
                <td>${item.entity_Name ?? ''}</td>
                <td>${item.locationName ?? ''}</td>
                <td>${item.trade_Name ?? ''}</td>
                <td>${item.skillcategory_Name ?? ''}</td>
                <td>${item.payGradeName ?? ''}</td>
            </tr>
        `;
    });
    $('#grade-entity-mapping-list tbody').html(tbody);
}
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString(); // Customize if needed
}
