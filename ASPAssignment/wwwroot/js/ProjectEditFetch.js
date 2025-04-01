document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById("editprojectModal");
    const form = modal?.querySelector("form");
    const previewImg = document.getElementById("editProjectPreviewImage");
    const fileInput = document.getElementById("editProjectImageInput");
    const uploadBtn = document.getElementById("editProjectUploadBtn");

    if (!modal || !form) return;

    // 🧠 Ladda data vid öppning av modal
    modal.addEventListener("show.bs.modal", async (event) => {
        console.log("🔍 Modal opened!");

        const button = event.relatedTarget;
        console.log("🧪 relatedTarget:", button);

        const projectId = button?.getAttribute("data-project-id");
        console.log("📦 projectId:", projectId);

        if (!projectId) return;

        try {
            const response = await fetch(`/Project/GetProject/${projectId}`);
            const project = await response.json();
            console.log("✅ Project data:", project);

            form.querySelector('[name="Id"]').value = project.id;
            form.querySelector('[name="ProjectName"]').value = project.projectName;
            form.querySelector('[name="ClientName"]').value = project.clientName;
            form.querySelector('[name="Description"]').value = project.description ?? "";
            form.querySelector('[name="StartDate"]').value = project.startDate.substring(0, 10);
            form.querySelector('[name="EndDate"]').value = project.endDate.substring(0, 10);
            form.querySelector('[name="Budget"]').value = project.budget;
            previewImg.src = project.projectImagePath ?? "/img/upload.svg";

            const select = form.querySelector('[name="SelectedMemberId"]');
            if (select) {
                Array.from(select.options).forEach(option => {
                    option.selected = project.memberIds.includes(option.value);
                });
            }
        } catch (error) {
            console.error("❌ Kunde inte hämta projektdata:", error);
        }
    });
    // 🖼 Förhandsgranskning av bild
    uploadBtn?.addEventListener("click", () => fileInput?.click());
    fileInput?.addEventListener("change", function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => previewImg.src = e.target.result;
            reader.readAsDataURL(file);
        }
    });

    // 🧼 Inline-validering
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
        const errorContainer = document.getElementById("edit-project-errors");

        if (errorContainer) errorContainer.innerHTML = "";

        form.querySelectorAll(".input-validation-error").forEach(i => i.classList.remove("input-validation-error"));
        form.querySelectorAll("span[data-valmsg-for]").forEach(s => s.textContent = "");

        try {
            const response = await fetch("/Project/Update", {
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
                    const span = form.querySelector(`span[data-valmsg-for="${field.field}"]`);
                    if (input) input.classList.add("input-validation-error");
                    if (span) span.textContent = field.errors.join(" ");
                });
            }
        } catch (err) {
            console.error("Error updating project:", err);
            if (errorContainer) errorContainer.innerHTML = "<div>An unexpected error occurred.</div>";
        }
    });
});
