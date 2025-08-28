$(document).ready(function () {
    $("#saveDetailsButton").click(function (e) {
        e.preventDefault();

        // Collecting form data
        var formData = {
            companyName: $("#CompanyName").val(),
            company_Code: $("#Company_Code").val(),
            companyShortName: $("#CompanyShortName").val(),
            companyPrintName: $("#CompanyPrintName").val(),
            companyCustomName: $("#ccname").val(),
            companyType: $("select[asp-for='CompanyType_ID']").val(),
            statutoryCompliance: $("select[placeholder='Statutory Compliance For']").val(),
            language: $("select[placeholder='Language']").val(),
            registeredAddress: {
                contactType: $("#addr[name='addr']").val(),
                name: $("#addr[name='name']").val(),
                email: $("#email[name='email']").val(),
                country: $("#phone1countries").val(),
                phoneNumber: $("#phone[name='phone']").val(),
                address: $("input[asp-for='CompanyCorrespondance.CompanyAddress']").val(),
                city: $("select[placeholder='City']").val(),
                pinCode: $("#pincode").val()
            },
            communicationAddress: {
                contactType: $("#addr[name='addr']").val(),
                name: $("#addr[name='name']").val(),
                email: $("#email[name='email']").val(),
                country: $("#phone2countries").val(),
                phoneNumber: $("#phone[name='phone']").val(),
                address: $("input[placeholder='Enter address']").val()
            },
            companyCorrespondance: {
                companyAddress: $("#CompanyAddress").val(),
                buildingNo: $("#buildingNo").val(),
                buildingName: $("#buildingName").val(),
                street: $("#street").val(),
                countryId: $("#countries").val(),
                stateId: $("select[name='State_ID']").val(),
                cityId: $("select[name='City_ID']").val(),
                locationId: $("select[name='Location_ID']").val(),
                Primary_Phone_no: $('#phone1countries option:selected').text() + '-' + $('#Primary_Phone_no').val(),
                Secondary_Phone_No: $('#phone2countries option:selected').text() + '-' + $("#Secondary_Phone_No").val(),
                Primary_Email_Id: $("#Primary_Email_Id").val(),
                Secondary_Email_ID: $("#Secondary_Email_ID").val(),
                websiteUrl: $("#websiteUrl").val(),
                companyLogoImagePath: $("#companyLogoImagePath").val()
            },
            companyStatutory: {
                statutoryTypeId: $("select[name='Statutory_Type_Id']").val(),
                statutoryTypeName: $("#statutoryTypeName").val()
            }
        };
        //console.log(formData);
        // AJAX call to save data
        $.ajax({
            url: "/Company/AddCompanyDetails", // Replace with your API endpoint
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                if (response.success != null && response.success != "") {
                    showAlert("success", response.message);
                } else {
                    showAlert("danger", response.message);
                }
            },
            error: function (xhr, status, error) {
                // Handle error
                alert("Error: " + xhr.responseText);
            }
        });
    });
});