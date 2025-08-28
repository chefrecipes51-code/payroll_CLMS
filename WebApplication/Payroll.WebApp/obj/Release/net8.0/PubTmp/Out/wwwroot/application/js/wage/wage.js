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