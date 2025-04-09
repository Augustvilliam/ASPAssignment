document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("load-projects").addEventListener("click", function () {
        loadPartialView("/Navigation/LoadProjects");
    });

    document.getElementById("load-team-members").addEventListener("click", function () {
        loadPartialView("/Navigation/LoadTeamMembers");
    });

    function loadPartialView(url) {
        const container = document.querySelector(".hero");

        container.classList.remove("visible");
        container.classList.add("fade-in");
        container.innerHTML = "";

        fetch(url)
            .then(response => response.text())
            .then(html => {
                container.innerHTML = html;
                requestAnimationFrame(() => {
                    container.classList.add("visible");
                });

                // Re-init funktionalitet efter reload
                initMoreMenu();
                initStatusFilter();
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

                const status = this.getAttribute("data-status") || "";

                // Markera aktiv
                document.querySelectorAll(".status-bar .navbar-link").forEach(l => l.classList.remove("active"));
                this.classList.add("active");

                loadPartialView(`/Navigation/LoadProjects${status ? `?status=${status}` : ""}`);
            });
        });
    }

    function initMoreMenu() {
        document.querySelectorAll(".more-btn").forEach(btn => {
            btn.addEventListener("click", (e) => {
                e.preventDefault();
                const container = btn.closest(".more-container");
                const menu = container.querySelector(".more-menu");

                // Stäng andra menyer
                document.querySelectorAll(".more-menu").forEach(m => {
                    if (m !== menu) m.classList.add("d-none");
                });

                menu.classList.toggle("d-none");
            });
        });

        document.addEventListener("click", (e) => {
            if (!e.target.closest(".more-container")) {
                document.querySelectorAll(".more-menu").forEach(menu => menu.classList.add("d-none"));
            }
        });
    }

    // Initiera filter vid första laddning
    initStatusFilter();
});
