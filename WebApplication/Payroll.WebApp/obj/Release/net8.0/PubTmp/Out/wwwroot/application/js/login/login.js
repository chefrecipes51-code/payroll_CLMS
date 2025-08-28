
//////////////////////////////////////////////Refresh Captcha after 5 min page stady///////////////////////////////////////
        /////////////////////////////////////////Commented Rohit Code :- START////////////////////////////////////////////////////
        //function refreshCaptcha() {
        //    const timestamp = new Date().getTime(); // Generate a unique timestamp
        //    document.getElementById('imgCaptcha').src = '/Account/GenerateCaptcha?' + timestamp;
        //}
        /////////////////////////////////////////Commented Rohit Code :- END //////////////////////////////////////////////////////
let idleTimeout = 30 * 60 * 1000; // 5 minutes (300,000 ms)
let timeout;

function refreshCaptcha() {
    document.getElementById("imgCaptcha").src = "/Account/GenerateCaptcha?" + new Date().getTime();
}

function resetTimer() {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
        refreshCaptcha(); // Refresh the CAPTCHA when idle time exceeds 5 minutes
    }, idleTimeout);
}

// Reset timer on user interactions
window.onload = resetTimer;
document.onmousemove = resetTimer;
document.onkeydown = resetTimer;
document.onclick = resetTimer;  // Captures clicks
document.oninput = resetTimer;  // Captures text input changes
//////////////////////////////////////////////Refresh Captcha after 5 min page stady:- END///////////////////////////////////////
/* # Designer Team Login Js Code Start */

$('.loader-dot-pulse').on('click', function () {
    var self = this;
    $(this).addClass('btn__dots--loading');
    $(this).append('<span class="btn__dots"><i></i><i></i><i></i></span>');
    setTimeout(function () {
        $(self).removeClass('btn__dots--loading');
        $(self).find('.btn__dots').remove();
    }, 10000);
});


$('.loader-dot-pulse').on('onclick', function () {
    var self = this;
    $(this).addClass('btn__dots--loading');
    $(this).append('<span class="btn__dots"><i></i><i></i><i></i></span>');
    setTimeout(function () {
        $(self).removeClass('btn__dots--loading');
        $(self).find('.btn__dots').remove();
    }, 10000);
});
/* # Designer Team Login Js Code End */

/* ---------------------------------------- */



/* # Development Team Login Js Code Start */

var authConfig = null;
var email = null;

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

$(document).ready(function () {

    /* # Designer Team Login Js Code Start */
    $('.owl-slider').owlCarousel({
        items: 1,
        loop: true,
        margin: 10,
        autoplay: true,
        autoplayTimeout: 3000,
        dots: true,
        mouseDrag: true,
        navigation: true,
    });

    // Attach the event handler to multiple elements
    $('#txtPassword,#txtNewPassword,#txtCaptcha,#txtConfirmPassword').on('copy paste', disableCopyPaste);

    function disableCopyPaste(e) {
        e.preventDefault();
        toastr.info('This functionality is disabled.', 'Error');
    }

    $("#txtCaptcha").on('input', function () {
        // Limit input to 6 digits
        if (this.value.length > 6) {
            this.value = this.value.slice(0, 6);
        }
    });

    $('.btn-password-show-hide').on('click', function (e) {
        e.preventDefault(); // Prevent form submission or other actions
        const passwordField = $(this).siblings('input');  // Get the input field associated with the button
        const isPassword = passwordField.attr('type') === 'password';

        // Toggle the password field type and eye icon class
        passwordField.attr('type', isPassword ? 'text' : 'password');
        $(this).find('.sprite-icons').toggleClass('open-eye close-eye');
    });

    // Password-Auth-Config default values it will make dynamic IN OTP verification.
    authConfig = {
        startWithCharType: true,
        endWithNumType: true,
        numberOfSpecialCharacters: 1,
        numberOfDigits: 6,
        numberOfCharacters: 5,
        excludeSequence: 3,
        hasSpecialCharacter: true,
        passwordMinLength: 10,
        passwordMaxLength: 16,
    };

    displayPasswordStrength(0);
    $("#txtNewPassword").on("input", function () {
        let password = $(this).val();
        let score = password ? calculatePasswordStrength(password, authConfig) : 0;
        displayPasswordStrength(score);
    });

    // Populate password rules when the page loads after click on Info button.
    populatePasswordRules(authConfig);

    // Handle password input changes
    $('#txtNewPassword').on('input', function () {
        const password = $(this).val();
        validatePassword(password, authConfig);
    });

    // Show tooltip on click over the info icon
    $("#passwordInfo").click(function (e) {
        const offset = $(this).offset();
        const $tooltip = $(".password-tooltip");

        if ($tooltip.is(":visible")) {
            $tooltip.fadeOut(200);
        } else {
            $tooltip.css({
                top: offset.top + $(this).outerHeight() + 5,
                left: offset.left
            }).stop(true, true).fadeIn(200);
        }
        e.stopPropagation();  // Prevent event propagation
    });

    $('#btnSignIn').click(function () {

        event.preventDefault();

        const $this = $(this);
        $this.prop('disabled', true);

        var loginModel = {
            Username: $("#txtUsername").val().trim(),
            Password: $("#txtPassword").val().trim(),
            Captcha: $('#txtCaptcha').val().trim(),
            RememberMe: $("#rememberme").prop("checked") //Added By Harshida 11-02-25
        };

        var validationMessages = {
            username: $('#common-validation-message'),
            password: $('#common-validation-message'),
            captcha: $('#common-validation-message')
        };

        if (!validateLoginModel(loginModel, validationMessages)) {
            $this.prop('disabled', false);
            return;
        }

        $.ajax({
            type: 'POST',
            url: "/Account/Auth/",
            data: JSON.stringify(loginModel),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (responseModel)
            {
                if (responseModel.isSuccess) {
                    if (responseModel.message != null && responseModel.message != "") {
                        toastr.info(responseModel.message, 'Information');
                    }

                    if (responseModel.result.startsWith("/")) {
                        window.location.href = responseModel.result;
                    }
                    else if (responseModel.verifyUserCode == responseModel.authCode) {
                        sendEmail(responseModel.result, responseModel.templateType)
                            .then(() => {
                                $('#LoginForm').toggleClass('d-block d-none');
                                $('#OTPVerificationForm').toggleClass('d-none d-block').html(responseModel.data);
                            })
                            .catch(() => {
                                setTimeout(function () {
                                    toastr.clear();
                                    $this.prop('disabled', false);
                                }, 2000);
                            });
                    }
                    else if (responseModel.statusCode == responseModel.authCode) {
                        // Bio-Metric Authentication Logic.
                    }
                }
                else {
                    toastr.error(responseModel.message, 'Error');
                    $('#common-validation-message').text('Invalid login request. Please check your credentials and try again.');
                    refreshCaptcha();
                    setTimeout(function () {
                        toastr.clear();
                        $this.prop('disabled', false);
                    }, 3000);
                }
            },
            error: function (xhr) {
                refreshCaptcha()
                $this.prop('disabled', false);
                alert("An error occurred: " + xhr.responseText);
            }
        });
    });

    $('#txtUsername, #txtPassword, #txtCaptcha').on('input', function () {
        // Clear the specific validation message related to the changed field
        var inputId = $(this).attr('id');
        if (inputId === 'txtUsername') {
            $('#common-validation-message').text('');
            $('#txtUsername').removeClass('error_input');
        } else if (inputId === 'txtPassword') {
            $('#common-validation-message').text('');
            $('#txtPassword').removeClass('error_input');
            $('#btnShowPassword').css('top', '0.9rem');
        } else if (inputId === 'txtCaptcha') {
            $('#common-validation-message').text('');
            $('#txtCaptcha').removeClass('error_input');
        }
    });

    $('#btnForgotPassword').click(function () {
        $.ajax({
            url: '/Account/ForgotPassword',
            type: 'GET',
            success: function (responseModel) {
                $('#LoginForm').addClass('d-block d-none');
                $("#ForgotPasswordForm").toggleClass('d-none d-block');
                $("#ForgotPasswordForm").html(responseModel.data);
            },
            error: function (xhr, status, error) {
                console.error("Error loading Forgot Password form: ", error);
            }
        });
    });
});

function calculatePasswordStrength(password, config) {
    let score = 0;

    // Check password length
    if (password.length >= config.passwordMinLength) {
        score += 20;
    }

    // Check for special characters
    const specialCharMatch = password.match(/[!@#$%^&*(),.?":{}|<>]/g) || [];
    if (config.hasSpecialCharacter && specialCharMatch.length >= config.numberOfSpecialCharacters) {
        score += config.numberOfSpecialCharacters * 10;
    }

    // Check for letters and digits count
    const letterCount = (password.match(/[A-Za-z]/g) || []).length;
    const digitCount = (password.match(/\d/g) || []).length;

    if (letterCount >= config.numberOfCharacters) {
        score += 10;
    }

    if (digitCount >= config.numberOfDigits) {
        score += 10;
    }

    // Check for start with character and end with digit
    if (config.startWithCharType && /^[A-Za-z]/.test(password)) {
        score += 10;
    }

    if (config.endWithNumType && /\d$/.test(password)) {
        score += 10;
    }

    // Exclude repeating sequences
    if (config.excludeSequence) {
        if (!hasRepeatingSequence(password, config.excludeSequence)) {
            score += 10; // Increase score if no repeating sequences are found
        }
        if (!hasAscendingSequence(password, config.excludeSequence)) {
            score += 10; // Increase score if no repeating sequences are found
        }
    }

    // Additional criteria for achieving "Excellent" level
    if (password.length = config.passwordMaxLength && /[A-Z]/.test(password) && /[a-z]/.test(password) && /\d/.test(password)) {
        score += 10; // Extra points for strong complexity
    }

    return Math.min(Math.max(score, 0), 100); // Ensure score is between 0 and 100%
}

function hasRepeatingSequence(password, length) {
    let sequences = new Set(); // Use a Set to track sequences we've already seen

    // Loop through the password and extract substrings of the specified length
    for (let i = 0; i <= password.length - length; i++) {
        const sequence = password.substring(i, i + length);

        // If the sequence has already been seen, it's a repeating sequence
        if (sequences.has(sequence)) {
            return true;  // Repetition found, return true
        }

        // Add the current sequence to the set
        sequences.add(sequence);
    }

    return false;  // No repetition found
}

function hasAscendingSequence(password) {
    // Check for ascending sequence of numbers (e.g., 12345)
    for (let i = 0; i < password.length - 1; i++) {
        const currentChar = password[i];
        const nextChar = password[i + 1];

        // Check if the characters are numbers and in ascending order (e.g., '1' -> '2')
        if (/\d/.test(currentChar) && /\d/.test(nextChar)) {
            if (parseInt(nextChar) === parseInt(currentChar) + 1) {
                // Check for a longer sequence
                let j = i + 1;
                while (j < password.length - 1 && parseInt(password[j + 1]) === parseInt(password[j]) + 1) {
                    j++;
                }
                if (j - i >= 4) {  // 4 digits or more in ascending order
                    return true;
                }
            }
        }

        // Check if the characters are letters and in ascending order (e.g., 'a' -> 'b')
        if (/[a-zA-Z]/.test(currentChar) && /[a-zA-Z]/.test(nextChar)) {
            if (nextChar.charCodeAt(0) === currentChar.charCodeAt(0) + 1) {
                // Check for a longer sequence
                let j = i + 1;
                while (j < password.length - 1 && password[j + 1].charCodeAt(0) === password[j].charCodeAt(0) + 1) {
                    j++;
                }
                if (j - i >= 4) {  // 4 characters or more in ascending order
                    return true;
                }
            }
        }
    }
    return false;  // No ascending sequence found
}

function displayPasswordStrength(score) {
    const $strengthLabel = $(".text-label");
    const $saveButton = $("#btnUpdatePassword");

    if (score >= 10) {
        $('.password-strength-container').removeClass('d-none');
    }

    // Define score ranges and corresponding classes/messages
    const levels = [
        { minScore: 0, maxScore: 9, className: "", message: "", colors: ["transparent", "transparent", "transparent"] },
        { minScore: 0, maxScore: 70, className: "weak", message: "Weak", colors: ["red", "transparent", "transparent"] },
        { minScore: 71, maxScore: 90, className: "fair", message: "Fair", colors: ["orange", "orange", "transparent"] },
        { minScore: 91, maxScore: 100, className: "good", message: "Good", colors: ["green", "green", "green"] }
    ];

    // Find the appropriate level for the current score
    const level = levels.find(l => score >= l.minScore && score <= l.maxScore);

    // Remove any previously applied classes and reset colors
    $(".password-strength").removeClass("weak fair good").css("border-color", "");

    if (level) {
        // Apply the message, class, and colors for the current level
        $strengthLabel.text(level.message);
        $(".password-strength").addClass(level.className);

        // Set border colors for the level
        $(".password-strength").each(function (index) {
            $(this).css("border-color", level.colors[index] || "transparent");
        });
    } else {
        $strengthLabel.text(""); // Clear label if no level matches
    }

    // Enable save button only if score is 100, otherwise disable it
    $saveButton.prop("disabled", score < 100);
}

function validateLoginModel(model, validationMessages) {
    var isValid = true;

    // Clear validation messages only when validation starts
    $.each(validationMessages, function (key, message) {
        message.text('');  // Clear any previous messages
    });

    // Username validation
    if (!model.Username) {
        validationMessages.username.text('Invalid login request. Please check your credentials and try again.');
        $('#txtUsername').addClass('error_input');
        isValid = false;
    }
    // Password validation
    if (!model.Password) {
        validationMessages.password.text('Invalid login request. Please check your credentials and try again.');
        $('#txtPassword').addClass('error_input');
        isValid = false;
    }
    // Captcha validation
    if (!model.Captcha) {
        validationMessages.captcha.text('Invalid login request. Please check your credentials and try again.');
        $('#txtCaptcha').addClass('error_input');
        isValid = false;
    }
    if (isValid == false) {
        refreshCaptcha();
    }
    return isValid;
}

function sendOTP(template) {
    const $btnSendOTP = $('#btnSendOTP');
    const $txtEmail = $("#txtEmail");
    const $emailValidationMessage = $('#email-validation-message');

    // Disable the button initially
    $btnSendOTP.prop('disabled', true);

    // Get and trim the email value
    const email = $txtEmail.val().trim();

    // Validation Helper Functions
    const showValidationMessage = (message) => {
        $emailValidationMessage.text(message);
        $txtEmail.addClass('error_input');
    };

    const resetValidationState = () => {
        $emailValidationMessage.text('');
        $txtEmail.removeClass('error_input');
    };

    // Email required validation
    if (!email) {
        showValidationMessage('Email address is required.');
        $btnSendOTP.prop('disabled', false);
        return;
    }

    // Email format validation
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailPattern.test(email)) {
        showValidationMessage('Please enter a valid registered email address.');
        //toastr.info('Please fill your registered email address for password recovery.', 'Forgot Password');
        $btnSendOTP.prop('disabled', false);
        return;
    }

    // If validation passes, reset validation state
    resetValidationState();

    // Call sendEmail function
    sendEmail(email, template)
        .then(() => {
            // Dynamically load the OTPVerification Form partial view
            $.ajax({
                url: '/Account/OTPVerification',
                type: 'GET',
                success: function (responseModel) {
                    $('#ForgotPasswordForm').addClass('d-none');
                    const updatedContent = $('<div>').html(responseModel.data);
                    updatedContent.find('#spanEmail').text(email);
                    $('#OTPVerificationForm').removeClass('d-none').addClass('d-block').html(updatedContent.html());
                },
                error: function () {
                    $btnSendOTP.prop('disabled', false);
                    console.error('Failed to load the OTP verification form. Please try again later.', 'Error');
                }
            });
        })
        .catch(() => {
            $btnSendOTP.prop('disabled', false);
            setTimeout(() => {
                toastr.clear();
                $btnSendOTP.prop('disabled', false);
            }, 2000); // Adjust delay as needed
        });
}

function sendEmail(email, templateType) {

    var sendEmailModel = {
        Email: email,
        TemplateType: templateType
    };

    return new Promise((resolve, reject) => {
        $.ajax({
            type: 'POST',
            url: '/Account/SendEmail/',
            data: JSON.stringify(sendEmailModel),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (responseModel) {
                if (responseModel.isSuccess) {
                    toastr.success(responseModel.message, 'Success');
                    resolve();
                } else {
                    toastr.error(responseModel.message, 'Error');
                    reject();
                }
            },
            error: function () {
                $('#email-validation-message').text('An error occurred. Please try again later.');
                reject(); // Reject the promise on AJAX error
            }
        });
    });
}

function verifyOTP() {

    const $btnVerifyOTP = $('#verifyOTP-btn');
    $btnVerifyOTP.prop('disabled', true);

    $('#OTP-validation-message').text('');

    var email = $("#spanEmail").text().trim() ? $("#spanEmail").text().trim() : $("#spanEmail").val().trim();
    var otp = $("#txtOTP").val();

    // Check if OTP is empty or not.
    if (otp === '') {
        $('#OTP-validation-message').text('Please enter your OTP.');
        $('#txtOTP').addClass('error_input');
        $btnVerifyOTP.prop('disabled', false);
        return;
    }
    else {
        $('#OTP-validation-message').text('');
        $('#txtOTP').removeClass('error_input');
    }

    // Validate OTP (must be 6 digits)
    var otpPattern = /^[0-9]{6}$/;
    if (!otpPattern.test(otp)) {
        $('#OTP-validation-message').text('OTP must be a 6-digit number.');
        $('#txtOTP').addClass('error_input');
        $btnVerifyOTP.prop('disabled', false);
        return;
    }
    else {
        $('#OTP-validation-message').text('');
        $('#txtOTP').removeClass('error_input');
    }

    var emailModel = {
        Email: email,
        OTP: otp
    };

    // Make AJAX POST request
    $.ajax({
        type: 'POST',
        url: '/Account/VerifyOTP/',
        data: JSON.stringify(emailModel), // Passing model
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (responseModel) {
            authConfig = responseModel.result;
            if (responseModel.isSuccess) {
                toastr.success(responseModel.message, 'Success');  // OTP verified successfully
                populatePasswordRules(authConfig);
                // Note :- here 1,2,3 is a template type based template id nothing else.
                if (responseModel.templateType == "3") {
                    window.location.href = responseModel.redirectUrl;
                    return;
                }
                else if (responseModel.templateType == "1") {
                    $('#OTPVerificationForm').toggleClass('d-block d-none');
                    $('#UpdatePasswordForm').toggleClass('d-none d-block').html(responseModel.data);
                }
            }
            else {
                $btnVerifyOTP.prop('disabled', false);
                $('#txtOTP').addClass('error_input');
                $('#OTP-validation-message').text('Incorrect OTP, Please Enter Correct OTP.');
            }
        },
        error: function (xhr, status, error) {
            $btnVerifyOTP.prop('disabled', false);
            $('#OTP-validation-message').text('An error occurred. Please try again later.');
        }
    });
}

function updatePassword() {
    var email = $("#spanEmail").text();
    $('#new-password-validation-message').text('');
    $('#confirm-password-validation-message').text('');

    var newPassword = $("#txtNewPassword").val().trim();
    var confirmPassword = $("#txtConfirmPassword").val().trim();

    // Validate new and confirm password
    if (newPassword === '') {
        $('#new-confirm-password-validation-message').text('Please enter your new password.');
        return;
    }

    if (confirmPassword === '') {
        $('#new-confirm-password-validation-message').text('Please enter your confirm password.');
        return;
    }

    if (newPassword !== confirmPassword) {
        $('#new-confirm-password-validation-message').text('Password and Confirm password do not match.');
        return;
    }

    var forgotPasswordAndResetPasswordModel = {
        Email: email,
        NewPassword: newPassword
    };

    $.ajax({
        type: 'POST',
        url: '/User/UpdateUserPassword/',
        data: JSON.stringify(forgotPasswordAndResetPasswordModel), // Passing model
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (responseModel) {
            if (responseModel.isSuccess) {
                toastr.success(responseModel.message, 'Success');
                setTimeout(function () {
                    window.location.href = responseModel.redirectUrl; // some issue that's why use static.
                }, 4000);
            }
            else {
                toastr.error(responseModel.message, 'Error');
            }
        },
        error: function () {
            $('#OTP-validation-message').text('An error occurred. Please try again later.');
        }
    });
}

function populatePasswordRules(authConfig) {
    const $tooltip = $(".password-info");  // Tooltip container
    const $ruleList = $tooltip.find("ul");  // List inside the tooltip
    $ruleList.empty();  // Clear previous content

    // Define rules based on authConfig properties
    const rules = [
        { text: `Starts with a letter.`, isValid: false },
        { text: `Ends with a number.`, isValid: false },
        { text: `At least ${authConfig.passwordMinLength} characters long.`, isValid: false },
        { text: `No more than ${authConfig.passwordMaxLength} characters long.`, isValid: false },
        { text: `At least ${authConfig.numberOfSpecialCharacters} special character(s)`, isValid: false },
        { text: `At least ${authConfig.numberOfDigits} digit(s).`, isValid: false },
        { text: `No sequential characters of ${authConfig.excludeSequence}.`, isValid: false }
    ];

    // Populate the tooltip with rules
    rules.forEach(rule => {
        const $ruleItem = $("<li>").addClass("sm-text text_primary");  // Create the rule list item

        // Add color and icon based on validity
        if (rule.isValid) {
            $ruleItem.css("color", "green").html(`<span>&#10003;</span> ${rule.text}`);  // Green checkmark for valid rule
        } else {
            $ruleItem.css("color", "black").html(`<span>&#8226;</span> ${rule.text}`);  // Black bullet for invalid rule
        }

        // Append rule item to the tooltip list
        $ruleList.append($ruleItem);
    });
}

function validatePassword(password, config) {
    const $tooltip = $(".password-info"); // Tooltip container
    const $ruleList = $tooltip.find("ul"); // Rule list inside the tooltip
    $ruleList.empty(); // Clear previous content

    let rules = [
        { text: 'Starts with a letter', isValid: /^[A-Za-z]/.test(password) },
        { text: 'Ends with a number', isValid: /\d$/.test(password) },
        { text: `At least ${config.passwordMinLength} characters long`, isValid: password.length >= config.passwordMinLength },
        { text: `No more than ${config.passwordMaxLength} characters long`, isValid: password.length <= config.passwordMaxLength },
        { text: `At least ${config.numberOfSpecialCharacters} special character(s)`, isValid: (password.replace(/[A-Za-z0-9]/g, '').length) >= config.numberOfSpecialCharacters },
        { text: `At least ${config.numberOfDigits} digit(s)`, isValid: (password.match(/\d/g) || []).length >= config.numberOfDigits },
        { text: `No sequential characters of ${config.excludeSequence}`, isValid: !hasSequentialChars(password, config.excludeSequence) }
    ];

    rules.forEach(rule => {
        const $ruleItem = $("<li>").addClass("sm-text text_primary");

        // Add green checkmark if valid, black dot if invalid
        if (rule.isValid) {
            $ruleItem.css("color", "green").html(`<span>&#10003;</span> ${rule.text}`);
        } else {
            $ruleItem.css("color", "black").html(`<span>&#8226;</span> ${rule.text}`);
        }

        // Append each rule item to the tooltip
        $ruleList.append($ruleItem);
    });

    return { isValid: rules.every(rule => rule.isValid), rules: rules };
}

function hasSequentialChars(password, excludeSequence) {
    for (let i = 0; i < password.length - excludeSequence + 1; i++) {
        const substring = password.substring(i, i + excludeSequence);
        if (/012|123|234|345|456|567|678|789|890/.test(substring)) {
            return true;
        }
    }
    return false;
}
