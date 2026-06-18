$(document).ready(function () {

    CheckPagePermission();

   // startPermissionObserver(); // watch for dynamically created buttons


});

$(document).on("draw.dt", function () {

    if (!window.pagePermission) return;

    ApplyButtonPermissions(window.pagePermission);

});
function CheckPagePermission() {

    var permissionsKey = typeof GetAuthScopedKey === "function" ? GetAuthScopedKey("UserPermissions") : "UserPermissions";
    var permissions = localStorage.getItem(permissionsKey) || localStorage.getItem("UserPermissions");

    if (!permissions) {

        console.warn("Permissions missing. Reloading...");
        LoadUserPermissions(); // reload from API
        return;

    }

    permissions = JSON.parse(permissions);

    // get current page
    var currentPage = window.location.pathname.split('/').pop().toLowerCase();

    var pagePermission = permissions.find(function (p) {

        if (!p.PagePath || p.PagePath === "#")
            return false;

        var dbPage = p.PagePath.replace("~", "").split('/').pop().toLowerCase();

        return dbPage === currentPage;

    });

    if (!pagePermission) {
        console.warn("No permission found for page:", currentPage);
        return;
    }

    console.log("Matched Permission:", pagePermission);

    window.pagePermission = pagePermission;

    if (!pagePermission.ViewPer) {
        // optional redirect
        // alert("You don't have permission to access this page");
        // window.location.href = "/store/W_Dashboard.aspx";
    }

    ApplyButtonPermissions(pagePermission);

}





function ApplyButtonPermissions(permission) {
    if (!permission.InsertPer) {
        //$(".btnAdd")
        //    .addClass("disabled")
        //    .css("pointer-events", "none")
        //    .css("opacity", "0.5");
        $(".dt-buttons button:has(i.ti-plus)")
            .addClass("disabled")
            .css("pointer-events", "none")
            .css("opacity", "0.5")
            .attr("data-bs-toggle", "tooltip")
            .attr("data-bs-title", "You don't have permission");
    }

    if (!permission.UpdatePer) {
        $(".item-edit").remove();
      //  $(".item-edit").css("display", "none");
        //$(".item-edit")
        //    .addClass("disabled")
        //    .css("pointer-events", "none")
        //    .css("opacity", "0.5");

    }

    if (!permission.DeletePer) {
        $(".item-delete").remove();
        /*    $(".delete-record").hide();*/
        //$(".delete-record")
        //    .prop("disabled", true)
        //    .addClass("disabled")
        //    .css("pointer-events", "none")
        //    .css("opacity", "0.6");
    }

}

function startPermissionObserver() {

    const observer = new MutationObserver(function () {

        if (window.pagePermission) {
            ApplyButtonPermissions(window.pagePermission);
        }

    });

    observer.observe(document.body, {
        childList: true,
        subtree: true
    });

}

function LoadUserPermissions() {

    var userId = localStorage.getItem("LoginID");
    var permissionsKey = typeof GetAuthScopedKey === "function" ? GetAuthScopedKey("UserPermissions") : "UserPermissions";

    if (!userId || userId === "null" || userId === "undefined") {
        if (typeof RedirectToLogin === "function") {
            RedirectToLogin();
        }
        return;
    }

    $.ajax({
        type: "GET",
        url: APIUrl + "api/Z_UsersRoleForm/GetUserPermissions?UserID=" + userId,
        headers: {
            'Authorization': 'Bearer ' + GetAccessToken()
        },
        success: function (result) {

            var data = JSON.parse(result.message);

            localStorage.setItem(permissionsKey, JSON.stringify(data));
            localStorage.setItem("UserPermissions", JSON.stringify(data));

            console.log("Permissions Stored:", data);
            CheckPagePermission();

        }
    });
}
