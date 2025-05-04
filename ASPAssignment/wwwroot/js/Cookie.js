document.addEventListener('DOMContentLoaded', () => {
    const overlay = document.getElementById('cookie-overlay');
    const analyticsCheckbox = document.getElementById('analytics-cookies');
    const marketingCheckbox = document.getElementById('marketing-cookies');
    const acceptBtn = document.getElementById('accept-selected');
    const denyBtn = document.getElementById('deny-all');
    const fab = document.getElementById('cookie-manager-btn');

    // Läs in sparat val
    let cookiePrefs;
    try {
        cookiePrefs = JSON.parse(localStorage.getItem('cookiePreferences'));
    } catch {
        cookiePrefs = null;
    }

    // Visa overlay om inget val gjorts
    if (!cookiePrefs) {
        overlay?.classList.remove('hidden');
    } else {
        fab?.classList.remove('hidden');
        applyCookiePreferences(cookiePrefs);
    }

    // Skydda mot null-element
    acceptBtn?.addEventListener('click', () => savePreferences({
        necessary: true,
        analytics: analyticsCheckbox?.checked,
        marketing: marketingCheckbox?.checked
    }));

    denyBtn?.addEventListener('click', () => savePreferences({
        necessary: true,
        analytics: false,
        marketing: false
    }));

    fab?.addEventListener('click', () => {
        const currentPrefs = JSON.parse(localStorage.getItem('cookiePreferences'));
        analyticsCheckbox.checked = currentPrefs?.analytics || false;
        marketingCheckbox.checked = currentPrefs?.marketing || false;
        overlay?.classList.remove('hidden');
    });

    function savePreferences(prefs) {
        localStorage.setItem('cookiePreferences', JSON.stringify(prefs));
        overlay?.classList.add('hidden');
        fab?.classList.remove('hidden');
        applyCookiePreferences(prefs);
    }

    function applyCookiePreferences(prefs) {
        // —— Analytics ——
        if (prefs.analytics) {
            // Exempel: dynamiskt ladda GA
            if (!window.gaLoaded) {
                const script = document.createElement('script');
                script.src = 'https://www.googletagmanager.com/gtag/js?id=UA-XXXXXXXXX-X';
                script.async = true;
                document.head.appendChild(script);

                window.dataLayer = window.dataLayer || [];
                function gtag() { dataLayer.push(arguments); }
                gtag('js', new Date());
                gtag('config', 'UA-XXXXXXXXX-X');

                window.gaLoaded = true;
            }
        } else {
            // Radera ev. analytics‐cookie
            document.cookie = '_ga=; Max-Age=0; path=/';
            document.cookie = '_gid=; Max-Age=0; path=/';
        }

        // —— Marketing ——
        if (prefs.marketing) {
            // Ladda t.ex. Facebook Pixel
            if (!window.fbq) {
                const script = document.createElement('script');
                script.src = 'https://connect.facebook.net/en_US/fbevents.js';
                document.head.appendChild(script);
                window.fbq = function () { window.fbq.callMethod ? window.fbq.callMethod.apply(window.fbq, arguments) : window.fbq.queue.push(arguments); };
                fbq.queue = fbq.queue || [];
                fbq('init', 'YOUR_PIXEL_ID');
                fbq('track', 'PageView');
            }
        } else {
            // Rensa marknadsföringscookies
            document.cookie = '_fbp=; Max-Age=0; path=/';
        }
    }
});
