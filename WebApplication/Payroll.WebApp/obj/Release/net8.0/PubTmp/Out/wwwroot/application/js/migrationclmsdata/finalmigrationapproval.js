/// <summary>
/// Developer Name :- Abhishek Yadav
///  Created Date  :- 03-APR-2025
/// </summary>


// Button loader on click
$('.loader-dot-pulse').on('click', function () {
    var self = this;
    $(this).addClass('btn__dots--loading');
    $(this).append('<span class="btn__dots"><i></i><i></i><i></i></span>');
    setTimeout(function () {
        $(self).removeClass('btn__dots--loading');
        $(self).find('.btn__dots').remove();
    }, 2000);
});

// Dropdown active state handling
const dropdownMenu = document.getElementById('dropdownMenu');
if (dropdownMenu) {
    dropdownMenu.addEventListener('click', (event) => {
        if (event.target.classList.contains('dropdown-item')) {
            dropdownMenu.querySelectorAll('.dropdown-item').forEach((item) => {
                item.classList.remove('active');
            });
            event.target.classList.add('active');
        }
    });
}

$(document).ready(function () {
    $('#TemlateSeviceId').change(function () {
        var selectedService = $('#TemlateSeviceId option:selected').text();
        debugger;
        if (selectedService) {
            $.ajax({
                url: '/FinalDataApproval/GetServiceData',
                type: 'GET',
                data: { serviceName: selectedService },
                success: function (response) {
                    if (response.success) {
                        $('.no-data-message').hide();
                        /* bindDataTable(response.columns, response.data);*/
                        //  Extract column names dynamically from the first row
                        var columns = response.data.length > 0 ? Object.keys(response.data[0]) : [];

                        //  Pass both columns and data to bindDataTable
                        bindDataTable(columns, response.data); 
                        if (response.returnCount > 0) {
                            $('#PendingReturn').show();
                        }
                        else {
                            $('#PendingReturn').hide();
                        }
                    } else {
                        var $table = $("#user-list");

                        // Destroy existing DataTable instance
                        if ($.fn.DataTable.isDataTable($table)) {
                            $table.DataTable().destroy();
                        }

                        var $thead = $table.find("thead tr");
                        var $tbody = $table.find("tbody");

                        $thead.empty();
                        $tbody.empty();
                        $('.no-data-message').show();
                        showAlert("warning", "No Data Available");
                    }
                },
                error: function () {
                    alert("Error fetching data");
                }
            });
        }
    });
});

// Function to bind DataTable
function bindDataTable(columns, data) {
    var $table = $("#user-list");

    // Destroy existing DataTable instance
    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().destroy();
    }

    var $thead = $table.find("thead tr");
    var $tbody = $table.find("tbody");

    $thead.empty();
    $tbody.empty();

    // Add Select All Checkbox Column
    $thead.append('<th class="sticky_cell checkbox"><div class="input-group"><input id="checklist6" class="form-check-input mt-0" type="checkbox" value=""></div></th>');

    $.each(columns, function (index, columnName) {
        if (columnName.toLowerCase() != 'id') {
            $thead.append('<th>' + columnName + '</th>');
        }
    });

    //$thead.append('<th class="sticky_cell">Actions</th>');

    $.each(data, function (index, item) {
        var recordId = item["id"] || item["RecordId"] || item["UniqueId"] || "";
        var row = `<tr data-id="${recordId}"><td class="sticky_cell checkbox">
                      <div class="input-group">
                          <input type="checkbox" class="row-checkbox form-check-input mt-0">
                      </div>
                   </td>`;

        $.each(columns, function (i, columnName) {
            if (columnName.toLowerCase() != 'id') {
                var cellValue = item[columnName];

                // **Fix: Ensure null, undefined, and objects are replaced with blank**
                if (cellValue === null || cellValue === undefined || typeof cellValue === "object") {
                    cellValue = ''; // Replace with blank
                }

                row += `<td>${cellValue}</td>`;
            }
        });
        //row += `<td class="sticky_cell">
        //            <div class="table-button-group">
        //                <button class="btn btn-icon-sm btn-icon-success">
        //                    <span class="sprite-icons check-round-success-bg"></span>
        //                </button>
        //                <button class="btn btn-icon-sm btn-icon-danger">
        //                    <span class="sprite-icons close-round-danger-bg"></span>
        //                </button>
        //            </div>
        //        </td>`;

        row += '</tr>';
        $tbody.append(row);
    });

    var dataTable = $table.DataTable({
        responsive: true,
        autoWidth: false,
        order: [[1, 'asc']],
        searching: true,
        paging: false,
        info: false,
        scrollX: true,
        fixedHeader: { header: true, footer: false },
        lengthChange: false,
        dom: '<"row"<"col-sm-6 total-entries"><"col-sm-6 text-right"f>>rt',
        language: { search: "", searchPlaceholder: "Search here..." },
        columnDefs: [{ targets: 0, orderable: false }
        ]
    });

    let totalEntries = data.length;
    $(".total-entries").html(`Total Records: ${totalEntries}`);

    // Select All Checkbox Handling
    $("#checklist6").on("change", function () {
        $(".row-checkbox").prop("checked", this.checked);
        updateButtonLabel();
    });

    // Individual Checkbox Handling
    $table.on('change', '.row-checkbox', function () {

        var totalRows = $(".row-checkbox").length;
        var selectedCount = $(".row-checkbox:checked").length;
        $("#checklist6").prop("checked", totalRows === selectedCount);
        updateButtonLabel();
    });

    function updateButtonLabel() {
        var selectedCount = $('.row-checkbox:checked').length;
        var totalRows = $('.row-checkbox').length;

        if (selectedCount === totalRows) {
            $(".btn-success-light.icon-title").html('<span class="sprite-icons check-round-success-bg"></span> Accept All');
            $(".btn-danger-blush.icon-title").html('<span class="sprite-icons close-round-danger-bg"></span> Return All');
            $("#remove-selected").html('Clear All');
        } else if (selectedCount > 0) {
            $(".btn-success-light.icon-title").html(`<span class="sprite-icons check-round-success-bg"></span> Accept Selected (${selectedCount})`);
            $(".btn-danger-blush.icon-title").html(`<span class="sprite-icons close-round-danger-bg"></span> Return Selected (${selectedCount})`);
            $("#remove-selected").html(`Clear Selected (${selectedCount})`);

        } else {
            $(".btn-success-light.icon-title").html('<span class="sprite-icons check-round-success-bg"></span> Accept');
            $(".btn-danger-blush.icon-title").html('<span class="sprite-icons close-round-danger-bg"></span> Return');
            $("#remove-selected").html('Clear');
        }
    }
}





// Move staging to final table in Acceptance Case
$(document).ready(function () {
    $("#btnVerifySelected").on("click", function () {
        UncheckSelection();
        var selectedIds = [];
        $(".row-checkbox:checked").each(function () {
            let row = $(this).closest("tr"); // Find the closest <tr>
            let recordId = row.data("id"); // Get data-id attribute
            if (recordId) {
                selectedIds.push(recordId);
            }
        });

        if (selectedIds.length === 0) {
            alert("Please select at least one record.");
            return;
        }

        var serviceId = $('#TemlateSeviceId').val();

        $.ajax({
            url: "/FinalDataApproval/VerifyAndMoveRecords",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ serviceId: serviceId, selectedIds: selectedIds }),
            success: function (response) {
                let alertType = response.messageType == 1 ? "success" : "danger";
                showAlert(alertType, response.message);

                let selectedService = $('#TemlateSeviceId option:selected').text();

                //Update DatataTable after status update

                if (selectedService) {
                    $.ajax({
                        url: '/FinalDataApproval/GetServiceData',
                        type: 'GET',
                        data: { serviceName: selectedService },
                        success: function (response) {
                            if (response.success) {
                                $('.no-data-message').hide();
                                bindDataTable(response.columns, response.data);
                            } else {
                                $('.no-data-message').show();
                                var $table = $("#user-list");

                                // Destroy existing DataTable instance
                                if ($.fn.DataTable.isDataTable($table)) {
                                    $table.DataTable().destroy();
                                }

                                var $thead = $table.find("thead tr");
                                var $tbody = $table.find("tbody");

                                $thead.empty();
                                $tbody.empty();

                                showAlert("warning", "No Data Available");
                                //var selectedService = $('#TemlateSeviceId option:selected').text();
                                //$()
                            }
                        },
                        error: function () {
                            alert("Error fetching data");
                        }
                    });
                }
            },
            error: function () {
                alert("Failed to process request. Please try again.");
            }
        });
    });
});

// Update status as return to staging
$(document).ready(function () {
    $("#btnReturnSelected").on("click", function () {
        UncheckSelection();
        var selectedIds = [];
        $(".row-checkbox:checked").each(function () {
            let row = $(this).closest("tr"); // Find the closest <tr>
            let recordId = row.data("id"); // Get data-id attribute
            if (recordId) {
                selectedIds.push(recordId);
            }
        });

        if (selectedIds.length === 0) {
            alert("Please select at least one record.");
            return;
        }

        var serviceId = $('#TemlateSeviceId').val();

        $.ajax({
            url: "/FinalDataApproval/UpdateReturnStatus",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ serviceId: serviceId, selectedIds: selectedIds }),
            success: function (response) {
                let alertType = response.messageType == 1 ? "success" : "danger";
                showAlert(alertType, response.message);
                let selectedService = $('#TemlateSeviceId option:selected').text();

                console.log($('#TemlateSeviceId option:selected').text());

                //Update DatataTable after status update

                if (selectedService) {
                    $.ajax({
                        url: '/FinalDataApproval/GetServiceData',
                        type: 'GET',
                        data: { serviceName: selectedService },
                        success: function (response) {
                            if (response.success) {
                                $('.no-data-message').hide();
                                bindDataTable(response.columns, response.data);
                                $('#PendingReturn').show();
                            } else {
                                var $table = $("#user-list");

                                // Destroy existing DataTable instance
                                if ($.fn.DataTable.isDataTable($table)) {
                                    $table.DataTable().destroy();
                                }

                                var $thead = $table.find("thead tr");
                                var $tbody = $table.find("tbody");

                                $thead.empty();
                                $tbody.empty();
                                $('.no-data-message').show();
                                showAlert("warning", "No Data Available");

                            }
                        },
                        error: function () {
                            alert("Error fetching data");
                        }
                    });
                }
            },
            error: function () {
                alert("Failed to process request. Please try again.");
            }
        });
    });
});

$("#remove-selected").on("click", function () {
    $(".row-checkbox:checked").prop("checked", false);
    $("#checklist6").prop("checked", false);
    UncheckSelection();
});

//Change button text while uncheck checbox from list
function UncheckSelection() {
    $("#remove-selected").html('Clear');
    $(".btn-success-light.icon-title").html('<span class="sprite-icons check-round-success-bg"></span> Accept');
    $(".btn-danger-blush.icon-title").html('<span class="sprite-icons close-round-danger-bg"></span> Return');
}

$(document).ready(function () {
    // When "Download Returned Data" button is clicked
    $('#PendingReturn').on('click', function () {
        debugger;
        var selectedServiceId = $('#TemlateSeviceId').val();
        var SelectedserviceName = $('#TemlateSeviceId option:selected').text();

        if (!selectedServiceId) {
            alert("Please select a service before downloading.");
            return;
        }

        //var apiUrl = "/FinalDataApproval/DownloadReturnedData?serviceId=" + encodeURIComponent(selectedServiceId) + "&serviceName=" + encodeURIComponent(SelectedserviceName);
        var apiUrl = "/FinalDataApproval/DownloadReturnedData?serviceId="
            + encodeURIComponent(selectedServiceId)
            + "&serviceName="
            + encodeURIComponent(SelectedserviceName);

        $(this).hide();
        // Trigger file download
        window.location.href = apiUrl;
    });
});

