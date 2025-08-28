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
    $(document).on('click', '.redirectFromGLGroupList', function () {
        var glgroupid = $(this).data("glgroupid");

        fetch('/GLGroup/EncryptId?id=' + encodeURIComponent(glgroupid))
            .then(response => response.text())
            .then(encryptedId => {
                window.location.href = "/GLGroup/AddUpdateGLGroup?glgroupId=" + encodeURIComponent(encryptedId);
            })
            .catch(error => console.error('Encryption error:', error));
    });
    $(document).on("click", ".glGroupDelete", function () {
        const glGroupId = $(this).data("glgroupid");
        $("#confirmGLGroupDelete").data("glgroupid", glGroupId);
    });
    $("#confirmGLGroupDelete").on("click", function () {
        const glGroupId = $(this).data("glgroupid");

        const model = {
            GL_Group_Id: glGroupId
        };

        $.ajax({
            url: "/GLGroup/DeleteGLGroup",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(model),
            success: function (response) {
                $("#deleteGLGroupModal").modal("hide");

                if (response.success) {
                    showAlert("success", response.message);
                    reloadGLGroupGrid();
                } else {
                    showAlert("danger", response.message);
                    reloadGLGroupGrid();
                }
            },
            error: function () {
                showAlert("danger", "An error occurred while deleting the GL Group.");
            }
        });
    });
    $("#deleteGLGroupModal").on("hidden.bs.modal", function () {
        $(".glGroupDelete:first").focus();
    });
    function reloadGLGroupGrid() {
        $.ajax({
            url: '/GLGroup/Index',
            type: 'GET',
            success: function (data) {
                $("#gl-group-list tbody").html($(data).find("#gl-group-list tbody").html());
            },
            error: function () {
                showAlert("danger", "Failed to reload GL Group grid.");
            }
        });
    }
});
