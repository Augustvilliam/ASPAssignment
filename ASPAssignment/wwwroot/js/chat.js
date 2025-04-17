document.addEventListener("DOMContentLoaded", () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .build();

    const form = document.querySelector(".chat-input-container form");
    const input = form.querySelector("input");
    const chatView = document.querySelector(".chat-view");

    connection.on("ReceiveMessage", (user, message) => {
        const msgDiv = document.createElement("div");
        msgDiv.classList.add("chat-message");
        msgDiv.innerHTML = `<span>${message}</span><p>${user}</p>`;
        chatView.appendChild(msgDiv);
        chatView.scrollTop = chatView.scrollHeight;
    });

    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const message = input.value.trim();
        const recipientId = window.selectedRecipientId;

        if (!message) return;
        if (!recipientId) {
            alert("Välj en mottagare i listan innan du skickar.");
            return;
        }

        try {
            await connection.invoke("SendPrivateMessage", recipientId, message);
            input.value = "";
        } catch (err) {
            console.error("🚨 Kunde inte skicka meddelande:", err);
        }
    });

    connection.start()
        .then(() => console.log("✅ SignalR-anslutning upprättad"))
        .catch(err => console.error("SignalR error:", err));
});
