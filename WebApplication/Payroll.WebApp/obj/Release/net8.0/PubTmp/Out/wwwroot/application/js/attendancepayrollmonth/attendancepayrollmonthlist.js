/// <summary>
/// Developer Name :- Abhishek Yadav
///  Created Date  :- 26-05-2025
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>

$(document).ready(function () {
    ////////////////////// Bind Company List:- Start////////////////////////////////////
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
            console.error('Failed to load company list.');
        }
    }); 
    $(document).on('click', '#dropdownMenuCompany .dropdown-item', function () {
        debugger;
        var selectedName = $(this).data('name');
        var selectedId = $(this).data('id');

        $('#dropdownCompany').text(selectedName);
        $('#SelectedCompanyId').val(selectedId);

       
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
    ////////////////////// Bind Company List:- End////////////////////////////////////
    ////////////////////// Bind Finanacial Group:- Start//////////////////////////////
    $(document).on('click', '#dropdownMenuWorkOrder .dropdown-item', function () {
        debugger;
        const selectedName = $(this).text();
        const selectedId = $(this).data('id');

        $('#dropdownWorkOrder').text(selectedName);
        $('#SelectedWorkorderId').val(selectedId);
        var CompanyId = $('#SelectedCompanyId').val();
        var contractorCode = $('#SelectedContractorCode').val();
        var workOrderCode = $('#SelectedWorkorderId').val();

        // Call Payroll Month Group API based on selected company ID
        $.ajax({
            url: '/AttendancePayrollMonth/GetPayrollMonthGroupListJson?companyid=' + CompanyId +
                '&Contractor_code=' + encodeURIComponent(contractorCode) +
                '&Workorder_code=' + encodeURIComponent(workOrderCode),
            type: 'GET',
            success: function (data) {
                var dropdownMenu = $('#dropdownMenuPayGroup');
                dropdownMenu.empty(); // Clear existing

                if (data && data.length > 0) {
                    $.each(data, function (index, item) {
                        var listItem = $('<li>').append(
                            $('<a>')
                                .addClass('dropdown-item')
                                .attr('href', '#')
                                .attr('data-id', item.fYearDate)
                                .text(item.fYearDate)
                        );
                        dropdownMenu.append(listItem);
                    });
                } else {
                    dropdownMenu.append('<li><span class="dropdown-item">No pay group found</span></li>');
                }
            },
            error: function () {
                console.error('Failed to load payroll group.');
            }
        });
    });

    /////////////////////  Bind WorkOrder On Contractor Change: - Start ////////////////////////////

    $(document).on('click', '#dropdownMenuContractor .dropdown-item', function () {
        debugger;
        const contractorName = $(this).text();
        const contractorCode = $(this).attr('data-value');

        // Update selected contractor in UI and hidden input
        $('#dropdownContractorData').text(contractorName); // <-- FIXED ID
        $('#SelectedContractorCode').val(contractorCode);

        // Reset work order UI
        $('#dropdownWorkOrder').text('Loading...');
        $('#dropdownMenuWorkOrder').empty();
       /* $('#SelectedWorkorderId').val('');*/

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
                    //$(document).off('click', '#dropdownMenuWorkOrder .dropdown-item');
                    //$(document).on('click', '#dropdownMenuWorkOrder .dropdown-item', function (e) {
                    //    e.preventDefault();
                    //    const selectedText = $(this).text();
                    //    const selectedId = $(this).data('id');
                    //    $('#dropdownWorkOrder').text(selectedText);
                    //    $('#SelectedWorkorderId').val(selectedId);
                    //});
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

    ////////////////////// Bind Payroll Month Group List:- Start////////////////////////////////////
   
    $(document).on('click', '#dropdownMenuPayGroup .dropdown-item', function () {
        var selectedGroupName = $(this).text();
        var selectedGroupId = $(this).data('id');

        $('#dropdownPayGroup').text(selectedGroupName);
        $('#SelectedPayGroupId').val(selectedGroupId);

        var selectedCompanyId = $('#SelectedCompanyId').val();
        var contractorcode = $('#SelectedContractorCode').val();
        var workordercode = $('#SelectedWorkorderId').val();

        if (selectedCompanyId && selectedGroupId && contractorcode && workordercode) {
            fetchPayrollMonthList(selectedCompanyId, contractorcode, workordercode, selectedGroupId);
        }
    });
    function fetchPayrollMonthList(companyId, contractorcode, workordercode, payrollGroup) {
        $.ajax({
            url: '/AttendancePayrollMonth/GetPayrollMonthListJson',
            type: 'GET',
            data: {
                companyid: companyId,
                ContractorCode: contractorcode,
                WorkOrderCode: workordercode,
                payrollGroup: payrollGroup
            },
            success: function (data) {
                var tableBody = $('#payroll-month-list tbody');
                tableBody.empty(); // Clear existing rows
                let totaldays = 0;
                
                if (data && data.length > 0) {
                    console.log(data);
                    $.each(data, function (index, item) {
                        totaldays += item.days;
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
                    // Append total days row
                    var totalRow = `<tr style="font-weight:bold;">
                    <td colspan="5" class="text-end">Total Days</td>
                     <td>${totaldays}</td>
                </tr>`;
                    tableBody.append(totalRow);
                } else {
                    tableBody.append('<tr><td colspan="5" class="text-center">No data available</td></tr>');
                }
            },
            error: function () {
                //alert('Error fetching data');
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