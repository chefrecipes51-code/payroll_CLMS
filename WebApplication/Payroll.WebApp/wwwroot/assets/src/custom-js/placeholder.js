$(document).ready(function () {
    const inputElements = document.querySelectorAll('.form-control');
  
    // Function to capitalize the first letter of the placeholder
    function capitalizeFirstLetterOfPlaceholder(inputElement) {
      const placeholderText = inputElement.getAttribute('placeholder');
      if (placeholderText) {
        const capitalizedPlaceholder = placeholderText.charAt(0).toUpperCase() + placeholderText.slice(1);
        inputElement.setAttribute('placeholder', capitalizedPlaceholder);
      }
    }
  
    // Loop through all selected input elements and capitalize their placeholder's first letter
    inputElements.forEach(inputElement => {
      capitalizeFirstLetterOfPlaceholder(inputElement);
    });
  });