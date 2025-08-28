/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 15-April-2025
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>

$(document).ready(function () {
    ////////////////////// Bind Company List:- Start////////////////////////////////////
    $.ajax({
        url: '/PayrollPeriod/GetCompanyProfilesListJson',
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
            console.error('Failed to load company list.');
        }
    }); 
    $(document).on('click', '#dropdownMenuCompany .dropdown-item', function () {
        var selectedName = $(this).data('name');
        var selectedId = $(this).data('id');

        $('#dropdownCompany').text(selectedName);
        $('#SelectedCompanyId').val(selectedId);

       // console.log('Clearing table...');
        $('#payroll-month-list tbody').empty().append('<tr><td colspan="6" class="text-center">No data available</td></tr>');

        // Clear previous pay group selection
        $('#dropdownPayGroup').text('Select Pay Group');
        $('#SelectedPayGroupId').val('');

        // Call Payroll Month Group API based on selected company ID
        $.ajax({
            url: '/PayrollPeriod/GetPayrollMonthGroupListJson?companyid=' + selectedId,
            type: 'GET',
            success: function (data) {
                var dropdownMenu = $('#dropdownMenuPayGroup');
                dropdownMenu.empty(); // Clear existing

                if (data && data.length > 0) {
                    $.each(data, function (index, item) {
                        var listItem = $('<li>').append(
                            $('<button>')
                                .addClass('dropdown-item')
                                .attr('type', 'button')
                                .attr('data-id', item.fYearDate)
                                .text(item.fYearDate)
                        );
                        dropdownMenu.append(listItem);
                    });
                } else {
                    dropdownMenu.append('<li><span class="dropdown-item text-muted">No pay group found</span></li>');
                }
            },
            error: function () {
                console.error('Failed to load payroll group.');
            }
        });
    });

    ////////////////////// Bind Company List:- End////////////////////////////////////

    ////////////////////// Bind Payroll Month Group List:- Start////////////////////////////////////
   
    $(document).on('click', '#dropdownMenuPayGroup .dropdown-item', function () {
        var selectedGroupName = $(this).text();
        var selectedGroupId = $(this).data('id');

        if (!selectedGroupId) {
            // If "No pay group found" clicked, just clear
            $('#dropdownPayGroup').text('Select Pay Group');
            $('#SelectedPayGroupId').val('');
            $('#payroll-month-list tbody').empty().append('<tr><td colspan="6" class="text-center">No data available</td></tr>');
            return;
        }

        $('#dropdownPayGroup').text(selectedGroupName);
        $('#SelectedPayGroupId').val(selectedGroupId);

        var selectedCompanyId = $('#SelectedCompanyId').val();

        if (selectedCompanyId && selectedGroupId) {
            fetchPayrollMonthList(selectedCompanyId, selectedGroupId);
        }
    });

    function fetchPayrollMonthList(companyId, payrollGroup) {
        $.ajax({
            url: '/PayrollPeriod/GetPayrollMonthListJson',
            type: 'GET',
            data: {
                companyid: companyId,
                payrollGroup: payrollGroup
            },
            success: function (data) {
                var tableBody = $('#payroll-month-list tbody');
                tableBody.empty(); // Clear existing rows

                if (data && data.length > 0) {
                    $.each(data, function (index, item) {
                        var row = `<tr>
                            <td>${item.period_Code}</td>
                            <td>${item.period_Name}</td>
                            <td>${item.customGroupName}</td>
                            <td>${formatDate(item.periodFrom_Date)}</td>
                            <td>${formatDate(item.periodTo_Date)}</td>
                            <td>${item.days}</td>
                        </tr>`;
                        tableBody.append(row);
                    });
                } else {
                    tableBody.append('<tr><td colspan="6" class="text-center">No data available</td></tr>');
                }
            },
            error: function () {
                showAlert("danger", "Error fetching data.");
            }
        });
    }

    function formatDate(dateString) {
        const date = new Date(dateString);
        if (!isNaN(date)) {
            return date.toLocaleDateString('en-GB'); // Format: dd/mm/yyyy
        }
        return dateString;
    }
    ////////////////////// Bind Payroll Month Group List:- End////////////////////////////////////
});