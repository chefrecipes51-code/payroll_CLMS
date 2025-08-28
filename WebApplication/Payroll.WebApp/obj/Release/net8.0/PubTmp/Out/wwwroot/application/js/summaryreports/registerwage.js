$(document).ready(function () {
    var globalSalaryData = [];
    const sessionCompanyId = $("#sessionCompanyId").val();
    $.getScript('/assets/src/js/select2.min.js', function () {
        $.getScript('/assets/src/custom-js/select2.js', function () {
            //initializeSelect2('.select2_search_ctm');
        });
    });
    fetchCompanies(sessionCompanyId, function () {
        resetAllFilters(); // Optional, if it just resets UI
        fetchPreviousMonthYearForReport(sessionCompanyId).done(function () {
            fetchContractors(sessionCompanyId).done(function () {
                fetchLocations(sessionCompanyId).done(function () {
                    fetchSalaryReport(); // ✅ Fetch salary data only after all dropdowns & dates are ready
                });
            });
        });
    });
    function resetAfterCompany() {
        populateMultiSelectDropdown('#mapLocationDropdown', [], 'value', 'text');
        populateMultiSelectDropdown('#mapContractorDropdown', [], 'value', 'text');
    }
    function resetAllFilters() {
        //$('#mapCompanyDropdown').val(sessionCompanyId).trigger('change');
        //$('#mapContractorDropdown').val([]).trigger('change');
        //$('#mapLocationDropdown').val([]).trigger('change'); 
        //fetchContractors(sessionCompanyId);
        //$(".input_error_msg").text('');
    }
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

        dropdown.off("select2:open"); // Unbind previous
        dropdown.select2("destroy");  // Destroy if already initialized
        initializeSelect2(dropdown); // Safe initialize
    }
    function populateSingleSelectDropdown(selector, data, valueField, textField, selectedValue = "") {
        const dropdown = $(selector);
        dropdown.empty().append('<option value="">Select</option>');

        if (Array.isArray(data) && data.length > 0) {
            $.each(data, function (index, item) {
                dropdown.append(
                    $('<option></option>')
                        .attr("value", item[valueField])
                        .text(item[textField])
                );
            });
        }

        dropdown.val(selectedValue).trigger('change');

        if ($.fn.select2) {
            dropdown.select2("destroy");
            dropdown.select2({
                placeholder: dropdown.data('placeholder') || 'Select',
                allowClear: true,
                width: '100%',
                dropdownAutoWidth: true,
                dropdownParent: $('#standardFilterModal') // Important for modal
            });
        }
    }
    function populateMultiSelectDropdown(selector, data, valueField, textField, selectedValues = []) {
        const dropdown = $(selector);
        dropdown.empty();

        if (Array.isArray(data) && data.length > 0) {
            $.each(data, function (index, item) {
                dropdown.append(
                    $('<option></option>')
                        .attr("value", item[valueField])
                        .text(item[textField])
                );
            });
        }

        dropdown.val(selectedValues).trigger('change');

        if ($.fn.select2) {
            dropdown.select2("destroy");
            dropdown.select2({
                placeholder: dropdown.data('placeholder') || 'Select',
                allowClear: true,
                width: '100%',
                multiple: true,
                dropdownAutoWidth: true,
                dropdownParent: $('#standardFilterModal') // Required inside modal
            });
        }
    }
    function fetchPreviousMonthYearForReport(companyId) {
        return $.ajax({
            url: '/ContractorValidation/GetPreviousMonthYearPeriod',
            type: 'GET',
            data: { companyId: companyId }
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
    //function fetchSalaryReport() {
    //    const companyID = parseInt($('#mapCompanyDropdown').val()) || 0;
    //    const contractorIDs = $('#mapContractorDropdown').val() || [];
    //    const locationIDs = $('#mapLocationDropdown').val() || [];
    //    const payrollMonth = parseInt($('#hiddenMonthId').val()) || 0;
    //    const payrollYear = parseInt($('#year').val()) || 0;
    //    const financialYearStart = $('#edatef').val() || "";

    //    const requestData = {
    //        companyID: companyID,
    //        contractorIDs: contractorIDs.join(','),
    //        payrollMonth: payrollMonth,
    //        payrollYear: payrollYear,
    //        financialYearStart: financialYearStart
    //    };

    //    console.log("Requesting Salary Report with:", requestData);

    //    // ✅ Return the AJAX Promise
    //    return $.ajax({
    //        url: '/RegisterWagesReport/FetchSalaryReport',
    //        type: 'POST',
    //        contentType: 'application/json',
    //        data: JSON.stringify(requestData),
    //        success: function (response) {
    //            if (response && Array.isArray(response)) {
    //                populateSalaryTable(response);
    //                // 🛠️ Add this here to check if DataTables is picking up rows
    //                console.log("Rows count after population:", $("#wagereport-list").DataTable().rows().count());

    //            } else {
    //                console.warn("No valid salary data received.");
    //            }
    //        },
    //        error: function () {
    //            alert("Error fetching salary report.");
    //        }
    //    });
    //}   
    function fetchSalaryReport() {
        const companyID = parseInt($('#mapCompanyDropdown').val()) || 0;
        const contractorIDs = $('#mapContractorDropdown').val() || [];
        const locationIDs = $('#mapLocationDropdown').val() || [];
        const payrollMonth = parseInt($('#hiddenMonthId').val()) || 0;
        const payrollYear = parseInt($('#year').val()) || 0;
        const financialYearStart = $('#edatef').val() || "";

        const requestData = {
            companyID: companyID,
            contractorIDs: contractorIDs.join(','),
            payrollMonth: payrollMonth,
            payrollYear: payrollYear,
            financialYearStart: financialYearStart
        };

        return $.ajax({
            url: '/RegisterWagesReport/FetchSalaryReport',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(requestData),
            success: function (response) {
                if (response && Array.isArray(response)) {
                    globalSalaryData = response;
                    populateSalaryTable(response); // ✅ Update existing table
                } else {
                    console.warn("No valid salary data received.");
                }
            },
            error: function () {
                alert("Error fetching salary report.");
            }
        });
    }
    function populateSalaryTable(data) {

        const table = $('#wagereport-list').DataTable(); // Already initialized globally

        table.clear(); // Clear previous rows

        const formattedRows = data.map(item => [
            item.entityName || '',
            item.entityCode || '',
            item.skillName || '',
            item.totalWorkingDays ?? 0,
            item.presentDays ?? 0,
            formatNumber(item.basicWages),
            formatNumber(item.da),
            formatNumber(item.totalOTHours),
            formatNumber(item.wage_Per_Day),
            formatNumber(item.otherearning),
            formatNumber(item.totalEarnings),
            formatNumber(item.totalDeductions),
            formatNumber(item.netPay)
        ]);

        table.rows.add(formattedRows).draw();

        console.log("✅ Data loaded. Row count:", table.rows().count());
    }
    function formatNumber(value) {
        return value !== null && value !== undefined
            ? parseFloat(value).toFixed(2)
            : "0.00";
    }
    function initializeSelect2(selector) {
        const el = $(selector);
        if ($.fn.select2 && el.length) {
            el.select2({
                placeholder: el.data('placeholder') || 'Select',
                allowClear: true,
                width: '100%',
                dropdownAutoWidth: true
            });
        }
    }
    function fetchCompanies(selectedValue = '', callback = null) {
        $.ajax({
            url: '/DropDown/FetchCompaniesDropdown',
            type: 'GET',
            success: function (data) {
                populateSingleSelectDropdown('#mapCompanyDropdown', data, 'value', 'text', selectedValue);
                if (callback && typeof callback === "function") {
                    setTimeout(callback, 200);
                }
            },
            error: function () {
                console.error("Error fetching companies");
            }
        });
    }
    function fetchContractors(companyId) {
        return $.ajax({
            url: `/DropDown/FetchContractorDropdown?contractor_ID=0&company_ID=${companyId}`,
            type: 'GET',
            success: function (data) {
                populateMultiSelectDropdown('#mapContractorDropdown', data, 'value', 'text');
            },
            error: function () {
                console.error("Error fetching contractors");
            }
        });
    }
    function fetchLocations(companyId) {
        return $.ajax({
            url: `/DropDown/FetchDistinctLocationDropdown?companyId=${companyId}`,
            type: 'GET',
            success: function (data) {
                populateMultiSelectDropdown('#mapLocationDropdown', data, 'value', 'text');
            },
            error: function () {
                console.error("Error fetching locations");
            }
        });
    }

    $(document).on('change', '#mapCompanyDropdown', function () {
        const companyId = $(this).val();

        if (companyId) {
            fetchPreviousMonthYearForReport(companyId)
                .done(() => {
                    fetchContractors(companyId)
                        .done(() => {
                            fetchLocations(companyId)
                                .done(() => {
                                    fetchSalaryReport(); // Final fetch
                                });
                        });
                });
        }
    });
    $(document).on('change', '#mapContractorDropdown', function () {
        const companyId = $('#mapCompanyDropdown').val();
        //resetAfterCompany(); // If needed for this level
        if (companyId) {
            fetchLocations(companyId);
        }
    });
    $(document).on('click', '#applyEntityFilterBtn', function () {
        fetchSalaryReport().then(() => {
            const modalEl = document.getElementById('standardFilterModal');
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            if (modalInstance) {
                modalInstance.hide();
            }
        });
    });
    $(document).on('click.export', '.export-data', function (e) {
        e.preventDefault();
        console.log("Export click ");
        const type = $(this).data('type');
        const tableId = $(this).data('table-id');

        if (tableId === 'wagereport-list') {
            if (!Array.isArray(globalSalaryData) || globalSalaryData.length === 0) {
                alert("No data available for export.");
                return;
            }

            switch (type) {
                case 'csv':
                    exportSalaryToCSVWage(tableId + '-export');
                    break;
                case 'xlsx':
                    exportSalaryToXLSXWage(tableId + '-export');
                    break;
                case 'pdf':
                    exportSalaryToPDFWage(tableId + '-export');
                    break;
                default:
                    alert("Export type not supported.");
            }
        } else {
            exportData(type, tableId); // fallback for other tables
        }
    });
    function exportSalaryToCSVWage(fileName) {
        const headers = [
            "Entity Name", "Entity Code", "Skill Name", "Total Working Days", "Present Days",
            "Basic", "DA", "OT Hrs", "Total Earnings", "Total Deductions", "Net Salary",
            "Gross Pay", "Net Pay", "Emp Contribution", "Emplr Contribution"
        ];

        const rows = globalSalaryData.map(item => [
            item.entityName || '',
            item.entityCode || '',
            item.skillName || '',
            item.totalWorkingDays ?? 0,
            item.presentDays ?? 0,
            formatNumber(item.basicWages),
            formatNumber(item.da),
            formatNumber(item.totalOTHours),
            formatNumber(item.totalEarnings),
            formatNumber(item.totalDeductions),
            formatNumber(item.net_Salary),
            formatNumber(item.grossPay),
            formatNumber(item.netPay),
            formatNumber(item.totalEmpContribution),
            formatNumber(item.totalEmplrContribution)
        ]);
        console.log("CSV EXPORt", rows);
        const csvContent = headers.join(',') + '\n' + rows.map(r => r.join(',')).join('\n');
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName + '.csv';
        link.click();
    }
    function exportSalaryToXLSXWage(fileName) {
        const headers = [
            "Entity Name", "Entity Code", "Skill Name", "Total Working Days", "Present Days",
            "Basic", "DA", "OT Hrs", "Total Earnings", "Total Deductions", "Net Salary",
            "Gross Pay", "Net Pay", "Emp Contribution", "Emplr Contribution"
        ];

        const rows = globalSalaryData.map(item => [
            item.entityName || '',
            item.entityCode || '',
            item.skillName || '',
            item.totalWorkingDays ?? 0,
            item.presentDays ?? 0,
            parseFloat(item.basicWages ?? 0),
            parseFloat(item.da ?? 0),
            parseFloat(item.totalOTHours ?? 0),
            parseFloat(item.totalEarnings ?? 0),
            parseFloat(item.totalDeductions ?? 0),
            parseFloat(item.net_Salary ?? 0),
            parseFloat(item.grossPay ?? 0),
            parseFloat(item.netPay ?? 0),
            parseFloat(item.totalEmpContribution ?? 0),
            parseFloat(item.totalEmplrContribution ?? 0)
        ]);

        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.aoa_to_sheet([headers, ...rows]);

        XLSX.utils.book_append_sheet(wb, ws, "SalaryReport");
        XLSX.writeFile(wb, fileName + '.xlsx');
    }
    function exportSalaryToPDFWage(fileName) {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();

        const headers = [
            "Entity", "Code", "Skill", "Work Days", "Present",
            "Basic", "DA", "OT Hrs", "Earnings", "Deductions", "Net Salary",
            "Gross", "Net", "Emp", "Emplr"
        ];

        const rows = globalSalaryData.map(item => [
            item.entityName || '',
            item.entityCode || '',
            item.skillName || '',
            item.totalWorkingDays ?? 0,
            item.presentDays ?? 0,
            formatNumber(item.basicWages),
            formatNumber(item.da),
            formatNumber(item.totalOTHours),
            formatNumber(item.totalEarnings),
            formatNumber(item.totalDeductions),
            formatNumber(item.net_Salary),
            formatNumber(item.grossPay),
            formatNumber(item.netPay),
            formatNumber(item.totalEmpContribution),
            formatNumber(item.totalEmplrContribution)
        ]);

        doc.text("Register of Wages Report", 14, 10);
        doc.autoTable({
            startY: 20,
            head: [headers],
            body: rows,
            theme: 'grid',
            styles: { fontSize: 6, cellPadding: 1 }
        });

        doc.save(fileName + '.pdf');
    }
});
