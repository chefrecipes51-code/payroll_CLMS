$(document).ready(function () {
    $(document).on('readystatechange', function () {
        if (document.readyState === 'complete') {
            hideLoader(); // Hide loader and overlay when document is loaded
        }
    });
});

function showLoader() {
    $('#overlay').show(); // Show overlay and loader
    $('#loader').show();
}

function hideLoader() {
    $('#overlay').hide(); // Hide overlay and loader
    $('#loader').hide();
}
