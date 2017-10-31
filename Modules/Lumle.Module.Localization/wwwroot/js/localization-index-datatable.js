$(document).ready(function () {

    $("#cultureDataTable").dataTable({
        "language": {
            "url": languageFileUrl
        },
        "columnDefs": [
            {
                "searchable": false,
                "orderable": false,
                "targets": [0, 2]
            }
        ]
    });
    $(".dataTables_filter input").attr("placeholder", "Search...");

    $(".switch-toggle").on("click", function () {
        var hasClass = $(this).hasClass("switch-on");
        $(this).removeAttr("class");

        var culture = $(this).attr("value");

        if (hasClass) {
            var cultureViewModel = { "Name": culture, "IsActive": false };
            $(this).addClass("switch-toggle switch-off");
            $.ajax({
                url: rootDir + "localization/culture/updateCulture",
                type: "POST",
                data: JSON.stringify(cultureViewModel),
                contentType: "application/json",
                headers: { "RequestVerificationToken": token },
                success: function (response) {
                    if (response != null && response.success) {
                        displaySuccess(response.message, response.messageTitle);
                    } else {
                        displayError(response.message, response.messageTitle);
                    }
                },
                error: function () {
                    displayError("Opps!!! something went wrong. Please try again.","Error occured");
                }
            });
        }
        else {
            var cultureViewModel = { "Name": culture, "IsActive": true };

            $(this).addClass("switch-toggle switch-on");
            $.ajax({
                url: rootDir + "localization/culture/updateCulture",
                type: "POST",
                data: JSON.stringify(cultureViewModel),
                contentType: "application/json",
                headers: { "RequestVerificationToken": token },
                success: function (response) {
                    if (response != null && response.success) {
                        displaySuccess(response.message, response.messageTitle);
                    } else {
                        displayError(response.message, response.messageTitle);
                    }
                },
                error: function () {
                    displayError("Opps!!! something went wrong. Please try again.");
                }
            });
        }
    });

    function displaySuccess(message,messageTitle) {
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
});
