//function validateFormRequired(value, errorElement, errorMessage) {
//    if (!value || value.trim() === "") {
//        $(errorElement).text(errorMessage);
//        return false;
//    }
//    $(errorElement).text(""); // Clear error if valid
//    return true;
//}

function validateFormRequired(value, errorElement, errorMessage) { 
    if (!value || value.trim() === "") {
        $(errorElement).text(errorMessage);
        return false;
    }
    if (/\s{2,}/.test(value)) {
        $(errorElement).text("Value should not contain consecutive white spaces.");
        return false;
    }
    $(errorElement).text(""); 
    return true;
}

function validateFormMaxLength(value, maxLength, errorElement, errorMessage) {  
    if (value.trim().length > maxLength) {
        $(errorElement).text(errorMessage);
        return false;
    }
    $(errorElement).text(""); // Clear error if valid
    return true;
}

// Checks if from <= to (for numbers like salary, amount etc.)
function validateNumericRange(minVal, maxVal, errorSelector, message) {
    if (parseFloat(minVal) > parseFloat(maxVal)) {
        $(errorSelector).text(message);
        return false;
    } else {
        $(errorSelector).text("");
        return true;
    }
}

// Checks if from <= to (for dates)
function validateDateRange(fromDateStr, toDateStr, errorSelector, message) {
    var fromDate = new Date(fromDateStr);
    var toDate = new Date(toDateStr);
    if (fromDate > toDate) {
        $(errorSelector).text(message);
        return false;
    } else {
        $(errorSelector).text("");
        return true;
    }
}

// Checks if numeric field is not negative
function validateNonNegative(value, errorSelector, message) {
    if (parseFloat(value) < 0) {
        $(errorSelector).text(message);
        return false;
    } else {
        $(errorSelector).text("");
        return true;
    }
}

function validateMonthRange(value, errorSelector, errorMessage) {
    if (value && (value < 1 || value > 12)) {
        $(errorSelector).text(errorMessage);
        return false;
    }
    $(errorSelector).text("");
    return true;
}

function validateNonNegativeIfNotNull(value, errorSelector, errorMessage) {
    if (value !== "" && parseFloat(value) < 0) {
        $(errorSelector).text(errorMessage);
        return false;
    }
    $(errorSelector).text("");
    return true;
}

////////////////////////////////// Phone Number Validation Data Type Start:- nvarchar(20)/////////////////////////////////////
function validateFormPrimaryPhoneNumber(value, errorElement) {
    var phoneRegex = /^[0-9]{10,11}$/; // Only digits, min 10 - max 11 characters

    if (!value.trim()) {
        $(errorElement).text("Primary phone number is required.");
        return false;
    }

    if (!phoneRegex.test(value)) {
        $(errorElement).text("Primary phone number must be 10 to 11 digits long.");
        return false;
    }

    $(errorElement).text("");
    return true;
}
function validateFormSecondaryPhoneNumber(value, errorElement) {
    var phoneRegex = /^[0-9]{10,11}$/; // Only digits, min 10 - max 11 characters

    if (value.trim() === "") {
        $(errorElement).text(""); // Secondary phone is optional
        return true;
    }

    if (!phoneRegex.test(value)) {
        $(errorElement).text("Secondary phone number must be 10 to 11 digits long.");
        return false;
    }

    $(errorElement).text("");
    return true;
}
////////////////////////////////// Phone Number Validation Data Type End:- nvarchar(20)/////////////////////////////////////

////////////////////////////////// Email Validation Data Type Start:- nvarchar(100)/////////////////////////////////////

function validateFormPrimaryEmail(value, errorElement) {
    // Trim spaces
    value = value.trim();

    // Check for leading spaces or leading dots
    if (/^[.\s]/.test(value)) {
        $(errorElement).text("Email should not start with a space or dot.");
        return false;
    }
    if (value.trim() === "") {
        $(errorElement).text("Please Provide Email Address."); 
        return true;
    }
    var emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(value)) {
        $(errorElement).text("Please enter a valid email address (e.g., example@domain.com).");
        return false;
    }
    $(errorElement).text("");
    return true;
}
function validateFormSecondaryEmail(value, errorElement) {
    // Trim spaces
    value = value.trim();

    // Check for leading spaces or leading dots
    if (/^[.\s]/.test(value)) {
        $(errorElement).text("Email should not start with a space or dot.");
        return false;
    }
    var emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!emailRegex.test(value)) {
        $(errorElement).text("Please enter a valid email address (e.g., example@domain.com).");
        return false;
    }
    $(errorElement).text("");
    return true;
}
////////////////////////////////// Email Validation Data Type End:- nvarchar(100)/////////////////////////////////////

function validateFormURL(value, errorElement) {
    var urlRegex = /^(https?:\/\/)?([\w-]+\.)+[\w-]{2,}(\/[\w-./?%&=]*)?$/i;
    if (value.trim() !== "" && !urlRegex.test(value)) {
        $(errorElement).text("Please enter a valid Website URL (e.g., https://example.com).");
        return false;
    }
    $(errorElement).text("");
    return true;
}
function validateFormDropdown(value, errorElement, errorMessage) {
    if (!value || value === "") {
        $(errorElement).text(errorMessage);
        return false;
    }
    $(errorElement).text("");
    return true;
}