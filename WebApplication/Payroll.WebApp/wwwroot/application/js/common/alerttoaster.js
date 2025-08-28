 /**
 * Create By : Priyanshi Jain
 * Function to show alerts dynamically based on type
 * param {string} type - Type of alert ('success', 'danger', 'warning', 'primary')
 * param {string} message - Alert message to display
 */

function showAlert(type, message) {
    var alertTypes = {
        success: 'success-checked',
        danger: 'error-info',
        warning: 'warning-triangle',
        primary: 'primary-bell'
    };
    var iconClass = alertTypes[type] || 'primary-bell';
    var alertHtml =
        '<div class="alert alert-' + type + '" role="alert">' +
        '<div class="alert-content">' +
        '<div class="d-flex align-items-center gap-2">' +
        '<span class="sprite-icons ' + iconClass + '"></span>' +
        '<p class="lg-text">' + message + '</p>' +
        '</div>' +
        '<button class="btn btn_close_alert"><span class="sprite-icons close-icon"></span></button>' +
        '</div>' +
        '</div>';

    $(".alert-custom").html(alertHtml).fadeIn();

    // Automatically hide the alert after 5 seconds
    setTimeout(function () {
        $(".alert-custom").fadeOut();
    }, 5000);

    // Close the alert on button click
    $(".alert-custom").on("click", ".btn_close_alert", function () {
        $(".alert-custom").fadeOut();
    });
}

function showPopupAlert(type, message) {
    var alertTypes = {
        success: 'success-checked',
        danger: 'error-info',
        warning: 'warning-triangle',
        primary: 'primary-bell'
    };
    var iconClass = alertTypes[type] || 'primary-bell';
    var alertHtml =
        '<div class="alert alert-' + type + '" role="alert">' +
        '<div class="alert-content">' +
        '<div class="d-flex align-items-center gap-2">' +
        '<span class="sprite-icons ' + iconClass + '"></span>' +
        '<p class="lg-text mb-0">' + message + '</p>' +
        '</div>' +
        '<button class="btn btn_close_alert"><span class="sprite-icons close-icon"></span></button>' +
        '</div>' +
        '</div>';

    var $alertContainer = $(".offcanvas.show .popupalert-custom");

    if ($alertContainer.length) {
        $alertContainer.html(alertHtml).fadeIn();

        // Remove previous event handlers before adding a new one
        $alertContainer.off("click", ".btn_close_alert").on("click", ".btn_close_alert", function () {
            $alertContainer.fadeOut();
        });

        setTimeout(function () {
            $alertContainer.fadeOut();
        }, 3000);
    }
}
