

$('#emailTemplateDataTable').dataTable({
    "language": {
        "url": languageFileUrl
    },
        responsive: true,
        "columnDefs": [
            {
                "targets": [3], // column or columns numbers
                "orderable": false  // set orderable for selected columns
            }]
});

$(".dataTables_filter input").attr("placeholder", "Search...");