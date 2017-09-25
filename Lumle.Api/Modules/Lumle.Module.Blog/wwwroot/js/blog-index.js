$(function () {
  
    var table = $("#articleDataTable").DataTable({
        "language": {
            "url": languageFileUrl
        },
        "serverSide": true,
        "ajax": {
            "type": "POST",
            "url": rootDir + "blog/article/DataHandler",
            "contentType": "application/json; charset=utf-8",
            "headers": { "RequestVerificationToken": token },
            "data": function (data) {
                return JSON.stringify(data);
            },
            "error": function (xhr) {
                var response = JSON.parse(xhr.getResponseHeader('X-Responded-JSON'));
                if (response.status === 401) {
                    window.location.href = rootDir + "login?ReturnUrl=%2Fauthorization/User";
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
            {
                "data": "title",
                "render": $.fn.dataTable.render.text()
            },
            { "data": "author" },
            {"data": "formatedCreatedDate"},
            {
                "data": "id", "title": "Action", "render": actionButton,
                "searchable": false,
                "orderable": false,
                "visible": hasPermissionForUpdate == true ? true : hasPermissionForDelete == true ? true : false
            }
        ],
        "order": [1, "asc"]
    });
    $(".dataTables_filter input").attr("placeholder", "Search...");
});

var actionButton = function (data, type, full, meta) {
    $.fn.dataTable.render.text();
    var modalId = "deletdModel" + data;
    var actionBtn = "";
    if (hasPermissionForUpdate) {
        actionBtn = '<a href="' + rootDir + 'blog/article/edit/' + data + '"class="btn btn-info btn-sm"><i class="glyph-icon icon-pencil"></i> '  + editButtonDisplayName + '</a>';
    }
    if (hasPermissionForDelete) {
            actionBtn += '&nbsp;&nbsp;<button class="btn btn-danger btn-sm" data-toggle="modal" data-target="#' + modalId +
                                    '"><i class="glyph-icon icon-trash-o"></i> ' + deleteButtonDisplayName + '</button>';
            actionBtn += deleteModalSection(modalId, data, full.title);
    }
    return actionBtn;
}
//delete modal section
var deleteModalSection = function (modalId, articleId, title) {
    var modal = '<div id="' + modalId + '"class="modal fade in"><div class="modal-dialog" style="width:40% !important; margin-top:10% !important;">' +
                '<div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-hidden="true">x</button>' +
                '<h3 class="modal-title">Delete User</h3></div><form action="' + rootDir + 'blog/article/delete/' + articleId + '" role="form" method="post">' +
                '<div class="modal-body">Are you sure you want to delete this record ?</div>' +
                '<div class="modal-footer"><button data-dismiss="modal" class="btn btn-default" type="button">Cancel</button>' +
                '<button class="btn btn-success" type="submit">Delete</button></div></form></div></div></div>';
    return modal;
}