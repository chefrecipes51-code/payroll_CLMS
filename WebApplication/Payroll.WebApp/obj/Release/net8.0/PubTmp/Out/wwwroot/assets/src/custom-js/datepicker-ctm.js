$(function () {
    $('.datepicker').datepicker();

    $('.btn-datepicker').on('click', function () {
        $(this).siblings('.datepicker').datepicker('show');
    });
});