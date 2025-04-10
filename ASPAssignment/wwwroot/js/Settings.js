<script>
    document.addEventListener("DOMContentLoaded", () => {
        const fileInput = document.getElementById("settingsImageInput");
    const previewImg = document.getElementById("settingsPreviewImage");
    const uploadBtn = document.getElementById("settingsUploadBtn");

    if (!fileInput || !previewImg || !uploadBtn) return;

        uploadBtn.addEventListener("click", (e) => {
        e.preventDefault();
    fileInput.click();
        });

    fileInput.addEventListener("change", function () {
            const file = this.files[0];
    if (file) {
                const reader = new FileReader();
                reader.onload = (e) => {
        previewImg.src = e.target.result;
                };
    reader.readAsDataURL(file);
            }
        });
    });
</script>
