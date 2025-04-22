document.addEventListener('DOMContentLoaded', () => {
    // ----- Elementreferenser -----
    const bodyEl = document.querySelector('.notis-body');
    const emptyEl = document.querySelector('.notis-body .no-notis');
    const bellBtn = document.querySelector('.bell-group .bell-btn');
    const notifContainer = document.querySelector('.notis-container');
    const notifMark = document.querySelector('.bell-group .notificaton-mark');
    const counterElem = document.querySelector('.notis-counter span');

    // ----- Helpers -----
    function updateMark() {
        const count = parseInt(counterElem.textContent, 10) || 0;
        notifMark.style.display = count > 0 ? 'block' : 'none';
    }
    function updateCounter(delta) {
        let count = parseInt(counterElem.textContent, 10) || 0;
        count = Math.max(0, count + delta);
        counterElem.textContent = count;
        updateMark();
        updateEmptyState()
    }
    function updateEmptyState() {
        // Läs in badge‑värdet som number
        const count = parseInt(counterElem.textContent, 10) || 0;
        // Visa "Inga notiser" bara om count === 0
        emptyEl.style.display = count === 0 ? 'block' : 'none';
    }

    // ----- Bell‑knapp: toggle panel -----
    bellBtn.addEventListener('click', e => {
        e.stopPropagation();
        notifContainer.classList.toggle('d-flex');
        bellBtn.setAttribute('aria-expanded', notifContainer.classList.contains('d-flex'));
    });

    // Klick utanför panelen stänger den
    document.addEventListener('click', e => {
        if (!notifContainer.contains(e.target) && !bellBtn.contains(e.target)) {
            notifContainer.classList.remove('d-flex');
            bellBtn.setAttribute('aria-expanded', 'false');
        }
    });

    // Dismiss‑knappar på redan existerande notiser
    document.querySelectorAll('.notis-body .close-btn').forEach(btn => {
        btn.addEventListener('click', e => {
            const el = e.target.closest('.notis');
            el?.remove();
            updateCounter(-1);
        });
    });

    // ----- SignalR‑anslutning -----
    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/notificationHub')
        .withAutomaticReconnect()
        .build();

    connection.on('ReceiveNotification', notification => {
        // Bygg notis-element
        const el = document.createElement('div');
        el.classList.add('notis');
        el.dataset.id = notification.notificationId;

        // Beräkna relativ tid
        const ts = new Date(notification.timestamp);
        const now = new Date();
        const diffMins = Math.floor((now - ts) / 60000);
        const timeText = diffMins === 0 ? 'just nu'
            : diffMins === 1 ? 'för 1 minut sedan'
                : `för ${diffMins} minuter sedan`;

        el.innerHTML = `
            <img src="${notification.imageUrl}" 
                 onerror="this.src='/img/proimg.svg'" alt="">
            <div class="notis-content">
              <span>${notification.message}</span>
              <p>${timeText}</p>
            </div>
            <button class="close-btn" aria-label="Stäng notis">×</button>
        `;

        // Hooka dismiss‑knapp
        el.querySelector('.close-btn').addEventListener('click', () => {
            el.remove();
            updateCounter(-1);
        });

        // Lägg till notisen överst
        bodyEl.prepend(el);
        // Öka räknaren och visa pricken om nödvändigt
        updateCounter(1);
    });

    connection.start()
        .then(() => console.log('Ansluten till NotificationHub'))
        .catch(err => console.error('SignalR-error:', err.toString()));

    // Initiera badge‑state
    updateMark();
    updateEmptyState()
});
