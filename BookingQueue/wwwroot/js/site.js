// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function googleTranslateElementInit() {
    new google.translate.TranslateElement({
        pageLanguage: 'ru',
        includedLanguages: 'en,ru,ky', // Specify the languages you want to include
        layout: google.translate.TranslateElement.InlineLayout.SIMPLE, // Change the layout to Inline
        autoDisplay: false, // Prevent the automatic display of the translation widget
    }, 'google_translate_element');
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

$(document).ready(function() {
    $('.phone-input').inputmask('+999 999 999999');
});