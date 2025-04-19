document.addEventListener("DOMContentLoaded", () => {
    const sidebar = document.querySelector(".chat-sidebar");
    const chatView = document.querySelector(".chat-view");
    if (!sidebar || !chatView) return;

    window.selectedRecipientId = null;

    function renderHistory(history) {
        history.forEach(msg => {
            const msgDiv = document.createElement("div");
            msgDiv.classList.add(
                msg.senderId === window.currentUserId
                    ? "chat-message"
                    : "chat-message-response"
            );
            msgDiv.innerHTML = `<span>${msg.text}</span><p>${msg.senderName}</p>`;
            chatView.appendChild(msgDiv);
        });
        chatView.scrollTop = chatView.scrollHeight;
    }

    function initRecipientFocus() {
        const cards = sidebar.querySelectorAll(".chat-card");
        cards.forEach(card => {
            card.addEventListener("click", () => {
                cards.forEach(c => c.classList.remove("active"));
                card.classList.add("active");
                card.scrollIntoView({ behavior: "smooth", block: "nearest" });
            });
        });
    }

    fetch("/Member/Search")
        .then(res => res.json())
        .then(members => {
            sidebar.innerHTML = `<h3 style="margin-left: 1rem;">Members:</h3>`;

            // Filtrera bort dig själv, om du vill
            const others = members.filter(m => m.id !== window.currentUserId);

            others.forEach(member => {
                const displayName = member.fullName && member.fullName.trim()
                    ? member.fullName
                    : member.email;  // fallback till email

                const button = document.createElement("button");
                button.classList.add("chat-card");
                button.dataset.userid = member.id;
                button.innerHTML = `
                    <img src="${member.avatarUrl}" alt="">
                    <span>${displayName}</span>
                `;

                button.addEventListener("click", () => {
                    // Rensa vy & välj
                    chatView.innerHTML = "";
                    window.selectedRecipientId = member.id;

                    // Hämta historik
                    fetch(`/Chat/History?otherUserId=${member.id}`)
                        .then(r => {
                            if (!r.ok) throw new Error(`History-hämtning fel: ${r.status}`);
                            return r.text();
                        })
                        .then(text => {
                            const history = text ? JSON.parse(text) : [];
                            renderHistory(history);
                        })
                        .catch(err => {
                            console.error("🚨 Kunde inte hämta historik:", err);
                        });
                });

                sidebar.appendChild(button);
            });

            initRecipientFocus();
        })
        .catch(err => {
            console.error("🚨 Kunde inte hämta medlemmar:", err);
        });
});
