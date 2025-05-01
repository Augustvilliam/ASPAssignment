document.addEventListener("DOMContentLoaded", () => {
    const container = document.getElementById("dynamic-content");
    if (!container) return;

    document.body.addEventListener("click", async e => {
        const t = e.target;

        // Sidomeny
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

        // Status-bar
        if (t.matches(".status-bar .navbar-link")) {
            e.preventDefault();
            const status = t.dataset.status || "";
            activateStatus(t);
            await loadView(`/Navigation/LoadProjects?status=${encodeURIComponent(status)}`);
            return;
        }

        // Projekts-pagination
        if (t.matches(".project-page")) {
            e.preventDefault();
            const page = t.dataset.page;
            const status = t.dataset.status || "";
            await loadView(`/Navigation/LoadProjects?status=${encodeURIComponent(status)}&page=${page}`);
            return;
        }

        // Members-pagination
        if (t.matches(".member-page")) {
            e.preventDefault();
            const page = t.dataset.page;
            await loadView(`/Navigation/LoadMembers?page=${page}`);
            return;
        }
    });

    // Global search
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
            }, 300);
        });
    }

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

    function activateStatus(el) {
        document.querySelectorAll(".status-bar .navbar-link")
            .forEach(l => l.classList.remove("active"));
        el.classList.add("active");
    }
});
