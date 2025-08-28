// script1.js

function greetUser(name) {
    console.log(`Hello, ${name}! Welcome to our website.`);
}

function displayDate() {
    const currentDate = new Date();
    const formattedDate = currentDate.toLocaleDateString();
    console.log(`Today's date is: ${formattedDate}`);
}

// Example user interaction
document.addEventListener('DOMContentLoaded', () => {
    const greetButton = document.getElementById('greetButton');
    const userNameInput = document.getElementById('userNameInput');

    greetButton.addEventListener('click', () => {
        const userName = userNameInput.value || 'Guest';
        greetUser(userName);
        displayDate();
    });
});
