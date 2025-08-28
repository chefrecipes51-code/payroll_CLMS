/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 07-02-'25
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>


//////////////////////////////////////////// REGION FOR FETCH COMPANY BASIC DETAILS START///////////////////////////////////////////////
/// <summary>
/// Note:- Bind Company type
/// </summary>
function loadCompanyTypes() {
    $.ajax({
        url: "/DropDown/FetchCompanyTypeDropdown",
        type: "GET",
        dataType: "json",
        success: function (response) {
            let dropdown = $("#CompanyType_ID");
            let selectedValue = dropdown.data("selected-value"); // Get value from data attribute
            //console.log("dropdown" + dropdown);
            dropdown.empty().append('<option value="">Select Company Type</option>');

            if (response.length > 0) {
                $.each(response, function (index, item) {
                    dropdown.append(`<option value="${item.value}">${item.text}</option>`);
                });

                if (selectedValue) {
                    dropdown.val(selectedValue).trigger("change"); // Select the model value
                }
            } else {
                //console.warn("No Company Types Available.");
            }
        },
        error: function (xhr, status, error) {
            showAlert("danger", "Error fetching company types:"+ error);           
        }
    });
}

/// <summary>
/// Note:- Bind Company If PArent Exist
/// </summary>
function checkIsParent() {
    let isChecked = $("#IsParent").prop("checked");
    let parentCompanyContainer = $("#parentCompanyContainer");
    parentCompanyContainer.show();
    loadParentCompanies();
    //if (isChecked) {
    //    parentCompanyContainer.show();
    //    loadParentCompanies(); // Fetch and bind companies when IsParent is true
    //} else {
    //    parentCompanyContainer.hide();
    //    $("#ParentCompany_Id").empty(); // Clear the dropdown
    //}
}
function loadParentCompanies() {
   
    var currentNotCompanyName = $("#CompanyName").val().trim(); 
    //console.log("currentNotCompanyName-currentNotCompanyName" + currentNotCompanyName);
    $.ajax({
        url: "/DropDown/FetchCompaniesDropdown",
        type: "GET",
        dataType: "json",
        success: function (responseParentCompanies) {
            var dropdownParentCompanies = $("#ParentCompany_Id");
            var selectedParentCompaniesValue = dropdownParentCompanies.data("selected-value");
            //console.log("Setting selected parent company:", selectedParentCompaniesValue);
            dropdownParentCompanies.empty().append('<option value="">Select Parent Company</option>');
            if (responseParentCompanies.length > 0) {
                $.each(responseParentCompanies, function (indexParentCompanies, itemParentCompanies) {
                    //console.log("Setting selected parent company:" + itemParentCompanies.value + "Text" + itemParentCompanies.text);
                    //dropdownParentCompanies.append(`<option value="${itemParentCompanies.value}">${itemParentCompanies.text}</option>`);
                    //console.log("itemParentCompanies.text.trim()"+itemParentCompanies.text.trim());
                    if (itemParentCompanies.text.trim() !== currentNotCompanyName) {
                        dropdownParentCompanies.append(`<option value="${itemParentCompanies.value}">${itemParentCompanies.text}</option>`);
                    }
                });
                if (selectedParentCompaniesValue) {
                    dropdownParentCompanies.val(selectedParentCompaniesValue).trigger("change");
                }
            } else {
                //console.warn("No Parent Companies Available.");
            }
        },
        error: function (xhr, status, error) {
            showAlert("danger", "Error fetching companies:" + error);      
           
        }
    });
}
$(document).ready(function () {
    loadCompanyTypes();
    checkIsParent(); 
    $("#IsParent").change(function () {
        checkIsParent();
    });
});


//////////////////////////////////////////// REGION FOR FETCH COMPANY BASIC DETAILS END///////////////////////////////////////////////

//////////////////////////////////////////// REGION FOR UPDATE COMPANY BASIC DETAILS START///////////////////////////////////////////////
function updateCompanyPartial() {
    var companyDemographicId = $("#GlobalCompany_Id").val();
    //alert("UpdatecompanyDemographicId" + companyDemographicId);
    $.ajax({
        url: '/Company/GetCompanyDemographicDetailsPartialByCompanyId',
        type: 'GET',
        data: { companyDemographicId: companyDemographicId },
        success: function (html) {
            //console.log("Received HTML: " + html);
            $('#v-pills-home').html(html); 
            $.getScript("/application/js/companymaster/demographicdata.js").done(function () {
                loadCompanyTypes();
                loadParentCompanies();
            });
            //loadCompanyTypes();  
            //loadParentCompanies(); 
            //checkIsParent();    
           // $('#v-pills-home-tab').tab('show');
        },
        error: function (xhr, status, error) {
            showAlert("danger", "An error occurred while updating value:" + error);    
        }
    });
}
$(document).ready(function () {
    $("#saveCompanyDemographicDetailsButton").click(function () {
       // console.log("saveCompanyDemographicDetailsButton Clicked");
        if (!validateCompanyDetails()) {
            return; // Stop execution if validation fails
        }
        //debugger; //Added By Chirag
        var formDataDemographicDetails = {
            //Company_Id: $("#GlobalCompany_Id").val(),
            Company_Id: parseInt($("#GlobalCompany_Id").val()) || 0,
            CompanyName: $("#CompanyName").val(),
            Company_Code: $("#Company_Code").val(),
            CompanyShortName: $("#CompanyShortName").val(),
            CompanyPrintName: $("#CompanyPrintName").val(),
            Has_Subsidary: $("#Has_Subsidary").prop("checked"),
            IsParent: $("#IsParent").prop("checked"),
            //ParentCompany_Id: $("#ParentCompany_Id").val(),
            ParentCompany_Id: parseInt($("#ParentCompany_Id").val()) || 0, 
            //CompanyType_ID: $("#CompanyType_ID").val()
            CompanyType_ID: parseInt($("#CompanyType_ID").val()) || 0
        };
        //console.log(formDataDemographicDetails);
        $.ajax({
            url: "/Company/UpdateCompanyDemographicDetails",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formDataDemographicDetails),
            success: function (response) {
                //debugger; //Added By Chirag
                showAlert("success", response.message);
                //alert("Company details updated successfully!" + response.success);
                if (response.success) {
                    updateCompanyPartial();
                }
            },
            error: function (xhr, status, error) {
                //showAlert("danger", "Error updating company: " + xhr.responseText);
                //console.log("Error updating company: " + xhr.responseText);
            }
        });
    });

    $("#resetAllCompanyMaster").click(function () {
        var companyMasterResetId = $("#GlobalCompany_Id").val();
        //console.log("companyMasterResetId" + companyMasterResetId);
        if (companyMasterResetId) {
            $.ajax({
                url: '/Company/GetCompanyForResetRecord',
                type: 'GET',
                data: { companyDemographicId: companyMasterResetId },
                success: function (response) {
                    if (response.success) {
                        //console.log(response.data);
                        fillCompanyForm(response.data);                        
                    } 
                },
                error: function (xhr, status, error) {
                    showAlert("danger", "Error while resetting: " + error);
                }
            });
        } else {
            //resetCompanyMasterForm(); // If ID is empty, reset form
        }
    });
});
function fillCompanyForm(data) {
    $("#CompanyName").val(data.companyName);
    $("#Company_Code").val(data.company_Code);
    $("#CompanyShortName").val(data.companyShortName);
    $("#CompanyPrintName").val(data.companyPrintName);
    $("#CompanyType_ID").val(data.companyType_ID).trigger('change');
    $("#ParentCompany_Id").val(data.parentCompany_Id).trigger('change');
    $("#Has_Subsidary").prop('checked', data.has_Subsidary);
    $("#IsParent").prop('checked', data.isParent);

    if ((data.companyType_ID && data.companyType_ID > 0) && data.companyType_ID) {
        loadCompanyTypes();
    }
    if ((data.parentCompany_Id && data.parentCompany_Id > 0) && data.parentCompany_Id) {
        loadParentCompanies();
    }
}
function validateCompanyDetails() {
    var companyNameForvalidateCompanyDetails = $("#CompanyName").val().trim(); // Trim spaces from both ends
    var companyCodeForvalidateCompanyDetails = $("#Company_Code").val().trim();
    var companyShortNameForvalidateCompanyDetails = $("#CompanyShortName").val().trim();
    $("#CompanyName-error").text(""); // Clear previous errors
    $("#CompanyCode-error").text("");
    $("#CompanyShortName-error").text("");

    // Validate Company Name:- Start
    if (companyNameForvalidateCompanyDetails === "") {
        $("#CompanyName-error").text("Please provide company name.");
        return false;
    }

    if (/[^a-zA-Z0-9 ]/.test(companyNameForvalidateCompanyDetails)) {
        $("#CompanyName-error").text("Company name cannot contain special characters.");
        return false;
    }

    if (/\s{2,}/.test(companyNameForvalidateCompanyDetails)) {
        $("#CompanyName-error").text("Company name cannot have consecutive spaces.");
        return false;
    }

    if (companyNameForvalidateCompanyDetails.length > 200) {
        $("#CompanyName-error").text("Company name cannot exceed 200 characters.");
        return false;
    }
    // Validate Company Name:- End

    // Validate Company Code:- Start
    companyCodeForvalidateCompanyDetails = companyCodeForvalidateCompanyDetails.trim();
    //console.log("companyCodeForvalidateCompanyDetails", companyCodeForvalidateCompanyDetails);
    if (companyCodeForvalidateCompanyDetails === "") {
        $("#CompanyCode-error").text("Please provide company code.");
        return false;
    }
    if (/[^a-zA-Z0-9]/.test(companyCodeForvalidateCompanyDetails)) {
        $("#CompanyCode-error").text("Company code cannot contain special characters or spaces.");
        return false;
    }

    if (companyCodeForvalidateCompanyDetails.length > 6) {
        $("#CompanyCode-error").text("Company code cannot exceed 6 characters.");
        return false;
    }
    if (/^\s|\s$/.test(companyCodeForvalidateCompanyDetails)) {
        $("#CompanyCode-error").text("Company code cannot begin or end with a space.");
        return false;
    }
    //if (/\s{2,}/.test(companyCodeForvalidateCompanyDetails)) {
    //    $("#CompanyCode-error").text("Company Code cannot contain consecutive spaces.");
    //    return false;
    //}
    // Validate Company Code:- End

    // Validate Short Name :- Start
    if (companyShortNameForvalidateCompanyDetails === "") {
        $("#CompanyShortName-error").text("Please provide company short name.");
        return false;
    }
    if (/[^a-zA-Z0-9 ]/.test(companyShortNameForvalidateCompanyDetails)) {
        $("#CompanyShortName-error").text("Company short name cannot contain special characters.");
        return false;
    }
    if (companyShortNameForvalidateCompanyDetails.length > 20) {
        $("#CompanyShortName-error").text("Company short name cannot exceed 20 characters.");
        return false;
    }
    // Validate Short Name:- End

    return true;
}
//////////////////////////////////////////// REGION FOR UPDATE COMPANY BASIC DETAILS END///////////////////////////////////////////////

//////////////////////////////////////////// REGION FOR UPDATE COMPANY BASIC DETAILS END///////////////////////////////////////////////
