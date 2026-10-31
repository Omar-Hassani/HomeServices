// HomeServices — تفاعلات لوحة الإدارة
(function () {
    'use strict';

    // نافذة الملاحظة للقرارات التي تتطلب سبباً (رفض أو إيقاف ملف مقدم خدمة)
    const modalEl = document.getElementById('noteModal');
    if (modalEl && window.bootstrap) {
        const modal = window.bootstrap.Modal.getOrCreateInstance(modalEl);
        const form = modalEl.querySelector('[data-note-form]');
        const textarea = form.querySelector('textarea');

        document.querySelectorAll('[data-note-action]').forEach((btn) => {
            btn.addEventListener('click', () => {
                form.action = btn.dataset.noteAction;
                modalEl.querySelector('[data-note-title]').textContent = btn.dataset.noteTitle;
                modalEl.querySelector('[data-note-subtitle]').textContent = btn.dataset.noteSubtitle || '';
                modalEl.querySelector('[data-note-submit]').textContent = btn.dataset.noteButton || 'تأكيد';
                textarea.value = '';
                modal.show();
            });
        });

        modalEl.addEventListener('shown.bs.modal', () => textarea.focus());
    }

    // معاينة أيقونة ولون الفئة أثناء الكتابة
    const iconInput = document.querySelector('[data-icon-input]');
    const iconPreview = document.querySelector('[data-icon-preview]');
    if (iconInput && iconPreview) {
        const valid = /^fa-(solid|regular|brands) fa-[a-z0-9-]+$/;
        iconInput.addEventListener('input', () => {
            if (valid.test(iconInput.value.trim())) iconPreview.className = iconInput.value.trim();
        });
    }

    const colorInput = document.querySelector('[data-color-input]');
    if (colorInput && iconPreview) {
        colorInput.addEventListener('input', () => {
            iconPreview.parentElement.style.setProperty('--accent', colorInput.value);
        });
    }

    // فتح لوحة المحافظة المشار إليها في الرابط بعد الحفظ (#city-5)
    if (location.hash.startsWith('#city-')) {
        const panel = document.querySelector(`${location.hash} [data-city-panel]`);
        if (panel && window.bootstrap) {
            window.bootstrap.Collapse.getOrCreateInstance(panel, { toggle: false }).show();
        }
    }
})();
