// select2

$(document).ready(function () {

  $("#country").select2();

  function format(item, state) {
    if (!item.id) {
      return item.text;
    }
    var countryUrl = "https://hatscripts.github.io/circle-flags/flags/";
    var stateUrl = "https://oxguy3.github.io/flags/svg/us/";
    var url = state ? stateUrl : countryUrl;
    var img = $("<img>", {
      class: "img-flag",
      width: 26,
      src: url + item.element.value.toLowerCase() + ".svg"
    });
    var span = $("<span>", {
      text: " " + item.text
    });
    span.prepend(img);
    return span;
  }


  $(".flag-countries").select2({
    templateResult: function (item) {
      return format(item, false);
    }
  });

  // $(".js-select2").select2({
  //   closeOnSelect: false,
  //   placeholder: "Select companieses",
  //   allowHtml: true,
  //   allowClear: true,
  //   tags: true,
  // });

  // $('#countriesname').select2({
  //   placeholder: "Select country",
  //   allowClear: true,
  // });
  // $('#companytype').select2({
  //   placeholder: "Select company type",
  //   allowClear: true,
  // });
  // $('#state').select2({
  //   placeholder: "Select state",
  //   allowClear: true,
  // });
  $('.multi-selection').select2({
    placeholder: "Select branch",
    closeOnSelect: false,
    allowHtml: true,
    allowClear: true,
    tags: true,
    width: '100%',  // Ensures the Select2 input spans the full width of its container
    // dropdownAutoWidth: true,
  });
  
  $('.with-search').select2({
    selectOnClose: true,
    allowClear: true,
    multiple: false,
    dropdownAutoWidth: true,   // Auto adjust dropdown width
    width: '100%',
  });

  
// Initialize the offcanvas using the default "offcanvas" class
var myOffcanvas = new bootstrap.Offcanvas(document.querySelector('.offcanvas'));

// Wait for the offcanvas to be shown and then initialize Select2
$('.offcanvas').on('shown.bs.offcanvas', function () {
    $('.with-search').select2({
        // selectOnClose: true,
        // allowClear: true,
        // multiple: false,
        dropdownAutoWidth: true,   // Auto adjust dropdown width
        width: '100%',
    });
});

  // $('#department').select2({
  //   placeholder: "Select department",
  //   closeOnSelect: false,
  //   allowHtml: true,
  //   allowClear: true,
  //   tags: true,
  // });
  // $('#role').select2({
  //   placeholder: "Select role",
  //   closeOnSelect: false,
  //   allowHtml: true,
  //   allowClear: true,
  //   tags: true,
  // });
});
// });

// new js added using class name
$('.select2_search_ctm').select2({
  placeholder: "Select",
  allowClear: true,
  multiple: false,
  dropdownAutoWidth: true,   // Auto adjust dropdown width
  width: '100%',

  ajax: {
    url: 'your-api-endpoint',
    dataType: 'json',
    processResults: function (data) {
      return {
        results: data.items
      };
    }
  }
});

$(document).ready(function () {
  // Initialize Select2
  $('.select2_search_ctm').select2();

  // Add the class to the select2-results dropdown
  $('.select2_search_ctm').on('select2:open', function () {
    // Add the class to the results container
    $('.select2-results').addClass('select2_search_ctm_result');
  });
});

// $(document).ready(function () {
//   // Initialize select2
//   $('.select2_search_ctm').select2({
//     placeholder: "Select company type",
//     allowClear: true
//   });

//   $('.select2_search_ctm').on('select2:open', function() {
//     var width = $(this).outerWidth(); // Get the width of the input box
//     $('.form-conntrol').css('width', width); // Set dropdown width to match input width
//   });

//   // Add class when dropdown opens
//   $('.select2_search_ctm').on('select2:open', function () {
//     // Add custom class to the select2 container when the dropdown is opened
//     $('.select2-container').addClass('open_search_results_ctm');
//   });

//   // Optionally, remove the class when the dropdown is closed
//   $('.select2_search_ctm').on('select2:close', function () {
//     // Remove custom class when the dropdown is closed
//     $('.select2-container').removeClass('open_search_results_ctm');
//   });
// });

// $(document).ready(function () {
//   // Initialize all select2 elements with the class .select2_search_ctm
//   $('.select2_search_ctm').each(function () {
//     var $selectElement = $(this); // Cache the current select element

//     // Initialize select2 for each element
//     $selectElement.select2({
//       placeholder: "Select company type",
//       allowClear: true
//     });

//     // Adjust dropdown width when it opens
//     $selectElement.on('select2:open', function() {
//       var width = $selectElement.outerWidth(); // Get the width of the current select element
//       $selectElement.next('.select2').find('.select2-dropdown').css('width', width); // Set dropdown width to match select element
//     });

//     // Add custom class when dropdown opens
//     $selectElement.on('select2:open', function () {
//       // Add custom class to the specific select2 container when the dropdown is opened
//       $selectElement.next('.select2').addClass('open_search_results_ctm');
//     });

//     // Remove custom class when dropdown closes
//     $selectElement.on('select2:close', function () {
//       // Remove custom class when the dropdown is closed
//       $selectElement.next('.select2').removeClass('open_search_results_ctm');
//     });
//   });
// });

$(document).ready(function () {
  // Initialize all select2 elements with the class .select2_search_ctm
  $('.select2_search_ctm').each(function () {
    var $selectElement = $(this); // Cache the current select element

    // Initialize select2 for each element (ensure multiple is not set)
    $selectElement.select2({
      placeholder: "Select company type",
      allowClear: true,
      multiple: false // Ensure multiple selection is disabled (no checkboxes)
    });

    // Adjust dropdown width when it opens
    $selectElement.on('select2:open', function () {
      var width = $selectElement.outerWidth(); // Get the width of the current select element
      $selectElement.next('.select2').find('.select2-dropdown').css('width', width); // Set dropdown width to match select element
    });

    // Add custom class when dropdown opens
    $selectElement.on('select2:open', function () {
      // Add custom class to the specific select2 container when the dropdown is opened
      $selectElement.next('.select2').addClass('open_search_results_ctm');
    });

    // Remove custom class when dropdown closes
    $selectElement.on('select2:close', function () {
      // Remove custom class when the dropdown is closed
      $selectElement.next('.select2').removeClass('open_search_results_ctm');
    });
  });
});


