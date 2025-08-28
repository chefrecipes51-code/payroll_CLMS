//Dropdown list binding and event
   

    loadDropdown('#salaryBasicDropdown', '/DropDown/FetchSalaryBasicsDropdown');
loadDropdown('#componentValueTypeDropdown', '/DropDown/FetchComponentValueTypeDropdown');
loadDropdown('#salaryFrequencyDropdown', '/DropDown/FetchSalaryFrequencyDropdown');
loadDropdown('#payrollHeadDropdown', '/DropDown/FetchPayrollHeadsDropdown');
loadDropdown('#formulaDropdown', '/DropDown/FetchFormulaTypeDropdown');


// Add a delay before calling loadPayGradeDropdown
setTimeout(function () {
    loadPayGradeDropdown('#payGradeDropdown', 1); 
}, 500); // 500ms delay


// Function to populate dropdown from URL
function loadDropdown(selector, url) {
    $.ajax({
        url: url,
        type: 'GET',
        success: function (data) {
            const $dropdown = $(selector);
            $dropdown.empty().append($('<option>', {
                value: '',
                text: '-- Select --'
            }));

            $.each(data, function (i, item) {
                $dropdown.append($('<option>', {
                    value: item.value,
                    text: item.text
                }));
            });

            const selectedValue = $dropdown.data("selected-value");
            $dropdown.val(selectedValue);

            $dropdown.trigger('change'); // for select2 or any events
        },
        error: function () {
            console.error('Failed to load dropdown data for:', selector);
        }
    });
}

function loadPayGradeDropdown(targetDropdown, companyId) {

    $.ajax({
        url: `/DropDown/FetchActivePayGradeTypeDropdown`,
        method: 'GET',
        data: {
            companyId: companyId
        },
        success: function (data) {
            debugger;
            let dropdown = $(targetDropdown);
            dropdown.empty();
            dropdown.append('<option value="">Select Pay Grade</option>');

            $.each(data, function (index, item) {
                dropdown.append(`<option value="${item.value}">${item.text}</option>`);
            });

            const selectedValue = dropdown.data("selected-value");
            dropdown.val(selectedValue);
            console.log(dropdown.data("selected-value"));

            loadPayComponents('#payComponentDropdown');
        },
        error: function (xhr, status, error) {
            console.error("Failed to load PayGrade dropdown:", error);
            showAlert('danger', "An error occurred while loading the PayGrade dropdown.");
        }
    });
}

function loadPayComponents(targetDropdown) {
    debugger;
    $.ajax({
        url: '/DropDown/FetchIsParentPaycomponentDropdown',
        method: 'GET',

        success: function (data) {
            debugger;
            const dropdown = $(targetDropdown);
            dropdown.empty(); // Clear existing options
            dropdown.append('<option value="">Select Pay Component</option>'); // Add default option

            data.forEach(item => {
                dropdown.append(`<option value="${item.value}">${item.text}</option>`);
            });

        },
        error: function (xhr, status, error) {
            debugger;
            console.error("Failed to load Component dropdown:", error);
            showAlert('danger', "An error occurred while loading the Component dropdown.");
        }
    });
}

function loadPayComponentsChild(targetDropdown, selectType, selectedId, callback) {
    debugger;
    $.ajax({
        url: `/DropDown/FetchPaycomponentChildDropdown?selectType=${encodeURIComponent(selectType)}&EarningDeduction_Id=${encodeURIComponent(selectedId)}`,
        method: 'GET',

        success: function (data) {
            debugger;
            const dropdown = $(targetDropdown);
            dropdown.empty(); // Clear existing options
            dropdown.append('<option value="">Select Pay Component</option>'); // Add default option

            data.forEach(item => {
                dropdown.append(`<option value="${item.value}">${item.text}</option>`);
            });
            if (callback) callback();
        },
        error: function (xhr, status, error) {
            debugger;
            console.error("Failed to load Component dropdown:", error);
            showAlert('danger', "An error occurred while loading the Component dropdown.");
        }
    });
}


let isProgrammaticChange = false;
function editDetail(index) {

    $('#addMoreDetails').hide();
    $('#saveDetails').show();


    debugger;
    // Get the selected detail from the list
    const detail = salaryStructureDetailsList[index];

    if (detail) {
        // Populate the form fields in the 'salary-structure-card' div
        isProgrammaticChange = true;


        $("#payComponentDropdown").val(detail.earningDeductionID).trigger('change');

        // Use the callback to set child dropdown value after AJAX completes
        loadPayComponentsChild('#paySubComponentDropdown', 'H', detail.earningDeductionID, function () {
            $("#paySubComponentDropdown").val(detail.subEarningDeductionID).trigger('change');
        });

        $("#componentValueTypeDropdown").val(detail.calculationType).trigger('change');
        $("#taxableDropdown").val(detail.iStaxable ? "1" : "0").trigger('change');
        debugger;

        $("#payrollHeadDropdown").val(detail.earningDeductionType).trigger('change');
        //$("#paycomponentname").val(detail.earningDeductionValue || "");
        debugger;
        $("#amountval").val(isNaN(parseFloat(detail.earningDeductionValue)) ? 0 : parseFloat(detail.earningDeductionValue));
        $("#formulaDropdown").val(detail.formula_ID).trigger('change');

        $("#remarks").val(detail.remarks || "");

        // Store the index of the row being edited
        $("#saveDetails").data("edit-index", index);

        setTimeout(() => {
            isProgrammaticChange = false;
        }, 1000);

        // loadPayComponents('#payComponentDropdown');

        // Scroll to the component input section
        document.getElementById("componentSection").scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        });

    } else {
        showAlert('danger', "Failed to load the selected detail for editing.");
    }
}

$(document).ready(function () {

    $('.select2_search_ctm').select2({
        placeholder: function () {
            return $(this).data('placeholder'); // Set dynamic placeholder from data-placeholder attribute
        },
        allowClear: true,  // Allows clearing the selection (if needed)
        multiple: false,   // Ensure it's a single select dropdown
        dropdownAutoWidth: true,  // Auto adjust dropdown width
        width: '100%'      // Ensures the dropdown takes up full width of its container
    });

    renderSalaryStructureTable();

    // Attach change event to payGradeDropdown
    $('#payGradeDropdown').on('change', function () {
        const selectedId = $(this).val(); // Get the selected value (ID)

        if (selectedId) {
            // Make an AJAX call to fetch data based on the selected ID
            $.ajax({
                url: `/PayConfiguration/GetPayGradeDetailsById/${selectedId}`, // API endpoint
                method: 'GET',
                success: function (response) {
                    if (response.success) {
                        let data = response.data;
                        console.log(data);
                        debugger;
                        // Set minSalary and maxSalary values
                        $('#minsalary').val(parseFloat(data.minSalary).toFixed(0));
                        $('#maxsalary').val(parseFloat(data.maxSalary).toFixed(0));

                        // Calculate the difference and set it in the salaryRange text box
                        const salaryRange = data.maxSalary + '-' + data.minSalary;
                        $('#salaryRange').val(salaryRange);
                    } else {
                        // alert('No data found for the selected Pay Grade.');
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Error fetching Pay Grade details:', error);
                    showAlert('danger', 'An error occurred while fetching Pay Grade details.');
                }
            });
        } else {
            // Clear the fields if no value is selected
            $('#minsalary').val('');
            $('#maxsalary').val('');
            $('#salaryRange').val('');
        }
    });


   


    // Attach change event to payComponentDropdown
    $('#payComponentDropdown').on('change', function () {
        debugger;

        const selectedId = $(this).val(); // Get the selected value (ID)
        loadPayComponentsChild('#paySubComponentDropdown', 'H', selectedId);

        // Add a delay before continuing
        setTimeout(function () {
            if (!isProgrammaticChange) {
                debugger;
                if (selectedId) {
                    // Make an AJAX call to fetch data based on the selected ID
                    $.ajax({
                        url: `/PayConfiguration/GetPayComponentDetailsById/${selectedId}`, // API endpoint
                        method: 'GET',
                        success: function (response) {
                            if (response.success) {
                                let data = response.data;
                                // Bind the fetched data to the respective fields
                                // $('#salaryFrequencyDropdown').val(data.salaryFrequency).trigger('change'); // Set value and trigger change event
                                debugger;
                                if (data.calculationType === 3) {
                                    $('#formulaSection').show();
                                    $('#amountSection').hide();
                                    // $('#formulaSectionbutton').show();
                                    // loadDropdownWithSelectedValue('#formulaDropdown', '/DropDown/FetchFormulaTypeDropdown', function () {
                                    //     setSelectedValueInDropdown('#formulaDropdown', data.formula_Id);
                                    // });
                                }
                                else if (data.calculationType === 2) {
                                    $('#formulaSection').hide();
                                    $('#amountSection').show();
                                }
                                //////////////////////////For Formula Check:- End
                                $('#payrollHeadDropdown').val(data.earningDeductionType).trigger('change');
                                $('#amoutval').text(data.Formula || ''); // Set text for dropdownTextbox
                                $('#paycomponentname').val(data.earningDeductionName || ''); // Set value for paycomponentname
                                $('#payrollHeadDropdown').val(data.earningDeductionType).trigger('change');
                                $("#taxableDropdown").val(data.taxable).trigger('change');
                                $("#calculationType").val(data.calculationType).trigger('change');
                                $("#formulaDropdown").val(data.formula_Id).trigger('change');
                                $('#remarks').val(data.remarks || ''); // Set value for remarks
                            } else {
                                debugger;
                               // showAlert('danger', 'No data found for the selected Pay Component.');
                            }
                        },
                        error: function (xhr, status, error) {
                            console.error('Error fetching Pay Component details:', error);
                            showAlert('danger', 'An error occurred while fetching Pay Component details.');
                        }
                    });
                } else {
                    // Clear the fields if no value is selected
                    // $('#salaryFrequencyDropdown').val('').trigger('change');
                    $('#dropdownTextbox').text('');
                    $('#paycomponentname').val('');
                    $('#amountval').val('');
                    $('#remarks').val('');
                }
            }
        }, 999); // 500ms delay before continuing
    });

    $('#componentValueTypeDropdown').on('change', function () {
        const selectedValue = $(this).val();
        if (selectedValue === "3") {
            $('#formulaSection').show();
            $('#amountSection').hide();
            $('#amountval').val('');
        } else {
            $('#formulaSection').hide();
            $('#amountSection').show();
            $("#formulaDropdown").val("").trigger("change");
        }
    });
});
   