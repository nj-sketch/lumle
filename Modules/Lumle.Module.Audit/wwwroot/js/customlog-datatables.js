var startDate = $("#dp1").val();
var endDate = $("#dp2").val();
var username = $('#UsernameSearch').val();
var logLevel = $('#LogLevelSearch').val();
var userAgent = $('#BrowserSearch').val();
var table;

$(function () {
    $('#customLogDataTable').DataTable().clear().destroy();
    CustomLogReport();

    $(".dataTables_filter input").attr("placeholder", "Search...");

});

$('#UsernameSearch').change(function () {
    username = $('#UsernameSearch').val();
    $('#customLogDataTable').DataTable().clear().destroy();
    CustomLogReport();
});

$('#LogLevelSearch').change(function () {
    logLevel = $('#LogLevelSearch').val();
    $('#customLogDataTable').DataTable().clear().destroy();
    CustomLogReport();
});

$('#BrowserSearch').change(function () {
    userAgent = $('#BrowserSearch').val();
    $('#customLogDataTable').DataTable().clear().destroy();
    CustomLogReport();
});

// Datepicker filters
var checkin = $('#dp1').datepicker({
    autoclose: true,
    todayHighlight: true

}).on('changeDate', function (ev) {
    startDate = $("#dp1").val();
    if (ev.date.valueOf() > checkout.datepicker("getDate").valueOf() || !checkout.datepicker("getDate").valueOf()) {

        var newDate = new Date(ev.date);
        newDate.setDate(newDate.getDate() + 1);
        checkout.datepicker("update", newDate);

    }
    $('#dp2')[0].focus();
});

var checkout = $('#dp2').datepicker({
    beforeShowDay: function (date) {
        if (!checkin.datepicker("getDate").valueOf()) {
            return date.valueOf() >= new Date().valueOf();
        } else {
            return date.valueOf() > checkin.datepicker("getDate").valueOf();
        }
    },
    autoclose: true

}).on('changeDate', function (ev) {
    endDate = $("#dp2").val();
    $("#customLogDataTable").DataTable().clear().destroy();
    CustomLogReport();
});

function CustomLogReport() {
    table = $("#customLogDataTable").DataTable({
        "language": {
            "url": languageFileUrl
        },
        "serverSide": true,
        "ajax": {
            "type": "POST",
            "url": rootDir + "audit/CustomLog/DataHandler",
            "contentType": "application/json; charset=utf-8",
            "data": function (data) {
                data.StartDate = startDate;
                data.EndDate = endDate;
                data.Username = username;
                data.LogLevel = logLevel;
                data.userAgent = userAgent;
                return data = JSON.stringify(data);
            },
            "error": function (xhr) {
                var response = JSON.parse(xhr.getResponseHeader('X-Responded-JSON'));
                if (response.status === 401) {
                    window.location.href = "/login?ReturnUrl=%2Fauthorization/User";
                } else {
                    alert(xhr.error);
                }
            }
        },
        "processing": true,
        "paging": true,
        "deferRender": true,
        "columnDefs": [
            {
                "searchable": false,
                "orderable": false,
                "targets": 0,
                "width": "3%"
            },
            {
                "searchable": true,
                "orderable": true,
                "targets": 1,
                "width": "10%"
            },
            {
                "searchable": true,
                "orderable": true,
                "targets": 2,
                "width": "10%"
            },
            {
                "searchable": true,
                "orderable": false,
                "targets": 3,
                "width": "10%"
            },
            {
                "searchable": false,
                "orderable": true,
                "targets": 4,
                "width": "10%"
            },
            {
                "searchable": true,
                "orderable": false,
                "targets": 5,
                "width": "5%"
            },
            {
                "searchable": false,
                "orderable": false,
                "targets": 6,
                "border": true
            }
        ],
        "columns": [
            { "data": "sn" },
            { "data": "username" },
            { "data": "remoteAddress" },
            { "data": "level" },
            { "data": "convertedCreatedDate" },
            { "data": "userAgent" },
            { "data": "message" }
        ],
        "order": [1, "asc"]
    });
}

