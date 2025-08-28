/// <summary>
/// Developer Name :- Abhishek Yadav
///  Created Date  :- 26-05-2025
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>

$(document).ready(function () {
    $.ajax({
        url: '/AttendancePayrollMonth/GetCompanyProfilesListJson',
        type: 'GET',
        success: function (data) {
            if (data && data.length > 0) {
                var dropdownMenu = $('#dropdownMenuCompany');
                dropdownMenu.empty(); // Clear existing
                $.each(data, function (index, company) {
                    var listItem = $('<li>').append(
                        $('<a>')
                            .addClass('dropdown-item')
                            .attr('href', '#')
                            .attr('data-id', company.company_Id)
                            .attr('data-name', company.companyName)
                            .text(company.companyName)
                    );
                    dropdownMenu.append(listItem);
                });
            }
        },
        error: function () {
            //console.error('Failed to load company list.');
        }
    });
    $(document).on('click', '#dropdownMenuCompany .dropdown-item', function () {
        var selectedName = $(this).data('name');
        var selectedId = $(this).data('id');

        $('#dropdownCompany').text(selectedName);
        $('#SelectedCompanyId').val(selectedId);
    });
});
function formatDateToInput(date) {
    const yyyy = date.getFullYear();
    const mm = String(date.getMonth() + 1).padStart(2, '0');
    const dd = String(date.getDate()).padStart(2, '0');
    return `${yyyy}-${mm}-${dd}`;
}
function calculateEndDate(startDateStr) {
    const startDate = new Date(startDateStr);
    const endDate = new Date(startDate);
    endDate.setMonth(endDate.getMonth() + 1); 
    endDate.setDate(endDate.getDate() - 1);  
    return formatDateToInput(endDate);
}
function isWithinFinancialYear(dateStr, startStr, endStr) {
    const date = new Date(dateStr);
    const start = new Date(startStr);
    const end = new Date(endStr);
    return date >= start && date <= end;
}
$(document).on('click', '#dropdownMenuCompany .dropdown-item', function () {
    const selectedName = $(this).data('name');
    const selectedId = $(this).data('id');
    $('#dropdownCompany').text(selectedName);
    $('#SelectedCompanyId').val(selectedId);
    $.ajax({
        url: '/AttendancePayrollMonth/getcompanyfinYeardetailsbyid?companyId=' + selectedId,
        type: 'GET',
        success: function (response) {
            if (response && response.success) {
                const result = response.result;               
                const start = new Date(result.startDate);
                const end = new Date(result.endDate);
                const formatDate = (date) => {
                    const yyyy = date.getFullYear();
                    const mm = String(date.getMonth() + 1).padStart(2, '0');
                    const dd = String(date.getDate()).padStart(2, '0');
                    return `${yyyy}-${mm}-${dd}`;
                };
                const formatYearDate = (date) => {
                    const yyyy = date.getFullYear();                  
                    return `${yyyy}`;
                };
                $('#txtFinancialSDate').val(formatDate(start));
                $('#txtFinancialSYear').val(formatYearDate(start));
                $('#txtFinancialEDate').val(formatDate(end));
                $('#txtFinancialEYear').val(formatYearDate(end));
                const formattedStart = formatDate(start);
                $('#comFinYearSDate').val(formattedStart);
                const calculatedEnd = calculateEndDate(formattedStart);
                $('#comFinYearEDate').val(calculatedEnd);     
            } else {
                $('#txtFinancialSDate, #txtFinancialEDate').val('');
                showAlert("danger", response.message || 'Data not found.');               
            }
        },
        error: function () {
            $('#txtFinancialSDate, #txtFinancialEDate').val('');
            //alert('Error fetching company financial year.');
            showAlert("danger", 'Error fetching company financial year.'); 
        }
    });
});
$(document).on('change', '#comFinYearSDate', function () {
    var newStartDate = $(this).val();
    var finStart = $('#txtFinancialSDate').val();
    var finEnd = $('#txtFinancialEDate').val();
    if (!isWithinFinancialYear(newStartDate, finStart, finEnd)) {        
        showAlert("danger", "Selected date must be within the financial year!");      
        $(this).val('');
        $('#comFinYearEDate').val('');
        return;
    }
    var newEndDate = calculateEndDate(newStartDate);
    $('#comFinYearEDate').val(newEndDate);
});

$('#btnGeneratePayroll').click(function () {
    var startYear = $('#txtFinancialSYear').val();
    var endYear = $('#txtFinancialEYear').val();
    var companyId = $('#SelectedCompanyId').val();
    var periodFrom = $('#comFinYearSDate').val();
    var periodTo = $('#comFinYearEDate').val();
    if (!startYear || !endYear || !companyId || !periodFrom || !periodTo) {
        //alert("Required fields are missing.");
        showAlert("danger", "Required fields are missing.");  
        return;
    }
    $.ajax({
        url: '/AttendancePayrollMonth/GeneratePayrollPeriod',
        type: 'POST',
        data: {
            PeriodType: `${startYear}-${endYear}`,
            CompanyId: companyId,
            PeriodFDate: periodFrom,
            PeriodEDate: periodTo
        },
        success: function (response) {           
            if (response.success) {               
                showAlert("success", response.message);
            } else {              
                showAlert("danger", response.message);  
            }
        },
        error: function (xhr, status, error) {
            //console.error(error);
            showAlert("danger", error);
           // alert("Something went wrong.");
        }
    });
});
