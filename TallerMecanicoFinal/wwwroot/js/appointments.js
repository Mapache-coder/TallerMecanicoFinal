(function () {
    const modal = document.getElementById('appointmentModal');
    const modalContent = document.getElementById('appointmentModalContent');

    if (!modal || !modalContent) {
        return;
    }

    const loadModal = async (url) => {
        const response = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
        modalContent.innerHTML = await response.text();
        if (window.jQuery && window.jQuery.validator && window.jQuery.validator.unobtrusive) {
            window.jQuery.validator.unobtrusive.parse(modalContent);
        }
    };

    modal.addEventListener('show.bs.modal', async (event) => {
        const trigger = event.relatedTarget;
        const url = trigger?.getAttribute('data-url');
        if (url) {
            await loadModal(url);
        }
    });
})();