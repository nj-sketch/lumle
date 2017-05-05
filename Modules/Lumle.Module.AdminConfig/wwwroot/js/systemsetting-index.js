
$(function () {
    if (hasPermissionForUpdate === false) {
        $("#systemMaintananceModeclass").prop("disabled", true);
        modeValue = $("input:checkbox[name=systemMaintananceMode]");
        var ischecked = modeValue.is(":checked");
        if (ischecked) {
            $("#checkAll").prop("disabled", true);
            $("input:checkbox[name=ckbox]").prop("disabled", true);
            $("#btnSubmit").prop("disabled", true);
        }
    }
    var state;
    var modal = $("#confirmModel");
    $("#systemMaintananceModeclass").on("click", function () {
        if (hasPermissionForUpdate === true) {
            modeValue = $("input:checkbox[name=systemMaintananceMode]");
            var ischecked = modeValue.is(":checked");
            if (ischecked) {
                state = "0";
                modal.modal("show");
            }
            else {
                removeInfoClass();
                $("#divRoles").css("display", "block");
                $("input:checkbox[name=systemMaintananceMode]").prop("checked", true);
                $("#lblStatus").html("ON");
                $("#systemMaintananceModeclass").addClass("switch-toggle switch-on");
                $("input:checkbox[name=ckbox]").prop("checked", false);
                $("#checkAll").prop("checked", false);
            }
        }
    });

    $("#btnSubmit").on("click", function () {
        state = "1";
        modal.modal("show");
    });

    $("input[type=password]").keyup(function (e) {
        if (e.keyCode !== 13) {
            var userCredentail = $("input[type=password]").val();
            if (userCredentail.trim() !== "") {
                $("#btnOk").prop("disabled", false);
            }
            else {
                $("#btnOk").prop("disabled", true);
            }
        }
    });

    $("input[type=password]").keydown(function (e) {
        var userCredentail = $("input[type=password]").val();
        if (userCredentail.trim() != "") {
            if (e.keyCode === 13) {
                $("#btnOk").prop("disabled", true);
                $("#loadingImgforPopup").show();
                var userCredentail = $("input[type=password]").val();
                AuthenticateUser(userCredentail);
            }
        }
    });


    $("#btnOk").on("click", function () {
        $("#btnOk").prop("disabled", true);
        $("#loadingImgforPopup").show();
        var userCredentail = $("input[type=password]").val();
        AuthenticateUser(userCredentail);
    });
    $("#checkAll").on("click", function () {
        /// Check/uncheck checkboxes 
        $('input[name="ckbox"]').prop("checked", this.checked);
    });


    $("input:checkbox[name=ckbox]").on("click", function () {
        // If checkbox is checked

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

    CheckAll();

    function AuthenticateUser(userCredentail) {
        var model = JSON.stringify({
            Credential: userCredentail
        });
        var token = $("input[type=hidden][name=__RequestVerificationToken]").val();

        $.ajax({
            url: rootDir + "adminconfig/systemsetting/CheckUserCredential",
            type: "POST",
            contentType: "application/json",
            data: model,
            headers: {
                "RequestVerificationToken": token
            },
            success: function (data) {
                if (data.status) {
                    SaveData();

                } else {
                    $("#btnOk").prop("disabled", false);
                    $("#loadingImgforPopup").hide();
                    $("input[type=password]").val("");
                    displayError("Opps. your credentail doesnot match. Please try again.");
                }

            },
            error: function () {
                $("#btnOk").prop("disabled", false);
                $("#loadingImgforPopup").hide();
                displayError("Opps. something went wrong. Please try again.");
            }
        });
    }

    function SaveData() {
        var id = $("input:checkbox[name=systemMaintananceMode]").attr("value");
        var roles = getAllRolesValue();
        var model = JSON.stringify({
            Status: state,
            Id: id,
            Roles: roles
        });
        var token = $("input[type=hidden][name=__RequestVerificationToken]").val();

        $.ajax({
            url: rootDir + "adminconfig/systemsetting/UpdateSystemMaintenanceMode",
            type: "POST",
            contentType: "application/json",
            data: model,
            headers: {
                "RequestVerificationToken": token
            },
            success: function (response) {
                $("#btnOk").prop("disabled", false);
                $("#loadingImgforPopup").hide();
                modal.modal("hide");
                $("input[type=checkbox]").removeProp("disabled");
                if (response != null && response.success) {
                    displaySuccess(response.message, response.messageTitle);
                } else {
                    displayError(response.message, response.messageTitle);
                }
                window.location.reload(true);
            },
            error: function () {
                tr.find("button").prop("disabled", false);
                tr.find("img").hide();
                displayError("Opps. something went wrong. Please try again.", "Error occured");
            }
        });
    }

    ///clear information
    function removeInfoClass() {
        $("#systemMaintananceModeclass").removeAttr("class");
    };

    ///get all checked roles
    function getAllRolesValue() {
        var selected = new Array();
        $("input:checkbox[name=ckbox]:checked").each(function () {
            selected.push($(this).val());
        });
        return selected;
    };

    ///check all when all roles checked on page load
    function CheckAll() {
        var checkedAll = IfCheckedAll();
        if (checkedAll) {
            $("#checkAll").prop("checked", "checked");
        }
    }

    ///check if all role checked
    function IfCheckedAll() {
        var $chkboxChecked = $("input:checkbox[name=ckbox]:checked");
        var ckLenght = $chkboxChecked.length;

        var ckListLenght = $(".checkList").length;
        if (ckLenght === ckListLenght) {
            return true;
        }
        else {
            return false;
        }
    }
    function IfCheckedAny() {
        var $chkboxChecked = $("input:checkbox[name=ckbox]:checked");
        var ckLenght = $chkboxChecked.length;
        if (ckLenght > 0) {
            return true;
        }
        else {
            return false;
        }
    }

    function displaySuccess(message) {
        toastr.success(message, 'Successfully updated');
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
    function displayError(message) {
        toastr.error(message, 'Error Occured');
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

