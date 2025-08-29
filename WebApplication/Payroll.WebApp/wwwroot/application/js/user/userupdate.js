$(document).ready(function () {

    var userData = sessionStorage.getItem("userData");
    $(document).on('shown.bs.tab', 'button[data-bs-toggle="pill"]', function (e) {
        var target = $(e.target).attr("data-bs-target");

        if (target === "#v-pills-profile-edit") {
            $("#resetButtonWrapper").show();  // Show Reset on Company Details tab
        } else {
            $("#resetButtonWrapper").hide();  // Hide elsewhere
        }
    });

    // Trigger on page load
    $(document).ready(function () {
        if ($("#v-pills-profile-edit-tab").hasClass("active")) {
            $("#resetButtonWrapper").show();
        } else {
            $("#resetButtonWrapper").hide();
        }
    });
    $(document).on("click", "#reseteditButton", function () {
        // Check if the Company Details tab is currently active
        if ($("#v-pills-profile-edit").hasClass("active")) {
            // Reset the "City" dropdown (branch)
            $("#branch").val(null).trigger("change.select2");

            // Reset the "Locations" multi-select dropdown (department)
            $("#department").val(null).trigger("change.select2");

            // Optional: clear validation messages if any
            $("#branch-error").text("");
            $("#department-error").text("");
        }
    });
    $(document).ready(function () {
        $('#branch').select2({
            placeholder: $('#branch').data('placeholder'),
            width: '100%',
            dropdownAutoWidth: true
        });

        $('#department').select2({
            placeholder: $('#department').data('placeholder'),
            width: '100%',
            dropdownAutoWidth: true
        });
    });


    if (userData) {
        var user = JSON.parse(userData);
        var userId = user.user_id;
        var companyId = user.company_Ids ? user.company_Ids[0] : null; // Assuming the user has at least one company.
        var locationId = user.locationDetails.map(location => location.location_ID);
        // Function to fill User Information
        function fillUserInformation() {
            $("#userId").val(user.user_id);
            $("#usertype").val(user.userType_Id).trigger("change");
            $("#countries").val(user.country_Id).trigger("change");
            $("#salutationsDropdown").val(user.salutation).trigger("change");
            $("#username").val(user.username);
            $("#fname").val(user.firstName);
            $("#mname").val(user.middleName);
            $("#lname").val(user.lastName);
            $("#phone").val(user.contactNo);
            $("#email").val(user.email);
        }
        // Function to fill Company & Location Details
        function fillCompanyDetails() {
            let companyId = user.company_Ids ? user.company_Ids[0] : null; // Get first company ID if available
            let userId = user.user_id || null; // Corrected User ID field

            // Fill Companies (Multi-Select)
            if (user.company_Ids && user.company_Ids.length > 0) {
                $("#Companies").val(user.company_Ids).trigger("change"); // Multiple selection
            }

            // Fill Location Details (Single Selection)
            if (user.locationDetails && user.locationDetails.length > 0) {
                let firstLocation = user.locationDetails[0]; // Use first location entry
                $("#countriesname").val(firstLocation.country_ID).trigger("change");
                $("#state").val(firstLocation.state_Id).trigger("change");
                $("#branch").val(firstLocation.city_ID).trigger("change");
            }

            $.ajax({
                url: `/DropDown/GetCompanyLocationData`, // Adjust ControllerName
                type: "GET",
                data: { companyId: companyId, userId: userId },
                dataType: "json",
                async: false,
                success: function (response) {
                    if (response.isSuccess && response.result) {
                        let data = response.result;

                        // ✅ Populate City Dropdown - FIXED: Filter by selected state
                        if (data.cities && data.cities.length > 0) {
                            let cityDropdown = $("#branch");
                            cityDropdown.empty();
                            cityDropdown.append(`<option value="">Select City</option>`);
                            
                            // ✅ CRITICAL FIX: Get the selected state ID to filter cities
                            let selectedStateId = user.locationDetails ? user.locationDetails[0]?.state_Id : null;
                            console.log('🏛️ Selected State ID for filtering:', selectedStateId);
                            
                            // ✅ Filter cities by selected state
                            let filteredCities = selectedStateId 
                                ? data.cities.filter(city => city.state_Id == selectedStateId || city.stateId == selectedStateId)
                                : data.cities;
                            
                            console.log('🏙️ Filtered cities count:', filteredCities.length);
                            console.log('🏙️ All cities count:', data.cities.length);
                            
                            filteredCities.forEach(function (city) {
                                cityDropdown.append(`<option value="${city.city_ID}">${city.city_Name}</option>`);
                            });

                            // Set selected city if it exists
                            let selectedCityId = user.locationDetails ? user.locationDetails[0]?.city_ID : null;
                            if (selectedCityId) {
                                console.log('🎯 Setting selected city:', selectedCityId);
                                cityDropdown.val(selectedCityId).trigger("change");
                            }
                        }

                        // ✅ Populate Location Dropdown with Locations
                        function populateLocations(cityId = null) {
                            let departmentDropdown = $("#department");
                            departmentDropdown.empty();
                            departmentDropdown.append(`<option value="">Select Location</option>`);

                            let filteredLocations = cityId
                                ? data.locations.filter(loc => loc.cityId == cityId)
                                : data.locations;

                            filteredLocations.forEach(function (location) {
                                if (location.locationName) {
                                    departmentDropdown.append(`<option value="${location.correspondance_ID}">${location.locationName}</option>`);
                                }
                            });

                            // ✅ Set selected values if they exist
                            let locationIds = user.locationDetails ? user.locationDetails.map(location => location.location_ID) : [];
                            setTimeout(() => {
                                departmentDropdown.val(locationIds).trigger("change");
                            }, 100);
                        }

                        // Populate locations initially
                        populateLocations();

                        // ✅ State Change Event - Filter Cities by State
                        $("#state").change(function () {
                            let selectedStateId = $(this).val();
                            console.log('🏛️ State changed to:', selectedStateId);
                            
                            let cityDropdown = $("#branch");
                            cityDropdown.empty();
                            cityDropdown.append(`<option value="">Select City</option>`);
                            
                            // Filter cities by selected state
                            let filteredCities = selectedStateId 
                                ? data.cities.filter(city => city.state_Id == selectedStateId || city.stateId == selectedStateId)
                                : data.cities;
                            
                            console.log('🏙️ Cities filtered for state:', filteredCities.length);
                            
                            filteredCities.forEach(function (city) {
                                cityDropdown.append(`<option value="${city.city_ID}">${city.city_Name}</option>`);
                            });
                            
                            // Clear city selection and locations when state changes
                            cityDropdown.val('').trigger("change");
                        });

                        // ✅ City Change Event - Filter Locations
                        $("#branch").change(function () {
                            let selectedCity = $(this).val();
                            populateLocations(selectedCity);
                        });
                    } else {
                        console.warn(response.message || "No location data received.");
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error fetching company location data:", error);
                }
            });
        }

        function fillUserLocationsAndRoles() {
            $.ajax({
                url: `/User/FetchUserLocationWiseRole?userId=${userId}&companyId=${companyId}&correspondanceId=null`,
                type: "GET",
                contentType: "application/json",
                dataType: "json",
                success: function (response) {
                    sessionStorage.setItem("latestRolesData", JSON.stringify(response.data));
                    if (response.success && response.data) {
                        let locations = response.data.locationWiseRoles.map(role => ({
                            location_ID: role.correspondance_Id,
                            locationName: role.locationName
                        }));

                        locations = [...new Map(locations.map(item => [item["location_ID"], item])).values()];

                        //populateLocationDropdown(locations);

                        populateUserRolesGrid(response.data.locationWiseRoles);
                    } else {
                        //console.error("Invalid response structure:", response);
                        //showAlert("danger", response.message || "Failed to fetch locations.");
                    }
                },
                error: function () {
                    showAlert("danger", "An error occurred while fetching locations.");
                }
            });
        }
        function populateLocationDropdown(locations) {
            let locationDropdown = $("#locationrole");
            locationDropdown.empty();
            locationDropdown.append(`<option value="">Select location</option>`);
            locations.forEach(location => {
                locationDropdown.append(`<option value="${location.location_ID}">${location.locationName}</option>`);
            });
        }
        function fetchRolesByLocation(locationId) {
            $.ajax({
                url: `/User/FetchUserLocationWiseRole?userId=${userId}&companyId=${companyId}&correspondanceId=${locationId}`,
                type: "GET",
                contentType: "application/json",
                dataType: "json",
                success: function (response) {
                    if (response.success && response.data) {
                        sessionStorage.setItem("latestRolesData", JSON.stringify(response.data)); // Store full response
                        // Clear the existing grid before populating new roles
                        $("#userRolesGrid tbody").empty();
                        if (response.data.hasOwnProperty("locationWiseRoles")) {
                            populateUserRolesGrid(response.data.locationWiseRoles);
                        }
                        if (response.data.hasOwnProperty("roleMenuHeaders")) {
                            populateRoleDropdown(response.data.roleMenuHeaders);
                        }
                    } else {
                        showAlert("danger", response.message || "No roles found.");
                    }
                },
                error: function () {
                    showAlert("danger", "An error occurred while fetching roles.");
                }
            });
        }
        function populateRoleDropdown(roleMenuHeaders) {
            let roleDropdown = $("#role");
            let roleError = $("#role-error");
            roleDropdown.empty();

            if (!roleMenuHeaders || roleMenuHeaders.length === 0) {
                roleDropdown.append(`<option value="">No roles assigned for this location</option>`);
                roleError.text("No roles assigned for the selected location.").show();
                //showAlert("warning", "No roles assigned for the selected location.");
                return;
            }

            // If roles are available, clear any previous error messages
            roleError.text("").hide();

            if (roleMenuHeaders.length === 1) {
                // Only one role – auto-select
                const role = roleMenuHeaders[0];
                roleDropdown.append(`<option value="${role.role_Menu_Hdr_Id}" selected>${role.roleName}</option>`);
            } else {
                // Multiple roles – show select option and list all
                roleDropdown.append(`<option value="">Select Role</option>`);
                roleMenuHeaders.forEach(role => {
                    roleDropdown.append(`<option value="${role.role_Menu_Hdr_Id}">${role.roleName}</option>`);
                });
            }

            // Trigger Select2 update if using Select2
            roleDropdown.trigger("change.select2");
        }
        function populateUserRolesGrid(roles) {
            let userRolesGrid = $("#userRolesGrid tbody");
            userRolesGrid.empty(); // Clear old rows (including temporary mapped rows)
            if (roles && roles.length > 0) {
                let rows = roles.map(role => {
                    let roleId = role.role_User_Id || "New";
                    let correspondanceId = role.correspondance_Id;
                    let locationName = role.locationName;
                    let effective_From = role.effective_From;
                    let roleName = role.roleName || role.role_Id;
                    let statusText = role.isActive ? "Active" : "Inactive";
                    /* console.log(role);*/
                    if (roleId === "New") {
                        return ''; // API se aaye new fake rows skip karo
                    }
                    // Check if role already exists
                    let isDuplicate = false;
                    $("#userRolesGrid tbody tr").each(function () {
                        let existingLocation = $(this).find("td:eq(1)").text().trim();
                        let existingRole = $(this).find("td:eq(2)").text().trim();
                        if (existingLocation === locationName && existingRole === roleName) {
                            isDuplicate = true;
                            return false; // Break loop
                        }
                    });

                    if (!isDuplicate) {
                        return `<tr>
                    <td hidden>${roleId}</td>
                    <td>${locationName}</td>
                    <td>${roleName}</td>
                    <td>${effective_From}</td>
                    <td class="sticky_cell">
                        <div class="form-check form-switch">
                            <input class="form-check-input role-toggle" type="checkbox" role="switch"
                                id="status-${roleId}" data-role-id="${roleId}" ${role.isActive ? "checked" : ""}>
                            <label class="form-check-label" for="status-${roleId}" id="label-${roleId}">
                                ${statusText}
                            </label>
                        </div>
                    </td>
                    <td hidden>${correspondanceId || ""}</td> <!-- Hidden Location ID column -->

                </tr>`;
                    } else {
                        return ''; // Duplicate role ko add mat karein
                    }
                }).join('');

                userRolesGrid.append(rows); // Naye rows ko append karein
            }

            $(".role-toggle").off("change").on("change", function () {
                let roleId = $(this).data("role-id");
                let isActive = $(this).prop("checked");

                $("#label-" + roleId).text(isActive ? "Active" : "Inactive");
                updateUserRoleStatus(roleId, isActive);
            });
        }

        // Function to update role status via AJAX
        function updateUserRoleStatus(roleId, isActive) {
            var userData = sessionStorage.getItem("userData");
            var user = userData ? JSON.parse(userData) : null;

            if (!user || !user.user_id) {
                showAlert("danger", "User not found!");
                return;
            }

            let updateData = {
                User_Id: user.user_id,
                Role_User_Id: roleId,
                ActualActivestatus: isActive
            };

            $.ajax({
                url: "/User/UpdateUserRoleStatusMaster",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(updateData),
                success: function (response) {
                    if (response.success) {
                        showAlert("success", response.message);

                        // Update UI immediately without page refresh
                        let toggleSwitch = $(`#status-${roleId}`);
                        toggleSwitch.prop("checked", isActive);
                        $(`#label-${roleId}`).text(isActive ? "Active" : "Inactive");

                        // Update sessionStorage to reflect change
                        let userRoles = user.userRoles.map(role => {
                            if (role.role_User_Id === roleId) {
                                role.isActive = isActive;
                            }
                            return role;
                        });

                        user.userRoles = userRoles;
                        sessionStorage.setItem("userData", JSON.stringify(user));

                    } else {
                        showAlert("danger", response.message);
                    }
                },
                error: function () {
                    showAlert("danger", "An error occurred while updating the role status.: " + xhr.responseText);
                }
            });
        }
        function fillUserLocations() {
            let userData = sessionStorage.getItem("userData");
            let user = userData ? JSON.parse(userData) : null;

            if (!user || !user.locationDetails || user.locationDetails.length === 0) {
                $("#userLocationsGrid tbody").html(`
            <tr>
                <td colspan="3" style="text-align: center; color: gray;">No locations available</td>
            </tr>
        `);
                return;
            }
            let userLocationsGrid = $("#userLocationsGrid tbody");
            userLocationsGrid.empty(); // Clear previous data

            if (user.locationDetails && user.locationDetails.length > 0) {
                let uniqueLocations = {};
                let activeLocations = [];
                user.locationDetails.forEach(location => {
                    uniqueLocations[location.correspondance_ID] = location; // Use correspondance_ID as the key
                });
                let rows = Object.values(uniqueLocations).map(location => {
                    let locationId = location.correspondance_ID;
                    let locationName = location.locationName || `Location ${locationId}`;
                    let isActive = location.isActive ? "checked" : "";
                    if (location.isActive) {
                        activeLocations.push({
                            location_ID: locationId,
                            locationName: locationName
                        });
                    }
                    return `<tr>
                                <td hidden>${locationId}</td>
                                <td>${locationName}</td>
                                <td class="sticky_cell">
                                    <div class="form-check form-switch">
                                        <input class="form-check-input location-toggle" type="checkbox" role="switch"
                                               id="status-${locationId}" data-location-id="${locationId}" ${isActive}>
                                        <label class="form-check-label" for="status-${locationId}">
                                            ${location.isActive ? "Active" : "Inactive"}
                                        </label>
                                    </div>
                                </td>
                            </tr>`;
                }).join('');

                userLocationsGrid.append(rows);
                populateLocationDropdown(activeLocations);
            } else {
                userLocationsGrid.append(`<tr>
                            <td colspan="3" style="text-align: center; color: gray;">No locations available</td>
                        </tr>`);
            }

            // Bind change event to toggle switch
            $(".location-toggle").off("change").on("change", function () {
                let locationId = $(this).data("location-id");
                let isActive = $(this).prop("checked");
                updateUserLocationStatus(locationId, isActive);
            });
        }
        function updateUserLocationStatus(locationId, isActive) {
            var userData = sessionStorage.getItem("userData");
            var user = userData ? JSON.parse(userData) : null;
            if (!user || !user.user_id) {
                showAlert("danger", "User not found!");
                return;
            }
            let updateData = {
                User_Id: user.user_id,
                Correspondance_ID: locationId,
                ActualActivestatus: isActive
            };
            $.ajax({
                url: "/User/UpdateUserLocationStatusMaster",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(updateData),
                success: function (response) {
                    if (response.success) {
                        showAlert("success", response.message);
                        // Update UI immediately
                        let toggleSwitch = $(`#status-${locationId}`);
                        toggleSwitch.prop("checked", isActive);
                        toggleSwitch.next("label").text(isActive ? "Active" : "Inactive");
                        // ✅ Update `sessionStorage` immediately
                        let updatedUserData = JSON.parse(sessionStorage.getItem("userData"));
                        updatedUserData.locationDetails.forEach(location => {
                            if (location.correspondance_ID === locationId) {
                                location.isActive = isActive;
                            }
                        });
                        sessionStorage.setItem("userData", JSON.stringify(updatedUserData));
                        // 🔄 Refresh grid to reflect updated data
                        fillUserLocations();
                    } else {
                        showAlert("danger", response.message);
                    }
                },
                error: function () {
                    showAlert("danger", "An error occurred while updating location status.: " + xhr.responseText);

                }
            });
        }
        $(document).on("click", "#saveLocations", function () {
            saveUserLocations();
        });
        function saveUserLocations() {

            var selectedLocations = $("#department").val() || [];
            $("#department-error").text(""); // Clear previous error

            // ✅ Ensure selectedLocations is an array
            if (!Array.isArray(selectedLocations)) {
                selectedLocations = [];
            }

            // ✅ Alert if no location selected (Modal Popup)
            if (selectedLocations.length === 0) {
                $('#noLocationModal').modal('show');
                return;
            }

            let userData = sessionStorage.getItem("userData");
            let user = userData ? JSON.parse(userData) : null;

            if (!user || !user.user_id) {
                showAlert("danger", "User not found!");
                return;
            }

            var mapUserLocationDTO = {
                User_ID: user.user_id,
                CreatedBy: user.user_id, // Assuming a placeholder CreatedBy
                UserMapLocations: selectedLocations.map(locationId => ({
                    User_ID: user.user_id,
                    Company_Id: user.company_Ids.length > 0 ? user.company_Ids[0] : null,// Assign first company
                    Correspondance_ID: parseInt(locationId),
                    IsActive: true,
                    IsUserMapToLowLevel: false,
                    IsDeleted: false
                }))
            };

            $.ajax({
                url: "/User/AddMapUserLocationRecord",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(mapUserLocationDTO),
                success: function (response) {
                    if (response.success) {
                        showAlert("success", response.message);
                        // 🔄 Update sessionStorage with new locations
                        let updatedUserData = JSON.parse(sessionStorage.getItem("userData"));
                        if (!updatedUserData.locationDetails) {
                            updatedUserData.locationDetails = [];
                        }

                        selectedLocations.forEach(locationId => {
                            let locationExists = updatedUserData.locationDetails.some(loc => loc.correspondance_ID === parseInt(locationId));
                            if (!locationExists) {
                                updatedUserData.locationDetails.push({
                                    correspondance_ID: parseInt(locationId),
                                    locationName: $("#department option[value='" + locationId + "']").text(),
                                    isActive: true
                                });
                            }
                        });

                        sessionStorage.setItem("userData", JSON.stringify(updatedUserData));

                        // Fetch updated roles from the server before refreshing the grid
                        fetchUpdatedUserRoles(user.user_id);
                        fillUserLocationsAndRoles(); // Fetch updated roles
                        fillUserLocations(); // Refresh the locations grid
                        // ✅ **Ensure the modal/pop-up appears after saving**
                        setTimeout(function () {
                            $('#saveSuccessModal').modal('show'); // Show a success modal
                        }, 500);
                        setTimeout(function () {
                            let mappingTab = $("#v-pills-mapping-tab");
                            if (mappingTab.length) {
                                mappingTab.trigger("click");
                            }
                        }, 1000);
                    } else {
                        showAlert("danger", response.message);
                    }
                },
                error: function (xhr) {
                    showAlert("danger", "An error occurred while saving locations:: " + xhr.responseText);
                }
            });
        }
        // Modal for No Location Selected
        $("#yesMoveNextTab").on("click", function () {
            let mappingTab = $("#v-pills-mapping-tab");
            if (mappingTab.length) {
                mappingTab.trigger("click");
                fillUserLocationsAndRoles(); // ✅ Ensure grid refreshes
            } else {
            }
        });
        function fetchUpdatedUserRoles(userId) {
            $.ajax({
                url: `/User/GetUserRecordById?userId=${userId}`,
                type: "GET",
                contentType: "application/json",
                dataType: "json",
                data: { userId: userId },
                success: function (response) {
                    if (response.success) {
                        let userData = sessionStorage.getItem("userData");
                        let user = userData ? JSON.parse(userData) : null;
                        /*    console.log(user);*/
                        if (user) {
                            user.userRoles = response.roles; // Update roles
                            sessionStorage.setItem("userData", JSON.stringify(user));
                        }
                        fillUserLocations();
                        //fillUserRoles(); // Refresh the grid with new data
                    } else {
                        //showAlert("danger", "Failed to fetch updated roles.");
                    }
                },
                error: function () {
                    showAlert("danger", "An error occurred while fetching updated roles.");
                }
            });
        }
        $(document).ready(function () {
            // Event handler for "Next Tab" button
            $("#saveRoles").on("click", function () {
                storeRolesForNextTab();
            });
        });
        function storeRolesForNextTab() {
            let allMappings = [];
            //console.log("Grid row count: ", $("#userRolesGrid tbody tr").length);
            $("#userRolesGrid tbody tr").each(function () {
                let isActive = $(this).find("td:eq(4)").find(".role-toggle").prop("checked");
                //console.log("Is active:", isActive);
                if (!isActive) {
                    return;
                }

                let locationName = $(this).find("td:eq(1)").text().trim();
                let roleName = $(this).find("td:eq(2)").text().trim();
                let effectiveDate = $(this).find("td:eq(3)").text().trim();
                let roleId = $(this).find("td:eq(0)").text().trim();  // Get hidden role ID
                //console.log("LocationName:", locationName);
                //console.log("RoleName:", roleName);
                //console.log("RoleId:", roleId);
                let locationId = $(this).find("td:eq(5)").text().trim(); // Try getting hidden location ID

                if (!locationId) {
                    // If not found, resolve from dropdown
                    locationId = $("#locationrole option").filter(function () {
                        return $(this).text().trim().toLowerCase() === locationName.toLowerCase();
                    }).val();
                }
                //console.log("Resolved LocationId:", locationId);
                let roleMenuHdrId = null;

                // Fetch stored roles data
                let storedRoles = sessionStorage.getItem("latestRolesData");
                if (storedRoles) {
                    let rolesData = JSON.parse(storedRoles);
                    // Find the matching role from locationWiseRoles
                    let matchingRole = rolesData.locationWiseRoles.find(r => r.role_User_Id == roleId);
                    /*console.log("matchingRole",matchingRole);*/
                    if (matchingRole) {
                        roleMenuHdrId = matchingRole.role_Menu_Header_Id;
                        /* console.log("roleMenuHdrId", roleMenuHdrId);*/
                    } else {
                        // If role_User_Id is "New", fetch roleMenuHdrId from roleMenuHeaders
                        let matchingRoleHeader = rolesData.roleMenuHeaders.find(h => h.roleName === roleName);
                        //console.log("matchingRoleHeader", matchingRoleHeader);
                        if (matchingRoleHeader) {
                            roleMenuHdrId = matchingRoleHeader.role_Menu_Hdr_Id;
                            //console.log("roleMenuHdrId", roleMenuHdrId);
                        }
                    }
                } else {
                    //console.log("No stored roles found in sessionStorage.");
                }

                if (!locationId) {
                    //console.log("Skipping row due to missing locationId");
                    return;
                }

                allMappings.push({
                    locationId: locationId,
                    locationName: locationName,
                    roleId: roleId,
                    roleName: roleName,
                    effectiveDate: effectiveDate,
                    roleMenuHdrId: roleMenuHdrId || "N/A"  // Ensure it is not null
                });
            });
            //console.log(JSON.stringify(allMappings));
            sessionStorage.setItem("nextTabRoleLocation", JSON.stringify(allMappings));
            // Navigate to next tab
            // Wait for dropdown population before switching tabs
            setTimeout(function () {
                $("#v-pills-messages-tab").tab("show");
            }, 300); // Ensures data is set before navigating
        }

        //$("#v-pills-messages-tab").on("shown.bs.tab", function () {
        //    // Ensure dropdown data is available
        //    setTimeout(function () {
        //        populateRolesAndLocationsInNextTab();
        //        populateRolesDropdown();

        //        let selectedLocationId = $("#userRoleLocation").val();
        //        let selectedRoleId = $("#userRoleEdit").val();
        //        //console.log(selectedRoleId);
        //        if (selectedLocationId && selectedRoleId) {
        //            populateRolesAndLocationsInNextTab();
        //            populateRolesDropdown();
        //        } else {
        //            $(".treeview").empty();
        //            showAlert("warning", "Please select both Location and Role before proceeding.");
        //        }
        //    }, 300);
        //});
        function populateRolesAndLocationsInNextTab() {
            let storedData = sessionStorage.getItem("nextTabRoleLocation");

            if (storedData) {
                let roleLocationData = JSON.parse(storedData);
                let locationDropdown = $("#userRoleLocation");
                /* console.log("roleLocationData", roleLocationData);*/
                locationDropdown.empty().append(`<option value="">Select Location</option>`);

                let locationMap = new Map();
                roleLocationData.forEach(mapping => {
                    if (!locationMap.has(mapping.locationId)) {
                        locationDropdown.append(`<option value="${mapping.locationId}">${mapping.locationName}</option>`);
                        locationMap.set(mapping.locationId, mapping.locationName);
                    }
                });

                locationDropdown.trigger("change");
            }
        }
        // Populate the roles dropdown dynamically
        function populateRolesDropdown() {
            let roleData = sessionStorage.getItem("nextTabRoles");

            if (roleData) {
                roleData = JSON.parse(roleData);
                let roleDropdown = $("#userRoleEdit");
                roleDropdown.empty();

                let combinedRoles = [...roleData.selectedRoles, ...roleData.activeRoles];

                let uniqueRoles = [];
                let roleIds = new Set();

                combinedRoles.forEach(role => {
                    if (!roleIds.has(role.roleId)) {
                        roleIds.add(role.roleId);
                        uniqueRoles.push(role);
                    }
                });

                uniqueRoles.forEach(role => {
                    roleDropdown.append(`<option value="${role.roleId}">${role.roleName}</option>`);
                });

                roleDropdown.trigger("change");
            }
        }

        function setEffectiveDateFromMapping(locationId, roleMenuHdrId) {
            let storedData = sessionStorage.getItem("nextTabRoleLocation");
            if (!storedData) return;

            const roleLocationData = JSON.parse(storedData);

            const matched = roleLocationData.find(item =>
                item.locationId === locationId &&
                item.roleMenuHdrId.toString() === roleMenuHdrId.toString()
            );

            if (matched && matched.effectiveDate) {
                const dateObj = convertToDateObject(matched.effectiveDate);
                $("#effectiveFromDtEdit").datepicker('setDate', dateObj);

                // Double-check and fallback if input doesn't populate
                setTimeout(() => {
                    const current = $("#effectiveFromDtEdit").val();
                    if (!current) {
                        const day = String(dateObj.getDate()).padStart(2, '0');
                        const month = String(dateObj.getMonth() + 1).padStart(2, '0');
                        const year = dateObj.getFullYear();
                        $("#effectiveFromDtEdit").val(`${day}/${month}/${year}`);
                    }
                }, 100);
            } else {
                $("#effectiveFromDtEdit").datepicker('setDate', new Date());
            }
        }

        function convertToDateObject(dashedDate) {
            const parts = dashedDate.split("-");
            return new Date(parts[2], parts[1] - 1, parts[0]);
        }
        // Ensure dropdown and role lists are populated when switching to the next tab

        $("#userRoleLocation").on("change", function () {
            let selectedLocationId = $(this).val();

            // 🔁 Reset effective date when location changes
            $("#effectiveFromDtEdit").val(""); // or use .datepicker("setDate", null) if you're using jQuery UI Datepicker

            sessionStorage.setItem("selectedLocation", selectedLocationId); // Store CorrespondanceId
            let storedData = sessionStorage.getItem("nextTabRoleLocation");

            if (storedData) {
                let roleLocationData = JSON.parse(storedData);
                let roleDropdown = $("#userRoleEdit");
                roleDropdown.empty();

                const rolesForLocation = roleLocationData.filter(r => r.locationId === selectedLocationId);

                if (rolesForLocation.length === 1) {
                    // Only one role — auto-select
                    const role = rolesForLocation[0];
                    roleDropdown.append(`<option value="${role.roleMenuHdrId}" selected>${role.roleName}</option>`);
                    roleDropdown.trigger("change");

                    // Set effective date
                    setEffectiveDateFromMapping(selectedLocationId, role.roleMenuHdrId);
                } else if (rolesForLocation.length > 1) {
                    // Multiple roles — let user choose
                    roleDropdown.append(`<option value="">Select Role</option>`);
                    rolesForLocation.forEach(mapping => {
                        roleDropdown.append(`<option value="${mapping.roleMenuHdrId}">${mapping.roleName}</option>`);
                    });
                } else {
                    // No roles available
                    roleDropdown.append(`<option value="">No roles for this location</option>`);
                }
            }
        });

        $("#userRoleEdit").change(function () {
            let roleId = $(this).val();
            let companyId = $("#Companies").val();
            let locationId = $("#userRoleLocation").val();

            // Set effective date based on selected location + role
            if (locationId && roleId) {
                setEffectiveDateFromMapping(locationId, roleId);
            }
            /* console.log(roleId);*/
            let correspondanceId = sessionStorage.getItem("selectedLocation"); // Fetch CorrespondanceId
            if (roleId && companyId) {  // Ensure roleId is not null
                fetchUserRoleMenu(roleId, companyId, userId, roleId, correspondanceId);
            }
        });

        // Function to fetch user role menu
        function fetchUserRoleMenu(roleId, companyId, userId, roleMenuHeaderId, correspondanceId) {
            $.ajax({
                url: `/User/FetchUserRoleMenuEditByUserIdRoleIdCompanyId?companyId=${companyId}&roleId=${roleId}&userId=${userId}&roleMenuHeaderId=${roleMenuHeaderId}&correspondanceId=${correspondanceId}`,
                type: "GET",
                dataType: "json",
                success: function (response) {
                    if (response.success) {
                        // alert("Data fetched successfully");

                        // Clear previous treeview data
                        $('.treeview').empty();

                        // Ensure `response.data.result` exists
                        const menuData = response.data.result;
                        console.log(menuData);
                        if (menuData && menuData.length > 0) {
                            const menuTreeHtml = generateTreeView(menuData, 0);  // Start with ParentMenu_Id = 0
                            $('.treeview').append(menuTreeHtml);
                            initializeCheckboxLogic();  // Initialize toggle functionality
                        } else {
                            showAlert("success", "No menu data available");
                        }
                    } else {
                        showAlert("danger", response.message);
                    }

                },
            });
        }

        function generateTreeView(menuItems, parentId) {
            let treeHtml = '<ul>';
            let hasCheckedChild = false; // Track if any child is checked

            menuItems.forEach(function (item) {
                if (item.parentMenuId === parentId) {
                    let permissionsHtml = '';
                    let isChecked = item.hasPerDtl ? 'checked' : '';

                    // Check if permissions need to be shown
                    if (item.hasPerDtl || item.menuName == 'Home') {
                        permissionsHtml = `
                <div id="permissions-${item.menu_Id}" class="permissions" style="padding: 10px; margin: 10px 0; border: 1px solid #ccc; border-radius: 5px; background-color: #f9f9f9;">
                    <div style="display: flex; flex-wrap: wrap; gap: 10px;">
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="add-${item.menu_Id}" ${item.grantAdd ? 'checked' : ''}> Add</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="view-${item.menu_Id}" ${item.grantView ? 'checked' : ''}> View</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="edit-${item.menu_Id}" ${item.grantEdit ? 'checked' : ''}> Edit</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="delete-${item.menu_Id}" ${item.grantDelete ? 'checked' : ''}> Delete</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="approve-${item.menu_Id}" ${item.grantApprove ? 'checked' : ''}> Approve</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="rptprint-${item.menu_Id}" ${item.grantRptPrint ? 'checked' : ''}> Report Print</label>
                        <label style="flex: 1 0 45%;"><input type="checkbox" id="rptdownload-${item.menu_Id}" ${item.grantRptDownload ? 'checked' : ''}> Report Download</label>
                    </div>
                </div>`;
                    }

                    // Recursive call for child menus
                    let childTreeHtml = generateTreeView(menuItems, item.menu_Id);

                    //// Check if any child menu is checked
                    //if (childTreeHtml.includes('checked')) {
                    //    isChecked = 'checked'; // Mark parent as checked if any child is checked
                    //    hasCheckedChild = true; // Mark that this parent has checked children
                    //}

                    if (item.menuName == 'Home') {
                        isChecked = 'checked'; // Ensure 'Home' is always checked
                    }

                    treeHtml += `
                                <li>
                                   <i class="fa fa-angle-right toggle-arrow" data-menu-id="${item.menu_Id}" style="cursor: pointer; margin-right: 5px;"></i>
        <input type="checkbox" id="menu-${item.menu_Id}" class="menu-checkbox" data-menu-id="${item.menu_Id}" ${isChecked} />
        <label for="menu-${item.menu_Id}">${item.menuName}</label>
                                    ${permissionsHtml}
                                    ${childTreeHtml}
                                </li>`;
                }
            });

            treeHtml += '</ul>';

            return treeHtml;
        }

        function initializeCheckboxLogic() {
            $(document).on("change", ".menu-checkbox", function () {
                let isChecked = $(this).is(":checked");
                let menuId = $(this).data("menu-id");
                let parentLi = $(this).closest("li");

                if (!isChecked) {
                    // Uncheck all child checkboxes and permissions
                    parentLi.find("ul .menu-checkbox, .permissions input[type='checkbox']").prop("checked", false);
                } else {
                    // Check only the permissions that were originally granted
                    restoreOriginalPermissions(menuId);
                }

                // Update parent checkboxes recursively
                updateParentCheckbox($(this));
            });

            function updateParentCheckbox(childCheckbox) {
                let parentLi = childCheckbox.closest("ul").parent("li");
                if (parentLi.length) {
                    let parentCheckbox = parentLi.children("input.menu-checkbox");
                    let anyChildChecked = parentLi.find("> ul .menu-checkbox:checked").length > 0;
                    let anyPermissionChecked = parentLi.find(".permissions input[type='checkbox']:checked").length > 0;

                    parentCheckbox.prop("checked", anyChildChecked || anyPermissionChecked);
                    updateParentCheckbox(parentCheckbox);
                }
            }

            function restoreOriginalPermissions(menuId) {
                let permissions = $(`#permissions-${menuId}`);
                if (permissions.length) {
                    permissions.find("input[type='checkbox']").each(function () {
                        let permId = $(this).attr("id");
                        if ($(`#${permId}`).data("original") === true) {
                            $(this).prop("checked", true);
                        }
                    });
                }
            }

            $(".permissions input[type='checkbox']").each(function () {
                $(this).data("original", $(this).is(":checked"));
            });

            $(document).on("change", ".permissions input[type='checkbox']", function () {
                let permissionDiv = $(this).closest(".permissions");
                let parentLi = permissionDiv.closest("li");
                let menuCheckbox = parentLi.children(".menu-checkbox");

                if ($(this).is(":checked")) {
                    menuCheckbox.prop("checked", true);
                    updateParentCheckbox(menuCheckbox);
                }
            });
            $(document).on("click", ".menu-label", function (e) {
                // Prevent triggering checkbox when label is clicked
                e.preventDefault();

                let parentLi = $(this).closest("li");
                let childUl = parentLi.children("ul");

                if (childUl.length > 0) {
                    childUl.toggle(); // Toggle visibility
                }
            });

            // Expand menu if any checkbox is checked
            $(".menu-checkbox:checked").each(function () {
                const parentLi = $(this).closest("li");
                parentLi.parents("ul").show(); // Show all ancestor <ul>
                parentLi.parents("li").children(".toggle-arrow")
                    .removeClass("fa-angle-right")
                    .addClass("fa-angle-down");
                //let toggleButton = $(this).siblings(".toggle");
                //if (!toggleButton.hasClass("expanded")) {
                //    toggleButton.addClass("expanded").text("-");
                //    $(this).siblings("ul").show();
                //}
            });
            $(document).on("click", ".toggle-arrow", function (e) {
                e.stopPropagation();

                const arrow = $(this);
                const parentLi = arrow.closest("li");
                const directChildUl = parentLi.children("ul");

                const isExpanded = arrow.hasClass("fa-angle-down");

                if (isExpanded) {
                    // COLLAPSE: hide all nested children under this parent
                    parentLi.find("ul").hide();
                    parentLi.find(".toggle-arrow")
                        .removeClass("fa-angle-down")
                        .addClass("fa-angle-right");
                } else {
                    // COLLAPSE all nested items under this parent first
                    parentLi.find("ul").hide();
                    parentLi.find(".toggle-arrow")
                        .removeClass("fa-angle-down")
                        .addClass("fa-angle-right");

                    // Then EXPAND only direct children
                    directChildUl.show();
                    arrow.removeClass("fa-angle-right").addClass("fa-angle-down");
                }
            });


            // Expand/Collapse toggle icon
            //$(document).on("click", ".toggle", function () {
            //    let childUl = $(this).siblings("ul");

            //    if (childUl.length) {
            //        childUl.toggle();

            //        // Toggle class and update text accordingly
            //        if ($(this).hasClass("expanded")) {
            //            $(this).removeClass("expanded").text("+");
            //        } else {
            //            $(this).addClass("expanded").text("-");
            //        }
            //    }
            //});
        }

        function collectSelectedPermissions() {
            let selectedMenus = [];

            $(".menu-checkbox:checked").each(function () {
                let menuId = $(this).data("menu-id");
                let roleMenuDtlId = $(this).data("role-menu-dtl-id") || 0;
                // Check if any permission is checked
                let hasPerDtl =
                    $("#add-" + menuId).is(":checked") ||
                    $("#view-" + menuId).is(":checked") ||
                    $("#edit-" + menuId).is(":checked") ||
                    $("#delete-" + menuId).is(":checked") ||
                    $("#approve-" + menuId).is(":checked") ||
                    $("#rptprint-" + menuId).is(":checked") ||
                    $("#rptdownload-" + menuId).is(":checked") ||
                    $("#docDownload-" + menuId).is(":checked") ||
                    $("#docUpload-" + menuId).is(":checked");

                selectedMenus.push({
                    RoleMenuDtlId: roleMenuDtlId,
                    RoleMenuHdrId: $("#userRoleEdit").val() || 0, // Get header ID dynamically
                    MenuId: menuId,
                    HasPerDtl: hasPerDtl,
                    Permissions: {
                        Add: $("#add-" + menuId).is(":checked"),
                        View: $("#view-" + menuId).is(":checked"),
                        Edit: $("#edit-" + menuId).is(":checked"),
                        Delete: $("#delete-" + menuId).is(":checked"),
                        Approve: $("#approve-" + menuId).is(":checked"),
                        RptPrint: $("#rptprint-" + menuId).is(":checked"),
                        RptDownload: $("#rptdownload-" + menuId).is(":checked"),
                        DocDownload: $("#docDownload-" + menuId).is(":checked"),
                        DocUpload: $("#docUpload-" + menuId).is(":checked")
                    }
                });
            });

            return selectedMenus;
        }

        $("#btnSavePermission").click(function () {

            let isValid = true;

            // Location Validation
            const locationId = $("#userRoleLocation").val();
            if (!locationId) {
                $("#userRoleLocation-error").text("Location is required.").show();
                isValid = false;
            } else {
                $("#userRoleLocation-error").text("").hide(); // ✅ Clear location error only when valid
            }

            // Role Validation
            const roleId = $("#userRoleEdit").val();
            //console.log("roleIdroleId", roleId);
            //console.log("roleIdroleId", roleId.length);
            if (!roleId || roleId.length === 0) {
                $("#userRoleEdit-error").text("Role is required.").show();
                isValid = false;
            } else {
                $("#userRoleEdit-error").text("").hide(); // ✅ Clear role error only when valid
            }

            // Effective Date Validation
            const dateValue = $("#effectiveFromDtEdit").val();
            let formattedEditDate = null;
            if (dateValue) {
                const parts = dateValue.split('/'); // Split 'dd/mm/yyyy'
                if (parts.length === 3) {
                    formattedEditDate = `${parts[2]}-${parts[1]}-${parts[0]}`; // Convert to 'yyyy-mm-dd'
                }
            }

            if (formattedEditDate) {
            }
            //const formattedEditDate = dateValue ? new Date(dateValue).toISOString().split('T')[0] : null;

            if (!dateValue) {
                $("#effectiveFromDtEdit-error").text("Effective Date is required.").show();
                isValid = false;
            } else {
                $("#effectiveFromDtEdit-error").text("").hide(); // ✅ Clear date error only when valid
            }



            // Stop submission if validation fails
            if (!isValid) {
                return;
            }
            let selectedMenus = collectSelectedPermissions();

            if (!user || !user.user_id) {
                alert("User information is missing.");
                return;
            }
            if (selectedMenus.length === 0) {
                alert("Please select at least one permission.");
                return;
            }


            let requestData = {
                RoleId: roleId, // Get selected RoleId from dropdown
                RoleMenuHdrId: roleId || 0, // Auto-generated
                UserId: user.user_id,
                CompanyId: $("#Companies").val(),
                CorrespondanceId: locationId,
                EffectiveFromDt: formattedEditDate,
                //EffectiveFromDt: new Date().toISOString(),
                CreatedBy: user.user_id,
                PermissionsData: selectedMenus
            };
            $.ajax({
                url: "/User/SaveUserRoleMenuPermissions",
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(requestData),
                success: function (response) {
                    if (response.type === "success") {
                        //showAlert("success", response.message);
                        // Correct Bootstrap 5 Modal Initialization
                        const userEditModal = document.getElementById('userEditModal');
                        if (userEditModal) {
                            const modalInstance = new bootstrap.Modal(userEditModal);
                            modalInstance.show();
                        } else {
                        }
                    } else {
                        showAlert("danger", response.message);
                    }
                },
                error: function (xhr, status, error) {
                    showAlert("danger", "An error occurred while save permissions.");

                }
            });
        });

        $("#yesEditRole").click(function () {
            var userEditModal = bootstrap.Modal.getInstance(document.getElementById('userEditModal'));
            userEditModal.hide();
        });

        $("#noAnotherRole").click(function () {
            // ✅ Clear specific session storage items
            sessionStorage.removeItem("userData");
            sessionStorage.removeItem("nextTabRoles");
            // Redirect to the list page
            window.location.href = "/User/Index";
        });

        function removeRole(roleId) {
            // Implement logic to remove the role from userRoles array and update the UI
        }
        // If using Select2, initialize it
        if ($.fn.select2) {
            $("#role").select2({
                placeholder: "Select role",
                allowClear: true
            });
        }
        // Load data based on tab click
        //$('button[data-bs-toggle="pill"]').on("shown.bs.tab", function (e) {
        //    var target = $(e.target).attr("data-bs-target");

        //    if (target === "#v-pills-home") {
        //        fillUserInformation();
        //    } else if (target === "#v-pills-profile-edit") {
        //        fillCompanyDetails();
        //    } else if (target === "#v-pills-mapping") {
        //        //fillUserLocationsAndRoles(); // ✅ Ensure grid refreshes
        //        //fillUserRoles();
        //        //loadRolesDropdown();
        //    }
        //});
        //// Load User Information by default
        //fillUserInformation();
        // Unified handler for all tab switches
        $(document).on("shown.bs.tab", 'button[data-bs-toggle="pill"]', function (e) {
            var target = $(e.target).attr("data-bs-target");
            handleTabActivation(target);
        });
        function handleTabActivation(targetId) {
            switch (targetId) {
                case "#v-pills-home":
                    fillUserInformation();
                    break;
                case "#v-pills-profile-edit":
                    fillCompanyDetails();
                    fillUserLocations();
                    break;
                case "#v-pills-mapping":
                    fillUserLocationsAndRoles();
                    break;
                case "#v-pills-messages":
                    // Ensure required sessionStorage is set
                    let nextTabRoleLocation = sessionStorage.getItem("nextTabRoleLocation");
                    if (!nextTabRoleLocation) {
                        storeRolesForNextTab(); // Prepare the sessionStorage if missing
                    }

                    populateRolesAndLocationsInNextTab();
                    populateRolesDropdown();
                    setTimeout(function () {
                        //populateRolesAndLocationsInNextTab();
                        //populateRolesDropdown();
                        let selectedLocationId = $("#userRoleLocation").val();
                        let selectedRoleId = $("#userRoleEdit").val();
                        if (selectedLocationId && selectedRoleId) {
                            populateRolesAndLocationsInNextTab();
                            populateRolesDropdown();
                        } else {
                            $(".treeview").empty();
                            showAlert("warning", "Please select both Location and Role before proceeding.");
                        }
                    }, 300);
            }
        }

       

        // Handle Next Tab Button Click
        $("#nextEditTab").on("click", function () {
            let activeTab = $("#v-pills-tab .nav-link.active");
            let nextTab = activeTab.next();

            if (nextTab.length) {
                let nextTabContentId = nextTab.attr("data-bs-target");

                // Switch tab manually
                activeTab.removeClass("active");
                nextTab.addClass("active");
                $(".tab-pane").removeClass("show active");
                $(nextTabContentId).addClass("show active");

                // Manually trigger the tab change event
                /*  nextTab.trigger("shown.bs.tab");*/
                nextTab.tab("show"); // Triggers BS5 event properly

                // Call data loading functions manually
                if (nextTabContentId === "#v-pills-home") {
                    fillUserInformation();
                } else if (nextTabContentId === "#v-pills-profile-edit") {
                    fillCompanyDetails();
                    fillUserLocations();
                    //fillUserRoles();
                }
            }
        });

        // **Tab Click Event Listener**
        $("#v-pills-profile-edit-tab").on("shown.bs.tab", function () {
            fillCompanyDetails();
            fillUserLocations();
            //fillUserRoles();
        });
        // Fetch locations and roles when the tab is clicked
        $("#locationrole").on("change", function () {
            let selectedLocationId = $(this).val();
            if (selectedLocationId) {
                fetchRolesByLocation(selectedLocationId);
            } else {
                $("#role").empty().append(`<option value="">Select Role</option>`);
            }
        });

        // Save Mapping button click event
        $("#saveMapping").on("click", function () {
            let selectedLocationId = $("#locationrole").val();
            let selectedLocationName = $("#locationrole option:selected").text();
            let selectedRoles = $("#role").val(); // Get multiple selected roles
            let effectiveFrom = new Date();
            let formattedDate = ("0" + effectiveFrom.getDate()).slice(-2) + "-" +
                ("0" + (effectiveFrom.getMonth() + 1)).slice(-2) + "-" +
                effectiveFrom.getFullYear();
            // Clear previous error messages
            $("#locationrole-error").text("");
            $("#role-error").text("");
            // Validation: Ensure location and at least one role is selected
            if (!selectedLocationId) {
                $("#locationrole-error").text("Please select a Location before saving."); // Display error in text box
                return;
            }
            if (!selectedRoles || selectedRoles.length === 0 || selectedRoles == null) {
                $("#role-error").text("Please select at least one Role before saving.").show();
                return;
            }

            // Loop through each selected role and add a separate row for each
            selectedRoles.forEach(function (roleId) {
                let roleName = $("#role option[value='" + roleId + "']").text();

                // Check if this location-role pair already exists in the grid
                let isDuplicate = false;
                $("#userRolesGrid tbody tr").each(function () {
                    let existingLocation = $(this).find("td:eq(1)").text();
                    let existingRole = $(this).find("td:eq(2)").text();
                    if (existingLocation === selectedLocationName && existingRole === roleName) {
                        isDuplicate = true;
                        return false; // Break loop
                    }
                });

                if (!isDuplicate) {
                    // Dynamically add the new row to the grid
                    let newRow = `<tr>
                <td hidden>New</td>
                <td>${selectedLocationName}</td>
                <td>${roleName}</td>
                <td>${formattedDate}</td> <!-- Updated date format -->
                <td class="sticky_cell">
                    <div class="form-check form-switch">
                        <input class="form-check-input role-toggle" type="checkbox" role="switch" checked>
                        <label class="form-check-label">Active</label>
                    </div>
                </td>
            </tr>`;

                    $("#userRolesGrid tbody").append(newRow);
                }
            });

            // Reset dropdowns for new entry
            $("#locationrole").val('').trigger("change");
            $("#role").empty().append(`<option value="">Select Role</option>`);

            showAlert("success", "Mapping(s) added successfully.");
        });
    }
    $('#effectiveFromDtEdit').datepicker({
        format: 'dd/mm/yyyy',
        startDate: '0d',
        autoclose: true,
        todayHighlight: true
    }).on('changeDate', function (e) {
        // Allow valid date
        if (new Date(e.date) < new Date().setHours(0, 0, 0, 0)) {
            $('#effectiveFromDtEdit').val('');
            showAlert("warning", "Past dates are not allowed.");
        }
    });

    // Detect manual clearing or backspace reset
    $('#effectiveFromDtEdit').on('input', function () {
        if (!$(this).val()) {
            let locId = $("#userRoleLocation").val();
            let roleId = $("#userRoleEdit").val();
            if (locId && roleId) {
                setEffectiveDateFromMapping(locId, roleId);
            }
        }
    });

});
