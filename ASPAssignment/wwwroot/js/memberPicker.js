document.addEventListener('DOMContentLoaded', () => {
    // Initiera pickern för Create‐modal
    initMemberSearchPicker({
        toggleBtnSel: '#toggleMemberSearch_create',
        dropdownSel: '#memberSearchDropdown_create',
        inputSel: '#memberSearchInput_create',
        suggSel: '#memberSuggestions_create',
        selectedSel: '#selectedMembers_create',
        hiddenSel: '#selectedMemberIds_create',
        apiUrl: '/api/tags/members'
    });
    initMemberSearchPicker({
        toggleBtnSel: '#edit-toggle-member-list',
        dropdownSel: '#edit-member-dropdown',
        inputSel: '#edit-member-search-input',
        suggSel: '#edit-member-suggestions',
        selectedSel: '#edit-selected-members',
        hiddenSel: '#edit-selected-member-ids',
        apiUrl: '/api/tags/members'
    });
});

function initMemberSearchPicker(opts) {
    const toggleBtn = document.querySelector(opts.toggleBtnSel);
    const dropdown = document.querySelector(opts.dropdownSel);
    const input = document.querySelector(opts.inputSel);
    const suggBox = document.querySelector(opts.suggSel);
    const selBox = document.querySelector(opts.selectedSel);
    const hiddenEl = document.querySelector(opts.hiddenSel);
    if (!toggleBtn || !dropdown || !input || !suggBox || !selBox || !hiddenEl) return;

    let allMembers = [];
    let loaded = false;
    let lastList = [];

    // 1) Klick på förstoringsglas: visa dropdown & ladda medlemmar om första gången
    toggleBtn.addEventListener('click', async () => {
        dropdown.classList.toggle('d-none');
        if (!loaded) {
            try {
                console.log('Hämtar alla medlemmar från', opts.apiUrl);
                const res = await fetch(`${opts.apiUrl}?term=`);
                console.log('Svar från API:', res.status, await res.clone().text());
                allMembers = res.ok ? await res.json() : [];
                console.log('allMembers:', allMembers);
                renderList(allMembers);
            } catch (e) {
                console.error('Could not load members', e);
            }
            loaded = true;
        }
        input.focus();
    });



    // 2) Filtrera medan du skriver
    let timer;
    input.addEventListener('input', () => {
        clearTimeout(timer);
        timer = setTimeout(() => {
            const term = input.value.trim().toLowerCase();
            lastList = term
                ? allMembers.filter(m => {
                    const name = `${m.firstName} ${m.lastName}`.toLowerCase();
                    return name.includes(term) || m.email.toLowerCase().includes(term);
                })
                : allMembers;
            renderList(lastList);
        }, 200);
    });

    // 3) Render-funktion för dropdown‐listan
    function renderList(list) {
        if (!list.length) {
            suggBox.innerHTML = '<li class="text-muted p-2">Inga träffar</li>';
            return;
        }
        suggBox.innerHTML = list.map(m => {
            const name = (m.firstName || m.lastName)
                ? `${m.firstName} ${m.lastName}`.trim()
                : m.email;
            const avatar = m.profileImagePath || '/img/default-user.svg';
            return `
        <li data-id="${m.id}">
          <img src="${avatar}" class="avatar-sm rounded-circle me-2" alt="">
          <span>${name}</span>
        </li>`;
        }).join('');
    }

    // 4) Klick på ett list‐item → skapa tag + hidden input
    suggBox.addEventListener('click', e => {
        const li = e.target.closest('li[data-id]');
        if (!li) return;
        const id = li.dataset.id;
        const member = loaded
            ? allMembers.find(m => m.id === id)
            : null;
        if (!member) return;

        // Undvik dubletter
        if (hiddenEl.querySelector(`input[value="${id}"]`)) {
            input.value = '';
            dropdown.classList.add('d-none');
            return;
        }

        const name = (member.firstName || member.lastName)
            ? `${member.firstName} ${member.lastName}`.trim()
            : member.email;
        const avatar = member.profileImagePath || '/img/default-user.svg';

        // Bygg tag
        const tag = document.createElement('div');
        tag.className = 'selected-member';
        tag.innerHTML = `
      <img src="${avatar}" alt="">
      <span>${name}</span>
      <button type="button" class="btn-close" aria-label="Remove">X</button>
    `;
        tag.querySelector('button').addEventListener('click', () => {
            tag.remove();
            hiddenEl.querySelector(`input[value="${id}"]`)?.remove();
        });
        selBox.appendChild(tag);

        // Hidden input
        const inp = document.createElement('input');
        inp.type = 'hidden';
        inp.name = 'SelectedMemberId';
        inp.value = id;
        hiddenEl.appendChild(inp);

        // Rensa fältet & stäng dropdown
        input.value = '';
        dropdown.classList.add('d-none');
    });

    // 5) Klick utanför pickern stänger dropdown
    document.addEventListener('click', e => {
        if (!e.target.closest('.member-picker')) {
            dropdown.classList.add('d-none');
        }
    });
}
