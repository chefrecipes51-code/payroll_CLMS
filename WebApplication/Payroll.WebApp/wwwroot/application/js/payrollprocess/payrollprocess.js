$(document).ready(function () {
    const sessionCompanyId = $("#sessionCompanyId").val();
    // Bind monthyear dropdown on page load (if required)
    BindMonthYearDropdown(sessionCompanyId);
    // On month change
    $('#monthyear').on('change', function () {
        if ($(this).val()) {
            $('#monthyear-error').text('');
        }
        var monthYear = $(this).text().trim();
        //console.log(monthYear);

        //let companyId = $(this).val();
        if (monthYear) {
            // Fetch Month-Year based on selected company
            BindCompanyDropdown(monthYear);
        }
    });
    // On company change
    $('#Companies').on('change', function () {
        if ($(this).val()) {
            $('#Companies-error').text('');
        }
        var monthYr = $('#monthyear').text().trim();

        var companyId = $('#Companies').val();

        if (monthYr && companyId) {
            // Populate Location, Contractor, Work Order
            BindLocationDropdown(monthYr, companyId);
            BindContractorDropdown(companyId, monthYr);
            BindWorkOrderDropdown(monthYr, companyId);
        }
    });
    // Clear error when at least one location is selected
    $('#location').on('change', function () {
        const val = $(this).val();
        if (val && val.length > 0) {
            $('#location-error').text('');
        }
    });
    // Clear error when at least one contractor is selected
    $('#contractor').on('change', function () {
        const val = $(this).val();
        if (val && val.length > 0) {
            $('#contractor-error').text('');
        }
    });
    // Clear error when at least one workorder is selected
    $('#workorder').on('change', function () {
        const val = $(this).val();
        if (val && val.length > 0) {
            $('#workorder-error').text('');
        }
    });
    function BindCompanyDropdown(month_Yr) {
        //const month_Yr = "06-2025";
        $.ajax({
            url: `/DropDown/FetchCompanyPayrollValidationDropdown?month_Yr=${month_Yr}`,
            type: 'GET',
            //type: 'GET',
            ////url: '/DropDown/FetchCompanyPayrollValidationDropdown',
            //data: { month_Yr },
            success: function (response) {
                var $dropdown = $('#Companies');
                $dropdown.empty().append('<option></option>');
                $.each(response, function (i, item) {
                    $dropdown.append($('<option>', {
                        value: item.value,
                        text: item.text
                    }));
                });
            }
        });
    }
    function BindMonthYearDropdown(companyId) {
        $.ajax({
            type: 'GET',
            url: '/DropDown/FetchPreviousMonthYearPeriodByCmpIdDropdown',
            data: { company_ID: companyId },
            success: function (response) {
                var $dropdown = $('#monthyear');
                $dropdown.empty().append('<option></option>');
                $.each(response, function (i, item) {
                    $dropdown.append($('<option>', {
                        value: item.value,
                        text: item.value + '-' + item.text
                    }));    
                });
            }
        });
    }
    function BindLocationDropdown(monthYr, companyId) {
        $.ajax({
            type: 'GET',
            url: '/DropDown/FetchCompanyLocationPayrollValidationDropdown',
            data: { month_Yr: monthYr, company_ID: companyId, isActive: true },
            success: function (response) {
                var $dropdown = $('#location');
                if ($dropdown.hasClass("select2-hidden-accessible")) {
                    $dropdown.select2("destroy");
                }
                $dropdown.empty();

                var selectedValues = [];
                $.each(response, function (i, item) {
                    $dropdown.append($('<option>', {
                        value: item.value,
                        text: item.text
                    }));
                    selectedValues.push(item.value); // collect all values
                });

                $dropdown.select2({
                    placeholder: "Select location",
                    width: '100%'
                });

                // Select all options
                $dropdown.val(selectedValues).trigger('change');
            }
        });
    }
    function BindContractorDropdown(companyId, monthYr) {
        $.ajax({
            type: 'GET',
            url: '/DropDown/FetchContractorPayrollValidationDropdown',
            data: { company_ID: companyId, month_Yr: monthYr, isActive: true },
            success: function (response) {
                var $dropdown = $('#contractor');
                if ($dropdown.hasClass("select2-hidden-accessible")) {
                    $dropdown.select2("destroy");
                }
                $dropdown.empty();

                var selectedValues = [];
                $.each(response, function (i, item) {
                    $dropdown.append($('<option>', {
                        value: item.value,
                        text: item.text
                    }));
                    selectedValues.push(item.value);
                });

                $dropdown.select2({
                    placeholder: "Select contractor",
                    width: '100%'
                });

                $dropdown.val(selectedValues).trigger('change');
            }
        });
    }
    function BindWorkOrderDropdown(monthYr, companyId) {
        $.ajax({
            type: 'GET',
            url: '/DropDown/FetchWorkOrderPayrollValidationDropdown',
            data: { month_Yr: monthYr, company_ID: companyId, isActive: true },
            success: function (response) {
                var $dropdown = $('#workorder');
                if ($dropdown.hasClass("select2-hidden-accessible")) {
                    $dropdown.select2("destroy");
                }
                $dropdown.empty();

                var selectedValues = [];
                $.each(response, function (i, item) {
                    $dropdown.append($('<option>', {
                        value: item.value,
                        text: item.text
                    }));
                    selectedValues.push(item.value);
                });

                $dropdown.select2({
                    placeholder: "Select workorder",
                    width: '100%'
                });

                $dropdown.val(selectedValues).trigger('change');
            }
        });
    }

    $('#btnfilterProcessCalculate').on('click', function (e) {
        var isValid = true;
        // Clear all previous errors
        $('.input_error_msg').text('');
        const monthYear = $('#monthyear').val();
        //const monthYearId = $('#monthyear').text().trim();
        const monthYearText = $('#monthyear option:selected').text().trim(); // Should contain "Month - Year"
        const company = $('#Companies').val();
        const locations = $('#location').val();
        const contractors = $('#contractor').val();
        const workorders = $('#workorder').val();

        if (!monthYear) {
            $('#monthyear-error').text('Please select Month-Year.');
            isValid = false;
        }

        if (!company) {
            $('#Companies-error').text('Please select Company.');
            isValid = false;
        }
        // Extract year from "Month - Year" (e.g., "May - 2025")
        var year_ID = null;
        if (monthYearText.includes('-')) {
            const parts = monthYearText.split('-');
            year_ID = parseInt(parts[1].trim());
        }
        //console.log("year_ID", year_ID);
        if (isValid) {
            // Create the DTO object to send to the backend
            const dto = {
                company_ID: parseInt(company),
                month_ID: parseInt(monthYear),  // Assuming format is "06-2024"
                year_ID: year_ID,
                locationIDs: locations.join(','), // assuming multiselect
                contractorIDs: contractors.join(','),
                workOrderIDs: workorders.join(',')
            };
            $.ajax({
                url: '/PayrollProcessing/UpdatePayrollTransDataForProcess',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(dto),
                success: function (response) {
                    //console.log(response);
                    //if (response.success) {
                    if (response.updatedRecords > 0) {
                        $('#calculateConfirmation').modal('show');
                    } else {
                        showAlert('danger', response.message || 'Failed to update data.');
                        //alert(response.message || 'Failed to update data.');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error:', error);
                    alert('An unexpected error occurred.');
                }
            });
        }
    });

    $('.btn-transparent-danger').on('click', function () {
        $('#monthyear, #Companies, #location, #contractor, #workorder')
            .val(null)
            .trigger('change');

        $('.input_error_msg').text('');
    });

    $('#btnSingleRun').on('click', function () {
        $.ajax({
            url: '/PayrollProcessing/SetPayrollProcessMode',
            type: 'POST',
            data: { mode: 1 }, // Single Run
            success: function (response) {
                if (response.success) {
                    $('#calculateConfirmation').modal('hide');
                    $('#calculateProgress').modal('show'); // or redirect
                } else {
                    showAlert('danger', response.message || 'Failed to set mode.');
                    //alert(response.message || 'Failed to set mode.');
                }
            },
            error: function () {
                alert('Error setting session value.');
            }
        });
    });

    // ✅ Notification Hub
    const notificationConnection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationhub")
        .build();

    var pollingInterval;
    var isPolling = false;
    var payrollProcessId, payrollHeaderId, processSequenceId;
    var firstProcessSequenceId = 1;
    var secondProcessSequenceId;
    var firstPhaseReturnedSequenceId; 

    $(document).ready(function () {
        Promise.all([
            notificationConnection.start()
        ]).then(() => {
            //console.log("✅ SignalR connections established");
            const connectionId = notificationConnection.connectionId;

            // ✅ Attach SignalR listener only ONCE
            notificationConnection.on("ReceiveNotificationCount1", function (data) {
                if (data) {
                    updatePayrollStatus(data); // updates modal
                }
            });

            // ✅ Button click handlers
            $('#btnPhaseWise').on('click', function () {
                const dto = buildDto(firstProcessSequenceId, 0, connectionId);
                //console.log("btnPhaseWise Response:", dto);
                handlePayrollProcess(dto, true); // true: it's first phase
            });

            $('#nextphaseButton').on('click', function () {
                const dto = buildDto(processSequenceId, payrollProcessId, connectionId);
                //console.log("nextphaseButton Response:", dto);
                handlePayrollProcess(dto, false); // false: next phase
            });

        }).catch(function (err) {
            console.error("❌ Error connecting SignalR:", err.toString());
        });
    });

    // ✅ Helper: Build DTO
    function buildDto(sequenceId, existingProcessId, connectionId) {
        const monthYearText = $('#monthyear option:selected').text().trim(); // Should contain "Month - Year"
        var year_ID = null;
        if (monthYearText.includes('-')) {
            const parts = monthYearText.split('-');
            year_ID = parseInt(parts[1].trim());
        }
        return {
            Mode: 2,
            Cmp_Id: parseInt($('#Companies').val()),
            Month_Id: parseInt($('#monthyear').val()),
            Year_Id: year_ID,
            Payroll_Process_Id: existingProcessId,
            Payroll_header_Id: 0,
            Process_Sequence_Id: sequenceId,
            SignalRConnectionId: connectionId,
        };
    }

    // ✅ Common Handler for Both Buttons
    function handlePayrollProcess(dto, isFirstPhase) {
        //console.log(isFirstPhase ? "▶️ First phase DTO:" : "⏭️ Next phase DTO:", dto);

        $('#calculateConfirmation').modal('hide');
        // ✅ Set phase title before showing modal
        const phaseTitle = isFirstPhase ? 'Phase 1/2' : 'Phase 2/2';
        const descriptionTitle = isFirstPhase ? 'Payroll earning is in progress. Please wait for a while.' : 'Payroll deduction is in progress. Please wait for a while.';
        $('#phasepopup').text(phaseTitle);
        $('#phasepopupdetail').text(descriptionTitle);

        $('#calculateProgress').modal('show');

        // ✅ Start polling once
        if (!isPolling) {
            isPolling = true;
            pollingInterval = setInterval(() => fetchNotificationStatus(dto), 3000);
            //console.log("▶️ Polling started...");
        }

        $.ajax({
            url: '/PayrollProcessing/PostStartPayrollProcess',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (res) {
                //console.log("ℹ️ Response:", res);

                if (res.success) {
                    payrollProcessId = res.processId;
                    payrollHeaderId = res.payrollheaderId;
                    processSequenceId = res.processSequenceId;
                    secondProcessSequenceId = res.processSequenceId;
                    //const sequenceIdForFetch = isFirstPhase ? firstProcessSequenceId : secondProcessSequenceId;
                    // ✅ Store the first phase response ID only once
                    if (isFirstPhase) {
                        firstPhaseReturnedSequenceId = res.processSequenceId;
                    }

                    // ✅ Determine correct sequenceId to pass to table
                    const sequenceIdForFetch = isFirstPhase
                        ? firstProcessSequenceId                     // 1 for first phase
                        : firstPhaseReturnedSequenceId;             // response from first phase
                    //console.log("✅ Fetching Table with:", sequenceIdForFetch);
                    //console.log("next phase method :", dto.Cmp_Id, payrollProcessId, payrollHeaderId, dto.Month_Id, dto.Year_Id, sequenceIdForFetch, dto.Mode);
                    fetchPhaseWiseTable(dto.Cmp_Id, payrollProcessId, payrollHeaderId, dto.Month_Id, dto.Year_Id, sequenceIdForFetch, dto.Mode);
                } else {
                   /* alert("❌ Payroll start failed: " + res.message);*/
                    $('#calculateProgress').modal('hide');
                }
            },
            error: function () {
                //alert("❌ Failed to queue payroll process.");
                $('#calculateProgress').modal('hide');
            }
        });
    }

    // ✅ Supporting Functions
    function fetchNotificationStatus(dto) {
        //console.log("fetchNotificationStatus", dto);

        $.ajax({
            url: '/ViewNotification/TriggerSignalRUpdate',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (data) {
                if (data && data.status === 1) {
                    stopPolling();
                    //console.log("✅ Notification polling stopped.");
                }
            },
            error: function (xhr, status, error) {
                console.error("❌ Failed to fetch notification status:", error);
            }
        });
    }
    function stopPolling() {
        clearInterval(pollingInterval);
        isPolling = false;
    }
    function updatePayrollStatus(data) {
        const dto = {
            Cmp_Id: parseInt($('#Companies').val()),
            Month_Id: parseInt($('#monthyear').val()),
            Year_Id: parseInt($('#monthyear').text().trim()),
            Mode: 2,
        };
        //console.log("🔔 Notification Update:", data);

        const total = data.total || 0;
        const completed = data.completed || 0;
        const remaining = data.remaining || 0;

        // ✅ Update values
        $('#notif-completed').text(`Completed: ${completed}`);
        $('#notif-remaining').text(`Remaining: ${remaining}`);
        $('#notif-total').text(`Total: ${total}`);

        const percent = total > 0 ? Math.round((completed / total) * 100) : 0;

        // ✅ Set progress bar width & aria
        const $progressBar = $('.progress-bar');
        $progressBar
            .css('width', `${percent}%`)
            .attr('aria-valuenow', percent);

        // ✅ Set percentage text
        $('#progress-percent').text(`${percent}%`);

        // ✅ Remove default background classes
        $progressBar.removeClass('bg-success bg-warning bg-danger bg-info bg-primary');

        // ✅ Set dynamic gradient based on % (using shades of purple)
        const gradientColor = getGradientColor(percent);
        $progressBar.css({
            'background': gradientColor,
            'transition': 'width 0.5s ease'
        });
        // ✅ Auto-close on 100%
        if (percent === 100) {
            setTimeout(() => {
                $('#calculateProgress').modal('hide');
            }, 1500);
        }
    }
    function getGradientColor(percent) {
        if (percent < 30) {
            return 'linear-gradient(to right, #E1BEE7, #CE93D8)'; // light purple
        } else if (percent < 70) {
            return 'linear-gradient(to right, #BA68C8, #AB47BC)'; // medium purple
        } else {
            return 'linear-gradient(to right, #8E24AA, #6A1B9A)'; // dark purple
        }
    }
    function fetchPhaseWiseTable(companyId, payrollProcessId, payrollHeaderId, monthId, yearId, processSequenceId, payrollRunType) {
        const dto = {
            Company_Id: companyId,
            Payroll_Process_Id: payrollProcessId,
            Payroll_Header_Id: 0,
            MonthId: monthId,
            YearId: yearId,
            Process_Sequence_Id: processSequenceId,
            Payroll_Run_Type: payrollRunType
        };

        $.ajax({
            url: '/PayrollProcessing/GetPhaseWiseComponentPayrollProcess',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (response) {
                //console.log("Phase data:", response.data);

                if (response.success && response.data && response.data.length > 0) {
                    $('#filterDiv').hide();
                    generateDynamicTable(response.data);
                    //console.log("generateDynamicTable response: ", response.data);
                    $('#dynamicTableSection').show();

                    // ✅ Phase Label Logic
                    if (processSequenceId === 1) {
                        $('#titlephase').text("Earning");
                        $('#nextphaseButton').show();
                    }
                    else {
                        $('#titlephase').text("Deduction");
                        $('#nextphaseButton').hide();
                    }
                   
                } else {
                    showAlert('danger', "No payroll data found.");
                }
            },
            error: function () {
                alert("Error loading payroll table data.");
            }
        });
    }
 
    function generateDynamicTable(data) {
        const tableId = 'dynamic-payroll-table';
        const tableSelector = '#' + tableId;

        // ✅ Destroy and fully remove table wrapper
        if ($.fn.DataTable.isDataTable(tableSelector)) {
            $(tableSelector).DataTable().clear().destroy();
        }

        // ✅ Fully clear thead, tbody and replace table
        $(tableSelector).find('thead').empty();
        $(tableSelector).find('tbody').empty();

        if (!Array.isArray(data) || data.length === 0) return;

        // ✅ Extract unique columns from entire dataset
        const allKeys = new Set();
        data.forEach(row => {
            Object.keys(row).forEach(key => allKeys.add(key));
        });

        const columns = Array.from(allKeys);
        //console.log("✅ Final dynamic columns:", columns);

        // ✅ Create header
        const headerRow = $('<tr>');
        columns.forEach(col => {
            headerRow.append($('<th>').text(formatHeader(col)));
        });
        $(tableSelector).find('thead').append(headerRow);

        // ✅ Create body
        data.forEach(row => {
            const tr = $('<tr>');
            columns.forEach(col => {
                const val = row[col] !== null && row[col] !== undefined ? row[col] : '0';
                //const val = row[col] !== null && row[col] !== undefined ? row[col] : '-';
                tr.append($('<td>').text(val));
            });
            $(tableSelector).find('tbody').append(tr);
        });

        // ✅ Initialize DataTable again
        makePayrollDataTable(tableId);
    }

    // Optional: Convert snake_case or camelCase to readable text
    function formatHeader(header) {
        return header
            .replace(/([a-z])([A-Z])/g, '$1 $2')     // camelCase → camel Case
            .replace(/_/g, ' ')                      // snake_case → snake case
            .replace(/\b\w/g, c => c.toUpperCase()); // capitalize words
    }
    function makePayrollDataTable(tableId) {
        var searchbox = '<"form-group has-search margin-bottom-search mb-0"f>';
        var _buttons = tableId === "migrate" ? '' : '<"#customButtons.d-flex align-items-center gap-4 me-4">';
        var _language = {
            "info": "<span class='active'>_START_</span>-<span>_END_</span> of <span class='dataTables_info_total'>_TOTAL_</span>",
            "infoFiltered": "",
            "infoEmpty": "",
            "search": "<img src='/assets/img/icons/search.svg' class='' alt='search' width='20' height='20'>",
            "lengthMenu": "Show rows per page _MENU_",
            "paginate": {
                "next": "<img src='/assets/img/icons/next-table-data.svg' width='12' height='12' />",
                "previous": "<img src='/assets/img/icons/prev-table-data.svg' width='12' height='12' />"
            }
        };

        var _dom = `<"d-flex justify-content-between align-items-center p-2"
                <"d-flex align-items-center"
                    ${searchbox}
                >
                <"#last_section.last_section align-items-center d-flex"
                    ${_buttons}<"#dt_length.custom_table_length d-flex align-items-center"l>
                    <"#dt_info"i>
                    <"#dt_paginate.dt_paginate"p>
                >
            >t`;

        // ✅ Just initialize the table (no `columns`, no `columnDefs`)
        $('#' + tableId).DataTable({
            scrollY: true,
            fixedHeader: true,
            language: _language,
            dom: _dom,
            order: [] // Optional: disables initial sort
        });

        // Optional DOM adjustment
        setTimeout(function () {
            $("#" + tableId + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
        }, 100);
    }

});