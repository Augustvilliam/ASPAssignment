document.addEventListener('DOMContentLoaded', () => {
    // Create‐modal
    initMemberSearchPicker({
        toggleBtnSel: '#toggleMemberSearch_create',
        dropdownSel: '#memberSearchDropdown_create',
        inputSel: '#memberSearchInput_create',
        suggSel: '#memberSuggestions_create',
        selectedSel: '#selectedMembers_create',
        hiddenSel: '#selectedMemberIds_create',
        apiUrl: '/api/tags/members'
    });

    // Edit‐modal
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

    function resetPicker() {
        selBox.innerHTML = '';
        hiddenEl.innerHTML = '';
        input.value = '';
        dropdown.classList.add('d-none');
    }

    // Hitta närliggande .modal och bind villkorlig reset
    const modal = toggleBtn.closest('.modal');
    if (modal) {
        // Vid öppning: bara reset om vi INTE redan pre‐populerat medlemmar
        modal.addEventListener('show.bs.modal', () => {
            if (!modal.dataset.keepMembers) {
                resetPicker();
            }
        });
        // När vi stänger: ta bort flaggan så nästa gång använder vi riktiga data igen
        modal.addEventListener('hidden.bs.modal', () => {
            delete modal.dataset.keepMembers;
        });
    }

    // Klick på förstoringsglaset
    toggleBtn.addEventListener('click', async () => {
        dropdown.classList.toggle('d-none');
        if (!loaded) {
            try {
                const res = await fetch(`${opts.apiUrl}?term=`);
                allMembers = res.ok ? await res.json() : [];
                renderList(allMembers);
            } catch (e) {
                console.error('Could not load members', e);
            }
            loaded = true;
        }
        input.focus();
    });

    // Sök medan du skriver
    let timer;
    input.addEventListener('input', () => {
        clearTimeout(timer);
        timer = setTimeout(() => {
            const term = input.value.trim().toLowerCase();
            const filtered = term
                ? allMembers.filter(m => {
                    const name = `${m.firstName} ${m.lastName}`.toLowerCase();
                    return name.includes(term)
                        || m.email.toLowerCase().includes(term);
                })
                : allMembers;
            renderList(filtered);
        }, 200);
    });

    function renderList(list) {
        const selectedIds = new Set(
            Array.from(hiddenEl.querySelectorAll('input[name="SelectedMemberId"]'))
                .map(i => i.value)
        );
        const filtered = list.filter(m => !selectedIds.has(m.id));
        if (!filtered.length) {
            suggBox.innerHTML = '<li class="text-muted p-2">Inga träffar</li>';
            return;
        }
        suggBox.innerHTML = filtered.map(m => {
            const name = m.firstName || m.lastName
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

    // Klick på suggestion → tag + hidden input
    suggBox.addEventListener('click', e => {
        const li = e.target.closest('li[data-id]');
        if (!li) return;
        const id = li.dataset.id;
        const member = loaded ? allMembers.find(m => m.id === id) : null;
        if (!member) return;

        // dubletter?
        if (hiddenEl.querySelector(`input[value="${id}"]`)) {
            input.value = '';
            dropdown.classList.add('d-none');
            return;
        }

        const name = member.firstName || member.lastName
            ? `${member.firstName} ${member.lastName}`.trim()
            : member.email;
        const avatar = member.profileImagePath || '/img/default-user.svg';

        // gör tag
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
            renderList(allMembers);
        });
        selBox.appendChild(tag);

        // gör hidden-input
        const inp = document.createElement('input');
        inp.type = 'hidden';
        inp.name = 'SelectedMemberId';
        inp.value = id;
        hiddenEl.appendChild(inp);

        input.value = '';
        dropdown.classList.add('d-none');
    });

    // Klick utanför pickern stänger dropdown
    document.addEventListener('click', e => {
        if (!e.target.closest('.member-picker')) {
            dropdown.classList.add('d-none');
        }
    });
}

// API‐hjälp för att populera redan sparade medlemmar
window.memberPickerAPI = {
    addMemberToPicker(member, selBox, hiddenEl) {
        const id = member.id;
        if (hiddenEl.querySelector(`input[value="${id}"]`)) return;

        const tag = document.createElement('div');
        tag.className = 'selected-member';
        tag.innerHTML = `
      <img src="${member.avatarUrl || member.profileImagePath || '/img/default-user.svg'}" alt="">
      <span>${member.fullName || `${member.firstName} ${member.lastName}`.trim()}</span>
      <button type="button" class="btn-close" aria-label="Remove">X</button>
    `;
        tag.querySelector('button').addEventListener('click', () => {
            tag.remove();
            hiddenEl.querySelector(`input[value="${id}"]`)?.remove();
        });
        selBox.appendChild(tag);

        const inp = document.createElement('input');
        inp.type = 'hidden';
        inp.name = 'SelectedMemberId';
        inp.value = id;
        hiddenEl.appendChild(inp);
    }
};
