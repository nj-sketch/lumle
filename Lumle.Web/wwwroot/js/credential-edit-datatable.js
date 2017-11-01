$(document).ready(function () {

    var successMessage = "";
    var errorMessage = "";
    var table = $("#credentialDataTable").DataTable();

    $("#credentialDataTable tbody").on("click", ".btn", function (e) {
    e.preventDefault();

    var tr = $(this).parents("tr");

    var data = table.row(tr).data();

    var credential = { "Id": data[4], "Value": tr.find("input").val() };

    $(this).prop("disabled", true);
    tr.find("img").show();

    $.ajax({
        type: "POST",
        url: rootDir + "adminconfig/credential/edit",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(credential),
        headers: {
            "RequestVerificationToken": token
        },
        success: function (response) {
            tr.find("button").prop("disabled", false);
            tr.find("img").hide();

            if (response != null && response.success) {
                displaySuccess(response.message, response.messageTitle);
            } else {
                displayError(response.message, response.messageTitle);
            }
        },
        error: function () {
            tr.find("button").prop("disabled", false);
            tr.find("img").hide();
            errorMessage = "Opps. something went wrong. Please try again.";
            displayError(errorMessage,"Error occured");
        }
    });
});
});

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
