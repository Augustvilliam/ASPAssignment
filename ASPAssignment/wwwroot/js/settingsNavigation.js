document.addEventListener("DOMContentLoaded", function () {
    const navLinks = document.querySelectorAll(".settings-nav");
    const content = document.querySelector(".settings-content");

    navLinks.forEach(link => {
        link.addEventListener("click", async (e) => {
            e.preventDefault();
            const section = link.dataset.section;

            const response = await fetch(`/Settings/LoadSection?section=${section}`);
            const html = await response.text();
            content.innerHTML = html;

            navLinks.forEach(l => l.classList.remove("active"));
            link.classList.add("active");
        });
    });
});
