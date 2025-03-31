// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ajaxError(function (event, xhr) {
    console.log("AJAX error handler triggered. Status code:", xhr.status);

    if (xhr.status === 403) {
        var response = xhr.responseJSON || {};
        var errorMessage = response.message || "You are not authorized to perform this action.";        console.log("403 Forbidden error. Message:", errorMessage);
        toastr.error(errorMessage);
    } else if (xhr.status === 401) {
        console.log("401 Unauthorized error.");
        toastr.error("You are not authenticated. Please log in.");
    } else {
        console.log("Unexpected error occurred. Status code:", xhr.status);
        toastr.error("An unexpected error occurred.");
    }
});