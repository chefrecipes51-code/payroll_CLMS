/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 14-Feb-2025
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>

$(document).ready(function () {
    $("#v-pills-home-tab").addClass("active");
    $("#v-pills-home").addClass("show active");
    loadCompanyCurrencyID();
});
function getParameterByName(name) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.has(name) ? urlParams.get(name) : null;
}

////////////////////////////////////////// Note:- Bind Company Currency "START" ///////////////////////////////////////////////////

function loadCompanyCurrencyID() {
    var loadcompanyId = $("#GlobalCompany_Id").val();
    //var companyId = getParameterByName('companyId');
    //alert(loadcompanyId);
    $.ajax({
        url: '/Company/GetCompanyCurrencyId',
        type: 'GET',
        data: { companyId: loadcompanyId },
        success: function (response) {
            //console.log(response);
            if (response.success) {
                //console.log("Currency ID:", response.currency_ID);
                loadCompanyCurrencyList(response.currency_ID);
            } else {
                //console.log("Company not found.");
                loadCompanyCurrencyList("Currency ID"+null);
            }
        },
        error: function (xhr, status, error) {
            showAlert("danger", "Error updating value" + error);
            //console.error("AJAX Error: ", error);
            //console.error("Response Text: ", xhr.responseText);
            //alert("An error occurred: " + xhr.status + " - " + error);
        }
    });
}
function loadCompanyCurrencyList(selectedCurrencyId) {
    $.ajax({
        url: "/DropDown/FetchCurrencyDropdown",
        type: "GET",
        dataType: "json",
        success: function (response) {
            let dropdownForCompanyBsic = $("#CompanyCurrency_ID");
            //alert("Dropdown element:", dropdownForCompanyBsic);
            //alert("Dropdown HTML:", dropdownForCompanyBsic.html());
            dropdownForCompanyBsic.empty().append('<option value="">Select Company Currency</option>');
            //alert("dropdownForCompanyBsic" + dropdownForCompanyBsic);
          

            if (response.length > 0) {
                $.each(response, function (index, item) {
                    dropdownForCompanyBsic.append(`<option value="${item.value}">${item.text}</option>`);
                });

                if (selectedCurrencyId) {
                    dropdownForCompanyBsic.val(selectedCurrencyId).trigger("change"); // Select the fetched Currency_ID
                }
            } else {
                //console.warn("No Company Types Available.");
            }
        },
        error: function (xhr, status, error) {
            //alert("Currency ID" + error);
            //console.error("Error fetching company types:", error);
        }
    });
}

////////////////////////////////////////// Note:- Bind Company Currency "END" ///////////////////////////////////////////////////


////////////////////////////////////////////Company Configuration ADD:- Start////////////////////////////////////////////////////

$(document).ready(function () {
    $("#saveCompanyConfigurationButton").click(function () {
        $('#sdate, #edate').prop('disabled', false);
        var sdateValue = $("#sdate").val().split(" ")[0]; // Take only date part
        var edateValue = $("#edate").val().split(" ")[0]; // Take only date part
        var currencyIdValue = $("#CompanyCurrency_ID").val();       

        var formCompanyConfigurationData = {
            CompanyId: parseInt($("#GlobalCompany_Id").val()) || 0, // Ensure integer
            StartDate: sdateValue,
            EndDate: edateValue,
            CurrencyId: parseInt(currencyIdValue)
        };

        //console.log("Submitting Data:", formCompanyConfigurationData); // Debugging
        $.ajax({
            url: "/Company/UpdateCompanyConfiguration",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formCompanyConfigurationData),          
            success: function (response) {               
                if (response.success) {
                    $("#v-pills-financial #sdate").val(response.startDate);
                    $("#v-pills-financial #edate").val(response.endDate);
                    $('#v-pills-financial #sdate').prop('disabled', true);
                    $('#v-pills-financial #edate').prop('disabled', true);
                    showAlert("success", response.message);                   
                }
                else {                   
                    if (response.message === "Response:- Financial year already exists Configured scccessfully for others") {
                        $("#v-pills-financial #sdate").val(response.startDate);
                        $("#v-pills-financial #edate").val(response.endDate);
                        $('#v-pills-financial #sdate').prop('disabled', true);
                        $('#v-pills-financial #edate').prop('disabled', true);
                        showAlert("success", response.message);
                    }
                    else {
                        showAlert("danger", response.message);
                    }                    
                }
            },
            error: function (xhr, status, error) {
                showAlert("danger", "Error updating value" + xhr.responseText);
                //alert("Error updating company: " + );
            }
        });
    });
});
////////////////////////////////////////////Company Configuration ADD:- End////////////////////////////////////////////////////