(function ($) {
    "use strict";
    
    // Dropdown on mouse hover
    $(document).ready(function () {
        function toggleNavbarMethod() {
            if ($(window).width() > 992) {
                $('.navbar .dropdown').on('mouseover', function () {
                    $('.dropdown-toggle', this).trigger('click');
                }).on('mouseout', function () {
                    $('.dropdown-toggle', this).trigger('click').blur();
                });
            } else {
                $('.navbar .dropdown').off('mouseover').off('mouseout');
            }
        }
        toggleNavbarMethod();
        $(window).resize(toggleNavbarMethod);
    });
    
    
    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({scrollTop: 0}, 1500, 'easeInOutExpo');
        return false;
    });
    

    // Date and time picker
    $('.date').datetimepicker({
        format: 'L'
    });
    $('.time').datetimepicker({
        format: 'LT'
    });


    // Testimonials carousel
    $(".testimonial-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 1500,
        margin: 30,
        dots: true,
        loop: true,
        center: true,
        responsive: {
            0:{
                items:1
            },
            576:{
                items:1
            },
            768:{
                items:2
            },
            992:{
                items:3
            }
        }
    });
    
})(jQuery);

window.adminLayout = {
    initResize: function (dotnetHelper) {
        // Initial check
        if (window.innerWidth) {
            dotnetHelper.invokeMethodAsync('OnResize', window.innerWidth);
        }

        // Setup resize listener
        this.resizeHandler = function () {
            dotnetHelper.invokeMethodAsync('OnResize', window.innerWidth);
        };

        window.addEventListener('resize', this.resizeHandler);

        // Also handle orientation change
        window.addEventListener('orientationchange', function () {
            setTimeout(function () {
                dotnetHelper.invokeMethodAsync('OnResize', window.innerWidth);
            }, 300);
        });
    },

    isMobile: function () {
        return window.innerWidth < 768;
    },

    removeResizeListener: function () {
        if (this.resizeHandler) {
            window.removeEventListener('resize', this.resizeHandler);
        }
    }
};
// ====== SIDEBAR TOGGLE FUNCTIONS ======
window.toggleSidebar = function () {
    const sidebar = document.getElementById('mainSidebar');
    const overlay = document.getElementById('sidebarOverlay');

    if (sidebar) {
        sidebar.classList.toggle('open');
    }
    if (overlay) {
        overlay.classList.toggle('active');
    }
};

window.closeSidebar = function () {
    const sidebar = document.getElementById('mainSidebar');
    const overlay = document.getElementById('sidebarOverlay');

    if (sidebar) {
        sidebar.classList.remove('open');
    }
    if (overlay) {
        overlay.classList.remove('active');
    }
};

window.openSidebar = function () {
    const sidebar = document.getElementById('mainSidebar');
    const overlay = document.getElementById('sidebarOverlay');

    if (sidebar) {
        sidebar.classList.add('open');
    }
    if (overlay) {
        overlay.classList.add('active');
    }
};

// Close sidebar when clicking overlay
document.addEventListener('DOMContentLoaded', function () {
    const overlay = document.getElementById('sidebarOverlay');
    if (overlay) {
        overlay.addEventListener('click', window.closeSidebar);
    }
});

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
        const overlay = document.getElementById('sidebarOverlay');
        if (sidebar) {
            sidebar.classList.remove('open');
        }
        if (overlay) {
            overlay.classList.remove('active');
        }
    }
});
