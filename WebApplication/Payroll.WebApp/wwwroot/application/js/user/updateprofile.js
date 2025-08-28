$(document).ready(function () {
    // Step 1: Restrict OTP input to only numeric characters
    $("#txtOTP").on('input', function () {
        // Replace any non-numeric characters with an empty string
        this.value = this.value.replace(/\D/g, '');

        // Limit input to 6 digits
        if (this.value.length > 6) {
            this.value = this.value.slice(0, 6);
        }
    });

    // Load company locations when Company or User Type changes
    $("#CompanyName, #UserTypeName").change(function () {
        loadCompanyLocations();
    });
    // ✅ Call loadCompanyLocations when the "Permission" tab is clicked
    $("#v-pills-permissions-tab").on("click", function () {
        resetPermissionTab();
        loadCompanyLocations();
    });

    // Reset dropdowns when returning to the permission tab
    function resetPermissionTab() {
        $("#userCity").empty().append(`<option value="">Select City</option>`);
        $("#userLocation").empty().append(`<option value="">Select Location</option>`);
        $("#userRole").empty().append(`<option value="">Select Role</option>`);
        $('.treeview').empty(); // ✅ Clear the tree view
    }

});
$(document).ready(function () {
    $(".select2_search_ctm").each(function () {
        $(this).select2({
            placeholder: $(this).data("placeholder"),
            width: '100%',
            dropdownAutoWidth: true
        });
    });
});

// ✅ Function to load company locations
function loadCompanyLocations() {
    const companyId = $("#CompanyName").attr("itemid");
    const userId = $("#UserTypeName").attr("itemid");

    if (!companyId || companyId == 0 || !userId) return;

    $.ajax({
        url: `/DropDown/GetCompanyLocationData`,
        type: "GET",
        data: { companyId: companyId, userId: userId },
        dataType: "json",
        success: function (response) {
            if (response.isSuccess && response.result) {
                const data = response.result;
                console.log(data);
                const cityDropdown = $("#userCity");
                const locationDropdown = $("#userLocation");
                const roleDropdown = $("#userRole");

                // Reset dropdowns and placeholders
                resetSelect(cityDropdown, "Select City");
                resetSelect(locationDropdown, "Select Location");
                resetSelect(roleDropdown, "Select Role");

                // Populate cities
                if (Array.isArray(data.cities)) {
                    data.cities.forEach(city => {
                        cityDropdown.append(`<option value="${city.city_ID}">${city.city_Name}</option>`);
                    });
                }

                // On city change, update locations
                cityDropdown.off("change").on("change", function () {
                    const selectedCityId = parseInt($(this).val()); // Convert to number
                    console.log("Selected city ID:", selectedCityId);

                    resetSelect(locationDropdown, "Select Location");
                    resetSelect(roleDropdown, "Select Role");

                    if (selectedCityId && Array.isArray(data.locations)) {
                        const filteredLocations = data.locations.filter(loc => loc.cityId === selectedCityId);
                        console.log("Filtered locations:", filteredLocations);

                        filteredLocations.forEach(location => {
                            locationDropdown.append(
                                `<option value="${location.correspondance_ID}">${location.locationName}</option>`
                            );
                        });

                        locationDropdown.trigger("change.select2");
                    }
                });



                // On location change, fetch roles
                locationDropdown.off("change").on("change", function () {
                    const selectedLocationId = $(this).val();
                    resetSelect(roleDropdown, "Select Role");

                    if (selectedLocationId) {
                        fetchRolesForSelectedLocation(userId, companyId, selectedLocationId);
                    }
                });

                // Re-initialize Select2 (if needed)
                reinitializeSelect2(cityDropdown);
                reinitializeSelect2(locationDropdown);
                reinitializeSelect2(roleDropdown);

            } else {
                console.warn(response.message || "No data received.");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data:", error);
        }
    });
}

// Utility to reset select2 dropdown
function resetSelect($selectElement, placeholderText) {
    $selectElement.empty().append(`<option value="">${placeholderText}</option>`).val("").trigger("change.select2");
}

// Utility to reinitialize Select2 if not already initialized
function reinitializeSelect2($selectElement) {
    if ($selectElement.hasClass("select2-hidden-accessible")) {
        $selectElement.select2("destroy");
    }
    $selectElement.select2({
        placeholder: $selectElement.data("placeholder"),
        width: '100%',
        dropdownAutoWidth: true
    });
}

//function loadCompanyLocations() {
//    const companyId = $("#CompanyName").attr("itemid");
//    const userId = $("#UserTypeName").attr("itemid");
//    const selectedCityIds = $('#selectedCityIds').val()?.split(',') || []; // Get the selected city values (can be multiple)
//    const selectedStateIds = $('#selectedStateIds').val()?.split(',') || []; // Get the selected city values (can be multiple)
//    console.log(selectedStateIds);
//    if (!companyId || companyId == 0 || !userId) {
//        return;
//    }

//    $.ajax({
//        url: `/DropDown/GetCompanyLocationData`,
//        type: "GET",
//        data: { companyId: companyId, userId: userId },
//        dataType: "json",
//        success: function (response) {
//            if (response.isSuccess && response.result) {
//                const data = response.result;
//                console.log(data);
//                const cityDropdown = $("#userCity");
//                const locationDropdown = $("#userLocation");
//                const roleDropdown = $("#userRole");

//                // Reset and initialize all dropdowns
//                cityDropdown.empty().append(`<option value="">Select City</option>`);
//                locationDropdown.empty().append(`<option value="">Select Location</option>`);
//                roleDropdown.empty().append(`<option value="">Select Role</option>`);

//                // Populate cities
//                if (data.cities?.length > 0) {
                   
//                    const filteredCities = data.cities.filter(city => selectedCityIds.includes(String(city.city_ID)));
//                    console.log(filteredCities);
//                    filteredCities.forEach(city => {
//                        cityDropdown.append(`<option value="${city.city_ID}" selected>${city.city_Name}</option>`);
//                    });
//                    // Set selected cities (if any)
//                    if (selectedCityIds.length > 0) {
//                        cityDropdown.val(selectedCityIds).trigger('change'); // Set multiple selected values and trigger change
//                    }

//                    // Reinitialize Select2 after setting the values
//                    cityDropdown.trigger('change.select2'); // Trigger Select2 change event to refresh UI
//                    reInitSelect2("#userCity"); // Reinitialize Select2 if you're using it
//                }

//                // On city change -> update locations
//                cityDropdown.off("change").on("change", function () {
//                    const selectedCityId = $(this).val();
//                    locationDropdown.empty().append(`<option value="">Select Location</option>`);
//                    roleDropdown.empty().append(`<option value="">Select Role</option>`);

//                    if (selectedCityId && data.locations?.length > 0) {
//                        const filteredLocations = data.locations.filter(loc => loc.cityId == selectedCityId);
//                        filteredLocations.forEach(location => {
//                            locationDropdown.append(`<option value="${location.correspondance_ID}">${location.locationName}</option>`);
//                        });
//                        reInitSelect2("#userLocation");
//                    }
//                });

//                // On location change -> update roles
//                locationDropdown.off("change").on("change", function () {
//                    const selectedLocationId = $(this).val();
//                    roleDropdown.empty().append(`<option value="">Select Role</option>`);
//                    if (selectedLocationId) {
//                        fetchRolesForSelectedLocation(userId, companyId, selectedLocationId);
//                    }
//                });

//                // If a city was selected in the past, populate the corresponding location
//                const selectedCityId = cityDropdown.val();
//                if (selectedCityId) {
//                    const filteredLocations = data.locations.filter(loc => loc.cityId == selectedCityId);
//                    filteredLocations.forEach(location => {
//                        locationDropdown.append(`<option value="${location.correspondance_ID}">${location.locationName}</option>`);
//                    });
//                    reInitSelect2("#userLocation");
//                }

//                // If a location was selected in the past, populate the corresponding roles
//                const selectedLocationId = locationDropdown.val();
//                if (selectedLocationId) {
//                    fetchRolesForSelectedLocation(userId, companyId, selectedLocationId);
//                }
//            } else {
//                console.warn(response.message || "No city/location data received.");
//            }
//        },
//        error: function (xhr, status, error) {
//            console.error("Error fetching company location data:", error);
//        }
//    });
//}
//function reInitSelect2(selector) {
//    if ($(selector).hasClass("select2-hidden-accessible")) {
//        $(selector).select2('destroy');
//    }

//    $(selector).select2({
//        width: '100%',
//        placeholder: $(selector).data("placeholder") || "Select an option",
//        allowClear: true
//    });

//    $(selector).on("select2:open", function () {
//        $(".select2-results").addClass("select2_search_ctm_result");
//    });
//}



function fetchRolesForSelectedLocation(userId, companyId, correspondanceId) {
    if (!correspondanceId) {
        $("#userRole").empty().append(`<option value="">Select Role</option>`);
        return;
    }

    $.ajax({
        url: `/User/FetchUserLocationWiseRole`,
        type: "GET",
        data: { userId: userId, companyId: companyId, correspondanceId: correspondanceId },
        dataType: "json",
        success: function (response) {
            let roleDropdown = $("#userRole");
            roleDropdown.empty();
            roleDropdown.append(`<option value="">Select Role</option>`);

            if (response.success && response.data && response.data.roleMenuHeaders) {
                response.data.roleMenuHeaders.forEach(function (role) {
                    roleDropdown.append(`<option value="${role.role_Menu_Hdr_Id}">${role.roleName}</option>`);
                });
            } else {
            }
        },
        error: function (xhr, status, error) {
        }
    });
}

/// Added by Krunali gohil payroll-377
// code start

$('#userRole').on('Keyup change', function () {

    let roleId = $("#userRole").val(); // Selected role ID
    console.log(roleId);
    if (!roleId) {
        $('.treeview').empty(); // ✅ Clear tree view if no role is selected
        return;
    }
    const companyId = $("#CompanyName").attr("itemid");
    let userId = $("#UserTypeName").attr("itemid");
    let correspondanceId = $("#userLocation").val();
    if (roleId && companyId) {
        fetchMenuData(roleId, companyId, userId, roleId, correspondanceId);
    }
});
function fetchMenuData(roleId, companyId, userId, roleMenuHeaderId, correspondanceId) {
    if (!roleId) {
        $('.treeview').empty(); // ✅ Prevent displaying data if no role is selected
        return;
    }
    $.ajax({
        url: `/User/FetchUserRoleMenuEditByUserIdRoleIdCompanyId?companyId=${companyId}&roleId=${roleId}&userId=${userId}&roleMenuHeaderId=${roleMenuHeaderId}&correspondanceId=${correspondanceId}`,  // Example URL (adjust as needed)
        method: 'GET',
        success: function (response) {
            console.log(response);
            $('.treeview').empty(); // Clear previous treeview data

            if (response.success) {
                const menuData = response.data.result;

                if (menuData && menuData.length > 0) {
                    const menuTreeHtml = generateTreeView(menuData, 0);
                    $('.treeview').append(menuTreeHtml);
                    initializeTreeToggle();
                } else {
                    $('.treeview').html('<p class="text-muted">No menu data available.</p>');
                }
            } else {
                showAlert("danger", response.message);
            }
        },
        error: function () {
            alert('Error fetching menu data.');
        }
    });
}
function initializeTreeToggle() {
    // Ensure all submenus are visible initially
    $('.treeview ul').show(); // Show all nested <ul> elements
    //// Ensure all submenus are hidden initially, except the root menu
    //$('.treeview ul').hide();  // Hide all nested <ul> by default
    //$('.treeview > ul').show(); // Show only the top-level <ul> (root menu)

    //// Toggle submenu visibility on click
    //$('.treeview').on('click', '.toggle', function () {
    //    // Toggle the visibility of the next <ul> sibling (the submenu)
    //    $(this).siblings('ul').toggle();

    //    // Toggle the 'open' class to change the appearance of the toggle button
    //    $(this).toggleClass('open');

    //    // Optional: Change the icon or appearance based on the open/close state
    //    if ($(this).hasClass('open')) {
    //        $(this).text('-');  // Example: change to a minus sign when open
    //    } else {
    //        $(this).text('+');  // Example: change to a plus sign when closed
    //    }
    //});
}

//// Krunali Date 12-02-25:- These code is basically given by Krunali :- Start
function generateTreeView(menuItems, parentId) {
    let treeHtml = '<ul>';

    menuItems.forEach(function (item) {
        if (item.parentMenuId === parentId) {
            let isChecked = item.hasPerDtl ? 'checked' : '';
            let isDisabled = 'disabled'; // Ensure all checkboxes remain disabled
            let disabledStyle = 'style="background-color: #6020A7; border-color: #6020A7;"';
            let permissionsHtml = `
                <div id="permissions-${item.menu_Id}" class="permissions" style="padding: 10px; margin: 10px 0; border: 1px solid #ccc; border-radius: 5px; background-color: #f9f9f9;">
                    <div style="display: flex; flex-wrap: wrap; gap: 10px;">
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="add-${item.menu_Id}" ${item.grantAdd ? 'checked' : ''} disabled ${disabledStyle}> Add</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="view-${item.menu_Id}" ${item.grantView ? 'checked' : ''} disabled ${disabledStyle}> View</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="edit-${item.menu_Id}" ${item.grantEdit ? 'checked' : ''} disabled ${disabledStyle}> Edit</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="delete-${item.menu_Id}" ${item.grantDelete ? 'checked' : ''} disabled ${disabledStyle}> Delete</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="approve-${item.menu_Id}" ${item.grantApprove ? 'checked' : ''} disabled ${disabledStyle}> Approve</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="rptprint-${item.menu_Id}" ${item.grantRptPrint ? 'checked' : ''} disabled ${disabledStyle}> Report Print</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="rptdownload-${item.menu_Id}" ${item.grantRptDownload ? 'checked' : ''} disabled ${disabledStyle}> Report Download</label>
                    </div>
                </div>`;

            // Recursive call for child menus
            let childTreeHtml = generateTreeView(menuItems, item.menu_Id);

            treeHtml += `
                <li>
                    <span class="toggle"></span>
                    <input type="checkbox" id="menu-${item.menu_Id}" class="menu-checkbox" data-menu-id="${item.menu_Id}" ${isChecked} ${isDisabled} ${disabledStyle} />
                    <label class="lg-text-500" for="menu-${item.menu_Id}">${item.menuName}</label>
                    ${permissionsHtml}
                    ${childTreeHtml} <!-- Recursive Call -->
                </li>`;
        }
    });

    treeHtml += '</ul>';
    return treeHtml;
}

//// Krunali Date 12-02-25:- These code is basically given by Krunali :- End


//$(document).ready(function () {
//    let userId = $("UserTypeName").itemid;// Get selected company ID
//    userId = $("#UserTypeName").attr("itemid");// Get selected company ID
//    if (userId != 0) {
//        populateRoleDropdown(userId); // Populate role dropdown
//    }

//});

//function populateRoleDropdown(userId) {
//    const roleDropdown = $("#userRole");
//    roleDropdown.empty();  // Clear previous options
//    roleDropdown.append(new Option("Select Role", ""));

//    // Make an AJAX request to fetch roles
//    $.ajax({
//        url: `/User/FetchUserRoleMenuByUserId?userId=${userId}`,  // Replace with your actual endpoint to fetch roles
//        method: 'GET',  // Method type (GET, POST, etc.)
//        success: function (response) {
//            // Check if response.data and response.data.result exist and are not empty
//            if (response && response.data && response.data.result && response.data.result.length > 0) {
//                // Populate dropdown with roles
//                response.data.result.forEach(function (role) {

//                    roleDropdown.append(new Option(role.roleName, role.role_Id));
//                });

//            } else {
//                // If no roles are available, show a default option
//                roleDropdown.append(new Option("No Role Selected", ""));
//            }
//        },
//        error: function () {
//            // If there's an error fetching the roles, display an error message
//            roleDropdown.append(new Option("Error fetching roles", ""));
//        }
//    });
//}


// end code payroll-377

var authConfig = null;

function SendEmailAndVerifyOTP(template) {

    showLoader();
    $('#email-validation-message').text('');
    $('#OTP-validation-message').text('');

    var email = $("#txtEmail").val();
    var templateType = template; // forgor password & reset password set from template value.
    var otp = $("#txtOTP").val();
    var isOtpStage = $('#otp-section').hasClass('div-show'); // Check if OTP is visible

    if (!isOtpStage) {
        if (email === '') {
            $('#email-validation-message').text('Please enter your email.');
            return;
        }

        if (!validateEmail(email)) {
            $('#email-validation-message').text('Valid email is required.');
            return;
        }

        $.ajax({
            type: 'POST',
            url: '/User/SendUpdateUserPasswordEmailWithOTP/',
            data: JSON.stringify({ Email: email, TemplateType: templateType }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (responseModel) {
                if (responseModel.isSuccess) {
                    hideLoader();
                    $("#txtEmail").prop('disabled', true);
                    $('#otp-section').toggleClass('div-hide div-show');
                    $('#verify-btn').text('Verify OTP');
                    toastr.success(responseModel.message, 'Success');
                }
                else {

                    if (responseModel.StatusCode === 404) {
                        hideLoader();
                        toastr.error(responseModel.message, 'Error');
                    }
                    else {
                        hideLoader();
                        $('#otp-section').toggleClass('div-hide div-show');
                        $('#verify-btn').text('Verify OTP');
                        toastr.error(responseModel.message, 'Error');
                    }
                }
            },
            error: function (xhr, status, error) {
                hideLoader();
                $('#email-validation-message').text('An error occurred. Please try again later.');
            }
        });
    }
    else {
        $('#OTP-validation-message').text('');

        var otp = $("#txtOTP").val();

        // Check if OTP is empty
        if (otp === '') {
            $('#OTP-validation-message').text('Please enter your OTP.');
            return;
        }

        // Validate OTP (must be 6 digits)
        var otpPattern = /^[0-9]{6}$/;
        if (!otpPattern.test(otp)) {
            $('#OTP-validation-message').text('OTP must be a 6-digit number.');
            return;
        }

        // Prepare ForgotPasswordModel object with Email and OTP
        var forgotPasswordModel = {
            Email: email,
            OTP: otp
        };

        // Make AJAX POST request
        $.ajax({
            type: 'POST',
            url: '/User/VerifyOTP/',
            data: JSON.stringify(forgotPasswordModel), // Passing model
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (responseModel) {
                authConfig = responseModel.result;
                if (responseModel.isSuccess) {
                    hideLoader();
                    toastr.success(responseModel.message, 'Success');  // OTP verified successfully
                    $('#UpdatePasswordForm').toggleClass('div-show div-hide');
                    $('#verify-btn').addClass('div-hide');
                }
                else {
                    hideLoader();
                    toastr.error(responseModel.message, 'Error');
                }
            },
            error: function (xhr, status, error) {
                hideLoader();
                $('#OTP-validation-message').text('An error occurred. Please try again later.');
            }
        });
    }
}
// Show Edit button only when "User Information" tab is active
$(document).on('shown.bs.tab', 'button[data-bs-toggle="pill"]', function (e) {
    var target = $(e.target).attr("data-bs-target");

    if (target === "#v-pills-home") {
        $("#edit-button-wrapper").show();  // Show Edit button
    } else {
        $("#edit-button-wrapper").hide();  // Hide Edit button
    }
});

// Optional: Trigger this once on page load in case "User Info" is already active
$(document).ready(function () {
    if ($("#v-pills-home-tab").hasClass("active")) {
        $("#edit-button-wrapper").show();
    } else {
        $("#edit-button-wrapper").hide();
    }
});

$(document).on("click", "#userEditButton", function () {
    var userId = $(this).data("userid");
    loadUserData(userId);
});
function loadUserData(userId) {
    $.ajax({
        url: `/User/GetUserRecordById?userId=${userId}`,
        type: "GET",
        contentType: "application/json",
        dataType: "json",
        success: function (response) {
            if (response.isSuccess && response.data) {
                var userDetails = response.data;
                // Check if the user is trying to edit themselves
                //if (response.editSessionUserId === response.data.userId) {
                //    $('#selfEditNotAllowedModal').modal('show'); // 🚫 Show modal
                //    return;
                //}

                // ✅ Check if sessionStorage already contains userData
                if (sessionStorage.getItem("userData")) {
                    sessionStorage.removeItem("userData"); // Remove existing data
                }

                // ✅ Store new user details in sessionStorage
                sessionStorage.setItem("userData", JSON.stringify(userDetails));

                // ✅ Redirect to the new page
                window.location.href = "/User/UpdateRecord";
            } else {
                showAlert("danger", response.message);
            }
        },
        error: function () {
            showAlert("danger", "Error while loading user data.");
        }
    });
}

$('#btnShowPassword').click(function () {
    var passwordInput = $('#txtPassword');
    var passwordFieldType = passwordInput.attr('type');

    // Toggle password visibility
    if (passwordFieldType === 'password') {
        passwordInput.attr('type', 'text');
        $(this).addClass('fa fa-eye-slash').removeClass('icon-eye');
    } else {
        passwordInput.attr('type', 'password');
        $(this).addClass('icon-eye').removeClass('fa fa-eye-slash');
    }
});
$('#UpdatePassword').click(function () {

    showLoader();
    $('#new-password-validation-message').text('');
    $('#confirm-password-validation-message').text('');

    var email = $('#txtEmail').val().trim();
    var newPassword = $("#txtNewPassword").val().trim();
    var confirmPassword = $("#txtConfirmPassword").val().trim();

    // Validate new and confirm password
    if (newPassword === '') {
        $('#new-password-validation-message').text('Please enter your new password.');
        return;
    }
    if (confirmPassword === '') {
        $('#confirm-password-validation-message').text('Please enter your confirm password.');
        return;
    }
    if (newPassword !== confirmPassword) {
        $('#confirm-password-validation-message').text('Password and Confirm password do not match.');
        return;
    }

    // Validate the password configuration
    var validationResult = validatePassword(newPassword, authConfig);
    if (!validationResult.isValid) {
        $('#new-password-validation-message').text(validationResult.message);
        return; // Prevent the request if validation fails
    }

    var forgotPasswordAndResetPasswordModel = {
        Email: email,
        NewPassword: newPassword
    };

    $.ajax({
        type: 'POST',
        url: '/User/UpdateUserPassword/',
        data: JSON.stringify(forgotPasswordAndResetPasswordModel), // Passing model
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (responseModel) {
            if (responseModel.isSuccess) {
                hideLoader();
                toastr.success(responseModel.message, 'Success');
                setTimeout(function () {
                    window.location.href = 'https://localhost:7093/';
                }, 2000);
            } else {
                toastr.error(responseModel.message, 'Error');
            }
        },
        error: function () {
            hideLoader();
            $('#OTP-validation-message').text('An error occurred. Please try again later.');
        }
    });
});

function validatePassword(password, config) {
    let messages = [];

    // Check minimum length
    if (config.passwordMinLength && password.length < config.passwordMinLength) {
        messages.push(`Password must be at least ${config.passwordMinLength} characters long.`);
    }

    // Check maximum length
    if (config.passwordMaxLength && password.length > config.passwordMaxLength) {
        messages.push(`Password must not exceed ${config.passwordMaxLength} characters.`);
    }

    // Check special characters
    if (config.hasSpecialCharacter && config.numberOfSpecialCharacters > 0) {
        const specialChars = password.replace(/[A-Za-z0-9]/g, '').length;
        if (specialChars < config.numberOfSpecialCharacters) {
            messages.push(`Password must contain at least ${config.numberOfSpecialCharacters} special character(s).`);
        }
    }

    // Check starting character
    if (config.startWithCharType && !/^[A-Za-z]/.test(password)) {
        messages.push('Password must start with a letter.');
    }

    // Check ending character
    if (config.endWithNumType && !/\d$/.test(password)) {
        messages.push('Password must end with a number.');
    }

    // Check number of digits
    if (config.numberOfDigits && (password.match(/\d/g) || []).length < config.numberOfDigits) {
        messages.push(`Password must contain at least ${config.numberOfDigits} digit(s).`);
    }

    // Check for sequential characters
    if (config.excludeSequence && hasSequentialChars(password, config.excludeSequence)) {
        messages.push('Password contains a sequence of characters that are not allowed.');
    }

    return { isValid: messages.length === 0, message: messages.join(' ') };
}

function hasSequentialChars(password, length) {
    for (let i = 0; i <= password.length - length; i++) {
        let segment = password.slice(i, i + length);
        if (/(\d)\1{2,}/.test(segment) || /([a-zA-Z])\1{2,}/.test(segment)) {
            return true; // Found sequential characters
        }
    }
    return false;
}

function validateEmail(email) {
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

