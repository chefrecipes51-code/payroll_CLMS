$(document).ready(function () {
    const sessionCompanyId = $("#sessionCompanyId").val();

    // Load Select2 plugin
    $.getScript('/assets/src/js/select2.min.js', function () {
        $.getScript('/assets/src/custom-js/select2.js');
    });

    $(document).on('change', '#mapCompanyDropdown', function () {
        $('#mapCompanyDropdown-error').text('');
    });

    $(document).on('change', '#mapContractorDropdown', function () {
        $('#mapContractorDropdown-error').text('');
    });

    function validateFilterForm() {
        let isValid = true;
        $(".input_error_msg").text("");

        const company = $('#mapCompanyDropdown').val();
        const contractors = $('#mapContractorDropdown').val();

        if (!company) {
            $('#mapCompanyDropdown-error').text('Please select company.');
            isValid = false;
        }

        if (!Array.isArray(contractors) || contractors.length === 0) {
            $('#mapContractorDropdown-error').text('Please select at least one contractor.');
            isValid = false;
        }

        return isValid;
    }

    //fetchCompanies(sessionCompanyId, function () {
    //    resetAllFilters();
    //    fetchPreviousMonthYearForReport(sessionCompanyId).done(function () {
    //        fetchContractors(sessionCompanyId).done(function () {
    //            fetchLocations(sessionCompanyId);
    //        });
    //    });
    //});
    (async function initializeFilters() {
        try {
            await fetchCompanies(sessionCompanyId);
            await fetchPreviousMonthYearForReport(sessionCompanyId);
            await fetchContractors(sessionCompanyId);
            await fetchLocations(sessionCompanyId);
           // resetAllFilters(); 
        } catch (err) {
            console.error("Initialization error:", err);
            alert("Failed to initialize filters.");
        }
    })();


    function resetAllFilters() {
        $('#mapCompanyDropdown').val(sessionCompanyId).trigger('change');
        $('#mapContractorDropdown').val('').trigger('change');
        $('#mapLocationDropdown').val('').trigger('change');
        $('#edatef').val('').trigger('change');
        $('#monthName').val('');
        $('#year').val('');
        $(".input_error_msg").text('');
        $('#taxdeductionreport-list').DataTable().clear().draw();
    }

    $(document).on('click', '#resetFilterBtn', function () {
        resetAllFilters();
    });

    function populateSingleSelectDropdown(selector, data, valueField, textField, selectedValue = "") {
        const dropdown = $(selector);
        dropdown.empty().append('<option value="">Select</option>');

        $.each(data || [], function (index, item) {
            dropdown.append(
                $('<option></option>').attr("value", item[valueField]).text(item[textField])
            );
        });

        dropdown.val(selectedValue).trigger('change');

        if ($.fn.select2) {
            dropdown.select2("destroy");
            dropdown.select2({
                placeholder: dropdown.data('placeholder') || 'Select',
                allowClear: true,
                width: '100%',
                dropdownAutoWidth: true,
                dropdownParent: $('#standardFilterModal')
            });
        }
    }

    function populateMultiSelectDropdown(selector, data, valueField, textField, selectedValues = []) {
        const dropdown = $(selector);
        dropdown.empty();

        $.each(data || [], function (index, item) {
            dropdown.append(
                $('<option></option>').attr("value", item[valueField]).text(item[textField])
            );
        });

        dropdown.val(selectedValues).trigger('change');

        if ($.fn.select2) {
            dropdown.select2("destroy");
            dropdown.select2({
                placeholder: dropdown.data('placeholder') || 'Select',
                allowClear: true,
                width: '100%',
                multiple: true,
                dropdownAutoWidth: true,
                dropdownParent: $('#standardFilterModal')
            });
        }
    }

    //function fetchPreviousMonthYearForReport(companyId) {
    //    return $.ajax({
    //        url: '/ContractorValidation/GetPreviousMonthYearPeriod',
    //        type: 'GET',
    //        data: { companyId }
    //    }).done(function (response) {
    //        if (response.isSuccess && response.data) {
    //            $('#hiddenMonthId').val(response.data.month_Id);
    //            $('#year').val(response.data.year);
    //            $('#monthName').val(response.data.monthName);
    //        } else {
    //            alert(response.message || "Unable to fetch pay period.");
    //        }
    //    }).fail(function () {
    //        alert("Error fetching previous month/year.");
    //    });
    //}
    function fetchPreviousMonthYearForReport(companyId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/ContractorValidation/GetPreviousMonthYearPeriod',
                type: 'GET',
                data: { companyId },
                success: function (response) {
                    if (response.isSuccess && response.data) {
                        $('#hiddenMonthId').val(response.data.month_Id);
                        $('#year').val(response.data.year);
                        $('#monthName').val(response.data.monthName);
                        resolve(); // ✅ Resolve only if success
                    } else {
                        alert(response.message || "Unable to fetch pay period.");
                        reject(new Error("No data received"));
                    }
                },
                error: function () {
                    alert("Error fetching previous month/year.");
                    reject(new Error("AJAX request failed"));
                }
            });
        });
    }

    function fetchTDSReport() {
        const requestData = {
            CompanyID: parseInt($('#mapCompanyDropdown').val()) || 0,
            CompanyLocationIDs: null, // You can update if needed
            ContractorIDs: ($('#mapContractorDropdown').val() || []).join(','),
            EntityIDs: null, // Extend filter if required
            PayrollMonth: parseInt($('#hiddenMonthId').val()) || 0,
            PayrollYear: parseInt($('#year').val()) || 0,
            ProcessedDate: null, // Add logic if needed
            FinancialYearStart: formatDateToYMD($('#edatef').val())
        };

        return $.ajax({
            url: '/Report/GetTaxDeductionReport',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(requestData),
            success: function (response) {
                console.log("TDS Response:", response);
                if (response?.isSuccess && Array.isArray(response.data)) {
                    populateTdsTable(response.data);
                } else {
                    showAlert("danger", response.message || "No data found.");
                }
            },
            error: function () {
                alert("Error fetching TDS report.");
            }
        });
    }

    function populateTdsTable(data) {
        const table = $('#taxdeductionreport-list').DataTable();
        table.clear();

        const rows = data.map(item => [
            item.entity_Code || '',
            item.entity_Name || '',
            item.payrollNo || '',
            formatNumber(item.taxableIncome),
            formatNumber(item.incomeTax),
            formatNumber(item.professionalTax),
            formatNumber(item.totalTaxDeductions),
            formatNumber(item.netTaxableAmount)
        ]);

        table.rows.add(rows).draw();
    }

    function formatNumber(value) {
        return value != null ? parseFloat(value).toFixed(2) : "0.00";
    }

    function formatDateToYMD(dateStr) {
        const parts = dateStr?.split('/');
        return parts?.length === 3 ? `${parts[2]}-${parts[1]}-${parts[0]}` : null;
    }

    //function fetchCompanies(selectedValue = '', callback = null) {
    //    $.ajax({
    //        url: '/DropDown/FetchCompaniesDropdown',
    //        type: 'GET',
    //        success: function (data) {
    //            populateSingleSelectDropdown('#mapCompanyDropdown', data, 'value', 'text', selectedValue);
    //            if (typeof callback === 'function') setTimeout(callback, 200);
    //        }
    //    });
    //}
    function fetchCompanies(selectedValue = '') {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/DropDown/FetchCompaniesDropdown',
                type: 'GET',
                success: function (data) {
                    populateSingleSelectDropdown('#mapCompanyDropdown', data, 'value', 'text', selectedValue);
                    resolve(); // done
                },
                error: reject
            });
        });
    }

    //function fetchContractors(companyId) {
    //    return $.ajax({
    //        url: `/DropDown/FetchContractorDropdown?contractor_ID=0&company_ID=${companyId}`,
    //        type: 'GET',
    //        success: function (data) {
    //            populateMultiSelectDropdown('#mapContractorDropdown', data, 'value', 'text');
    //        }
    //    });
    //}
    function fetchContractors(companyId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `/DropDown/FetchContractorDropdown?contractor_ID=0&company_ID=${companyId}`,
                type: 'GET',
                success: function (data) {
                    populateMultiSelectDropdown('#mapContractorDropdown', data, 'value', 'text');
                    resolve(); // explicitly resolve when done
                },
                error: reject
            });
        });
    }

    //function fetchLocations(companyId) {
    //    return $.ajax({
    //        url: `/DropDown/FetchDistinctLocationDropdown?companyId=${companyId}`,
    //        type: 'GET',
    //        success: function (data) {
    //            populateMultiSelectDropdown('#mapLocationDropdown', data, 'value', 'text');
    //        }
    //    });
    //}
    function fetchLocations(companyId) {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: `/DropDown/FetchDistinctLocationDropdown?companyId=${companyId}`,
                type: 'GET',
                success: function (data) {
                    populateMultiSelectDropdown('#mapLocationDropdown', data, 'value', 'text');
                    resolve(); // explicitly resolve
                },
                error: reject
            });
        });
    }
    $(document).on('change', '#mapCompanyDropdown', async function () {
        const companyId = $(this).val();
        if (!companyId) return;

        try {
            await fetchPreviousMonthYearForReport(companyId);
            await fetchContractors(companyId);
            await fetchLocations(companyId);
        } catch (err) {
            console.error("Error while handling company change:", err);
            alert("Failed to fetch filters based on selected company.");
        }
    });
    $(document).on('change', '#mapContractorDropdown', function () {
        const companyId = $('#mapCompanyDropdown').val();
        if (companyId) {
            fetchLocations(companyId);
        }
    });
    $(document).on('click', '#applyEntityFilterBtn', function () {
        if (!validateFilterForm()) return;
        fetchTDSReport().then(() => {
            const modalEl = document.getElementById('standardFilterModal');
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            if (modalInstance) modalInstance.hide();
        });
    });
    $(document).on('click.export', '.export-data', function (e) {
        e.preventDefault();
        const type = $(this).data('type');
        const tableId = $(this).data('table-id');

        if (tableId === 'taxdeductionreport-list') {
            const rowCount = $('#taxdeductionreport-list').DataTable().rows().count();
            if (rowCount === 0) return alert("No data available for export.");

            switch (type) {
                case 'csv': exportTdsToCSV(tableId + '-export'); break;
                case 'xlsx': exportTdsToXLSX(tableId + '-export'); break;
                case 'pdf': exportTdsToPDF(tableId + '-export'); break;
                default: alert("Export type not supported.");
            }
        }
    });
    function exportTdsToCSV(fileName) {
        const headers = ["Entity Code", "Payroll No", "Taxable Income", "Income Tax", "Professional Tax", "Total Tax", "Net Taxable"];
        const rows = $('#taxdeductionreport-list').DataTable().rows().data().toArray();
        const csvContent = [headers.join(','), ...rows.map(r => r.join(','))].join('\n');
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });

        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName + '.csv';
        link.click();
    }
    function exportTdsToXLSX(fileName) {
        const headers = ["Entity Code", "Payroll No", "Taxable Income", "Income Tax", "Professional Tax", "Total Tax", "Net Taxable"];
        const rows = $('#taxdeductionreport-list').DataTable().rows().data().toArray();

        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.aoa_to_sheet([headers, ...rows]);
        XLSX.utils.book_append_sheet(wb, ws, "TDSReport");
        XLSX.writeFile(wb, fileName + '.xlsx');
    }

    function exportTdsToPDF(fileName) {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();
        const headers = ["Entity Code", "Payroll No", "Taxable Income", "Income Tax", "Professional Tax", "Total Tax", "Net Taxable"];
        const rows = $('#taxdeductionreport-list').DataTable().rows().data().toArray();

        doc.text("TDS Report", 14, 10);
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
