document.addEventListener("DOMContentLoaded", () => {
    const sidebar = document.querySelector(".chat-sidebar");
    const chatView = document.querySelector(".chat-view");

    // Event delegation för Message-knappar
    document.body.addEventListener('click', e => {
        const btn = e.target.closest('.message-btn');
        if (!btn) return;

        const memberId = btn.getAttribute('data-member-id');
        if (!memberId) return;

        // Öppna chat-modalen
        const chatModalEl = document.getElementById('chatModal');
        if (!chatModalEl) return;
        const chatModal = bootstrap.Modal.getInstance(chatModalEl) || new bootstrap.Modal(chatModalEl);
        chatModal.show();

        // Markera och ladda historik
        window.selectedRecipientId = memberId;
        document.querySelectorAll('.chat-card.active').forEach(c => c.classList.remove('active'));
        const chatBtn = document.querySelector(`.chat-card[data-userid="${memberId}"]`);
        if (chatBtn) chatBtn.classList.add('active');

        fetch(`/Chat/History?otherUserId=${memberId}`)
            .then(res => res.ok ? res.json() : Promise.reject(`History fel: ${res.status}`))
            .then(history => {
                if (typeof renderHistory === 'function') renderHistory(history);
                chatBtn?.querySelector('.unread-dot')?.remove();
            })
            .catch(err => console.error("🚨 History-fel via message-btn:", err));
    });

    if (!sidebar || !chatView) return;
    window.selectedRecipientId = null;

    // Renderfunktion för historik
    function renderHistory(history) {
        chatView.innerHTML = "";
        history.forEach(msg => {
            const isMe = msg.senderId === window.currentUserId;
            const div = document.createElement("div");
            div.classList.add(isMe ? "chat-message" : "chat-message-response");
            div.innerHTML = `
                <span>${msg.text}</span>
                <p>${msg.senderName}</p>`;
            chatView.appendChild(div);
        });
        chatView.scrollTop = chatView.scrollHeight;
    }

    // Initiera sidomeny
    function initRecipientFocus() {
        document.querySelectorAll(".chat-card").forEach(card => {
            card.addEventListener("click", () => {
                document.querySelectorAll('.chat-card.active').forEach(c => c.classList.remove('active'));
                card.classList.add("active");
                card.scrollIntoView({ behavior: "smooth", block: "nearest" });
            });
        });
    }

    // Ladda medlemmar
    fetch("/Member/Search")
        .then(res => res.ok ? res.json() : Promise.reject(`Member/Search fel: ${res.status}`))
        .then(members => {
            sidebar.innerHTML = `<h3 style=\"margin-left: 1rem;\">Members:</h3>`;
            const others = Array.isArray(members)
                ? members.filter(m => m.id !== window.currentUserId)
                : [];

            others.forEach(member => {
                const displayName = member.fullName?.trim() || member.email;
                const btn = document.createElement("button");
                btn.classList.add("chat-card");
                btn.dataset.userid = member.id;
                btn.innerHTML = `
                    <img src=\"${member.avatarUrl}\" alt=\"\">
                    <span>${displayName}</span>`;

                btn.addEventListener("click", () => {
                    window.selectedRecipientId = member.id;
                    sidebar.querySelectorAll('.chat-card.active').forEach(c => c.classList.remove('active'));
                    btn.classList.add('active');
                    chatView.innerHTML = "";
                    fetch(`/Chat/History?otherUserId=${member.id}`)
                        .then(r => r.ok ? r.json() : Promise.reject(`History fel: ${r.status}`))
                        .then(history => {
                            renderHistory(history);
                            btn.querySelector('.unread-dot')?.remove();
                        })
                        .catch(err => console.error("🚨 History-fel:", err));
                });
                sidebar.appendChild(btn);
            });
            initRecipientFocus();

            // Lägg initiala oläst prickar
            fetch("/Chat/UnreadCounts")
                .then(r => r.ok ? r.json() : [])
                .then(unreadList => {
                    const map = Object.fromEntries((Array.isArray(unreadList) ? unreadList : [])
                        .map(u => [u.otherUserId, u.unreadCount]));
                    document.querySelectorAll('.chat-card').forEach(c => {
                        if (map[c.dataset.userid] > 0) {
                            const dot = document.createElement('span');
                            dot.classList.add('unread-dot');
                            c.querySelector('span')?.after(dot);
                        }
                    });
                })
                .catch(err => console.warn("⚠️ Initial unreadCounts fel:", err));
        })
        .catch(err => console.error("🚨 Fel vid laddning medlemmar:", err));
});
