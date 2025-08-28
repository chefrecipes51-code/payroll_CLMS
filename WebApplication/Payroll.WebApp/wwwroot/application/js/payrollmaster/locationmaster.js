$(document).ready(function () {

    $('#locationCountryDropdown').on('change', function () {
        if ($(this).val()) {
            $('#locationcountryDropdown-error').text('');
        }
    });
    // Dynamically set the form mode based on locationId (for example)
    $('.btn[data-bs-dismiss="offcanvas"]').on('click', function () {
        const isEditMode = $('#locationId').val() !== '';  // Check if it's Edit mode based on the presence of locationId
    });
    // Reusable function to clear error messages on input change
    function clearErrorOnChange(selector, errorSelector) {
        $(selector).on('change input', function () {
            if ($(this).val()) {
                $(errorSelector).text('');
            }
        });
    }

    // Clear error messages for dropdowns and inputs
    clearErrorOnChange('#locationCountryDropdown', '#locationcountryDropdown-error');
    clearErrorOnChange('#locationStateDropdown', '#locationstateDropdown-error');
    clearErrorOnChange('#locationCityDropdown', '#locationcityDropdown-error');
    clearErrorOnChange('#locationname', '#locationname-error');

    // Validate and collect form data
    function validateAndCollectFormData() {
        var isValid = true;
        var data = {};
        var fields = [
            { id: '#locationCountryDropdown', errorId: '#locationcountryDropdown-error', errorMsg: 'Please select the Country.', key: 'CountryId' },
            { id: '#locationStateDropdown', errorId: '#locationstateDropdown-error', errorMsg: 'Please select the State.', key: 'StateId' },
            { id: '#locationCityDropdown', errorId: '#locationcityDropdown-error', errorMsg: 'Please select the City.', key: 'CityId' },
            { id: '#locationname', errorId: '#locationname-error', errorMsg: 'Please enter the Location Name.', key: 'LocationName' }
        ];

        $('.input_error_msg').text(''); // Clear all error messages

        fields.forEach(function (field) {
            var value = $(field.id).val();
            if (!value) {
                $(field.errorId).text(field.errorMsg);
                isValid = false;
            } else {
                data[field.key] = value; // Collect valid data
            }
        });

        return { isValid: isValid, data: data };
    }
    // Function to set the form title

    // Save form data (Add or Edit)
    function saveFormData(data, url, method) {
        $.ajax({
            type: method,
            url: url,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(data),
            success: function (response) {
                if (response.success) {
                    showAlert('success', response.message);
                    $('#addLocation').offcanvas('hide'); // Close the form
                    LoadLocationPartial();
                } else {
                    showAlert('danger', response.message || 'An error occurred.');
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
    $(document).on('click', '#addLocationButton', function () {
        setFormTitle(false); // Set the title to "Add Location Details"
        // Reset the form completely for new entry
        $('#locationForm')[0].reset();
        $('#locationId').val(''); // Ensure ID is cleared for Add mode
        $('#locationCountryDropdown').val('').trigger('change');
        $('#locationStateDropdown').val('').trigger('change');
        $('#locationCityDropdown').val('');
        $('#locationname').val('');

        // Hide the active status toggle for Add Form
        $('#toggleContainer').hide();
        // Show the form as a new location
        $('#addLocation').offcanvas('show');
        // Fetch countries and populate the country dropdown
        fetchAddLocationDropdownData('/DropDown/FetchCountriesDropdown', {}, '#locationCountryDropdown', '')
            .done(function () {
                // Trigger change only after the country dropdown is populated
                const selectedCountryId = $('#locationCountryDropdown').val();
                if (selectedCountryId) {
                    $('#locationCountryDropdown').trigger('change'); // Trigger change to load states
                }
            });
    });

    // Country change handler
    $(document).on('change', '#locationCountryDropdown', function () {
        const countryId = $(this).val();
        if (countryId) {
            fetchLocationStateDropdown(countryId); // Fetch states when country changes
        } else {
            // Clear state and city dropdowns if no country is selected
            clearDropdown('#locationStateDropdown', '');
            clearDropdown('#locationCityDropdown', '');
        }
    });

    // State change handler
    $(document).on('change', '#locationStateDropdown', function () {
        const stateId = $(this).val();
        if (stateId) {
            fetchLocationCityDropdown(stateId); // Fetch cities when state changes
        } else {
            // Clear city dropdown if no state is selected
            clearDropdown('#locationCityDropdown', '');
        }
    });

    // Utility function to clear and reset dropdowns
    function clearDropdown(selector, placeholder) {
        $(selector).empty().append(new Option(placeholder, ''));
    }
    // Generic function to fetch dropdown data
    function fetchAddLocationDropdownData(url, params, targetDropdown, placeholder) {
        return $.ajax({
            url: url,
            type: 'GET',
            data: params,
            success: function (response) {
                const dropdown = $(targetDropdown);
                dropdown.empty().append(new Option(placeholder, '')); // Add placeholder
                if (response && Array.isArray(response)) {
                    response.forEach(function (item) {
                        dropdown.append(new Option(item.text, item.value)); // Populate options
                    });
                }
            },
            error: function (error) {
            }
        });
    }
    // Update label based on checkbox status
    // Function to show or hide the toggle button based on mode



    // Click handler for Save button
    // $('#saveLocationButton').on('click', function () {
    $(document).on('click', '#saveLocationButton', function () {
        var formData = validateAndCollectFormData();
        if (formData.isValid) {
            const locationId = $('#locationId').val();
            const locationName = $('#locationname').val();
            const togglestatus = $('#locationActiveToggle').prop('checked');
            const isEdit = locationId && parseInt(locationId) > 0; // Determine if it's an update
            const url = isEdit ? '/PayrollMaster/UpdateLocation' : '/PayrollMaster/AddLocation';
            const method = isEdit ? 'POST' : 'POST';
            // Add LocationId to formData if it's an update
            if (isEdit) {
                formData.data.Location_Id = locationId;
                formData.data.LocationName = locationName;
                formData.data.IsActive = togglestatus;
            }
            saveFormData(formData.data, url, method);
        }
    });

    $(document).on('click', '[id^="tblLocation-"]', function () {
        const locationId = $(this).data('locationid');
        if (locationId) {
            openEditLocationForm(locationId);
        }
    });

    $(document).on('click', '#resetLocationButton', function () {
        const isEditMode = $('#locationId').val() !== '';

        if (isEditMode && originalLocationFormData) {
            $('#locationId').val(originalLocationFormData.Location_Id);

            // Step 1: Set Country and trigger change
            $('#locationCountryDropdown').val(originalLocationFormData.Country_Id).trigger('change');

            fetchLocationStateDropdown(originalLocationFormData.Country_Id, originalLocationFormData.State_Id)
                .done(() => {
                    // Call city dropdown only after state is loaded
                    fetchLocationCityDropdown(originalLocationFormData.State_Id, originalLocationFormData.CityId);
                });
            // Step 4: Restore other fields
            $('#locationname').val(originalLocationFormData.LocationName);
            $('#locationActiveToggle').prop('checked', originalLocationFormData.IsActive);
            $('#activeStatusLabel').text(originalLocationFormData.IsActive ? 'Active' : 'Inactive');
            $('#toggleContainer').show();
        } else {
            $('#locationForm')[0].reset();
            $('#locationCountryDropdown').val('').trigger('change');
            $('#locationStateDropdown').val('').trigger('change');
            $('#locationCityDropdown').val('');
            $('#locationname').val('');
            $('#toggleContainer').hide();
        }
    });
    function waitForCitiesToLoad(callback) {
        const interval = setInterval(() => {
            const cityDropdown = $('#locationCityDropdown');

            // Ensure options are loaded and not just the placeholder
            if (cityDropdown.find('option').length > 1) {
                clearInterval(interval);
                callback();
            }
        }, 100); // check every 100ms, stop when cities are loaded
    }


});

// Function to reset the form
function toggleVisibility(isAddMode) {
    // Handle initial state of the label based on the checkbox
    if ($('#locationActiveToggle').prop('checked')) {
        $('#activeStatusLabel').text('Active');
    } else {
        $('#activeStatusLabel').text('Inactive');
    }

    // Bind the change event to update the label dynamically
    $('#locationActiveToggle').on('change', function () {
        if ($(this).prop('checked')) {
            $('#activeStatusLabel').text('Active');
        } else {
            $('#activeStatusLabel').text('Inactive');
        }
    });

    // For Add mode, ensure the toggle is set to Inactive by default
    if (isAddMode) {
        $('#locationActiveToggle').prop('checked', false); // Set to Inactive
        $('#activeStatusLabel').text('Inactive'); // Update label text
        $('#toggleContainer').hide(); // Hide toggle in Add mode
    } else {
        $('#toggleContainer').show(); // Show toggle in Edit mode
    }
}
function setFormTitle(isTitleEditMode) {
    const title = isTitleEditMode ? "Update Location Details" : "Add Location Details";
    $('#formTitle').text(title); // Assuming the form title has an ID `formTitle`
}

function openEditLocationForm(locationId) {
    if (locationId) {
        $.get(`/PayrollMaster/GetLocationDetails?id=${locationId}`, function (response) {
            if (response.success) {
                const location = response.data;
                
                // Store the original form data
                originalLocationFormData = {
                    Location_Id: location.location_Id,
                    Country_Id: location.countryId,
                    State_Id: location.state_Id,
                    CityId: location.cityId,
                    City_Name: location.city_Name,
                    LocationName: location.locationName,
                    IsActive: location.isActive
                };
                $('#locationId').val(location.location_Id);
                $('#locationCountryDropdown').val(location.countryId).trigger('change'); // Trigger change for state population
                $('#locationStateDropdown').val(location.state_Id).trigger('change'); // Trigger change for city population
                $('#locationCityDropdown').val(location.cityId);
                $('#locationname').val(location.locationName);

                // Make LocationName field read-only and gray
                $('#locationname')
                    .prop('readonly', true)
                    .css({
                        'background-color': '#e9ecef', // light gray
                        'color': '#6c757d',            // muted text
                        'cursor': 'not-allowed'        // show not-allowed cursor
                    });

                // Set the active/inactive toggle
                // Ensure toggle is visible for edit mode
                $('#toggleContainer').show();
                if (location.isActive) {
                    $('#locationActiveToggle').prop('checked', true);
                    $('#activeStatusLabel').text('Active');
                } else {
                    $('#locationActiveToggle').prop('checked', false);
                    $('#activeStatusLabel').text('Inactive');
                }
                $('#locationActiveToggle').change(function () {
                    $('#activeStatusLabel').text($(this).prop('checked') ? 'Active' : 'Inactive');
                });
                //$('#locationActiveToggle').prop('checked', location.isActive); // Set the toggle based on `isActive`
                //$('#activeStatusLabel').text(location.isActive ? 'Active' : 'Inactive'); // Update label text
                // Reset form to Edit mode
                setFormTitle(true); // Set the title to "Update Location Details"
                $('#addLocation').offcanvas('show'); // Open the form
                populateLocationDropdownValues(response.data);
            }
        }).fail(function (error) {
            if (error.status === 401) {
                window.location.href = "/Account/LoginPage"; // Redirect on unauthorized
            } else {
                alert('An error occurred: ' + (error.statusText || 'Unknown error'));
            }
        });
    } else {
        $('#locationForm')[0].reset();
        $('#locationId').val('');
    }

}
// On page load, check for the stored active tab and activate it
$(document).ready(function () {
    // Check if there's an active tab saved in localStorage
    var activeTab = localStorage.getItem('activeTab');

    if (activeTab) {
        // If activeTab is found in localStorage, activate it
        $('#' + activeTab).addClass('active');
        $('#' + activeTab).attr('aria-selected', 'true');

        // Show the content corresponding to the active tab
        $('#' + activeTab.replace('-tab', '')).addClass('show active');

        // Remove active class from other tabs and content
        $('.nav-link').not('#' + activeTab).removeClass('active').attr('aria-selected', 'false');
        $('.tab-content > .tab-pane').not('#' + activeTab.replace('-tab', '')).removeClass('show active');
        // Set a timeout to clear localStorage after 5 seconds
        setTimeout(function () {
            localStorage.removeItem('activeTab'); // Clear the active tab from localStorage
        }, 5000); // 5000ms = 5 seconds
    }
});
$(document).ready(function () {
    var selectedButton = null;
    var isRequestInProgress = false;

    $(document).on('click', '.btn-danger-light-icon[data-bs-target="#deleteLocation"]', function () {
        selectedButton = $(this);
    });

    $(document).on('click', '#confirmLocationDelete', function () {
        if (isRequestInProgress) return; // Prevent multiple clicks
        isRequestInProgress = true;

        if (!selectedButton) {
            isRequestInProgress = false;
            return;
        }

        var locationId = selectedButton.data('location-id');

        if (!locationId) {
            showAlert("danger", "Invalid location ID.");
            isRequestInProgress = false;
            return;
        }

        var rowId = `row-${locationId}`; // Construct the row ID

        var rowData = { Location_Id: locationId };

        $.ajax({
            url: '/PayrollMaster/DeleteLocation',
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
                    LoadLocationPartial();
                } else {
                    showAlert("danger", response.message || "Failed to delete location. Please try again.");
                }
                $('#deleteLocation').modal('hide');

            },
            error: function (error) {
                if (error.status === 401) {
                    window.location.href = "/Account/LoginPage"; // Redirect if session expired
                } else {
                    showAlert("danger", "An error occurred: " + (error.statusText || "Please try again."));
                }
                $('#deleteLocation').modal('hide');
            },
            complete: function () {
                isRequestInProgress = false;
                // **Ensure modal cleanup to prevent UI glitches**
                setTimeout(() => {
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();
                }, 300);
            }
        });
    });
});

// Open Edit Form when a location is selected
function LoadLocationPartial() {
    $.ajax({
        url: '/PayrollMaster/LoadLocationPartial', // Ensure this matches your controller route
        type: 'GET',
        success: function (response) {
            let tableId = "tblLocation";
            let table = $('#' + tableId);
            if ($.fn.DataTable.isDataTable(table)) {
                table.DataTable().destroy(); // Destroy existing instance before updating
            }
            $('#tblLocation tbody').html($(response).find('tbody').html());
            makeDataTable(tableId); // Reinitialize DataTable
        },
        error: function () {
            alert('Failed to refresh Location list.');
        }
    });
}
function populateLocationDropdownValues(data) {
    fetchLocationDropdownData('/DropDown/FetchCountriesDropdown', {}, '#locationCountryDropdown', '')
        .done(() => {
            $('#locationCountryDropdown').val(data.countryId).trigger('change');

            fetchLocationStateDropdown(data.countryId, data.state_Id).done(() => {
                fetchLocationCityDropdown(data.state_Id, data.cityId);
            });
        })
        .fail(() => alert('Error loading dropdown data.'));
}

function fetchLocationDropdownData(url, data, dropdownId, placeholderText) {
    const deferred = $.Deferred();
    $.ajax({
        url: url,
        type: 'GET',
        data: data,
        async: false,
        success: function (response) {
            populateLocationDropdown(dropdownId, response, placeholderText);
            deferred.resolve();
        },
        error: function () {
            deferred.reject();
        }
    });
    return deferred.promise();
}
function populateLocationDropdown(dropdownId, items, placeholderText) {
    const dropdown = $(dropdownId);

    // Destroy select2 if already initialized
    if ($.fn.select2 && dropdown.hasClass("select2-hidden-accessible")) {
        dropdown.select2('destroy');
    }

    dropdown.empty().append(new Option(placeholderText, ''));

    items.forEach(item => {
        dropdown.append(new Option(item.text, item.value));
    });

    // Re-initialize select2 with proper settings
    if ($.fn.select2) {
        dropdown.select2({ width: '100%' });
    }

    // Trigger change event to apply new options
    dropdown.trigger('change');
}

function fetchLocationStateDropdown(countryId, stateId) {
    return fetchLocationDropdownData('/DropDown/FetchStateDropdown/' + countryId, {}, '#locationStateDropdown', '')
        .done(() => {
            if (stateId) {
                $('#locationStateDropdown').val(stateId).trigger('change');
            }
        });
}
function fetchLocationCityDropdown(stateId, cityId) {
    return fetchLocationDropdownData('/DropDown/FetchCityDropdown/' + stateId, {}, '#locationCityDropdown', '')
        .done(() => {
            let $cityDropdown = $('#locationCityDropdown');

            // Check if cityId is provided
            if (cityId) {
                // Check if the cityId exists in the dropdown
                if ($cityDropdown.find('option[value="' + cityId + '"]').length === 0) {
                    // If not, append it manually
                    $cityDropdown.append(
                        $('<option></option>')
                            .val(cityId)
                            .text('Previously selected city')
                    );
                }

                // Now set the value
                $cityDropdown.val(cityId).trigger('change');
            }
        });
}
