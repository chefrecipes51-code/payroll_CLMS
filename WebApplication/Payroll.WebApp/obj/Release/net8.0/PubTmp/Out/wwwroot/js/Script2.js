// script2.js

function showAlert(message) {
    alert(message);
}

// Example usage of the showAlert function
document.addEventListener('DOMContentLoaded', () => {
    const alertButton = document.getElementById('alertButton');

    alertButton.addEventListener('click', () => {
        showAlert('This is an alert message from script2.js!');
    });

    // Function to change the background color of the container
    const container = document.querySelector('.container');
    const changeColorButton = document.getElementById('changeColorButton');

    changeColorButton.addEventListener('click', () => {
        container.style.backgroundColor = container.style.backgroundColor === 'lightgreen' ? 'white' : 'lightgreen';
    });
});
