$(document).ready(function () {
    const sessionCompanyId = $("#sessionCompanyId").val();

    $.getScript('/assets/src/js/select2.min.js', function () {
        $.getScript('/assets/src/custom-js/select2.js');
    });

    // Initial binding
    fetchCompanies(sessionCompanyId, function () {
        resetFilters();
        fetchPreviousMonthYear(sessionCompanyId).done(function () {
            fetchContractors(sessionCompanyId).done(function () {
                fetchLocations(sessionCompanyId);
            });
        });
    });

    function resetFilters() {
        $('#mapCompanyDropdown').val(sessionCompanyId).trigger('change');
        $('#mapContractorDropdown').val('').trigger('change');
        $('#mapLocationDropdown').val('').trigger('change');
        $('#edatef').val('');
        $('#monthName').val('');
        $('#year').val('');
        $(".input_error_msg").text('');

        const table = $('#lossdamage-list').DataTable();
        table.clear().draw();
    }

    // Dropdown population
    function populateSingleSelect(selector, data, valueField, textField, selectedValue = "") {
        const dropdown = $(selector);
        dropdown.empty().append('<option value="">Select</option>');

        $.each(data, function (i, item) {
            dropdown.append(
                $('<option></option>').val(item[valueField]).text(item[textField])
            );
        });

        dropdown.val(selectedValue).trigger('change');
        dropdown.select2({ placeholder: 'Select', allowClear: true, width: '100%', dropdownParent: $('#standardFilterModal') });
    }

    function populateMultiSelect(selector, data, valueField, textField, selectedValues = []) {
        const dropdown = $(selector);
        dropdown.empty();

        $.each(data, function (i, item) {
            dropdown.append(
                $('<option></option>').val(item[valueField]).text(item[textField])
            );
        });

        dropdown.val(selectedValues).trigger('change');
        dropdown.select2({ placeholder: 'Select', allowClear: true, multiple: true, width: '100%', dropdownParent: $('#standardFilterModal') });
    }

    // Fetch functions
    function fetchCompanies(selectedValue = '', callback = null) {
        $.ajax({
            url: '/DropDown/FetchCompaniesDropdown',
            type: 'GET',
            success: function (data) {
                populateSingleSelect('#mapCompanyDropdown', data, 'value', 'text', selectedValue);
                if (callback) setTimeout(callback, 200);
            },
            error: function () {
                console.error("Error fetching companies.");
            }
        });
    }

    function fetchContractors(companyId) {
        return $.ajax({
            url: `/DropDown/FetchContractorDropdown?contractor_ID=0&company_ID=${companyId}`,
            type: 'GET',
            success: function (data) {
                populateMultiSelect('#mapContractorDropdown', data, 'value', 'text');
            },
            error: function () {
                console.error("Error fetching contractors.");
            }
        });
    }

    function fetchLocations(companyId) {
        return $.ajax({
            url: `/DropDown/FetchDistinctLocationDropdown?companyId=${companyId}`,
            type: 'GET',
            success: function (data) {
                populateMultiSelect('#mapLocationDropdown', data, 'value', 'text');
            },
            error: function () {
                console.error("Error fetching locations.");
            }
        });
    }

    function fetchPreviousMonthYear(companyId) {
        return $.ajax({
            url: '/ContractorValidation/GetPreviousMonthYearPeriod',
            type: 'GET',
            data: { companyId }
        }).done(function (res) {
            if (res.isSuccess && res.data) {
                $('#hiddenMonthId').val(res.data.month_Id);
                $('#monthName').val(res.data.monthName);
                $('#year').val(res.data.year);
            } else {
                alert(res.message || "Failed to fetch month/year.");
            }
        }).fail(function () {
            alert("Error fetching month/year.");
        });
    }

    // Fetch Loss/Damage Report
    function fetchLossDamageReport() {
        const companyID = parseInt($('#mapCompanyDropdown').val()) || 0;
        const contractorIDs = $('#mapContractorDropdown').val() || [];
        const locationIDs = $('#mapLocationDropdown').val() || [];
        const payrollMonth = parseInt($('#hiddenMonthId').val()) || 0;
        const payrollYear = parseInt($('#year').val()) || 0;

        let fyStart = null;
        const fyRaw = $('#edatef').val();
        if (fyRaw) {
            const [dd, mm, yyyy] = fyRaw.split('/');
            fyStart = `${yyyy}-${mm}-${dd}`;
        }

        const payload = {
            CompanyID: companyID,
            CompanyLocationIDs: locationIDs.join(','),
            ContractorIDs: contractorIDs.join(','),
            EntityIDs: null,
            PayrollMonth: payrollMonth,
            PayrollYear: payrollYear,
            FinancialYearStart: fyStart
        };

        console.log("Fetching loss/damage report:", payload);

        return $.ajax({
            url: '/Report/FetchLossDamageRegisterReport',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(payload),
            success: function (res) {
                console.log("✅ Success response:", res);

                if (Array.isArray(res)) {
                    populateLossDamageTable(res);
                } else if (res.isSuccess && Array.isArray(res.data)) {
                    populateLossDamageTable(res.data);
                } else {
                    console.warn(res.message || "No data found.");
                }
            },
            error: function (xhr, status, error) {
                console.error("❌ AJAX Error:", error);
                console.log("XHR:", xhr);
                console.log("Status:", status);
            }
        });
    }
    function populateLossDamageTable(data) {
        const table = $('#lossdamage-list').DataTable();
        table.clear();

        const rows = data.map(item => [
            item.entity_Code || '',
            item.entity_Name || '',
            item.father_Name || '',
            item.skillcategory_Name || '',
            item.type || '',
            item.damage_Date ? formatDate(item.damage_Date) : '',
            item.name_Of_WitNess || '',
            formatNumber(item.total_Amount),
            item.no_Of_Installment || '',
            item.recoveryDate || '',
            item.remarks || '',
            formatNumber(item.emI_Amount)
        ]);

        table.rows.add(rows).draw();
    }
    function formatDate(dateString) {
        if (!dateString) return '';
        const date = new Date(dateString);
        const day = String(date.getDate()).padStart(2, '0');
        const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are 0-indexed
        const year = date.getFullYear();
        return `${day}/${month}/${year}`;
    }

    function formatNumber(val) {
        return val !== null && val !== undefined ? parseFloat(val).toFixed(2) : "0.00";
    }

    // Filter change handlers
    $(document).on('change', '#mapCompanyDropdown', function () {
        const companyId = $(this).val();
        if (companyId) {
            fetchPreviousMonthYear(companyId).done(() => {
                fetchContractors(companyId).done(() => {
                    fetchLocations(companyId);
                });
            });
        }
    });

    $(document).on('change', '#mapContractorDropdown', function () {
        const companyId = $('#mapCompanyDropdown').val();
        if (companyId) fetchLocations(companyId);
    });

    $(document).on('click', '#resetEntityFilterBtn', function () {
        resetFilters();
    });

    $(document).on('click', '#applyEntityFilterBtn', function () {
        fetchLossDamageReport().then(() => {
            const modal = bootstrap.Modal.getInstance(document.getElementById('standardFilterModal'));
            if (modal) modal.hide();
        });
    });

    // Export
    $(document).on('click', '.export-data', function (e) {
        e.preventDefault();
        const type = $(this).data('type');
        const tableId = 'lossdamage-list';

        if ($('#' + tableId).DataTable().rows().count() === 0) {
            alert("No data available for export.");
            return;
        }

        switch (type) {
            case 'csv':
                exportTableToCSV(tableId);
                break;
            case 'xlsx':
                exportTableToXLSX(tableId);
                break;
            case 'pdf':
                exportTableToPDF(tableId);
                break;
        }
    });

    function exportTableToCSV(tableId) {
        const headers = [
            "Entity Code", "Entity Name", "Father Name", "Skill Name", "Type", "Damage Date",
            "Name Of Witness", "Total Amount", "No. of Installments", "Recovery Date", "Remarks", "EMI Amount"
        ];
        const rows = $('#' + tableId).DataTable().rows().data().toArray();
        const csv = [headers.join(',')].concat(rows.map(r => r.join(','))).join('\n');

        const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
        const link = document.createElement("a");
        link.href = URL.createObjectURL(blob);
        link.download = "LossDamageReport.csv";
        link.click();
    }

    function exportTableToXLSX(tableId) {
        const headers = [
            "Entity Code", "Entity Name", "Father Name", "Skill Name", "Type", "Damage Date",
            "Name Of Witness", "Total Amount", "No. of Installments", "Recovery Date", "Remarks", "EMI Amount"
        ];
        const rows = $('#' + tableId).DataTable().rows().data().toArray();

        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.aoa_to_sheet([headers, ...rows]);
        XLSX.utils.book_append_sheet(wb, ws, "LossDamage");
        XLSX.writeFile(wb, "LossDamageReport.xlsx");
    }

    function exportTableToPDF(tableId) {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();
        const headers = [
            "Entity Code", "Entity Name", "Father Name", "Skill Name", "Type", "Damage Date",
            "Name Of Witness", "Total Amount", "No. of Installments", "Recovery Date", "Remarks", "EMI Amount"
        ];
        const rows = $('#' + tableId).DataTable().rows().data().toArray();

        doc.text("Loss/Damage Register", 14, 10);
        doc.autoTable({
            startY: 20,
            head: [headers],
            body: rows,
            theme: 'grid',
            styles: { fontSize: 6, cellPadding: 1 }
        });

        doc.save("LossDamageReport.pdf");
    }
});
