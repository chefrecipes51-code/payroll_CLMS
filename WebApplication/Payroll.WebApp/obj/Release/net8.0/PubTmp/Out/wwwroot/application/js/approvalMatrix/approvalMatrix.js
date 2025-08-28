$(document).ready(function () {
    initializeDropDownList();
    initializeGrid();
    initializeFormSubmit();
});



// Initialize dropdowns for both Add and Edit forms
function initializeDropDownList() {

    $('.select2_search_ctm').select2({
        placeholder: function () {
            return $(this).data('placeholder'); // Set dynamic placeholder from data-placeholder attribute
        },
        allowClear: true,  // Allows clearing the selection (if needed)
        multiple: false,   // Ensure it's a single select dropdown
        dropdownAutoWidth: true,  // Auto adjust dropdown width
        width: '100%'      // Ensures the dropdown takes up full width of its container
    });

   
    loadModules('#moduleSelect', function () {
        $('#moduleSelect').trigger('change');
    });

    loadUsers('#userSelect', function () {
        // $('#userSelect').trigger('change');
    });


    // Module Change Event
    $(document).on('change', '#moduleSelect', function () {
        var moduleId = $(this).val();
        loadServices(moduleId, '#serviceSelect', function () {
            $('#serviceSelect').trigger('change');
        });
    });

}

function initializeGrid() {

    // Add More Button Functionality
    $(document).on("click", ".add-more", function () {
        debugger;
        validateGetData();      

        let newRow = $(".approval-rowHidden:first").clone(); // Clone the first row
        newRow.removeClass("approval-rowHidden").addClass("approval-row");

        // Clear values in cloned row
        newRow.find("select, input").val("");
      
        // Append the cloned row to the Select User
        $("#approvalRows").append(newRow);
    });

    // Remove Button Functionality
    $(document).on("click", ".remove", function () {
        if ($(".approval-row").length > 1) {
            $(this).closest(".approval-row").remove();
        } else {
            showAlert("danger", "At least one row must remain.");
        }
    });
}

function initializeFormSubmit() {
    $("#getApprovalData").click(function () {
        $("#loadingSpinner").show(); // Show the spinner when request starts

        let selectedServiceId = $("#serviceSelect").val(); // Get selected user ID from dropdown
        validateGetData();      
        FunGetLevels(selectedServiceId);
    });

    $("#submitApprovalSetup").click(function () {
        let approvalSetUpDetails = [];
        let isError = false;
        let isDuplicate = false;
        let userIds = new Set(); // To check for duplicate users
        let levelSet = new Set(); // To check if all levels 1,2,3 are present

        //Get all available levels dynamically
        let requiredLevels = new Set();
        $(".level-select option").each(function () {
            let levelValue = parseInt($(this).val());
            if (!isNaN(levelValue)) {
                requiredLevels.add(levelValue);
            }
        });

        $(".approval-row").each(function () {

            let serviceId = $(".service-select").val();
            let userId = $(this).find(".user-select").val();
            let levelNumber = $(this).find(".level-select").val();
            let isAlternate = document.getElementById("sequenceApproval").checked ? false : true;

            if (userId && levelNumber ) {
                if (userIds.has(userId)) {                    
                    isDuplicate = true;                    
                    return false; // Exit loop early
                }
                userIds.add(userId);
                levelSet.add(parseInt(levelNumber));

                approvalSetUpDetails.push({
                    ServiceID: parseInt(serviceId),
                    LevelNumber: parseInt(levelNumber),
                    UserID: parseInt(userId),
                    SequenceOrder: parseInt(levelNumber),
                    IsAlternate: isAlternate
                });
            }
            else {
                isError = true;
            }
        });

        if (isDuplicate) {
            showAlert("danger", `Duplicate User selected. Please choose a different user.`);
            return;
        }
        if (isError) {
            showAlert("danger", "Please select a user, level, and sequence order for all approval rows.");
            return;
        }
        validateGetData();
      
        // Ensure all dynamically required levels are present
        if (document.getElementById("sequenceApproval").checked) {
            let missingLevels = [...requiredLevels].filter(level => !levelSet.has(level));
            if (missingLevels.length > 0) {
                showAlert("danger", `Missing levels: ${missingLevels.join(", ")}. Please select all required levels.`);
                return;
            }
        }

        if (approvalSetUpDetails.length === 0) {
            showAlert("danger", "Please add at least one user.");
            return;
        }

        let requestData = {
            ModuleId: 0,
            CreatedBy: 0,
            UpdatedBy: 0,
            MessageType: 0,
            MessageMode: 0,
            approvalSetUpDetails: approvalSetUpDetails
        };

        $.ajax({
            url: "/ApprovalSetUp/add",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(requestData),
            success: function (response) {
                showAlert("success", "Approval setup added successfully!");
                //console.log(response);
            },
            error: function (xhr, status, error) {
                showAlert("danger", "Error: " + xhr.responseText);
            }
        });
    });
}

//ajax call
function FunGetLevels(selectedServiceId) {
    $.ajax({

        url: "/ApprovalSetUp/getservicebyid/" + selectedServiceId,  // Call Web App API, not Microservice directly
        type: "GET",
        dataType: "json",
        success: function (response) {
        
            if (response.isSuccess && response.result) {
                let numberOfLevels = response.result.numberOfLevels;
                let $levelSelect = $("#levelSelect");

                $levelSelect.empty().append('<option value="">-- Select Level --</option>');

                for (let i = 1; i <= numberOfLevels; i++) {
                    $levelSelect.append(`<option value="${i}">Level ${i}</option>`);
                }

                FunGetApprovalSetUpData(selectedServiceId);
            } else {
                showAlert("danger", "Failed to retrieve levels.");
            }
        },
        error: function (xhr) {
            $("#loadingSpinner").hide();
            console.error("Error fetching levels:", xhr.responseText);
            showAlert("danger", "Error fetching levels.");
        }
    });
}

function FunGetApprovalSetUpData(selectedServiceId) {

    $.ajax({
        url: "/ApprovalSetUp/GetApprovalSetUpByServiceId/" + selectedServiceId,  // Call Web App API, not Microservice directly
        type: "GET",
        dataType: "json",
        success: function (response) {
                
            if (response.isSuccess && response.result) {
                populateApprovalRows(response.result);
            } else {
                showAlert("danger", "Failed to retrieve approval data.");
            }
        },
        error: function (xhr) {
            $("#loadingSpinner").hide();
            console.error("Error fetching levels:", xhr.responseText);
            showAlert("danger", "Error fetching levels.");
        }
    });
}

function populateApprovalRows(data) {
    $(".approval-row").remove(); // Remove all but the first template row
    // $(".approval-row").not(":first").remove();  
    data.forEach(function (item, index) {
        let newRow = $(".approval-rowHidden:first").clone(); // Clone template row
        newRow.removeClass("approval-rowHidden").addClass("approval-row");

        newRow.find("#userSelect").val(item.userID); // Set user
        newRow.find("#levelSelect").val(item.levelNumber); // Set level

        $("#approvalRows").append(newRow); // Append the row

    });

    if (data.length > 0) {
        // $(".approval-row:first").remove();
    }
    else {
        let newRow = $(".approval-rowHidden:first").clone(); // Clone template row
        newRow.removeClass("approval-rowHidden").addClass("approval-row");

        $("#approvalRows").append(newRow); // Append the row
    }
}

//Bind Dropdown--------------------------------
function loadModules(targetDropdown, callback) {
    $.ajax({
        url: '/DropDown/FetchModulesDropdown',
        method: 'GET',
        success: function (data) {
            populateDropdown(targetDropdown, data);
            if (callback) callback();
        },
        error: function () {
            showError(targetDropdown + '-error', 'Failed to load modules.');
        }
    });
}

function loadUsers(targetDropdown, callback) {
    $.ajax({
        url: '/DropDown/GetUsersDropdown',
        method: 'GET',
        success: function (data) {
            populateDropdown(targetDropdown, data);
            if (callback) callback();
        },
        error: function () {
            showError(targetDropdown + '-error', 'Failed to load modules.');
        }
    });
}

// Load states dropdown based on selected country
function loadServices(moduleId, targetDropdown, callback) {
    if (!moduleId) return;
    $.ajax({
        url: '/DropDown/FetchServicesDropdown/' + moduleId,
        method: 'GET',
        success: function (data) {
            populateDropdown(targetDropdown, data);
            if (callback) callback();
        },
        error: function () {
            showError(targetDropdown + '-error', 'Failed to load states.');
        }
    });
}

// Populate dropdowns with API data
function populateDropdown(dropdownId, data) {
    var dropdown = $(dropdownId);
    dropdown.empty();
    // dropdown.append('<option value="" disabled selected>Select an option</option>');
    // Get label text for setting placeholder

    var labelText = $('label[for="' + dropdownId.replace('#', '') + '"]').text();
    var placeholderText = labelText ? 'Select ' + labelText.trim() : 'Select an option';

    // Set placeholder dynamically based on label
    dropdown.append('<option value="" disabled selected>' + placeholderText + '</option>');

    if (!data || !Array.isArray(data)) {
        return;
    }

    data.forEach(function (item) {
        if (item && item.value !== undefined && item.text) {
            dropdown.append('<option value="' + item.value + '">' + item.text + '</option>');
        }
    });
    // Re-initialize select2 to apply the updated options
    dropdown.trigger('change');
}

function validateGetData() {
    let isValid = true;
    isValid &= validateField('moduleSelect', 'Please select the Module.');
    isValid &= validateField('serviceSelect', 'Please select the Service.');
    return isValid;
}

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
//--------------------------------------------------
