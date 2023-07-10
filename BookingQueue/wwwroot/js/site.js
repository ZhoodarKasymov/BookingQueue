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

function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    } else {
        var expires = "";
    }
    document.cookie = name + "=" + value + expires + "; path=/";
}

function googleTranslateElementInit() {
    new google.translate.TranslateElement({
        pageLanguage: 'ru',
        includedLanguages: 'ru,ky', // Specify the languages you want to include
        autoDisplay: false, // Prevent the automatic display of the translation widget
    }, 'google_translate_element');

    if (getCookie("googtrans") === "/ru/ky") {
        $("#kyr-lang").addClass("lang-link-active");
    } else {
        $("#rus-lang").addClass("lang-link-active");
    }
}

// Example starter JavaScript for disabling form submissions if there are invalid fields
(function () {
    'use strict'

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
    $('.phone-input').inputmask('+999 999 999999');
});

function ChangeLanguageToKyr() {
    createCookie("googtrans", "/ru/ky", 1);
    location.reload();
}

function ChangeLanguageToRu() {
    createCookie("googtrans", "/ru/ru", 1);
    location.reload();
}