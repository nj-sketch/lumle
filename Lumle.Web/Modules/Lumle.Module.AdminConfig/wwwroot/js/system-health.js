$(document).ready(function () {
    var url = rootDir + "adminconfig/systemhealth/status";
    $.getJSON(url, function (data) {
        for (var i = 0; i < data.length; i++) {
            $('#' + data[i].serviceName).removeAttr("class");
            $('#' + data[i].serviceName + 'Img').hide();
            $('#' + data[i].serviceName + 'Lbl').html(data[i].message);
            if (data[i].status) {
                $('#' + data[i].serviceName).addClass("glyph-icon icon-check");
                $('#' + data[i].serviceName + 'Msg').addClass("alert alert-success");
            }
            else {
                $('#' + data[i].serviceName).addClass("glyph-icon icon-remove");
                $('#' + data[i].serviceName + 'Msg').addClass("alert alert-danger");
            }
        
        }
    });
});