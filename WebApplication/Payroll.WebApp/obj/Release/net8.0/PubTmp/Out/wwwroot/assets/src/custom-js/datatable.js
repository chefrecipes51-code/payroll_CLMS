/* 
* Added By:- Harshida Parmar (09-01-'25)
* EXPORT DATA FOR CSV AND EXCEL: - Start */
function exportTableToCSV(tableId, fileName) {
    // Get the DataTable instance
    var table = $('#' + tableId).DataTable();

    // Get all data from the DataTable (including filtered rows)
    var allData = table.rows().data();

    var headers = [];
    var data = [];

    // Get the table headers (excluding Actions, Is Active, and Is Deleted columns)
    $('#' + tableId + ' thead th').each(function (index) {
        var headerText = $(this).text().trim();
        // Exclude Actions, Is Active, and Is Deleted columns
        if (headerText !== 'Actions' && headerText !== 'Is Active' && headerText !== 'Is Deleted') {
            headers.push(headerText);
        }
    });

    // Loop through the data and format it
    allData.each(function (value, index) {
        var rowData = [];
        value.forEach(function (cell, cellIndex) {
            var headerText = $('#' + tableId + ' thead th').eq(cellIndex).text().trim();

            // Exclude Is Active and Is Deleted columns
            if (headerText !== 'Is Active' && headerText !== 'Is Deleted' && headerText !== 'Actions') {
                rowData.push(cell);
            }
        });

        data.push(rowData);
    });

    // Generate CSV content
    var csvContent = headers.join(',') + '\n' + data.map(row => row.join(',')).join('\n');

    // Trigger download
    var blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    var link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = fileName + '.csv';
    link.click();
}

function exportTableToPDF(tableId, fileName) {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF();

    // Get the DataTable instance
    var table = $('#' + tableId).DataTable();

    // Get all data from the DataTable
    var allData = table.rows().data();

    var headers = [];
    var body = [];

    // Get the table headers (excluding Actions, Is Active, and Is Deleted columns)
    $('#' + tableId + ' thead th').each(function (index) {
        var headerText = $(this).text().trim();
        // Exclude Actions, Is Active, and Is Deleted columns
        if (headerText !== 'Actions' && headerText !== 'Is Active' && headerText !== 'Is Deleted') {
            headers.push(headerText);
        }
    });

    // Loop through the data and format it
    allData.each(function (value, index) {
        var rowData = [];
        value.forEach(function (cell, cellIndex) {
            var headerText = $('#' + tableId + ' thead th').eq(cellIndex).text().trim();

            // Exclude Is Active and Is Deleted columns
            if (headerText !== 'Is Active' && headerText !== 'Is Deleted' && headerText !== 'Actions') {
                rowData.push(cell);
            }
        });

        body.push(rowData);
    });

    // Generate PDF content
    doc.text("User List Export", 10, 10);
    doc.autoTable({
        head: [headers],
        body: body,
        theme: 'grid',
        startY: 20
    });

    doc.save(fileName + '.pdf');
}

function exportData(type, tableId) {
    const table = $(`#${tableId}`).DataTable();

    if (!table) {
        console.error(`Table with ID '${tableId}' not found.`);
        return;
    }

    // Perform export logic based on type
    switch (type) {
        case 'csv':
            exportTableToCSV(tableId, tableId + '-export');
            break;
        case 'pdf':
            exportTableToPDF(tableId, tableId + '-export');
            break;
        default:
            console.error(`Unsupported export type: ${type}`);
    }
}
/* EXPORT DATA FOR CSV AND EXCEL: - End */
function makeDataTable(tableId) {
    var searchbox = '<"form-group has-search margin-bottom-search mb-0"f>';
    var _buttons = tableId === "migrate" ? '' : '<"#customButtons.d-flex align-items-center gap-4 me-4">'; // No buttons for "migrate"

    var _language = {
        "info": "<span class='active'>_START_</span>-<span>_END_</span> of <span class='dataTables_info_total'>_TOTAL_</span>",
        "infoFiltered": "",
        "infoEmpty": "",
        "search": "<img src='/assets/img/icons/search.svg' class='' alt='search' width='20' height='20'>",
        "lengthMenu": "Show rows per page _MENU_",
        "paginate": {
            "next": "<img src='/assets/img/icons/next-table-data.svg' width='12' height='12' />",
            "previous": "<img src='/assets/img/icons/prev-table-data.svg' width='12' height='12' />"
        }
    };

    var _dom = `<"d-flex justify-content-between align-items-center p-2"
                    <"d-flex align-items-center"
                        ${searchbox}
                    >
                    <"#last_section.last_section align-items-center d-flex"
                        ${_buttons}<"#dt_length.custom_table_length d-flex align-items-center"l>
                        <"#dt_info"i>
                        <"#dt_paginate.dt_paginate"p>
                    >
                >t`;

    document.addEventListener('DOMContentLoaded', function () {
        // Only add custom buttons if it's not the "migrate" table
        if (tableId !== "migrate") {
            const customButtonsContainer = document.getElementById('customButtons');
            if (customButtonsContainer && customButtonsContainer.children.length === 0) {
                const buttons = [
                    { id: "btn4", class: "btn btn_primary_outline icon-title text-nowrap", value: "Clear Filter" },
                    { id: "btn1", icon: "/assets/img/icons/column-edit.svg", target: "#offcanvas1", tooltip: "Edit Column", iconWidth: 20, iconHeight: 20 },
                    {
                        id: "btn2", icon: "/assets/img/icons/filter.svg", tooltip: "Filter", iconWidth: 20, iconHeight: 20, attributes: {
                            "data-bs-toggle": "modal",
                            "data-bs-target": "#standardFilterModal"
                        }
                    },
                    { id: "btn3", icon: "/assets/img/icons/download.svg", tooltip: "Download Options", iconWidth: 20, iconHeight: 20 }
                ];

                buttons.forEach(button => {
                    if (button.id === "btn3") {
                        // Create a dropdown button
                        const dropdownContainer = document.createElement('div');
                        dropdownContainer.className = 'dropdown';

                        const dropdownButton = document.createElement('button');
                        dropdownButton.id = button.id;
                        dropdownButton.className = 'btn btn_primary_light_icon_sm';
                        dropdownButton.setAttribute('data-bs-toggle', 'dropdown');
                        dropdownButton.setAttribute('aria-expanded', 'false');
                        dropdownButton.setAttribute('title', button.tooltip);
                        dropdownButton.setAttribute('aria-label', button.tooltip);

                        const icon = document.createElement('img');
                        icon.src = button.icon;
                        icon.alt = button.tooltip;
                        icon.width = button.iconWidth;
                        icon.height = button.iconHeight;
                        dropdownButton.appendChild(icon);

                        const dropdownMenu = document.createElement('ul');
                        dropdownMenu.className = 'dropdown-menu';

                        const options = [
                            { icon: "/assets/img/icons/xlsx.svg", text: "XLSX", value: "xlsx" },
                            { text: "CSV", value: "csv", icon: "/assets/img/icons/csv.svg" },
                            { text: "PDF", value: "pdf", icon: "/assets/img/icons/pdf.svg" }
                        ];

                        options.forEach(option => {
                            const listItem = document.createElement('li');
                            const anchor = document.createElement('a');
                            anchor.className = 'dropdown-item export-data d-flex align-items-center flex-row-reverse justify-content-end';
                            anchor.href = '#';

                            // ✅ ADD THESE:
                            anchor.setAttribute('data-type', option.value);
                            anchor.setAttribute('data-table-id', tableId);  // Use the dynamic table ID

                            const optionIcon = document.createElement('img');
                            optionIcon.src = option.icon;
                            optionIcon.alt = option.text;
                            optionIcon.width = 24;
                            optionIcon.height = 24;
                            optionIcon.className = 'me-2';

                            anchor.innerText = option.text;
                            anchor.appendChild(optionIcon);
                            listItem.appendChild(anchor);
                            dropdownMenu.appendChild(listItem);
                        });
                        dropdownContainer.appendChild(dropdownButton);
                        dropdownContainer.appendChild(dropdownMenu);
                        customButtonsContainer.appendChild(dropdownContainer);

                    } else if (button.id === "btn4") {
                        // Clear Filter button (text-based)
                        const btn = document.createElement('button');
                        btn.id = button.id;
                        btn.className = button.class;
                        btn.innerText = button.value;
                        customButtonsContainer.appendChild(btn);

                    } else {
                        // btn1 (offcanvas), btn2 (modal)
                        const btn = document.createElement('button');
                        btn.id = button.id;
                        btn.className = 'btn btn_primary_light_icon_sm';
                        btn.setAttribute('title', button.tooltip);
                        btn.setAttribute('aria-label', button.tooltip);
                        btn.style.border = "none";
                        btn.style.outline = "none";

                        if (button.id === "btn1") {
                            // Offcanvas behavior
                            btn.setAttribute('data-bs-toggle', 'offcanvas');
                            btn.setAttribute('data-bs-target', button.target);
                        } else if (button.id === "btn2") {
                            // Modal behavior
                            btn.setAttribute('data-bs-toggle', 'modal');
                            btn.setAttribute('data-bs-target', '#standardFilterModal');
                        }

                        const icon = document.createElement('img');
                        icon.src = button.icon;
                        icon.alt = button.tooltip;
                        icon.width = button.iconWidth;
                        icon.height = button.iconHeight;
                        btn.appendChild(icon);

                        customButtonsContainer.appendChild(btn);
                    }
                });

            }
        }

    });

    // Initialize the DataTable
    table = $('#' + tableId).DataTable({
        scrollY: true,
        fixedHeader: true,
        className: 'mdl-data-table__cell--non-numeric',
        language: _language,
        dom: _dom,
        "columnDefs": [
            { "orderable": false, "targets": 0 }
        ],
        drawCallback: function (settings) {
            // Update checkbox states when clicked
        }
    });

    // Adjust table header
    setTimeout(function () {
        $("#" + tableId + "_wrapper #dataTables_tbl_header").insertBefore($("#tableResponsive"));
    }, 100);
}


// Initialize DataTables for specific tables
makeDataTable("user-list");
makeDataTable("roles-permission");//Added By Priyanshi
makeDataTable("company-correspondances-list"); //Added By Harshida
makeDataTable("company-statutory-list"); //Added By Harshida
makeDataTable("formula-list"); //Added By Harshida
makeDataTable("ptaxslab-list"); //Added By Harshida
makeDataTable("approval-list"); //Added By Harshida
makeDataTable("area-master-list");//Added By Priyanshi
makeDataTable("tblLocation");//Added By Priyanshi
makeDataTable("department-master-list");//Added By Priyanshi
makeDataTable("accounting-head-list"); //Added By Harshida
makeDataTable("gl-group-list"); //Added By Harshida
makeDataTable("validatecontractors-list"); //Added By Harshida
makeDataTable("wagereport-list"); //Added By Harshida
makeDataTable("fineregisterreport-list"); //Added By Priyanshi
makeDataTable("salaryslipreport-list"); //Added By Priyanshi
makeDataTable("overtimereport-list"); //Added By Priyanshi
makeDataTable("loanandadvancereport-list"); //Added By Priyanshi
//makeDataTable("validatepay-list"); //Added By Harshida
//makeDataTable("validatecompliance-list"); //Added By Harshida
//makeDataTable("validateattendance-list"); //Added By Harshida

makeDataTable("lossdamage-list"); //Added By Harshida

makeDataTable("area-master-list");
makeDataTable("tblLocation");
makeDataTable("department-master-list");
makeDataTable("company-list"); //Added By Harshida
makeDataTable("subsidiarymaster-list"); //Added By Krunali
makeDataTable("pay-component-list"); //Added By Priyanshi
makeDataTable("pay-grade-list"); //Added By Priyanshi
makeDataTable("assign-tax-regime-list"); //Added By Priyanshi
makeDataTable("approval-list"); //Added By chirag
makeDataTable("contractor-list"); //Added By forum 
makeDataTable("grade-entity-mapping-list"); //Added By Priyanshi 
//makeDataTable("roles-permission");
// ✅ Event binding for dynamically created export buttons

