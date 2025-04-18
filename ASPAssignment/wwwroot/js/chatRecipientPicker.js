document.addEventListener("DOMContentLoaded", () => {
    const sidebar = document.querySelector(".chat-sidebar");
    const chatView = document.querySelector(".chat-view");
    if (!sidebar || !chatView) return;

    // 1) Definiera renderHistory här
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

    window.selectedRecipientId = null;

    fetch("/Member/Search")
        .then(res => res.json())
        .then(members => {
            sidebar.innerHTML = `<h3 style="margin-left: 1rem;">Members:</h3>`;
            members.forEach(member => {
                const button = document.createElement("button");
                /* … bygg din knapp … */
                button.addEventListener("click", () => {
                    // töm vy
                    chatView.innerHTML = "";

                    // setta ny recipient
                    window.selectedRecipientId = member.id;

                    // hämta & rendera historik
                    fetch(`/Chat/History?otherUserId=${member.id}`)
                        .then(r => r.json())
                        .then(history => renderHistory(history));
                });
                sidebar.appendChild(button);
            });
        });
});
