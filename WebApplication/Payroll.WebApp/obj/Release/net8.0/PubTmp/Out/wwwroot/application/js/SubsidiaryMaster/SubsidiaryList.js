$(document).ready(function () {

	const urlParams = new URLSearchParams(window.location.search);
	const alertType = urlParams.get('alertType');
	const alertMessage = urlParams.get('alertMessage');

	if (alertType && alertMessage) {
		// Show SweetAlert based on the parameters
		Swal.fire({
			title: alertType === 'success' ? 'Success' : 'Error',
			text: decodeURIComponent(alertMessage),
			icon: alertType,
			confirmButtonText: 'OK'
		});
	}


	$(document).on("click", "#subsidiarymaster", function (e) {

		window.location.href = "/SubsidiaryMaster/AddSubsidiaryMaster";
	});


	var selectedButton = null;

	$(document).on("click", "#getDataButton", function (e) {
		e.preventDefault(); // Prevent default action to ensure AJAX works properly if it's a form button.

		//sessionStorage.clear();

		// Get the current row where the button is located
		var $row = $(this).closest('tr'); // Assuming the button is inside a table row

		// Get the row's itemid (or you can get any other attribute related to the row)
		const subsidiaryId = $row.attr('itemid'); // Get the itemid attribute of the row

		// If no subsidiaryId is found, show an error
		if (!subsidiaryId) {
			//console.error("Subsidiary ID is missing!");
			showAlert("error", "No subsidiary ID found.");
			return;
		}

		if (subsidiaryId) {
			const url = `/SubsidiaryMaster/AddSubsidiaryMaster?subsidiary_Id=${subsidiaryId}`;

			//console.log("URL with Subsidiary ID:", url);

			// Redirect to the new URL
			window.location.href = url;
		}
		


		//// AJAX request to fetch subsidiary details
		//$.ajax({
		//	method: 'GET',
		//	url: `/SubsidiaryMaster/GetSubsidiaryById?subsidiaryId=${subsidiaryId}`, // Ensure correct endpoint
		//	contentType: 'application/json',
		//	dataType: "json",
		//	success: function (response) {
		//		console.log(response); // Log the full response
		//		if (response && response.data) {

		//			var subsidiaryDetails = response.data;

		//			// Extract only the "subsidiary_Id" from the object (or another field you need)
		//			var subsidiaryId = subsidiaryDetails.subsidiary_Id;

		//			// Create the query string with only the id
		//			const url = `/SubsidiaryMaster/AddSubsidiaryMaster?subsidiary_Id=${subsidiaryId}`;

		//			console.log("URL with Subsidiary ID:", url);

		//			// Redirect to the new URL
		//			window.location.href = url;

		//		} else {
		//			console.error("No valid data received from server.");
		//			showAlert("error", "No data found for the given Subsidiary ID.");
		//		}
		//	},
		//	error: function (xhr, status, error) {
		//		console.error("AJAX Error:", error);
		//		showAlert("error", "Error updating record.");
		//	}
		//});
	});

	$(document).on('click', '.btn-danger-light-icon[data-bs-target="#deleteSubsidiary"]', function () {
		selectedButton = $(this);
		//console.log('Delete button selected:', selectedButton);
	});


	var isRequestInProgress = false;

	$(document).on("click", "#confirmSubsidiaryDelete", function (e) {

		if (isRequestInProgress) return; // Prevent multiple clicks
		isRequestInProgress = true;

		e.preventDefault();  // Prevents any unexpected behavior

		var subsidiaryId = selectedButton.data('subsidiaryid');
		var rowId = `row-${subsidiaryId}`;

		if (!subsidiaryId) {
			showAlert("danger", "Subsidiary ID not found!");
			return;
		}

		const data = {
			Subsidiary_Id: subsidiaryId
		};

		$.ajax({
			type: "POST",  // Correct method for deleting
			url: `/SubsidiaryMaster/DeleteSubsidiaryMaster/${data.Subsidiary_Id}`,  // Pass the Subsidiary_Id as part of the URL
			contentType: "application/json",
			data: JSON.stringify(data),  // Send data in body (should contain the full object including Subsidiary_Id)
			success: function (response) {

				if (response.success) {					
					$(`#${rowId}`).fadeOut(500, function () {
						$(this).remove(); // Remove the row from DOM after fadeOut
					});					
					showAlert("success", response.message);
					//location.reload();
				} else {
					showAlert("error", response.message);
				}
				$('#deleteSubsidiary').modal('hide'); // Hide the modal after success
			},
			error: function (xhr, status, error) {
				showAlert("error", "Error deleting record.");  // Show error alert in case of failure
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

$(document).ready(function () {
	//var userPermissions = JSON.parse($("#userPermissionsData").val()); // Convert to object
	
	//if (!userPermissions.grantAdd) $("#addSubsidiaryButton").hide();

	//if (!userPermissions.grantEdit) $(".btn-edit-hide").hide();
	//if (!userPermissions.grantDelete) $(".btn-delete-hide").hide();

});
