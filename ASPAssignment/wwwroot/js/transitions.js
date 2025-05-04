
document.addEventListener('DOMContentLoaded', function () {
    setTimeout(function () {
        document.body.classList.add('fade-in-body');
    }, 10);
});
document.querySelectorAll('.modal').forEach(modalEl => {
    const dialog = modalEl.querySelector('.modal-dialog');
    modalEl.addEventListener('show.bs.modal', () => {
        // Plocka vilken animation just den modal‐IDen ska ha
        if (modalEl.id === 'createprojectModal') modalEl.classList.add('slide-left');
        if (modalEl.id === 'editTeamMemberModal') modalEl.classList.add('slide-left');
        if (modalEl.id === 'editprojectModal') modalEl.classList.add('slide-left');
    });
    modalEl.addEventListener('hidden.bs.modal', () => {
        // Ta bort klasserna så de inte återanvänds fel
        modalEl.classList.remove('pop-in', 'slide-left', 'bounce-in');
    });
});

