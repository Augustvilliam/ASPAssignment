let isMemberPickerInitialized = false;
let isEditMemberPickerInitialized = false;
let isCreateProjectModalInitialized = false;

let editSelectedMembers = [];

// Kör alla initieringar och nollställ flags
function initAll() {
    initCreateProjectModal();
    initEditProjectModal();
    initEditTeamMemberModal();
    initMoreMenu();
    initDeleteModal();
    resetInitFlags();
}

document.addEventListener("DOMContentLoaded", initAll);

// Generisk member-picker
function initMemberPickerGeneric(modal, opts) {
    if (modal.dataset.memberPickerInited) return;
    modal.dataset.memberPickerInited = 'true';

    const btnSel = modal.querySelector(opts.toggleBtnSelector);
    const ulSel = modal.querySelector(opts.suggestionListSelector);
    const contSel = modal.querySelector(opts.selectedContainerSel);
    const idsSel = modal.querySelector(opts.selectedIdsInputSel);
    if (!btnSel || !ulSel || !contSel || !idsSel) {
        console.warn("⚠️ Member picker-element saknas i DOM");
        return;
    }

    let selectedMembers = Array.isArray(opts.existingMembers)
        ? opts.existingMembers.slice()
        : [];
    let allMembers = [];

    modal.addEventListener('show.bs.modal', () => {
        selectedMembers = [];
        contSel.innerHTML = '';
        idsSel.innerHTML = '';
        ulSel.innerHTML = '';
        ulSel.style.display = 'none';

        if (Array.isArray(opts.existingMembers) && opts.existingMembers.length) {
            selectedMembers = opts.existingMembers.map(m => ({ id: m.id, fullName: m.fullName, avatarUrl: m.avatarUrl }));
            renderSelected();
            updateHidden();
        }
    });

    btnSel.addEventListener('click', async (e) => {
        e.preventDefault();
        const isVisible = ulSel.style.display === 'block';
        ulSel.style.display = isVisible ? 'none' : 'block';
        if (!isVisible && allMembers.length === 0) {
            try { allMembers = await fetch('/Member/Search').then(r => r.json()); }
            catch (err) { console.error("🚨 Fel vid hämtning av medlemmar:", err); }
        }
        renderSuggestions(allMembers);
    });

    function renderSuggestions(list) {
        ulSel.innerHTML = '';
        list.forEach(m => {
            if (selectedMembers.some(x => x.id === m.id)) return;
            const li = document.createElement('li');
            li.innerHTML = `<img src="${m.avatarUrl}" alt=""> ${m.fullName}`;
            li.addEventListener('click', () => { selectedMembers.push(m); renderSelected(); updateHidden(); ulSel.style.display = 'none'; });
            ulSel.appendChild(li);
        });
    }

    function renderSelected() {
        contSel.innerHTML = '';
        selectedMembers.forEach(m => {
            const div = document.createElement('div');
            div.classList.add('selected-member');
            div.innerHTML = `
                <img src="${m.avatarUrl}" alt="">
                ${m.fullName}
                <button type="button" class="remove-btn" data-id="${m.id}">&times;</button>
            `;
            contSel.appendChild(div);
        });
        contSel.querySelectorAll('.remove-btn').forEach(btn => btn.addEventListener('click', () => {
            selectedMembers = selectedMembers.filter(x => x.id !== btn.dataset.id);
            renderSelected();
            updateHidden();
            renderSuggestions(allMembers);
        }));
    }

    function updateHidden() {
        idsSel.innerHTML = '';
        selectedMembers.forEach(m => {
            const inp = document.createElement('input');
            inp.type = 'hidden';
            inp.name = 'SelectedMemberId';
            inp.value = m.id;
            idsSel.appendChild(inp);
        });
    }
}
// CREATE project modal
function initCreateProjectModal() {
    const modal = document.getElementById("createprojectModal");
    if (!modal || isCreateProjectModalInitialized) return;
    isCreateProjectModalInitialized = true;

    const form = modal.querySelector("form");
    const fileInput = document.getElementById("projectImageInput");
    const previewImg = document.getElementById("projectPreviewImage");
    const uploadBtn = document.getElementById("projectUploadBtn");
    const errorContainer = document.getElementById("create-project-errors");
    initImagePreview(fileInput, previewImg, uploadBtn);

    // Initiera pickern med tom existingMembers
    initMemberPickerGeneric(modal, {
        toggleBtnSelector: '#toggle-member-list',
        suggestionListSelector: '#member-suggestions',
        selectedContainerSel: '#selected-members',
        selectedIdsInputSel: '#selected-member-ids',
        existingMembers: []
    });

    form.addEventListener("submit", async (e) => {
        e.preventDefault(); clearValidation(form);
        const data = new FormData(form);
        if (errorContainer) errorContainer.innerHTML = '';
        try {
            const resp = await fetch("/Project/Create", { method: "POST", body: data });
            await handleFormResponse(resp, modal, "/Navigation/LoadProjects", errorContainer);
        } catch (err) { console.error("Create project error:", err); }
    });
    form.dataset.submitBound = 'true';
}
// EDIT project modal
function initEditProjectModal() {
    const modal = document.getElementById("editprojectModal");
    if (!modal) return;

    const form = modal.querySelector("form");
    const fileInput = document.getElementById("editProjectImageInput");
    const previewImg = document.getElementById("editProjectPreviewImage");
    const uploadBtn = document.getElementById("editProjectUploadBtn");
    const errorContainer = document.getElementById("edit-project-errors");
    initImagePreview(fileInput, previewImg, uploadBtn);

    // Initiera pickern EN gång med tom befintlig lista
    initMemberPickerGeneric(modal, {
        toggleBtnSelector: '#edit-toggle-member-list',
        suggestionListSelector: '#edit-member-suggestions',
        selectedContainerSel: '#edit-selected-members',
        selectedIdsInputSel: '#edit-selected-member-ids',
        existingMembers: []
    });

    // Vid varje öppning: hämta project, fyll form och återfylla pickern
    modal.addEventListener("show.bs.modal", async (event) => {
        const button = event.relatedTarget;
        const projectId = button?.getAttribute("data-project-id");
        if (!projectId) return;

        try {
            const resp = await fetch(`/Project/GetProject/${projectId}`);
            const project = await resp.json();

            // Fyll i formulärfälten
            form.querySelector('[name="Id"]').value = project.id;
            form.querySelector('[name="ProjectName"]').value = project.projectName;
            form.querySelector('[name="ClientName"]').value = project.clientName;
            form.querySelector('[name="Description"]').value = project.description || '';
            form.querySelector('[name="StartDate"]').value = project.startDate.substring(0, 10);
            form.querySelector('[name="EndDate"]').value = project.endDate.substring(0, 10);
            form.querySelector('[name="Budget"]').value = project.budget;
            form.querySelector('[name="Status"]').value = project.status;
            previewImg.src = project.projectImagePath || "/img/upload.svg";

            // Återställ pickerns state så den kan fyllas på med nya medlemmar
            delete modal.dataset.memberPickerInited;
            initMemberPickerGeneric(modal, {
                toggleBtnSelector: '#edit-toggle-member-list',
                suggestionListSelector: '#edit-member-suggestions',
                selectedContainerSel: '#edit-selected-members',
                selectedIdsInputSel: '#edit-selected-member-ids',
                existingMembers: project.members
            });
        } catch (err) {
            console.error("❌ Kunde inte hämta projektdata:", err);
        }
    });

    form.addEventListener("submit", async (e) => {
        e.preventDefault(); clearValidation(form);

        const data = new FormData(form);
        console.log("⚙️ Hidden SelectedMemberId inputs:",
            Array.from(data.getAll("SelectedMemberId"))
        );
        if (errorContainer) errorContainer.innerHTML = '';
        try {
            const resp = await fetch("/Project/Update", { method: "POST", body: data });
            await handleFormResponse(resp, modal, "/Navigation/LoadProjects", errorContainer);
        } catch (err) { console.error("Update project error:", err); }
    });
    form.dataset.editSubmitBound = 'true';
}
function initEditTeamMemberModal() {
    const modal = document.getElementById("editTeamMemberModal");
    if (!modal || modal.dataset.teamModalBound) return;
    modal.dataset.teamModalBound = 'true';

    console.log("🔁 initEditTeamMemberModal körs med roles");

    const form = modal.querySelector("form");
    const fileInput = document.getElementById("profilepic");
    const previewImg = document.getElementById("previewImage");
    const uploadBtn = document.getElementById("uploadPreviewBtn");
    const errorContainer = document.getElementById("edit-member-errors");
    const roleSelect = form.querySelector('[name="RoleId"]');

    initImagePreview(fileInput, previewImg, uploadBtn);

    modal.addEventListener("show.bs.modal", async (event) => {
        const button = event.relatedTarget;
        const memberId = button?.getAttribute("data-member-id");
        if (!memberId) return;

        try {
            const response = await fetch(`/Member/GetMember/${memberId}`);
            const data = await response.json();
            const member = data.member;
            const roles = data.roles || [];

            // Fyll i formulärfälten
            form.querySelector('[name="Id"]').value = member.id;
            form.querySelector('[name="FirstName"]').value = member.firstName;
            form.querySelector('[name="LastName"]').value = member.lastName;
            form.querySelector('[name="Email"]').value = member.email;
            form.querySelector('[name="Phone"]').value = member.phone || "";
            previewImg.src = member.profileImagePath ?? "/img/upload.svg";

            // Ladda dropdown med roller
            if (roleSelect) {
                roleSelect.innerHTML = '';
                roles.forEach(r => {
                    const opt = document.createElement('option');
                    opt.value = r.id;
                    opt.text = r.name;
                    roleSelect.appendChild(opt);
                });
                // Sätta valt värde
                roleSelect.value = member.roleId || '';
            }
        } catch (err) {
            console.error("❌ Kunde inte hämta memberdata eller roller:", err);
        }
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
    const buttons = document.querySelectorAll(".more-btn");
    if (!buttons.length) return; // ✔️ finns inget att initiera

    buttons.forEach(btn => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();
            const container = btn.closest(".more-container");
            const menu = container?.querySelector(".more-menu");

            if (!menu) return;

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
function resetInitFlags() {
    isCreateProjectModalInitialized = false;
    isMemberPickerInitialized = false;
    isEditMemberPickerInitialized = false;
    editSelectedMembers = [];

    // Ta bort alla dataset‑flaggor på dina modals och formulär
    const clearDataSet = (el, keys) => {
        if (!el) return;
        keys.forEach(k => delete el.dataset[k]);
    };

    // CREATE
    const createModal = document.getElementById("createprojectModal");
    clearDataSet(createModal, ["memberPickerInited"]);
    const createForm = createModal?.querySelector("form");
    clearDataSet(createForm, ["submitBound"]);

    // EDIT PROJECT
    const editProjModal = document.getElementById("editprojectModal");
    clearDataSet(editProjModal, ["memberPickerInited", "editModalBound"]);
    const editProjForm = editProjModal?.querySelector("form");
    clearDataSet(editProjForm, ["editSubmitBound"]);

    // EDIT TEAM MEMBER
    const teamModal = document.getElementById("editTeamMemberModal");
    clearDataSet(teamModal, ["teamModalBound"]);
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
            if (dynamicContent) {
                const viewResponse = await fetch(reloadUrl);
                const html = await viewResponse.text();
                dynamicContent.innerHTML = html;
                resetInitFlags();
                initAll();
            }
            else {
                console.error("❌ Dynamic content-element saknas i DOM. ingen reload är gjord.");

            }
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




