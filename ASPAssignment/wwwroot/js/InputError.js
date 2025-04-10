document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.form-input, .form-input-login').forEach(input => {
        input.addEventListener('input', () => {
            input.classList.remove('input-error');
        });
    });
});
