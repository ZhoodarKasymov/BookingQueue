// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function getCookie(name) {
    var cookies = document.cookie.split("; ");
    for (var i = 0; i < cookies.length; i++) {
        var cookie = cookies[i].split("=");
        if (cookie[0] === name) {
            return cookie[1];
        }
    }
    return "";
}

// Example starter JavaScript for disabling form submissions if there are invalid fields
(function () {
    'use strict'

    if (getCookie(".AspNetCore.Culture") === "c%3Duk%7Cuic%3Duk") {
        $("#kyr-lang").addClass("lang-link-active");
    } else {
        $("#rus-lang").addClass("lang-link-active");
    }

    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    var forms = document.querySelectorAll('.needs-validation')

    // Loop over them and prevent submission
    Array.prototype.slice.call(forms)
        .forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault()
                    event.stopPropagation()
                }

                form.classList.add('was-validated')
            }, false)
        })
})();

$(document).ready(function () {
    $('.phone-input').inputmask('+996 (999) 99-99-99');
});