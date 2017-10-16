var resourceFilterBy = 0;
var table;

$(function () {
   
    Resource();

    $('#resourceFilterBy').change(function () {
        resourceFilterBy = $('#resourceFilterBy').val();
        $('#resourceDataTable').DataTable().clear().destroy();
        Resource();
    });

    ///save resource 
    $("#resourceDataTable tbody").on("click", ".btn", function (e) {
        e.preventDefault();

        var tr = $(this).parents("tr");

        var data = table.row(tr).data();

        var resource = { "CultureId": data.cultureId, "ResourceCategoryId": data.resourceCategoryId, "Key": data.key, "Value": tr.find("input").val() };

        $(this).prop("disabled", true);
        tr.find("img").show();
        var token = $("input[type=hidden][name=__RequestVerificationToken]").val();
        $.ajax({
            type: "POST",
            url: rootDir + "localization/culture/addOrUpdateResource",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(resource),
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
                displayError("Opps. something went wrong. Please try again.","Error occured");
            }
        });
    });

    $('input[type=file]').change(function () {
        var fileSelected = $(this).val();
        if (fileSelected !=="") {
            $('input[type=submit]').prop('disabled', false);
        } else {
            $('input[type=submit]').prop('disabled', true);
        }
    });
    $('form').on('submit', function () {
        $(this).find('input[type=submit]').prop('disabled', true);
        $("#loadingImg").show();
        $(this).get(0).submit();
        return false;
    });

    $("#importExcelModel").removeAttr("padding-right");
    
});


function Resource () {
   
    var culture = $("#lblCulture").attr("value");
    table = $("#resourceDataTable").DataTable({
        "language": {
            "url": languageFileUrl
        },
        "serverSide": true,
        "distory": true,
        "ajax": {
            "type": "POST",
            "url": rootDir + "localization/culture/DataHandler",
            "contentType": "application/json; charset=utf-8",
            "headers": { "RequestVerificationToken": token },
            "data": function (data) {
                data.Culture = culture;
                data.ResourceCategoryId = resourceFilterBy
                return data = JSON.stringify(data);
            },
            "error": function (xhr) {
                var response = JSON.parse(xhr.getResponseHeader('X-Responded-JSON'));
                if (response.status === 401) {
                    window.location.href = rootDir+"/login?ReturnUrl=%2localization/culture";
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
                "targets": [1,2]
            }
        ],
        "columns": [
            { "data": "sn" },
            { "data": "cultureId" },
            { "data": "resourceCategoryId" },
            { "data": "key", "render": $.fn.dataTable.render.text() },
            { "data": "value", "render": renderInput},
            {
                "data": "cultureId", "render": actionButton,
                "searchable": false,
                "orderable": false,
                "visible": hasPermissionForUpdateResource==true?true:false
            }
        ],
        "order": [1, "asc"],
    });

};


var renderInput = function (data) {
    if (hasPermissionForUpdateResource) {
        return '<input value="' + data + '"class="form-control" style="width:100%" />';
    }
    else {
        return '<input value="' + data + '"class="form-control" readonly style="width:100%" />';
    }
    
}

var actionButton = function () {
    return '<button class="btn btn-success btn-xs">' + saveButtonDisplayName + '</button><img style="width: 30px;display:none" src="' + rootDir + 'assets/images/spinner/loader-dark.gif" alt="saving..." display="none">';
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



