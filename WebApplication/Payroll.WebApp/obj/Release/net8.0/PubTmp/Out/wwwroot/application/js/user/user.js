/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-280,290,367                                                          *
 *  Description:                                                                                    *
 *  This JavaScript code implements validation, dynamic behavior, and treeview navigation           *
 *  for a multi-tab form containing user information and company details. It includes:              *
 *  - Real-time validation for individual input fields and dropdowns.                               *
 *  - Dynamic filtering of text fields to prevent special characters.                               *
 *  - Cascading dropdowns to dynamically load values based on user selection.                       *
 *  - Form submission using AJAX with client-side validation before sending data.                   *
 *  - Treeview navigation to dynamically enable, disable, and switch tabs based on validation.      *
 *                                                                                                  *                                                                                                  *
 *  Methods:                                                                                        *
 *  - validateField : Validates individual fields and displays error messages as needed.            *
 *  - validatePhoneNumber : Validates phone numbers to ensure they contain 10 digits.               *
 *  - validateEmail : Validates email format using a regular expression.                            *
 *  - validateSpecialCharacters : Ensures text fields like names contain no special characters.     *
 *  - validateMinLength : Validates if a field value meets the minimum required length.             *
 *  - validateNameFields : Validates both first and last name fields for required and length rules. *
 *  - validateFirstTab : Validates all fields in the user information tab.                          *
 *  - validateSecondTab : Validates all fields in the company details tab.                          *
 *  - toggleCompanyDetailsTab : Enables or disables the "Company Details" tab.                      *
 *  - handleFormSubmission : Handles the final form submission after validation.                    *
 *  - Treeview Tab Navigation: Prevents manual tab switching unless the current tab is valid.       *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 30-Dec-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/

/* #region 1: User Creation and Save User Permissions */
$(document).ready(function () {
    /* Generic reset button (btn-transparent-danger) */
    $('.btn-transparent-danger').on('click', function () {
        let activeTab = $('.tab-pane.active');

        // Reset textboxes inside the active tab
        activeTab.find('input[type="text"], input[type="email"], input[type="tel"]').val('');

        // Reset dropdowns (Select2 and normal select)
        activeTab.find('select').val('').trigger('change');

        // Clear validation error messages
        activeTab.find('.error_input').removeClass('error_input');
        activeTab.find('[id$="-error"]').text('').hide();
    });
    // Function to validate a field based on field ID and error message
    function validateField(fieldId, errorMessage) {
        const field = $(`#${fieldId}`);
        const errorElement = $(`#${fieldId}-error`);
        let value = field.val();  // Change 'const' to 'let' to allow reassignment

        // If using Select2, check for empty or null value
        if (field.hasClass("select2-hidden-accessible")) {
            value = Array.isArray(value) ? value.join("").trim() : (value || "").trim();
        } else {
            value = (value || "").trim();
        }

        if (!value || value.length === 0) {
            field.addClass('error_input');
            errorElement.text(errorMessage).show();
            return false;
        } else {
            field.removeClass('error_input');
            errorElement.text('').hide();
            return true;
        }
    }

    // Validate Phone Number (10-digit number)
    function validatePhoneNumber(phone) {
        const phoneRegex = /^[0-9]{10}$/; // Allows only 10-digit numbers
        return phoneRegex.test(phone);
    }

    // Validate Email format (simple regex)
    function validateEmail(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/; // Basic email format validation
        return emailRegex.test(email);
    }

    // Validate Special Characters (for names like first name, last name)
    function validateSpecialCharacters(name) {
        const specialCharRegex = /^[a-zA-Z\s]*$/; // Allows only letters and spaces
        return specialCharRegex.test(name);
    }

    // Dynamically filter input for First Name (remove special characters)
    $('#fname').on('input', function () {
        const fname = $(this).val();

        // Check if the first name contains special characters
        if (!validateSpecialCharacters(fname)) {
            // Remove special characters if found
            $(this).val(fname.replace(/[^a-zA-Z\s]/g, ''));
            // Display error message for special characters
            $('#fname-error').text('First Name cannot contain special characters or numbers.');
        }
        else {
            // Clear error message if input is valid
            $('#fname-error').text('');
        }
    });
    // Dynamically filter input for Last Name (remove special characters)
    $('#lname').on('input', function () {
        const lname = $(this).val();
        // Check if the last name contains special characters
        if (!validateSpecialCharacters(lname)) {
            // Remove special characters if found
            $(this).val(lname.replace(/[^a-zA-Z\s]/g, ''));
            // Display error message for special characters
            $('#lname-error').text('Last Name cannot contain special characters or numbers.');
        }
        else {
            // Clear error message if input is valid
            $('#lname-error').text('');
        }
    });

    //Added by krunali gohil - 16/01/2025 payroll-377
    $(document).on('click', '#getDataButton[data-bs-target="#viewUser"]', function () {
        var userId = $(this).data("userid"); // Get user ID from data attribute
        if (userId) {
            //    window.location.href = `/User/UserProfile?userId=` + userId;
            fetch('/User/EncryptId?id=' + encodeURIComponent(userId))
                .then(response => response.text())
                .then(encryptedId => {
                    window.location.href = "/User/UserProfile?userId=" + encodeURIComponent(encryptedId);
                })
                .catch(error => console.error('Encryption error:', error));
        }
    });
    //end method payroll-377

    // Validate minimum length (e.g., for name fields)
    function validateMinLength(fieldId, minLength) {
        const field = $(`#${fieldId}`);
        const value = field.val().trim();
        if (value.length < minLength) {
            field.addClass('error_input');
            $(`#${fieldId}-error`).text(`Please enter minimum ${minLength} characters.`).show();
            return false;
        }
        return true;
    }

    // Validate First and Last Name fields (required and minimum 2 characters)
    function validateNameFields() {
        let isValid = true;

        // Validate First Name: Required and Minimum 2 characters
        isValid &= validateField('fname', 'Please enter the First Name.');
        isValid &= validateMinLength('fname', 2);

        // Validate Last Name: Required and Minimum 2 characters
        isValid &= validateField('lname', 'Please enter the Last Name.');
        isValid &= validateMinLength('lname', 2);

        return isValid;
    }
    // Validate fields in the first tab (user information)
    function validateFirstTab() {
        let isValid = true;

        isValid &= validateField('phone', 'Please enter the Phone Number.') &&
            validatePhoneNumber($('#phone').val().trim());
        // Validate Email - Show error message if email format is invalid
        const email = $('#email').val().trim();
        if (!email) {
            isValid &= validateField('email', 'Please enter the email address.');
        } else if (!validateEmail(email)) {
            $('#email-error').text('please enter a valid email address.').show();
            $('#email').addClass('error_input');
            isValid = false;
        } else {
            $('#email-error').text('').hide();
            $('#email').removeClass('error_input');
        }

        isValid &= validateField('usertype', 'Please select the User Type.');
        isValid &= validateField('countries', 'Please select the Country.');
        isValid &= validateField('salutationsDropdown', 'Please select the salutation.');
        isValid &= validateField('username', 'please enter the username.');
        //isValid &= validateField('fname', 'Enter first name is required.');
        //isValid &= validateField('lname', 'Enter last name is required.');
        // Check First and Last Name validation
        isValid &= validateNameFields();
        return isValid;
    }
    //added by abhishek 13-02-2025
    function validateChangePwdFields() {

        let isValid = true;

        // Validate First Name: Required and Minimum 3 characters
        isValid &= validateField('oldpwd', 'Please enter the old password.');
        //isValid &= validateMinLength('OldPassword', 3);

        // Validate Last Name: Required and Minimum 3 characters
        isValid &= validateField('newpwd', 'Please enter the new password.');
        //isValid &= validateMinLength('NewPassword', 3);

        isValid &= validateField('Confirmpwd', 'Please enter the confirm password.');
        //isValid &= validateMinLength('Confirmpwd', 3);

        // Compare New Password and Confirm Password
        let newPassword = document.getElementById('newpwd').value;
        let confirmPassword = document.getElementById('Confirmpwd').value;

        if (newPassword !== confirmPassword) {
            alert("New Password and Confirm Password do not match.");
            isValid = false;
        }

        return isValid;
    }
    // Prevent manual input of non-numeric values in Phone Field
    $('#phone').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, ''); // Replace non-numeric characters
    });

    // Validate fields in the second tab (company details)
    function validateSecondTab() {
        let isValid = true;

        isValid &= validateField('Companies', 'Please select the Company.');
        isValid &= validateField('countriesname', 'Please select the Country.');
        isValid &= validateField('state', 'Please select the State.');
        isValid &= validateField('branch', 'Please select the City.');
        isValid &= validateField('department', 'Please select the Location.');
        isValid &= validateField('role', 'Please select the Role.');
        isValid &= validateField('effectiveFromDt', 'Please enter the Effective From Date.');

        return isValid;
    }

    // Attach real-time validation for text fields (user information)
    $('#usertype, #salutationsDropdown,#fname, #lname, #email, #phone, #username,#countries').on('input', function () {
        validateField($(this).attr('id'), `${$(this).attr('placeholder')} is required.`);
    });

    // Attach real-time validation for dropdown fields (company details)
    $('#usertype, #salutationsDropdown, #countries, #state, #branch, #department, #role, #Companies, #countriesname').on('change', function () {
        validateField($(this).attr('id'), 'This field is required.');
    });

    // Attach onfocus event to clear error messages and styles when the user focuses on the field
    $('#fname, #lname, #email, #phone, #username,#countries,#usertype, #salutationsDropdown').on('focus', function () {
        const fieldId = $(this).attr('id');
        $(`#${fieldId}`).removeClass('error_input');
        $(`#${fieldId}-error`).text('').hide();
    });


    // Function to toggle tab state based on validation
    function toggleTabState(tabId, disable) {
        const tabElement = $(`#${tabId}`);
        if (disable) {
            tabElement.addClass('disabled').attr('aria-disabled', 'true').attr('tabindex', '-1');
        } else {
            tabElement.removeClass('disabled').removeAttr('aria-disabled').removeAttr('tabindex');
        }
    }


    // Initial State: Disable all tabs except the first
    toggleTabState('v-pills-profile-tab', true);
    toggleTabState('v-pills-messages-tab', true);

    // Prevent manual tab switching if validation fails
    $('button[data-bs-toggle="pill"]').on('click', function (e) {
        const clickedTabId = $(this).attr('id');

        if (clickedTabId === 'v-pills-profile-tab' && !validateFirstTab()) {
            e.preventDefault();
            $('#common-validation-message').text('Please correct the errors in the User Information tab.');
        } else if (clickedTabId === 'v-pills-messages-tab' && !validateSecondTab()) {
            e.preventDefault();
            $('#common-validation-message').text('Please correct the errors in the Company Details tab.');
        }
    });

    // Handle "Next" button on the first tab
    $('#nextTab').on('click', function () {
        if (validateFirstTab()) {
            toggleTabState('v-pills-profile-tab', false);
            $('#v-pills-profile-tab').trigger('click');
        } else {
            $('#common-validation-message').text('Please correct the errors in the User Information tab.');
        }
    });

    // Handle "Previous" button on the second tab with validation
    $('#previousFirstTab').on('click', function (e) {
        if (validateFirstTab()) {
            toggleTabState('v-pills-home-tab', false);  // Enable tab explicitly to ensure it's clickable
            $('#v-pills-home-tab').trigger('click');
        } else {
            $('#common-validation-message').text('Please correct the errors in the User Information tab.');
        }
    });

    let selectedRoles = [];  // Declare outside the function to store roles globally

    // Handle form submission or tab change after validating second tab
    $("#nextButton").on("click", function (e) {
        e.preventDefault();
        if (validateSecondTab()) {
            const dateValue = $("#effectiveFromDt").val();
            const [day, month, year] = dateValue.split('/');
            const formattedDate = new Date(`${year}-${month}-${day}`).toISOString().split('T')[0];
            //const formattedDate = dateValue ? new Date(dateValue).toISOString().split('T')[0] : null;
            const data = {
                userType_Id: $("#usertype").val(),
                Entity: $("#dropdownTextbox").text().trim(),
                Salutation: $("#salutationsDropdown").val(),
                FirstName: $("#fname").val(),
                MiddleName: $("#mname").val(),
                LastName: $("#lname").val(),
                country: $("#countries").val(),
                PhoneNumber: $('#countries').val() + '-' + $('#phone').val(),
                //PhoneNumber1: $('#countries option:selected').text() + '-' + $('#phone').val(),
                Email: $("#email").val(),
                Username: $("#username").val(),
                CompanyId: $("#Companies").val(), // Select2 dropdown
                CountryId: $("#countriesname").val(),
                State: $("#state").val(),
                City: $("#branch").val(),
                EffectiveFromDt: formattedDate, // Ensure correct format
                Correspondance_ID: $("#department").val() ? $("#department").val().map(Number) : [], // Multi-select values
                Role_Menu_Header_Id: $("#role").val() ? $("#role").val().map(Number) : [] // Multi-select values
            };
            selectedRoles = $("#role").val() ? $("#role").val().map(function (roleId) {
                return {
                    id: roleId,
                    text: $("#role option[value='" + roleId + "']").text().trim()
                };
            }) : [];

            $.ajax({
                url: "/User/AddRecord",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(data),
                success: function (response) {
                    if (response.success != null && response.success != "") {
                        $("#userId").val(response.userId); //Added By Harshida Save value of UserID in Textbox for future use.
                        $("#usereffectiveFromDt").val(response.usereffectiveFromDt); //Added By Harshida Save value of effectiveFromDt in Textbox for future use.
                        showAlert("success", response.message);
                        setTimeout(function () {
                            // Redirect to the list page
                            window.location.href = "/User/Index";
                        }, 1000);

                        /* $("#v-pills-messages-tab").tab('show');*/
                        //// Reset all input fields
                        //$("#firstTab")[0].reset(); // Reset the form fields
                        //// Reset all input fields
                        //$("#secondTab")[0].reset(); // Reset the form fields
                        //// Reset Select2 dropdowns
                        //$("#usertype,#salutationsDropdown,#countries,#Companies, #countriesname, #state, #branch, #role, #department").val(null).trigger("change");
                        //// Reset manually populated text fields
                        //$("#dropdownTextbox").text("");
                        //$("#common-validation-message").text("");
                    } else {
                        showAlert("danger", response.message);
                    }
                },
                error: function (xhr) {
                    showAlert("danger", "An error occurred while creating the user: " + xhr.responseText);
                }
            });
            // Add your form submission or tab change logic here
        } else {
            $('#common-validation-message').text('Please correct the errors in the second tab.');

        }

    });

    //User changed password by Abhishek - 18-02-2025 Task 484
    $("#Savechangepwd").on("click", function (e) {
        if (validateChangePwdFields()) {
            const data = {
                OldPassword: $("#oldpwd").val(),
                NewPassword: $("#newpwd").val()
            };
            $.ajax({
                url: "/User/ChangePassword",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(data),
                success: function (response) {
                    if (response.success != null && response.success != "") {

                        alert(response.message);
                        setTimeout(function () {
                            handleLogout(); // Call logout after delay
                        }, 1000); // 1-second delay
                    } else {
                        showAlert("danger", response.message);
                    }
                },
                error: function (xhr) {
                    showAlert("danger", "An error occurred while creating the user: " + xhr.responseText);
                }
            });
            // Add your form submission or tab change logic here
        } else {
            $('#common-validation-message').text('Please correct the errors in the second tab.');

        }

    });
    //$(document).on("click", "#userEditButton", function () {
    //    var userId = 79;//$(this).data("id");
    //    loadUserData(userId);
    //});
    //function loadUserData(userId) {
    //    $.ajax({
    //        url: `/User/GetUserRecordById?userId=${userId}`,
    //        type: "GET",
    //        contentType: "application/json",
    //        dataType: "json",
    //        success: function (response) {
    //            if (response.isSuccess && response.data) {
    //                var userDetails = response.data;

    //                // ✅ Check if sessionStorage already contains userData
    //                if (sessionStorage.getItem("userData")) {
    //                    sessionStorage.removeItem("userData"); // Remove existing data
    //                }

    //                // ✅ Store new user details in sessionStorage
    //                sessionStorage.setItem("userData", JSON.stringify(userDetails));

    //                // ✅ Redirect to the new page
    //                window.location.href = "/User/UpdateRecord";
    //            } else {
    //                showAlert("danger",response.message);
    //            }
    //        },
    //        error: function () {
    //            showAlert("danger", "Error while loading user data.");
    //        }
    //    });
    //}
    function populateRoleDropdown() {
        const roleDropdown = $("#userRole");
        roleDropdown.empty();  // Clear previous options

        if (selectedRoles.length > 0) {
            selectedRoles.forEach(function (role) {
                roleDropdown.append(new Option(role.text, role.id));
            });
        } else {
            roleDropdown.append(new Option("No Role Selected", ""));
        }
    }

    // Clear errors on input focus
    $('input, select').on('focus change', function () {
        $(this).removeClass('error_input');
        $(`#${$(this).attr('id')}-error`).text('').hide();
    });

    $('#v-pills-messages-tab').on('shown.bs.tab', function () {
        populateRoleDropdown(); // Populate role dropdown

        // Initialize variables for roleId and companyId
        let roleId = $("#userRole").val(); // Selected role ID from populated dropdown
        const companyId = $("#Companies").val(); // Get selected company ID

        // Attach change event to dynamically update roleId
        $('#userRole').on('change', function () {
            roleId = $(this).val();  // Update roleId based on current dropdown selection
            if (roleId && companyId) {
                fetchMenuData(roleId, companyId); // Fetch menu data with new roleId
            } else {
                //alert('Please select a role.');
            }
        });

        // Initial fetch when tab is shown
        if (roleId && companyId) {
            fetchMenuData(roleId, companyId);  // Fetch menu data with the initial selection
        } else {
            //alert('Please select a role.');
        }
    });
    // Function to fetch menu data
    function fetchMenuData(roleId, companyId) {
        $.ajax({
            url: `/User/FetchUserRoleMenuByRoleId?roleId=${roleId}&company_Id=${companyId}`,
            method: 'GET',
            success: function (response) {
                if (response.success) {
                    const menuData = response.data.result;
                    $('.treeview').empty();  // Clear previous treeview data
                    const menuTreeHtml = generateTreeView(menuData, 0);  // Start with ParentMenu_Id = 0
                    $('.treeview').append(menuTreeHtml);
                    initializeTreeToggle();  // Initialize toggle functionality
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert('Error fetching menu data.');
            }
        });
    }

    // Recursive function to generate tree view HTML
    /*Created By Priyanshi :- Start*/
    //function generateTreeView(menuItems, parentId) {
    //    let treeHtml = '<ul>';
    //    menuItems.forEach(function (item) {
    //        if (item.parentMenuId === parentId) {
    //            let permissionsHtml = '';
    //            if (item.hasPerDtl) {
    //                permissionsHtml = `
    //                <div id="permissions-${item.menu_Id}" class="permissions" style="display: none; padding: 10px; margin: 10px 0; border: 1px solid #ccc; border-radius: 5px; background-color: #f9f9f9;">
    //                    <div style="display: flex; flex-wrap: wrap; gap: 10px;">
    //                        ${item.grantAdd ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="add-' + item.menu_Id + '"> Add</label>' : ''}
    //                        ${item.grantView ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="view-' + item.menu_Id + '"> View</label>' : ''}
    //                        ${item.grantEdit ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="edit-' + item.menu_Id + '"> Edit</label>' : ''}
    //                        ${item.grantDelete ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="delete-' + item.menu_Id + '"> Delete</label>' : ''}
    //                        ${item.grantRptPrint ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="rptprint-' + item.menu_Id + '"> Report Print</label>' : ''}
    //                        ${item.grantRptDownload ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="rptdownload-' + item.menu_Id + '"> Report Download</label>' : ''}
    //                        ${item.docDownload ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="docdownload-' + item.menu_Id + '"> Document Download</label>' : ''}
    //                        ${item.docUpload ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="docupload-' + item.menu_Id + '"> Document Upload</label>' : ''}
    //                    </div>
    //                </div>`;
    //            }
    //            treeHtml += `
    //            <li>
    //                <span class="toggle"></span>
    //                <input type="checkbox" id="menu-${item.menu_Id}" class="menu-checkbox" data-menu-id="${item.menu_Id}"
    //                    data-role-menu-hdr-id="${item.role_Menu_Hdr_Id}"
    //                    data-role-menu-dtl-id="${item.role_Menu_Dtl_Id}"
    //                >
    //                <label class="lg-text-500" for="menu-${item.menu_Id}">${item.menuName}</label>
    //                ${permissionsHtml}
    //                ${generateTreeView(menuItems, item.menu_Id)} <!-- Recursive Call -->
    //            </li>`;
    //        }
    //    });
    //    treeHtml += '</ul>';
    //    return treeHtml;
    //}

    // Function to recursively uncheck a checkbox, its children, grandchildren, and the permission checkboxes inside the permission div
    /* Created By Priyanshi:- End */

    function generateTreeView(menuItems, parentId) {
        let treeHtml = '<ul>';
        menuItems.forEach(function (item) {
            if (item.parentMenuId === parentId) {  // Parent-child relation is maintained here
                let permissionsHtml = '';
                if (item.hasPerDtl) {
                    permissionsHtml = `
                <div id="permissions-${item.menu_Id}" class="permissions" style="display: none; padding: 10px; margin: 10px 0; border: 1px solid #ccc; border-radius: 5px; background-color: #f9f9f9;">
                    <div style="display: flex; flex-wrap: wrap; gap: 10px;">
                        ${item.grantAdd ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="add-' + item.menu_Id + '"> Add</label>' : ''}
                        ${item.grantView ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="view-' + item.menu_Id + '"> View</label>' : ''}
                        ${item.grantEdit ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="edit-' + item.menu_Id + '"> Edit</label>' : ''}
                        ${item.grantDelete ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="delete-' + item.menu_Id + '"> Delete</label>' : ''}
                        ${item.grantRptPrint ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="rptprint-' + item.menu_Id + '"> Report Print</label>' : ''}
                        ${item.grantRptDownload ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="rptdownload-' + item.menu_Id + '"> Report Download</label>' : ''}
                        ${item.docDownload ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="docdownload-' + item.menu_Id + '"> Document Download</label>' : ''}
                        ${item.docUpload ? '<label style="flex: 1 0 45%;"><input type="checkbox" id="docupload-' + item.menu_Id + '"> Document Upload</label>' : ''}
                    </div>
                </div>`;
                }
                treeHtml += `
            <li>
                <span class="toggle"></span>
                <input type="checkbox" id="menu-${item.menu_Id}" 
                    class="menu-checkbox" 
                    data-menu-id="${item.menu_Id}" 
                    data-role-menu-hdr-id="${item.role_Menu_Hdr_Id}" 
                    data-role-menu-dtl-id="${item.role_Menu_Dtl_Id}">
                <label class="lg-text-500" for="menu-${item.menu_Id}">${item.menuName}</label>
                ${permissionsHtml}
                ${generateTreeView(menuItems, item.menu_Id)} <!-- Recursive Call -->
            </li>`;
            }
        });
        treeHtml += '</ul>';
        return treeHtml;
    }
    function uncheckAllChildren(menuId) {
        // Uncheck the current checkbox
        $(`#menu-${menuId}`).prop('checked', false);

        // Uncheck all permission checkboxes within the associated permission div
        $(`#permissions-${menuId}`).find('input[type="checkbox"]').prop('checked', false);

        // Hide the permissions for this menu item
        $(`#permissions-${menuId}`).slideUp();

        // Find and recursively uncheck all child checkboxes
        $(`#menu-${menuId}`).closest('li').find('ul li').each(function () {
            const childMenuId = $(this).find('.menu-checkbox').data('menu-id');
            uncheckAllChildren(childMenuId); // Recursively uncheck all child checkboxes and hide permissions
        });
    }

    // Handle checkbox click to toggle permissions visibility and uncheck child/grandchild checkboxes when parent is unchecked
    $(document).on('change', '.menu-checkbox', function () {
        const menuId = $(this).data('menu-id');
        const isChecked = $(this).is(':checked');

        // Toggle visibility of permissions based on checkbox state
        if (isChecked) {
            $(`#permissions-${menuId}`).slideDown();
        } else {
            $(`#permissions-${menuId}`).slideUp();
        }

        // If this checkbox is unchecked, uncheck all child/grandchild checkboxes and hide permissions
        if (!isChecked) {
            uncheckAllChildren(menuId); // Uncheck child/grandchild checkboxes and hide permissions
        }
    });

    // Initialize toggle functionality
    function initializeTreeToggle() {
        $('.treeview').on('click', '.toggle', function () {
            $(this).siblings('ul').toggle();
            $(this).toggleClass('open');
        });
    }

    $('#savePermissions').on('click', function () {
        const permissionsData = getSelectedPermissions();
        const dateValue = $("#effectiveFromDt").val();
        const formattedDate = dateValue ? new Date(dateValue).toISOString().split('T')[0] : null;

        const roleId = $('#userRole').val();  // Get selected role ID
        const companyId = $('#Companies').val();  // Get selected company ID
        const userId = $('#userId').val(); //Added By Harshida 13-01-'25
        const effectiveFromDt = formattedDate; //Added By Harshida 13-01-'25
        if (permissionsData.length > 0 && roleId && companyId) {
            $.ajax({
                url: '/User/SaveUserRoleMenuPermissions',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ roleId, companyId, permissionsData, userId, effectiveFromDt }),
                success: function (response) {
                    if (response.type == "success") {
                        //showAlert("success", response.message || 'Permissions saved successfully!');   
                        var userAddedModal = new bootstrap.Modal(document.getElementById('userAddedModal'));
                        userAddedModal.show();
                    }
                    else {
                        showAlert("danger", response.message || 'Failed to save permissions.');
                    }
                },
                error: function () {
                    showAlert("danger", "An error occurred while creating the user: " + xhr.responseText);
                }
            });
        } else {
            showAlert("danger", "Please select permissions and ensure Role are selected.");
        }
    });

});
/* #endregion */

/* #region 2: User Creation and Save User Permissions */

//
////MOST IMP NOTE:- For below function
////1 . "addNewUser()" is used after inserted successfully and if wish to insert to another user.
////2 . "goToUserList()" is used after  inserted successfully and if wish to redirect to LIST PAGE.
//
function addNewUser() { //Please do not remove these fuction READ ABOVE NOTE
    const url = "/User/AddRecord";
    window.location.href = url;
}
function goToUserList() { //Please do not remove these fuction READ ABOVE NOTE
    const url = "/User/Index";
    window.location.href = url;
}
//Added By Harshida 09-01-'25:- Start
function getSelectedPermissions() {
    const selectedPermissions = [];

    // Loop through each checkbox with the class 'menu-checkbox'
    $('.menu-checkbox:checked').each(function () {
        const menuId = $(this).data('menu-id');

        // Traverse up the DOM tree to find the closest parent 'li' and its associated 'parent-id'
        const parentMenuId = $(this).closest("ul").closest("li").find("input.menu-checkbox").data("menu-id") || null;

        const roleMenuHdrId = $(this).data('role-menu-hdr-id');
        const roleMenuDtlId = $(this).data('role-menu-dtl-id');

        // Collect permissions for the current menu item
        const permissions = {
            menuId: menuId,
            parentMenuId: parentMenuId, // Parent ID of the current menu item
            roleMenuHdrId: roleMenuHdrId,
            roleMenuDtlId: roleMenuDtlId,
            permissions: {
                add: $('#add-' + menuId).prop('checked'),
                view: $('#view-' + menuId).prop('checked'),
                edit: $('#edit-' + menuId).prop('checked'),
                delete: $('#delete-' + menuId).prop('checked'),
                rptPrint: $('#rptprint-' + menuId).prop('checked'),
                rptDownload: $('#rptdownload-' + menuId).prop('checked'),
                docDownload: $('#docdownload-' + menuId).prop('checked'),
                docUpload: $('#docupload-' + menuId).prop('checked')
            }
        };

        // Add the collected permissions data to the selectedPermissions array
        selectedPermissions.push(permissions);
    });

    // Return the array of selected permissions
    return selectedPermissions;
}
$(document).ready(function () {
    let selectedButton = null;

    $(document).on('click', '.btn-danger-light-icon[data-bs-target="#deleteUser"]', function () {
        selectedButton = $(this);
    });

    $('#confirmUserDelete').on('click', function () {
        if (selectedButton) {
            var userId = selectedButton.data('userid');
            let rowId = `row-${userId}`; // Construct the row ID
            var email = selectedButton.data('email');
            var contactNo = selectedButton.data('contactno');
            var companyName = selectedButton.data('companyname');

            // Prepare data object
            var rowData = {
                UserId: userId,
                Email: email,
                ContactNo: contactNo,
                CompanyName: companyName
            };

            $.ajax({
                url: '/User/DeleteUser', // Use the URL directly
                datatype: 'json',
                data: rowData,
                success: function (response) {
                    if (response === 'Record deleted successfully') {
                        // Hide the row
                        $(`#${rowId}`).fadeOut(500, function () {
                            $(this).remove(); // Remove the row from DOM after fadeOut
                        });
                        showAlert('success', response);
                    }
                    $('#deleteUser').modal('hide');
                },
                error: function (error) {
                    $('#deleteUser').modal('hide');
                }
            });
        }
    });
    $("#username").on("keydown", function (event) {
        if (event.keyCode === 9) {
            $(this).trigger("change");
        }
    });
    $("#username").on("change", function () {
        var email = $(this).val();
        if (email) {
            $.ajax({
                url: '/User/CheckUserExist',
                type: 'GET',
                data: { email: email },
                success: function (response) {
                    if (response.success) {
                        //showAlert("success", response.message);                       
                    }
                    else {
                        showAlert("danger", response.message);
                        $("#username").focus();
                    }
                },
                error: function () {
                    showAlert("danger", "An error occurred while checking the email.");
                }
            });
        } else {
            showAlert("danger", "Please enter a valid email.");
        }
    });






    $('#effectiveFromDt').datepicker({
        format: 'dd/mm/yyyy',     // Display date format
        startDate: '0d',          // Disable past dates in calendar
        autoclose: true,          // Close calendar automatically after selection
        todayHighlight: true,     // Highlight today's date
        beforeShowDay: function (date) {
            if (date < new Date().setHours(0, 0, 0, 0)) {
                return {
                    enabled: false,  // Disable past dates
                    classes: 'disabled-date' // Add a custom class for styling
                };
            }
            return { enabled: true };
        }
    }).datepicker('setDate', new Date())   // Set current date by default
        .on('changeDate', function (e) {
            // Clear the date if a past date is somehow selected
            if (new Date(e.date) < new Date().setHours(0, 0, 0, 0)) {
                $('#effectiveFromDt').val('');
                showAlert("warning", "Past date are not allowed.");
            }
        });

    // Prevent manual typing of past dates
    $('#effectiveFromDt').on('keypress keydown paste', function (e) {
        e.preventDefault();  // Block manual typing
    });



});
$(document).ready(function () {

});



