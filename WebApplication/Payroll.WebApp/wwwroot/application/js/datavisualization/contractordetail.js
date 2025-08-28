$(document).ready(function () {
    //LoadContractor();

    debugger;
    // Get all table rows from the contractor table
    const $rows = $('#contractor-list tbody tr');

    // Total contractor count
    const totalCount = $rows.length;

    // Count active and inactive contractors
    const activeCount = $rows.filter(function () {
        return $(this).data('isactive') === true;
    }).length;

    const inactiveCount = $rows.filter(function () {
        return $(this).data('isactive') === false;
    }).length;

    // Update the counts in UI
    $('#totalcontractorCount span').text(totalCount.toString().padStart(2, '0'));
    $('#totalActivecontractorCount span').text(activeCount.toString().padStart(2, '0'));
    $('#totalInactivecontractorCount span').text(inactiveCount.toString().padStart(2, '0'));
    $(document).on('click', '#resetContractorBtn', function () {
        debugger;
        // Get all table rows from the contractor table
        const $rows = $('#contractorTable tbody tr');

        // Total contractor count
        const totalCount = $rows.length;

        // Count active and inactive contractors
        const activeCount = $rows.filter(function () {
            return $(this).data('isactive') === true;
        }).length;

        const inactiveCount = $rows.filter(function () {
            return $(this).data('isactive') === false;
        }).length;

        // Update the counts in UI
        $('#totalcontractorCount span').text(totalCount.toString().padStart(2, '0'));
        $('#totalActivecontractorCount span').text(activeCount.toString().padStart(2, '0'));
        $('#totalInactivecontractorCount span').text(inactiveCount.toString().padStart(2, '0'));
    });
    $(document).on('click', '.redirectToContractorProfile', function () {
        var contractorId = $(this).data('contractorid');
        console.log(contractorId);
        fetch('/DataVisualisation/EncryptId?id=' + encodeURIComponent(contractorId))
            .then(response => response.text())
            .then(encryptedId => {
                window.location.href = "/DataVisualisation/ContractorProfile?contractorId=" + encodeURIComponent(encryptedId);
            })
            .catch(error => console.error('Encryption error:', error));
    });
});

//function LoadContractor() {
//    $.ajax({
//        url: '/DataVisualisation/ContractorList',
//        type: 'GET',
//        success: function (result) {
//            var tableId = "contractor-list";
//            if ($.fn.DataTable && $.fn.dataTable.isDataTable(`#${tableId}`)) {
//                $(`#${tableId}`).DataTable().destroy();
//            }
//            if (result.count > 0) {
//                makeDataTable(tableId);
//            } else {
//                // Initialize the variable before using it
//                var $table = $(`#${tableId}`);
//                // Manually add "no data" message if no rows
//                $table.find('tbody').html(
//                    '<tr><td colspan="9" class="text-center">No data available.</td></tr>'
//                );
//            }
//        },
//        error: function () {
//            $('#payGradeContainer').html('<div class="text-danger text-center">Failed to load data.</div>');
//        }

//    });


//}
