
        document.addEventListener('DOMContentLoaded', () => {
            const bellBtn = document.querySelector('.bell-group .bell-btn');
        const notifContainer = document.querySelector('.notis-container');
        const notifMark = document.querySelector('.bell-group .notificaton-mark');
        const counterElem = document.querySelector('.notis-counter span');

        // Helper för att uppdatera den röda pricken baserat på antalet notiser
        function updateMark() {
                const count = parseInt(counterElem.textContent, 10) || 0;
                if (count > 0) {
            notifMark.style.display = 'block';
                } else {
            notifMark.style.display = 'none';
                }
            }

        // Helper för att uppdatera badge‑räknaren
        function updateCounter(delta) {
            let count = parseInt(counterElem.textContent, 10) || 0;
        count = Math.max(0, count + delta);
        counterElem.textContent = count;
        updateMark();
            }

            // Klick på bell‑knappen togglar notispanelen
            bellBtn.addEventListener('click', (e) => {
            e.stopPropagation();
        notifContainer.classList.toggle('d-flex');
        // Sätt aria-expanded för a11y
        bellBtn.setAttribute('aria-expanded', notifContainer.classList.contains('d-flex'));
            });

            // Klick utanför panelen stänger den
            document.addEventListener('click', (e) => {
                if (!notifContainer.contains(e.target) && !bellBtn.contains(e.target)) {
            notifContainer.classList.remove('d-flex');
        bellBtn.setAttribute('aria-expanded', 'false');
                }
            });

            // Dismiss‑knappar på notiser
            document.querySelectorAll('.notis-body .close-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const notisEl = e.target.closest('.notis');
                notisEl.remove();
                updateCounter(-1);
            });
            });

        // Initiera initial state
        updateMark();
        });
