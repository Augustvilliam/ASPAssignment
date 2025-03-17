document.addEventListener("DOMContentLoaded", function () {
    const toggleSwitch = document.getElementById("darkmode-switch");
    const rootElement = document.documentElement;

    // Kolla om dark mode är aktiverat i localStorage
    if (localStorage.getItem("theme") === "dark") {
        rootElement.setAttribute("data-theme", "dark");
        toggleSwitch.checked = true;
    }

    toggleSwitch.addEventListener("change", function () {
        if (this.checked) {
            rootElement.setAttribute("data-theme", "dark");
            localStorage.setItem("theme", "dark");
        } else {
            rootElement.removeAttribute("data-theme");
            localStorage.setItem("theme", "light");
        }
    });
});
