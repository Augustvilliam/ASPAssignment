document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("load-projects").addEventListener("click", function () {
        loadPartialView("/Navigation/LoadProjects");
    });

    document.getElementById("load-team-members").addEventListener("click", function () {
        loadPartialView("/Navigation/LoadTeamMembers");
    });

    function loadPartialView(url) {
        const container = document.querySelector("#dynamic-content");

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
