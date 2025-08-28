$(document).ready(function () {
    const sessionCompanyId = $("#sessionCompanyId").val();
    async function initializeFilters() {
        try {
            await fetchCompanies(sessionCompanyId);
            await fetchPreviousMonthYearForReport(sessionCompanyId);
            await fetchContractors(sessionCompanyId);
            await fetchLocations(sessionCompanyId);
        } catch (err) {
            console.error("Initialization error:", err);
            alert("Failed to initialize filters.");
        }
    }
    initializeFilters();

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

    function resetAllFilters() {
        $('#mapCompanyDropdown').val(sessionCompanyId).trigger('change');
        $('#mapContractorDropdown').val('').trigger('change');
        $('#mapLocationDropdown').val('').trigger('change');
        $('#edatef').val('').trigger('change');
        $('#monthName').val('');
        $('#year').val('');
        $(".input_error_msg").text('');
        $('#contractorpaymentreport-list').DataTable().clear().draw();
    }

    $('#resetFilterBtn').on('click', resetAllFilters);

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

    $('#applyEntityFilterBtn').on('click', function () {
        if (!validateFilterForm()) return;

        fetchContractorPaymentReport().then(() => {
            const modal = bootstrap.Modal.getInstance(document.getElementById('standardFilterModal'));
            if (modal) modal.hide();
        });
    });

    function fetchContractorPaymentReport() {
        const requestData = {
            CompanyID: parseInt($('#mapCompanyDropdown').val()) || 0,
            CompanyLocationIDs: ($('#mapLocationDropdown').val() || []).join(','),
            ContractorIDs: ($('#mapContractorDropdown').val() || []).join(','),
            EntityIDs: null,
            PayrollMonth: parseInt($('#hiddenMonthId').val()) || 0,
            PayrollYear: parseInt($('#year').val()) || 0,
            ProcessedDate: null,
            FinancialYearStart: formatDateToYMD($('#edatef').val())
        };

        return $.ajax({
            url: '/Report/GetContractorPaymentRegister',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(requestData),
            success: function (response) {
                console.log(response.data);
                if (response?.isSuccess && Array.isArray(response.data)) {
                    populateContractorPaymentTable(response.data);
                } else {
                    alert(response.message || "No data found.");
                }
            },
            error: function () {
                alert("Error fetching Contractor Payment Report.");
            }
        });
    }

    function populateContractorPaymentTable(data) {
        const table = $('#contractorpaymentreport-list').DataTable();
        table.clear();

        const rows = data.map(item => [
            item.contractor_Code || '',
            item.contractor_Name || '',
            item.payroll_MonthYear || '',
            formatNumber(item.total_Menpower),
            formatNumber(item.totalEarnings),
            formatNumber(item.totalDeductions),
            formatNumber(item.netPay)
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

    function fetchCompanies(selectedValue = '') {
        return $.get('/DropDown/FetchCompaniesDropdown', function (data) {
            populateSingleSelectDropdown('#mapCompanyDropdown', data, 'value', 'text', selectedValue);
        });
    }

    function fetchContractors(companyId) {
        return $.get(`/DropDown/FetchContractorDropdown?contractor_ID=0&company_ID=${companyId}`, function (data) {
            populateMultiSelectDropdown('#mapContractorDropdown', data, 'value', 'text');
        });
    }

    function fetchLocations(companyId) {
        return $.get(`/DropDown/FetchDistinctLocationDropdown?companyId=${companyId}`, function (data) {
            populateMultiSelectDropdown('#mapLocationDropdown', data, 'value', 'text');
        });
    }

    function fetchPreviousMonthYearForReport(companyId) {
        return $.get('/ContractorValidation/GetPreviousMonthYearPeriod', { companyId }, function (response) {
            if (response.isSuccess && response.data) {
                $('#hiddenMonthId').val(response.data.month_Id);
                $('#year').val(response.data.year);
                $('#monthName').val(response.data.monthName);
            } else {
                alert(response.message || "Unable to fetch pay period.");
            }
        });
    }
});
