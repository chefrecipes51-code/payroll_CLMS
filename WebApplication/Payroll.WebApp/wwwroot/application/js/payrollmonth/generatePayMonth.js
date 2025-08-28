/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 14-April-2025
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>
$(document).ready(function () {
    //$("#fdate1").datepicker("destroy"); 
    // Load company list on page load
    bindCompanyDropdown();   
    $('.calculated-days-header, .daysCount').hide();
});
function formatDateForBackend(dateStr) {
    const date = new Date(dateStr);
    const yyyy = date.getFullYear();
    const mm = String(date.getMonth() + 1).padStart(2, '0');
    const dd = String(date.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
}

//// Handle Company Selection
$(document).on('click', '#dropdownMenuCompany .dropdown-item', function () {
    const selectedName = $(this).data('name');
    const selectedId = $(this).data('id');
    $('#dropdownCompany').text(selectedName);
    $('#SelectedCompanyId').val(selectedId);
    $('#dropdownFinancialYear').text('Loading...'); 
    clearFinancialInputs();

    $.ajax({
        url: '/PayrollPeriod/getcompanyfinYeardetailsbyid',
        type: 'GET',
        data: { companyId: selectedId },
        success: function (response) {
            if (response && response.success && response.result) {


                ////////////
                const companyIdForMoth = $('#SelectedCompanyId').val();
                const backendStartDate = formatDateForBackend(response.result.startDate);
                console.log(response.result.startDate);
                $.ajax({
                    url: '/PayrollPeriod/GetPayMonthBySDate',
                    type: 'POST',
                    data: JSON.stringify({ companyId: companyIdForMoth, selectedDate: backendStartDate }),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (response) {
                        if (response.success) {
                            populatePayrollDates(response);
                        } else {
                            showAlert("danger", response.message);
                            setTimeout(function () {
                                window.location.href = '/PayrollPeriod/Index';
                            }, 5500);
                        }
                    },

                    error: function () {
                        console.error("Error fetching payroll month.");
                    }
                });
                /////////////////
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

                /////////////////////////////////////////////FETCH 12 RECORD
             

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
});
//$(document).on('change', '#fdate1', function () {
//    const selectedDateStr = $(this).val();
//    const startDateStr = $('#comFinYearSDate').val();
//    const endDateStr = $('#comFinYearEDate').val();

//    if (!isWithinFinancialYear(selectedDateStr, startDateStr, endDateStr)) {
//        showAlert("danger", "Selected date is outside the financial year range.");

//        const selectedDate = new Date(selectedDateStr);
//        const startDate = new Date(startDateStr);
//        const endDate = new Date(endDateStr);

//        var correctedDate;
//        if (selectedDate < startDate) {
//            correctedDate = startDateStr;
//        } else {
//            correctedDate = endDateStr;
//        }
//        $(this).val(correctedDate);
//        return;
//    }
//    const companyId = $('#SelectedCompanyId').val();
//    const selectedDateToPassStr = $(this).val();
//    const parts = selectedDateToPassStr.split('/');
//    const finalDate = `${parts[2]}-${parts[0].padStart(2, '0')}-${parts[1].padStart(2, '0')}`;
//    $.ajax({
//        url: '/PayrollMonth/GetPayMonthBySDate',
//        type: 'POST',
//        data: JSON.stringify({ companyId: companyId, selectedDate: finalDate }),
//        contentType: 'application/json; charset=utf-8',
//        dataType: 'json',
//        success: function (response) {           
//            if (response.success) {
//                populatePayrollDates(response);
//            } else {
//                showAlert("danger", response.message);
//                setTimeout(function () {
//                    window.location.href = '/PayrollMonth/Index';
//                }, 5500);
//            }
//        },

//        error: function () {
//            console.error("Error fetching payroll month.");
//        }
//    });
//});


//// Save Payroll Period
$('#btnGeneratePayroll').click(function () {
    const companyId = $('#SelectedCompanyId').val();
    const startYear = $('#comFinYearSDate').val();
    const endYear = $('#comFinYearEDate').val(); // <-- Add this line!
    const periodFrom = $('#fdate1').val();
    const periodTo = $('#tdate1').val();

    if (!startYear || !endYear || !companyId || !periodFrom || !periodTo) {
        showAlert("danger", "Required fields are missing.");
        return;
    }

    $.ajax({
        url: '/PayrollPeriod/GeneratePayrollPeriod',
        type: 'POST',
        data: {
            PeriodType: `${startYear}-${endYear}`,
            CompanyId: companyId,
            PeriodFDate: periodFrom,
            PeriodEDate: periodTo
        },
        success: function (response) {
            showAlert(response.success ? "success" : "danger", response.message);
            if (response.type == 2) {
                for (var i = 1; i <= 12; i++) {
                    $(`#fdate${i}`).val('');
                    $(`#tdate${i}`).val('');
                    $(`#daysCount${i}`).hide(); 
                    $('.calculated-days-header').hide();
                }
            }
            else {
                setTimeout(function () {
                    window.location.href = '/PayrollPeriod/PayrollPeriodList';
                }, 5500);
            }
        },
        error: function (xhr, status, error) {
            showAlert("danger", error);
        }
    });
});

//// Utilities
function bindCompanyDropdown() {
    $.ajax({
        url: '/PayrollPeriod/GetCompanyProfilesListJson',
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
