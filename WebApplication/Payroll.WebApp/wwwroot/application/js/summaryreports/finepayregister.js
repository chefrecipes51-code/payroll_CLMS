$(document).ready(function () {
    const sessionCompanyId = $("#sessionCompanyId").val();
    const $companyDropdown = $('#mapCompanyDropdown');
    const $contractorDropdown = $('#mapContractorDropdown');
    const $locationDropdown = $('#mapLocationDropdown');
    const $dataTable = $('#fineregisterreport-list').DataTable();

    // Load Select2 scripts
    $.getScript('/assets/src/js/select2.min.js', function () {
        $.getScript('/assets/src/custom-js/select2.js');
    });

    // Clear validation errors on dropdown change
    $companyDropdown.on('change', () => $('#mapCompanyDropdown-error').text(''));
    $contractorDropdown.on('change', () => $('#mapContractorDropdown-error').text(''));

    // Page Initialization - only once
    fetchCompanies(sessionCompanyId, () => {
        $companyDropdown.val(sessionCompanyId);
        fetchPreviousMonthYearForReport(sessionCompanyId)
            .done(() => {
                fetchContractors(sessionCompanyId)
                    .done(() => {
                        fetchLocations(sessionCompanyId);
                    });
            });
    });

    // On Company change
    $companyDropdown.on('change', function () {
        const companyId = $(this).val();
        if (companyId) {
            fetchPreviousMonthYearForReport(companyId)
                .done(() => {
                    fetchContractors(companyId)
                        .done(() => fetchLocations(companyId));
                });
        }
    });

    // On Contractor change
    $contractorDropdown.on('change', function () {
        const companyId = $companyDropdown.val();
        if (companyId) {
            fetchLocations(companyId);
        }
    });

    // Apply Filter
    $('#applyEntityFilterBtn').on('click', function () {
        if (!validateFilterForm()) return;

        fetchFineRegisterReport().then(() => {
            const modal = bootstrap.Modal.getInstance(document.getElementById('standardFilterModal'));
            if (modal) modal.hide();
        });
    });

    // Reset filters
    $('#resetFilterBtn').on('click', resetAllFilters);

    // Export
    $(document).on('click.export', '.export-data', function (e) {
        e.preventDefault();
        const type = $(this).data('type');
        const tableId = $(this).data('table-id');

        if (tableId === 'fineregisterreport-list') {
            if ($dataTable.rows().count() === 0) return alert("No data available for export.");
            const fileName = tableId + '-export';
            if (type === 'csv') exportFineToCSV(fileName);
            else if (type === 'xlsx') exportFineToXLSX(fileName);
            else if (type === 'pdf') exportFineToPDF(fileName);
            else alert("Export type not supported.");
        }
    });

    // Helper functions
    function validateFilterForm() {
        let isValid = true;
        $(".input_error_msg").text("");

        if (!$companyDropdown.val()) {
            $('#mapCompanyDropdown-error').text('Please select company.');
            isValid = false;
        }

        const contractors = $contractorDropdown.val();
        if (!Array.isArray(contractors) || contractors.length === 0) {
            $('#mapContractorDropdown-error').text('Please select at least one contractor.');
            isValid = false;
        }

        return isValid;
    }

    function resetAllFilters() {
        $contractorDropdown.val('').trigger('change');
        $locationDropdown.val('').trigger('change');
        $('#edatef').val('').trigger('change');
        $('#monthName').val('');
        $('#year').val('');
        $(".input_error_msg").text('');
        $dataTable.clear().draw();
    }

    function fetchCompanies(selectedValue = '', callback = null) {
        $.get('/DropDown/FetchCompaniesDropdown')
            .done(data => {
                populateSingleSelectDropdown('#mapCompanyDropdown', data, 'value', 'text', selectedValue);
                if (typeof callback === 'function') callback();
            })
            .fail(() => console.error("Error fetching companies"));
    }

    function fetchContractors(companyId) {
        return $.get(`/DropDown/FetchContractorDropdown?contractor_ID=0&company_ID=${companyId}`)
            .done(data => populateMultiSelectDropdown('#mapContractorDropdown', data, 'value', 'text'))
            .fail(() => console.error("Error fetching contractors"));
    }

    function fetchLocations(companyId) {
        return $.get(`/DropDown/FetchDistinctLocationDropdown?companyId=${companyId}`)
            .done(data => populateMultiSelectDropdown('#mapLocationDropdown', data, 'value', 'text'))
            .fail(() => console.error("Error fetching locations"));
    }

    function fetchPreviousMonthYearForReport(companyId) {
        return $.get('/ContractorValidation/GetPreviousMonthYearPeriod', { companyId })
            .done(response => {
                if (response.isSuccess && response.data) {
                    $('#hiddenMonthId').val(response.data.month_Id);
                    $('#year').val(response.data.year);
                    $('#monthName').val(response.data.monthName);
                } else {
                    alert(response.message || "Unable to fetch pay period.");
                }
            })
            .fail(() => alert("Error fetching previous month/year."));
    }

    function fetchFineRegisterReport() {
        const financialYearRaw = $('#edatef').val();
        let financialYearStart = null;
        if (financialYearRaw) {
            const parts = financialYearRaw.split('/');
            if (parts.length === 3) {
                financialYearStart = `${parts[2]}-${parts[1]}-${parts[0]}`;
            }
        }

        const requestData = {
            CompanyID: parseInt($companyDropdown.val()) || 0,
            ContractorIDs: ($contractorDropdown.val() || []).join(','),
            CompanyLocationIDs: null,
            EntityIDs: null,
            PayrollMonth: parseInt($('#hiddenMonthId').val()) || 0,
            PayrollYear: parseInt($('#year').val()) || 0,
            FinancialYearStart: financialYearStart
        };

        return $.ajax({
            url: '/RegisterWagesReport/GetFineRegisterReport',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(requestData)
        })
            .done(response => {
                if (response && response.isSuccess && Array.isArray(response.data)) {
                    populateFineRegisterTable(response.data);
                } else {
                    showAlert("danger", response.message || "No fine register data found.");
                }
            })
            .fail(() => alert("Error fetching fine register report."));
    }

    function populateFineRegisterTable(data) {
        const formattedRows = data.map(item => [
            item.entity_Code || '',
            item.entity_Name || '',
            item.father_Name || '',
            item.skillcategory_Name || '',
            item.reason || '',
            item.showCaused || '',
            item.date_Of_Offence || '',
            item.name_Of_WitNess || '',
            item.waage_Period || '',
            item.dt_Of_Realization || '',
            formatNumber(item.amount),
            item.remarks || ''
        ]);

        $dataTable.clear().rows.add(formattedRows).draw();
    }

    function populateSingleSelectDropdown(selector, data, valueField, textField, selectedValue = "") {
        const $dropdown = $(selector).empty().append('<option value="">Select</option>');
        data.forEach(item => {
            $dropdown.append(new Option(item[textField], item[valueField]));
        });

        $dropdown.val(selectedValue);

        if ($.fn.select2) {
            $dropdown.select2("destroy").select2({
                placeholder: $dropdown.data('placeholder') || 'Select',
                allowClear: true,
                width: '100%',
                dropdownAutoWidth: true,
                dropdownParent: $('#standardFilterModal')
            });
        }
    }

    function populateMultiSelectDropdown(selector, data, valueField, textField, selectedValues = []) {
        const $dropdown = $(selector).empty();
        data.forEach(item => {
            $dropdown.append(new Option(item[textField], item[valueField]));
        });

        $dropdown.val(selectedValues).trigger('change');

        if ($.fn.select2) {
            $dropdown.select2("destroy").select2({
                placeholder: $dropdown.data('placeholder') || 'Select',
                allowClear: true,
                multiple: true,
                width: '100%',
                dropdownAutoWidth: true,
                dropdownParent: $('#standardFilterModal')
            });
        }
    }

    function formatNumber(value) {
        return value != null ? parseFloat(value).toFixed(2) : "0.00";
    }

    function exportFineToCSV(fileName) {
        const headers = ["Entity Code", "Entity Name", "Father Name", "Skill Category", "Reason",
            "Show Caused", "Date Of Offence", "Name Of Witness", "Wage Period",
            "Date Of Realization", "Amount", "Remarks"];
        const rows = $dataTable.rows().data().toArray();
        const csvRows = rows.map(r => r.join(','));
        const blob = new Blob([headers.join(',') + '\n' + csvRows.join('\n')], { type: 'text/csv;charset=utf-8;' });

        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName + '.csv';
        link.click();
    }

    function exportFineToXLSX(fileName) {
        const headers = ["Entity Code", "Entity Name", "Father Name", "Skill Category", "Reason",
            "Show Caused", "Date Of Offence", "Name Of Witness", "Wage Period",
            "Date Of Realization", "Amount", "Remarks"];
        const rows = $dataTable.rows().data().toArray();
        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.aoa_to_sheet([headers, ...rows]);
        XLSX.utils.book_append_sheet(wb, ws, "FineRegister");
        XLSX.writeFile(wb, fileName + '.xlsx');
    }

    function exportFineToPDF(fileName) {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();
        const headers = ["Entity Code", "Entity Name", "Father Name", "Skill Category", "Reason",
            "Show Caused", "Date Of Offence", "Name Of Witness", "Wage Period",
            "Date Of Realization", "Amount", "Remarks"];
        const rows = $dataTable.rows().data().toArray();

        doc.text("Fine Register Report", 14, 10);
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
