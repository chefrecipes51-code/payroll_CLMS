// tree view

document.querySelectorAll('.treeview .toggle, .treeview input[type="checkbox"]').forEach((element) => {
    element.addEventListener('click', function () {
      // Locate the nearest child <ul> for expanding/collapsing
      const parentLi = this.closest('li');
      const childList = parentLi.querySelector('ul');
      const toggleButton = parentLi.querySelector('.toggle');
  
      if (childList) {
        // Toggle visibility of the child <ul>
        childList.style.display = childList.style.display === 'block' ? 'none' : 'block';
  
        // Update toggle icon state
        if (toggleButton) {
          toggleButton.classList.toggle('open');
        }
      }
    });
  });