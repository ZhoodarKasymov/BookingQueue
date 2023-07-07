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