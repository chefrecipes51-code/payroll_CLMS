/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 25-06-2025
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>


///////////////////////////////  SAVE Account Header:- Start////////////////////////////////////
$("#btnGeneralAccount").on("click", async function () {
    const isFormValid = await validateGeneralAccountForm();
    if (!isFormValid) return;

    const parentId = $("#Parent_Gl_Group_Id").val();
    const subId = $("#Sub_Gl_Group_Id").val();
    const isSubVisible = $("#Sub_Gl_Group_Id").closest(".col-lg-6").is(":visible");

    const requestData = {
        Accounting_Head_Id: $("#Accounting_Head_Id").val() || 0,
        Accounting_Head_Name: $("#Accounting_Head_Name").val().trim(),
        GL_Code: $("#GL_Code").val().trim(),
        Account_Type: parseInt($("#Account_Type").val()),
        Tran_Type: parseInt($("#Transaction_Type").val()),
        IsActive: $("#generalAccountActiveToggle").is(":checked"),
        Parent_Gl_Group_Id: parseInt(parentId),
        Sub_Gl_Group_Id: (isSubVisible && subId) ? parseInt(subId) : null
    };

    $.ajax({
        url: "/GeneralAccount/AddGeneralAccount",
        type: "POST",
        data: JSON.stringify(requestData),
        contentType: "application/json",
        success: function (response) {
            if (response.success) {
                showAlert("success", response.message);
                setTimeout(function () {
                    window.location.href = '/GeneralAccount/Index';
                }, 4500);
            } else {
                showAlert("danger", response.message);
            }
        },
        error: function (xhr) {
            showAlert("danger", "An unexpected error occurred. Please try again.");
        }
    });
});

///////////////////////////////  SAVE Account Header:- End////////////////////////////////////

$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const encryptedId = urlParams.get('generalAccountId');
    $('#Sub_Gl_Group_Id').closest('.col-lg-6').hide();

    loadDropdowns()
        .then(() => {
            if (encryptedId) {
                $("#btnGeneralAccount").text("Update");
                $("#toggleContainer").show();
                fetchGeneralAccountById(encryptedId);
            } else {
                $("#btnGeneralAccount").text("Add");
                $("#toggleContainer").hide();
            }
        })
        .catch(() => {
            console.warn("Initial dropdown binding failed, retrying...");
            return loadDropdowns();
        });

    // Event to load sub GL group on change of parent GL group
    $('#Parent_Gl_Group_Id').on('change', function () {
        const parentId = $(this).val();
        if (parentId) {
            fetchAndBindGLDropdown(parentId, '#Sub_Gl_Group_Id', 'Select Sub GL Group')
                .then(hasData => {
                    $('#Sub_Gl_Group_Id').closest('.col-lg-6').toggle(hasData);
                });
        } else {
            $('#Sub_Gl_Group_Id').closest('.col-lg-6').hide();
        }
    });

});
async function loadDropdowns() {
    try {
        // Step 1: Bind Account Type
        await fetchAndBindDropdownForGeneralAccount(
            '/DropDown/FetchAccountHeadDropdown',
            '#Account_Type',
            'Select Account Type'
        );

        // Step 2: Bind Transaction Type
        await fetchAndBindDropdownForGeneralAccount(
            '/DropDown/FetchTranTypeDropdown',
            '#Transaction_Type',
            'Select Transaction Type'
        );

        // Step 3: Bind Parent GL Group only after both above succeed
        await fetchAndBindGLDropdown(0, '#Parent_Gl_Group_Id', 'Select Parent GL Group');

    } catch (err) {
        console.error("Dropdown binding sequence failed:", err);
    }
}
function fetchAndBindGLDropdown(parentId, dropdownId, defaultText) {
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
                        $dropdown.append(`<option value="${item.value}">${item.text}</option>`);
                    });

                    hasValidData = true;
                }

                if ($.fn.select2) {
                    $dropdown.select2({
                        width: '100%',
                        dropdownAutoWidth: true
                    });
                }

                resolve(hasValidData); // ✅ Only return true if we appended items
            },
            error: function (error) {
                console.error(`GL dropdown fetch failed for parentId=${parentId}:`, error);
                reject(error);
            }
        });
    });
}
function fetchAndBindDropdownForGeneralAccount(url, dropdownSelector, placeholder, data = null) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: url,
            type: 'GET',
            data: data ?? {},
            success: function (response) {
                const $dropdown = $(dropdownSelector);

                if ($.fn.select2 && $dropdown.hasClass("select2-hidden-accessible")) {
                    $dropdown.select2('destroy');
                }

                $dropdown.empty().append(`<option value="">${placeholder}</option>`);

                response.forEach(function (item) {
                    $dropdown.append(new Option(item.text, item.value));
                });

                if ($.fn.select2) {
                    $dropdown.select2({
                        width: '100%',
                        dropdownAutoWidth: true
                    });
                }

                resolve();
            },
            error: function (error) {
                console.error(`Dropdown fetch failed for ${dropdownSelector}:`, error);
                reject();
            }
        });
    });
}
function fetchGeneralAccountById(encryptedId) {
    $.ajax({
        url: `/GeneralAccount/GetGeneralAccountByEncryptedId?generalAccountId=${encodeURIComponent(encryptedId)}`,
        type: 'GET',
        success: async function (response) {
            if (response && response.accounting_Head_Name !== undefined) {
                $('#Accounting_Head_Id').val(response.accounting_Head_Id);
                $('#Accounting_Head_Name').val(response.accounting_Head_Name);
                $('#GL_Code').val(response.gL_Code);

                await loadDropdowns(); // Rebind dropdowns before setting values

                $('#Account_Type').val(response.account_Type).trigger('change');
                $('#Transaction_Type').val(response.tran_Type).trigger('change');
                $('#Parent_Gl_Group_Id').val(response.parent_Gl_Group_Id).trigger('change');

                if (response.sub_Gl_Group_Id && response.sub_Gl_Group_Id !== 0) {
                    fetchAndBindGLDropdown(response.parent_Gl_Group_Id, '#Sub_Gl_Group_Id', 'Select Sub GL Group')
                        .then(hasData => {
                            $('#Sub_Gl_Group_Id').val(response.sub_Gl_Group_Id).trigger('change');
                            $('#Sub_Gl_Group_Id').closest('.form-group').toggle(hasData);
                        });
                } else {
                    $('#Sub_Gl_Group_Id').closest('.form-group').hide();
                }

                $('#generalAccountActiveToggle').prop('checked', response.isActive);
            } else {
                console.error("Failed to load General Account by ID:", response?.message);
            }
        },
        error: function () {
            console.error("AJAX error loading General Account.");
        }
    });
}
async function validateGeneralAccountForm() {
    var isValid = true;
    var firstInvalidField = null;

    const headName = $("#Accounting_Head_Name").val().trim();
    const glCode = $("#GL_Code").val().trim();
    const accountType = $("#Account_Type").val();
    const transactionType = $("#Transaction_Type").val();
    const parentGlGroupId = $("#Parent_Gl_Group_Id").val();
    const subGlGroupId = $("#Sub_Gl_Group_Id").val();
    const isSubGlVisible = $("#Sub_Gl_Group_Id").closest(".col-lg-6").is(":visible");

    // Accounting Head Name
    if (!validateFormRequired(headName, "#Accounting_Head_Name-error", "Please enter Accounting Head Name.")) {
        isValid = false;
        firstInvalidField ??= "#Accounting_Head_Name";
    } else if (!validateFormMaxLength(headName, 200, "#Accounting_Head_Name-error", "Accounting Head Name must not exceed 200 characters.")) {
        isValid = false;
        firstInvalidField ??= "#Accounting_Head_Name";
    }

    // GL Code
    if (!validateFormRequired(glCode, "#GL_Code-error", "Please enter GL Code.")) {
        isValid = false;
        firstInvalidField ??= "#GL_Code";
    } else if (!validateFormMaxLength(glCode, 10, "#GL_Code-error", "GL Code must not exceed 10 characters.")) {
        isValid = false;
        firstInvalidField ??= "#GL_Code";
    }

    // Account Type
    if (!validateFormRequired(accountType, "#Account_Type-error", "Please select Account Type.")) {
        isValid = false;
        firstInvalidField ??= "#Account_Type";
    }

    // Transaction Type
    if (!validateFormRequired(transactionType, "#Transaction_Type-error", "Please select Transaction Type.")) {
        isValid = false;
        firstInvalidField ??= "#Transaction_Type";
    }

    // ✅ Parent GL Group must be selected
    if (!validateFormRequired(parentGlGroupId, "#Parent_Gl_Group_Id-error", "Please select Parent GL Group.")) {
        isValid = false;
        firstInvalidField ??= "#Parent_Gl_Group_Id";
    }

    // ✅ Sub GL Group required only if dropdown is visible and has options
    if (isSubGlVisible && $("#Sub_Gl_Group_Id option").length > 1 && !subGlGroupId) {
        $("#Sub_Gl_Group_Id-error").text("Please select Sub GL Group.").show();
        isValid = false;
        firstInvalidField ??= "#Sub_Gl_Group_Id";
    } else {
        $("#Sub_Gl_Group_Id-error").text("").hide();
    }

    // Set focus on the first invalid input
    if (!isValid && firstInvalidField) {
        $(firstInvalidField).focus();
    }

    return isValid;
}

