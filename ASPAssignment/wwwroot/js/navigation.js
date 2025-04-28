
document.addEventListener("DOMContentLoaded", function () {
    const projectsBtn = document.getElementById("load-projects");
    const teamBtn = document.getElementById("load-team-members");
    const container = document.getElementById("dynamic-content"); // Uppdaterad container

    if (projectsBtn)
        projectsBtn.addEventListener("click", () =>
            loadPartialView("/Navigation/LoadProjects"));

    if (teamBtn)
        teamBtn.addEventListener("click", () =>
            loadPartialView("/Navigation/LoadMembers"));

    function loadPartialView(url) {
        if (!container) return;

        container.classList.remove("visible");
        container.classList.add("fade-in");
        container.innerHTML = "";

        fetch(url)
            .then(response => response.text())
            .then(html => {
                container.innerHTML = html;
                requestAnimationFrame(() =>
                    container.classList.add("visible")
                );

                resetInitFlags?.();
                initAll?.();
                initStatusFilter?.();

                bindProjectPagination?.();
                bindMemberPagination?.();
            })
            .catch(error => {
                container.innerHTML = "<div class='text-danger'>Kunde inte ladda innehållet.</div>";
                console.error("Error loading view:", error);
            });
    }

    function initStatusFilter() {
        document.querySelectorAll(".status-bar .navbar-link").forEach(link => {
            link.addEventListener("click", function (e) {
                e.preventDefault();
                const status = this.dataset.status || "";

                document.querySelectorAll(".status-bar .navbar-link")
                    .forEach(l => l.classList.remove("active"));
                this.classList.add("active");

                loadPartialView(`/Navigation/LoadProjects?status=${status}`);
            });
        });
    }

    function bindProjectPagination() {
        document.querySelectorAll('.project-page').forEach(link => {
            link.addEventListener('click', e => {
                e.preventDefault();
                const page = link.dataset.page;
                const status = link.dataset.status || '';
                loadPartialView(`/Navigation/LoadProjects?status=${status}&page=${page}`);
            });
        });
    }

    function initGlobalSearch() {
        const input = document.getElementById("globalSearch");
        if (!input) return;
        let timeout;
        input.addEventListener("input", () => {
            clearTimeout(timeout);
            const term = input.value.trim();
            timeout = setTimeout(() => {
                // Avgör om vi är i Members-vyn genom att leta efter #MemberView
                const isMemberView = !!document.getElementById("MemberView");
                if (isMemberView) {
                    // Ladda om med term i Members
                    loadPartialView(`/Navigation/LoadMembers?term=${encodeURIComponent(term)}`);
                } else {
                    // Ladda om med term i Projects
                    loadPartialView(`/Navigation/LoadProjects?term=${encodeURIComponent(term)}`);
                }
            }, 300);
        });
    }
    function bindMemberPagination() {
        document.querySelectorAll('.member-page').forEach(link => {
            link.addEventListener('click', e => {
                e.preventDefault();
                const page = link.dataset.page;
                loadPartialView(`/Navigation/LoadMembers?page=${page}`);
            });
        });
    }

    // Initial bindings
    initStatusFilter();
    bindProjectPagination();
    bindMemberPagination();
    initGlobalSearch();
});
