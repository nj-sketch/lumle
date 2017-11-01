
$(document).ready(function () {

    $("#roleTable").DataTable({
        "language": {
            "url": languageFileUrl
        },
        "responsive": true,
        "columnDefs": [
            {
                "targets":[3, 5], // column or columns numbers
                "orderable": false  // set orderable for selected columns
            }]
    });

    $(".dataTables_filter input").attr("placeholder", "Search...");

});