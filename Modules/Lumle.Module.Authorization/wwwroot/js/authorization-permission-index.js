$(function () {

    //$(".btnDelete").on("click", function (e) {
    //    e.preventDefault();
    //    var data = $(this).attr("value");
    //    var href = rootDir + "authorization/Permission/deleteconfirm/" + data;
    //    $("#modalbodyContent").load(href, function () {
    //        $("#deleteconfirmModal").modal({
    //            keyboard: true
    //        }, "show");
    //    });
    //    return false;
    //});

    $("#permissionTable").DataTable({
        "responsive": true,
        "columnDefs": [
            {
                "targets": 2, // column or columns numbers
                "orderable": false  // set orderable for selected columns
            }]
    });

    $(".dataTables_filter input").attr("placeholder", "Search...");
});
