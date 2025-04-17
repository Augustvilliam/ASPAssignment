document.addEventListener('DOMContentLoaded', () => {
    const overlay = document.getElementById('cookie-overlay');
    const analyticsCheckbox = document.getElementById('analytics-cookies');
    const marketingCheckbox = document.getElementById('marketing-cookies');

    const acceptBtn = document.getElementById('accept-selected');
    const denyBtn = document.getElementById('deny-all');
    const fab = document.getElementById('cookie-manager-btn');

    const cookiePrefs = JSON.parse(localStorage.getItem('cookiePreferences'));

    // Visa overlay om inget val gjorts
    if (!cookiePrefs) {
        overlay.classList.remove('hidden');
    } else {
        fab?.classList.remove('hidden');
        applyCookiePreferences(cookiePrefs);
    }

    // Hantera "Spara val"
    acceptBtn.addEventListener('click', () => {
        const prefs = {
            necessary: true,
            analytics: analyticsCheckbox.checked,
            marketing: marketingCheckbox.checked
        };
        savePreferences(prefs);
    });

    // Hantera "Neka alla"
    denyBtn.addEventListener('click', () => {
        const prefs = {
            necessary: true,
            analytics: false,
            marketing: false
        };
        savePreferences(prefs);
    });

    // Öppna bannern från FAB-knappen
    fab?.addEventListener('click', () => {
        const currentPrefs = JSON.parse(localStorage.getItem('cookiePreferences'));
        analyticsCheckbox.checked = currentPrefs?.analytics || false;
        marketingCheckbox.checked = currentPrefs?.marketing || false;
        overlay.classList.remove('hidden');
    });

    // Spara preferenser & tillämpa
    function savePreferences(prefs) {
        localStorage.setItem('cookiePreferences', JSON.stringify(prefs));
        overlay.classList.add('hidden');
        fab?.classList.remove('hidden');
        applyCookiePreferences(prefs);
    }

    // Här implementerar du laddning/rensning av cookies och tjänster
    function applyCookiePreferences(prefs) {
        if (prefs.analytics) {
            // Ladda analytics script, t.ex. Google Analytics
        } else {
            // Rensa analytics cookies om nödvändigt
        }

        if (prefs.marketing) {
            // Ladda marknadsföringstjänster
        } else {
            // Rensa marknadsföringscookies
        }
    }
});
