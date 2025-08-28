/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 26-06-2025
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>

///////////////////////////////  SAVE G L Group:- Start////////////////////////////////////
$("#btnGLGroup").on("click", async function () {
    const isFormValid = await validateGLGroupForm();
    if (!isFormValid) return;

    const requestData = {
        GL_Group_Id: $("#GL_Group_Id").val() || 0,
        Group_Name: $("#Group_Name").val().trim(),
        Parent_GL_Group_Id: $("#Parent_Gl_Group_Id").val() ? parseInt($("#Parent_Gl_Group_Id").val()) : 0,
        Level: parseInt($("#Level").val()) || 0,
        IsActive: $("#glGroupActiveToggle").is(":visible") ? $("#glGroupActiveToggle").is(":checked") : true
    };

    $.ajax({
        url: "/GLGroup/AddGLGroup",
        type: "POST",
        data: JSON.stringify(requestData),
        contentType: "application/json",
        success: function (response) {
            if (response.success) {
                showAlert("success", response.message);
                setTimeout(function () {
                    window.location.href = '/GLGroup/Index';
                }, 4500);
            } else {
                showAlert("danger", response.message);
            }
        },
        error: function () {
            showAlert("danger", "An unexpected error occurred. Please try again.");
        }
    });
});
///////////////////////////////  SAVE G L Group:- End////////////////////////////////////

$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const encryptedId = urlParams.get('glgroupId');
 
    loadDropdowns()
        .then(() => {
            if (encryptedId) {
                $("#btnGLGroup").text("Update");
                $("#toggleContainer").show();
                fetchGLGroupById(encryptedId);
            } else {
                $("#btnGLGroup").text("Add");
                $("#toggleContainer").hide();
            }
        })
        .catch(() => {
            console.warn("Initial dropdown binding failed, retrying...");
            return loadDropdowns();
        });
});
function fetchGLGroupById(encryptedId) {
    $.ajax({
        url: `/GLGroup/GetGLGroupByEncryptedId?glgroupId=${encodeURIComponent(encryptedId)}`,
        type: 'GET',
        success: async function (response) {
            console.log(response);
            if (response && response.group_Name !== undefined) {
                await fetchAndBindGLGroupDropdown(0, '#Parent_Gl_Group_Id', 'Select Parent GL Group', response.gL_Group_Id);


                $('#GL_Group_Id').val(response.gL_Group_Id);
                $('#Group_Name').val(response.group_Name);
                //$('#Parent_Gl_Group_Id').val(response.parent_GL_Group_Id).trigger('change');
                if (response.parent_GL_Group_Id === 0) {
                    $('#Parent_Gl_Group_Id').val("").trigger('change');
                } else {
                    $('#Parent_Gl_Group_Id').val(response.parent_GL_Group_Id).trigger('change');
                }

                $('#Level').val(response.level || 1);

                $('#glGroupActiveToggle').prop('checked', response.isActive);
                $('#glGroupStatusLabel').text(response.isActive ? 'Active' : 'Inactive');

                $('#toggleContainer').show();
                $('#btnGLGroup').text('Update');
            } else {
                console.error("Failed to load GL Group:", response?.message);
            }
        },
        error: function () {
            console.error("AJAX error fetching GL Group.");
        }
    });
}
async function loadDropdowns() {
    try {
        await fetchAndBindGLGroupDropdown(0, '#Parent_Gl_Group_Id', 'Select Parent GL Group');
    } catch (err) {
        console.error("Dropdown binding sequence failed:", err);
    }
}
function fetchAndBindGLGroupDropdown(parentId, dropdownId, defaultText, excludeId = 0) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: `/DropDown/FetchGLDropdown?parentId=${parentId}`,
            type: 'GET',
            success: function (data) {
                const $dropdown = $(dropdownId);

                if ($.fn.select2 && $dropdown.hasClass("select2-hidden-accessible")) {
                    $dropdown.select2('destroy');
                }

                $dropdown.empty().append(`<option value="">${defaultText}</option>`);
                let hasValidData = false;

                if (data && data.length > 0) {
                    $.each(data, function (i, item) {
                        if (parseInt(item.value) !== parseInt(excludeId)) {
                            $dropdown.append(`<option value="${item.value}">${item.text}</option>`);
                        }
                    });
                    hasValidData = true;
                }

                if ($.fn.select2) {
                    $dropdown.select2({
                        width: '100%',
                        dropdownAutoWidth: true
                    });
                }

                resolve(hasValidData);
            },
            error: function (error) {
                console.error(`GL dropdown fetch failed for parentId=${parentId}:`, error);
                reject(error);
            }
        });
    });
}

async function validateGLGroupForm() {
    var isValid = true;
    var firstInvalidField = null;

    const groupName = $("#Group_Name").val().trim();
    const parentGLGroupId = $("#Parent_Gl_Group_Id").val();
    const level = $("#Level").val();

    if (!validateFormRequired(groupName, "#Group_Name-error", "Please enter GL Group Name.")) {
        isValid = false;
        firstInvalidField ??= "#Group_Name";
    } else if (!validateFormMaxLength(groupName, 100, "#Group_Name-error", "GL Group Name must not exceed 100 characters.")) {
        isValid = false;
        firstInvalidField ??= "#Group_Name";
    }

    if (parentGLGroupId && isNaN(parseInt(parentGLGroupId))) {
        $("#Parent_Gl_Group_Id-error").text("Invalid Parent GL Group selection.").show();
        isValid = false;
        firstInvalidField ??= "#Parent_Gl_Group_Id";
    } else {
        $("#Parent_Gl_Group_Id-error").text("").hide();
    }

    if (!validateFormRequired(level, "#Level-error", "Please enter Level.")) {
        isValid = false;
        firstInvalidField ??= "#Level";
    } else if (parseInt(level) < 1) {
        $("#Level-error").text("Level must be at least 1.").show();
        isValid = false;
        firstInvalidField ??= "#Level";
    } else {
        $("#Level-error").text("").hide();
    }

    // 🔹 Focus first invalid input
    if (!isValid && firstInvalidField) {
        $(firstInvalidField).focus();
    }

    return isValid;
}
