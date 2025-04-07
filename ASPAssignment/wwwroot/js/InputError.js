document.querySelectorAll('.form-input').forEach(input => {
input.addEventListener('input', () => {
    if (input.classList.contains('input-error')) {
        input.classList.remove('input-error');
    }
});
});

document.querySelectorAll('.form-input-login').forEach(input => {
    input.addEventListener('input', () => {
        if (input.classList.contains('input-error')) {
            input.classList.remove('input-error');
        }
    });
});
