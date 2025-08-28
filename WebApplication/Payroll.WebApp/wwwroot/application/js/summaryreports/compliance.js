$(document).ready(function () {
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
                    // ✅ Initialize the table only once after DOM is ready
                    /*fetchComplianceReport();*/ // ✅ Fetch Compliance data only after all dropdowns & dates are ready
                });
            });
        });
    });
    function resetAllFilters() {
        $('#mapCompanyDropdown').val(sessionCompanyId).trigger('change');
        $('#mapContractorDropdown').val('').trigger('change');
        $('#mapLocationDropdown').val('').trigger('change');
        $('#edatef').val('').trigger('change');
        $('#monthName').val('');
        $('#year').val('');
        $(".input_error_msg").text('');

        // ✅ Clear DataTable rows as well
        const table = $('#compliancereport-list').DataTable();
        table.clear().draw();
    }

    $(document).on('click', '#resetFilterBtn', function () {
        resetAllFilters();
    });
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
    function fetchComplianceReport() {
        const companyID = parseInt($('#mapCompanyDropdown').val()) || 0;
        const contractorIDs = $('#mapContractorDropdown').val() || [];
        const locationIDs = $('#mapLocationDropdown').val() || [];
        const payrollMonth = parseInt($('#hiddenMonthId').val()) || 0;
        const payrollYear = parseInt($('#year').val()) || 0;
        const requestData = {
            CompanyID: companyID,
            CompanyLocationIDs: null,//locationIDs.join(','),
            ContractorIDs: contractorIDs.join(','),
            EntityIDs: null, // Replace with actual selected values if needed
            PayrollMonth: payrollMonth,
            PayrollYear: payrollYear,
        };

        console.log("requestData", requestData);
        return $.ajax({
            url: '/RegisterWagesReport/GetComplianceReport',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(requestData),
            success: function (response) {
                if (response && response.isSuccess && Array.isArray(response.data)) {
                    populateComplianceTable(response.data);
                } else {
                    console.warn(response.message || "No Compliance data found.");
                }
            },
            error: function () {
                alert("Error fetching Compliance report.");
            }
        });
    }
    function populateComplianceTable(data) {
        const table = $('#compliancereport-list').DataTable();
        table.clear();
        const formattedRows = data.map(item => [
            item.entity_Code || '',
            item.entity_Name || '',
            item.skillcategory_Name || '',
            item.esic_No || '',
            item.pf_No || '',
            item.contractor_Name || '',
            item.month_Id || '',
            formatNumber(item.labour_Fine),
            formatNumber(item.esI_Employer_Contribution),
            formatNumber(item.esI_Employee_Contribution),
            formatNumber(item.pf_Employeer_Contribution),
            formatNumber(item.pf_Employee_Contribution),
        ]);

        table.rows.add(formattedRows).draw();
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
                console.log(data);
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
                                    //fetchFineRegisterReport(); // Final fetch
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
        fetchComplianceReport().then(() => {
            const modalEl = document.getElementById('standardFilterModal');
            const modalInstance = bootstrap.Modal.getInstance(modalEl);
            if (modalInstance) {
                modalInstance.hide();
            }
        });
    });
    $(document).on('click.export', '.export-data', function (e) {
        e.preventDefault();
        const type = $(this).data('type');
        const tableId = $(this).data('table-id');

        if (tableId === 'compliancereport-list') {
            const rowCount = $('#compliancereport-list').DataTable().rows().count();
            if (rowCount === 0) {
                alert("No data available for export.");
                return;
            }

            switch (type) {
                case 'csv':
                    exportFineToCSV(tableId + '-export');
                    break;
                case 'xlsx':
                    exportFineToXLSX(tableId + '-export');
                    break;
                case 'pdf':
                    exportFineToPDF(tableId + '-export');
                    break;
                default:
                    alert("Export type not supported.");
            }
        } else {
            // fallback or handle other tables
            exportData(type, tableId);
        }
    });
    function exportFineToCSV(fileName) {
        const headers = [
            "Entity Code", "Entity Name", "Skill Category", "Esic_No", "PF_No",
            "Contractor_Name", "Month_Id", "Labour_Fine", "ESI_Employer_Contribution",
            "ESI_Employee_Contribution", "PF_Employeer_Contribution", "PF_Employee_Contribution"
        ];

        const rows = $('#compliancereport-list').DataTable().rows().data().toArray();

        const csvRows = rows.map(r => r.join(','));
        const csvContent = headers.join(',') + '\n' + csvRows.join('\n');
        const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });

        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName + '.csv';
        link.click();
    }
    function exportFineToXLSX(fileName) {
        const headers = [
            "Entity Code", "Entity Name", "Skill Category", "Esic_No", "PF_No",
            "Contractor_Name", "Month_Id", "Labour_Fine", "ESI_Employer_Contribution",
            "ESI_Employee_Contribution", "PF_Employeer_Contribution", "PF_Employee_Contribution"
        ];

        const rows = $('#compliancereport-list').DataTable().rows().data().toArray();

        const wb = XLSX.utils.book_new();
        const ws = XLSX.utils.aoa_to_sheet([headers, ...rows]);
        XLSX.utils.book_append_sheet(wb, ws, "Compliance");
        XLSX.writeFile(wb, fileName + '.xlsx');
    }
    function exportFineToPDF(fileName) {
        const { jsPDF } = window.jspdf;
        const doc = new jsPDF();

        const headers = [
            "Entity Code", "Entity Name", "Skill Category", "Esic_No", "PF_No",
            "Contractor_Name", "Month_Id", "Labour_Fine", "ESI_Employer_Contribution",
            "ESI_Employee_Contribution", "PF_Employeer_Contribution", "PF_Employee_Contribution"
        ];

        const rows = $('#compliancereport-list').DataTable().rows().data().toArray();

        doc.text("Compliance Report", 14, 10);
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
