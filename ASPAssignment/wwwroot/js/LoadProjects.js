document.addEventListener("DOMContentLoaded", () => {
    const uploadBtn = document.getElementById("projectUploadBtn");
    const fileInput = document.getElementById("projectImageInput");
    const previewImg = document.getElementById("projectPreviewImage");

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
});
