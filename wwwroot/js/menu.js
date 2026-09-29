window.lumenMenu = (function () {
    let observer = null;
    let handlersAttached = false;

    function init() {
        // Prevent double-init
        if (handlersAttached) return;

        const categorySlider = document.getElementById('categorySlider');
        const scrollTopBtn = document.getElementById('scrollToTop');
        const header = document.querySelector('header');

        // --- Scroll handler (sticky + scroll-to-top) ---
        window.addEventListener('scroll', () => {
            const heroHeight = header ? header.offsetHeight : 0;

            if (window.scrollY >= heroHeight) {
                categorySlider?.classList.add('sticky');
            } else {
                categorySlider?.classList.remove('sticky');
            }

            if (window.scrollY >= 600) {
                if (scrollTopBtn) scrollTopBtn.style.display = "block";
            } else {
                if (scrollTopBtn) scrollTopBtn.style.display = "none";
            }
        });
        window.downloadFile = function (filename, contentType, base64) {
            const link = document.createElement('a');
            link.download = filename;
            link.href = `data:${contentType};base64,${base64}`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        };
        // --- Category tag clicks (smooth scroll) ---
        document.querySelectorAll('.category-tag').forEach(tag => {
            tag.addEventListener('click', () => {
                const targetId = tag.getAttribute('data-target');
                const targetSection = document.getElementById(targetId);
                if (targetSection) {
                    targetSection.scrollIntoView({ behavior: 'smooth' });
                }
            });
        });

        // --- Scroll-to-top button ---
        scrollTopBtn?.addEventListener('click', () => {
            window.scrollTo({ top: 0, behavior: 'smooth' });
        });

        // --- IntersectionObserver for active tag ---
        const sections = document.querySelectorAll('.menu-section');
        const observerOptions = { threshold: [0.25, 0.5, 0.75] };

        observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    document.querySelectorAll('.category-tag').forEach(tag =>
                        tag.classList.remove('active'));

                    const activeTag = document.querySelector(
                        `.category-tag[data-target="${entry.target.id}"]`);

                    if (activeTag) {
                        activeTag.classList.add('active');
                        setTimeout(() => {
                            activeTag.scrollIntoView({
                                behavior: 'smooth',
                                inline: 'center',
                                block: 'nearest'
                            });
                        }, 100);
                    }
                }
            });
        }, observerOptions);

        sections.forEach(section => observer.observe(section));

        handlersAttached = true;
    }

    return { init };
})();