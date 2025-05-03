document.addEventListener("DOMContentLoaded", () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .configureLogging(signalR.LogLevel.Debug)
        .build();

    // Gör connection åtkomlig i konsollen för manuell test
    window.chatConnection = connection;

    const form = document.querySelector(".chat-input-container form");
    const input = form.querySelector("input");
    const chatView = document.querySelector(".chat-view");

    console.log("🔌 chat.js loaded, chatView=", chatView);
    console.log("💡 Registrerar ReceiveMessage- och ReceivePrivateMessage-handlers");

    // 1) Broadcast-meddelanden (alla)
    connection.on("ReceiveMessage", (senderName, message) => {
        console.log("📥 ReceiveMessage fired:", senderName, message);
        const msgDiv = document.createElement("div");
        msgDiv.classList.add(
            senderName === window.currentUserName
                ? "chat-message"
                : "chat-message-response"
        );
        msgDiv.innerHTML = `<span>${message}</span><p>${senderName}</p>`;
        chatView.appendChild(msgDiv);
        chatView.scrollTop = chatView.scrollHeight;
    });

    // 2) Privata meddelanden
    connection.on("ReceivePrivateMessage",
        (senderName, message, senderId, recipientId) => {

            // 1) Ignorera meddelanden som inte tillhör den öppna chatten
            if (window.selectedRecipientId !== senderId
                && window.selectedRecipientId !== recipientId) {
                return;
            }

            // 2) Avgör om det är mitt eller inkommande
            const isMe = senderId === window.currentUserId;
            const msgDiv = document.createElement("div");
            msgDiv.classList.add(isMe
                ? "chat-message"
                : "chat-message-response"
            );

            msgDiv.innerHTML = `
      <span>${message}</span>
      <p>${senderName}</p>
    `;

            chatView.appendChild(msgDiv);
            chatView.scrollTop = chatView.scrollHeight;
        });

    // 3) Skicka privatmeddelande
    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const message = input.value.trim();
        const recipientId = window.selectedRecipientId;
        console.log("🛰️ Submit fired – message:", message, "to recipient:", recipientId);

        if (!message) return;
        if (!recipientId) {
            alert("Välj en mottagare innan du skickar.");
            return;
        }

        try {
            console.log("⏳ Ringer SendPrivateMessage...");
            await connection.invoke("SendPrivateMessage", recipientId, message);
            console.log("✅ invoke-klart, tömmer input");
            input.value = "";
        } catch (err) {
            console.error("🚨 Kunde inte skicka meddelande:", err);
        }
    });

    // 4) Starta SignalR‑anslutningen
    connection.start()
        .then(() => console.log("✅ SignalR-anslutning upprättad"))
        .catch(err => console.error("SignalR error:", err));
});
