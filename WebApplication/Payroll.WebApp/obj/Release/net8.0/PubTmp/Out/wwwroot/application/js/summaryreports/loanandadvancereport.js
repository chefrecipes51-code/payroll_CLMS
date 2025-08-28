$(document).ready(function () {
    const $companyDropdown = $('#mapCompanyDropdown');
    const $contractorDropdown = $('#mapContractorDropdown');
    const $locationDropdown = $('#mapLocationDropdown');
    const $edatef = $('#edatef');
    const $monthName = $('#monthName');
    const $year = $('#year');
    const $hiddenMonthId = $('#hiddenMonthId');
    const sessionCompanyId = $("#sessionCompanyId").val();
    const $dataTable = $('#loanandadvancereport-list').DataTable();

    // 1. Load Select2 Scripts
    $.getScript('/assets/src/js/select2.min.js', function () {
        $.getScript('/assets/src/custom-js/select2.js');
    });

    // 2. Clear Error on Dropdown Change
    $(document).on('change', '#mapCompanyDropdown, #mapContractorDropdown', function () {
        $(`#${this.id}-error`).text('');
    });

    // 3. Reset Filters Function (but NOT trigger change on Company)
    function resetAllFilters() {
        $companyDropdown.val(sessionCompanyId);
        $contractorDropdown.val('').trigger('change');
        $locationDropdown.val('').trigger('change');
        $edatef.val('').trigger('change');
        $monthName.val('');
        $year.val('');
        $(".input_error_msg").text('');
        $dataTable.clear().draw();
    }

    $('#resetFilterBtn').on('click', resetAllFilters);

    // 4. Validate Filter Form
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

    // 5. Dropdown Helper Functions
    function populateSingleSelectDropdown(selector, data, valueField, textField, selectedValue = "") {
        const $dropdown = $(selector).empty().append('<option value="">Select</option>');
        data?.forEach(item => {
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
        data?.forEach(item => {
            $dropdown.append(new Option(item[textField], item[valueField]));
        });

        $dropdown.val(selectedValues).trigger('change');

        if ($.fn.select2) {
            $dropdown.select2("destroy").select2({
                placeholder: $dropdown.data('placeholder') || 'Select',
                allowClear: true,
                width: '100%',
                multiple: true,
                dropdownAutoWidth: true,
                dropdownParent: $('#standardFilterModal')
            });
        }
    }

    // 6. Fetch Functions
    function fetchCompanies(selectedValue = '', callback = null) {
        $.get('/DropDown/FetchCompaniesDropdown')
            .done(data => {
                populateSingleSelectDropdown('#mapCompanyDropdown', data, 'value', 'text', selectedValue);
                if (typeof callback === 'function') setTimeout(callback, 200);
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
            .done(res => {
                if (res.isSuccess && res.data) {
                    $hiddenMonthId.val(res.data.month_Id);
                    $year.val(res.data.year);
                    $monthName.val(res.data.monthName);
                } else {
                    alert(res.message || "Unable to fetch pay period.");
                }
            })
            .fail(() => alert("Error fetching previous month/year."));
    }

    function fetchLoanandAdvanceReport() {
        const requestData = {
            CompanyID: parseInt($companyDropdown.val()) || 0,
            CompanyLocationIDs: null,
            ContractorIDs: ($contractorDropdown.val() || []).join(','),
            EntityIDs: null,
            PayrollMonth: parseInt($hiddenMonthId.val()) || 0,
            PayrollYear: parseInt($year.val()) || 0,
            FinancialYearStart: formatDateInput($edatef.val())
        };

        return $.ajax({
            url: '/RegisterWagesReport/GetLoanandAdvanceReport',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(requestData)
        })
            .done(response => {
                if (response?.isSuccess && Array.isArray(response.data)) {
                    populateLoanandAdvanceTable(response.data);
                } else {
                    showAlert("danger", response.message);
                }
            })
            .fail(() => alert("Error fetching report."));
    }

    function populateLoanandAdvanceTable(data) {
        const formattedRows = data.map(item => [
            item.entity_Code || '',
            item.entity_Name || '',
            item.father_Name || '',
            item.skillcategory_Name || '',
            formatNumber(item.wage_Payable),
            formatNumber(item.loan_Amount),
            formatDate(item.issue_Date),
            item.purpose_of_loan || '',
            item.no_Of_Instalments || 0,
            formatNumber(item.monthly_Installment),
            formatDate(item.instalment_St_Date),
            formatDate(item.last_Instalment_Paymengt_Dt)
        ]);

        $dataTable.clear().rows.add(formattedRows).draw();
    }

    function formatNumber(value) {
        return value ? parseFloat(value).toFixed(2) : "0.00";
    }

    function formatDate(date) {
        return date ? new Date(date).toLocaleDateString('en-GB') : '';
    }

    function formatDateInput(dateStr) {
        if (!dateStr) return null;
        const parts = dateStr.split('/');
        return parts.length === 3 ? `${parts[2]}-${parts[1]}-${parts[0]}` : null;
    }

    // 7. Event Bindings
    $('#applyEntityFilterBtn').on('click', function () {
        if (!validateFilterForm()) return;

        fetchLoanandAdvanceReport().then(() => {
            const modal = bootstrap.Modal.getInstance(document.getElementById('standardFilterModal'));
            if (modal) modal.hide();
        });
    });

    $(document).on('click.export', '.export-data', function (e) {
        e.preventDefault();
        const type = $(this).data('type');
        const tableId = $(this).data('table-id');

        const rows = $dataTable.rows().count();
        if (rows === 0) return alert("No data available for export.");

        const fileName = `${tableId}-export`;
        switch (type) {
            case 'csv': exportFineToCSV(fileName); break;
            case 'xlsx': exportFineToXLSX(fileName); break;
            case 'pdf': exportFineToPDF(fileName); break;
            default: alert("Export type not supported.");
        }
    });

    // 8. Export Helpers
    function exportFineToCSV(fileName) {
        const rows = $dataTable.rows().data().toArray();
        const headers = ["Entity Code", "Entity Name", "Father Name", "Skill Category", "Wage Payable", "Loan Amount", "Issue Date", "Purpose", "Installments", "Monthly Installment", "Start Date", "Last Payment"];
        const csv = headers.join(',') + '\n' + rows.map(r => r.join(',')).join('\n');
        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = `${fileName}.csv`;
        link.click();
    }

    function exportFineToXLSX(fileName) {
        const headers = ["Entity Code", "Entity Name", "Father Name", "Skill Category", "Wage Payable", "Loan Amount", "Issue Date", "Purpose", "Installments", "Monthly Installment", "Start Date", "Last Payment"];
        const rows = $dataTable.rows().data().toArray();
        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.aoa_to_sheet([headers, ...rows]);
        XLSX.utils.book_append_sheet(wb, ws, "Report");
        XLSX.writeFile(wb, `${fileName}.xlsx`);
    }

    function exportFineToPDF(fileName) {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();
        const headers = ["Entity Code", "Entity Name", "Father Name", "Skill Category", "Wage Payable", "Loan Amount", "Issue Date", "Purpose", "Installments", "Monthly Installment", "Start Date", "Last Payment"];
        const rows = $dataTable.rows().data().toArray();

        doc.text("Loan & Advance Report", 14, 10);
        doc.autoTable({
            startY: 20,
            head: [headers],
            body: rows,
            theme: 'grid',
            styles: { fontSize: 6, cellPadding: 1 }
        });

        doc.save(`${fileName}.pdf`);
    }

    // ✅ 9. Initialize Page (Only one call to `fetchPreviousMonthYearForReport`)
    fetchCompanies(sessionCompanyId, () => {
        $companyDropdown.val(sessionCompanyId);
        fetchPreviousMonthYearForReport(sessionCompanyId)
            .done(() => {
                fetchContractors(sessionCompanyId)
                    .done(() => fetchLocations(sessionCompanyId));
            });
    });

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

    $contractorDropdown.on('change', function () {
        const companyId = $companyDropdown.val();
        if (companyId) {
            fetchLocations(companyId);
        }
    });
});
