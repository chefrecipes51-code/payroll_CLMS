/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 25-June-2025
///  IMP NOTE      :-
///                1) IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
///                2) Why new file BECAUSE IN calculatepayformula.js file AT PAGE LOAD/DOCUMENT LOAD
///                   fetching the "PAY COMPONENT LIST" which not required at the time of LIST FORMULA.
/// </summary>

$(document).ready(function () {   
    $(document).on('click', '.redirectFromGeneralAccountList', function () {
        var generalaccountid = $(this).data("generalaccountid");

        fetch('/GeneralAccount/EncryptId?id=' + encodeURIComponent(generalaccountid))
            .then(response => response.text())
            .then(encryptedId => {
                window.location.href = "/GeneralAccount/AddUpdateAccountMaster?generalAccountId=" + encodeURIComponent(encryptedId);
            })
            .catch(error => console.error('Encryption error:', error));
    });

    // Open modal and set ID
    $(document).on("click", ".generalAccountDelete", function () {
        var accountId = $(this).data("generalaccountid");
        $("#confirmAccountingHeadDelete").data("generalaccountid", accountId);
    });

    // On confirm delete
    $('#confirmAccountingHeadDelete').on('click', function () {
        var accountId = $(this).data('generalaccountid');

        var model = {
            Accounting_Head_Id: accountId
        };

        $.ajax({
            url: "/GeneralAccount/DeleteGeneralAccount",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(model),
            success: function (response) {
                $('#deleteAccountingHead').modal('hide');

                if (response.success) {
                    showAlert("success", response.message);
                    reloadAccountingGrid();
                } else {
                    showAlert("danger", response.message);
                    reloadAccountingGrid();
                }
            },
            error: function () {
                showAlert("danger", "An error occurred while deleting the accounting head.");
            }
        });
    });

    // Refocus logic (optional UX)
    $('#deleteAccountingHead').on('hidden.bs.modal', function () {
        $('.generalAccountDelete:first').focus();
    });

    // Reload grid after deletion
    function reloadAccountingGrid() {
        $.ajax({
            url: '/GeneralAccount/Index',
            type: 'GET',
            success: function (data) {
                $('#accounting-head-list tbody').html($(data).find('#accounting-head-list tbody').html());
            },
            error: function () {
                showAlert("danger", "Failed to reload accounting grid.");
            }
        });
    }

});
