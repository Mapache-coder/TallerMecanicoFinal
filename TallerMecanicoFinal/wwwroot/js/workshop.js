(function () {
    const modal = document.getElementById('workshopModal');
    const modalContent = document.getElementById('workshopModalContent');
    const boardSelector = '#workshopBoard';

    if (!modal || !modalContent) {
        return;
    }

    const refreshBoard = async (url) => {
        const response = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
        const html = await response.text();
        const board = document.querySelector(boardSelector);
        if (board) {
            board.outerHTML = html;
        }
    };

    const loadModal = async (url) => {
        const response = await fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
        modalContent.innerHTML = await response.text();
        if (window.jQuery && window.jQuery.validator && window.jQuery.validator.unobtrusive) {
            window.jQuery.validator.unobtrusive.parse(modalContent);
        }
    };

    modal.addEventListener('show.bs.modal', async (event) => {
        const trigger = event.relatedTarget;
        const url = trigger?.href;
        if (url) {
            await loadModal(url);
        }
    });

    modalContent.addEventListener('submit', async (event) => {
        const form = event.target;
        if (!(form instanceof HTMLFormElement)) {
            return;
        }

        event.preventDefault();

        const response = await fetch(form.action, {
            method: form.method || 'POST',
            body: new FormData(form),
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        const contentType = response.headers.get('content-type') ?? '';
        if (contentType.includes('application/json')) {
            const data = await response.json();
            if (data.refreshUrl) {
                const bootstrapModal = bootstrap.Modal.getInstance(modal);
                bootstrapModal?.hide();
                await refreshBoard(data.refreshUrl);
            }
            return;
        }

        modalContent.innerHTML = await response.text();
        if (window.jQuery && window.jQuery.validator && window.jQuery.validator.unobtrusive) {
            window.jQuery.validator.unobtrusive.parse(modalContent);
        }
    });
})();