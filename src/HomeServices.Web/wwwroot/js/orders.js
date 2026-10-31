// HomeServices — نموذج طلب الخدمة: تحديث السعر التقديري في الملخص عند اختيار الخدمة
(function () {
    'use strict';

    const priceTarget = document.querySelector('[data-selected-price]');
    if (!priceTarget) return;

    const update = () => {
        const selected = document.querySelector('input[name="ServiceId"]:checked');
        priceTarget.textContent = selected ? selected.dataset.price : 'اختر الخدمة';
    };

    document.querySelectorAll('input[name="ServiceId"]').forEach((input) => input.addEventListener('change', update));
    update();
})();
