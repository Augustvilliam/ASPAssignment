document.addEventListener("DOMContentLoaded", () => {
    const sidebar = document.querySelector(".chat-sidebar");
    if (!sidebar) {
        console.warn("❌ Ingen .chat-sidebar hittades – kan inte initiera recipient picker.");
        return;
    }

    let selectedId = null;

    window.selectedRecipientId = null;

    fetch("/Member/Search")
        .then(res => res.json())
        .then(members => {
            sidebar.innerHTML = `<h3 style="margin-left: 1rem;">Members:</h3>`;
            members.forEach(member => {
                const button = document.createElement("button");
                button.classList.add("chat-card");
                button.dataset.userid = member.id;
                button.innerHTML = `
                    <img src="${member.avatarUrl}" alt="">
                    <span>${member.fullName}</span>
                `;
                button.addEventListener("click", () => {
                    document.querySelectorAll(".chat-card").forEach(b => b.classList.remove("active"));
                    button.classList.add("active");
                    selectedId = member.id;
                    window.selectedRecipientId = selectedId;
                    console.log("🔹 Vald mottagare:", member.fullName);
                });
                sidebar.appendChild(button);
            });
        })
        .catch(err => {
            console.error("🚨 Kunde inte hämta medlemmar:", err);
        });
});
