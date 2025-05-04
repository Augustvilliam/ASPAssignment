function initAll() {
    resetInitFlags();
    initCreateProjectModal();
    initEditProjectModal();
    initEditTeamMemberModal();
    initMoreMenu();
    initDeleteModal();
    initAddMemberModal();
}
document.addEventListener("DOMContentLoaded", initAll);
const MAX_BUDGET = 2147483647; //maximalt värde för budget

// CREATE project modal
function initCreateProjectModal() {
    const modal = document.getElementById("createprojectModal");
    if (!modal) return;

    const form = modal.querySelector("form");
    const previewImg = document.getElementById("projectPreviewImage");
    const errorContainer = document.getElementById("create-project-errors");

    // ─── Rensa formuläret vid varje öppning ───
    modal.addEventListener("show.bs.modal", () => {
        form.reset();                             // töm alla inputs
        if (errorContainer) errorContainer.innerHTML = "";
        previewImg.src = "/img/upload.svg";       // återställ preview
        delete modal.dataset.memberPickerInited;  // låt pickern initieras om
        //Rensa quill.
        if (window.quillCreate) {
            window.quillCreate.root.innerHTML = '';
        }
    });

    // ─── Bind bara ett submit ───
    if (form.dataset.submitBound) return;
    form.dataset.submitBound = "true";

    const fileInput = document.getElementById("projectImageInput");
    const uploadBtn = document.getElementById("projectUploadBtn");
    initImagePreview(fileInput, previewImg, uploadBtn);


    form.addEventListener("submit", async (e) => {
        e.preventDefault(); clearValidation(form);
        const rules = {
            ProjectName: {
                fn: v => v.trim() !== "",
                msg: "Project Name is required."
            },
            ClientName: {
                fn: v => v.trim() !== "",
                msg: "Client Name is required."
            },
            StartDate: {
                fn: v => v !== "",
                msg: "Start Date is required."
            },
            EndDate: {
                fn: v => v !== "",
                msg: "End Date is required."
            },
            Budget: {
                fn: v => {
                    const n = Number(v);
                    return /^\d+$/.test(v) && n >= 0 && n <= MAX_BUDGET;
                },
                msg: `Budget must be a number between 0 and ${MAX_BUDGET}.`
            }
        };
        if (!validateForm(form, rules)) {
            return;
        }
        if (window.quillCreate) {
            document.getElementById('create-description-input').value = quillCreate.root.innerHTML;
        }
        const data = new FormData(form);
        errorContainer.innerHTML = "";
        try {
            const token = data.get("__RequestVerificationToken");
            const resp = await fetch(form.action || "/Project/Create", {
                method: "POST",
                body: data,
                headers: token ? { "RequestVerificationToken": token } : {},
                credentials: "same-origin"
            });
            await handleFormResponse(resp, modal, "/Navigation/LoadProjects", errorContainer);
        } catch (err) {
            console.error("Create project error:", err);
        }
    });
}
// EDIT project modal
function initEditProjectModal() {
    const modal = document.getElementById("editprojectModal");
    if (!modal) return;

    const form = modal.querySelector("form");
    if (form.dataset.editSubmitBound) return;
    form.dataset.editSubmitBound = 'true';

    const fileInput = document.getElementById("editProjectImageInput");
    const previewImg = document.getElementById("editProjectPreviewImage");
    const uploadBtn = document.getElementById("editProjectUploadBtn");
    const errorContainer = document.getElementById("edit-project-errors");
    initImagePreview(fileInput, previewImg, uploadBtn);

    modal.addEventListener("show.bs.modal", async (event) => {
        modal.dataset.keepMembers = "true";

        const button = event.relatedTarget;
        const projectId = button?.getAttribute("data-project-id");
        if (!projectId) return;

        // ── 1) Töm pickern ─────────────────────────
        const selBox = document.getElementById('edit-selected-members');
        const hiddenEl = document.getElementById('edit-selected-member-ids');
        selBox.innerHTML = '';
        hiddenEl.innerHTML = '';

        try {
            // ── 2) Hämta projektdata ─────────────────
            const resp = await fetch(`/Project/GetProject/${projectId}`);
            const project = await resp.json();


            // ── 3) Fyll i formuläret ───────────────────
            form.querySelector('[name="Id"]').value = project.id;
            form.querySelector('[name="ProjectName"]').value = project.projectName;
            form.querySelector('[name="ClientName"]').value = project.clientName;
            form.querySelector('[name="StartDate"]').value = project.startDate.slice(0, 10);
            form.querySelector('[name="EndDate"]').value = project.endDate.slice(0, 10);
            form.querySelector('[name="Budget"]').value = project.budget;
            form.querySelector('[name="Status"]').value = project.status;
            // beskrivning via Quill
            form.querySelector('[name="Description"]').value = project.description || '';
            window.quillEdit.root.innerHTML = project.description || '';
            previewImg.src = project.projectImagePath || "/img/upload.svg";

            // ── 4) Populera pickern med redan valda medlemmar ──
            // (använder ditt memberPickerAPI)
            const members = project.members || [];
            members.forEach(m => {
                window.memberPickerAPI.addMemberToPicker({
                    id: m.id,
                    fullName: m.fullName,
                    avatarUrl: m.avatarUrl
                }, selBox, hiddenEl);
            });
        }
        catch (err) {
            console.error("❌ Kunde inte hämta projektdata:", err);
        }
    });
    form.addEventListener("submit", async e => {
        e.preventDefault();
        clearValidation(form);

        // Valideringsregler för Edit Project
        const rules = {
            ProjectName: {
                fn: v => v.trim() !== "",
                msg: "Project Name is required."
            },
            ClientName: {
                fn: v => v.trim() !== "",
                msg: "Client Name is required."
            },
            StartDate: {
                fn: v => v !== "",
                msg: "Start Date is required."
            },
            EndDate: {
                fn: v => v !== "",
                msg: "End Date is required."
            },
            Budget: {
                fn: v => {
                    // bara heltal, 0 <= n <= MAX_BUDGET
                    return /^\d+$/.test(v) && Number(v) >= 0 && Number(v) <= MAX_BUDGET;
                },
                msg: `Budget must be an integer between 0 and ${MAX_BUDGET}.`
            }
        };

        // Kör valideringen – avbryt om något är fel
        if (!validateForm(form, rules)) {
            return;
        }

        // Flytta in Quill-text om du använder det
        if (window.quillEdit) {
            document.getElementById('edit-description-input')
                .value = quillEdit.root.innerHTML;
        }

        // Skapa FormData och skicka
        const data = new FormData(form);
        const token = data.get("__RequestVerificationToken");
        const errorContainer = document.getElementById("edit-project-errors");
        errorContainer.innerHTML = "";  // rensa gamla fel

        try {
            const resp = await fetch(form.action || "/Project/Update", {
                method: "POST",
                body: data,
                headers: token ? { "RequestVerificationToken": token } : {},
                credentials: "same-origin"
            });
            await handleFormResponse(resp, modal, "/Navigation/LoadProjects", errorContainer);
        } catch (err) {
            console.error("Update project error:", err);
        }
    });
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
            form.querySelector('[name="StreetAddress"]').value = member.streetAddress || "";
            form.querySelector('[name="City"]').value = member.city || "";
            form.querySelector('[name="PostalCode"]').value = member.postalCode || "";
            form.querySelector('[name="BirthDate"]').value = member.birthDate
                ? member.birthDate.slice(0, 10)
                : "";

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
                roleSelect.value = member.roleId || '';
            }
        } catch (err) {
            console.error("❌ Kunde inte hämta memberdata eller roller:", err);
        }
    });

    form.addEventListener("submit", async e => {
        e.preventDefault();
        clearValidation(form);

        // Valideringsregler för medlem
        const rules = {
            FirstName: {
                fn: v => /^[A-Za-zÅÄÖåäö]+$/.test(v.trim()),
                msg: "First name must only contain letters"
            },
            LastName: {
                fn: v => /^[A-Za-zÅÄÖåäö]+$/.test(v.trim()),
                msg: "Last name must only contain letters"
            },
            Phone: {
                fn: v => v === "" || /^\d+$/.test(v),
                msg: "Phone Number name must only contain numbers"
            },
            PostalCode: {
                fn: v => v === "" || /^\d+$/.test(v),
                msg: "Postal-code must only contain Numbers"
            },
            RoleId: {
                fn: v => v.trim() !== "",
                msg: "You must choose a role"
            }
        };

        if (!validateForm(form, rules)) {
            return;
        }

        // Skapa och skicka FormData
        const data = new FormData(form);
        errorContainer.innerHTML = "";

        try {
            const resp = await fetch(form.action, {
                method: "POST",
                body: data,
                credentials: "same-origin"
            });
            await handleFormResponse(resp, modal, "/Navigation/LoadMembers", errorContainer);
        } catch (err) {
            console.error("Update member error:", err);
        }
    });
}
function initMoreMenu() {
    // Förhindra dubbelbindning
    if (initMoreMenu.bound) return;
    initMoreMenu.bound = true;

    document.body.addEventListener("click", function (e) {
        // 1) Klick på själva "..."-knappen (.more-btn)
        const moreBtn = e.target.closest(".more-btn");
        if (moreBtn) {
            e.preventDefault();
            const container = moreBtn.closest(".more-container");
            const menu = container?.querySelector(".more-menu");
            if (!menu) return;

            // Dölj alla andra
            document.querySelectorAll(".more-menu")
                .forEach(m => { if (m !== menu) m.classList.add("d-none"); });

            // Växla den här
            menu.classList.toggle("d-none");
            return;
        }


        // 3) Klick på delete-knappen
        const delBtn = e.target.closest(".delete-btn");
        if (delBtn) {
            document.querySelectorAll(".more-menu")
                .forEach(m => m.classList.add("d-none"));

            const container = delBtn.closest(".more-container");
            const projectId = container?.querySelector(".more-btn")?.dataset.projectId;
            if (!projectId) return;

            document.getElementById("deleteProjectId").value = projectId;
            document.getElementById("deleteConfirmationInput").value = "";
            document.getElementById("deleteError").classList.add("d-none");
            new bootstrap.Modal(document.getElementById("deleteProjectModal")).show();
            return;
        }

        // 4) Klick utanför .more-container stänger alla menyer
        if (!e.target.closest(".more-container")) {
            document.querySelectorAll(".more-menu")
                .forEach(m => m.classList.add("d-none"));
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
function resetInitFlags() {
    const clearDataSet = (el, keys) => {
        if (!el) return;
        keys.forEach(k => delete el.dataset[k]);
    };

    clearDataSet(document.getElementById("createprojectModal"), ["memberPickerInited"]);
    clearDataSet(document.getElementById("editprojectModal"), ["memberPickerInited"]);
    clearDataSet(document.getElementById("editTeamMemberModal"), ["memberPickerInited"]);
}
function initDeleteModal() {
    const form = document.getElementById("confirmDeleteForm");
    if (!form || form.dataset.submitBound) return;

    form.addEventListener("submit", function (e) {
        e.preventDefault();

        const confirmationInput = document.getElementById("deleteConfirmationInput");
        const projectId = document.getElementById("deleteProjectId");
        const errorDiv = document.getElementById("deleteError");
        const submitBtn = form.querySelector('button[type="submit"]');  // <— hämta knappen

        if (confirmationInput.value !== "DELETE") {
            errorDiv.classList.remove("d-none");

            if (submitBtn) {
                submitBtn.classList.add("shake");
                submitBtn.addEventListener("animationend", () => {
                    submitBtn.classList.remove("shake");
                }, { once: true });
            }
            return;
        }

        fetch(`/Project/Delete/${projectId.value}`, {
            method: "DELETE"
        })
            .then(response => {
                handleDeleteResponse(
                    response,
                    document.getElementById("deleteProjectModal"),
                    "/Navigation/LoadProjects"
                );
            })
            .catch(err => {
                console.error("🚨 Fetch error vid delete:", err);
            });
    });

    form.dataset.submitBound = "true";
}
function initAddMemberModal() {
    const modalEl = document.getElementById('addMemberModal');
    if (!modalEl) return;

    modalEl.addEventListener('show.bs.modal', event => {
        const button = event.relatedTarget;
        const projectId = button.getAttribute('data-project-id');
        modalEl.querySelector('#addMemberProjectId').value = projectId;

        // Rensa eventuell tidigare state i member-picker …
        const sel = modalEl.querySelector('#selectedMembers_add');
        const ids = modalEl.querySelector('#selectedMemberIds_add');
        sel.innerHTML = '';
        ids.innerHTML = '';
        // … och initiera din memberPickerAPI för denna modal …
        window.memberPickerAPI.initPicker(
            modalEl,
            'add'
        );
    });

    // Submit‐knapp
    const submitBtn = document.getElementById('addMembersSubmit');
    submitBtn.addEventListener('click', async () => {
        const form = document.getElementById('add-member-form');
        const data = new FormData(form);
        try {
            const resp = await fetch('/Project/AddMembers', {
                method: 'POST',
                body: data
            });
            if (resp.ok) {
                bootstrap.Modal.getInstance(modalEl).hide();
                // ev. uppdatera UI (t.ex. reload projects-listan)
            } else {
                // valideringsfel …
            }
        } catch (e) {
            console.error(e);
        }
    });
}
function clearValidation(form) {
    form.querySelectorAll(".input-validation-error").forEach(i => i.classList.remove("input-validation-error"));
    form.querySelectorAll("span[data-valmsg-for]").forEach(s => s.textContent = "");
}
// Validering för modalerna.

//dessa tre sista är 99% copy-paste från chatgpt-mini-high
function validateForm(form, rules) {
    let isValid = true;
    Object.entries(rules).forEach(([name, { fn, msg }]) => {
        const input = form.querySelector(`[name="${name}"]`);
        const span = form.querySelector(`span[data-valmsg-for="${name}"]`);
        if (!input) return;            
        if (!fn(input.value)) {
            isValid = false;
            input.classList.add("input-validation-error");
            if (span) span.textContent = msg;
        }
    });
    return isValid;
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
    const submitBtn = modal.querySelector('button[type="submit"]');

    if (!response.ok) {
        // Lägg på shake-animationen
        if (submitBtn) {
            submitBtn.classList.add('shake');
            submitBtn.addEventListener('animationend', () => {
                submitBtn.classList.remove('shake');
            }, { once: true });
        }

        console.error("❌ Delete misslyckades, status:", response.status);
        return;
    }

    // Om vi kommer hit är det OK
    bootstrap.Modal.getInstance(modal)?.hide();
    const dynamicContent = document.getElementById("dynamic-content");
    const viewResponse = await fetch(reloadUrl);
    const html = await viewResponse.text();
    dynamicContent.innerHTML = html;

    resetInitFlags();
    initAll();
}




