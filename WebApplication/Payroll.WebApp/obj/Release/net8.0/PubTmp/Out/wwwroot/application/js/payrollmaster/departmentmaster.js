$(document).ready(function () {
    // Open Add Department Form
    $('#addDepartmentButton').click(function () {
        loadDepartmentForm(0); // Load empty form for adding a new department
    });

    // Open Edit Department Form
    window.openDepartmentEditButton = function (departmentId) {
        loadDepartmentForm(departmentId);
    };

    // Reset Form
    $('#resetDepartmentButton').click(function () {
        $('#departmentForm')[0].reset();
        $('#departmentId').val(0);
        $('#depttoggleContainer').hide(); // Hide toggle on reset
    });

    // Save or Update Department
    window.saveDepartmentButton = function () {
        var isValid = true;

        var departmentName = $('#departmentName').val().trim();
        var departmentCode = $('#departmentCode').val().trim();

        // Clear previous error messages
        $('.input_error_msg').text('');

        // Validate Department Name
        if (!departmentName) {
            $('#departmentName-error').text('Please enter the department name.');
            isValid = false;
        } else if (departmentName.length < 3) {
            $('#departmentName-error').text('Department Name must be at least 3 characters.');
            isValid = false;
        }

        // Validate Department Code
        if (!departmentCode) {
            $('#departmentCode-error').text('Please enter the department code.');
            isValid = false;
        } else if (departmentCode.length < 2) {
            $('#departmentCode-error').text('Department Code must be at least 2 characters.');
            isValid = false;
        }

        if (!isValid) return; // Stop submission if validation fails

        var departmentMasterDTO = {
            Department_Id: $('#departmentId').val() || 0,
            DepartmentName: departmentName,
            DepartmentCode: departmentCode,
            IsActive: $('#departmentActiveToggle').prop('checked'),
        };

        $.ajax({
            url: '/PayrollMaster/SaveOrUpdateDepartment',
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(departmentMasterDTO),
            success: function (response) {
                if (response.success) {
                    showAlert("success", response.message);
                    $('#addDepartment').offcanvas('hide');
                    fetchDepartmentList();
                } else {
                    showAlert("danger", response.message);
                    $('#addDepartment').offcanvas('hide');
                }
            },
            error: function (error) {
                if (error.status === 401) {
                    window.location.href = "/Account/LoginPage"; // Redirect if session expired
                } else {
                    showAlert("danger", "An error occurred while saving the department: " + xhr.responseText);
                }
            }
        });
    };


    // Load Add/Edit Department Form
    function loadDepartmentForm(departmentId) {
        $.ajax({
            url: '/PayrollMaster/GetDepartmentDetailsById/' + departmentId,
            type: 'GET',
            success: function (response) {
                $('#editdepartmentContainer').html(response);
                $('#addDepartment').offcanvas('show'); // Open the form
                //var offcanvasElement = document.getElementById('addDepartment');
                //            var myOffcanvas = new bootstrap.Offcanvas(offcanvasElement);
                //myOffcanvas.show();
                // Update form title based on Add or Edit mode
                if (departmentId === 0) {
                    $('#deptformTitle').text('Add Department Details');
                    $('#departmentCode').prop('disabled', false).removeClass('disabled-input'); // Enable departmentCode
                    $('#depttoggleContainer').hide(); // Hide toggle for Add mode
                } else {
                    $('#deptformTitle').text('Update Department Details');
                    $('#departmentCode').prop('disabled', true).addClass('disabled-input'); // Disable departmentCode
                    $('#depttoggleContainer').show(); // Ensure toggle is visible in Edit mode
                    // Set the checkbox state based on the fetched data
                    if (response.isactive) {
                        $('#departmentactivetoggle').prop('checked', true);
                    } else {
                        $('#departmentactivetoggle').prop('checked', false);
                    }
                }

                // Ensure toggle and label update properly
                // Ensure toggle and label update properly
                if ($('#departmentActiveToggle').length > 0) {
                    updateStatusLabel(); // Immediately update status label
                    $('#departmentActiveToggle').off('change').on('change', function () {
                        updateStatusLabel();
                    });
                }

            },
            error: function (error) {
                if (error.status === 401) {
                    window.location.href = "/Account/LoginPage"; // Redirect if session expired
                } else {
                    showAlert("danger", "An error occurred: " + (error.statusText || "Please try again."));
                }
            },
        });
    }

    // Update Status Label based on the toggle state
    function updateStatusLabel() {
        if ($('#departmentActiveToggle').prop('checked')) {
            $('#deptactiveStatusLabel').text('Active');
        } else {
            $('#deptactiveStatusLabel').text('Inactive');
        }
    }
    // Fetch and Reload Department List
    function fetchDepartmentList() {
        $.ajax({
            url: '/PayrollMaster/FetchDepartmentList',
            type: 'GET',
            success: function (response) {
                let tableId = "department-master-list";
                let table = $('#' + tableId);
                if ($.fn.DataTable.isDataTable(table)) {
                    table.DataTable().destroy(); // Destroy existing instance before updating
                }
                $('#department-master-list tbody').html($(response).find('tbody').html());
                makeDataTable(tableId); // Reinitialize DataTable
            },
            error: function (error) {
                if (error.status === 401) {
                    window.location.href = "/Account/LoginPage"; // Redirect if session expired
                } else {
                    showAlert("danger", "An error occurred: " + (error.statusText || "Please try again."));
                }
            },
        });
    }

    var selectedButton = null;
    var isRequestInProgress = false;

    // Capture the clicked button and store the associated data
    $(document).on('click', '.btn-danger-light-icon[data-bs-target="#deleteDepartment"]', function () {
        selectedButton = $(this);
    });

    // $('#confirmDepartmentDelete').on('click', function () {
    $(document).on('click', '#confirmDepartmentDelete', function () {
        if (isRequestInProgress) return; // Prevent multiple clicks
        isRequestInProgress = true;

        if (!selectedButton) {
            isRequestInProgress = false;
            return;
        }

        var departmentId = selectedButton.data('department-id');

        if (!departmentId) {
            console.error("Error: Department ID is undefined.");
            showAlert("danger", "Invalid department ID.");
            isRequestInProgress = false;
            return;
        }

        var rowId = `row-${departmentId}`; // Construct the row ID

        var rowData = { Department_Id: departmentId };

        $.ajax({
            url: '/PayrollMaster/DeleteDepartment', // Ensure this URL matches your backend route
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify(rowData),
            success: function (response) {
                if (response && response.success) {
                    // Hide the row
                    $(`#${rowId}`).fadeOut(500, function () {
                        $(this).remove();
                    });
                    showAlert("success", response.message);
                    fetchDepartmentList();
                } else {
                    showAlert("danger", response.message || "Failed to delete department. Please try again.");
                }
                $('#deleteDepartment').modal('hide'); // Close modal after response
            },
            error: function (error) {
                if (error.status === 401) {
                    window.location.href = "/Account/LoginPage"; // Redirect if session expired
                } else {
                    showAlert("danger", "An error occurred: " + (error.statusText || "Please try again."));
                }
                $('#deleteDepartment').modal('hide');
            },
            complete: function () {
                isRequestInProgress = false;
                // **Forcefully remove lingering modal backdrop**
                setTimeout(() => {
                    $('body').removeClass('modal-open');
                    $('.modal-backdrop').remove();
                }, 300);
            }
        });
    });
});


