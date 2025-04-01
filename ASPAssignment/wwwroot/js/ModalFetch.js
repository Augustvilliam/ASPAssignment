document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById('editTeamMemberModal');
    const previewImg = document.getElementById("previewImage");
    const uploadBtn = document.getElementById("uploadPreviewBtn");
    const fileInput = document.getElementById("profilepic");
    const form = document.getElementById("edit-member-form");
    const errorContainer = document.getElementById("edit-member-errors");

    if (!modal || !form) return;

    // 🔼 Bildförhandsvisning
    uploadBtn?.addEventListener("click", () => fileInput?.click());

    fileInput?.addEventListener("change", function () {
        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = (e) => previewImg.src = e.target.result;
            reader.readAsDataURL(file);
        }
    });

    // 📥 Fyll i formuläret när modalen öppnas
    modal.addEventListener('show.bs.modal', async (event) => {
        const button = event.relatedTarget;
        const memberId = button.getAttribute('data-member-id');
        if (!memberId) return;

        try {
            const response = await fetch(`/Member/GetMember/${memberId}`);
            if (!response.ok) throw new Error("Failed to fetch member");

            const member = await response.json();
            console.log("Fetched member:", member); // ✅ Debug

            document.getElementById('member-id').value = member.id || "";
            document.getElementById('FirstName').value = member.firstName || "";
            document.getElementById('LastName').value = member.lastName || "";
            document.getElementById('Email').value = member.email || "";
            document.getElementById('Phone').value = member.phone || "";
            document.getElementById('JobTitle').value = member.jobTitle || "";
            previewImg.src = member.profileImagePath || "/img/upload.svg";

        } catch (err) {
            console.error("Error loading member data:", err);
        }
    });

    // 🚀 Skicka uppdaterad info via AJAX
    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const data = new FormData(form);
        errorContainer.innerHTML = "";

        try {
            const response = await fetch("/Member/Update", {
                method: "POST",
                body: data
            });

            if (response.ok) {
                const result = await response.json();
                if (result.success) {
                    bootstrap.Modal.getInstance(modal)?.hide();
                    location.reload(); // Eller: uppdatera bara partial
                }
            } else {
                const errors = await response.json();
                errors.forEach(field => {
                    field.errors.forEach(err => {
                        const div = document.createElement("div");
                        div.textContent = err;
                        errorContainer.appendChild(div);
                    });
                });
            }
        } catch (err) {
            console.error("Error submitting form:", err);
        }
    });
});
