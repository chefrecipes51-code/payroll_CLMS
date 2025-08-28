/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 28-April-2025
///  IMP NOTE      :- 1. IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
///                   2. Using These file perform Only fetching of Pay Component list.
///                   3. Using these file perform CREATE FORMULA operation ONLY.
/// </summary>

////////////////////////////// Common Function for Formula:- Start///////////////////////////////////////

function AddFormulaUsingPayComponent() {
    var finalformulaInput = $("#formulaInput").val();
    finalformulaInput = finalformulaInput.replace(/\[([^\]]+)\]/g, '$1');

    var data = {
        Formula_Id: $("#formula_Id").val(),
        Formula_Code: $("#formulaCode").val(),
        Formula_Computation: finalformulaInput,
        FormulaName: $("#formulaName").val(),
        IsActive: $("#formulaActiveToggle").is(":checked")
    };

    $.ajax({
        url: "/FormulaMaster/AddFormula",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                $("#formulaCode").val('');
                $("#formulaName").val('');
                $("#formulaInput").val('');
                document.activeElement.blur();
                $("#addFormula").modal('hide');
                showAlert("success", response.message);
            } else {
                showAlert("danger", response.message);
            }
        },
        error: function (xhr) {
            alert("Error: " + xhr.responseText);
        }
    });
}

function validateComponenntFormulausingNCalc() {
    const formulaWithUnderscore = $("#hiddenFieldWithUnderscores").val();

    return new Promise((resolve, reject) => {
        $.ajax({
            url: "/FormulaMaster/ValidateFormula",
            dataType: "json",
            data: { formulaVal: formulaWithUnderscore },
            success: function (response) {
                if (response.success) {
                    resolve(null); // No error                   
                } else {
                    resolve(response.message); // Validation error message
                }
            },
            error: function (err) {
                reject("Error in validating formula");
            }
        });
    });
}

async function validateCompoenntFormulaForm() {
    let isValid = true;
    let firstInvalidField = null;

    const formulaCode = $("#formulaCode").val();
    const formulaName = $("#formulaName").val();
    const rawformulaInput = $("#formulaInput").val();
    const formulaInput = rawformulaInput.trim();

    if (!validateFormRequired(formulaCode, "#formulaCode-error", "Please enter formula code.")) {
        isValid = false;
        firstInvalidField ??= "#formulaCode";
    } else if (!validateFormMaxLength(formulaCode, 20, "#formulaCode-error", "Formula code must not exceed 20 characters.")) {
        isValid = false;
        firstInvalidField ??= "#formulaCode";
    }

    if (!validateFormRequired(formulaName, "#formulaName-error", "Please enter formula name.")) {
        isValid = false;
        firstInvalidField ??= "#formulaName";
    } else if (!validateFormMaxLength(formulaName, 50, "#formulaName-error", "Formula name must not exceed 50 characters.")) {
        isValid = false;
        firstInvalidField ??= "#formulaName";
    }

    if (!validateFormRequired(formulaInput, "#formulaInput-error", "Please enter formula.")) {
        isValid = false;
        firstInvalidField ??= "#formulaInput";
    } else if (!validateFormMaxLength(formulaInput, 250, "#formulaInput-error", "Formula must not exceed 250 characters.")) {
        isValid = false;
        firstInvalidField ??= "#formulaInput";
    } else {
        const basicValidationMessage = validateComponentFormula(formulaInput);
        if (basicValidationMessage) {
            $("#formulaInput-error").text(basicValidationMessage);
            isValid = false;
            firstInvalidField ??= "#formulaInput";
        } else {
            try {
                const formulaValidationMessage = await validateComponenntFormulausingNCalc();
                if (formulaValidationMessage) {
                    $("#formulaInput-error").text(formulaValidationMessage);
                    isValid = false;
                    firstInvalidField ??= "#formulaInput";
                } else {
                    $("#formulaInput-error").text("");
                }
            } catch (e) {
                showAlert("danger", e);
                isValid = false;
            }
        }
    }

    if (!isValid && firstInvalidField) {
        $(firstInvalidField).focus();
    }

    return isValid;
}

function validateComponentFormula(formula) { 
    const operators = ["+", "-", "*", "/", "=", "<>", "<", ">", "<=", ">=", "AND", "OR"];
    const logicalOperators = ["AND", "OR"];
    const comparisonOperators = ["=", "<>", "<", ">", "<=", ">="];

    const tokenizeFormula = (formula) => {
        const regex = /<=|>=|<>|[+\-*/=<>()]|\d+(\.\d+)?|AND|OR|[a-zA-Z0-9 _]+/gi;
        return formula.match(regex)?.map(t => t.trim()).filter(Boolean) || [];
    };

    const isOperator = (token) => typeof token === "string" && operators.includes(token.toUpperCase());
    const isLogicalOperator = (token) => typeof token === "string" && logicalOperators.includes(token.toUpperCase());
    const isComparisonOperator = (token) => typeof token === "string" && comparisonOperators.includes(token.toUpperCase());
    const isNumber = (token) => typeof token === "string" && /^[0-9]+(\.[0-9]+)?$/.test(token);
    const isField = (token) => typeof token === "string" && !isOperator(token) && !isNumber(token) && token !== "(" && token !== ")";
    const isValidToken = (token) =>
        typeof token === "string" && (isField(token) || isNumber(token) || isOperator(token) || token === "(" || token === ")");

    const tokens = tokenizeFormula(formula);
    if (tokens.length === 0) return "Formula cannot be empty.";

    let openParens = 0;
    let comparisonCount = 0;

    for (let i = 0; i < tokens.length; i++) {
        const token = tokens[i];
        const prev = tokens[i - 1];
        const next = tokens[i + 1];

        if (!isValidToken(token)) return `Invalid token: "${token}"`;

        if (token === "(") openParens++;
        if (token === ")") openParens--;
        if (openParens < 0) return "Unbalanced parentheses.";

        if ((isField(token) || isNumber(token)) && (isField(next) || isNumber(next))) {
            return "Missing operator between fields or numbers.";
        }

        if (isOperator(token)) {
            if (!prev || !next) return `Operator "${token}" cannot be at the start or end.`;
            if (isOperator(prev)) return `Invalid sequence: "${prev} ${token}"`;
            if (isOperator(next)) return `Invalid sequence: "${token} ${next}"`;

            if (isComparisonOperator(token)) comparisonCount++;
            if (comparisonCount > 1) return "Only one comparison operator allowed.";
        }

        if (token === "(" && next && !(isField(next) || isNumber(next) || next === "(")) {
            return `Invalid token after "(": "${next}"`;
        }

        if (token === ")" && prev && !(isField(prev) || isNumber(prev) || prev === ")")) {
            return `Invalid token before ")": "${prev}"`;
        }

        if ((isField(token) || isNumber(token)) && next === "(") {
            return `Missing operator before "(": "${token} ("`;
        }

        if (token === ")" && (isField(next) || isNumber(next))) {
            return `Missing operator after ")": ") ${next}"`;
        }
    }

    if (openParens !== 0) return "Unbalanced parentheses.";

    return null;  // No error
}
function showComponentFormulaSuggestionDropdown(suggestions) {
    var suggestionBox = $('#suggestionBox');
    suggestionBox.empty();

    suggestions.forEach(function (item) {
        var div = $('<div></div>').addClass('suggestion-item').text(item.formula_Computation);
        div.on('click', function () {
            $('#formulaInput').val(item.formula_Computation);
            hideSuggestionDropdown();
        });
        suggestionBox.append(div);
    });

    var inputOffset = $('#formulaInput').offset();
    suggestionBox.css({
        top: inputOffset.top + $('#formulaInput').outerHeight(),
        left: inputOffset.left,
        width: $('#formulaInput').outerWidth()
    });

    suggestionBox.show();
}
function hideComponentFormulaSuggestionDropdown() {
    $('#suggestionBox').hide();
}
function fetchComponentFormulaSuggestions(searchText) {
    $.ajax({
        url: '/FormulaMaster/GetFormulaSuggestions',
        type: 'POST',
        data: { searchParam: searchText },
        success: function (response) {
            if (response.success && response.data.length > 0) {
                showComponentFormulaSuggestionDropdown(response.data);
            } else {
                hideComponentFormulaSuggestionDropdown();
            }
        },
        error: function () {
            hideComponentFormulaSuggestionDropdown();
        }
    });
}
function updateComponentFormulaHiddenField() {
    var visibleFormula = $("#formulaInput").val();
    // Replace [ ] field names: remove brackets and replace spaces inside fields with underscores
    var hiddenFormula = visibleFormula.replace(/\[([^\]]+)\]/g, function (match, p1) {
        return p1.replace(/\s+/g, "_");
    });
    $("#hiddenFieldWithUnderscores").val(hiddenFormula);
}
function appendToFormulaComponent(value) {
    var formula = $("#formulaInput").val();
    formula += value;

    if (allowedFormulaComponents.includes(value) && !appendedFormulaComponents.includes(value)) {
        appendedFormulaComponents.push(value);

        $("#componentList li").each(function () {
            const itemName = $(this).data("name")?.trim();
            if (itemName === value) {
                $(this).addClass("highlighted-component");
            }
        });
    }

    $("#formulaInput").val(formula.trim());
    updateComponentFormulaHiddenField();
}
////////////////////////////// Common Function for Formula:- End/////////////////////////////////////////
var allowedFormulaComponents = [];
var appendedFormulaComponents = [];
$(document).ready(function () {
//////////////////////////////// MODEL POP-UP Fetch Record:- Start //////////////////////////////////////////////
    $('#addFormula').on('show.bs.modal', function () {
        resetModalForm(
            '#addFormula',
            ['#formulaCode', '#formulaName', '#formulaInput', '#field'],
            ['#formulaCode-error', '#formulaName-error', '#formulaInput-error']
        );
        $('#componentList').empty();
        allowedFormulaComponents = [];

        $.ajax({
            url: '/FormulaMaster/GetPayComponentList',
            type: 'GET',
            success: function (data) {
                if (data.success === false) {
                    $('#componentList').append('<li class="text-danger">' + data.message + '</li>');
                    return;
                }

                data.data.forEach(function (item) {
                    var colorClass = 'text-primary';
                    if (item.earningDeductionType === 1) colorClass = 'text-success';
                    else if (item.earningDeductionType === 2) colorClass = 'text-danger';

                    $('#componentList').append(`
                    <li class="list-group-item field-item" data-id="${item.earningDeduction_Id}" data-name="${item.earningDeductionName}" data-status="${item.isActive}">
                        <span class="${colorClass}">${item.earningDeductionName}</span>
                    </li>
                `);

                    allowedFormulaComponents.push(item.earningDeductionName);
                });
            },
            error: function () {
                $('#componentList').append('<li class="text-danger">Failed to load fields.</li>');
            }
        });
    });

//////////////////////////////// MODEL POP-UP Fetch Record:- End //////////////////////////////////////////////


///////////////////////////////  SAVE formula using Model POP UP:- Start////////////////////////////////////
    $("#btnApplyFormula").on("click", async function () {
        const isFormValid = await validateCompoenntFormulaForm();
        if (!isFormValid) return;

        var finalformulaInput = $("#formulaInput").val();
        finalformulaInput = finalformulaInput.replace(/\[([^\]]+)\]/g, '$1');

        var data = {
            Formula_Id: $("#formula_Id").val(),
            Formula_Code: $("#formulaCode").val(),
            Formula_Computation: finalformulaInput,
            FormulaName: $("#formulaName").val(),
            IsActive: $("#formulaActiveToggle").is(":checked")
        };

        $.ajax({
            url: "/FormulaMaster/AddFormula",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(data),
            success: function (response) {
                console.log(response.resultFormula);
                if (response.success) {                   
                    var newlyInsertedformulaId = response.resultFormula.formula_Id;
                    $("#formulaCode").val('');
                    $("#formulaName").val('');
                    $("#formulaInput").val('');
                    document.activeElement.blur();
                    $("#addFormula").modal('hide');
                    showAlert("success", response.message);                   
                    loadDropdownWithSelectedValue('#formulaDropdown', '/DropDown/FetchFormulaTypeDropdown', function () {
                        setSelectedValueInDropdown('#formulaDropdown', newlyInsertedformulaId);
                    });
                }
                else
                {
                    $("#formulaCode").val('');
                    $("#formulaName").val('');
                    $("#formulaInput").val('');
                    document.activeElement.blur();
                    $("#addFormula").modal('hide');
                    showAlert("danger", response.message);
                }
            },
            error: function (xhr, status, error) {
                alert("Error: " + xhr.responseText);
            }
        });
    });

///////////////////////////////  SAVE formula using Model POP UP:- End////////////////////////////////////

///////////////////////////////  Append Existing to Formula:- Start////////////////////////////////////////////
    $(document).on('click', function (e) {
        if (!$(e.target).closest('#formulaInput, #suggestionBox').length) {
            hideComponentFormulaSuggestionDropdown();
        }
    });
    //$("#formulaInput").on("input", function () {
    //    var searchText = $(this).val();

    //    // Update the hidden field
    //    updateComponentFormulaHiddenField();

    //    // Fetch suggestions if the input length is greater than 2
    //    if (searchText.length > 2) {
    //        fetchComponentFormulaSuggestions(searchText);
    //    } else {
    //        hideComponentFormulaSuggestionDropdown(); // Hide suggestions if the text is too short
    //    }
    //});


    $("#formulaInput").on("input", function () {
        var formula = $(this).val();

        updateComponentFormulaHiddenField();

        // Split formula into components by operators/keywords
        const currentComponents = formula
            .split(/[\+\-\*\/\=\(\)",<>]+|\band\b|\bor\b|\b<=\b|\b>=\b|\b<\b|\b>\b/gi)
            .map(c => c.trim())
            .filter(c => c.length > 0);

        const uniqueComponents = [...new Set(currentComponents)];

        // Clear all highlights
        $('#componentList li').removeClass('highlighted-component');

        // Re-apply highlights only if component name exactly matches
        uniqueComponents.forEach(component => {
            $('#componentList li').each(function () {
                if ($(this).data('name')?.trim() === component) {
                    $(this).addClass('highlighted-component');
                }
            });
        });

        // Update global appended components list to keep in sync with formula
        appendedFormulaComponents = uniqueComponents;

        if (formula.length > 2) {
            fetchComponentFormulaSuggestions(formula);
        } else {
            hideComponentFormulaSuggestionDropdown();
        }
    });

    var activateComponentId = null;
    $(document).on("click", ".formula-btn", function () {
        const value = $(this).text().trim();
        appendToFormulaComponent(value, false); // false means no square brackets for operators/conditions
    });
    $(".operator").on("click", function () {
        const value = $(this).text().trim();
        appendToFormulaComponent(value);
    }); 
    $(document).on("click", ".field-item", function () {
        const field = $(this).text().trim();
        appendToFormulaComponent(field, true);
    });
  
    $("#formulaInput").on("blur", function () {
        var val = $(this).val();
        val = val.replace(/\[|\]/g, ''); // Remove any brackets typed manually
        $(this).val(val);
    });

//////////////////////////////   Append Existing to Formula:- End///////////////////////////////////////////
///////////////////// SORT THE PAY COMPONENT USING "Search available fields":-Start///////////////////////
    $(document).on("input", "#field", function () {
        const searchText = $(this).val().toLowerCase();

        $("#componentList li").each(function () {
            const itemText = $(this).text().toLowerCase();

            if (itemText.includes(searchText)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });
    ///////////////////// SORT THE PAY COMPONENT USING "Search available fields":-End///////////////////////
    /////////////////////////////////////////////// Reset Model POPUP:- Start/////////////////////////////////////////
    function resetModalForm(modalId, fieldSelectors = [], errorSelectors = []) {
        // Clear all text inputs and textareas
        fieldSelectors.forEach(function (selector) {
            $(selector).val('').removeClass('is-invalid');
        });

        // Clear all validation error messages
        errorSelectors.forEach(function (selector) {
            $(selector).text('');
        });

        // Optional: Clear custom dynamic lists (if any)
        $(`${modalId} #componentList`).empty(); // Adjust for your dynamic lists if needed
    }
    /////////////////////////////////////////////// Reset Model POPUP:- End/////////////////////////////////////////

});
