$(document).ready(function () {

    ////////////////////////////////////////Checkbox Gets Checked or Un Checked:- Start//////////////////////////////////////////////////////
    $('#checkAll').on('change', function () {
        var isChecked = $(this).is(':checked');
        $('#approval-list tbody tr:visible input.row-checkbox').prop('checked', isChecked);
    });
    $('#approval-list tbody').on('change', 'input.row-checkbox', function () {
        const totalVisible = $('#approval-list tbody tr:visible input.row-checkbox').length;
        const totalCheckedVisible = $('#approval-list tbody tr:visible input.row-checkbox:checked').length;

        $('#checkAll').prop('checked', totalVisible === totalCheckedVisible);
    });
    ////////////////////////////////////////Checkbox Gets Checked or Un Checked:- End//////////////////////////////////////////////////////

    //////////////////////////////////////// PAGE LOAD EVENT Fire:- Start////////////////////////////////////////
   
    const today = new Date();
    const displayToday = today.toLocaleDateString('en-GB', {
        day: '2-digit',
        month: 'short',
        year: 'numeric'
    }).replace(/ /g, '-');
    $('#effectiveFromDtEdit').val(displayToday);
    $('#effectiveFromDtEdit').datepicker({
        format: 'dd-M-yy',
        autoclose: true
    }).on('changeDate', function () {
        fetchApprovals(); // This will now fire on date change
    });
    fetchApprovals(); // Trigger on page load
    //////////////////////////////////////// PAGE LOAD EVENT Fire:- End////////////////////////////////////////

    //////////////////////////Approve Or Reject User:- Start //////////////////////////
    //$('#btnVerifySelected').on('click', function () {
    //    const selected = getSelectedApprovals();
    //    if (selected.length === 0) {
    //        showAlert("danger", "Please select at least one record.");
    //        return;
    //    }
    //    // TODO: Call your API here to verify approval(s)
    //    console.log('Verify clicked:', selected);
    //});
    $('#btnVerifySelected').on('click', function () {
        const selected = getSelectedApprovals();
        if (selected.length === 0) {
            showAlert("danger", "Please select at least one record.");
            return;
        }
        verifyApprovals(selected); 
    });
    $('#btnReturnSelected').on('click', function () {
        const selected = getSelectedApprovals();
        if (selected.length === 0) {
            showAlert("danger", "Please select at least one record.");           
            return;
        }
        console.log('Return clicked:', selected);
        $('#rejectionUser').modal('show');       
    });
    function verifyApprovals(approvals) {
        $.ajax({
            url: '/ApprovalList/VerifyApprovalsFromUI', 
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(approvals),
            success: function (response) {
                showAlert("success", response.message || "Approvals verified.");
            },
            error: function () {
                showAlert("danger", "Something went wrong while verifying approvals.");
            }
        });
    }

    //////////////////////////Approve Or Reject User:- End //////////////////////////  
    function fetchApprovals() {
        var selectedDateText = $('#effectiveFromDtEdit').val();
        var selectedDate = selectedDateText
            ? formatDateForApiYYYYMMDD(new Date(selectedDateText))
            : formatDateForApiYYYYMMDD(new Date());

        $.ajax({
            url: '/ApprovalList/GetApprovalList',
            type: 'GET',
            data: { requestDate: selectedDate },
            success: function (response) {
                var tbody = $('#approval-list tbody');
                tbody.empty();
                const counts = response.data?.summaryCounts || {
                    total: 0,
                    approved: 0,
                    rejected: 0,
                    pending: 0
                };

                // Update Counts - always, even if no data
                $('.btn-total').text(`Total : ${counts.total || 0}`);
                $('.btn-approval').text(`Approval : ${counts.approved || 0}`);
                $('.btn-rejected').text(`Rejected : ${counts.rejected || 0}`);
                $('.btn-pending').text(`Pending : ${counts.pending || 0}`);
                if (response.success && response.data) {
                    console.log(response.data);
                    const data = response.data.approvalList || [];
                    const counts = response.data.summaryCounts || {};                   
                    data.forEach(function (item) {
                        var row = `
                            <tr>
                              <td class="sticky_cell checkbox">
                                    <div class="input-group">
                                        <input type="checkbox" class="form-check-input mt-0 row-checkbox"
                                               data-approval-id="${item.approval_Hdr_Id}"
                                               data-approval-status="${item.approval_Status}"
                                               data-approval-id-dtl="${item.approval_ID}" />
                                    </div>
                                </td>
                                <td>${item.audit_ID || ''}</td>
                                <td>${item.moduleName || ''}</td>

                                <td>${item.approverName || ''}</td>
                                <td>${item.reportedBy || ''}</td>
                                <td>${item.request_Date || ''}</td>
                                <td>${item.approvalStatus || ''}</td>
                                <td class="sticky_cell">
                                    <button type="button"
                                            class="redirectFromFormulaList btn btn-primary-light-icon-md btn-edit-hide"
                                            data-approval-id="${item.approval_Hdr_Id}"
                                            data-approval-status="${item.approval_Status}"
                                            data-approval-id-dtl="${item.approval_ID}">
                                        <span class="sprite-icons eye-primary-dark"></span>
                                    </button>
                                </td>
                            </tr>`;
                        tbody.append(row);
                    });

                    // Update Counts
                    //$('.btn-total').text(`Total : ${counts.total || 0}`);
                    //$('.btn-approval').text(`Approval : ${counts.approved || 0}`);
                    //$('.btn-rejected').text(`Rejected : ${counts.rejected || 0}`);
                    //$('.btn-pending').text(`Pending : ${counts.pending || 0}`);
                } else {
                    showAlert("danger", response.message || 'No records found.');   
                }
            },
            error: function () {
                showAlert("danger", 'Error occurred while fetching data.');  
            }
        });
    }
    function getSelectedApprovals() {
        const selected = [];
        $('#approval-list tbody input.row-checkbox:checked').each(function () {
            selected.push({
                approval_ID: $(this).data('approval-id-dtl'),
                approval_Hdr_Id: $(this).data('approval-id'),
                approval_Status: $(this).data('approval-status')
            });
        });
        return selected;
    }

});