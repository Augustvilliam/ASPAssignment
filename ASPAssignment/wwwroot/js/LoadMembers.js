document.addEventListener('DOMContentLoaded', () => {
    const modal = document.getElementById('editTeamMemberModal');

    if (!modal) return;

    modal.addEventListener('show.bs.modal', async function (event) {
        const button = event.relatedTarget;
        const memberId = button.getAttribute('data-member-id');

        if (!memberId) return;

        const response = await fetch(`/Member/GetMember/${memberId}`);
        const member = await response.json();

        document.getElementById('member-id').value = member.id;
        document.getElementById('FirstName').value = member.firstName;
        document.getElementById('LastName').value = member.lastName;
        document.getElementById('Email').value = member.email;
        document.getElementById('Phone').value = member.phone || "";
        document.getElementById('JobTitle').value = member.jobTitle || "";

    });
});

document.addEventListener("DOMContentLoaded", () => {
    const uploadBtn = document.getElementById("uploadPreviewBtn");
    const fileInput = document.getElementById("profilepic");
    const previewImg = document.getElementById("previewImage");

    if (!uploadBtn || !fileInput || !previewImg) return;

    uploadBtn.addEventListener("click", () => {
        fileInput.click();
    });

    fileInput.addEventListener("change", function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                previewImg.src = e.target.result;
            };
            reader.readAsDataURL(file);
        }
    });

    const modal = document.getElementById('editTeamMemberModal');
    modal.addEventListener('show.bs.modal', async function (event) {
        const button = event.relatedTarget;
        const memberId = button.getAttribute('data-member-id');

        if (!memberId) return;

        const response = await fetch(`/Member/GetMember/${memberId}`);
        const member = await response.json();

        if (member.profileImagePath)
            previewImg.src = member.profileImagePath;
        else
            previewImg.src = "/img/upload.svg";

        document.getElementById('member-id').value = member.id;
        document.getElementById('FirstName').value = member.firstName;
        document.getElementById('LastName').value = member.lastName;
        document.getElementById('Email').value = member.email;
        document.getElementById('Phone').value = member.phone || "";
        document.getElementById('JobTitle').value = member.jobTitle || "";

    });
});

document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById('editTeamMemberModal');
    const previewImg = document.getElementById("previewImage");
    const uploadBtn = document.getElementById("uploadPreviewBtn");
    const fileInput = document.getElementById("profilepic");

    if (!modal) return;

    // Preview image handling
    uploadBtn?.addEventListener("click", () => fileInput?.click());

    fileInput?.addEventListener("change", function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => previewImg.src = e.target.result;
            reader.readAsDataURL(file);
        }
    });

    modal.addEventListener('show.bs.modal', async (event) => {
        const button = event.relatedTarget;
        const memberId = button.getAttribute('data-member-id');

        if (!memberId) return;

        const response = await fetch(`/Member/GetMember/${memberId}`);
        const member = await response.json();

        document.getElementById('member-id').value = member.id;
        document.getElementById('FirstName').value = member.firstName;
        document.getElementById('LastName').value = member.lastName;
        document.getElementById('Email').value = member.email;
        document.getElementById('Phone').value = member.phone || "";
        document.getElementById('JobTitle').value = member.jobTitle || "";


        previewImg.src = member.profileImagePath || "/img/upload.svg";
    });

    // Submit form via AJAX
    document.getElementById("edit-member-form")?.addEventListener("submit", async function (e) {
        e.preventDefault();
        const form = e.target;
        const data = new FormData(form);
        const errorContainer = document.getElementById("edit-member-errors");

        try {
            const response = await fetch("/Member/Update", {
                method: "POST",
                body: data
            });

            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    bootstrap.Modal.getInstance(modal)?.hide();
                    location.reload(); // Eller: ladda om partial
                }
            } else {
                const errors = await response.json();
                errorContainer.innerHTML = "";
                errors.forEach(field => {
                    field.errors.forEach(err => {
                        const div = document.createElement("div");
                        div.textContent = err;
                        errorContainer.appendChild(div);
                    });
                });
            }
        } catch (error) {
            console.error("Error submitting member form", error);
        }
    });
});

