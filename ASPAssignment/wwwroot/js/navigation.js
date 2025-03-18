
document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("load-projects").addEventListener("click", function () {
        loadPartialView("/Navigation/LoadProjects");
    });

    document.getElementById("load-team-members").addEventListener("click", function () {
        loadPartialView("/Navigation/LoadTeamMembers");
    });

    function loadPartialView(url) {
        fetch(url)
            .then(response => response.text())
            .then(html => {
                document.getElementById("dynamic-content").innerHTML = html;
            })
            .catch(error => console.error("Error loading view:", error));
    }
});