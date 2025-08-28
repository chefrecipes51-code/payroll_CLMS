/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 24-Jan-2025
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>

$(document).ready(function () {

    // Prevent manual input of non-numeric values in Phone Field
    $('#Primary_Phone_No').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, ''); // Replace non-numeric characters
    });

    // Prevent manual input of non-numeric values in Phone Field
    $('#Secondary_Phone_No').on('input', function () {
        this.value = this.value.replace(/[^0-9]/g, ''); // Replace non-numeric characters
    });


    $(document).on('change', '#companyCorrespondanceCountryDropdown', function () {
        //const countryId = $(this).val();
        //if (countryId) {
        //    fetchCompanyCorrespondanceStateDropdown(countryId); // Fetch states when country changes
        //} else {
        //    // Clear state and city dropdowns if no country is selected
        //    clearDropdown('#companyCorrespondanceStateDropdown', 'Select State');
        //    clearDropdown('#companyCorrespondanceCityDropdown', 'Select City');
        //    clearDropdown('#companyCorrespondanceLocationDropdown', 'Select Location');
        //}       
        const countryId = $(this).val();
        clearDropdown('#companyCorrespondanceStateDropdown', 'Select State');
        clearDropdown('#companyCorrespondanceCityDropdown', 'Select City');
        clearDropdown('#companyCorrespondanceLocationDropdown', 'Select Location');

        if (countryId) {
            fetchCompanyCorrespondanceStateDropdown(countryId);
        }
    });
    $(document).on('change', '#companyCorrespondanceStateDropdown', function () {
        const stateId = $(this).val();
        clearDropdown('#companyCorrespondanceCityDropdown', 'Select City');
        clearDropdown('#companyCorrespondanceLocationDropdown', 'Select Location');

        if (stateId) {
            fetchCompanyCorrespondanceCityDropdown(stateId);
        }
        //const stateId = $(this).val();
        //if (stateId) {
        //    fetchCompanyCorrespondanceCityDropdown(stateId); // Fetch cities when state changes
        //} else {
        //    // Clear city dropdown if no state is selected
        //    clearDropdown('#companyCorrespondanceCityDropdown', 'Select City');
        //    clearDropdown('#companyCorrespondanceLocationDropdown', 'Select Location');
        //}
    });
    $(document).on('change', '#companyCorrespondanceCityDropdown', function () {
        const cityId = $(this).val();
        clearDropdown('#companyCorrespondanceLocationDropdown', 'Select Location');

        if (cityId) {
            fetchCompanyCorrespondanceLocationDropdown(cityId);
        }
        //const stateId = $(this).val();
        //if (stateId) {
        //    fetchCompanyCorrespondanceLocationDropdown(stateId); // Fetch cities when state changes
        //} else {
        //    // Clear city dropdown if no state is selected
        //    clearDropdown('#companyCorrespondanceLocationDropdown', 'Select Location');
        //}
    });
    function clearDropdown(selector, placeholder) {
        $(selector).empty().append(new Option(placeholder, ''));
        //$(selector).empty().append(new Option(placeholder, '')).trigger('change');
    }
    //function clearDropdown(selector, placeholder) {
    //    return $.Deferred().resolve().done(() => {
    //        $(selector).empty().append(new Option(placeholder, '')).trigger('change');
    //    });
    //}
});
$(document).ready(function () {
    $(document).on('click', '#saveCompanyCorrespondanceButton', function () {
        const correspondanceId = $('#Correspondance_ID').val();
        const companyId = $('#Company_Id').val();
        const isEdit = correspondanceId && parseInt(correspondanceId) > 0;
        //console.log("isEdit" + isEdit);
        const url = isEdit ? '/Company/UpdateCompanyCorrespondance' : '/Company/AddCompanyCorrespondance';
        const method = 'POST';
        if (isEdit) {
            var formEditData = validateAndCollectCorrespondanceDataForEdit();
            //saveCorrespondanceData(formEditData.data, url, method);
            if (formEditData.isValid) { // Only proceed if validation passes
                saveCorrespondanceData(formEditData.data, url, method);
            }
        }
        else {
            var formAddData = validateAndCollectCorrespondanceDataForAdd();
            //saveCorrespondanceData(formAddData.data, url, method);
            if (formAddData.isValid) { // Only proceed if validation passes
                saveCorrespondanceData(formAddData.data, url, method);
            }
        }       
    });
    function updateCorrespondancePartial() {
        var companytCorrespondanceId = $("#GlobalCompany_Id").val();
        if ($.fn.DataTable.isDataTable('#company-correspondances-list')) {
            $('#company-correspondances-list').DataTable().destroy();
        }

        $.ajax({
            url: '/Company/GetCorrespondancePartialByCompanyId',
            type: 'GET',
            data: { companytCorrespondanceId: companytCorrespondanceId },
            success: function (html) {                
                $('#company-correspondances-list tbody').html($(html).find('tbody').html());
                makeDataTable("company-correspondances-list");
            },
            error: function (xhr, status, error) {
                //alert("An error occurred while updating the partial view: " + error);
                //console.log("An error occurred while updating the partial view: " + error);
            }
        });
    }
    function saveCorrespondanceData(formData, url, method) {        
        $.ajax({
            url: url,
            type: method,
            data: formData,
            success: function (response) {
                if (response.success) {
                    //showAlert("success", response.message);
                    //$('#editCorrespondanceOffcanvas').offcanvas('hide');
                    //$('.offcanvas-backdrop').remove();
                    //$('#v-pills-profile-tab').tab('show');
                    //updateCorrespondancePartial();
                    showAlert("success", response.message);

                    var offcanvasElement = document.getElementById('editCorrespondanceOffcanvas');
                    var offcanvasInstance = bootstrap.Offcanvas.getInstance(offcanvasElement);
                    if (!offcanvasInstance) {
                        offcanvasInstance = new bootstrap.Offcanvas(offcanvasElement);
                    }
                    offcanvasInstance.hide();

                    setTimeout(function () {
                        $('.offcanvas-backdrop').remove();
                    }, 500);

                    if (document.getElementById('v-pills-profile-tab')) {
                        var tab = new bootstrap.Tab(document.getElementById('v-pills-profile-tab'));
                        tab.show();
                    }
                    setTimeout(function () {
                        updateCorrespondancePartial();
                    }, 500);
                } else {
                    showAlert("danger", response.message);                    
                    //console.log("Failed to save correspondance: " + response.message);
                }
            },
            error: function (xhr, status, error) {
                //console.log("An error occurred: For For For  " + error);
            }
        });
    }

});
function resetForm(isAddMode = false) {
    //console.log("From Add" + isAddMode);
    // Set the form title based on whether it's Add or Edit mode
    setFormTitle(!isAddMode);  // Pass true for Edit mode, false for Add mode
    // Toggle visibility of the toggle button
    //toggleVisibility(isAddMode);
    if (isAddMode) {
        // Reset everything for adding a new location
        $('#Correspondance_ID').val(''); // Clear hidden Correspondance ID
        // $('#Company_Id').val(''); // Clear hidden Company_Id
        // $('#CreatedBy').val(''); // Clear hidden CreatedBy
        $('#CompanyAddress').val('');
        $('#Building_No').val('');
        $('#Building_Name').val('');
        $('#Street').val('');
        $('#Primary_Phone_No').val('');
        $('#Secondary_Phone_No').val('');
        $('#Primary_Email_Id').val('');
        $('#Secondary_Email_ID').val('');
        $('#Website_url').val('');
        $('#companyCorrespondanceCountryDropdown').val('').trigger('change');
        $('#companyCorrespondanceStateDropdown').val('').trigger('change');
        $('#companyCorrespondanceCityDropdown').val('').trigger('change');
        $('#companyCorrespondanceLocationDropdown').val('').trigger('change');
        // Hide the toggle in Add mode
        //$('#toggleContainer').hide();
    } else {
        // Check if it's in edit mode (locationId is not empty)
        //const isEditMode = originalLocationFormData.Location_Id !== '';
        //if (isEditMode) {
        //    $('#toggleContainer').show();
        //    // Reset the form fields to original values for the edit form
        //    $('#locationId').val(originalLocationFormData.Location_Id);
        //    $('#locationCountryDropdown').val(originalLocationFormData.Country_Id).trigger('change');
        //    $('#locationStateDropdown').val(originalLocationFormData.State_Id).trigger('change');
        //    $('#locationCityDropdown').val(originalLocationFormData.CityId).trigger('change');
        //    $('#locationname').val(originalLocationFormData.LocationName);
        //    // Set toggle based on the original data for edit mode
        //    if (originalLocationFormData.IsActive) {
        //        $('#locationActiveToggle').prop('checked', true);
        //        $('#activeStatusLabel').text('Active');
        //    } else {
        //        $('#locationActiveToggle').prop('checked', false);
        //        $('#activeStatusLabel').text('Inactive');
        //    }
        //}
    }
    // Clear error messages (common for both add & edit modes)
    $('.input_error_msg').text('');
}
$(document).ready(function () {
    $(document).on('click', '#addCompanyCorrespondanceButton', function () { 
        $('#toggleContainer').hide();
        resetForm(true);
        setFormTitle(false);         
        //$('#editCorrespondanceOffcanvas').offcanvas('show');
        // Ensure Bootstrap Offcanvas is properly initialized
        var offcanvasElement = document.getElementById('editCorrespondanceOffcanvas');
        var offcanvasInstance = bootstrap.Offcanvas.getInstance(offcanvasElement);

        if (!offcanvasInstance) {
            offcanvasInstance = new bootstrap.Offcanvas(offcanvasElement);
        }

        offcanvasInstance.show(); // Show Offcanvas

        // Fetch countries and populate the country dropdown
        fetchCompanyCorrespondanceDropdownData('/DropDown/FetchCommonCountriesDropdown', {}, '#companyCorrespondanceCountryDropdown', 'Select Country')
            .done(function () {
                // Trigger change only after the country dropdown is populated
                const selectedCountryId = $('#companyCorrespondanceCountryDropdown').val();
                if (selectedCountryId) {
                    $('#companyCorrespondanceCountryDropdown').trigger('change'); // Trigger change to load states
                }
            });
    });
    $('#btnResetCompanyCorrespondance').click(function () {
        let correspondanceResetId = $('#Correspondance_ID').val(); // Get Correspondance_ID

        if (correspondanceResetId === '' || correspondanceResetId === null) {
            // ADD MODE - Clear all fields
            $('#Correspondance_ID').val('');
            // $('#Company_Id').val('');
            // $('#CreatedBy').val('');
            $('#CompanyAddress').val('');
            $('#Building_No').val('');
            $('#Building_Name').val('');
            $('#Street').val('');
            $('#Primary_Phone_No').val('');
            $('#Secondary_Phone_No').val('');
            $('#Primary_Email_Id').val('');
            $('#Secondary_Email_ID').val('');
            $('#Website_url').val('');
            $('#companyCorrespondanceCountryDropdown').val('').trigger('change');
            $('#companyCorrespondanceStateDropdown').val('').trigger('change');
            $('#companyCorrespondanceCityDropdown').val('').trigger('change');
            $('#companyCorrespondanceLocationDropdown').val('').trigger('change');
        } else {            
            resetCompanyCorrespondance(correspondanceResetId);
        }
    });
});
//////////////////////////////////////////////////////////////Validation Region:-Start////////////////////////////////////////////////////
function validateCompanyAddress() {
    var rawCompanyAddress = $('#CompanyAddress').val(); // Get raw input
    var companyAddress = rawCompanyAddress.trim(); // Trimmed for length validation

    // Check for empty after trim
    if (companyAddress === "") {
        $("#CompanyAddress-error").text("Please provide company address.");
        return false;
    }

    // Check for leading or trailing whitespace in raw input
    if (/^\s|\s$/.test(rawCompanyAddress)) {
        $("#CompanyAddress-error").text("Please provide company address.");
        return false;
    }

    // Check length after trimming
    if (companyAddress.length > 400) {
        $("#CompanyAddress-error").text("Company address must not exceed 400 characters.");
        return false;
    }

    $("#CompanyAddress-error").text(""); // Clear error if valid
    return true;
}
function validateBuildingNo() {
    var rawBuildingNo = $('#Building_No').val(); // Get raw input

    if (!rawBuildingNo || rawBuildingNo.trim() === "") {
        // 🔹 Field is empty or null — considered valid (nullable in DB)
        $("#Building-No-error").text(""); // Clear error if any
        return true;
    }

    var buildingNo = rawBuildingNo.trim(); // Trimmed value

    // Check for leading or trailing whitespace in raw input
    if (/^\s|\s$/.test(rawBuildingNo)) {
        $("#Building-No-error").text("Please provide building no.");
        return false;
    }

    if (buildingNo.length > 20) {
        $("#Building-No-error").text("Building no cannot exceed 20 characters.");
        return false;
    }

    $("#Building-No-error").text(""); // Clear error if valid
    return true;
}
function validateBuildingName() {
    var rawBuildingName = $('#Building_Name').val(); // Raw input
    if (!rawBuildingName) return true; // ✅ Skip validation if empty/null

    var buildingName = rawBuildingName.trim(); // Trimmed version

    // Check for leading/trailing whitespace
    if (/^\s|\s$/.test(rawBuildingName)) {
        $("#Building-Name-error").text("Please provide building name.");
        return false;
    }

    if (buildingName.length > 100) {
        $("#Building-Name-error").text("Building name cannot exceed 100 characters.");
        return false;
    }

    $("#Building-Name-error").text(""); // Clear error
    return true;
}
function validateStreet() {
    var rawStreet = $('#Street').val(); // Get raw input

    if (!rawStreet || rawStreet.trim() === "") {
        $("#Street-error").text(""); // Clear error if any
        return true;
    }

    var objStreet = rawStreet.trim(); // Trimmed value

    // Check for leading or trailing whitespace in raw input
    if (/^\s|\s$/.test(objStreet)) {
        $("#Street-error").text("Please provide street.");
        return false;
    }

    if (objStreet.length > 100) {
        $("#Street-error").text("Street cannot exceed 100 characters.");
        return false;
    }

    $("#Street-error").text(""); // Clear error if valid
    return true;
}

function validateCountry() {
    var countryId = $('#companyCorrespondanceCountryDropdown').val();
    if (!countryId || countryId === "") {
        $("#companyCorrespondancecountryDropdown-error").text("Please select a country.");
        return false;
    }
    $("#companyCorrespondancecountryDropdown-error").text(""); // Clear error if valid
    return true;
}
function validateState() {
    var stateId = $('#companyCorrespondanceStateDropdown').val();
    if (!stateId || stateId === "") {
        $("#companyCorrespondancestateDropdown-error").text("Please select a state.");
        return false;
    }
    $("#companyCorrespondancestateDropdown-error").text(""); // Clear error if valid
    return true;
}
function validateCity() {
    var cityId = $('#companyCorrespondanceCityDropdown').val();
    if (!cityId || cityId === "") {
        $("#companyCorrespondancecityDropdown-error").text("Please select a city.");
        return false;
    }
    $("#companyCorrespondancecityDropdown-error").text(""); // Clear error if valid
    return true;
}
function validateLocation() {
    var locationId = $('#companyCorrespondanceLocationDropdown').val();
    if (!locationId || locationId === "") {
        $("#companyCorrespondanceLocationDropdown-error").text("Please select a location.");
        return false;
    }
    $("#companyCorrespondanceLocationDropdown-error").text(""); // Clear error if valid
    return true;
}
function validatePrimaryPhoneNo() {
    var rawPrimaryPhoneNo = $('#Primary_Phone_No').val(); // Get raw input
    var primaryPhoneNo = rawPrimaryPhoneNo.trim(); // Trimmed version for validation
    var phoneRegex = /^[0-9]{10}$/;

    if (primaryPhoneNo === "") {
        $("#Primary-Phone-No-error").text("Please provide primary phone no.");
        return false;
    }

    // Check for leading or trailing whitespace in raw input
    if (/^\s|\s$/.test(rawPrimaryPhoneNo)) {
        $("#Primary-Phone-No-error").text("Please provide primary phone no.");
        return false;
    }

    if (!phoneRegex.test(primaryPhoneNo)) {
        $("#Primary-Phone-No-error").text("Please enter a valid 10-digit phone number.");
        return false;
    }

    $("#Primary-Phone-No-error").text(""); // Clear error if valid
    return true;
}
function validateSecondaryPhoneNo() {
    var rawSecondaryPhone = $('#Secondary_Phone_No').val(); // Get raw input
    var secondaryPhone = rawSecondaryPhone.trim(); // Trimmed value for validation
    var phoneRegex = /^[0-9]{10}$/;

    $("#Secondary-Phone-No-error").text(""); // Clear previous error message

    if (rawSecondaryPhone !== "") {
        if (/^\s|\s$/.test(rawSecondaryPhone)) {
            $("#Secondary-Phone-No-error").text("Please provide secondary phone number.");
            return false;
        }

        if (!phoneRegex.test(secondaryPhone)) {
            $("#Secondary-Phone-No-error").text("Please enter a valid 10-digit phone number.");
            return false;
        }
    }

    return true; // Valid if empty or matches all checks
}

function validatePrimaryEmailId() {
    var rawEmail = $('#Primary_Email_Id').val(); // Untrimmed input
    var primaryEmail = rawEmail.trim(); // Trimmed version for validation

    // Email regex explanation:
    // - Local part: No spaces, starts with alphanum, allows dot/underscore/hyphen in between
    // - Domain: Labels (e.g., example.com), no label starts/ends with hyphen, each label starts with alphanum
    var emailRegex = /^[a-zA-Z0-9](?!.*\.\.)[a-zA-Z0-9._%+-]*[a-zA-Z0-9]@[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*(\.[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*)+$/;

    $("#Primary-Email-Id-error").text(""); // Clear previous errors

    if (rawEmail === "") {
        $("#Primary-Email-Id-error").text("Please provide primary email ID.");
        return false;
    }

    // Check if raw input starts with whitespace, dot, or @
    if (/^[\s.@]/.test(rawEmail)) {
        $("#Primary-Email-Id-error").text("Please provide primary email ID.");
        return false;
    }

    // Check if input contains consecutive dots
    if (/\.\./.test(primaryEmail)) {
        $("#Primary-Email-Id-error").text("Email ID cannot contain consecutive dots.");
        return false;
    }

    if (primaryEmail.length > 100) {
        $("#Primary-Email-Id-error").text("Email ID should not exceed 100 characters.");
        return false;
    }

    // Validate using improved regex
    if (!emailRegex.test(primaryEmail)) {
        $("#Primary-Email-Id-error").text("Please enter a valid email address (e.g., example@domain.com).");
        return false;
    }
    return true; // Valid
}


function validateSecondaryEmailId() {
    var rawSecondaryEmail = $('#Secondary_Email_ID').val(); // Untrimmed input
    var secondaryEmail = rawSecondaryEmail.trim(); // Trimmed version for length check & regex
    // var emailRegex = /^[^\s@.][^\s@]*@[^\s@]+\.[^\s@]+$/; // First character must not be space, dot, or @
    // Email regex explanation:
    // - Local part: No spaces, starts with alphanum, allows dot/underscore/hyphen in between
    // - Domain: Labels (e.g., example.com), no label starts/ends with hyphen, each label starts with alphanum
    var emailRegex = /^[a-zA-Z0-9](?!.*\.\.)[a-zA-Z0-9._%+-]*[a-zA-Z0-9]@[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*(\.[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*)+$/;


    $("#Secondary-Email-ID-error").text(""); // Clear previous errors

    if (rawSecondaryEmail !== "") {  // Validate only if value is entered
        if (/^[\s.@]/.test(rawSecondaryEmail)) {
            $("#Secondary-Email-ID-error").text("Please provide secondary email ID.");
            return false;
        }
        // Check if raw input starts with whitespace, dot, or @
        if (/^[\s.@]/.test(rawSecondaryEmail)) {
            $("#Secondary-Email-ID-error").text("Please provide secondary email ID.");
            return false;
        }

        // Check if input contains consecutive dots
        if (/\.\./.test(rawSecondaryEmail)) {
            $("#Secondary-Email-ID-error").text("Email ID cannot contain consecutive dots.");
            return false;
        }
        if (secondaryEmail.length > 100) {
            $("#Secondary-Email-ID-error").text("Email ID should not exceed 100 characters.");
            return false;
        }

        if (!emailRegex.test(secondaryEmail)) {
            $("#Secondary-Email-ID-error").text("Please enter a valid email address (e.g., example@domain.com).");
            return false;
        }
    }

    return true;  // Valid if empty or passes all checks
}

function validateWebsiteUrl() {
    var companyWebsiteUrl = $('#Website_url').val();
    //var urlRegex = /^(https?:\/\/)?([\w-]+\.)+[\w-]{2,}(\/[\w-./?%&=]*)?$/i;
    //var urlRegex = /^(https?:\/\/)(www\.)?([a-zA-Z0-9]+(-?[a-zA-Z0-9])*\.)+[a-zA-Z]{2,}(\/[\w\-./?%&=]*)?$/;
    var urlRegex = /^(https?:\/\/)?(www\.)?[a-zA-Z0-9-]+\.[a-zA-Z0-9-]+\.[a-zA-Z]{2,}$/;


    if (companyWebsiteUrl !== "") {
        if (companyWebsiteUrl.length > 100) {
            $("#Website-url-error").text("Website URL must not exceed 100 characters.");
            return false;
        }
        if (!urlRegex.test(companyWebsiteUrl)) {
            $("#Website-url-error").text("Please enter a valid Website URL.");
            return false;
        }
    }
    $("#Website-url-error").text(""); // Clear error if valid
    return true;
}
function validateAndCollectCorrespondanceDataForEdit() {
    var isValid = true;
    $(".input_error_msg").text("");
   
    if (!validateCompanyAddress()) isValid = false;
    if (!validateBuildingNo()) isValid = false;
    if (!validateBuildingName()) isValid = false;
    if (!validateStreet()) isValid = false;    
    if (!validatePrimaryPhoneNo()) isValid = false;
    if (!validateSecondaryPhoneNo()) isValid = false;
    if (!validateCountry()) isValid = false;
    if (!validateState()) isValid = false;
    if (!validateCity()) isValid = false;
    if (!validateLocation()) isValid = false;
    if (!validateWebsiteUrl()) isValid = false;
    if (!validatePrimaryEmailId()) isValid = false;
    if (!validateSecondaryEmailId()) isValid = false;
    var formData = {
        Correspondance_ID: $('#Correspondance_ID').val(),
        Company_Id: $('#Company_Id').val(),
        CompanyAddress: $('#CompanyAddress').val(),
        Building_No: $('#Building_No').val(),
        Building_Name: $('#Building_Name').val(),
        Street: $('#Street').val(),
        Country_ID: $('#companyCorrespondanceCountryDropdown').val(),
        State_Id: $('#companyCorrespondanceStateDropdown').val(),
        City_ID: $('#companyCorrespondanceCityDropdown').val(),
        Location_ID: $('#companyCorrespondanceLocationDropdown').val(),
        Primary_Phone_no: $('#Primary_Phone_No').val(),
        Secondary_Phone_No: $('#Secondary_Phone_No').val(),
        Primary_Email_Id: $('#Primary_Email_Id').val(),
        Secondary_Email_ID: $('#Secondary_Email_ID').val(),
        WebsiteUrl: $('#Website_url').val(),
        CreatedBy: $('#CreatedBy').val(),
        IsActive: $('#companyCorrespondanceActiveToggle').is(':checked')
    };   
    return { isValid, data: formData };
}
function validateAndCollectCorrespondanceDataForAdd() {
    var isValid = true;
    $(".input_error_msg").text("");
    if (!validateCompanyAddress()) isValid = false;
    if (!validateBuildingNo()) isValid = false;
    if (!validateBuildingName()) isValid = false;
    if (!validateStreet()) isValid = false;
    if (!validatePrimaryPhoneNo()) isValid = false;
    if (!validateSecondaryPhoneNo()) isValid = false;
    if (!validateCountry()) isValid = false;
    if (!validateState()) isValid = false;
    if (!validateCity()) isValid = false;
    if (!validateLocation()) isValid = false;
    if (!validateWebsiteUrl()) isValid = false;
    if (!validatePrimaryEmailId()) isValid = false;
    if (!validateSecondaryEmailId()) isValid = false;
    var formData = {
        Correspondance_ID: $('#Correspondance_ID').val(),
        Company_Id: $('#GlobalCompany_Id').val(),
        CompanyAddress: $('#CompanyAddress').val(),
        Building_No: $('#Building_No').val(),
        Building_Name: $('#Building_Name').val(),
        Street: $('#Street').val(),
        Country_ID: $('#companyCorrespondanceCountryDropdown').val(),
        State_Id: $('#companyCorrespondanceStateDropdown').val(),
        City_ID: $('#companyCorrespondanceCityDropdown').val(),
        Location_ID: $('#companyCorrespondanceLocationDropdown').val(),
        Primary_Phone_no: $('#Primary_Phone_No').val(),
        Secondary_Phone_No: $('#Secondary_Phone_No').val(),
        Primary_Email_Id: $('#Primary_Email_Id').val(),
        Secondary_Email_ID: $('#Secondary_Email_ID').val(),
        WebsiteUrl: $('#Website_url').val(),
       // CreatedBy: $('#CreatedBy').val(),
        IsActive: $('#companyCorrespondanceActiveToggle').is(':checked')
    };
    //let isValid = true;
    return { isValid, data: formData };
}
//////////////////////////////////////////////////////////////Validation Region:-End////////////////////////////////////////////////////
function setFormTitle(isTitleEditMode) {
    //console.log("Title", isTitleEditMode);
    const title = isTitleEditMode ? "Update Company Correspondence Details" : "Add Company Correspondence Details";
    $('#editCorrespondanceOffcanvasLabel').text(title); // Assuming the form title has an ID `formTitle`
}
function fetchCompanyCorrespondanceDropdownData(url, data, dropdownId, placeholderText) {
    const deferred = $.Deferred();
    $.ajax({
        url: url,
        type: 'GET',
        data: data,
        async: false,
        success: function (response) {
            //console.log(`Dropdown data for ${dropdownId}:`, response);
            populateCompanyCorrespondanceDropdown(dropdownId, response, placeholderText);
            deferred.resolve();
        },
        error: function () {
            console.error(`Failed to fetch data for ${dropdownId}`);
            deferred.reject();
        }
    });
    return deferred.promise();
}
function fetchCompanyCorrespondanceStateDropdown(countryId, stateId) {
    //console.log("fetchStateDropdown", countryId, stateId);
    return fetchCompanyCorrespondanceDropdownData('/DropDown/FetchCommonStateDropdown', { Country_Id: countryId }, '#companyCorrespondanceStateDropdown', 'Select State')
        .done(() => {
            if (stateId) {
                $('#companyCorrespondanceStateDropdown').val(stateId).trigger('change');
            }
        });
}
function fetchCompanyCorrespondanceCityDropdown(state_Id, city_ID) {
    //console.log("fetchCityDropdown called with:", state_Id, city_ID);
    return fetchCompanyCorrespondanceDropdownData('/DropDown/FetchCommonCityDropdown', { State_ID: state_Id }, '#companyCorrespondanceCityDropdown', 'Select City')
        .done(() => {
            //console.log("Setting city dropdown value:", city_ID);
            if (city_ID) {
                $('#companyCorrespondanceCityDropdown').val(city_ID).trigger('change');
            }
        });
}
function fetchCompanyCorrespondanceLocationDropdown(city_ID, location_ID) {
    //console.log("fetchLocationDropdown called with:", city_ID, location_ID);
    return fetchCompanyCorrespondanceDropdownData('/DropDown/FetchCommonLocationsDropdown', { City_ID: city_ID }, '#companyCorrespondanceLocationDropdown', 'Select Location')
        .done(() => {
            //console.log("Setting location dropdown value:", location_ID);
            if (location_ID) {
                $('#companyCorrespondanceLocationDropdown').val(location_ID).trigger('change');
            }
        });
}
function populateCompanyCorrespondanceDropdown(dropdownId, items, placeholderText) {
    const dropdown = $(dropdownId);
    dropdown.empty().append(new Option(placeholderText, ''));
    items.forEach(item => {
        dropdown.append(new Option(item.text, item.value));
    });
}
function populateCompanyCorrespondanceDropdownValues(data) {
        fetchCompanyCorrespondanceDropdownData('/DropDown/FetchCommonCountriesDropdown', {}, '#companyCorrespondanceCountryDropdown', 'Select Country')
        .done(() => {         
            $('#companyCorrespondanceCountryDropdown').val(data.country_ID).trigger('change'); // Set value and trigger change
            return fetchCompanyCorrespondanceStateDropdown(data.country_ID, data.state_Id || '');
        })
        .done(() => {
            return fetchCompanyCorrespondanceCityDropdown(data.state_Id || '', data.city_ID || '');
        })
        .done(() => {
            return fetchCompanyCorrespondanceLocationDropdown(data.city_ID || '', data.location_ID || '');
        })
            .fail(() => console.log('Error loading dropdown data.'));
}
function editCorrespondance(correspondanceId) {
    $(".input_error_msg").text("");
    $.ajax({
        url: '/Company/GetCorrespondanceDetails?correspondanceId=' + correspondanceId,
        type: 'GET',
        success: function (response) {
            //console.log("AJAX response: ", response);  // Log the response
            if (response.success) {
                const companyCorrespondance = response.data;
                $('#toggleContainer').show(); 
                if (!companyCorrespondance) {
                    //console.log('Error: Data not found.');
                    return;
                }

                // Populate fields
                $('#Correspondance_ID').val(companyCorrespondance.correspondance_ID);
                $('#Company_Id').val(companyCorrespondance.company_Id);
                $('#CreatedBy').val(companyCorrespondance.createdBy);
                $('#CompanyAddress').val(companyCorrespondance.companyAddress);
                $('#Building_No').val(companyCorrespondance.building_No);
                $('#Building_Name').val(companyCorrespondance.building_Name);
                $('#Street').val(companyCorrespondance.street);

                // Dropdown fields (with trigger change to update dependent values)
                $('#companyCorrespondanceCountryDropdown').val(companyCorrespondance.country_ID).trigger('change');
                $('#companyCorrespondanceStateDropdown').val(companyCorrespondance.state_Id).trigger('change');
                $('#companyCorrespondanceCityDropdown').val(companyCorrespondance.city_ID).trigger('change');
                $('#companyCorrespondanceLocationDropdown').val(companyCorrespondance.location_ID).trigger('change');

                // Phone numbers & Emails
                $('#Primary_Phone_No').val(companyCorrespondance.primary_Phone_no);
                $('#Secondary_Phone_No').val(companyCorrespondance.secondary_Phone_No);
                $('#Primary_Email_Id').val(companyCorrespondance.primary_Email_Id);
                $('#Secondary_Email_ID').val(companyCorrespondance.secondary_Email_ID);

                // Website URL
                $('#Website_url').val(companyCorrespondance.websiteUrl);

                // Set the active/inactive toggle
                $('#companyCorrespondanceActiveToggle').prop('checked', companyCorrespondance.isActive);
                $('#companyCorrespondanceStatusLabel').text(companyCorrespondance.isActive ? 'Active' : 'Inactive');

                // Show the offcanvas
                setFormTitle(true); // Set the title to "Update Company Correspondance Details"
                var offcanvasElement = document.getElementById('editCorrespondanceOffcanvas');
                var myOffcanvas = new bootstrap.Offcanvas(offcanvasElement);
                myOffcanvas.show();
                populateCompanyCorrespondanceDropdownValues(response.data);
                //resetForm(false);
            }
          /*  $('#editCorrespondanceContainer').html(response);*/
           
        },
        error: function () {
            //console.log('Error fetching data.');
        }
    });
}
function resetCompanyCorrespondance(correspondanceId) {
    $.ajax({
        url: '/Company/GetCorrespondanceDetails?correspondanceId=' + correspondanceId,
        type: 'GET',
        success: function (response) {
            //console.log("AJAX response: ", response);  // Log the response
            if (response.success) {
                const companyCorrespondance = response.data;

                if (!companyCorrespondance) {
                    //console.log('Error: Data not found.');
                    return;
                }

                // Populate fields
                $('#Correspondance_ID').val(companyCorrespondance.correspondance_ID);
                $('#Company_Id').val(companyCorrespondance.company_Id);
                $('#CreatedBy').val(companyCorrespondance.createdBy);
                $('#CompanyAddress').val(companyCorrespondance.companyAddress);
                $('#Building_No').val(companyCorrespondance.building_No);
                $('#Building_Name').val(companyCorrespondance.building_Name);
                $('#Street').val(companyCorrespondance.street);

                // Dropdown fields (with trigger change to update dependent values)
                $('#companyCorrespondanceCountryDropdown').val(companyCorrespondance.country_ID).trigger('change');
                $('#companyCorrespondanceStateDropdown').val(companyCorrespondance.state_Id).trigger('change');
                $('#companyCorrespondanceCityDropdown').val(companyCorrespondance.city_ID).trigger('change');
                $('#companyCorrespondanceLocationDropdown').val(companyCorrespondance.location_ID).trigger('change');

                // Phone numbers & Emails
                $('#Primary_Phone_No').val(companyCorrespondance.primary_Phone_no);
                $('#Secondary_Phone_No').val(companyCorrespondance.secondary_Phone_No);
                $('#Primary_Email_Id').val(companyCorrespondance.primary_Email_Id);
                $('#Secondary_Email_ID').val(companyCorrespondance.secondary_Email_ID);

                // Website URL
                $('#Website_url').val(companyCorrespondance.websiteUrl);

                // Set the active/inactive toggle
                $('#companyCorrespondanceActiveToggle').prop('checked', companyCorrespondance.isActive);
                $('#companyCorrespondanceStatusLabel').text(companyCorrespondance.isActive ? 'Active' : 'Inactive');

               
                populateCompanyCorrespondanceDropdownValues(response.data);              
            }
            /*  $('#editCorrespondanceContainer').html(response);*/

        },
        error: function () {
            //console.log('Error fetching data.');
        }
    });
}

