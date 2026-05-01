<script lang="ts">
    import { page } from '$app/state';
    let circleId = $derived(page.params.id);

    let members = [
        { id: 1, name: 'Zubair Khan', phone: '+1 (555) 111-2222', role: 'Member', joined: 'Jan 2026' },
        { id: 2, name: 'Fatima Al-Sayed', phone: '+1 (555) 333-4444', role: 'Member', joined: 'Jan 2026' },
        { id: 3, name: 'Sheikh Abdullah', phone: '+1 (555) 000-0000', role: 'Imam', joined: 'Jan 2026' },
        { id: 4, name: 'Ahmed Ali', phone: '+1 (555) 123-4567', role: 'Member', joined: 'Feb 2026' }
    ];
</script>

<header class="page-header">
    <div class="header-content">
        <h1>Circle Members</h1>
        <p class="desc">Manage and invite community members to this circle.</p>
    </div>
    <a href="/circles/{circleId}/members/add" class="btn btn-primary">
        <span class="material-symbols-outlined">person_add</span>
        Add New Member
    </a>
</header>

<div class="glass-card members-card">
    <div class="members-table">
        <div class="table-header">
            <span>Name</span>
            <span>Phone</span>
            <span>Role</span>
            <span>Joined</span>
            <span></span>
        </div>
        {#each members as member}
            <div class="table-row">
                <div class="name-cell">
                    <div class="avatar-stub">{member.name[0]}</div>
                    <span class="member-name">{member.name}</span>
                </div>
                <span class="phone-cell">{member.phone}</span>
                <span class="role-cell" class:imam={member.role === 'Imam'}>{member.role}</span>
                <span class="joined-cell">{member.joined}</span>
                <div class="action-cell">
                    <button class="icon-btn material-symbols-outlined">more_vert</button>
                </div>
            </div>
        {/each}
    </div>
</div>

<style>
    .page-header { display: flex; justify-content: space-between; align-items: flex-end; margin-bottom: var(--spacing-xl); gap: 1rem; }
    h1 { font-size: 2rem; color: var(--primary); margin-bottom: 0.25rem; }
    .desc { color: var(--outline); }

    .btn { padding: 0.75rem 1.5rem; border-radius: var(--rounded-md); font-weight: 700; display: flex; align-items: center; gap: 0.5rem; cursor: pointer; transition: all 0.2s; border: none; font-size: 0.875rem; text-decoration: none; }
    .btn-primary { background-color: var(--primary); color: white; box-shadow: 0 4px 12px rgba(27, 107, 58, 0.15); }

    .members-card { border-radius: var(--rounded-xl); overflow: hidden; }
    .members-table { display: flex; flex-direction: column; }
    
    .table-header { display: grid; grid-template-columns: 2fr 1.5fr 1fr 1fr 0.5fr; padding: 1rem 1.5rem; background-color: var(--surface-container-low); font-size: 0.75rem; font-weight: 700; color: var(--outline); text-transform: uppercase; letter-spacing: 0.05em; }
    
    .table-row { display: grid; grid-template-columns: 2fr 1.5fr 1fr 1fr 0.5fr; padding: 1.25rem 1.5rem; align-items: center; border-bottom: 1px solid var(--surface-container); transition: background-color 0.2s; }
    .table-row:hover { background-color: var(--surface-container-lowest); }
    
    .name-cell { display: flex; align-items: center; gap: 1rem; }
    .avatar-stub { width: 32px; height: 32px; border-radius: var(--rounded-full); background-color: var(--primary-container); color: var(--on-primary-container); display: flex; align-items: center; justify-content: center; font-weight: 700; font-size: 0.75rem; }
    .member-name { font-weight: 600; color: var(--on-surface); }
    
    .phone-cell { color: var(--on-surface-variant); font-size: 0.875rem; }
    .role-cell { font-size: 0.75rem; font-weight: 700; padding: 0.25rem 0.5rem; border-radius: var(--rounded-sm); background-color: var(--surface-container); color: var(--outline); width: fit-content; }
    .role-cell.imam { background-color: rgba(0, 81, 39, 0.1); color: var(--primary); }
    
    .joined-cell { color: var(--outline); font-size: 0.875rem; }
    .action-cell { text-align: right; }
    .icon-btn { background: none; border: none; color: var(--outline); cursor: pointer; }
</style>
