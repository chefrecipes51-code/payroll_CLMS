$(document).ready(function() {
    // Detect changes on the radio buttons
    $('input[name="theme"]').change(function() {
      if ($(this).val() === 'dark') {
        // Switch to dark theme
        $('body').addClass('dark');
      } else {
        // Switch to light theme
        $('body').removeClass('dark');
      }
    });
  });


// new one
// function changeTheme(theme) {
//   var element = document.body;
//   if (theme === 'dark') {
//     element.classList.add("dark-mode");
//   } else {
//     element.classList.remove("dark-mode");
//   }
// }