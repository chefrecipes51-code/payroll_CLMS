


// Start upload preview image
// $(".gambar").attr("src", "https://user.gadjian.com/static/images/personnel_boy.png");
// var $uploadCrop,
// tempFilename,
// rawImg,
// imageId;
// function readFile(input) {
//    if (input.files && input.files[0]) {
//           var reader = new FileReader();
//           reader.onload = function (e) {
//       $('.upload-demo').addClass('ready');
//       $('#cropImagePop').modal('show');
//             rawImg = e.target.result;
//           }
//           reader.readAsDataURL(input.files[0]);
//       }
//       else {
//         swal("Sorry - you're browser doesn't support the FileReader API");
//     }
// }

// $uploadCrop = $('#upload-demo').croppie({
//   viewport: {
//     width: 150,
//     height: 200,
//   },
//   enforceBoundary: false,
//   enableExif: true
// });
// $('#cropImagePop').on('shown.bs.modal', function(){
//   // alert('Shown pop');
//   $uploadCrop.croppie('bind', {
//         url: rawImg
//       }).then(function(){
//         console.log('jQuery bind complete');
//       });
// });

// $('.item-img').on('change', function () { imageId = $(this).data('id'); tempFilename = $(this).val();
//                                          $('#cancelCropBtn').data('id', imageId); readFile(this); });
// $('#cropImageBtn').on('click', function (ev) {
//   $uploadCrop.croppie('result', {
//     type: 'base64',
//     format: 'jpeg',
//     size: {width: 150, height: 200}
//   }).then(function (resp) {
//     $('#item-img-output').attr('src', resp);
//     $('#cropImagePop').modal('hide');
//   });
// });
// End upload preview image


// Peofile pic upload JS Start

// $(document).on('click', '#upload-aphoto', function (event) {
//   event.preventDefault(); // Prevent default behavior
//   $('#selectedFile').click(); // Trigger file input
// });

// // Handling file selection and opening modal
// $('#selectedFile').change(function () {
//   if (!this.files[0]) return; // Ensure file is selected
//   console.log('File selected, opening modal'); // Debug log
//   $('#emp_pro_pic_modal').modal('show'); // Open modal

//   let reader = new FileReader();
//   reader.addEventListener("load", function () {
//     window.src = reader.result; // Store image data
//     $('#selectedFile').val(''); // Reset file input
//   });
//   reader.readAsDataURL(this.files[0]); // Process selected file
// });

// let croppi;
// $('#emp_pro_pic_modal').on('shown.bs.modal', function () {
//   let width = document.getElementById('crop-image-container').offsetWidth - 20;
//   $('#crop-image-container').height((width - 80) + 'px');
//   croppi = $('#crop-image-container').croppie({
//     viewport: {
//       width: width,
//       height: width
//     },
//   });
//   $('.crop-img-preview').height(document.getElementById('crop-image-container').offsetHeight + 50 + 'px');
//   croppi.croppie('bind', {
//     url: window.src,
//   }).then(function () {
//     croppi.croppie('setZoom', 0);
//   });
// });
// $('#emp_pro_pic_modal').on('hidden.bs.modal', function () {
//   croppi.croppie('destroy');
// });

// $(document).on('click', '.save-modal', function (ev) {
//   croppi.croppie('result', {
//     size: 'original'
//   }).then(function (resp) {
//     $('#confirm-img').attr('src', resp);
//     $('.modal').modal('hide');
//   });
// });
// Profile Pic Uplaod JS END


// function format(item) {
//     if (!item.id) {
//       return item.text;
//     }
//     var countryUrl = "https://hatscripts.github.io/circle-flags/flags/";
//     var img = $("<img>", {
//       class: "img-flag",
//       width: 26,
//       src: countryUrl + item.element.value.toLowerCase() + ".svg"
//     });
//     var span = $("<span>", {
//       text: " " + item.text
//     });
//     span.prepend(img);
//     return span;
//   }

//   $(document).ready(function() {
//     $("#countries").select2({
//       templateResult: format
//     });
//   });

// tooltip bootstrap
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});
document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = Array.from(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.forEach(function (tooltipTriggerEl) {
        new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

// onclick disabled dropdown

function disableDropdown() {
    // Disable the dropdown button after it is clicked
    const dropdownButton = document.getElementById('masterDropdown');
    dropdownButton.disabled = true;
}