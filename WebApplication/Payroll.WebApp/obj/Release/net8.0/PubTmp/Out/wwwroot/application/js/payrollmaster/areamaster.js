var originalEditFormData = {}; // Object to store original form values
$(document).ready(function () {
    initializeForms();
    // Initialize select2 for all dropdowns with search functionality
    $('.select2_search_ctm').select2({
        placeholder: function () {
            return $(this).data('placeholder'); // Set dynamic placeholder from data-placeholder attribute
        },
        allowClear: true,  // Allows clearing the selection (if needed)
        multiple: false,   // Ensure it's a single select dropdown
        dropdownAutoWidth: true,  // Auto adjust dropdown width
        width: '100%'      // Ensures the dropdown takes up full width of its container
    });
    $(document).on("click", ".edit-btn[data-bs-target='#EditArea']", function () {
        var areaId = $(this).data("id");
        if (areaId) {
            showEditAreaForm(areaId);
        } else {
            console.error("Area ID missing!");
        }
    });
    $(document).on("click", "#addAreaButton", function () {
        resetAddForm();
        showAddAreaForm();
    });

    // Reset Add Area Form
    $(document).on("click", "#resetButton", function () {
        $("#addAreaForm")[0].reset(); // Reset the form
        $(".select2_search_ctm").val(null).trigger("change"); // Reset Select2 dropdowns
        $(".input_error_msg").text(""); // Clear error messages

        // Reinitialize dropdowns
        $("#CountryDropdown").trigger("change");
    });

    // Reset Edit Form to Initial Values
    $(document).on("click", "#editresetButton", function () {
        resetEditForm();
    });

    // Clear error messages on dropdown change
    $(document).on('change', '#CountryDropdown, #StateDropdown, #CityDropdown, #LocationDropdown, #EditCountryDropdown, #EditStateDropdown, #EditCityDropdown, #EditLocationDropdown', function () {
        var dropdownId = $(this).attr('id');
        var formPrefix = dropdownId.startsWith('Edit') ? 'edit' : ''; // Check if it's an edit form
        var errorId = formPrefix + dropdownId + '-error';

        hideError(errorId);
    });

    // Dropdown Change Events for Add Form
    $(document).on('change', '#CountryDropdown', function () {
        var countryId = $(this).val();
        loadStates(countryId, '#StateDropdown', function () {
            $('#StateDropdown').trigger('change');
        });
    });

    $(document).on('change', '#StateDropdown', function () {
        var stateId = $(this).val();
        loadCities(stateId, '#CityDropdown', function () {
            $('#CityDropdown').trigger('change');
        });
    });

    $(document).on('change', '#CityDropdown', function () {
        var cityId = $(this).val();
        loadLocations(cityId, '#LocationDropdown');
    });

    // Dropdown Change Events for Edit Form
    $(document).on('change', '#EditCountryDropdown', function () {
        var countryId = $(this).val();
        loadStates(countryId, '#EditStateDropdown', function () {
            $('#EditStateDropdown').trigger('change');
        });
    });

    $(document).on('change', '#EditStateDropdown', function () {
        var stateId = $(this).val();
        loadCities(stateId, '#EditCityDropdown', function () {
            $('#EditCityDropdown').trigger('change');
        });
    });

    $(document).on('change', '#EditCityDropdown', function () {
        var cityId = $(this).val();
        loadLocations(cityId, '#EditLocationDropdown');
    });



});

// Initialize dropdowns for both Add and Edit forms
function initializeForms() {
    loadCountries('#CountryDropdown', function () {
        $('#CountryDropdown').trigger('change');
    });

    loadCountries('#EditCountryDropdown', function () {
        $('#EditCountryDropdown').trigger('change');
    });
}

// Load countries dropdown
function loadCountries(targetDropdown, callback) {
    $.ajax({
        url: '/DropDown/FetchCountriesDropdown',
        method: 'GET',
        success: function (data) {
            populateDropdown(targetDropdown, data);
            if (callback) callback();
        },
        error: function () {
            showError(targetDropdown + '-error', 'Failed to load countries.');
        }
    });
}

// Load states dropdown based on selected country
function loadStates(countryId, targetDropdown, callback) {
    if (!countryId) return;
    $.ajax({
        url: '/DropDown/FetchStateDropdown/' + countryId,
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

// Load cities dropdown based on selected state
function loadCities(stateId, targetDropdown, callback) {
    if (!stateId) return;
    $.ajax({
        url: '/DropDown/FetchCityDropdown/' + stateId,
        method: 'GET',
        success: function (data) {
            populateDropdown(targetDropdown, data);
            if (callback) callback();
        },
        error: function () {
            showError(targetDropdown + '-error', 'Failed to load cities.');
        }
    });
}

// Load locations dropdown based on selected city
function loadLocations(cityId, targetDropdown, callback) {
    if (!cityId) return;
    $.ajax({
        url: '/DropDown/FetchLocationsDropdown/' + cityId,
        method: 'GET',
        success: function (data) {
            populateDropdown(targetDropdown, data);
            if (callback) callback();
        },
        error: function () {
            showError(targetDropdown + '-error', 'Failed to load locations.');
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

// Show Add Area Form
function showAddAreaForm() {
    $('#addAreaForm')[0].reset();
    $('#addArea').show();
    $('#EditArea').hide();

    loadCountries('#CountryDropdown', function () {
        $('#CountryDropdown').trigger('change');
    });
}

// Show Edit Area Form
function showEditAreaForm(areaId) {
    /* $('#editAreaForm')[0].reset();*/
    $('#EditArea').show();
    $('#addArea').hide();

    $.ajax({
        url: '/PayrollMaster/GetAreaDetailsById/' + areaId,
        method: 'GET',
        success: function (response) {
            var area = response.data;
            if (response) {
                $("#areaId").val(area.area_Id);
                $("#EditCountryDropdown").val(area.countryId).trigger("change");
                $("#EditStateDropdown").val(area.state_Id).trigger("change");
                $("#EditCityDropdown").val(area.cityid).trigger("change");
                $("#EditLocationDropdown").val(area.location_Id).trigger("change");
                $("#editareaname").val(area.areaName);
                $("#EditAreaIsActive").prop("checked", area.isActive);
                $("#isActiveLabel").text(response.isActive ? "Active" : "Inactive");

                // Store Original Values
                originalEditFormData = {
                    area_Id: area.area_Id,
                    countryId: area.countryId,
                    state_Id: area.state_Id,
                    cityid: area.cityid,
                    location_Id: area.location_Id,
                    areaName: area.areaName,
                    isActive: area.isActive
                };
            }
            loadCountries('#EditCountryDropdown', function () {
                $('#EditCountryDropdown').val(area.countryId).trigger('change');

                loadStates(area.countryId, '#EditStateDropdown', function () {
                    $('#EditStateDropdown').val(area.state_Id).trigger('change');

                    loadCities(area.state_Id, '#EditCityDropdown', function () {
                        $('#EditCityDropdown').val(area.cityid).trigger('change');

                        loadLocations(area.cityid, '#EditLocationDropdown', function () {
                            $('#EditLocationDropdown').val(area.location_Id).trigger('change');
                        });
                    });
                });
            });

            $('#areaId').val(area.area_Id);
            $('#editareaname').val(area.areaName);
            // Set the checkbox state and label
            var isActive = area.isActive;
            $('#EditAreaIsActive').prop('checked', isActive);
            updateIsActiveLabel(isActive); // Update the label based on IsActive value
        },
        error: function (error) {
            if (error.status === 401) {
                window.location.href = "/Account/LoginPage"; // Redirect if session expired
            } else {
                alert('An error occurred: ' + error.statusText);
            }
        }
    });
}
function resetAddForm() {
    $("#areaId").val('');
    $("#editareaname").val('');
    // $("#EditAreaIsActive").prop("checked", originalEditFormData.isActive);
    // $("#isActiveLabel").text(originalEditFormData.isActive ? "Active" : "Inactive");

    // Reset and Trigger Country Dropdown
    $("#EditCountryDropdown").val('').trigger("change");

    $("#EditStateDropdown").val('').trigger("change");

    $("#EditCityDropdown").val('').trigger("change");

    $("#EditLocationDropdown").val('').trigger("change");
}
// Function to Reset Edit Form to Stored Values
function resetEditForm() {
    $("#areaId").val(originalEditFormData.area_Id);
    $("#editareaname").val(originalEditFormData.areaName);
    $("#EditAreaIsActive").prop("checked", originalEditFormData.isActive);
    $("#isActiveLabel").text(originalEditFormData.isActive ? "Active" : "Inactive");

    // Reset and Trigger Country Dropdown
    $("#EditCountryDropdown").val(originalEditFormData.countryId).trigger("change");

    loadStates(originalEditFormData.countryId, "#EditStateDropdown", function () {
        $("#EditStateDropdown").val(originalEditFormData.state_Id).trigger("change");

        loadCities(originalEditFormData.state_Id, "#EditCityDropdown", function () {
            $("#EditCityDropdown").val(originalEditFormData.cityid).trigger("change");

            loadLocations(originalEditFormData.cityid, "#EditLocationDropdown", function () {
                $("#EditLocationDropdown").val(originalEditFormData.location_Id).trigger("change");
            });
        });
    });
}

function resetAddForm() {
    $("#areaId").val('');
    $("#editareaname").val('');
    //  $("#EditAreaIsActive").prop("checked", originalEditFormData.isActive);
    // $("#isActiveLabel").text(originalEditFormData.isActive ? "Active" : "Inactive");

    // Reset and Trigger Country Dropdown
    $("#EditCountryDropdown").val('').trigger("change");

    $("#EditStateDropdown").val('').trigger("change");

    $("#EditCityDropdown").val('').trigger("change");

    $("#EditLocationDropdown").val('').trigger("change");
}

// Toggle label based on the IsActive checkbox
function updateIsActiveLabel(isActive) {
    if (isActive) {
        $('#isActiveLabel').text('Active');
    } else {
        $('#isActiveLabel').text('Inactive');
    }
}

// Listen for checkbox toggle and update label
$(document).on('change', '#EditAreaIsActive', function () {
    var isChecked = $(this).prop('checked');
    updateIsActiveLabel(isChecked);
});

// Ensure checkbox reflects correct state when page is loaded
$(document).ready(function () {
    var isActive = $('#EditAreaIsActive').prop('checked');
    updateIsActiveLabel(isActive);
});
// Show error message for dropdowns
function showError(elementId, message) {
    $('#' + elementId).text(message).show();
}

//// Function to hide error message
function hideError(elementId) {
    $('#' + elementId).hide();
}
function validateAreaForm(formType) {
    var isValid = true;
    var formPrefix = formType === 'add' ? '' : 'edit';


    // Validate Area Name
    var areaName = $('#' + formPrefix + 'areaname').val();
    var areaNameError = validateTextField(areaName);

    if (areaName === '') {
        showError(formPrefix + 'areaname-error', 'Please enter the Area Name.');
        isValid = false;
    } else if (areaNameError) {
        showError(formPrefix + 'areaname-error', areaNameError);
        isValid = false;
    } else {
        hideError(formPrefix + 'areaname-error');
    }


    // Validate Country Dropdown
    var countryValue = $('#' + formPrefix + 'CountryDropdown').val();
    if (countryValue === '' || countryValue === null) {
        showError(formPrefix + 'countryDropdown-error', 'Please select the Country.');
        isValid = false;
    } else {
        hideError(formPrefix + 'countryDropdown-error');
    }

    // Validate State Dropdown
    var stateValue = $('#' + formPrefix + 'StateDropdown').val();
    if (stateValue === '' || stateValue === null) {
        showError(formPrefix + 'stateDropdown-error', 'Please select the State.');
        isValid = false;
    } else {
        hideError(formPrefix + 'stateDropdown-error');
    }

    // Validate City Dropdown
    var cityValue = $('#' + formPrefix + 'CityDropdown').val();
    if (cityValue === '' || cityValue === null) {
        showError(formPrefix + 'cityDropdown-error', 'Please select the City.');
        isValid = false;
    } else {
        hideError(formPrefix + 'cityDropdown-error');
    }

    // Validate Location Dropdown
    var locationValue = $('#' + formPrefix + 'LocationDropdown').val();
    if (locationValue === '' || locationValue === null) {
        showError(formPrefix + 'locationDropdown-error', 'Please select the Location.');
        isValid = false;
    } else {
        hideError(formPrefix + 'locationDropdown-error');
    }

    return isValid;
}

// Function to clear error messages immediately when user selects a valid option
function clearErrorOnSelect(dropdownId, errorId) {
    $('#' + dropdownId).on('change', function () {
        var selectedValue = $(this).val();
        if (selectedValue !== '' && selectedValue !== null) {
            hideError(errorId);
        }
    });
}

// Bind the clear error function to each dropdown
clearErrorOnSelect('CountryDropdown', 'countryDropdown-error');
clearErrorOnSelect('StateDropdown', 'stateDropdown-error');
clearErrorOnSelect('CityDropdown', 'cityDropdown-error');
clearErrorOnSelect('LocationDropdown', 'locationDropdown-error');
$(document).ready(function () {
    $('#saveDetailsButton').on('click', function (event) {
        // Prevent default form submission if needed
        event.preventDefault();

        if (validateAreaForm('add')) {
            var areaData = {
                CountryId: $('#CountryDropdown').val(),
                State_Id: $('#StateDropdown').val(),
                cityid: $('#CityDropdown').val(),
                Location_Id: $('#LocationDropdown').val(),
                AreaName: $('#areaname').val(),
                IsActive: $('#IsActive').prop('checked')
            };

            $.ajax({
                url: '/PayrollMaster/AreaList',
                method: 'POST',
                data: JSON.stringify(areaData),
                contentType: 'application/json',
                success: function (response) {
                    try {
                        if (response.success) {
                            showAlert("success", response.message);
                            $('#addArea').offcanvas('hide');
                            FetchAreaList();  // Update the area list
                        } else {
                            showAlert("danger", response.message);
                        }
                    } catch (error) {
                        showAlert("danger", "An unexpected error occurred.");
                    }
                },
                error: function (error) {
                    if (error.status === 401) {
                        window.location.href = "/Account/LoginPage"; // Redirect if session expired
                    } else {
                        alert('An error occurred: ' + error.statusText);
                    }
                }
            });
        }
    });
    $('#editDetailsButton').on('click', function () {
        // event.preventDefault(); // Prevent default form submission
        if (validateAreaForm('edit')) {
            var areaData = {
                Area_Id: $('#areaId').val(),
                Location_Id: $('#EditLocationDropdown').val(),
                AreaName: $('#editareaname').val(),
                IsActive: $('#EditAreaIsActive').prop('checked')
            };

            $.ajax({
                url: '/PayrollMaster/UpdateArea',
                method: 'POST',
                data: JSON.stringify(areaData),
                contentType: 'application/json',
                success: function (response) {
                    try {
                        if (response.success) {
                            showAlert("success", response.message);
                            $('#EditArea').offcanvas('hide');  // Hide the offcanvas after 3 seconds
                            FetchAreaList();  // Update the area list after the update

                        } else {
                            showAlert("danger", response.message);
                        }
                    } catch (error) {
                        showAlert("danger", "An unexpected error occurred.");
                    }
                },
                error: function (error) {
                    if (error.status === 401) {
                        window.location.href = "/Account/LoginPage"; // Redirect if session expired
                    } else {
                        alert('An error occurred: ' + error.statusText);
                    }
                }
            });
        }
    });
    function FetchAreaList() {
        $.ajax({
            url: '/PayrollMaster/FetchAreaList', // Ensure this matches your controller route
            type: 'GET',
            success: function (response) {
                let tableId = "area-master-list";
                let table = $('#' + tableId);

                if ($.fn.DataTable.isDataTable(table)) {
                    table.DataTable().destroy(); // Destroy existing instance before updating
                }

                $('#area-master-list tbody').html($(response).find('tbody').html());

                makeDataTable(tableId); // Reinitialize DataTable
            },
            error: function (error) {
                if (error.status === 401) {
                    window.location.href = "/Account/LoginPage"; // Redirect if session expired
                } else {
                    alert('An error occurred: ' + error.statusText);
                }
            }
        });
    }
    var selectedButton = null;
    var isRequestInProgress = false;

    $(document).on('click', '.btn-danger-light-icon[data-bs-target="#deleteArea"]', function () {
        selectedButton = $(this);
    });

    //$('#confirmAreaDelete').on('click', function () {
    $(document).on('click', '#confirmAreaDelete', function () {
        if (isRequestInProgress) return; // Prevent multiple requests
        isRequestInProgress = true;

        if (!selectedButton) {
            console.error("Error: No button was selected.");
            isRequestInProgress = false;
            return;
        }

        var areaId = selectedButton.data('area-id');

        if (!areaId) {
            showAlert("danger", "Invalid area ID.");
            isRequestInProgress = false;
            return;
        }

        var rowId = `row-${areaId}`; // Construct the row ID

        var rowData = { Area_Id: areaId };

        $.ajax({
            url: '/PayrollMaster/DeleteArea',
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(rowData),
            success: function (response) {
                if (response && response.success) {
                    $(`#${rowId}`).fadeOut(500, function () {
                        $(this).remove();
                    });
                    showAlert("success", response.message);
                    FetchAreaList();
                } else {
                    showAlert("danger", response.message || "Failed to delete area. Please try again.");
                }
                $('#deleteArea').modal('hide');
            },
            error: function (error) {
                if (error.status === 401) {
                    window.location.href = "/Account/LoginPage"; // Redirect if session expired
                } else {
                    showAlert("danger", "An error occurred: " + (error.statusText || "Please try again."));
                }
                $('#deleteArea').modal('hide');
            },
            complete: function () {
                isRequestInProgress = false;
                // Ensure modal cleanup to prevent UI glitches
                setTimeout(() => {
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();
                }, 300);
            }
        });
    });
});

