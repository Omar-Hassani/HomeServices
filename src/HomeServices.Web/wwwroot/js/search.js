// HomeServices — تفاعلات صفحات البحث والفئات
(function () {
    'use strict';

    // القوائم المترابطة: الفئة ← الخدمة، المحافظة ← المنطقة
    // القائمة الأم تحمل data-parent-select يشير للقائمة الفرعية، ومجموعات الفرعية تحمل data-parent
    document.querySelectorAll('[data-parent-select]').forEach((parent) => {
        const child = document.querySelector(parent.dataset.parentSelect);
        if (!child) return;

        const sync = (resetSelection) => {
            const parentValue = parent.value;
            child.querySelectorAll('optgroup[data-parent]').forEach((group) => {
                const visible = !parentValue || group.dataset.parent === parentValue;
                group.hidden = !visible;
                group.disabled = !visible;
            });

            const selected = child.selectedOptions[0];
            if (resetSelection && selected && selected.parentElement.disabled) {
                child.value = '';
            }
        };

        sync(false);
        parent.addEventListener('change', () => sync(true));

        // اختيار عنصر فرعي يضبط القائمة الأم تلقائياً
        child.addEventListener('change', () => {
            const group = child.selectedOptions[0]?.parentElement;
            if (group?.dataset.parent && parent.value !== group.dataset.parent) {
                parent.value = group.dataset.parent;
                sync(false);
            }
        });
    });

    // الإرسال التلقائي عند تغيير الترتيب
    document.querySelectorAll('[data-auto-submit]').forEach((el) => {
        el.addEventListener('change', () => el.form?.requestSubmit());
    });

    // عدم إرسال الحقول الفارغة للحصول على روابط نظيفة قابلة للمشاركة
    document.querySelectorAll('form[data-clean-empty]').forEach((form) => {
        form.addEventListener('submit', () => {
            form.querySelectorAll('input[name], select[name]').forEach((field) => {
                const isUncheckedChoice = (field.type === 'radio' || field.type === 'checkbox') && !field.checked;
                if (!isUncheckedChoice && field.value === '') {
                    field.disabled = true;
                }
            });
            // إعادة تفعيل الحقول إن عاد المستخدم للصفحة من ذاكرة المتصفح
            setTimeout(() => form.querySelectorAll('[disabled]').forEach((f) => {
                if (!f.closest('optgroup')) f.disabled = false;
            }), 0);
        });
    });
})();
