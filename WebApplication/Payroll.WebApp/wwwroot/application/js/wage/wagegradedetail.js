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


