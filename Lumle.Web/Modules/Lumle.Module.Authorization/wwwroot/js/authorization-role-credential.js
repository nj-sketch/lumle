$(function () {
    var readConstant = "read";
    var deleteConstant = "delete";
    $("#info").html("");
    var role = "";
    role = $("#roleid").html();
    $("#btnCancel").on("click", function () {
        window.location.href = rootDir + "role";
    });

    $("#btnSubmit").on("click", function () {
        $("#info").html("");

        var selected = getAllCredentialValue();
        var url = rootDir + "authorization/Role/permission";

        if (selected.length > 0) {
            $(this).prop("disabled", true);
            $("#loadingImg").show();
            $("input[type=checkbox]").prop("disabled", true);
            savePermission(url, selected);
        }
        else {
            removeInfoClass();
            $("#info").html("<p>Please Select role permission first.</p>");
            $("#info").addClass("col-md-12 alert alert-danger");
            $("html, body").animate({
                scrollTop: $("#page-header").offset().top
            }, 1800);
        }
    });

    ///check select ckbox if all perimission on page load
    checkAllIfAllPermission();

    ///get all checked role credentail
    function getAllCredentialValue() {
        var selected = new Array();
        $("input:checkbox[name=ckbox]:checked").each(function () {
            selected.push($(this).val());
        });
        return selected;
    };

    /// ajax call
    function savePermission(url, selected) {
        var model = JSON.stringify({
            RoleId: role,
            ClaimValues: selected
        });
        $.ajax({
            url: url,
            type: "POST",
            contentType: "application/json",
            data: model,
            headers: {
                "RequestVerificationToken": token
            },
            success: function (response) {
                $("#btnSubmit").removeProp("disabled");
                $("#loadingImg").hide();
                $("input[type=checkbox]").removeProp("disabled");
                if (response.success) {
                    removeInfoClass();
                    $("#info").html("<p>" + response.message + "</p>");
                    $("#info").addClass("col-md-12 alert alert-success");

                } else {
                    removeInfoClass();
                    $("#info").html("<p>" + response.message + "</p>");
                    $("#info").addClass("col-md-12 alert alert-danger");
                }
                $("html, body").animate({
                    scrollTop: $("#page-header").offset().top
                }, 1800);
            },
            error: function () {
                $("#btnSubmit").removeProp("disabled");
                $("#loadingImg").hide();
                $("input[type=checkbox]").removeProp("disabled");
                removeInfoClass();
                $("#info").html("<p>An error has occurred, Please try again later.</p>");
                $("#info").addClass("col-md-12 alert alert-danger");

                $("html, body").animate({
                    scrollTop: $("#page-header").offset().top
                }, 1800);
            }
        });
    };

    ///clear information
    function removeInfoClass() {
        $("#info").html("");
        $("#info").removeAttr("class");
    };

    /// Handle click on "Select all" control
    $("#checkAll").on("click", function () {

        /// Check/uncheck checkboxes 
        $('input[type="checkbox"]').prop("checked", this.checked);
    });

    $("input:checkbox[name=ckbox]").on("click", function () {
        // If checkbox is checked
        var value = $(this).val().split(".");
        var permission = value[value.length - 1].toLowerCase().trim();
        if ($(this).is(":checked")) {
            if (permission !== readConstant && permission !== deleteConstant) {
                var readValue = "";
                for (var i = 0; i <= value.length - 1; i++) {
                    if (i !== value.length - 1) {
                        readValue += value[i].trim();
                        readValue += ".";
                    } else {
                        readValue += readConstant;
                    }
                }
                $("input:checkbox[value='" + readValue + "']").prop("checked", true);
            }
            if (permission !== readConstant && permission === deleteConstant) {
                $(this).closest("div .submodule").find("input:checkbox[name=ckbox]").prop("checked", true);
            }
        }
        else {
            if (permission === readConstant) {
                $(this).closest("div .submodule").find("input:checkbox[name=ckbox]").prop("checked", false);
            }
        }
        var $chkboxChecked = $("input:checkbox[name=ckbox]:checked");
        var ckLenght = $chkboxChecked.length;

        var ckListLenght = $(".checkList").length;

        if (ckLenght === ckListLenght) {
            $("#checkAll").prop("checked", this.checked);
        }
        else {
            $("#checkAll").removeProp("checked");
        }

    });

    function checkAllIfAllPermission() {
        var $chkboxChecked = $("input:checkbox[name=ckbox]:checked");
        var ckLenght = $chkboxChecked.length;

        var ckListLenght = $(".checkList").length;

        if (ckLenght === ckListLenght) {
            $("#checkAll").prop("checked", "checked");
        }
        else {
            $("#checkAll").removeProp("checked");
        }
    }
});