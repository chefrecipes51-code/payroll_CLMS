// when input_error_msg show spacing related js
document.querySelectorAll('.form-group, .form-check, .form-switch').forEach(group => {
    if (group.querySelector('.input_error_msg')) {
        group.classList.add('has-error');
    }
});