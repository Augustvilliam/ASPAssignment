document.addEventListener("DOMContentLoaded", () => {
    const container = document.getElementById("dynamic-content");
    if (!container) return;

    document.body.addEventListener("click", async e => {
        const t = e.target;

        // ── Sidomeny ─────────────────────────────────────────────
        if (t.matches("#load-projects")) {
            e.preventDefault();
            await loadView("/Navigation/LoadProjects");
            return;
        }
        if (t.matches("#load-team-members")) {
            e.preventDefault();
            await loadView("/Navigation/LoadMembers");
            return;
        }

        // ── Status-bar ───────────────────────────────────────────
        if (t.matches(".status-bar .navbar-link")) {
            e.preventDefault();
            const status = t.dataset.status || "";

            // 1) Ladda om projekten
            await loadView(`/Navigation/LoadProjects?status=${encodeURIComponent(status)}`);

            // 2) Sätt active på rätt länk i den nya status-baren
            document
                .querySelectorAll(".status-bar .navbar-link")
                .forEach(l => l.classList.remove("active"));
            const newLink = document.querySelector(
                `.status-bar .navbar-link[data-status="${status}"]`
            );
            if (newLink) newLink.classList.add("active");

            return;
        }

        // ── Projekts-pagination ───────────────────────────────────
        if (t.matches(".project-page")) {
            e.preventDefault();
            const page = t.dataset.page;
            const status = t.dataset.status || "";
            await loadView(`/Navigation/LoadProjects?status=${encodeURIComponent(status)}&page=${page}`);

            // Efter sidbyte: sätt active på länk med samma status som innan
            document
                .querySelectorAll(".status-bar .navbar-link")
                .forEach(l => l.classList.remove("active"));
            const activeLink = document.querySelector(
                `.status-bar .navbar-link[data-status="${status}"]`
            );
            if (activeLink) activeLink.classList.add("active");

            return;
        }

        // ── Members-pagination ───────────────────────────────────
        if (t.matches(".member-page")) {
            e.preventDefault();
            const page = t.dataset.page;
            await loadView(`/Navigation/LoadMembers?page=${page}`);
            return;
        }
    });

    // ── Global search ─────────────────────────────────────────
    const searchInput = document.getElementById("globalSearch");
    if (searchInput) {
        let timeout;
        searchInput.addEventListener("input", () => {
            clearTimeout(timeout);
            timeout = setTimeout(async () => {
                const term = encodeURIComponent(searchInput.value.trim());
                const isMembers = !!document.getElementById("MemberView");
                const url = isMembers
                    ? `/Navigation/LoadMembers?term=${term}`
                    : `/Navigation/LoadProjects?term=${term}`;
                await loadView(url);

                // Efter sökning: om vi är i projektvy, markera "ALL" eftersom status-term elimineras
                if (!isMembers) {
                    document
                        .querySelectorAll(".status-bar .navbar-link")
                        .forEach(l => l.classList.remove("active"));
                    const allLink = document.querySelector(`.status-bar .navbar-link[data-status=""]`);
                    if (allLink) allLink.classList.add("active");
                }
            }, 300);
        });
    }

    // ── Hjälpfunktion för att ladda in partial och fade-in ─────
    async function loadView(url) {
        container.classList.remove("visible");
        container.classList.add("fade-in");
        container.innerHTML = "";
        try {
            const html = await fetch(url).then(r => r.text());
            container.innerHTML = html;
            requestAnimationFrame(() => container.classList.add("visible"));
        } catch {
            container.innerHTML = "<div class='text-danger'>Kunde inte ladda innehållet.</div>";
        }
    }
});
