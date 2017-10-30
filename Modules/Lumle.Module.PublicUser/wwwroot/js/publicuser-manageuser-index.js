$(function () {

    var table = $("#publicUserDataTable").DataTable({
        "language": {
            "url": languageFileUrl
        },
        "serverSide": true,
        "ajax": {
            "type": "POST",
            "url": rootDir + "publicuser/DataHandler",
            "contentType": "application/json; charset=utf-8",
            "headers": { 'RequestVerificationToken': antiforgeryToken },
            "data": function (data) {
                return JSON.stringify(data);
            },
            "error": function (xhr) {
                var response = JSON.parse(xhr.getResponseHeader('X-Responded-JSON'));
                if (response.status === 401) {
                    window.location.href = rootDir + "login?ReturnUrl=%2Fpublicuser";
                }
                else {
                    //alert(xhr.error);
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
                "width": "5%"
            },
            {
                "searchable": false,
                "orderable": false,
                "visible": false,
                "targets": 1
            }
        ],
        "columns": [
            { "data": "sn" },
            { "data": "id" },
            { "data": "userName"},
            { "data": "email" },
            { "data": "gender" },
            { "data": "provider" },
            {
                "data": "formatedCreatedDate",
                "searchable": false,
            },
            {
                "data": "isStaff", "render": renderUserType,
                "searchable": false,
            },
            {
                "data": "isEmailVerified", "render": renderIsEmailVerified,
                "searchable": false,
            },
            {
                "data": "isBlocked", "render": renderIsBlocked,
                "searchable": false,
            },
            {
                "data": "id", "title": "Action", "render": actionButton,
                "searchable": false,
                "orderable": false
                //  "visible": hasPermissionForUpdate == true ? true : false
            }
        ],
        "order": [1, "asc"]
    });
    $(".dataTables_filter input").attr("placeholder", "Search...");

    var modal = $("#updateModel");
    $("#publicUserDataTable tbody").on("click", ".btn-update", function () {
        var tr = $(this).parents("tr");
        var data = table.row(tr).data();
        $("#lblUserName").html(data.userName);
        $('#accountStatus>option[value="' + data.isBlocked + '"]').prop('selected', true);
        $('#userType>option[value="' + data.isStaff + '"]').prop('selected', true);
        $("#userId").val(data.id);
        modal.modal("show");
    });

    $("#btnOk").on("click", function () {
        $("#btnOk").prop("disabled", true);
        $("#loadingImgforPopup").show();
        var userId = $("#userId").val();
        var isBlocked =$("#accountStatus").val();
        var isStaff = $("#userType").val();
        UpdateUser(userId, isBlocked, isStaff);
    });


    //upate public user
    function UpdateUser(userId, isBlocked, isStaff) {
        var user = JSON.stringify({
            Id: userId,
            IsBlocked: isBlocked,
            IsStaff: isStaff
        });

        $.ajax({
            url: rootDir + "publicuser/update",
            type: "POST",
            contentType: "application/json",
            data: user,
            headers: {
                "RequestVerificationToken": antiforgeryToken
            },
            success: function (response) {
                debugger;
                $("#btnOk").prop("disabled", false);
                $("#loadingImgforPopup").hide();
                modal.modal("hide");
                if (response != null && response.success) {
                    displaySuccess(response.message, response.messageTitle);
                } else {
                    displayError(response.message, response.messageTitle);
                }
                window.location.reload(true);
            },
            error: function () {
                displayError("Opps. something went wrong. Please try again.", "Error occured");
            }

        });
    }
});

var actionButton = function (data, type, full, meta) {
    var actionBtn = '&nbsp;&nbsp;<button class="btn btn-info btn-xs btn-update"><i class="glyph-icon icon-pencil"></i> ' + editButtonDisplayName + '</button>';
    return actionBtn;
}

var renderIsBlocked = function (data) {
    if (data) {
        return '<span class="bs-label bg-orange alert-icon"><i class="glyph-icon tooltip-button  icon-warning" title="Blocked" data-original-title="Blocked"></i> Blocked</span>';
    }
    else {
        return '<span class="bs-label label-success"><i class="glyph-icon tooltip-button icon-check-circle" title="Enabled" data-original-title="Enabled"></i> Enabled </span>';
    }
}
//render whether user is Staff or not
var renderUserType = function (data) {
    if (data) {
        return '<span class="bs-label bg-yellow pricing-specs">Staff</span>';
    }
    else {
        return '<span class="bs-label label-success">General</span>';
    }
}

var renderIsEmailVerified = function (data) {
    if (data) {
        return '<span class="bs-label label-success"><i class="glyph-icon tooltip-button icon-check-circle" title="Confirmed" data-original-title="Confirmed"></i> Confirmed</span>';
    }
    else {
        return '<span class="bs-label bg-orange alert-icon"><i class="glyph-icon tooltip-button icon-warning" title="Not Confirmed" data-original-title="Not Confirmed"></i> Not Confirmed </span>';
    }
}

function displaySuccess(message, messageTitle) {
    toastr.success(message, messageTitle);
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": false,
        "positionClass": "toast-top-right",
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
        "positionClass": "toast-top-right",
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
