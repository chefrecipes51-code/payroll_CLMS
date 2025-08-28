$(document).ready(function () {
    var originalPayGradeData = null; // 🔥 Keep original edit data
    var isEditMode = false;
    // Clear errors as user types or selects
    $('#paygradecode, #paygradename, #description').on('input', function () {
        const fieldId = $(this).attr('id');
        $(`#${fieldId}-error`).text('');
    });

    $('#minsalary, #maxsalary').on('input', function () {
        var val = $(this).val();
        // Allow only valid decimal input
        val = val.replace(/[^0-9.]/g, '')
            .replace(/(\..*)\./g, '$1')
            .replace(/^(\d*\.\d{0,2}).*$/, '$1');
        $(this).val(val);

        const fieldId = $(this).attr('id');
        const value = val.trim();
        if (!value || /^\d+(\.\d{1,2})?$/.test(value)) {
            $(`#${fieldId}-error`).text('');
        }
    });

    // Format salary fields on blur
    $('#minsalary, #maxsalary').on('blur', function () {
        const val = parseFloat($(this).val());
        if (!isNaN(val)) {
            $(this).val(val.toFixed(2));
        }
    });
    function validatePayGradeForm() {
        var isValid = true;
        $(".input_error_msg").text("");

        const paygradecode = $("#paygradecode").val().trim();
        const paygradename = $("#paygradename").val().trim();
        const minsalary = $("#minsalary").val().trim();
        const maxsalary = $("#maxsalary").val().trim();
        const description = $("#description").val().trim();

        const decimalRegex = /^\d+(\.\d{1,2})?$/;

        if (!paygradecode) {
            $("#paygradecode-error").text("Please enter pay grade code.");
            isValid = false;
        }

        if (!paygradename) {
            $("#paygradename-error").text("Please enter pay grade name.");
            isValid = false;
        } else {
            const error = validateTextField(paygradename);
            if (error) {
                $('#paygradename-error').text(error);
                isValid = false;
            }
        }

        if (!minsalary) {
            $("#minsalary-error").text("Please enter minimum salary.");
            isValid = false;
        } else if (!decimalRegex.test(minsalary) || parseFloat(minsalary) < 0) {
            $("#minsalary-error").text("Minimum salary must be a valid non-negative decimal number.");
            isValid = false;
        }

        if (!maxsalary) {
            $("#maxsalary-error").text("Please enter maximum salary.");
            isValid = false;
        } else if (!decimalRegex.test(maxsalary) || parseFloat(maxsalary) < 0) {
            $("#maxsalary-error").text("Maximum salary must be a valid non-negative decimal number.");
            isValid = false;
        }

        const min = parseFloat(minsalary);
        const max = parseFloat(maxsalary);

        if (!isNaN(min) && !isNaN(max) && min > max) {
            $("#maxsalary-error").text("Maximum salary must be greater than or equal to minimum salary.");
            isValid = false;
        }
        return isValid;
    }
    // Save handler
    $(document).on('click', '#btnSavePayGrade', function (e) {
        e.preventDefault();

        if (!validatePayGradeForm()) return;
        const editId = $('#payGradeForm').attr('data-edit-id');  // 🔥 Corrected here
        const isEdit = editId !== undefined && editId !== '';
        // Capitalize the first varter of each word in the input value
        //var payGradeName = $('#paygradename').val().trim().split(' ').map(function (word) {
        //    return word.charAt(0).toUpperCase() + word.slice(1).toLowerCase();
        //}).join(' ');
        const dto = {
            payGradeCode: $("#paygradecode").val().trim(),
            //payGradeName: payGradeName,
            payGradeName: $("#paygradename").val().trim(),
            minSalary: parseFloat($("#minsalary").val()) || 0,
            maxSalary: parseFloat($("#maxsalary").val()) || 0,
            PayGradeDesc: $("#description").val().trim(),
            isActive: $('#payGradeActiveToggle').prop('checked'),
            Cmp_Id: 0,
            PayGrade_Id: isEdit ? editId : 0
        };

        const url = isEdit ? '/PayConfiguration/UpdatePayGrades' : '/PayConfiguration/AddPayGrades';

        $.ajax({
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(dto),
            success: function (response) {
                if (response.success) {
                    $('#paygradeadd').modal('hide');
                    setTimeout(function () {
                        showAlert("success", response.message);
                    }, 1000);
                    LoadPayGrade();
                    //window.location.href = "/PayConfiguration/PayGrade";
                } else {
                    showAlert("danger", response.message);
                }
            },
            error: function () {
                toastr.error("Something went wrong while saving the data.");
            }
        });
    });

    // Reset form
    $(document).on('click', '#btnResetPayGrade', function () {
        if (isEditMode && originalPayGradeData) {
            // If editing -> Reset to original data
            setPayGradeForm(originalPayGradeData);
        } else {
            // If adding -> Clear all fields
            clearPayGradeForm();
        }
    });
    $(document).on('click', '#editpayGrade', function () {
        var id = $(this).data('id');
        $.ajax({
            url: `/PayConfiguration/GetPayGradeDetailsById/${id}`,
            type: 'GET',
            success: function (response) {
                if (response.success) {
                    var data = response.data;
                    // Save Original Data
                    originalPayGradeData = data;
                    isEditMode = true;
                    setPayGradeForm(data);

                    // Update the hidden field to know it's an edit operation
                    // Set Edit ID properly
                    $('#payGradeForm').attr('data-edit-id', data.payGrade_Id);

                    // Change button text
                    $('#btnSavePayGrade').text('Update');
                    $('#formTitle').text('Edit Pay Grade'); // change title

                    // **OPEN MODAL**
                    $('#paygradeadd').modal('show');
                } else {
                    showAlert('danger', response.message);
                }
            },
            error: function () {
                toastr.error("Error fetching data for edit.");
            }
        });
    });
    $('#payGradeActiveToggle').on('change', function () {
        $('#activeStatusLabel').text(this.checked ? 'Active' : 'Inactive');
    });
    // Set Form with Data
    function setPayGradeForm(data) {
        // Reset validation messages
        $(".input_error_msg").text('');
        $('#paygradecode').val(data.payGradeCode);
        $('#paygradename').val(data.payGradeName);
        $('#minsalary').val(parseFloat(data.minSalary).toFixed(2));
        $('#maxsalary').val(parseFloat(data.maxSalary).toFixed(2));
        $('#description').val(data.payGradeDesc);
        $('#payGradeActiveToggle').prop('checked', data.isActive ?? false);
        $('#activeStatusLabel').text(data.isActive ? 'Active' : 'Inactive');
        // Show the toggle container only in edit mode
        if (isEditMode) {
            $('#togglePayGradeContainer').show();  // Show the container in edit mode
        } else {
            $('#togglePayGradeContainer').hide();  // Hide the container in add mode
        }
    }

    // Clear Form
    function clearPayGradeForm() {
        $('#paygradecode').val('');
        $('#paygradename').val('');
        $('#minsalary').val('');
        $('#maxsalary').val('');
        $('#description').val('');
        $('#payGradeActiveToggle').prop('checked', false);
        $('#activeStatusLabel').text('Inactive');
        $(".input_error_msg").text('');
    }

    $(document).on('click', '#btnAddPayGrade', function () {
        isEditMode = false;
        originalPayGradeData = null;
        clearPayGradeForm();
        $('#payGradeForm').removeAttr('data-edit-id');
        $('#btnSavePayGrade').text('Save');
        $('#formTitle').text('Add Pay Grade');
        $('#togglePayGradeContainer').hide();  // Hide the status toggle in add mode
        $('#paygradeadd').modal('show');
    });

});

$(document).ready(function () {
    LoadPayGrade();
});

function LoadPayGrade() {
    $.ajax({
        url: '/PayConfiguration/PayGradeList',
        type: 'GET',
        success: function (result) {
            $('#payGradeContainer').html(result.html);
            $('#payGradeCount').text(result.count);
            var tableId = "pay-grade-list";
            if ($.fn.DataTable && $.fn.dataTable.isDataTable(`#${tableId}`)) {
                $(`#${tableId}`).DataTable().destroy();
            }
            if (result.count > 0) {
                makeDataTable(tableId);
            } else {
                // Initialize the variable before using it
                var $table = $(`#${tableId}`);
                // Manually add "no data" message if no rows
                $table.find('tbody').html(
                    '<tr><td colspan="9" class="text-center">No data available.</td></tr>'
                );
            }
        },
        error: function () {
            $('#payGradeContainer').html('<div class="text-danger text-center">Failed to load data.</div>');
        }
    });
}


var selectedButton = null;
var isRequestInProgress = false;

$(document).on('click', '.btn-danger-light-icon[data-bs-target="#deletePayGrade"]', function () {
    selectedButton = $(this);
});
//$('#confirmAreaDevare').on('click', function () {
$(document).on('click', '#confirmPayGradeDelete', function () {
    if (isRequestInProgress) return; // Prevent multiple requests
    isRequestInProgress = true;

    if (!selectedButton) {
        console.error("Error: No button was selected.");
        isRequestInProgress = false;
        return;
    }

    var payGradeId = selectedButton.data('paygrade-id');

    if (!payGradeId) {
        showAlert("danger", "Invalid ID.");
        isRequestInProgress = false;
        return;
    }

    var rowId = `row-${payGradeId}`; // Construct the row ID

    var rowData = {
        PayGrade_Id: payGradeId,
        Cmp_Id: 0,
        PayGradeCode: "", // Ensure proper integer or NaN
        PayGradeName: "", // Ensure proper integer or NaN
        MinSalary: 0.00,
        MaxSalary: 0.00,
        PayGradeDesc: "",
    };

    $.ajax({
        url: '/PayConfiguration/DeletePayGrade',
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
                LoadPayGrade();
            } else {
                showAlert("danger", response.message || "Failed to delete grade. Please try again.");
            }
            $('#deletePayGrade').modal('hide');
        },
        error: function () {
            showAlert("danger", "An error occurred. Please try again.");
            $('#deletePayGrade').modal('hide');
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