// Optimized and corrected JavaScript
$(document).ready(function () {
    const sessionCompanyId = $("#sessionCompanyId").val();
    const $companyDropdown = $('#mapCompanyDropdown');
    const $contractorDropdown = $('#mapContractorDropdown');
    const $locationDropdown = $('#mapLocationDropdown');
    const $monthName = $('#monthName');
    const $year = $('#year');
    const $hiddenMonthId = $('#hiddenMonthId');
    const $dataTable = $('#salaryslipreport-list').DataTable();

    // Load Select2 scripts
    $.getScript('/assets/src/js/select2.min.js', function () {
        $.getScript('/assets/src/custom-js/select2.js');
    });

    // Clear validation errors on change
    $(document).on('change', '#mapCompanyDropdown, #mapContractorDropdown', function () {
        $('#' + this.id + '-error').text('');
    });

    // Reset Filters without triggering company change
    function resetAllFilters() {
        $companyDropdown.val(sessionCompanyId);
        $contractorDropdown.val('').trigger('change');
        $locationDropdown.val('').trigger('change');
        $('#edatef').val('').trigger('change');
        $monthName.val('');
        $year.val('');
        $('.input_error_msg').text('');
        $dataTable.clear().draw();
    }

    $('#resetFilterBtn').on('click', resetAllFilters);

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

    function populateSingleSelectDropdown(selector, data, valueField, textField, selectedValue = "") {
        const dropdown = $(selector).empty().append('<option value="">Select</option>');
        data.forEach(item => dropdown.append(new Option(item[textField], item[valueField])));
        dropdown.val(selectedValue);
        if ($.fn.select2) {
            dropdown.select2("destroy").select2({
                placeholder: dropdown.data('placeholder') || 'Select',
                allowClear: true,
                width: '100%',
                dropdownAutoWidth: true,
                dropdownParent: $('#standardFilterModal')
            });
        }
    }

    function populateMultiSelectDropdown(selector, data, valueField, textField, selectedValues = []) {
        const dropdown = $(selector).empty();
        data.forEach(item => dropdown.append(new Option(item[textField], item[valueField])));
        dropdown.val(selectedValues).trigger('change');
        if ($.fn.select2) {
            dropdown.select2("destroy").select2({
                placeholder: dropdown.data('placeholder') || 'Select',
                allowClear: true,
                width: '100%',
                multiple: true,
                dropdownAutoWidth: true,
                dropdownParent: $('#standardFilterModal')
            });
        }
    }

    function fetchPreviousMonthYearForReport(companyId) {
        return $.get('/ContractorValidation/GetPreviousMonthYearPeriod', { companyId })
            .done(response => {
                if (response.isSuccess && response.data) {
                    $hiddenMonthId.val(response.data.month_Id);
                    $year.val(response.data.year);
                    $monthName.val(response.data.monthName);
                } else {
                    alert(response.message || 'Unable to fetch pay period.');
                }
            })
            .fail(() => alert('Error fetching previous month/year.'));
    }

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

    function fetchSalarySlipReport() {
        const requestData = {
            CompanyID: parseInt($companyDropdown.val()) || 0,
            CompanyLocationIDs: null,
            ContractorIDs: ($contractorDropdown.val() || []).join(','),
            EntityIDs: null,
            PayrollMonth: parseInt($hiddenMonthId.val()) || 0,
            PayrollYear: parseInt($year.val()) || 0
        };

        return $.ajax({
            url: '/RegisterWagesReport/GetSalarySlipReport',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(requestData)
        }).done(response => {
            if (response?.isSuccess && Array.isArray(response.data)) {
                populateSalarySlipTable(response.data);
            } else {
                alert(response.message || 'No salary slip data found.');
            }
        }).fail(() => alert('Error fetching salary slip report.'));
    }

    function populateSalarySlipTable(data) {
        const rows = data.map(item => [
            item.entity_Code || '',
            item.entity_Name || '',
            item.entityAddress || '',
            item.contractor_Name || '',
            item.father_Name || '',
            item.month_Id || '',
            item.unit_Worked || '',
            formatNumber(item.daily_wage),
            formatNumber(item.total_Ot_Amount),
            formatNumber(item.grossPay),
            formatNumber(item.totalDeductions),
            formatNumber(item.netPay)
        ]);

        $dataTable.clear().rows.add(rows).draw();
    }

    function formatNumber(value) {
        return value != null ? parseFloat(value).toFixed(2) : '0.00';
    }

    $('#applyEntityFilterBtn').on('click', function () {
        if (!validateFilterForm()) return;
        fetchSalarySlipReport().then(() => {
            const modal = bootstrap.Modal.getInstance(document.getElementById('standardFilterModal'));
            if (modal) modal.hide();
        });
    });

    $companyDropdown.on('change', function () {
        const companyId = $(this).val();
        if (!companyId) return;
        fetchPreviousMonthYearForReport(companyId)
            .then(() => fetchContractors(companyId))
            .then(() => fetchLocations(companyId));
    });

    $contractorDropdown.on('change', function () {
        const companyId = $companyDropdown.val();
        if (companyId) fetchLocations(companyId);
    });

    // Initial load (Only once)
    fetchCompanies(sessionCompanyId, () => {
        $companyDropdown.val(sessionCompanyId);
        fetchPreviousMonthYearForReport(sessionCompanyId)
            .then(() => fetchContractors(sessionCompanyId))
            .then(() => fetchLocations(sessionCompanyId));
    });
});