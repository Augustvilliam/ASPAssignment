document.addEventListener('DOMContentLoaded', function () {
    // Create-modal
    window.quillCreate = new Quill('#create-description-editor', {
        theme: 'snow',
        placeholder: 'Skriv projektbeskrivning här…',
        modules: {
            toolbar: { container: '#create-toolbar' }
        }
    });

    // Edit-modal – gör tillgänglig globalt
    window.quillEdit = new Quill('#edit-description-editor', {
        theme: 'snow',
        modules: {
            toolbar: { container: '#edit-toolbar' }
        }
    });

    // Ladda in befintlig HTML i editorn
    const existing = document.getElementById('edit-description-input')?.value || '';
    quillEdit.root.innerHTML = existing;

    // På submit: flytta HTML till den dolda inputen
    document.getElementById('create-project-form')?.addEventListener('submit', function () {
        document.getElementById('create-description-input').value = quillCreate.root.innerHTML;
    });

    document.getElementById('edit-project-form')?.addEventListener('submit', function () {
        document.getElementById('edit-description-input').value = quillEdit.root.innerHTML;
    });
});
