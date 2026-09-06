// ============================================
// ADMIN LAYOUT - RESIZE HANDLING
// ============================================

window.adminLayout = {
    // Get window width
    getWindowWidth: function () {
        return window.innerWidth;
    },

    // Initialize resize listener
    initResize: function (dotnetHelper) {
        try {
            console.log('adminLayout.initResize called');

            // Initial check
            var width = this.getWindowWidth();
            dotnetHelper.invokeMethodAsync('OnResize', width);

            // Setup resize listener
            this.resizeHandler = function () {
                var width = window.innerWidth;
                dotnetHelper.invokeMethodAsync('OnResize', width);
            };

            window.addEventListener('resize', this.resizeHandler);

            // Handle orientation change
            window.addEventListener('orientationchange', function () {
                setTimeout(function () {
                    var width = window.innerWidth;
                    dotnetHelper.invokeMethodAsync('OnResize', width);
                }, 300);
            });

            console.log('adminLayout.initResize completed successfully');
        } catch (e) {
            console.log('Resize listener error:', e);
        }
    },

    // Check if mobile
    isMobile: function () {
        return window.innerWidth < 768;
    },

    // Remove resize listener
    removeResizeListener: function () {
        try {
            if (this.resizeHandler) {
                window.removeEventListener('resize', this.resizeHandler);
            }
        } catch (e) {
            console.log('Remove listener error:', e);
        }
    }
};

// ============================================
// SIDEBAR TOGGLE FUNCTIONS
// ============================================

window.toggleSidebar = function () {
    const sidebar = document.getElementById('mainSidebar');
    const overlay = document.querySelector('.sidebar-overlay');

    if (sidebar) {
        sidebar.classList.toggle('open');
    }
};

window.closeSidebar = function () {
    const sidebar = document.getElementById('mainSidebar');
    if (sidebar) {
        sidebar.classList.remove('open');
    }
};

window.openSidebar = function () {
    const sidebar = document.getElementById('mainSidebar');
    if (sidebar) {
        sidebar.classList.add('open');
    }
};

// Close sidebar on Escape key
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        window.closeSidebar();
    }
});

// Handle window resize - auto close on desktop
window.addEventListener('resize', function () {
    if (window.innerWidth > 768) {
        const sidebar = document.getElementById('mainSidebar');
        if (sidebar) {
            sidebar.classList.remove('open');
        }
    }
});

console.log('adminLayout initialized successfully');