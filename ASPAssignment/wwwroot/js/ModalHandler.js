let isMemberPickerInitialized = false;
let isEditMemberPickerInitialized = false;
let isCreateProjectModalInitialized = false;

let editSelectedMembers = [];

document.addEventListener("DOMContentLoaded", () => {
    initAll();
    resetInitFlags();
});


function initAll(){
    initCreateProjectModal();
    initEditProjectModal();
    initEditTeamMemberModal();
    initMoreMenu();
    initDeleteModal();
}
function resetInitFlags() {
    isCreateProjectModalInitialized = false;
    isMemberPickerInitialized = false;
    isEditMemberPickerInitialized = false;
    editSelectedMembers = [];
}
function initCreateProjectModal() {
    const modal = document.getElementById("createprojectModal");
    if (!modal || isCreateProjectModalInitialized) return;
    isCreateProjectModalInitialized = true;

    console.log("🔁 initCreateProjectModal körs");

    const form = modal.querySelector("form");
    const fileInput = document.getElementById("projectImageInput");
    const previewImg = document.getElementById("projectPreviewImage");
    const uploadBtn = document.getElementById("projectUploadBtn");
    const errorContainer = document.getElementById("create-project-errors");

    initImagePreview(fileInput, previewImg, uploadBtn);

    modal.addEventListener("show.bs.modal", () => {
        if (!isMemberPickerInitialized) {
            initMemberPicker(modal);
            isMemberPickerInitialized = true;
        }
    });

    // 🧠 Skydda mot dubbelbindning
    if (!form?.dataset.submitBound) {
        form?.addEventListener("submit", async (e) => {
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

        form.dataset.submitBound = "true"; // ✔️ Markera som bunden
    }
}
function initEditProjectModal() {
    console.log("✅ Kör initEditMemberPicker!");
    const modal = document.getElementById("editprojectModal");
    if (!modal) return;

    console.log("🔁 initEditProjectModal körs");

    const form = modal.querySelector("form");
    const fileInput = document.getElementById("editProjectImageInput");
    const previewImg = document.getElementById("editProjectPreviewImage");
    const uploadBtn = document.getElementById("editProjectUploadBtn");
    const errorContainer = document.getElementById("edit-project-errors");

    initImagePreview(fileInput, previewImg, uploadBtn);

    modal.addEventListener("show.bs.modal", async (event) => {
        initEditMemberPicker(modal); // Kör alltid!
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
        form.querySelector('[name="Status"]').value = project.status;
        previewImg.src = project.projectImagePath ?? "/img/upload.svg";

        if (!isEditMemberPickerInitialized) {
            initEditMemberPicker(modal);
            isEditMemberPickerInitialized = true;
        }

        // Nu finns renderEditSelectedMembers
        if (typeof window.renderEditSelectedMembers === "function") {
            window.renderEditSelectedMembers(project.members);
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
    if (!modal) return;

    console.log("🔁 initEditTeamMemberModal körs");

    const form = modal.querySelector("form");
    const fileInput = document.getElementById("profilepic");
    const previewImg = document.getElementById("previewImage");
    const uploadBtn = document.getElementById("uploadPreviewBtn");
    const errorContainer = document.getElementById("edit-member-errors");

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
function initMoreMenu() {
    document.querySelectorAll(".more-btn").forEach(btn => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();
            const container = btn.closest(".more-container");
            const menu = container.querySelector(".more-menu");

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

    // 🛠 Edit-knapp
    document.querySelectorAll(".edit-btn").forEach(btn => {
        btn.addEventListener("click", async (e) => {
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

    // 🗑 Delete-knapp
    document.querySelectorAll(".delete-btn").forEach(btn => {
        btn.addEventListener("click", (e) => {
            document.querySelectorAll(".more-menu").forEach(menu => menu.classList.add("d-none"));

            const container = btn.closest(".more-container");
            const projectId = container?.querySelector(".more-btn")?.getAttribute("data-project-id");
            if (!projectId) return;

            const deleteIdInput = document.getElementById("deleteProjectId");
            const confirmationInput = document.getElementById("deleteConfirmationInput");
            const errorDiv = document.getElementById("deleteError");

            if (!deleteIdInput || !confirmationInput || !errorDiv) {
                console.warn("⚠️ Delete-modalens element saknas – hoppar över delete-init.");
                return;
            }

            deleteIdInput.value = projectId;
            confirmationInput.value = "";
            errorDiv.classList.add("d-none");

            const modal = new bootstrap.Modal(document.getElementById("deleteProjectModal"));
            modal.show();
        });
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
function initMemberPicker(modal) {
    console.log("🧩 Initierar member picker");

    const toggleBtn = modal.querySelector('#toggle-member-list');
    const suggestionList = modal.querySelector('#member-suggestions');
    const selectedContainer = modal.querySelector('#selected-members');
    const selectedIdsInput = modal.querySelector('#selected-member-ids');

    if (!toggleBtn || !suggestionList || !selectedContainer || !selectedIdsInput) {
        console.warn("⚠️ Member picker-element saknas i DOM");
        return;
    }

    let selectedMembers = [];
    let allMembers = [];

    toggleBtn.addEventListener('click', async (e) => {
        e.preventDefault();

        const isVisible = suggestionList.style.display === 'block';
        suggestionList.style.display = isVisible ? 'none' : 'block';

        if (!isVisible && allMembers.length === 0) {
            try {
                console.log("🔍 Hämtar användare från /Member/Search");
                const response = await fetch('/Member/Search');
                allMembers = await response.json();

                if (!Array.isArray(allMembers) || allMembers.length === 0) {
                    console.warn("❌ Inga medlemmar hittades från /Member/Search");
                }

                renderSuggestionList(allMembers);
            } catch (error) {
                console.error("🚨 Fel vid hämtning av medlemmar:", error);
            }
        } else {
            renderSuggestionList(allMembers);
        }
    });

    function renderSuggestionList(data) {
        suggestionList.innerHTML = '';
        data.forEach(member => {
            if (selectedMembers.some(m => m.id === member.id)) return;

            const li = document.createElement('li');
            li.innerHTML = `<img src="${member.avatarUrl}" alt=""> ${member.fullName}`;
            li.addEventListener('click', () => selectMember(member));
            suggestionList.appendChild(li);
        });
    }

    function selectMember(member) {
        selectedMembers.push(member);
        renderSelectedMembers();
        updateSelectedIds();
        suggestionList.style.display = 'none';
    }

    function renderSelectedMembers() {
        selectedContainer.innerHTML = '';
        selectedMembers.forEach(member => {
            const div = document.createElement('div');
            div.classList.add('selected-member');
            div.innerHTML = `
                <img src="${member.avatarUrl}" alt="">
                ${member.fullName}
                <button type="button" class="remove-btn" data-id="${member.id}" style="margin-left: 6px; background:none; border:none;">&times;</button>
            `;
            selectedContainer.appendChild(div);
        });

        selectedContainer.querySelectorAll('.remove-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                removeMember(this.dataset.id);
            });
        });
    }

    function removeMember(id) {
        selectedMembers = selectedMembers.filter(m => m.id !== id);
        renderSelectedMembers();
        updateSelectedIds();
        renderSuggestionList(allMembers);
    }

    function updateSelectedIds() {
        selectedIdsInput.innerHTML = '';
        selectedMembers.forEach(member => {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'SelectedMemberId';
            input.value = member.id;
            selectedIdsInput.appendChild(input);
        });
    }
}
function initEditMemberPicker(modal) {
    console.log("🧩 Initierar EDIT member picker");

    const toggleBtn = modal.querySelector('#edit-toggle-member-list');
    const suggestionList = modal.querySelector('#edit-member-suggestions');
    const selectedContainer = modal.querySelector('#edit-selected-members');
    const selectedIdsInput = modal.querySelector('#edit-selected-member-ids');

    if (!toggleBtn || !suggestionList || !selectedContainer || !selectedIdsInput) {
        console.warn("⚠️ Edit member picker-element saknas i DOM");
        return;
    }

    let allMembers = [];

    toggleBtn.addEventListener('click', async (e) => {
        e.preventDefault();

        const isVisible = suggestionList.style.display === 'block';
        suggestionList.style.display = isVisible ? 'none' : 'block';

        if (!isVisible && allMembers.length === 0) {
            try {
                const response = await fetch('/Member/Search');
                allMembers = await response.json();
                renderSuggestionList(allMembers);
            } catch (err) {
                console.error("❌ Kunde inte hämta medlemmar:", err);
            }
        } else {
            renderSuggestionList(allMembers);
        }
    });

    function renderSuggestionList(data) {
        suggestionList.innerHTML = '';
        data.forEach(member => {
            if (editSelectedMembers.some(m => m.id === member.id)) return;

            const li = document.createElement('li');
            li.innerHTML = `<img src="${member.avatarUrl}" alt=""> ${member.fullName}`;
            li.addEventListener('click', () => selectMember(member));
            suggestionList.appendChild(li);
        });
    }

    function selectMember(member) {
        editSelectedMembers.push(member);
        renderEditSelectedMembers();
        updateSelectedIds();
        suggestionList.style.display = 'none';
    }

    function renderEditSelectedMembers() {
        selectedContainer.innerHTML = '';
        editSelectedMembers.forEach(member => {
            const div = document.createElement('div');
            div.classList.add('selected-member');
            div.innerHTML = `
                <img src="${member.avatarUrl}" alt="">
                ${member.fullName}
                <button type="button" class="remove-btn" data-id="${member.id}" style="margin-left: 6px; background:none; border:none;">&times;</button>
            `;
            selectedContainer.appendChild(div);
        });

        selectedContainer.querySelectorAll('.remove-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                removeMember(this.dataset.id);
            });
        });

        updateSelectedIds();
    }

    function removeMember(id) {
        editSelectedMembers = editSelectedMembers.filter(m => m.id !== id);
        renderEditSelectedMembers();
        updateSelectedIds();
        renderSuggestionList(allMembers);
    }

    function updateSelectedIds() {
        selectedIdsInput.innerHTML = '';
        editSelectedMembers.forEach(member => {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'SelectedMemberId';
            input.value = member.id;
            selectedIdsInput.appendChild(input);
        });
    }

    // Gör tillgänglig globalt
    window.renderEditSelectedMembers = renderEditSelectedMembers;
}
function initDeleteModal() {
    const form = document.getElementById("confirmDeleteForm");
    if (!form || form.dataset.submitBound) return;

    form.addEventListener("submit", function (e) {
        e.preventDefault();

        const confirmationInput = document.getElementById("deleteConfirmationInput");
        const projectId = document.getElementById("deleteProjectId");
        const errorDiv = document.getElementById("deleteError");

        if (confirmationInput.value !== "DELETE") {
            errorDiv.classList.remove("d-none");
            return;
        }

        fetch(`/Project/Delete/${projectId.value}`, {
            method: "DELETE"
        }).then(response => {
            handleDeleteResponse(
                response,
                document.getElementById("deleteProjectModal"),
                "/Navigation/LoadProjects"
            );
        }).catch(err => {
            console.error("🚨 Fetch error vid delete:", err);
        });
    });

    form.dataset.submitBound = "true"; // 🔒 Förhindrar multipla bindningar
}
function clearValidation(form) {
    form.querySelectorAll(".input-validation-error").forEach(i => i.classList.remove("input-validation-error"));
    form.querySelectorAll("span[data-valmsg-for]").forEach(s => s.textContent = "");
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

            resetInitFlags();
            initAll();  
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
async function handleDeleteResponse(response, modal, reloadUrl) {
    if (response.ok) {
        bootstrap.Modal.getInstance(modal)?.hide();
        const dynamicContent = document.getElementById("dynamic-content");
        const viewResponse = await fetch(reloadUrl);
        const html = await viewResponse.text();
        dynamicContent.innerHTML = html;

        resetInitFlags();
        initAll();

    } else {
        console.error("❌ Delete misslyckades, status:", response.status);
    }
}




