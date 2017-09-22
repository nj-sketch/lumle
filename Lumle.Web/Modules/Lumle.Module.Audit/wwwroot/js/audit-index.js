var nowTemp = new Date();
var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);
var startDate = $("#dp1").val();
var endDate = $("#dp2").val();
var module = $("#ModuleSearch").val();
var action = $("#ActionSearch").val();
var username = $("#UserSearch").val();
var field = $("#FieldSearch").val();
var table;
var rootDir;

$(function () {
    $("#auditDataTable").DataTable().clear().destroy();
   
    AuditReport();
    $(".dataTables_filter input").attr("placeholder", "Search...");
    $("#auditDataTable tbody").on("click",
        ".btn-info",
        function (e) {
            var keyFieldId = $(this).attr("keyfield");
            var tableName = $(this).attr("tablename");
            var auditId = $(this).attr("auditId");

            // hide the view button
            $(this).hide();
            var tr = $(this).parents("tr");
            $("#moduleTitle").html(tableName);
            tr.find("img").show();
            //var auditBtnId = $(this).prop("id");
            GetAuditHistory(keyFieldId, tableName, auditId);
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
        $("#auditDataTable").DataTable().clear().destroy();
        AuditReport();
    });

    $("#ModuleSearch").change(function () {
        module = $("#ModuleSearch").val();
        $("#auditDataTable").DataTable().clear().destroy();
        AuditReport();
    });

    $("#ActionSearch").change(function () {
        action = $("#ActionSearch").val();
        $("#auditDataTable").DataTable().clear().destroy();
        AuditReport();
    });

    $("#UserSearch").change(function () {
        username = $("#UserSearch").val();
        $("#auditDataTable").DataTable().clear().destroy();
        AuditReport();
    });

    $("#FieldSearch").change(function () {
        field = $("#FieldSearch").val();
        $("#auditDataTable").DataTable().clear().destroy();
        AuditReport();
    });

});

function AuditReport() {
    table = $("#auditDataTable").DataTable({
        "language": {
            "url": languageFileUrl
        },
        "serverSide": true,
        "ajax": {
            "type": "POST",
            "url": rootDir + "audit/auditlog/DataHandler",
            "contentType": "application/json; charset=utf-8",
            "headers": { "RequestVerificationToken": token },
            "data": function (data) {
                data.ModuleName = module;
                data.ActionName = action;
                data.StartDate = startDate;
                data.EndDate = endDate;
                data.UserName = username;
                data.FieldName = field;
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
                "searchable": true,
                "orderable": true,
                "targets": 4,
                "width": "10%"
            },
            {
                "searchable": false,
                "orderable": false,
                "targets": 5
            },
            {
                "searchable": false,
                "orderable": false,
                "targets": 6,
                "width": "5%",
                "border": true
            }
        ],
        "columns": [
            { "data": "sn" },
            { "data": "tableName" },
            { "data": "userId" },
            { "data": "auditType" },
            { "data": "convertedCreatedDate" },
            { "data": "auditSummary" },
            {
                "render": toolButton,
                "searchable": false,
                "orderable": false
            }
        ],
        "order": [1, "asc"]
    });
}


var toolButton = function (type, full, meta) {
    var keyField = meta.keyField;
    var tableName = meta.tableName;
    var auditId = meta.id;
    var btn =
        '<button id ="auditBtn' + auditId + '"class="btn btn-info" keyField="' + keyField + '" tableName="' + tableName + '" auditId="' + auditId + '">' +
            "<i class='glyph-icon icon-eye'></i>" +
            "</button>" + '<img style="width: 30px;display:none" src="' + rootDir + 'assets/images/spinner/loader-dark.gif" alt="loading..." display="none">';

    return btn;
}

function GetAuditHistory(auditLogId, tableName, auditId) {
    $("#audit").html("");
    var logWrapper = "";
    var url = rootDir + "audit/auditlog/report?id=" + auditLogId + "&tn=" + tableName;
    $.getJSON(url, function (auditTrail) {
        for (var i = 0; i < auditTrail.length; i++) {
            logWrapper = logWrapper + "<div class='col-md-12 panel-heading custom-audit-heading' data-toggle='collapse' data-parent='#accordion' href='#collapseOne" + i + "'" + "aria-expanded='true'><i class='glyph-icon icon-angle-down audit-btn-arr'></i>";
            if (auditId == auditTrail[i].auditId) {
                logWrapper = logWrapper +
                    "<span class='audit-title col-md-4'><i class='glyph-icon icon-asterisk'></i> Event Date: " +
                    auditTrail[i].createdDate +
                    "</span>";
            } else {
                logWrapper = logWrapper + "<span class='audit-title col-md-4'> Event Date: " + auditTrail[i].createdDate + "</span>";
            }
            logWrapper = logWrapper + "<span class='audit-title col-md-4'> Action By: " + auditTrail[i].actionPerformedBy + "</span>";
            logWrapper = logWrapper + "<span class='audit-title col-md-4 '> Action Type: " + auditTrail[i].auditActionTypeName + "</span></div>";
            logWrapper = logWrapper + "<div class='panel-collapse collapse panel-mb in' aria-expanded='true' id='collapseOne" + i + "'" + ">";
            logWrapper = logWrapper + " <table class='table table-bordered table-striped table-condensed table-wdt'>";
            logWrapper = logWrapper + "<tr class='text-warnings'><td class='orange-text'>Field name</td><td class='blue-text'>Before change</td><td class='green-text'>After change</td></tr>";

            if (auditTrail[i].auditInfos === undefined || auditTrail[i].auditInfos.length === 0) {
                logWrapper = logWrapper + "<tr>";
                logWrapper = logWrapper + "<td> Role Name: " + auditTrail[i].auditInfo.fieldName + "</td>";
                logWrapper = logWrapper + "<td>" + htmlHelper(formatString(auditTrail[i].auditInfo.valueBefore)) + "</td>";
                logWrapper = logWrapper + "<td>" + htmlHelper(formatString(auditTrail[i].auditInfo.valueAfter)) + "</td>";
                logWrapper = logWrapper + "</tr>";

            } else {
                for (var j = 0; j < auditTrail[i].auditInfos.length; j++) {
                    logWrapper = logWrapper + "<tr>";
                    logWrapper = logWrapper + "<td>" + auditTrail[i].auditInfos[j].fieldName + "</td>";
                    logWrapper = logWrapper + "<td>" + htmlHelper(formatNullValue(auditTrail[i].auditInfos[j].valueBefore)) + "</td>";
                    logWrapper = logWrapper + "<td>" + htmlHelper(formatNullValue(auditTrail[i].auditInfos[j].valueAfter)) + "</td>";
                    logWrapper = logWrapper + "</tr>";
                }
            }

            logWrapper = logWrapper + "</table></div>";
        }
        $("#audit").html(logWrapper);
        $("#auditModal").modal("show");
        $("#auditBtn" + auditId).next("img").hide();
        $("#auditBtn" + auditId).show();


    });
}

function formatString(value) {
    var regex = /","/g;
    var subst = ", ";
    var regex2 = /["\[\]]/g;
    var subst2 = "";

    var result = value.replace(regex, subst);

    return result.replace(regex2, subst2);
}


function formatNullValue(value) {
    var regex = /(\([a-zA-Z]+\))/g;
    var subst = "N/A";

    var result = value.replace(regex, subst);
    return result;
}

function htmlHelper(value) {
    return value.replace(/</g, "&lt;").replace(/>/g, "&gt;");
}
