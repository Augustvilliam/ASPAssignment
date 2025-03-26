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
        document.getElementById('firstname').value = member.firstName;
        document.getElementById('lastname').value = member.lastName;
        document.getElementById('email').value = member.email;
        document.getElementById('phone').value = member.phone || "";
        document.getElementById('jobtitle').value = member.jobTitle || "";
        document.getElementById('adress').value = member.adress || "";
    });
});

document.addEventListener("DOMContentLoaded", () => {
    const uploadBtn = document.getElementById("uploadPreviewBtn");
    const fileInput = document.getElementById("profilepic");
    const previewImg = document.getElementById("previewImage");

    if (!uploadBtn || !fileInput || !previewImg) return;

    // Klick på bilden → öppna file picker
    uploadBtn.addEventListener("click", () => {
        fileInput.click();
    });

    // När fil valts → visa förhandsvisning i knappen
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

    // Om användaren redan har profilbild → byt src direkt vid modalladdning
    const modal = document.getElementById('editTeamMemberModal');
    modal.addEventListener('show.bs.modal', async function (event) {
        const button = event.relatedTarget;
        const memberId = button.getAttribute('data-member-id');

        if (!memberId) return;

        const response = await fetch(`/Member/GetMember/${memberId}`);
        const member = await response.json();

        // 👇 Visa profilbild om det finns en sparad path
        if (member.profileImagePath)
            previewImg.src = member.profileImagePath;
        else
            previewImg.src = "/img/upload.svg";

        // Fyll resten som innan
        document.getElementById('member-id').value = member.id;
        document.getElementById('firstname').value = member.firstName;
        document.getElementById('lastname').value = member.lastName;
        document.getElementById('email').value = member.email;
        document.getElementById('phone').value = member.phone || "";
        document.getElementById('jobtitle').value = member.jobTitle || "";
        document.getElementById('adress').value = member.adress || "";
    });
});

