var loggedUser = "";
var loggedUserRolePriority = "";
$(function () {
    var table;

    loggedUser = $("#loggedUser").attr("value").trim();
    loggedUserRolePriority = parseInt($("#loggedUserRolePriority").attr("value").trim());
    
    userTable();
   
    $(".dataTables_filter input").attr("placeholder", "Search...");

    $("input[type=search]").keyup(function (e) {
        var searchValue = $(this).val();
        if (searchValue.trim() === "") {
            $("#exportPdf").attr('href', rootDir + "authorization/User/exportpdf");
            $("#exportExcel").attr('href', rootDir + "authorization/User/exportexcel");
        }
        else {
            $("#exportPdf").attr('href', rootDir + "authorization/User/exportpdf/" + searchValue);
            $("#exportExcel").attr('href', rootDir + "authorization/User/exportexcel/" + searchValue);
        }

        $("#userDataTable").DataTable().empty().destroy();
        userTable();
    });

    $("#userDataTable tbody").on("click", ".btn-resend", function (e) {
        e.preventDefault();
        var tr = $(this).parents("tr");
        var data = table.row(tr).data();
        var userId = data.id;

        $(this).hide();
        tr.find("img").show();
        var token = $("input[type=hidden][name=__RequestVerificationToken]").val();
        $.ajax({
            type: "POST",
            url: rootDir + "authorization/User/ResendActivationEmail",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(userId),
            headers: {
                "RequestVerificationToken": token
            },
            success: function (response) {
                if (response.success) {
                    tr.find("button").show();
                    tr.find("img").hide();
                    displaySuccess(response.message,response.messageTitle);
                }
            },
            error: function () {
                tr.find("button").show();
                tr.find("img").hide();
                displayError("Opps. something went wrong. Please try again.");
            }
        });
    });

    function userTable() {
        table = $("#userDataTable").DataTable({
            "language": {
                "url": languageFileUrl
            },
            "serverSide": true,
            "ajax": {
                "type": "POST",
                "url": rootDir + "authorization/User/DataHandler",
                "contentType": "application/json; charset=utf-8",
                "headers": { "RequestVerificationToken": token },
                "data": function (data) {
                    return data = JSON.stringify(data);
                },
                "error": function (xhr) {
                    var response = JSON.parse(xhr.getResponseHeader('X-Responded-JSON'));
                    //if (response.status === 401) {
                    //    window.location.href = rootDir + "login?ReturnUrl=%2Fauthorization/User";
                    //}
                    //else {
                    //    //alert(xhr.error);
                    //}
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
                    "width": "5%"
                },
                {
                    "searchable": false,
                    "orderable": false,
                    "visible": false,
                    "targets": 1
                },
                {
                    "searchable": false,
                    "orderable": false,
                    "width": "15%",
                    "targets": 4
                },
                {
                    "searchable": false,
                    "targets": 5,
                    "width": "10%"
                },
                {
                    "searchable": false,
                    "targets": 6,
                    "width": "15%"
                }
            ],
            "columns": [
                { "data": "sn" },
                { "data": "id" },
                { "data": "name", "render": $.fn.dataTable.render.text() },
                { "data": "email" },
                { "data": "roleName" },
                { "data": "accountStatus", "render": renderStatus },
                { "data": "emailConfirmed", "render": renderEmailConfirmed },
                {
                    "data": "id", "title": "Action", "render": actionButton,
                    "searchable": false,
                    "orderable": false,
                    "visible": hasPermissionForUpdate === true ? true : hasPermissionForDelete === true ? true : false
                }
            ],
            "order": [1, "asc"]
        });
    }

    $("#exportPdf").attr('target', "_blank");
    $("#exportExcel").attr('target', "_blank");
});


var renderStatus = function (data) {
    if (data === 1) {
        return '<span class="bs-label label-success">Active</span>';
    }
    else {
        return '<span class="bs-label label-danger">Inactive</span>';
    }
}
var renderEmailConfirmed = function (data) {
    if (data) {
        return '<span class="bs-label label-success"><i class="glyph-icon tooltip-button icon-check-circle" title="Confirmed" data-original-title="Confirmed"></i> Confirmed</span>';
    }
    else {
        return '<span class="bs-label bg-orange alert-icon"><i class="glyph-icon tooltip-button icon-warning" title="Not Confirmed" data-original-title="Not Confirmed"></i> Not Confirmed </span>';
    }
}
var actionButton = function (data, type, full, meta) {
    var modalId = "deletdModel" + data;
    var actionBtn = "";

    //Check priority
    var currentUserRolePriority = parseInt(full.rolePriority);
    if (loggedUserRolePriority <= currentUserRolePriority) {
        if (hasPermissionForUpdate) {
            actionBtn = '<a href="' + rootDir + 'authorization/User/edit/' + data + '"class="btn btn-info btn-xs"><i class="glyph-icon icon-pencil"></i> ' + editButtonDisplayName + '</a>';
        }
        if (hasPermissionForDelete) {
            if (loggedUser !== full.name.trim()) {
                actionBtn += '&nbsp;&nbsp;<button class="btn btn-danger btn-xs" data-toggle="modal" data-target="#' + modalId +
                                        '"><i class="glyph-icon icon-trash-o"></i> ' + deleteButtonDisplayName + '</button>';
                actionBtn += deleteModalSection(modalId, data, full.name);
            }
        }
    }

    if (!full.emailConfirmed) {
        actionBtn += '&nbsp;&nbsp<button class="btn btn-info btn-xs btn-resend"><i class="glyph-icon icon-refresh"></i> ' + resendButtonDisplayName + '</button><img style="width: 30px;display:none" src="' + rootDir + 'assets/images/spinner/loader-dark.gif" alt="Sending..." display="none">';
    }
    return actionBtn;
}
//delete modal section
var deleteModalSection = function (modalId, userId, userName) {
    var modal = '<div id="' + modalId + '"class="modal fade in"><div class="modal-dialog" style="width:40% !important; margin-top:10% !important;">' +
                '<div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-hidden="true">x</button>' +
                '<h3 class="modal-title">Delete User</h3></div><form action="' + rootDir + 'authorization/User/delete/' + userId + '" role="form" method="post">' +
                '<div class="modal-body">Are you sure you want to delete this record ?</div>' +
                '<div class="modal-footer"><button data-dismiss="modal" class="btn btn-default" type="button">' + cancelButtonDisplayName + '</button>' +
                '<button class="btn btn-success" type="submit">' + deleteButtonDisplayName + '</button></div></form></div></div></div>';
    return modal;
}
function displaySuccess(message,messageTitle) {
    toastr.success(message, messageTitle);
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "3000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
}
function displayError(message, messageTitle) {
    toastr.error(message, messageTitle);
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": false,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "3000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
}
