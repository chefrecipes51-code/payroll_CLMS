//$(function () {
//    const tabKeyPrefix = 'UniquePayrollTabId_';   // Prefix for tab keys based on paths
//    const currentPath = window.location.pathname; // Current path

//    fetch('/Base/GetTabIdAndRestrictedUrls', {
//        method: 'POST',
//        headers: { 'Content-Type': 'application/json' }
//    })
//        .then(response => {
//            if (!response.ok) {
//                console.error('Failed to fetch Tab ID and Restricted URLs from the server.');
//            }
//            return response.json();
//        })
//        .then(data => {
//            const { tabId, restrictedUrls } = data;

//            // Ensure Tab ID is available in the cookie or fetched from the server
//            fetchOrSetTabId(tabKeyPrefix, currentPath, tabId).then(tabId => {
//                if (tabId) {
//                    handleTabRestriction(tabId, restrictedUrls);
//                } else {
//                    console.error('Failed to retrieve a valid Tab ID.');
//                }
//            });
//        })
//        .catch(error => {
//            console.error('Error fetching Tab ID and Restricted URLs:', error);
//        });
//});

// Fetch or set the Tab ID
function fetchOrSetTabId(tabKeyPrefix, currentPath, initialTabId) {
    const tabKey = `${tabKeyPrefix}${currentPath}`; // Create a unique key based on the path

    let tabId = getCookie(tabKey); // Check if the tab ID is already in the cookies

    if (tabId) {
        return Promise.resolve(tabId); // Use the existing tab ID
    }

    // If no tab ID, use the one fetched from the server or create a new one
    if (initialTabId) {
        document.cookie = `${tabKey}=${initialTabId}; path=/; SameSite=Strict`;
        return Promise.resolve(initialTabId); // Return the fetched tab ID
    }

    return Promise.resolve(null); // If no valid tab ID, return null
}

// Handle tab restrictions
function handleTabRestriction(tabId, restrictedPaths) {
    if (!tabId) {
        console.error('Tab ID is not available.');
        return;
    }

    // Check if the current path matches any in the restricted paths list
    if (restrictedPaths.includes(window.location.pathname)) {
        restrictMultipleTabs(tabId);
    }
}

// Restrict multiple tabs
function restrictMultipleTabs(tabId) {
    const activeTabs = JSON.parse(localStorage.getItem('activeTabs') || '{}');

    if (activeTabs[tabId]) {
        alert('This page is already open in another tab.');
        window.location.href = '/Base/AccessDenied';
    } else {
        registerTab(tabId, activeTabs);
    }
}

// Register the active tab
function registerTab(tabId, activeTabs) {
    activeTabs[tabId] = true;
    localStorage.setItem('activeTabs', JSON.stringify(activeTabs));

    $(window).on('beforeunload', () => {
        delete activeTabs[tabId];
        localStorage.setItem('activeTabs', JSON.stringify(activeTabs));
    });
}

// Utility function to get a cookie by name
function getCookie(name) {
    const cookie = document.cookie.split('; ').find(row => row.startsWith(name + '='));
    return cookie ? cookie.split('=')[1] : null;
}