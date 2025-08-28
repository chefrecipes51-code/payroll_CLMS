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

/*
 * Note that this is toastr v2.1.3, the "latest" version in url has no more maintenance,
 * please go to https://cdnjs.com/libraries/toastr.js and pick a certain version you want to use,
 * make sure you copy the url from the website since the url may change between versions.
 * */
!function (e) { e(["jquery"], function (e) { return function () { function t(e, t, n) { return g({ type: O.error, iconClass: m().iconClasses.error, message: e, optionsOverride: n, title: t }) } function n(t, n) { return t || (t = m()), v = e("#" + t.containerId), v.length ? v : (n && (v = d(t)), v) } function o(e, t, n) { return g({ type: O.info, iconClass: m().iconClasses.info, message: e, optionsOverride: n, title: t }) } function s(e) { C = e } function i(e, t, n) { return g({ type: O.success, iconClass: m().iconClasses.success, message: e, optionsOverride: n, title: t }) } function a(e, t, n) { return g({ type: O.warning, iconClass: m().iconClasses.warning, message: e, optionsOverride: n, title: t }) } function r(e, t) { var o = m(); v || n(o), u(e, o, t) || l(o) } function c(t) { var o = m(); return v || n(o), t && 0 === e(":focus", t).length ? void h(t) : void (v.children().length && v.remove()) } function l(t) { for (var n = v.children(), o = n.length - 1; o >= 0; o--)u(e(n[o]), t) } function u(t, n, o) { var s = !(!o || !o.force) && o.force; return !(!t || !s && 0 !== e(":focus", t).length) && (t[n.hideMethod]({ duration: n.hideDuration, easing: n.hideEasing, complete: function () { h(t) } }), !0) } function d(t) { return v = e("<div/>").attr("id", t.containerId).addClass(t.positionClass), v.appendTo(e(t.target)), v } function p() { return { tapToDismiss: !0, toastClass: "toast", containerId: "toast-container", debug: !1, showMethod: "fadeIn", showDuration: 300, showEasing: "swing", onShown: void 0, hideMethod: "fadeOut", hideDuration: 1e3, hideEasing: "swing", onHidden: void 0, closeMethod: !1, closeDuration: !1, closeEasing: !1, closeOnHover: !0, extendedTimeOut: 1e3, iconClasses: { error: "toast-error", info: "toast-info", success: "toast-success", warning: "toast-warning" }, iconClass: "toast-info", positionClass: "toast-top-right", timeOut: 5e3, titleClass: "toast-title", messageClass: "toast-message", escapeHtml: !1, target: "body", closeHtml: '<button type="button">&times;</button>', closeClass: "toast-close-button", newestOnTop: !0, preventDuplicates: !1, progressBar: !1, progressClass: "toast-progress", rtl: !1 } } function f(e) { C && C(e) } function g(t) { function o(e) { return null == e && (e = ""), e.replace(/&/g, "&amp;").replace(/"/g, "&quot;").replace(/'/g, "&#39;").replace(/</g, "&lt;").replace(/>/g, "&gt;") } function s() { c(), u(), d(), p(), g(), C(), l(), i() } function i() { var e = ""; switch (t.iconClass) { case "toast-success": case "toast-info": e = "polite"; break; default: e = "assertive" }I.attr("aria-live", e) } function a() { E.closeOnHover && I.hover(H, D), !E.onclick && E.tapToDismiss && I.click(b), E.closeButton && j && j.click(function (e) { e.stopPropagation ? e.stopPropagation() : void 0 !== e.cancelBubble && e.cancelBubble !== !0 && (e.cancelBubble = !0), E.onCloseClick && E.onCloseClick(e), b(!0) }), E.onclick && I.click(function (e) { E.onclick(e), b() }) } function r() { I.hide(), I[E.showMethod]({ duration: E.showDuration, easing: E.showEasing, complete: E.onShown }), E.timeOut > 0 && (k = setTimeout(b, E.timeOut), F.maxHideTime = parseFloat(E.timeOut), F.hideEta = (new Date).getTime() + F.maxHideTime, E.progressBar && (F.intervalId = setInterval(x, 10))) } function c() { t.iconClass && I.addClass(E.toastClass).addClass(y) } function l() { E.newestOnTop ? v.prepend(I) : v.append(I) } function u() { if (t.title) { var e = t.title; E.escapeHtml && (e = o(t.title)), M.append(e).addClass(E.titleClass), I.append(M) } } function d() { if (t.message) { var e = t.message; E.escapeHtml && (e = o(t.message)), B.append(e).addClass(E.messageClass), I.append(B) } } function p() { E.closeButton && (j.addClass(E.closeClass).attr("role", "button"), I.prepend(j)) } function g() { E.progressBar && (q.addClass(E.progressClass), I.prepend(q)) } function C() { E.rtl && I.addClass("rtl") } function O(e, t) { if (e.preventDuplicates) { if (t.message === w) return !0; w = t.message } return !1 } function b(t) { var n = t && E.closeMethod !== !1 ? E.closeMethod : E.hideMethod, o = t && E.closeDuration !== !1 ? E.closeDuration : E.hideDuration, s = t && E.closeEasing !== !1 ? E.closeEasing : E.hideEasing; if (!e(":focus", I).length || t) return clearTimeout(F.intervalId), I[n]({ duration: o, easing: s, complete: function () { h(I), clearTimeout(k), E.onHidden && "hidden" !== P.state && E.onHidden(), P.state = "hidden", P.endTime = new Date, f(P) } }) } function D() { (E.timeOut > 0 || E.extendedTimeOut > 0) && (k = setTimeout(b, E.extendedTimeOut), F.maxHideTime = parseFloat(E.extendedTimeOut), F.hideEta = (new Date).getTime() + F.maxHideTime) } function H() { clearTimeout(k), F.hideEta = 0, I.stop(!0, !0)[E.showMethod]({ duration: E.showDuration, easing: E.showEasing }) } function x() { var e = (F.hideEta - (new Date).getTime()) / F.maxHideTime * 100; q.width(e + "%") } var E = m(), y = t.iconClass || E.iconClass; if ("undefined" != typeof t.optionsOverride && (E = e.extend(E, t.optionsOverride), y = t.optionsOverride.iconClass || y), !O(E, t)) { T++, v = n(E, !0); var k = null, I = e("<div/>"), M = e("<div/>"), B = e("<div/>"), q = e("<div/>"), j = e(E.closeHtml), F = { intervalId: null, hideEta: null, maxHideTime: null }, P = { toastId: T, state: "visible", startTime: new Date, options: E, map: t }; return s(), r(), a(), f(P), E.debug && console && console.log(P), I } } function m() { return e.extend({}, p(), b.options) } function h(e) { v || (v = n()), e.is(":visible") || (e.remove(), e = null, 0 === v.children().length && (v.remove(), w = void 0)) } var v, C, w, T = 0, O = { error: "error", info: "info", success: "success", warning: "warning" }, b = { clear: r, remove: c, error: t, getContainer: n, info: o, options: {}, subscribe: s, success: i, version: "2.1.3", warning: a }; return b }() }) }("function" == typeof define && define.amd ? define : function (e, t) { "undefined" != typeof module && module.exports ? module.exports = t(require("jquery")) : window.toastr = t(window.jQuery) });
//# sourceMappingURL=toastr.js.map

/* # Designer Team Login Js Code Start */

$(document).ready(function () {
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
});

$('.loader-dot-pulse').on('click', function () {
    var self = this;
    $(this).addClass('btn__dots--loading');
    $(this).append('<span class="btn__dots"><i></i><i></i><i></i></span>');
    setTimeout(function () {
        $(self).removeClass('btn__dots--loading');
        $(self).find('.btn__dots').remove();
    }, 2000);
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
    $("#txtOTP").on('input', function () {
        // Replace any non-numeric characters with an empty string
        this.value = this.value.replace(/\D/g, '');

        // Limit input to 6 digits
        if (this.value.length > 6) {
            this.value = this.value.slice(0, 6);
        }
    });

    $("#txtCaptcha").on('input', function () {
        // Limit input to 6 digits
        if (this.value.length > 6) {
            this.value = this.value.slice(0, 6);
        }
    });

    function disableCopyPaste(e) {
        e.preventDefault();
        toastr.info('This functionality is disabled.', 'Error');
    }

    // Attach the event handler to multiple elements
    $('#txtPassword, #txtCaptcha, #txtOTP, #txtConfirmPassword').on('copy paste', disableCopyPaste);

});

$(document).ready(function () {
    displayPasswordStrength(0);

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


function refreshCaptcha() {
    document.getElementById('imgCaptcha').src = '/Base/GenerateCaptcha?' + new Date().getTime();
}

// Event handler for all password fields
$('.btn-password-show-hide').on('click', function (e) {
    e.preventDefault(); // Prevent form submission or other actions
    funShowHidePassword($(this));  // Pass the clicked button (container)
});

// Function to toggle password visibility and icon
function funShowHidePassword(button) {
    const passwordField = button.siblings('input');  // Get the input field associated with the button
    const isPassword = passwordField.attr('type') === 'password';

    // Toggle the password field type
    passwordField.attr('type', isPassword ? 'text' : 'password');

    // Toggle the eye icon class
    button.find('.sprite-icons').toggleClass('open-eye close-eye');
}
$('#btnSignIn').click(function () {

    const $this = $(this);
    $this.prop('disabled', true);

    var loginModel = {
        Username: $("#txtUsername").val().trim(),
        Password: $("#txtPassword").val().trim(),
        Captcha: $('#txtCaptcha').val().trim()
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
        url: "/Login/Auth/",
        data: JSON.stringify(loginModel),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (responseModel) {
            if (responseModel.isSuccess) {
                if (responseModel.statusCode == responseModel.result.authType) {
                    // Call function for sending email and handle success or failure
                    sendEmail(responseModel.result.email, 3)
                        .then(() => {
                            $('#LoginForm').toggleClass('d-none');
                            $('#OTPForm').toggleClass('d-none d-block');
                            $('#hdTemplateId').val(3);
                            $("#spanEmail").text(responseModel.result.email);
                        })
                        .catch(() => {
                            setTimeout(function () {
                                toastr.clear();
                                $this.prop('disabled', false);
                            }, 2000); // Adjust delay as needed
                        });

                } else if (responseModel.redirectUrl) {
                    window.location.href = responseModel.redirectUrl;
                } else if (responseModel.result.roles.length > 1) {
                    var roleHtml = responseModel.result.roles.map(function (role) {
                        return `
                            <label style="display: flex; align-items: center;">
                                <input style="margin-right: 10px;" type="radio" name="role" value="${role.roleId}">
                                ${role.roleName}
                            </label><br>`;
                    }).join('');

                    $('#role-selection').html(roleHtml);
                    $('#roleModal').modal('show');
                }
            }
            else {
                toastr.error(responseModel.message, 'Error');
                $('#common-validation-message').text('Invalid login request. Please check your credentials and try again.');
                setTimeout(function () {
                    toastr.clear();
                    $this.prop('disabled', false);
                }, 3000);
            }
        },
        error: function (xhr) {
            $this.prop('disabled', false);
            alert("An error occurred: " + xhr.responseText);
        }
    });
});

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
    return isValid;
}

// Optional: To remove validation messages when the user changes input fields
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

function forgotPassword() {
    $('#LoginForm').addClass('d-none');
    $("#ForgotPasswordForm").removeClass('d-none');
}

$('#btnSendOTP').click(function () {
    var email = $("#txtEmail").val().trim(); // Get and trim the email value need only user email for forgot & reset password.

    if (!email) {
        $('#email-validation-message').text('Email address is required.');
        $("#txtEmail").addClass('error_input');
        return;
    }

    // Email format validation
    var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailPattern.test(email)) {
        $('#email-validation-message').text('Please enter a valid registered email address.');
        $("#txtEmail").addClass('error_input');
        toastr.info('Please fill your registered email address for password recovery.', 'Forgot Password');
        return;
    }

    //Note By Rohit Tiwari :- If validation passes, call sendEmail function
    sendEmail(email, 1)
        .then(() => {
            $('#ForgotPasswordForm').toggleClass('d-none');
            $('#OTPForm').toggleClass('d-none d-block');
            $('#hdTemplateId').val(1);
            $("#spanEmail").text(email);
        })
        .catch(() => {
            setTimeout(function () {
                toastr.clear();
                $this.prop('disabled', false);
            }, 2000); // Adjust delay as needed
        });
});

function sendEmail(email, templateType) {
    var sendEmailModel = {
        Email: email,
        TemplateType: templateType
    };

    return new Promise((resolve, reject) => {
        $.ajax({
            type: 'POST',
            url: '/Base/SendEmail/',
            data: JSON.stringify(sendEmailModel),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (responseModel) {
                if (responseModel.isSuccess) {
                    $('#otp-section').toggleClass('d-none d-block');
                    $('#verify-btn').text('Verify OTP');
                    toastr.success(responseModel.message, 'Success');
                    resolve(); // Resolve the promise on success
                } else {
                    toastr.error(responseModel.message, 'Error');
                    reject(); // Reject the promise on failure
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
    const $this = $("#verify-btn");
    $this.prop('disabled', true);

    var otpVerificationRequest = $('#hdTemplateId').val();
    $('#OTP-validation-message').text('');

    var email = $("#spanEmail").text();
    var otp = $("#txtOTP").val();

    // Check if OTP is empty or not.
    if (otp === '') {
        $('#OTP-validation-message').text('Please enter your OTP.');
        $('#txtOTP').addClass('error_input');
        $this.prop('disabled', false);
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
        $this.prop('disabled', false);
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
        url: '/Base/VerifyOTP/',
        data: JSON.stringify(emailModel), // Passing model
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (responseModel) {
            authConfig = responseModel.result;
            if (responseModel.isSuccess) {
                toastr.success(responseModel.message, 'Success');  // OTP verified successfully
                // Populate the password validation rules dynamically
                
                populatePasswordRules(authConfig);
                // Note :- here 1,2,3 is a template type based template id nothing else.
                if (otpVerificationRequest == 3) {
                    window.location.href = responseModel.redirectUrl;
                    return;
                }
                else if (otpVerificationRequest == 1) {
                    $('#OTPForm').toggleClass('d-block d-none');
                    $('#UpdatePasswordForm').toggleClass('d-none d-block');
                }
                else if (otpVerificationRequest == 2) {
                    $('#OTPForm').toggleClass('d-block d-none');
                    $('#ResetPasswordForm').toggleClass('d-none d-block');
                }
            }
            else {
                $('#txtOTP').addClass('error_input');
                $('#OTP-validation-message').text('Incorrect OTP, Please Enter Correct OTP.');
                /*toastr.error(responseModel.message, 'Error');*/  /* As per design team figma*/
                $this.prop('disabled', false);
            }
        },
        error: function (xhr, status, error) {
            //hideLoader();
            $('#OTP-validation-message').text('An error occurred. Please try again later.');
            $this.prop('disabled', false);
        }
    });
}

/* Update password */

$('#btnUpdatePassword').click(function () {

    $('#new-password-validation-message').text('');
    $('#confirm-password-validation-message').text('');

    email = $("#spanEmail").text();
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
                    window.location.href = 'https://localhost:7093'; //design issue in owl crausal that why static use here.
                    //window.location.href = responseModel.redirectUrl;
                }, 4000);
            }
            else {
                toastr.error(responseModel.message, 'Error');
            }
        },
        error: function () {
            //hideLoader();
            $('#OTP-validation-message').text('An error occurred. Please try again later.');
        }
    });
});

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


$(document).ready(function () {
    // Step 1: Restrict OTP input to only numeric characters
    $("#txtOTP").on('input', function () {
        // Replace any non-numeric characters with an empty string
        this.value = this.value.replace(/\D/g, '');

        // Limit input to 6 digits
        if (this.value.length > 6) {
            this.value = this.value.slice(0, 6);
        }
    });
});



$('#btnShowPassword').click(function () {
    var passwordInput = $('#txtPassword');
    var passwordFieldType = passwordInput.attr('type');

    // Toggle password visibility
    if (passwordFieldType === 'password') {
        passwordInput.attr('type', 'text');
        $(this).addClass('fa fa-eye-slash').removeClass('icon-eye');
    } else {
        passwordInput.attr('type', 'password');
        $(this).addClass('icon-eye').removeClass('fa fa-eye-slash');
    }
});


var authConfig = null;

function SendEmailAndVerifyOTP(template) {

    showLoader();
    $('#email-validation-message').text('');
    $('#OTP-validation-message').text('');

    var email = $("#txtEmail").val();
    var templateType = template; // forgor password & reset password set from template value.
    var otp = $("#txtOTP").val();
    var isOtpStage = $('#otp-section').hasClass('div-show'); // Check if OTP is visible

    if (!isOtpStage) {
        if (email === '') {
            $('#email-validation-message').text('Please enter your email.');
            return;
        }

        if (!validateEmail(email)) {
            $('#email-validation-message').text('Valid email is required.');
            return;
        }

        $.ajax({
            type: 'POST',
            url: '/User/SendUpdateUserPasswordEmailWithOTP/',
            data: JSON.stringify({ Email: email, TemplateType: templateType }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (responseModel) {
                if (responseModel.isSuccess) {
                    hideLoader();
                    $("#txtEmail").prop('disabled', true);
                    $('#otp-section').toggleClass('div-hide div-show');
                    $('#verify-btn').text('Verify OTP');
                    toastr.success(responseModel.message, 'Success');
                }
                else {

                    if (responseModel.StatusCode === 404) {
                        hideLoader();
                        toastr.error(responseModel.message, 'Error');
                    }
                    else {
                        hideLoader();
                        $('#otp-section').toggleClass('div-hide div-show');
                        $('#verify-btn').text('Verify OTP');
                        toastr.error(responseModel.message, 'Error');
                    }
                }
            },
            error: function (xhr, status, error) {
                hideLoader();
                $('#email-validation-message').text('An error occurred. Please try again later.');
            }
        });
    }
    else {
        $('#OTP-validation-message').text('');

        var otp = $("#txtOTP").val();

        // Check if OTP is empty
        if (otp === '') {
            $('#OTP-validation-message').text('Please enter your OTP.');
            return;
        }

        // Validate OTP (must be 6 digits)
        var otpPattern = /^[0-9]{6}$/;
        if (!otpPattern.test(otp)) {
            $('#OTP-validation-message').text('OTP must be a 6-digit number.');
            return;
        }

        // Prepare ForgotPasswordModel object with Email and OTP
        var forgotPasswordModel = {
            Email: email,
            OTP: otp
        };

        // Make AJAX POST request
        $.ajax({
            type: 'POST',
            url: '/User/VerifyOTP/',
            data: JSON.stringify(forgotPasswordModel), // Passing model
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (responseModel) {
                authConfig = responseModel.result;
                if (responseModel.isSuccess) {
                    hideLoader();
                    toastr.success(responseModel.message, 'Success');  // OTP verified successfully
                    $('#UpdatePasswordForm').toggleClass('div-show div-hide');
                    $('#verify-btn').addClass('div-hide');
                }
                else {
                    hideLoader();
                    toastr.error(responseModel.message, 'Error');
                }
            },
            error: function (xhr, status, error) {
                hideLoader();
                $('#OTP-validation-message').text('An error occurred. Please try again later.');
            }
        });
    }
}


$('#UpdatePassword').click(function () {

    showLoader();
    $('#new-password-validation-message').text('');
    $('#confirm-password-validation-message').text('');

    var email = $('#txtEmail').val().trim();
    var newPassword = $("#txtNewPassword").val().trim();
    var confirmPassword = $("#txtConfirmPassword").val().trim();

    // Validate new and confirm password
    if (newPassword === '') {
        $('#new-password-validation-message').text('Please enter your new password.');
        return;
    }
    if (confirmPassword === '') {
        $('#confirm-password-validation-message').text('Please enter your confirm password.');
        return;
    }
    if (newPassword !== confirmPassword) {
        $('#confirm-password-validation-message').text('Password and Confirm password do not match.');
        return;
    }

    // Validate the password configuration
    var validationResult = validatePassword(newPassword, authConfig);
    if (!validationResult.isValid) {
        $('#new-password-validation-message').text(validationResult.message);
        return; // Prevent the request if validation fails
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
                hideLoader();
                toastr.success(responseModel.message, 'Success');
                setTimeout(function () {
                    window.location.href = 'https://localhost:7093/';
                }, 2000);
            } else {
                toastr.error(responseModel.message, 'Error');
            }
        },
        error: function () {
            hideLoader();
            $('#OTP-validation-message').text('An error occurred. Please try again later.');
        }
    });
});

function validatePassword(password, config) {
    let messages = [];

    // Check minimum length
    if (config.passwordMinLength && password.length < config.passwordMinLength) {
        messages.push(`Password must be at least ${config.passwordMinLength} characters long.`);
    }

    // Check maximum length
    if (config.passwordMaxLength && password.length > config.passwordMaxLength) {
        messages.push(`Password must not exceed ${config.passwordMaxLength} characters.`);
    }

    // Check special characters
    if (config.hasSpecialCharacter && config.numberOfSpecialCharacters > 0) {
        const specialChars = password.replace(/[A-Za-z0-9]/g, '').length;
        if (specialChars < config.numberOfSpecialCharacters) {
            messages.push(`Password must contain at least ${config.numberOfSpecialCharacters} special character(s).`);
        }
    }

    // Check starting character
    if (config.startWithCharType && !/^[A-Za-z]/.test(password)) {
        messages.push('Password must start with a letter.');
    }

    // Check ending character
    if (config.endWithNumType && !/\d$/.test(password)) {
        messages.push('Password must end with a number.');
    }

    // Check number of digits
    if (config.numberOfDigits && (password.match(/\d/g) || []).length < config.numberOfDigits) {
        messages.push(`Password must contain at least ${config.numberOfDigits} digit(s).`);
    }

    // Check for sequential characters
    if (config.excludeSequence && hasSequentialChars(password, config.excludeSequence)) {
        messages.push('Password contains a sequence of characters that are not allowed.');
    }

    return { isValid: messages.length === 0, message: messages.join(' ') };
}

function hasSequentialChars(password, length) {
    for (let i = 0; i <= password.length - length; i++) {
        let segment = password.slice(i, i + length);
        if (/(\d)\1{2,}/.test(segment) || /([a-zA-Z])\1{2,}/.test(segment)) {
            return true; // Found sequential characters
        }
    }
    return false;
}

function validateEmail(email) {
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}


    $.ajax({
        type: "POST",
        url: "/User/GetUsers",
        data: '{}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccess,
        failure: function (responseModel) {
            alert(responseModel.d);
        },
        error: function (responseModel) {
            alert(responseModel.d);
        }
    });
});

function OnSuccess(responseModel)
{
    $("#tblUser").DataTable(
        {
            Filter: true,
            Sort: true,
            Paginate: true,
            pageLength: 5,
            lengthMenu: [5, 10, 15, 20, 25],
            processing: true,
            data: responseModel.modelListData,
            columns: [
                {
                    "title": "#",
                    render: function (data, type, row, meta) {
                        var index = meta.row + 1 + 1000
                        return index;
                    }
                },
                {
                    data: "FullName",
                    "title": "Full Name",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return `${row.firstName || ''} ${row.middleName || ''} ${row.lastName || ''}`.trim();
                    }
                },
                {
                    data: "Gender",
                    "title": "Gender",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return row.gender;
                    }
                },
                {
                    data: "Email",
                    "title": "Email",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return row.email;
                    }
                },
                {
                    data: "DateOfBirth",
                    "title": "Date Of Birth",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return row.dateOfBirth;
                    }
                },
                {
                    data: "IsActive",
                    "title": "Status",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        const statusButtonClass = row.isActive ? 'btn-success' : 'btn-danger';
                        const statusButtonText = row.isActive ? 'Active' : '&nbsp Inactive';
                        const handlePositionClass = row.isActive ? 'handle-right' : 'handle-left';
                    }
                }
            ],
            "columnDefs": [
                {
                    "targets": "_all",
                    "render": function (data, type, row, meta) {
                        return "<th>" + row.title + "</th>";
                    }
                }
            ]
        }
    );

};
$(document).ready(function () {
    $.ajax({
        type: "POST",
        url: "/WageGradeDetail/GetWageGradeDetail",
        data: '{}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccess,
        failure: function (response) {
            alert(response.d);
        },
        error: function (response) {
            alert(response.d);
        }
    });

});

function OnSuccess(response) {
    console.log(response);
    if ($.fn.DataTable) {
        $("#tblWageGrade").DataTable(
            {
                Filter: true,
                Sort: true,
                Paginate: true,
                pageLength: 5,
                lengthMenu: [5, 10, 15, 20, 25],
                processing: true,
                data: response.result,
                columns: [
                    {
                        data: "wage_Id", // Use lowercase as per your response
                        title: "#",
                        render: function (data, type, row, meta) {
                            return data.wage_Id; // This could also just return 'data'
                        },
                        width: '3%'
                    },
                    {
                        data: "wageGradeCode", // Use lowercase as per your response
                        title: "Wage Grade Code",
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return data.wageGradeCode; // This could also just return 'data'
                        }
                    },
                    {
                        data: "wageGradeName", // Use lowercase as per your response
                        title: "Name",
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return data.wageGradeName; // This could also just return 'data'
                        }
                    },
                    {
                        data: "wageGradeBasic", // Use lowercase as per your response
                        title: "Basic",
                        orderable: false,
                        render: function (data, type, row, meta) {
                            return data.wageGradeBasic; // This could also just return 'data'
                        }
                    },
                ],

            });
    }
}
$(document).ready(function () {
    $.ajax({
        type: "POST",
        url: "/WageGradeDetail/GetWageGradeDetail",
        data: '{}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: OnSuccess,
        failure: function (response) {
            alert(response.d);
        },
        error: function (response) {
            alert(response.d);
        }
    });
});

function OnSuccess(response) {
    // Ensure the DataTable initialization works after the data is loaded
    console.log(response);
    if ($.fn.DataTable) {
        $("#tblWageGrade").DataTable({
            filter: true,
            sort: true,
            paginate: true,
            pageLength: 5,
            lengthMenu: [5, 10, 15, 20, 25],
            processing: true,
            data: response.result,
            columns: [
                {
                    data: "wage_Id", // Use lowercase as per your response
                    title: "#",
                    render: function (data, type, row, meta) {
                        return row.wage_Id; // This could also just return 'data'
                    },
                    width: '3%'
                },
                {
                    data: "wageGradeCode", // Use lowercase as per your response
                    title: "Wage Grade Code",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return row.wageGradeCode; // This could also just return 'data'
                    }
                },
                {
                    data: "wageGradeName", // Use lowercase as per your response
                    title: "Name",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return row.wageGradeName; // This could also just return 'data'
                    }
                },
                {
                    data: "wageGradeBasic", // Use lowercase as per your response
                    title: "Basic",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        return row.wageGradeBasic; // This could also just return 'data'
                    }
                },
                {
                    data: "isActive", // Use lowercase as per your response
                    title: "Status",
                    orderable: false,
                    render: function (data, type, row, meta) {
                        const statusButtonClass = row.isActive ? 'btn-success' : 'btn-danger';
                        const statusButtonText = row.isActive ? 'Active' : 'Inactive';
                        const handlePositionClass = row.isActive ? 'handle-right' : 'handle-left';
                        return `<button type="button" class="btn btn-sm btn-toggle ${statusButtonClass} ml-2" data-id="${row.wage_Id}" aria-pressed="${row.isActive}" autocomplete="off">${statusButtonText} <div class="handle ${handlePositionClass}"></div></button>`;
                    }
                },
                {
                    title: "Action",
                    render: function (data, type, row, meta) {
                        return `<button type="button" class='btn btn-success' onclick="EditWage(${row.wage_Id});"> <i class="fa fa-edit"></i> </button>`;
                    },
                    orderable: false
                }
            ],
        });
    } else {
        console.error("DataTables library is not loaded.");
    }

}


