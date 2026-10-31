// HomeServices — تفاعلات صفحات الطلبات
(function () {
    'use strict';

    // نموذج طلب الخدمة: تحديث السعر التقديري في الملخص عند اختيار الخدمة
    const priceTarget = document.querySelector('[data-selected-price]');
    if (priceTarget) {
        const updatePrice = () => {
            const selected = document.querySelector('input[name="ServiceId"]:checked');
            priceTarget.textContent = selected ? selected.dataset.price : 'اختر الخدمة';
        };
        document.querySelectorAll('input[name="ServiceId"]').forEach((input) => input.addEventListener('change', updatePrice));
        updatePrice();
    }

    // نموذج التقييم: وصف نصي لعدد النجوم المختار
    const ratingLabel = document.querySelector('[data-rating-label]');
    if (ratingLabel) {
        const labels = { 1: 'سيئة', 2: 'مقبولة', 3: 'جيدة', 4: 'جيدة جداً', 5: 'ممتازة' };
        document.querySelectorAll('input[name="Rating"]').forEach((input) => {
            input.addEventListener('change', () => {
                ratingLabel.textContent = `تجربة ${labels[input.value]}`;
                ratingLabel.classList.add('is-set');
            });
        });
    }
})();
