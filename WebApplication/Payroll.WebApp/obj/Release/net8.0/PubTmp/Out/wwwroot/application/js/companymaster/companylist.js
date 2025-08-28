/// <summary>
/// Developer Name :- Harshida Parmar
///  Created Date  :- 17-Feb-2025
///  IMP NOTE      :- IF any function shows "0 references" then still do not remove because we are calling that
///                   function from BUTTON CLICK OR in MODEL-POP-UP.
/// </summary>

$(document).ready(function () {
    document.querySelectorAll('.redirectFromCompanyList').forEach(button => {
        button.addEventListener('click', function () {
            var companyId = this.getAttribute('data-companyid');

            fetch('/Company/EncryptId?id=' + encodeURIComponent(companyId))
                .then(response => response.text())
                .then(encryptedId => {
                    window.location.href = "/Company/Index?companyId=" + encodeURIComponent(encryptedId);
                })
                .catch(error => console.error('Encryption error:', error));
        });
    });
});
