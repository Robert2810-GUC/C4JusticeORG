/*!
 * C4Justice.org - Custom JavaScript
 * Communities United for Justice Fund
 */

(function () {
    'use strict';

    // ============================================================
    // PRELOADER
    // ============================================================
    window.addEventListener('load', function () {
        const preloader = document.getElementById('preloader');
        if (preloader) {
            setTimeout(function () {
                preloader.classList.add('loaded');
                setTimeout(function () {
                    preloader.remove();
                }, 500);
            }, 600);
        }
    });

    // ============================================================
    // THEME TOGGLE (Dark / Light)
    // ============================================================
    (function initTheme() {
        const savedTheme = localStorage.getItem('c4j-theme') || 'dark';
        document.documentElement.setAttribute('data-theme', savedTheme);
    })();

    document.addEventListener('DOMContentLoaded', function () {

        const themeToggleBtn = document.getElementById('themeToggle');
        if (themeToggleBtn) {
            themeToggleBtn.addEventListener('click', function () {
                const current = document.documentElement.getAttribute('data-theme');
                const next = current === 'dark' ? 'light' : 'dark';
                document.documentElement.setAttribute('data-theme', next);
                localStorage.setItem('c4j-theme', next);
            });
        }

        // ============================================================
        // AOS INIT
        // ============================================================
        if (typeof AOS !== 'undefined') {
            AOS.init({
                duration: 800,
                easing: 'ease-out-cubic',
                once: true,
                offset: 80,
                disable: 'mobile'
            });
        }

        // ========================================================
        // NAVBAR SCROLL EFFECT
        // ========================================================
        const navbar = document.getElementById('mainNav');
        if (navbar) {
            const handleScroll = function () {
                if (window.scrollY > 50) {
                    navbar.classList.add('scrolled');
                } else {
                    navbar.classList.remove('scrolled');
                }
            };
            window.addEventListener('scroll', handleScroll, { passive: true });
            handleScroll();
        }

        // ========================================================
        // COUNTER ANIMATION
        // ========================================================
        const statNumbers = document.querySelectorAll('.stat-number[data-count]');

        if (statNumbers.length > 0) {
            const observerOptions = {
                threshold: 0.5,
                rootMargin: '0px 0px -50px 0px'
            };

            const observer = new IntersectionObserver(function (entries) {
                entries.forEach(function (entry) {
                    if (entry.isIntersecting) {
                        const el = entry.target;
                        const target = parseInt(el.getAttribute('data-count'));
                        const suffix = '+';
                        let current = 0;
                        const duration = 2000;
                        const step = target / (duration / 16);

                        const timer = setInterval(function () {
                            current += step;
                            if (current >= target) {
                                current = target;
                                clearInterval(timer);
                            }

                            let display = Math.floor(current);
                            if (display >= 1000) {
                                display = (display / 1000).toFixed(1) + 'K';
                            }

                            el.textContent = display + (current >= target ? suffix : '');
                        }, 16);

                        observer.unobserve(el);
                    }
                });
            }, observerOptions);

            statNumbers.forEach(function (el) {
                observer.observe(el);
            });
        }

        // ========================================================
        // SMOOTH SCROLL for anchor links
        // ========================================================
        document.querySelectorAll('a[href^="#"]').forEach(function (anchor) {
            anchor.addEventListener('click', function (e) {
                const href = this.getAttribute('href');
                if (href === '#') return;

                const target = document.querySelector(href);
                if (target) {
                    e.preventDefault();
                    const navHeight = document.getElementById('mainNav')?.offsetHeight || 80;
                    const targetPosition = target.getBoundingClientRect().top + window.pageYOffset - navHeight - 20;

                    window.scrollTo({
                        top: targetPosition,
                        behavior: 'smooth'
                    });

                    // Close mobile nav if open
                    const navCollapse = document.getElementById('navbarNav');
                    if (navCollapse && navCollapse.classList.contains('show')) {
                        const bsCollapse = bootstrap.Collapse.getInstance(navCollapse);
                        if (bsCollapse) bsCollapse.hide();
                    }
                }
            });
        });

        // ========================================================
        // HERO IMAGE SLIDER
        // ========================================================
        const slider = document.getElementById('heroSlider');
        if (slider) {
            const slides = slider.querySelectorAll('.hero-slide');
            const dots = document.querySelectorAll('.slider-dot');
            const prevBtn = document.getElementById('sliderPrev');
            const nextBtn = document.getElementById('sliderNext');
            let currentSlide = 0;
            let autoPlayTimer = null;

            function goToSlide(index) {
                slides[currentSlide].classList.remove('active');
                if (dots[currentSlide]) dots[currentSlide].classList.remove('active');

                currentSlide = (index + slides.length) % slides.length;

                slides[currentSlide].classList.add('active');
                if (dots[currentSlide]) dots[currentSlide].classList.add('active');
            }

            function startAutoPlay() {
                stopAutoPlay();
                autoPlayTimer = setInterval(function () {
                    goToSlide(currentSlide + 1);
                }, 5000);
            }

            function stopAutoPlay() {
                if (autoPlayTimer) {
                    clearInterval(autoPlayTimer);
                    autoPlayTimer = null;
                }
            }

            if (prevBtn) {
                prevBtn.addEventListener('click', function () {
                    goToSlide(currentSlide - 1);
                    startAutoPlay();
                });
            }

            if (nextBtn) {
                nextBtn.addEventListener('click', function () {
                    goToSlide(currentSlide + 1);
                    startAutoPlay();
                });
            }

            dots.forEach(function (dot, i) {
                dot.addEventListener('click', function () {
                    goToSlide(i);
                    startAutoPlay();
                });
            });

            // Pause on hover
            slider.addEventListener('mouseenter', stopAutoPlay);
            slider.addEventListener('mouseleave', startAutoPlay);

            // Start auto-play
            if (slides.length > 1) {
                startAutoPlay();
            }
        }

        // ========================================================
        // HERO PARTICLE ANIMATION (Enhanced)
        // ========================================================
        const heroParticles = document.querySelector('.hero-particles');
        if (heroParticles) {
            let mouseX = 0, mouseY = 0;
            document.addEventListener('mousemove', function (e) {
                mouseX = (e.clientX / window.innerWidth - 0.5) * 20;
                mouseY = (e.clientY / window.innerHeight - 0.5) * 20;
                heroParticles.style.transform = `translate(${mouseX}px, ${mouseY}px)`;
            }, { passive: true });
        }

        // ========================================================
        // PILLAR CARD TILT EFFECT
        // ========================================================
        const tiltCards = document.querySelectorAll('.pillar-card, .event-card, .cta-card');
        tiltCards.forEach(function (card) {
            card.addEventListener('mousemove', function (e) {
                const rect = card.getBoundingClientRect();
                const x = e.clientX - rect.left;
                const y = e.clientY - rect.top;
                const centerX = rect.width / 2;
                const centerY = rect.height / 2;
                const rotateX = (y - centerY) / centerY * -5;
                const rotateY = (x - centerX) / centerX * 5;

                card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateY(-6px)`;
            });

            card.addEventListener('mouseleave', function () {
                card.style.transform = '';
            });
        });

        // ========================================================
        // FORM VALIDATION FEEDBACK
        // ========================================================
        const forms = document.querySelectorAll('form');
        forms.forEach(function (form) {
            const inputs = form.querySelectorAll('.form-input-c4j');
            inputs.forEach(function (input) {
                input.addEventListener('blur', function () {
                    if (this.value.trim() !== '' && this.checkValidity()) {
                        this.style.borderColor = 'rgba(22,163,74,0.5)';
                        this.style.boxShadow = '0 0 0 3px rgba(22,163,74,0.1)';
                    } else if (this.value.trim() !== '') {
                        this.style.borderColor = 'rgba(239,68,68,0.5)';
                        this.style.boxShadow = '0 0 0 3px rgba(239,68,68,0.1)';
                    }
                });

                input.addEventListener('focus', function () {
                    this.style.borderColor = '';
                    this.style.boxShadow = '';
                });
            });
        });

        // ========================================================
        // SCROLL INDICATOR FADE
        // ========================================================
        const scrollIndicator = document.querySelector('.scroll-indicator');
        if (scrollIndicator) {
            window.addEventListener('scroll', function () {
                if (window.scrollY > 100) {
                    scrollIndicator.style.opacity = '0';
                } else {
                    scrollIndicator.style.opacity = '1';
                }
            }, { passive: true });
        }

        // ========================================================
        // PAGE TRANSITION
        // ========================================================
        document.querySelectorAll('a:not([href^="#"]):not([target="_blank"]):not([href^="mailto"]):not([href^="tel"])').forEach(function (link) {
            link.addEventListener('click', function (e) {
                const href = this.getAttribute('href');
                if (!href || href.startsWith('http') || href.startsWith('//')) return;

                e.preventDefault();
                document.body.style.opacity = '0';
                document.body.style.transition = 'opacity 0.3s ease';

                setTimeout(function () {
                    window.location.href = href;
                }, 300);
            });
        });

        // Fade in on page load
        document.body.style.opacity = '0';
        document.body.style.transition = 'opacity 0.5s ease';
        requestAnimationFrame(function () {
            document.body.style.opacity = '1';
        });

    }); // end DOMContentLoaded

})();
