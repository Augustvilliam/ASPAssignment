document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("load-projects")?.addEventListener("click", function () {
        loadPartialView("/Navigation/LoadProjects");
    });

    document.getElementById("load-team-members")?.addEventListener("click", function () {
        loadPartialView("/Navigation/LoadTeamMembers");
    });

    document.querySelectorAll(".status-bar .navbar-link").forEach(link => {
        link.addEventListener("click", (e) => {
            e.preventDefault();
            const status = link.getAttribute("data-status");
            const url = status ? `/Navigation/LoadProjects?status=${status}` : "/Navigation/LoadProjects";
            loadPartialView(url);
        });
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
            })
            .catch(error => {
                container.innerHTML = "<div class='text-danger'>Kunde inte ladda innehållet.</div>";
                console.error("Error loading view:", error);
            });
    }
});
