/// <summary>
/// Developer Name :- Abhishek Yadav
///  Created Date  :- 26-05-2025
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>
$(document).ready(function () {
    // Load company list on page load
    bindCompanyDropdown();   
    $('.calculated-days-header, .daysCount').hide();
});
//// Handle Company Selection
$(document).on('click', '#dropdownMenuCompany .dropdown-item', function () {
    
    const selectedName = $(this).data('name');
    const selectedId = $(this).data('id');
    $('#dropdownCompany').text(selectedName);
    $('#SelectedCompanyId').val(selectedId);
    $('#dropdownFinancialYear').text('Loading...'); 
    $('#dropdownContractor').text('Loading...');
    clearFinancialInputs();
    //new
    $('#SelectedContractorCode').val('');
    $('#dropdownMenuFinancialYear').empty();

    $.ajax({
        url: '/AttendancePayrollMonth/getcompanyfinYeardetailsbyid',
        type: 'GET',
        data: { companyId: selectedId },
        success: function (response) {
            if (response && response.success && response.result) {
                const start = new Date(response.result.startDate);
                const end = new Date(response.result.endDate);
                const financialYearText = `${start.getFullYear()}-${end.getFullYear()}`;
                const year = start.getFullYear();
                // Set financial year dropdown button text and hidden input
                $('#dropdownFinancialYear').text(financialYearText);
                $('#txtFinancialSYear').val(year);
                $('#dropdownMenuFinancialYear').html(
                   // `<li><a class="dropdown-item" href="#" data-value="${year}">${year}</a></li>`
                    `<li><a class="dropdown-item" href="#" data-value="${financialYearText}">${financialYearText}</a></li>`
                );
                $(document).off('click', '#dropdownMenuFinancialYear .dropdown-item');
                $(document).on('click', '#dropdownMenuFinancialYear .dropdown-item', function (e) {
                    e.preventDefault();
                    const selectedYear = $(this).data('value');
                    $('#dropdownFinancialYear').text(selectedYear);
                    $('#txtFinancialSYear').val(selectedYear);
                });
                const formattedStart = formatDateToInputToDisplay(response.result.startDate);  
                const formattedEnd = formatDateToInputToDisplay(response.result.endDate);
               // $('#fdate1').val(formattedStart);
                $('#comFinYearSDate').val(formattedStart);
                $('#comFinYearEDate').val(formattedEnd); 
            } else {
                clearFinancialInputs();
                showAlert("danger", response.message || 'Data not found.');
            }
        },
        error: function () {
            clearFinancialInputs();
            showAlert("danger", 'Error fetching company financial year.');
        }
    });
    // AJAX #2: Contractor
    // -----------------------
    $.ajax({
        url: '/DropDown/FetchContractorDropdownWithCode',
        type: 'GET',
        data: { company_Id: selectedId },
        success: function (response) {
            if (Array.isArray(response) && response.length > 0) {
                let contractorItems = '';
                for (let i = 0; i < response.length; i++) {
                    const c = response[i];
                    contractorItems += `<li><a class="dropdown-item" href="#" data-value="${c.value}" data-name="${c.text}">${c.text}</a></li>`;
                }

                $('#dropdownMenuContractor').html(contractorItems);

                // Contractor dropdown <a> has id="dropdownFinancialYear" (as per your HTML)
                $('#dropdownContractorData').text('Select Contractor');
                $('#SelectedContractorCode').val('');

                // Bind click for contractor items
                //$(document).off('click', '#dropdownMenuContractor .dropdown-item');
                //$(document).on('click', '#dropdownMenuContractor .dropdown-item', function (e) {
                //    e.preventDefault();
                //    const selectedContractorName = $(this).data('name');
                //    const selectedContractorCode = $(this).data('code');

                //    $('#dropdownContractorData').text(selectedContractorName); // still using incorrect id
                //    $('#SelectedContractorCode').val(selectedContractorCode);
                //});
            } else {
                $('#dropdownContractorData').text('No Contractors Found');
                $('#SelectedContractorCode').val('');
            }
        },
        error: function () {
            $('#dropdownContractor').text('Error loading contractors');
            $('#SelectedContractorCode').val('');
        }
    });
});
$(document).on('change', '#fdate1', function () {
    const selectedDateStr = $(this).val();
    const startDateStr = $('#comFinYearSDate').val();
    const endDateStr = $('#comFinYearEDate').val();

    if (!isWithinFinancialYear(selectedDateStr, startDateStr, endDateStr)) {
        showAlert("danger", "Selected date is outside the financial year range.");

        const selectedDate = new Date(selectedDateStr);
        const startDate = new Date(startDateStr);
        const endDate = new Date(endDateStr);

        var correctedDate;
        if (selectedDate < startDate) {
            correctedDate = startDateStr;
        } else {
            correctedDate = endDateStr;
        }
        $(this).val(correctedDate);
        return;
    }
    const companyId = $('#SelectedCompanyId').val();
    const contractorcode = $('#SelectedContractorCode').val();
    const workordercode = $('#SelectedWorkorderId').val();
    const selectedDateToPassStr = $(this).val();
    const parts = selectedDateToPassStr.split('/');
    const finalDate = `${parts[2]}-${parts[0].padStart(2, '0')}-${parts[1].padStart(2, '0')}`;
    $.ajax({
        url: '/AttendancePayrollMonth/GetAttendancePayMonthBySDate',
        type: 'POST',
        data: JSON.stringify({ companyId: companyId, selectedDate: finalDate, Contractor_Code: contractorcode, WorkOrder_Code: workordercode }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (response) {           
            if (response.success) {
                populatePayrollDates(response);
            } else {
                showAlert("danger", response.message);
                setTimeout(function () {
                    window.location.href = '/AttendancePayrollMonth/Index';
                }, 5500);
            }
        },

        error: function () {
            console.error("Error fetching payroll month.");
        }
    });
});

//Bind WorkOrder dropdown on contractor change
$(document).on('click', '#dropdownMenuContractor .dropdown-item', function () {
    const contractorName = $(this).text();
    const contractorCode = $(this).attr('data-value');

    // Update selected contractor in UI and hidden input
    $('#dropdownContractorData').text(contractorName); // <-- FIXED ID
    $('#SelectedContractorCode').val(contractorCode);

    // Reset work order UI
    $('#dropdownWorkOrder').text('Loading...');
    $('#dropdownMenuWorkOrder').empty();
    $('#SelectedWorkorderId').val('');

    // Fetch work orders for selected contractor
    $.ajax({
        url: '/DropDown/FetchWorkOrderByContractor',
        type: 'GET',
        data: { contractorCode: contractorCode },
        success: function (response) {
            if (response && response.length > 0) {
                let workOrderItems = '';
                for (let i = 0; i < response.length; i++) {
                    const item = response[i];
                    workOrderItems += `<li><a class="dropdown-item" href="#" data-id="${item.value}">${item.text}</a></li>`;
                }
                $('#dropdownMenuWorkOrder').html(workOrderItems);
                $('#dropdownWorkOrder').text('Select WorkOrder');

                // Bind workorder item click
                $(document).off('click', '#dropdownMenuWorkOrder .dropdown-item');
                $(document).on('click', '#dropdownMenuWorkOrder .dropdown-item', function (e) {
                    e.preventDefault();
                    const selectedText = $(this).text();
                    const selectedId = $(this).data('id');
                    $('#dropdownWorkOrder').text(selectedText);
                    $('#SelectedWorkorderId').val(selectedId);
                });
            } else {
                $('#dropdownWorkOrder').text('No WorkOrders');
                $('#dropdownMenuWorkOrder').html('');
                $('#SelectedWorkorderId').val('');
            }
        },
        error: function () {
            $('#dropdownWorkOrder').text('Error');
            $('#dropdownMenuWorkOrder').html('');
            $('#SelectedWorkorderId').val('');
            showAlert("danger", "Failed to load work orders.");
        }
    });
});

$('#btnGeneratePayroll').click(function () {
    debugger;
    const companyId = $('#SelectedCompanyId').val();
    const ContractorId = $('#SelectedContractorId').val();
    const startYear = $('#comFinYearSDate').val();
    const endYear = $('#comFinYearEDate').val(); // <-- Add this line!
    const periodFrom = $('#fdate1').val();
    const periodTo = $('#tdate1').val();
    const contractorcode = $('#SelectedContractorCode').val();
    const workordercode = $('#SelectedWorkorderId').val();

    if (!startYear || !endYear || !companyId || !periodFrom || !periodTo) {
        showAlert("danger", "Required fields are missing.");
        return;
    }

    $.ajax({
        url: '/AttendancePayrollMonth/GenerateAttendancePayrollPeriod',
        type: 'POST',
        data: {
            PeriodType: `${startYear}-${endYear}`,
            CompanyId: companyId,
            PeriodFDate: periodFrom,
            PeriodEDate: periodTo,
            Contractor_Code: contractorcode,
            WorkOrder_Code: workordercode
        },
        success: function (response) {
            showAlert(response.success ? "success" : "danger", response.message);
            if (response.success) {
                setTimeout(function () {
                    window.location.href = '/AttendancePayrollMonth/AttendancePayrollMonthList';
                }, 5500);
            }
            else {
                setTimeout(function () {
                    window.location.href = '/AttendancePayrollMonth/index';
                }, 2500);
            }
            //if (response.success) {
            //    for (let i = 1; i <= 12; i++) {
            //        $(`#fdate${i}`).val('');
            //        $(`#tdate${i}`).val('');
            //    }
            //    $('#comFinYearSDate').val('');
            //    $('#comFinYearEDate').val('');
            //    $('#SelectedCompanyId').val(''); // reset selected value
            //    $('#SelectedFinancialYear').val(''); // reset selected value
            //    bindCompanyDropdown();
            //}
        },
        error: function (xhr, status, error) {
            showAlert("danger", error);
        }
    });
});

//// Utilities
function bindCompanyDropdown() {
    $.ajax({
        url: '/AttendancePayrollMonth/GetCompanyProfilesListJson',
        type: 'GET',
        success: function (data) {
            if (data && data.length > 0) {
                const dropdownMenu = $('#dropdownMenuCompany');
                dropdownMenu.empty();
                $.each(data, function (index, company) {
                    $('<li>').append(
                        $('<a>')
                            .addClass('dropdown-item')
                            .attr('href', '#')
                            .attr('data-id', company.company_Id)
                            .attr('data-name', company.companyName)
                            .text(company.companyName)
                    ).appendTo(dropdownMenu);
                });
            }
        }
    });
}
function populatePayrollDates(data) {
    console.log(data.result);
    if (data.success && data.result && Array.isArray(data.result) && data.result.length === 12) {
        const fdate1 = $('#fdate1');
        fdate1.datepicker('hide');
        fdate1.blur();
        $('.calculated-days-header, .calculated-days-value').show();
        for (var i = 0; i < 12; i++) {
            const item = data.result[i];
            const fromDateFormatted = formatDateToDisplay(item.periodFrom_Date);
            const toDateFormatted = formatDateToDisplay(item.periodTo_Date);
            $(`#fdate${i + 1}`).val(fromDateFormatted);
            $(`#tdate${i + 1}`).val(toDateFormatted);
            $(`#daysCount${i + 1}`).text(item.days); 
        }
        $('html, body').animate({ scrollTop: fdate1.offset().top - 100 }, 300);
        setTimeout(() => fdate1.blur(), 50);
    } else {
        alert("Expected 12 records. Data is invalid or incomplete.");
    }
}
function formatDateToDisplay(isoDate) {
    if (!isoDate) return '';
    const date = new Date(isoDate);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0'); // getMonth() is 0-based
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;  // change to MM/dd/yyyy if needed
}
function formatDateToInput(date) {
    return date.toISOString().split('T')[0];
}
function formatDateToInputToDisplay(dateString) {
    return dateString.split('T')[0]; // just take "YYYY-MM-DD" from the original string
}
function calculateEndDate(startDateStr) {
    const start = new Date(startDateStr);
    const end = new Date(start);
    end.setMonth(end.getMonth() + 1);
    end.setDate(end.getDate() - 1);
    return formatDateToInput(end);
}
function isWithinFinancialYear(dateStr, startStr, endStr) {
    const date = new Date(dateStr);
    return date >= new Date(startStr) && date <= new Date(endStr);
}
function clearFinancialInputs() {
    $(' #txtFinancialSYear, #fdate1, #comFinYearSDate, #comFinYearEDate').val('');
    $('#dropdownFinancialYear').text('Please Select');
    $('#dropdownMenuFinancialYear').empty();
}
