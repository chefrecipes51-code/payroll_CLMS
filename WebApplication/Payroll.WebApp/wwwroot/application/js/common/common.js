//document.addEventListener('contextmenu', function (e) {
//    e.preventDefault();
//});

//document.addEventListener('keydown', function (e) {
//    if (e.key === 'F12' ||
//        (e.ctrlKey && e.shiftKey && e.key === 'I'.charCodeAt(0)) ||
//        (e.ctrlKey && e.key === 'U'.charCodeAt(0))) {
//        e.preventDefault();
//    }
//});

// Automatically redirect to logout after 30 minutes of inactivity
let idleTimeout = 30 * 60 * 1000; // 3,00,000 milliseconds means 5 min
let timeout;

function resetTimer() {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
        //window.location.href = '/Account/Logout'; // Rohit code commented By Harshida 12-02-25
        timeout = setTimeout(handleLogout, idleTimeout);
    }, idleTimeout);
}

// Reset the timer on user activity
window.onload = resetTimer;
document.onmousemove = resetTimer;
document.onkeydown = resetTimer;
function handleLogout() {
    fetch('/Account/Logout', { method: 'GET' })
        .then(response => response.json()) // Parse JSON response
        .then(data => {
            if (data.success) {
                sessionStorage.clear();
                sessionStorage.setItem('isLoggedOut', 'true');
                localStorage.clear();
               // console.log("Redirecting to:", data); // Ensure correct URL

                // Check the value of `data.from`
                if (data.from === 1) {
                   // console.log("Redirecting to CLMSLandingPage");
                    //console.log("Redirecting to:", data.redirectUrl); 
                    window.location.href = data.redirectUrl; // Redirect to CLMSLandingPage
                }
                if (data.from === 2) {
                   // console.log("Redirecting to LoginPage");
                   // console.log("Redirecting to:", data.redirectUrl); 
                    window.location.href = data.redirectUrl; // Redirect to LoginPage
                }
            }
        })
        .catch(error => {
            console.error('Logout failed:', error);
        });
}

//PAYROLL-494 Mechanism to grant page level rights, hide resources based on permission.
//Chirag gurjar 4 mar 25
$(document).ready(function () {
    var userPermissionsData = $("#userPermissionsData").val();
    
    if (userPermissionsData && userPermissionsData.trim() !== "" && userPermissionsData !== "null") {  // Check if element has a value
        try {
            var userPermissions = JSON.parse($("#userPermissionsData").val()); // Convert to object

            if (!userPermissions.grantAdd) $(".btn-add-hide").hide();
            if (!userPermissions.grantEdit) $(".btn-edit-hide").hide();
            if (!userPermissions.grantDelete) $(".btn-delete-hide").hide();
            if (!userPermissions.grantView) $(".btn-view-hide").hide();
            
        }
        catch (error) {
         console.error("Error parsing userPermissionsData:", error);
    }
}

    //if (!userPermissions.grantEdit) $(".redirectFromCompanyList").hide();
    // if (!userPermissions.grantAdd) $("#saveCompanyConfigurationButton").hide();
    //if (!userPermissions.grantAdd) $("#addCompanyCorrespondanceButton").hide();
   // if (!userPermissions.grantAdd) $("#saveCompanyDemographicDetailsButton").hide();

});


// Rohit Tiwari Note :- Wrapper function for AJAX calls to simplify the process of writing and calling AJAX.
//                      I am not use or optimize this code because I am leave the orgnaization.
//function ajaxRequest(url, data, successCallback, errorCallback) {
//    $.ajax({
//        type: 'POST',
//        url: url,
//        data: JSON.stringify(data),
//        contentType: 'application/json; charset=utf-8',
//        dataType: 'json',
//        success: function (response) {
//            if (successCallback && typeof successCallback === 'function') {
//                successCallback(response);
//            }
//        },
//        error: function (xhr, status, error) {
//            if (errorCallback && typeof errorCallback === 'function') {
//                errorCallback(xhr, status, error);
//            } else {
//                console.error('An error occurred:', error);
//            }
//        }
//    });
//}


