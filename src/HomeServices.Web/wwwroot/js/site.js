// HomeServices — تفاعلات عامة على مستوى الموقع
(function () {
    'use strict';

    const prefersReducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    // ظل الهيدر عند التمرير
    const header = document.getElementById('site-header');
    if (header) {
        const updateHeader = () => header.classList.toggle('is-scrolled', window.scrollY > 8);
        updateHeader();
        window.addEventListener('scroll', updateHeader, { passive: true });
    }

    // تمييز رابط التنقل الحالي
    document.querySelectorAll('.main-nav a').forEach((link) => {
        const url = new URL(link.href, window.location.origin);
        if (!url.hash && url.pathname.toLowerCase() === window.location.pathname.toLowerCase()) {
            link.classList.add('active');
            link.setAttribute('aria-current', 'page');
        }
    });

    // الظهور التدريجي للعناصر عند دخولها مجال الرؤية
    const revealItems = document.querySelectorAll('[data-reveal]');
    if (!('IntersectionObserver' in window) || prefersReducedMotion) {
        revealItems.forEach((el) => el.classList.add('is-visible'));
    } else {
        const revealObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.12, rootMargin: '0px 0px -40px 0px' });

        revealItems.forEach((el) => revealObserver.observe(el));
    }

    // عدّاد متحرك للأرقام
    const counters = document.querySelectorAll('[data-count-to]');
    if (counters.length && 'IntersectionObserver' in window && !prefersReducedMotion) {
        const formatter = new Intl.NumberFormat('en-US');
        const animate = (el) => {
            const target = parseInt(el.dataset.countTo, 10) || 0;
            const duration = 1200;
            const start = performance.now();
            const step = (now) => {
                const progress = Math.min((now - start) / duration, 1);
                const eased = 1 - Math.pow(1 - progress, 3);
                el.textContent = formatter.format(Math.round(target * eased));
                if (progress < 1) requestAnimationFrame(step);
            };
            requestAnimationFrame(step);
        };

        const counterObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach((entry) => {
                if (entry.isIntersecting) {
                    animate(entry.target);
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.6 });

        counters.forEach((el) => counterObserver.observe(el));
    }

    // اقتراحات البحث: تعبئة الحقل وإرسال النموذج
    document.querySelectorAll('[data-suggestion]').forEach((chip) => {
        chip.addEventListener('click', () => {
            const form = document.querySelector('[data-search-form]');
            const input = form?.querySelector('input[name="q"]');
            if (!form || !input) return;
            input.value = chip.dataset.suggestion;
            form.requestSubmit();
        });
    });

    // حالة التحميل لأزرار الإرسال ومنع الإرسال المزدوج
    document.addEventListener('submit', (event) => {
        const form = event.target;
        if (!(form instanceof HTMLFormElement)) return;

        // احترام التحقق من جهة العميل (jQuery Validation) إن وُجد
        if (window.jQuery && window.jQuery(form).data('validator') && !window.jQuery(form).valid()) return;

        const button = form.querySelector('button[type="submit"]:not([data-no-loading])');
        if (!button || button.classList.contains('is-loading')) return;

        button.classList.add('is-loading');
        button.setAttribute('aria-busy', 'true');
        const spinner = document.createElement('span');
        spinner.className = 'spinner-border spinner-border-sm';
        spinner.setAttribute('aria-hidden', 'true');
        button.prepend(spinner);
        button.querySelector('i')?.classList.add('d-none');
    });

    // استعادة الأزرار عند الرجوع للصفحة من ذاكرة المتصفح
    window.addEventListener('pageshow', (event) => {
        if (!event.persisted) return;
        document.querySelectorAll('.btn.is-loading').forEach((button) => {
            button.classList.remove('is-loading');
            button.removeAttribute('aria-busy');
            button.querySelector('.spinner-border')?.remove();
            button.querySelector('i.d-none')?.classList.remove('d-none');
        });
    });

    // إظهار التنبيهات القادمة من الخادم
    if (window.bootstrap) {
        document.querySelectorAll('.toast[data-autoshow]').forEach((el) => {
            window.bootstrap.Toast.getOrCreateInstance(el, { delay: 5000 }).show();
        });
    }
})();
