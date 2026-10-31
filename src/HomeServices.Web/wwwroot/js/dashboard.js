// HomeServices — تفاعلات لوحة مقدم الخدمة
(function () {
    'use strict';

    // عدّاد الأحرف للحقول ذات الحد الأقصى
    document.querySelectorAll('[data-char-counter]').forEach((field) => {
        const counter = field.parentElement.querySelector('.char-counter');
        const max = parseInt(field.getAttribute('maxlength'), 10);
        if (!counter || !max) return;
        const update = () => { counter.textContent = `${field.value.length} / ${max}`; };
        update();
        field.addEventListener('input', update);
    });

    // معاينة الصورة قبل رفعها
    document.querySelectorAll('input[type="file"][data-image-preview]').forEach((input) => {
        const zone = input.closest('[data-image-preview-target]');
        const preview = zone?.querySelector('.dropzone-preview');
        const empty = zone?.querySelector('.dropzone-empty');
        if (!preview) return;

        input.addEventListener('change', () => {
            const file = input.files?.[0];
            if (!file) return;
            if (preview.src) URL.revokeObjectURL(preview.src);
            preview.src = URL.createObjectURL(file);
            preview.hidden = false;
            if (empty) empty.hidden = true;
            zone.classList.add('has-file');
        });
    });

    // تعديل خدمة: تعبئة النموذج من بيانات الصف
    const serviceForm = document.getElementById('service-form');
    if (serviceForm) {
        const title = document.querySelector('[data-form-title]');
        const resetBtn = serviceForm.querySelector('[data-form-reset]');
        const category = document.getElementById('service-category');
        const fields = {
            service: document.getElementById('Form_ServiceId'),
            price: document.getElementById('Form_Price'),
            unit: document.getElementById('Form_PriceUnit'),
            notes: document.getElementById('Form_Notes')
        };

        const setMode = (editing) => {
            title.textContent = editing ? title.dataset.editTitle : title.dataset.addTitle;
            resetBtn.classList.toggle('d-none', !editing);
            document.getElementById('service-form-panel').classList.toggle('is-editing', editing);
        };

        document.querySelectorAll('[data-edit-service]').forEach((btn) => {
            btn.addEventListener('click', () => {
                category.value = btn.dataset.categoryId;
                category.dispatchEvent(new Event('change'));
                fields.service.value = btn.dataset.serviceId;
                fields.price.value = btn.dataset.price;
                fields.unit.value = btn.dataset.unit;
                fields.notes.value = btn.dataset.notes || '';
                setMode(true);
                document.getElementById('service-form-panel').scrollIntoView({ behavior: 'smooth', block: 'start' });
                fields.price.focus({ preventScroll: true });
            });
        });

        resetBtn.addEventListener('click', () => {
            serviceForm.reset();
            category.value = '';
            category.dispatchEvent(new Event('change'));
            setMode(false);
        });
    }

    // مناطق العمل: عدّاد المناطق المختارة وتحديد الكل لكل محافظة
    const areaCounter = document.querySelector('[data-area-counter]');
    if (areaCounter) {
        const boxes = () => document.querySelectorAll('input[name="areaIds"]');
        const refresh = () => {
            areaCounter.textContent = String([...boxes()].filter((b) => b.checked).length);
            document.querySelectorAll('[data-city-group]').forEach((group) => {
                const count = group.querySelectorAll('input[name="areaIds"]:checked').length;
                group.querySelector('[data-city-count]').textContent = count > 0 ? String(count) : '';
            });
        };

        boxes().forEach((box) => box.addEventListener('change', refresh));

        document.querySelectorAll('[data-select-all-city]').forEach((btn) => {
            btn.addEventListener('click', () => {
                const groupBoxes = btn.closest('[data-city-group]').querySelectorAll('input[name="areaIds"]');
                const allChecked = [...groupBoxes].every((b) => b.checked);
                groupBoxes.forEach((b) => { b.checked = !allChecked; });
                refresh();
            });
        });
    }
})();
