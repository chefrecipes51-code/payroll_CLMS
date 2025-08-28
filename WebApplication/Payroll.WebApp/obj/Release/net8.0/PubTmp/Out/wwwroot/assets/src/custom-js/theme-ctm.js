// $(function() {
//     $(".color-checkbox").on("change", function() {
//         // Uncheck all checkboxes first
//         $(".color-checkbox").not(this).prop("checked", false);
        
//         // Get the theme class associated with the selected checkbox
//         var themeClass = $(this).data("theme");
        
//         if ($(this).is(":checked")) {
//             // Apply the selected theme
//             $("body").addClass(themeClass);
//         } else {
//             // Remove the selected theme if unchecked
//             $("body").removeClass(themeClass);
//         }
//     });
// });


$(function() {
    $(".color-checkbox").on("change", function() {
        // Get the theme class associated with the selected checkbox
        var themeClass = $(this).data("theme");

        if ($(this).is(":checked")) {
            // If checkbox is checked, remove any previous theme and add the new theme
            $("body").removeClass(function(index, className) {
                return (className.match(/\btheme-\S+/g) || []).join(" ");
            });
            $("body").addClass(themeClass);
        } else {
            // If checkbox is unchecked, remove the theme
            $("body").removeClass(themeClass);
        }
    });
});
