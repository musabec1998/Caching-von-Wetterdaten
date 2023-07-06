// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(".line1").on("click", function ()
{
    // ...das nächste Nachbar-Element anzeigen/ausblenden
    $(this).next(".line2").toggle();
});