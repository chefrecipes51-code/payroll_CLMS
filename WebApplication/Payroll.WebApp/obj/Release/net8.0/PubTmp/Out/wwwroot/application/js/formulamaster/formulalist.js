/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 25-April-2025
///  IMP NOTE      :-
///                1) IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
///                2) Why new file BECAUSE IN calculatepayformula.js file AT PAGE LOAD/DOCUMENT LOAD
///                   fetching the "PAY COMPONENT LIST" which not required at the time of LIST FORMULA.
/// </summary>

$(document).ready(function () {
    //document.querySelectorAll('.redirectFromFormulaList').forEach(button => {
    //    button.addEventListener('click', function () {
    //        var formulaId = $(this).data("formulaid");
    //        //var formulaId = this.getAttribute('data-formulaid');
    //        fetch('/FormulaMaster/EncryptId?id=' + encodeURIComponent(formulaId))
    //            .then(response => response.text())
    //            .then(encryptedId => {
    //                window.location.href = "/FormulaMaster/AddUpdateFormula?formulaId=" + encodeURIComponent(encryptedId);
    //            })
    //            .catch(error => console.error('Encryption error:', error));
    //    });
    //});
    $(document).on('click', '.redirectFromFormulaList', function () {
        var formulaId = $(this).data("formulaid");

        fetch('/FormulaMaster/EncryptId?id=' + encodeURIComponent(formulaId))
            .then(response => response.text())
            .then(encryptedId => {
                window.location.href = "/FormulaMaster/AddUpdateFormula?formulaId=" + encodeURIComponent(encryptedId);
            })
            .catch(error => console.error('Encryption error:', error));
    });

    // When clicking the delete button
    $(document).on("click", ".formulaMasterDelete", function () {
        var formulaId = $(this).data("formulaid");        
        $("#confirmFormulaDelete").data("formulaid", formulaId);
    });
    $('#confirmFormulaDelete').on('click', function () {
        var formulaId = $(this).data('formulaid'); 
        var model = {
            Formula_Id: formulaId
        };

        $.ajax({
            url: "/FormulaMaster/DeleteFormula",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(model),
            success: function (response) {
                if (response.success) {
                    $('#deleteFormula').modal('hide');
                    showAlert("success", response.message);
                    reloadFormulaGrid(); 
                } else {
                    $('#deleteFormula').modal('hide');                  
                    showAlert("danger", response.message); 
                    reloadFormulaGrid(); 
                }
            },            error: function (error) {
                showAlert("error", "An error occurred while deleting the formula.");
            }
        });
    });
    $('#deleteFormula').on('hidden.bs.modal', function () {
        $('.formulaMasterDelete:first').focus(); // Adjust selector to your use case
    });

    function reloadFormulaGrid() {
        $.ajax({
            url: '/FormulaMaster/Index', 
            type: 'GET',
            success: function (data) {
                $('#formula-list tbody').html($(data).find('#formula-list tbody').html());
            },
            error: function () {
                showAlert("error", "Failed to reload formula grid.");
            }
        });
    }
});
