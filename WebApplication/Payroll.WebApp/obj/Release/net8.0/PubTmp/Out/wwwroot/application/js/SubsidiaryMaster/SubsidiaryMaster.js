// AddSubsidiaryMaster Code

$(document).ready(function () {

	$('#SubsidiaryCode').val(''); // Resets a textbox with id="myTextbox"
	$("#SubsidiaryName").val('');


	// Get the "data" parameter from the URL query string
	const dataParam = new URLSearchParams(window.location.search).get('subsidiary_Id');

	// Decode the URL-encoded data
	const decodedData = decodeURIComponent(dataParam);

	// Parse the decoded string into a JavaScript object
	const dataObject = JSON.parse(decodedData);
	//console.log("subsidiaryData", dataObject);
	// Now you can access the "subsidiary_Name"
	function fetchSubsidiaryData(subsidiaryId) {
		$.ajax({
			method: 'GET',
			url: `/SubsidiaryMaster/GetSubsidiaryById?subsidiaryId=${subsidiaryId}`, // Ensure correct endpoint
			contentType: 'application/json',
			dataType: "json",
			success: function (response) {
				//console.log(response); // Log the full response
				if (response && response.data) {

					var subsidiaryData = response.data;
				//console.log("data", subsidiaryData);

					const subsidiaryName = subsidiaryData.subsidiary_Name;
					const subsidiaryCode = subsidiaryData.subsidiary_Code;
					$("#GlobalSubsidiaryId").val(subsidiaryData.subsidiary_Id);

					// Log the result to the console or use it as needed
					$("#SubsidiaryName").val(subsidiaryName.trim());
					$("#SubsidiaryCode").val(subsidiaryCode.trim()).prop('readonly', true);;

					$('#toggleContainer').css('visibility', 'visible'); // or use .show()


					if (subsidiaryData.isActive) {
						$("#SubsidiaryActiveToggle").prop('checked', true); // Set checkbox to checked
						$('#activeStatusLabel').text('Active');
					} else {
						$("#SubsidiaryActiveToggle").prop('checked', false); // Set checkbox to unchecked
						$('#activeStatusLabel').text('In-Active');
					}
					// Set Subsidiary Type dropdown value
					populateDropdown("#SubsidiaryType", subsidiaryData.subsidiaryType_Id);

					// Set Company dropdown value (if the company dropdown is populated with options)
					populateDropdown("#SubsidiaryCompany", subsidiaryData.company_Id);

					// Set Country dropdown value (if the country dropdown is populated with options)
					populateDropdown("#SubsidiaryCountry", subsidiaryData.countryId);

					// Set State dropdown value (if the state dropdown is populated with options)
					populateDropdown("#SubsidiaryState", subsidiaryData.state_Id);

					// Set City dropdown value (if the city dropdown is populated with options)
					populateDropdown("#SubsidiaryCity", subsidiaryData.cityid);

					// Set Location dropdown value (if the location dropdown is populated with options)
					populateDropdown("#SubsidiaryLocation", subsidiaryData.location_ID);

					// Set Area dropdown value (if the area dropdown is populated with options)
					populateDropdown("#SubsidiaryArea", subsidiaryData.area_id);

					function populateDropdown(dropdownId, selectedValue) {
						// Check if the dropdown has options before setting the value
						var $dropdown = $(dropdownId);

						// Log the dropdown and selected value for debugging
						//console.log(`Populating dropdown ${dropdownId} with selected value: ${selectedValue}`);

						// Check if the selected value is available as an option
						var optionExists = $dropdown.find('option[value="' + selectedValue + '"]').length > 0;

						if (optionExists) {
							$dropdown.val(selectedValue); // Set the selected value
							$dropdown.trigger('change');  // Trigger change event to refresh the dropdown (if needed)
							//console.log(`${dropdownId} populated with value ${selectedValue}`);
						} else {
							//console.log(`No matching option for value ${selectedValue} in dropdown ${dropdownId}`);
						}
					}

				} else {
					//console.error("No valid data received from server.");
					showAlert("error", "No data found for the given Subsidiary ID.");
				}
			},
			error: function (xhr, status, error) {
				//console.error("AJAX Error:", error);
				showAlert("error", "Error updating record.");
			}
		});

	}

	if (dataObject) {
		const subsidiaryId = dataObject;
		fetchSubsidiaryData(subsidiaryId);			
	}
	else {
		$('form')[0].reset();
	}
	// Function to populate a dropdown and set the selected value
	

	//validations for mandatory fields start----

	$('#SubsidiaryType', '#SubsidiaryCompany', '#SubsidiaryCountry', '#SubsidiaryState', '#SubsidiaryCity', '#SubsidiaryLocation', '#SubsidiaryArea').on('change', function () {
		validateField($(this).attr("id"), "This field is required!");
	});


	function validateField(fieldId, errorMessage) {
		const field = $(`#${fieldId}`);
		const errorElement = $(`#${fieldId}-error`);
		const value = field.val() ? String(field.val()).trim() : '';  // Ensure the value is always a string

		// Add event listener to remove error on input change
		field.on('input', function () {
			const currentValue = field.val() ? String(field.val()).trim() : '';

			if (currentValue) {
				// If the field has value, remove error class and error message
				field.removeClass('error_input');
				errorElement.text('').hide();
			}
		});

		// If field value is empty, show error
		if (!value) {
			field.addClass('error_input');
			errorElement.text(errorMessage).show();
			return false;
		} else {
			// If field value is valid, remove error
			field.removeClass('error_input');
			errorElement.text('').hide();
			return true;
		}
	}

	$('#SubsidiaryName').on('input', function () {
		const SubsidiaryName = $(this).val();
		if (!validateSpecialCharacters(SubsidiaryName)) {
			$(this).val(SubsidiaryName.replace(/[^a-zA-Z\s]/g, ''));
			$('SubsidiaryName-error').text('SubsidiaryName cannot contain special characters or numbers.');
		}
		else {
			$('#SubsidiaryName-error').text('');
		}
	});


	function validateSpecialCharacters(name) {
		const specialCharRegex = /^[a-zA-Z\s]*$/; // Allows only letters and spaces
		return specialCharRegex.test(name);
	}

	function validateMinLength(fieldId, minLength) {
		const field = $(`#${fieldId}`);
		const value = field.val().trim();
		if (value.length < minLength) {
			field.addClass('error_input');
			$(`#${fieldId}-error`).text(`Minimum ${minLength} characters required`).show();
			return false;
		}
		return true;
	}

	function validatefieldsLength() {
		var isValid = true;
		isValid &= validateMinLength('SubsidiaryName', 2);
		isValid &= validateMinLength('SubsidiaryCode', 3);
		return isValid;
	}
	function validateFieldLength(fieldId, maxLength, errorMessage) {
		const field = $(`#${fieldId}`);
		const value = field.val() ? String(field.val()).trim() : ''; // Ensure the value is always a string
		const errorElement = $(`#${fieldId}-error`);

		// Add event listener to remove error on input change
		field.on('input', function () {
			const currentValue = field.val() ? String(field.val()).trim() : '';

			if (currentValue.length <= maxLength) {
				// If the field length is valid, remove error class and error message
				field.removeClass('error_input');
				errorElement.text('').hide();
			}
		});

		// Validate length
		if (value.length > maxLength) {
			field.addClass('error_input');
			errorElement.text(errorMessage).show();
			return false;
		} else {
			// If field length is valid, remove error
			field.removeClass('error_input');
			errorElement.text('').hide();
			return true;
		}
	}

	function validateSubsidiaryTab() {
		var isValid = true;
		isValid &= validateField('SubsidiaryName', 'Please Enter SubsidiaryName');
		isValid &= validateField('SubsidiaryCode', 'Please Enter SubsidiaryCode');
		isValid &= validateField('SubsidiaryType', 'Please Select SubsidiaryType');
		isValid &= validateField('SubsidiaryCompany', 'Please Select Subsidiary Company');
		isValid &= validateField('SubsidiaryCountry', 'Please Select Subsidiary Country');
		isValid &= validateField('SubsidiaryState', 'Please Select Subsidiary State');
		isValid &= validateField('SubsidiaryCity', 'Please Select Subsidiary City');
		isValid &= validateField('SubsidiaryLocation', 'Please Select Subsidiary Location');
		isValid &= validateField('SubsidiaryArea', 'Please Select Subsidiary Area');
		isValid &= validatefieldsLength();
		isValid &= validateFieldLength('SubsidiaryName', 100, 'Subsidiary Name should not exceed 100 characters.');

		return isValid;
	}

	//validations for mandatory fields end----


	$('#SubsidiaryActiveToggle').change(function () {
		updateStatusLabel();
	});

	function updateStatusLabel() {
		if ($('#SubsidiaryActiveToggle').prop('checked')) {
			$('#activeStatusLabel').text('Active');
		}
		else {
			$('#activeStatusLabel').text('Inactive');
		}
	}

	//$('#subsidiaryOnboarding').on("click", function (e) {
	//	window.location.href = "/SubsidiaryMaster/Index";
	//});

	$('#subsidiaryMasterclear').on("click", function (e) {
		var subsidiaryIdForReset = $("#GlobalSubsidiaryId").val();
		if (subsidiaryIdForReset) {
			fetchSubsidiaryData(subsidiaryIdForReset);
		}
		else {
			$("#addTab")[0].reset();
			$(".select2_search_ctm").val(null).trigger('change');
			location.reload();
		}		
	});

	//$('#btnSaveSubsidiary').on('click', function (e) {

	$('button[data-bs-toggle="btnSave"]').on('click', function (e) {
		const clickButton = $(this).attr("id");

		if (clickButton === "btnSaveSubsidiary" && !validateSubsidiaryTab())
		{
			//showAlert("danger", "Error at validating fields!");
			e.preventDefault();
			return;  // Prevent further execution if validation fails
		}

		// Validate dropdowns
		$('select').each(function () {
			const errorMsg = validateDropdown($(this));
			if (errorMsg) {
				$(this).next('.error-message').text(errorMsg);
				isValid = false;
			} else {
				$(this).next('.error-message').text('');
			}
		});

		// Validation for dropdown
		function validateDropdown(dropdown) {
			const selectedValue = dropdown.val();
			if (selectedValue === "" || selectedValue === null) {
				return `Please select ${dropdown.attr('data-name')}.`;
			}
			return null;

		}
		const dataParam = new URLSearchParams(window.location.search).get('subsidiary_Id');

		// Decode the URL-encoded data
		const decodedData = decodeURIComponent(dataParam);

		// Parse the decoded string into a JavaScript object
		const subsidiary_Id = JSON.parse(decodedData);

		const data = {
			SubsidiaryType_Id: $("#SubsidiaryType").val(),
			Company_Id: $("#SubsidiaryCompany").val(),
			Subsidiary_Code: $("#SubsidiaryCode").val().trim(),
			Subsidiary_Name: $("#SubsidiaryName").val().trim(),
			Location_ID: $("#SubsidiaryLocation").val(),
			CountryId: $("#SubsidiaryCountry").val(),
			Area_id: $("#SubsidiaryArea").val(),
			State_Id: $("#SubsidiaryState").val(),
			Cityid: $("#SubsidiaryCity").val(),
			IsActive: $("#SubsidiaryActiveToggle").prop("checked")
		};
		//console.log(data);
		var url = "/SubsidiaryMaster/AddSubsidiaryMaster";  // Default to Add
		var method = "POST";  // Default method is POST (for adding)

		if (subsidiary_Id) {
			// If subsidiaryData exists, it's an update operation
			url = "/SubsidiaryMaster/UpdateSubsidiaryMaster";  // Update endpoint
			method = "POST";  // Use PUT method for updating
			data.Subsidiary_Id = subsidiary_Id;
		}

		//console.log("Data being sent:", JSON.stringify(data));
		//console.log("Request URL:", url);
		

		$.ajax({
			method: method,
			url: url,
			contentType: "application/json",
			dataType: "json",
			data: JSON.stringify(data),
			headers: {
				"Accept": "application/json"
			},
			success: function (response) {
				//console.log("Success:", response);
				if (response && response.success) {
					showAlert("success", response.message);
					//window.location.href = "/SubsidiaryMaster/Index";
				//	window.location.href = `/SubsidiaryMaster/Index?alertType=success&alertMessage=${encodeURIComponent(response.message)}`;
					//
					//alert("Subsidiary " + (subsidiaryData ? "Updated" : "Added") + " successfully!");
				} else {
					showAlert("danger", response.message);
				}
			},
			error: function (xhr, status, error) {
				//console.error("AJAX Error:", error);
				//console.error("Response Text:", xhr.responseText);
				showAlert("danger", "An error occurred while creating the subsidiary: " + xhr.responseText);
			}
		});
	});
});

$('#btncancel').on('click', function () {
	window.location.href = '/Company/CompanyList';
});
// Subsidiary List Code

///////////////////////////////////Code from FORAM Start:-/////////////////////////////////
$(document).ready(function () {
	const urlParams = new URLSearchParams(window.location.search);
	const pageLabel = document.getElementById("pageName");

	console.log("Has ID:", urlParams.has("subsidiary_Id"));

	if (pageLabel) {
		if (urlParams.has("subsidiary_Id")) {
			pageLabel.textContent = "Update Company Subsidiary";
		} else {
			pageLabel.textContent = "Add Company Subsidiary";
		}
	} else {
		console.warn("Element #pageName not found");
	}
});
///////////////////////////////////Code from FORAM End:-/////////////////////////////////



