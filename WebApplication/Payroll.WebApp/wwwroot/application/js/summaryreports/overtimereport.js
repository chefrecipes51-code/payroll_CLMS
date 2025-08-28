$(document).ready(function () {
    const sessionCompanyId = $('#sessionCompanyId').val();
    const $companyDropdown = $('#mapCompanyDropdown');
    const $contractorDropdown = $('#mapContractorDropdown');
    const $locationDropdown = $('#mapLocationDropdown');
    const $edatef = $('#edatef');
    const $monthName = $('#monthName');
    const $year = $('#year');
    const $dataTable = $('#overtimereport-list').DataTable();

    // Load Select2
    $.getScript('/assets/src/js/select2.min.js', function () {
        $.getScript('/assets/src/custom-js/select2.js');
    });

    // Clear validation messages
    $(document).on('change', '#mapCompanyDropdown, #mapContractorDropdown', function () {
        $(`#${this.id}-error`).text('');
    });

    // Initialize Page
    fetchCompanies(sessionCompanyId, () => {
        $companyDropdown.val(sessionCompanyId);
        fetchPreviousMonthYearForReport(sessionCompanyId)
            .done(() => {
                fetchContractors(sessionCompanyId)
                    .done(() => fetchLocations(sessionCompanyId));
            });
    });

    // Reset Filters
    $('#resetFilterBtn').on('click', function () {
        $companyDropdown.val(sessionCompanyId);
        $contractorDropdown.val('').trigger('change');
        $locationDropdown.val('').trigger('change');
        $edatef.val('').trigger('change');
        $monthName.val('');
        $year.val('');
        $('.input_error_msg').text('');
        $dataTable.clear().draw();
    });

    // Validate Form
    function validateFilterForm() {
        let isValid = true;
        $('.input_error_msg').text('');

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

    // Populate Dropdowns
    function populateSingleSelectDropdown(selector, data, valueField, textField, selectedValue = '') {
        const $dropdown = $(selector).empty().append('<option value="">Select</option>');
        data.forEach(item => $dropdown.append(new Option(item[textField], item[valueField])));
        $dropdown.val(selectedValue);
        if ($.fn.select2) $dropdown.select2({ placeholder: $dropdown.data('placeholder') || 'Select', allowClear: true, width: '100%', dropdownAutoWidth: true, dropdownParent: $('#standardFilterModal') });
    }

    function populateMultiSelectDropdown(selector, data, valueField, textField, selectedValues = []) {
        const $dropdown = $(selector).empty();
        data.forEach(item => $dropdown.append(new Option(item[textField], item[valueField])));
        $dropdown.val(selectedValues).trigger('change');
        if ($.fn.select2) $dropdown.select2({ placeholder: $dropdown.data('placeholder') || 'Select', allowClear: true, width: '100%', multiple: true, dropdownAutoWidth: true, dropdownParent: $('#standardFilterModal') });
    }

    // Fetch Data
    function fetchCompanies(selectedValue = '', callback = null) {
        $.get('/DropDown/FetchCompaniesDropdown')
            .done(data => {
                populateSingleSelectDropdown('#mapCompanyDropdown', data, 'value', 'text', selectedValue);
                if (typeof callback === 'function') callback();
            });
    }

    function fetchContractors(companyId) {
        return $.get(`/DropDown/FetchContractorDropdown?contractor_ID=0&company_ID=${companyId}`)
            .done(data => populateMultiSelectDropdown('#mapContractorDropdown', data, 'value', 'text'));
    }

    function fetchLocations(companyId) {
        return $.get(`/DropDown/FetchDistinctLocationDropdown?companyId=${companyId}`)
            .done(data => populateMultiSelectDropdown('#mapLocationDropdown', data, 'value', 'text'));
    }

    function fetchPreviousMonthYearForReport(companyId) {
        return $.get('/ContractorValidation/GetPreviousMonthYearPeriod', { companyId })
            .done(response => {
                if (response.isSuccess && response.data) {
                    $('#hiddenMonthId').val(response.data.month_Id);
                    $year.val(response.data.year);
                    $monthName.val(response.data.monthName);
                } else {
                    alert(response.message || 'Unable to fetch pay period.');
                }
            });
    }

    function fetchOvertimeReport() {
        const requestData = {
            CompanyID: parseInt($companyDropdown.val()) || 0,
            CompanyLocationIDs: null,
            ContractorIDs: ($contractorDropdown.val() || []).join(','),
            EntityIDs: null,
            PayrollMonth: parseInt($('#hiddenMonthId').val()) || 0,
            PayrollYear: parseInt($year.val()) || 0,
            FinancialYearStart: convertToDate($edatef.val())
        };

        return $.ajax({
            url: '/RegisterWagesReport/GetOvertimeReport',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(requestData)
        }).done(response => {
            if (response?.isSuccess && Array.isArray(response.data)) {
                populateOvertimeTable(response.data);
            } else {
                showAlert('danger', response.message);
            }
        });
    }

    function populateOvertimeTable(data) {
        const formattedRows = data.map(item => [
            item.entity_Code || '',
            item.entity_Name || '',
            item.gender || '',
            item.father_Name || '',
            item.skillcategory_Name || '',
            formatNumber(item.total_OT_Hours),
            formatNumber(item.oT_Rate),
            formatNumber(item.oT_Amount),
            formatDate(item.oT_Paid_Date)
        ]);
        $dataTable.clear().rows.add(formattedRows).draw();
    }

    function formatNumber(val) {
        return val != null ? parseFloat(val).toFixed(2) : '0.00';
    }

    function formatDate(date) {
        return date ? new Date(date).toLocaleDateString('en-GB') : '';
    }

    function convertToDate(ddmmyyyy) {
        if (!ddmmyyyy) return null;
        const [dd, mm, yyyy] = ddmmyyyy.split('/');
        return `${yyyy}-${mm}-${dd}`;
    }

    // Events
    $companyDropdown.on('change', function () {
        const companyId = $(this).val();
        if (companyId) {
            fetchPreviousMonthYearForReport(companyId)
                .done(() => fetchContractors(companyId).done(() => fetchLocations(companyId)));
        }
    });

    $contractorDropdown.on('change', function () {
        const companyId = $companyDropdown.val();
        if (companyId) fetchLocations(companyId);
    });

    $('#applyEntityFilterBtn').on('click', function () {
        if (!validateFilterForm()) return;
        fetchOvertimeReport().then(() => {
            const modal = bootstrap.Modal.getInstance(document.getElementById('standardFilterModal'));
            if (modal) modal.hide();
        });
    });

    // Export
    $(document).on('click.export', '.export-data', function (e) {
        e.preventDefault();
        const type = $(this).data('type');
        const tableId = $(this).data('table-id');

        const rowCount = $dataTable.rows().count();
        if (rowCount === 0) return alert('No data available for export.');

        const fileName = tableId + '-export';
        if (type === 'csv') exportToCSV(fileName);
        else if (type === 'xlsx') exportToXLSX(fileName);
        else if (type === 'pdf') exportToPDF(fileName);
        else alert('Export type not supported.');
    });

    function exportToCSV(fileName) {
        const headers = ["Entity Code", "Entity Name", "Gender", "Father Name", "Skill Category", "OT Hours", "OT Rate", "OT Amount", "OT Paid Date"];
        const rows = $dataTable.rows().data().toArray().map(r => r.join(','));
        const csv = headers.join(',') + '\n' + rows.join('\n');
        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName + '.csv';
        link.click();
    }

    function exportToXLSX(fileName) {
        const headers = ["Entity Code", "Entity Name", "Gender", "Father Name", "Skill Category", "OT Hours", "OT Rate", "OT Amount", "OT Paid Date"];
        const rows = $dataTable.rows().data().toArray();
        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.aoa_to_sheet([headers, ...rows]);
        XLSX.utils.book_append_sheet(wb, ws, "OvertimeReport");
        XLSX.writeFile(wb, fileName + '.xlsx');
    }

    function exportToPDF(fileName) {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();
        const headers = ["Entity Code", "Entity Name", "Gender", "Father Name", "Skill Category", "OT Hours", "OT Rate", "OT Amount", "OT Paid Date"];
        const rows = $dataTable.rows().data().toArray();

        doc.text("Overtime Report", 14, 10);
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
