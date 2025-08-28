$(document).ready(function () {
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
    // Validate Add/Edit Form on Save Click
    function validateMapDepartmentForm() {
        var isValid = true;

        // Clear previous error messages
        $(".input_error_msg").html("");

        // Validate dropdowns
        const requiredDropdowns = [
            { id: "#MapCompanyDropdown", errorId: "#mapcompanyDropdown-error", message: "Please select the company." },
            { id: "#MapCountryDropdown", errorId: "#mapcountryDropdown-error", message: "Please select the country." },
            { id: "#MapStateDropdown", errorId: "#mapstateDropdown-error", message: "Please select the state." },
            { id: "#MapCityDropdown", errorId: "#mapcityDropdown-error", message: "Please select the city." },
            { id: "#MapLocationDropdown", errorId: "#maplocationDropdown-error", message: "Please select the location." },
            { id: "#MapAreaDropdown", errorId: "#mapareaDropdown-error", message: "Please select the area." },
            { id: "#MapDepartmentDropdown", errorId: "#mapdepartmentDropdown-error", message: "Please select the department." },
            //{ id: "#MapFloorDropdown", errorId: "#mapfloorDropdown-error", message: "Please select the floor." }
        ];

        requiredDropdowns.forEach((dropdown) => {
            var value = $(dropdown.id).val();
            if (!value || value === "") {
                $(dropdown.errorId).html(dropdown.message);
                isValid = false;
            }
        });

        // Validate Department Code
        //var departmentCode = $("#mapdepartmentCode").val();
        //if (!departmentCode || departmentCode.trim() === "") {
        //    $("#mapdepartmentCode-error").html("Please enter the department code.");
        //    isValid = false;
        //} else if (departmentCode.trim().length > 50) {
        //    $("#mapdepartmentCode-error").html("Department code cannot exceed 50 characters.");
        //    isValid = false;
        //}

        return isValid;
    }
    // Open Add Form on Click
    $("#addmapdepartmentButton").on("click", function () {
        resetMapDepartmentForm();
        // Set departmentLocationId to 0 explicitly
        $("#departmentLocationId").val("0");

        var title = "Add Department Mapping";
        $("#mapformTitle").text(title);
        $("#addmapdepartment").offcanvas("show"); // Open the form
        //$("#formTitle").text("Add Department Mapping");
        //$("#addmapdepartment").offcanvas("show"); // Open the form
        // 🔹 Ensure the ID field is cleared
        var companyId = $("#MapCompanyDropdown").val();

        if (companyId) {
            loadAddCompanyLocations(companyId);
        }
        loadDropdowns(); // Load dropdowns for new entry
        setTimeout(() => {
            loadAllDepartments();
            loadFloors();
        }, 500); // Small delay to ensure data is fetched
    });

    // Open Edit Form on Click
    $(document).on("click", ".edit-btn[data-bs-target='#addmapdepartment']", function () {
        resetMapDepartmentForm(); // Reset form before opening
        var mapDepartmentId = $(this).data("id");
        var title = "Update Department Mapping";
        // var title = mapDepartmentId ? "Update Department Mapping" : "Add Department Mapping";
        $("#mapformTitle").text(title);
        $("#addmapdepartment").offcanvas("show");

        if (mapDepartmentId) {
            openMapDepartmentEditButton(mapDepartmentId);
        }
    });
    // Reset Form Fields and Validation Messages
    function resetMapDepartmentForm() {
        $("#addMapDepartmentForm")[0].reset(); // Reset form fields
        $(".input_error_msg").html(""); // Clear error messages
        $(".input_error_msg").text(""); // Also clear text content

        // Reset dropdowns
        $("#MapCompanyDropdown, #MapCountryDropdown, #MapStateDropdown, #MapCityDropdown, #MapLocationDropdown, #MapAreaDropdown, #MapDepartmentDropdown, #MapFloorDropdown")
            .val("")
            .trigger("change"); // Trigger change event if using Select2

        // Reset Active/Inactive Toggle
        $('#mapdepartmentActiveToggle').prop('checked', false);
        $('#activeMapStatusLabel').text("Inactive");

        // Hide the toggle button
        $("#toggleMapContainer").hide();
    }

    // Company Dropdown Change Event
    $("#MapCompanyDropdown").on("change", function () {
        var companyId = $(this).val();
        if (companyId) {
            loadAddCompanyLocations(companyId);
        } else {
            resetGeoDropdowns(); // Reset if no company is selected
        }
    });
    // Country Dropdown Change Event
    $("#MapCountryDropdown").on("change", function () {
        var selectedCountryId = $(this).val();
        var companyId = $("#MapCompanyDropdown").val();
        loadAddCompanyLocations(companyId, selectedCountryId);
    });
    // State Dropdown Change Event
    $("#MapStateDropdown").on("change", function () {
        var selectedCountryId = $("#MapCountryDropdown").val();
        var selectedStateId = $(this).val();
        var companyId = $("#MapCompanyDropdown").val();
        loadAddCompanyLocations(companyId, selectedCountryId, selectedStateId);
    });

    // City Dropdown Change Event
    $("#MapCityDropdown").on("change", function () {
        var selectedCountryId = $("#MapCountryDropdown").val();
        var selectedStateId = $("#MapStateDropdown").val();
        var selectedCityId = $(this).val();
        var companyId = $("#MapCompanyDropdown").val();
        loadAddCompanyLocations(companyId, selectedCountryId, selectedStateId, selectedCityId);
    });
    $("#MapLocationDropdown").on("change", function () {
        var selectedCountryId = $("#MapCountryDropdown").val();
        var selectedStateId = $("#MapStateDropdown").val();
        var selectedCityId = $("#MapCityDropdown").val();
        var selectedLocationId = $(this).val();
        var companyId = $("#MapCompanyDropdown").val();
        loadAddCompanyLocations(companyId, selectedCountryId, selectedStateId, selectedCityId, selectedLocationId);
    });
    $("#MapAreaDropdown").on("change", function () {
        var selectedCountryId = $("#MapCountryDropdown").val();
        var selectedStateId = $("#MapStateDropdown").val();
        var selectedCityId = $("#MapCityDropdown").val();
        var selectedLocationId = $("#MapLocationDropdown").val();
        var selectedAreaId = $(this).val();
        var companyId = $("#MapCompanyDropdown").val();
        loadAddCompanyLocations(companyId, selectedCountryId, selectedStateId, selectedCityId, selectedLocationId, selectedAreaId);
    });

    window.openMapDepartmentEditButton = function (departmentId) {
        resetMapDepartmentForm();
        //$("#formTitle").text("Update Department Mapping");
        //$("#addmapdepartment").offcanvas("show");

        fetchMapDepartmentDetails(departmentId);
    };
    // Reset dropdowns when company is changed
    // Function to reset all dropdowns
    function resetGeoDropdowns() {
        $("#MapCountryDropdown, #MapStateDropdown, #MapCityDropdown, #MapLocationDropdown").html('<option value=""></option>');
    }

    var originalDepartmentData = null; // Store original data
    // Fetch resetLocationButton editing
    function fetchMapDepartmentDetails(mapDepartmentId) {
        $.ajax({
            url: `/PayrollMaster/GetMapDepartmentLocationsById/${mapDepartmentId}`,
            type: "GET",
            success: function (response) {
                if (response.success) {
                    originalDepartmentData = response.data; // Store the original data
                    populateForm(response.data);
                    /*if (response.data.Department_Location_Id > 0) {*/
                    // Show toggle button in edit mode
                    $('#toggleMapContainer').show();

                    // Set the toggle state based on IsActive
                    if (response.data.isActive) {
                        $('#mapdepartmentActiveToggle').prop('checked', true);
                        $('#activeMapStatusLabel').text('Active');
                    } else {
                        $('#mapdepartmentActiveToggle').prop('checked', false);
                        $('#activeMapStatusLabel').text('Inactive');
                    }
                    // Listen for toggle change and update label
                    /*}*/
                    // Listen for toggle change and update label
                    $('#mapdepartmentActiveToggle').change(function () {
                        $('#activeMapStatusLabel').text($(this).prop('checked') ? 'Active' : 'Inactive');
                    });
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("Error fetching department details.");
            }
        });
    }

    // Populate form fields and dropdowns with response data
    function populateForm(data) {
        $("#departmentLocationId").val(data.departmentLocationId);
        $("#mapdepartmentActiveToggle").prop("checked", data.isActive);
        $("#activeMapStatusLabel").text(data.isActive ? "Active" : "Inactive");

        // Load dropdowns in the correct order
        loadCompanies(data.companyId);
        setTimeout(() => {
            loadCompanyLocations(data.companyId, data.countryId, data.stateId, data.cityId, data.locationId, data.areaId);

            setTimeout(() => {
                loadAllDepartments(data.departmentId); // Ensure departments are loaded
                loadFloors(data.floorId); // Ensure floors are loaded
            }, 500);
        }, 500);
    }
    function loadDropdowns() {
        loadCompanies();
        var companyId = $("#MapCompanyDropdown").val();
        if (companyId) {
            loadAddCompanyLocations(companyId);
        }

    }

    // Load company dropdown
    function loadCompanies(selectedCompanyId = "") {
        $.ajax({
            url: "/DropDown/FetchCompaniesDropdown",
            type: "GET",
            async: false,
            success: function (data) {
                populateDropdown("#MapCompanyDropdown", data, selectedCompanyId);
            },
        });
    }

    // Load company locations dynamically
    function loadCompanyLocations(companyId, selectedCountryId = "", selectedStateId = "", selectedCityId = "", selectedLocationId = "", selectedAreaId = "") {
        if (!companyId) return;

        $.ajax({
            url: `/DropDown/GetCompanyLocationData?companyId=${companyId}&userId=null`,
            type: "GET",
            async: false, // Ensure sequential loading
            success: function (response) {
                if (response.isSuccess && response.result) {
                    // Populate dropdowns in order: Country -> State -> City -> Location
                    populateGeoDropdown("#MapCountryDropdown", response.result.countries, selectedCountryId);

                    populateGeoDropdown("#MapStateDropdown", response.result.states, selectedStateId);

                    // Ensure state ID is a number
                    selectedStateId = Number(selectedStateId);
                    selectedCityId = Number(selectedCityId);
                    selectedLocationId = Number(selectedLocationId);

                    // Filter and populate city dropdown
                    var filteredCities = response.result.cities.filter(city => Number(city.state_Id) === selectedStateId);
                    populateGeoDropdown("#MapCityDropdown", filteredCities, selectedCityId);

                    // Filter and populate location dropdown
                    var filteredLocations = response.result.areaLocations.filter(loc => Number(loc.city_ID) === selectedCityId);
                    populateGeoDropdown("#MapLocationDropdown", filteredLocations, selectedLocationId);

                    // Populate area dropdown after a slight delay
                    setTimeout(() => {

                        var filteredAreas = response.result.areas.filter(a => Number(a.location_Id) === selectedLocationId);

                        var areaDropdown = $("#MapAreaDropdown");
                        areaDropdown.empty(); // Clear previous options

                        if (filteredAreas.length > 0) {
                            areaDropdown.append(`<option value="">Select Area</option>`); // Add default option
                            filteredAreas.forEach(area => {
                                var isSelected = area.area_Id == selectedAreaId ? "selected" : "";
                                areaDropdown.append(`<option value="${area.area_Id}" ${isSelected}>${area.areaName}</option>`);
                            });
                        }
                    }, 500);

                } else {
                    resetGeoDropdowns();
                }
            },
            error: function (err) {
                resetGeoDropdowns();
            }
        });
    }
    // Function to load company locations based on selected values
    function loadAddCompanyLocations(companyId, selectedCountryId = "", selectedStateId = "", selectedCityId = "", selectedLocationId = "", selectedAreaId = "") {
        if (!companyId) {
            resetGeoDropdowns();
            return;
        }


        $.ajax({
            url: `/DropDown/GetCompanyLocationData?companyId=${companyId}&userId=null`,
            type: "GET",
            async: false,
            success: function (response) {
                if (response.isSuccess && response.result) {
                    var data = response.result;
                    // Populate Country Dropdown
                    populateAddDropdown("#MapCountryDropdown", data.countries, "country_Id", "countryName", selectedCountryId);

                    // Populate State Dropdown based on selected country
                    if (selectedCountryId) {
                        var filteredStates = data.states.filter(s => s.countryId == selectedCountryId);
                        populateAddDropdown("#MapStateDropdown", filteredStates, "state_Id", "stateName", selectedStateId);
                    } else {
                        $("#MapStateDropdown").html('<option value="">Select a state</option>');
                    }

                    // Populate City Dropdown based on selected state
                    if (selectedStateId) {
                        var filteredCities = data.cities.filter(c => c.state_Id == selectedStateId);
                        populateAddDropdown("#MapCityDropdown", filteredCities, "city_ID", "city_Name", selectedCityId);
                    } else {
                        $("#MapCityDropdown").html('<option value="">Select a city</option>');
                    }

                    // Populate Location Dropdown based on selected city
                    if (selectedCityId) {
                        var filteredLocations = data.areaLocations.filter(l => l.city_ID == selectedCityId);
                        populateAddDropdown("#MapLocationDropdown", filteredLocations, "location_Id", "locationName", selectedLocationId);
                    } else {
                        $("#MapLocationDropdown").html('<option value="">Select a location</option>');
                    }

                    // Populate Area Dropdown directly inside this function
                    if (selectedLocationId) {
                        var filteredAreas = data.areas.filter(a => a.location_Id == selectedLocationId);
                        populateAddDropdown("#MapAreaDropdown", filteredAreas, "area_Id", "areaName", selectedAreaId);
                    } else {
                        $("#MapAreaDropdown").html('<option value="">Select an area</option>');
                    }
                    // Populate Location Dropdown based on selected city
                    //if (selectedLocationId) {
                    //    loadAreas(selectedLocationId, selectedAreaId);
                    //}
                }
            },
            error: function () {
                alert("Error fetching location data.");
            }
        });
    }

    // Function to populate dropdowns dynamically
    function populateAddDropdown(selector, data, valueField, textField, selectedValue = "") {
        var dropdown = $(selector);
        dropdown.empty().append('<option value=""></option>');

        if (data.length > 0) {
            $.each(data, function (index, item) {
                dropdown.append($('<option></option>')
                    .attr("value", item[valueField])
                    .text(item[textField])
                    .prop("selected", item[valueField] == selectedValue)); // Set selected value
            });
        }
    }

    // Load area dropdown based on location2
    function loadAreas(locationId, selectedAreaId = "") {
        if (!locationId) return;
        $.ajax({
            url: `/DropDown/FetchAreaLocationDropdown/${locationId}`,
            type: "GET",
            async: false,
            success: function (data) {
                populateDropdown("#MapAreaDropdown", data, selectedAreaId);
            }
        });
    }
    function loadAllDepartments(selectedDepartmentId = "") {
        $.ajax({
            url: `/DropDown/FetchAllDepartmentDropdown`,
            type: "GET",
            async: false,
            success: function (data) {
                populateDropdown("#MapDepartmentDropdown", data, selectedDepartmentId);
            }
        });
    }

    // Load floors dropdown
    function loadFloors(selectedFloorId = "") {
        if (!selectedFloorId) {
            $.ajax({
                url: `/DropDown/FetchFloorMasterDropdown`,
                type: "GET",
                async: false,
                success: function (data) {
                    populateDropdown("#MapFloorDropdown", data, selectedFloorId);
                }
            });
        }
        else {
            $.ajax({
                url: `/DropDown/FetchFloorMasterDropdown/${selectedFloorId}`,
                type: "GET",
                async: false,
                success: function (data) {
                    populateDropdown("#MapFloorDropdown", data, selectedFloorId);
                }
            });
        }
    }

    // Updated helper for generic dropdowns (like Areas, Departments, Floors)
    function populateDropdown(dropdownId, data, selectedValue = "") {
        var $dropdown = $(dropdownId);
        $dropdown.empty();
        // Use a generic default if you need one:
        $dropdown.append('<option value="" disabled selected></option>');

        if (!data || !Array.isArray(data)) {
            return;
        }

        // We try to guess the keys here too. You can adjust this mapping if needed.
        $.each(data, function (index, item) {
            // Try several keys:
            var value = item.value || item.area_Id || item.departmentId || item.floorId || "";
            var text = item.text || item.areaName || item.departmentName || item.floorName || "";

            if (value && text) {
                var isSelected = (value == selectedValue) ? "selected" : "";
                $dropdown.append(`<option value="${value}" ${isSelected}>${text}</option>`);
            }
        });
    }
    // Updated helper to populate geo dropdowns using a mapping config
    function populateGeoDropdown(dropdownSelector, data, selectedId) {
        const mapping = {
            "#MapCountryDropdown": { id: "country_Id", name: "countryName" },
            "#MapStateDropdown": { id: "state_Id", name: "stateName" },
            "#MapCityDropdown": { id: "city_ID", name: "city_Name" },
            "#MapLocationDropdown": { id: "location_Id", name: "locationName" }
        };

        const config = mapping[dropdownSelector] || { id: "id", name: "name" };
        var $dropdown = $(dropdownSelector);
        $dropdown.empty();
        $dropdown.append('<option value=""></option>');

        $.each(data, function (index, item) {
            if (item[config.id] !== undefined && item[config.name] !== undefined) {
                var isSelected = (item[config.id] == selectedId) ? "selected" : "";
                $dropdown.append(`<option value="${item[config.id]}" ${isSelected}>${item[config.name]}</option>`);
            }
        });

        $dropdown.trigger("change");
    }

    // Save or Update Department
    $(document).on("click", "#saveDetailsButton", function (e) {
        if (!validateMapDepartmentForm()) {
            e.preventDefault(); // Stop form submission if validation fails
            return false; // Explicitly exit function
        }
        saveOrUpdateMapDepartment();
    });

    function saveOrUpdateMapDepartment() {
        var departmentLocationId = $('#departmentLocationId').val() || 0;
        /* var departmentCode = $('#mapdepartmentCode').val();*/
        var companyId = $('#MapCompanyDropdown').val();
        var countryId = $('#MapCountryDropdown').val();
        var stateId = $('#MapStateDropdown').val();
        var cityId = $('#MapCityDropdown').val();
        var locationId = $('#MapLocationDropdown').val();
        var areaId = $('#MapAreaDropdown').val();
        var departmentId = $('#MapDepartmentDropdown').val();
        var floorId = $('#MapFloorDropdown').val() || 0;
        var isActive = $('#mapdepartmentActiveToggle').prop('checked');
        var mapDepartmentLocationDTO = {
            Department_Location_Id: departmentLocationId,
            //Department_Code: departmentCode,
            Company_Id: companyId,
            Country_ID: countryId,
            State_Id: stateId,
            City_ID: cityId,
            Location_Id: locationId,
            Area_Id: areaId,
            Department_Id: departmentId,
            Floor_Id: floorId,
            IsActive: isActive
        };

        // Send data to the server for Add or Edit
        $.ajax({
            url: '/PayrollMaster/SaveOrUpdateMapDepartment',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(mapDepartmentLocationDTO),
            success: function (response) {
                if (response.success) {
                    showAlert("success", response.message);
                    $('#addmapdepartment').offcanvas('hide');
                    fetchMapDepartmentLocationList();
                } else {
                    showAlert("danger", response.message);
                    $('#addmapdepartment').offcanvas('hide');
                    fetchMapDepartmentLocationList();
                }
            },
            error: function (error) {
                if (error.status === 401) {
                    window.location.href = "/Account/LoginPage"; // Redirect if session expired
                } else {
                    showAlert("danger", "An error occurred while saving the map department.");
                }
            }
        });
    }
    function fetchMapDepartmentLocationList() {
        $.ajax({
            url: '/PayrollMaster/FetchMapDepartmentLocationList',
            type: 'GET',
            success: function (response) {
                var tableId = "mapdepartment-master-list";
                var table = $('#' + tableId);
                if ($.fn.DataTable.isDataTable(table)) {
                    table.DataTable().destroy(); // Destroy existing instance before updating
                }
                $('#mapdepartment-master-list tbody').html($(response).find('tbody').html());
                makeDataTable(tableId); // Reinitialize DataTable
            },
            error: function () {
                alert('Failed to refresh department list.');
            }
        });
    }

    $(document).on('click', '#resetButton', function () {
        const isEditMode = $('#departmentLocationId').val() !== ''; // Check if it's Edit mode

        if (isEditMode) {
            // Restore original values from the API response
            if (originalDepartmentData) {
                $('#departmentLocationId').val(originalDepartmentData.departmentLocationId);
                //$('#mapdepartmentCode').val(originalDepartmentData.departmentCode);
                $('#MapCompanyDropdown').val(originalDepartmentData.companyId).trigger('change');
                $('#MapCountryDropdown').val(originalDepartmentData.countryId).trigger('change');
                $('#MapStateDropdown').val(originalDepartmentData.stateId);
                $('#MapCityDropdown').val(originalDepartmentData.cityId);
                $('#MapLocationDropdown').val(originalDepartmentData.locationId);
                $('#MapAreaDropdown').val(originalDepartmentData.areaId);
                $('#MapDepartmentDropdown').val(originalDepartmentData.departmentId);
                $('#MapFloorDropdown').val(originalDepartmentData.floorId);
                // Set Active Status
                $('#mapdepartmentActiveToggle').prop('checked', originalDepartmentData.isActive);
                $('#activeMapStatusLabel').text(originalDepartmentData.isActive ? 'Active' : 'Inactive');
                $('#toggleMapContainer').show(); // Show toggle button in edit mode
            }
        } else {
            // Reset all fields for Add Form
            $('#addMapDepartmentForm')[0].reset();
            $('#departmentLocationId').val(''); // Ensure ID is cleared
            //$('#mapdepartmentCode').val('');
            $('#MapCompanyDropdown').val('').trigger('change');
            $('#MapCountryDropdown').val('').trigger('change');
            $('#MapStateDropdown').val('').trigger('change');
            $('#MapCityDropdown').val('').trigger('change');
            $('#MapLocationDropdown').val('').trigger('change');
            $('#MapAreaDropdown').val('').trigger('change');
            $('#MapDepartmentDropdown').val('').trigger('change');
            $('#MapFloorDropdown').val('').trigger('change');
            $('#mapdepartmentActiveToggle').prop('checked', false);
            $('#activeMapStatusLabel').text('Inactive');
            $('#toggleMapContainer').hide(); // Hide toggle button in Add mode
            loadDropdowns(); // Reload dropdowns for new entry
        }
    });
    var selectedButton = null;
    var isRequestInProgress = false;

    // Capture the clicked button and store the associated data
    $(document).on('click', '.btn-danger-light-icon[data-bs-target="#deleteMapDepartment"]', function () {
        selectedButton = $(this);
    });

    // $('#confirmMapDepartmentDelete').on('click', function () {
    $(document).on('click', '#confirmMapDepartmentDelete', function () {
        if (isRequestInProgress) return; // Prevent multiple clicks
        isRequestInProgress = true;

        if (!selectedButton) {
            isRequestInProgress = false;
            return;
        }

        var departmentLocationId = selectedButton.data('department-location-id'); // Ensure correct data attribute

        if (!departmentLocationId) {
            showAlert("danger", "Invalid department mapping ID.");
            isRequestInProgress = false;
            return;
        }

        var rowId = `row-${departmentLocationId}`; // Construct the row ID

        var rowData = {
            Department_Location_Id: departmentLocationId
        };

        $.ajax({
            url: '/PayrollMaster/DeleteMapDepartment', // Ensure this URL matches your backend route
            type: 'POST', // Use POST since we're sending data
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(rowData),
            success: function (response) {
                if (response && response.success) {
                    // Hide the row
                    $(`#${rowId}`).fadeOut(500, function () {
                        $(this).remove(); // Remove the row from DOM after fadeOut
                    });
                    showAlert("success", response.message);
                    fetchMapDepartmentLocationList();
                } else {
                    showAlert("danger", response.message || "Failed to delete department mapping. Please try again.");
                }
                $('#deleteMapDepartment').modal('hide'); // Close modal after response
            },
            error: function () {
                showAlert("danger", "An error occurred. Please try again.");
                $('#deleteMapDepartment').modal('hide'); // Ensure modal hides even on error
            },
            complete: function () {
                isRequestInProgress = false;
                // **Forcefully remove any lingering modal backdrop**
                setTimeout(() => {
                    $('body').removeClass('modal-open'); // Remove modal-open class
                    $('.modal-backdrop').remove(); // Remove leftover backdrop
                }, 300);
            }
        });
    });
});


