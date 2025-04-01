document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById("createprojectModal");
    const form = modal?.querySelector("form");
    const fileInput = document.getElementById("projectImageInput");
    const previewImg = document.getElementById("projectPreviewImage");
    const uploadBtn = document.getElementById("projectUploadBtn");

    if (!modal || !form) return;

    // 🖼 Förhandsgranska bild
    uploadBtn?.addEventListener("click", () => fileInput?.click());
    fileInput?.addEventListener("change", function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => previewImg.src = e.target.result;
            reader.readAsDataURL(file);
        }
    });

    // 🧼 Rensa input-valideringsklasser när användaren skriver
    form.querySelectorAll("input, textarea, select").forEach(input => {
        input.addEventListener("input", () => {
            input.classList.remove("input-validation-error");
            const span = form.querySelector(`span[data-valmsg-for="${input.name}"]`);
            if (span) span.textContent = "";
        });
    });

    // 🚀 Submit via AJAX
    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const data = new FormData(form);

        const globalErrorContainer = document.getElementById("create-project-errors");
        if (globalErrorContainer) globalErrorContainer.innerHTML = "";

        // Rensa gamla fel innan nytt försök
        form.querySelectorAll(".input-validation-error").forEach(i => i.classList.remove("input-validation-error"));
        form.querySelectorAll("span[data-valmsg-for]").forEach(s => s.textContent = "");

        try {
            const response = await fetch("/Project/Create", {
                method: "POST",
                body: data
            });

            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    bootstrap.Modal.getInstance(modal)?.hide();

                    const dynamicContent = document.getElementById("dynamic-content");
                    const viewResponse = await fetch("/Navigation/LoadProjects");
                    const html = await viewResponse.text();
                    dynamicContent.innerHTML = html;
                }
            } else {
                const errors = await response.json();

                errors.forEach(field => {
                    const input = form.querySelector(`[name="${field.field}"]`);
                    const validationSpan = form.querySelector(`span[data-valmsg-for="${field.field}"]`);

                    if (input) input.classList.add("input-validation-error");
                    if (validationSpan) validationSpan.textContent = field.errors.join(" ");
                });
            }
        } catch (err) {
            console.error("Error submitting project form", err);
            if (globalErrorContainer) {
                globalErrorContainer.innerHTML = "<div>An unexpected error occurred.</div>";
            }
        }
    });
});
