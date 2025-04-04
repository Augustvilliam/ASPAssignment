document.addEventListener("DOMContentLoaded", () => {
    initCreateProjectModal();
    initEditProjectModal();
    initEditTeamMemberModal();
    initMoreMenu();
});

function initCreateProjectModal() {
    const modal = document.getElementById("createprojectModal");
    const form = modal?.querySelector("form");
    const fileInput = document.getElementById("projectImageInput");
    const previewImg = document.getElementById("projectPreviewImage");
    const uploadBtn = document.getElementById("projectUploadBtn");
    const errorContainer = document.getElementById("create-project-errors");

    if (!modal || !form) return;

    initImagePreview(fileInput, previewImg, uploadBtn);

    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        clearValidation(form);

        const data = new FormData(form);
        if (errorContainer) errorContainer.innerHTML = "";

        try {
            const response = await fetch("/Project/Create", {
                method: "POST",
                body: data
            });

            await handleFormResponse(response, modal, "/Navigation/LoadProjects", errorContainer);
        } catch (err) {
            console.error("Create project error:", err);
        }
    });
}

function initEditProjectModal() {
    const modal = document.getElementById("editprojectModal");
    const form = modal?.querySelector("form");
    const fileInput = document.getElementById("editProjectImageInput");
    const previewImg = document.getElementById("editProjectPreviewImage");
    const uploadBtn = document.getElementById("editProjectUploadBtn");
    const errorContainer = document.getElementById("edit-project-errors");

    if (!modal || !form) return;

    initImagePreview(fileInput, previewImg, uploadBtn);

    modal.addEventListener("show.bs.modal", async (event) => {
        const button = event.relatedTarget;
        const projectId = button?.getAttribute("data-project-id");
        if (!projectId) return;

        const response = await fetch(`/Project/GetProject/${projectId}`);
        const project = await response.json();

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
    });

    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        clearValidation(form);

        const data = new FormData(form);
        if (errorContainer) errorContainer.innerHTML = "";

        try {
            const response = await fetch("/Project/Update", {
                method: "POST",
                body: data
            });

            await handleFormResponse(response, modal, "/Navigation/LoadProjects", errorContainer);
        } catch (err) {
            console.error("Update project error:", err);
        }
    });
}

function initEditTeamMemberModal() {
    const modal = document.getElementById("editTeamMemberModal");
    const form = document.getElementById("edit-member-form");
    const fileInput = document.getElementById("profilepic");
    const previewImg = document.getElementById("previewImage");
    const uploadBtn = document.getElementById("uploadPreviewBtn");
    const errorContainer = document.getElementById("edit-member-errors");

    if (!modal || !form) return;

    initImagePreview(fileInput, previewImg, uploadBtn);

    modal.addEventListener("show.bs.modal", async (event) => {
        const button = event.relatedTarget;
        const memberId = button?.getAttribute("data-member-id");
        if (!memberId) return;

        const response = await fetch(`/Member/GetMember/${memberId}`);
        const member = await response.json();

        form.querySelector('[name="Id"]').value = member.id;
        form.querySelector('[name="FirstName"]').value = member.firstName;
        form.querySelector('[name="LastName"]').value = member.lastName;
        form.querySelector('[name="Email"]').value = member.email;
        form.querySelector('[name="Phone"]').value = member.phone || "";
        form.querySelector('[name="JobTitle"]').value = member.jobTitle || "";
        previewImg.src = member.profileImagePath ?? "/img/upload.svg";
    });

    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        clearValidation(form);

        const data = new FormData(form);
        if (errorContainer) errorContainer.innerHTML = "";

        try {
            const response = await fetch("/Member/Update", {
                method: "POST",
                body: data
            });

            await handleFormResponse(response, modal, "/Navigation/LoadMembers", errorContainer);
        } catch (err) {
            console.error("Update member error:", err);
        }
    });
}

function initImagePreview(fileInput, previewImg, uploadBtn) {
    if (!fileInput || !previewImg) return;
    uploadBtn?.addEventListener("click", () => fileInput.click());
    fileInput.addEventListener("change", function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => previewImg.src = e.target.result;
            reader.readAsDataURL(file);
        }
    });
}

async function handleFormResponse(response, modal, reloadUrl, errorContainer) {
    if (response.ok) {
        const result = await response.json();
        if (result.success) {
            bootstrap.Modal.getInstance(modal)?.hide();
            const dynamicContent = document.getElementById("dynamic-content");
            const viewResponse = await fetch(reloadUrl);
            const html = await viewResponse.text();
            dynamicContent.innerHTML = html;

            // 👇 Kör igen efter innehållet laddats in
            initMoreMenu();
        }
    } else {
        const errors = await response.json();
        errors.forEach(field => {
            const input = document.querySelector(`[name="${field.field}"]`);
            const span = document.querySelector(`span[data-valmsg-for="${field.field}"]`);
            if (input) input.classList.add("input-validation-error");
            if (span) span.textContent = field.errors.join(" ");
        });

        if (errorContainer) {
            errorContainer.innerHTML += "<div>Validation failed.</div>";
        }
    }
}
function initMoreMenu() {
    document.querySelectorAll(".more-btn").forEach(btn => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();
            const container = btn.closest(".more-container");
            const menu = container.querySelector(".more-menu");

            // Stäng andra öppna menyer
            document.querySelectorAll(".more-menu").forEach(m => {
                if (m !== menu) m.classList.add("d-none");
            });

            // Toggle meny för denna
            menu.classList.toggle("d-none");
        });
    });

    // Klick utanför = stäng alla menyer
    document.addEventListener("click", (e) => {
        if (!e.target.closest(".more-container")) {
            document.querySelectorAll(".more-menu").forEach(menu => menu.classList.add("d-none"));
        }
    });

    // 🛠 "Edit Project"-knapp i dropdown
    document.querySelectorAll(".edit-btn").forEach(btn => {
        btn.addEventListener("click", async (e) => {
            // Stäng meny direkt
            document.querySelectorAll(".more-menu").forEach(menu => menu.classList.add("d-none"));

            const container = btn.closest(".more-container");
            const projectId = container?.querySelector(".more-btn")?.getAttribute("data-project-id");
            if (!projectId) return;

            const modal = document.getElementById("editprojectModal");
            const form = modal?.querySelector("form");

            try {
                const response = await fetch(`/Project/GetProject/${projectId}`);
                const project = await response.json();

                form.querySelector('[name="Id"]').value = project.id;
                form.querySelector('[name="ProjectName"]').value = project.projectName;
                form.querySelector('[name="ClientName"]').value = project.clientName;
                form.querySelector('[name="Description"]').value = project.description ?? "";
                form.querySelector('[name="StartDate"]').value = project.startDate.substring(0, 10);
                form.querySelector('[name="EndDate"]').value = project.endDate.substring(0, 10);
                form.querySelector('[name="Budget"]').value = project.budget;
                document.getElementById("editProjectPreviewImage").src = project.projectImagePath ?? "/img/upload.svg";

                const select = form.querySelector('[name="SelectedMemberId"]');
                if (select) {
                    Array.from(select.options).forEach(option => {
                        option.selected = project.memberIds.includes(option.value);
                    });
                }

                bootstrap.Modal.getOrCreateInstance(modal).show();

            } catch (error) {
                console.error("❌ Kunde inte hämta projektdata:", error);
            }
        });
    });
}

function clearValidation(form) {
    form.querySelectorAll(".input-validation-error").forEach(i => i.classList.remove("input-validation-error"));
    form.querySelectorAll("span[data-valmsg-for]").forEach(s => s.textContent = "");
}

