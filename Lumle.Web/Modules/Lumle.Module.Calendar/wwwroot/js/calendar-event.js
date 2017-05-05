$(function () {
    $('#eventDataTable').DataTable({
        "language": {
            "url": languageFileUrl
        },
        responsive: true,
        "columnDefs": [
            {
                "targets": [4], // column or columns numbers
                "orderable": false  // set orderable for selected columns
            }]
    });

    $('.dataTables_filter input').attr("placeholder", "Search...");
});