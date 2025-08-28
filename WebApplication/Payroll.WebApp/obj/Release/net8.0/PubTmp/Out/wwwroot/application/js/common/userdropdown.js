/****************************************************************************************************
 *  Jira Task Ticket : PAYROLL-280,290                                                              *
 *  Description:                                                                                    *
 *  This script handles the dynamic population of cascading dropdowns for user-related data.        *
 *  The data is fetched using AJAX calls, and once the user selects an option in a dropdown, it     *
 *  triggers the population of subsequent dropdowns based on the previous selection. The data       *
 *  is cached globally to ensure that multiple dropdowns can reuse it efficiently.                  *
 *  The dropdowns are populated using a set of predefined static dropdown configurations and        *
 *  synchronous API calls. The Select2 library is used to enhance the user experience.              *
 *                                                                                                  *
 *  Methods:                                                                                        *
 *  - populateDropdown : Populates static dropdowns with data from specified API endpoints.         *
 *  - populateCascadingDropdown : Populates cascading dropdowns dynamically based on previous       *
 *    selections.                                                                                   *
 *  - clearDropdownsFrom : Clears dropdowns starting from a specific selector.                      *
 *  - fetchSync : Fetches data synchronously from a given API URL.                                  *
 *  - initializeSelect2 : Initializes the Select2 dropdown for improved user interface.             *
 *                                                                                                  *
 *  Author: Priyanshi Jain                                                                          *
 *  Date  : 30-Dec-2024                                                                             *
 *                                                                                                  *
 ****************************************************************************************************/
/* #region 1: Common DropDown and Cascading DropDown */
$(document).ready(function () {
    // Store the response data globally for reuse
    let cascadingDataDropDown = {};

    // Static dropdown configurationsusertype
    const dropdownConfigs = [
        { selector: '#usertype', url: '/DropDown/FetchUserTypeDropdown', placeholder: 'Select UserType', valueKey: 'value', textKey: 'text', autoSelectFirst: false },
        { selector: '#countries', url: '/DropDown/FetchCountriesDropdown', placeholder: 'Country', valueKey: 'value', textKey: 'text', autoSelectFirst: false },
        { selector: '#salutationsDropdown', url: '/DropDown/FetchSalutationsDropdown', placeholder: 'Salutation', valueKey: 'value', textKey: 'text', autoSelectFirst: false },
        { selector: '#Companies', url: '/DropDown/FetchCompaniesDropdown', placeholder: 'Select Company', valueKey: 'value', textKey: 'text', autoSelectFirst: false },
        { selector: '#location', url: '/DropDown/FetchLocationsDropdown', placeholder: 'Select Location', valueKey: 'value', textKey: 'text', autoSelectFirst: true },
        { selector: '#CountryDropdown', url: '/DropDown/FetchCountriesDropdown', placeholder: '', valueKey: 'value', textKey: 'text', autoSelectFirst: true },
    ];

    // Populate static dropdowns
    dropdownConfigs.forEach(config => populateDropdown(config));

    // Event handlers for cascading dropdowns
    $('#Companies').change(function () {
        const companyId = $(this).val();

        // Retrieve userId dynamically from session storage, if available
        var userId = null;
        var userData = sessionStorage.getItem("userData");
        if (userData) {
            var user = JSON.parse(userData);
            userId = user ? user.user_id : null;
        }
        if (companyId) {
            const response = fetchSync('/DropDown/GetCompanyLocationData', { companyId, userId });

            if (response && response.isSuccess && response.result) {
                cascadingDataDropDown = response.result; // Store data globally
                populateCascadingDropdown('#countriesname', cascadingDataDropDown.countries, 'country_Id', 'countryName', 'Select Country');
            } else {
                console.error('Error fetching cascading data');
                clearDropdownsFrom('#countriesname', 'Select Country');
            }
        } else {
            clearDropdownsFrom('#countriesname', 'Select Country');
        }
    });
    $('#countriesname').change(function () {
        const countryId = $(this).val();
        const states = cascadingDataDropDown.states?.filter(s => s.countryId == countryId) || [];
        populateCascadingDropdown('#state', states, 'state_Id', 'stateName', 'Select State');
    });
    $('#state').change(function () {
        const stateId = $(this).val();
        const cities = cascadingDataDropDown.cities?.filter(c => c.state_Id == stateId) || [];
        populateCascadingDropdown('#branch', cities, 'city_ID', 'city_Name', 'Select City');
    });
    $('#branch').change(function () {
        const cityId = $(this).val();
        const locations = cascadingDataDropDown.locations?.filter(l => l.cityId == cityId) || [];
        populateCascadingDropdown('#department', locations, 'correspondance_ID', 'locationName', 'Select Location');
    });
    $('#department').change(function () {
        const roles = cascadingDataDropDown.roles || [];
        populateCascadingDropdown('#role', roles, 'role_Menu_Hdr_Id', 'roleName', 'Select Role');
    });
    // Synchronous fetch function
    function fetchSync(url, data) {
        let result = null;
        $.ajax({
            url: url,
            type: 'GET',
            data: data,
            async: false,
            success: function (response) {
                result = response;
            },
            error: function (xhr, status, error) {
                console.error(`Error fetching data from ${url}:`, error);
            }
        });
        return result;
    }

    // Populate a dropdown dynamically
    function populateCascadingDropdown(selector, data, valueKey, textKey, placeholder) {
        const dropdown = $(selector);
        dropdown.empty();
        dropdown.append($('<option>', { value: '', text: placeholder }));
        if (data.length) {
            data.forEach(item => dropdown.append($('<option>', { value: item[valueKey], text: item[textKey] })));
        } else {
            dropdown.append($('<option>', { value: '', text: 'No data available' }));
        }
        initializeSelect2(selector, placeholder);
    }

    // Clear dropdowns starting from a specific selector
    function clearDropdownsFrom(selector, placeholder) {
        const placeholders = {
            '#countriesname': 'Select Country',
            '#state': 'Select State',
            '#branch': 'Select City',
            '#department': 'Select Location',
            '#role': 'Select Role'
        };

        const selectors = ['#countriesname', '#state', '#branch', '#department', '#role'];
        const index = selectors.indexOf(selector);
        if (index >= 0) {
            selectors.slice(index).forEach(sel => {
                const dropdown = $(sel);
                dropdown.empty().append($('<option>', { value: '', text: placeholders[sel] })); // Reset placeholder
                dropdown.trigger('change'); // Trigger Select2 update
            });
        }
    }

    // Populate static dropdowns
    function populateDropdown({ selector, url, placeholder, valueKey, textKey, autoSelectFirst = false }) {
        const dropdown = $(selector);
        dropdown.empty();
        const response = fetchSync(url);  // Assuming `fetchSync` is a function that handles synchronous data fetching.
        if (response && response.length > 0) {
            dropdown.append($('<option>', { value: '', text: placeholder }));
            response.forEach(item => dropdown.append($('<option>', { value: item[valueKey], text: item[textKey] })));
            if (autoSelectFirst) dropdown.val(response[0][valueKey]).trigger('change');
        } else {
            dropdown.append($('<option>', { value: '', text: 'No data available' }));
        }
        initializeSelect2(selector, placeholder);
    }

    // Initialize Select2 for better UX
    function initializeSelect2(selector, placeholder) {
        $(selector).select2({
            placeholder: placeholder,
            allowClear: true,
            width: '100%'
        });
    }
    /* #region 4: Reset all dropdwon data based on pages */
    $('#resetButton').click(function (e) {
        e.preventDefault(); // Prevent default button behavior

        // Check which tab is active
        if ($('#firstTab').is(':visible')) {
            // Reset all input fields and textareas in the first tab
            $('#firstTab')[0].reset();

            // Reset static dropdowns in the first tab by reinitializing them
            dropdownConfigs.forEach(config => {
                if ($(config.selector).closest('#firstTab').length) {
                    populateDropdown(config); // Reinitialize dropdown with the original logic
                }
            });

        } else if ($('#secondTab').is(':visible')) {
            // Reset all input fields and textareas in the second tab
            $('#secondTab')[0].reset();

            // Reset static dropdowns in the second tab by reinitializing them
            dropdownConfigs.forEach(config => {
                if ($(config.selector).closest('#secondTab').length) {
                    populateDropdown(config); // Reinitialize dropdown with the original logic
                }
            });

            // Reset cascading dropdowns in the second tab to their default state
            clearDropdownsFrom('#countriesname', 'Select Country');
        }

        // Additional handling to clear error classes or custom states for the active tab
        $('input, textarea', '#firstTab:visible, #secondTab:visible').removeClass('error'); // Remove error class
    });
    /* #endregion */
});
/* #endregion */

/* #region 2: Geographical Location DropDown Value */

const GeographicdropdownConfigs = [
    { selector: '#CountryDropdown', url: '/DropDown/FetchCountriesDropdown', placeholder: 'Select Country' }
];

function fetchDropdownData(url, data, dropdownId, placeholderText) {
    $.ajax({
        url: url,
        type: 'GET',
        data: data,
        async: false,
        success: function (response) {
            populateDropdown(dropdownId, response, placeholderText);
        },
        error: function (xhr, status, error) {
            console.error('Failed to fetch data for ' + dropdownId + ': ' + error);
        }
    });
}

function populateDropdown(dropdownId, items, placeholderText) {
    const dropdown = $(dropdownId);
    dropdown.empty().append(new Option(placeholderText, '')); // Placeholder option as default
    items.forEach(item => {
        dropdown.append(new Option(item.text, item.value));
    });
}

function clearDropdowns(dropdownIds, placeholderText) {
    dropdownIds.forEach(dropdownId => {
        $(dropdownId).empty().append(new Option(placeholderText, ''));
    });
}

// Initialize country dropdown on page load
$(document).ready(function () {
    GeographicdropdownConfigs.forEach(config => {
        fetchDropdownData(config.url, {}, config.selector, config.placeholder);
    });

    // Country selection event
    $('#CountryDropdown').change(function () {
        const countryId = $(this).val();
        if (countryId) {
            fetchDropdownData('/DropDown/FetchStateDropdown', { Country_Id: countryId }, '#StateDropdown', 'Select State');
            clearDropdowns(['#CityDropdown', '#LocationDropdown'], 'Select City/Location');
        } else {
            clearDropdowns(['#StateDropdown', '#CityDropdown', '#LocationDropdown'], 'Select Option');
        }
    });

    // State selection event
    $('#StateDropdown').change(function () {
        const stateId = $(this).val();
        if (stateId) {
            fetchDropdownData('/DropDown/FetchCityDropdown', { State_ID: stateId }, '#CityDropdown', 'Select City');
            clearDropdowns(['#LocationDropdown'], 'Select Location');
        } else {
            clearDropdowns(['#CityDropdown', '#LocationDropdown'], 'Select Option');
        }
    });

    // City selection event
    $('#CityDropdown').change(function () {
        const cityId = $(this).val();
        if (cityId) {
            fetchDropdownData('/DropDown/FetchLocationsDropdown', { City_ID: cityId }, '#LocationDropdown', 'Select Location');
        } else {
            clearDropdowns(['#LocationDropdown'], 'Select Location');
        }
    });
});

/* #endregion */


/* #region 3: DropDown Value Save in Cache and Disply */
//// Function to populate dropdown with data using jQuery
//function populateDropdownWithJQuery(dropdownId, data, defaultOption = "") {
//    const $dropdown = $(`#${dropdownId}`);

//    // Check if the dropdown element exists
//    if (!$dropdown.length) {
//        //console.error(`Dropdown with ID ${dropdownId} not found.`);
//        return;
//    }

//    // Clear any existing options
//    $dropdown.empty();

//    // Add the default option
//    $dropdown.append(`<option value="">${defaultOption}</option>`);

//    // Ensure data is not empty before adding options
//    if (data.length > 0) {
//        // Loop through the data and append options to the dropdown
//        $.each(data, function (index, item) {
//            $dropdown.append(new Option(item.text, item.value));  // Efficient way to add options
//        });

//        // Auto-select the first option
//        $dropdown.prop('selectedIndex', 1);  // Select the first option (excluding the default)
//    } else {
//        // Handle case when there are no options
//        $dropdown.append('<option value="">No countries available</option>');
//    }
//}

//// Function to fetch countries data with caching
//async function cacheCountriesData() {
//    const apiUrl = "/DropDown/FetchCountriesDropdown"; // API endpoint for countries data

//    // Open a cache storage to store the fetched data
//    const cache = await caches.open("app-cache");

//    // Check if data is already in the cache
//    const cachedResponse = await cache.match(apiUrl);
//    if (cachedResponse) {
//        console.log("Data fetched from cache"); // Log cache hit
//        return await cachedResponse.json(); // Return cached data
//    }

//    // Fetch data from the backend API if not found in cache
//    const response = await fetch(apiUrl);
//    if (response.ok) {
//        // Save the response in cache for future use
//        cache.put(apiUrl, response.clone());
//        console.log("Data fetched from API and cached");
//        return await response.json(); // Return fetched data
//    } else {
//        console.error("Failed to fetch countries data:", response.status);
//        return []; // Return empty array on failure
//    }
//}
//// On document ready, fetch countries data and populate dropdowns
//$(document).ready(async function () {
//    const countries = await cacheCountriesData();

//    // Check if countries data is available
//    if (countries && countries.length > 0) {
//        // Populate dropdowns with countries data and auto-select the first option
//        populateDropdownWithJQuery("Country_ID", countries);
//        populateDropdownWithJQuery("secondarycountries", countries);
//        populateDropdownWithJQuery("phone1countries", countries);
//        populateDropdownWithJQuery("phone2countries", countries);
//    } else {
//        console.error("No countries data available.");
//    }
//});
/* #endregion */



