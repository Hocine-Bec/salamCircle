<script lang="ts">
    import { userRole, currentUser } from '$lib/stores/auth';
    import { goto } from '$app/navigation';
    
    let circles = [
        { id: 1, name: 'Family Hajj Fund', balance: '1,750,000 DZD', members: 8, nextDate: 'July 15', isMyTurn: true, type: 'hajj' },
        { id: 2, name: 'Tech Explorers', balance: '450,000 DZD', members: 5, cycle: '4/12', isMyTurn: false, type: 'tech' },
        { id: 3, name: 'Community Sadaqah', balance: '120,000 DZD', members: 12, growth: '+15%', isMyTurn: false, type: 'sadaqah' }
    ];

    let pendingActions = [
        { id: 1, title: 'Invitation: Umrah 2025', desc: 'Brother Omar invited you to join a savings circle.', type: 'invitation' },
        { id: 2, title: 'Swap Request', desc: 'Yusuf wants to swap July for August in Tech Explorers.', type: 'swap' },
        { id: 3, title: 'Emergency Request', desc: 'Amina requested early payout for the Sadaqah circle.', type: 'emergency' }
    ];

    let filteredActions = $derived(pendingActions.filter(action => {
        if (action.type === 'emergency') return $userRole === 'imam';
        return true;
    }));

    function handleLogout() {
        goto('/login');
    }
</script>

<div class="dashboard-container">
    <div class="dashboard-header">
    <div class="header-left">
        <h1>As-salaam alaykum, Ahmed</h1>
        <p>Manage your ethical community funds and financial goals.</p>
    </div>
    <div class="header-right">
        <div class="user-chip">
            <div class="avatar">{$currentUser.name.split(' ').map(n => n[0]).join('')}</div>
            <div class="details">
                <p class="name">{$currentUser.name}</p>
                <p class="role">{$userRole === 'imam' ? 'Imam' : 'Member'}</p>
            </div>
        </div>
        <button class="icon-btn logout" onclick={handleLogout} title="Logout">
            <span class="material-symbols-outlined">logout</span>
        </button>
    </div>
</div>

<div class="dashboard-grid">
    <div class="left-column">
        <section class="circles-section">
            <div class="section-header">
                <h2>My Circles</h2>
            </div>
            <div class="circles-grid">
                {#each circles as circle}
                    <a href="/circles/{circle.id}" class="circle-card-link">
                        <div class="circle-card" class:my-turn={circle.isMyTurn} class:dark-card={circle.type === 'tech'}>
                            <div class="card-badge-row">
                                {#if circle.isMyTurn}
                                    <span class="turn-badge">Your Turn</span>
                                {:else if circle.type === 'tech'}
                                    <div class="type-icon"><span class="material-symbols-outlined">laptop_mac</span></div>
                                {:else}
                                     <div class="type-icon sadaqah"><span class="material-symbols-outlined">volunteer_activism</span></div>
                                {/if}
                                <button class="material-symbols-outlined more-btn">more_vert</button>
                            </div>
                            
                            <h3>{circle.name}</h3>
                            <div class="balance-row">
                                <span class="amount">{circle.balance}</span>
                                <span class="label">{circle.type === 'sadaqah' ? 'Monthly Pot' : 'Balance'}</span>
                            </div>

                            <div class="card-footer">
                                <div class="footer-item">
                                    <span class="material-symbols-outlined">group</span>
                                    {circle.members} Members
                                </div>
                                <div class="footer-item">
                                    {#if circle.nextDate}
                                        <span class="material-symbols-outlined">event</span>
                                        Next: {circle.nextDate}
                                    {:else if circle.cycle}
                                        <span class="material-symbols-outlined">sync</span>
                                        Cycle {circle.cycle}
                                    {:else if circle.growth}
                                        <span class="material-symbols-outlined">trending_up</span>
                                        {circle.growth} Growth
                                    {/if}
                                </div>
                            </div>
                        </div>
                    </a>
                {/each}

                <button class="create-circle-card">
                    <div class="add-icon">
                        <span class="material-symbols-outlined">add</span>
                    </div>
                    <span>Start New Circle</span>
                </button>
            </div>
        </section>

        <section class="activity-link-section">
            <a href="/logs" class="glass-card activity-btn">
                <div class="btn-content">
                    <span class="material-symbols-outlined">list_alt</span>
                    <div>
                        <p class="btn-title">View Transparency Logs</p>
                        <p class="btn-subtitle">Review all recent global activity</p>
                    </div>
                </div>
                <span class="material-symbols-outlined">chevron_right</span>
            </a>
        </section>
    </div>

    <aside class="right-column">
        <section class="pending-section">
            <h2>Pending Actions</h2>
            <div class="actions-list">
                {#each filteredActions as action}
                    <div class="action-card" class:invitation={action.type === 'invitation'} class:swap={action.type === 'swap'} class:emergency={action.type === 'emergency'}>
                        <div class="action-icon">
                            <span class="material-symbols-outlined" style={action.type === 'invitation' ? "font-variation-settings: 'FILL' 1" : ""}>
                                {action.type === 'invitation' ? 'mail' : action.type === 'swap' ? 'swap_horiz' : 'emergency'}
                            </span>
                        </div>
                        <div class="action-body">
                            <p class="action-title">{action.title}</p>
                            <p class="action-desc">{action.desc}</p>
                            <div class="action-btns">
                                {#if action.type === 'invitation'}
                                    <button class="btn-action primary">Accept</button>
                                    <button class="btn-action outline">Decline</button>
                                {:else if action.type === 'swap'}
                                    <button class="btn-action secondary">Review Request</button>
                                {:else if action.type === 'emergency'}
                                    <button class="btn-action error">Vote Now</button>
                                {/if}
                            </div>
                        </div>
                    </div>
                {/each}
            </div>
        </section>
    </aside>
</div>
</div>

<style>
    .dashboard-container {
        max-width: 1280px;
        margin: 0 auto;
        width: 100%;
    }
    .dashboard-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: var(--spacing-xl); gap: 2rem; flex-wrap: wrap; }
    .header-left h1 { font-size: 2rem; color: var(--primary); margin-bottom: 0.5rem; }
    .header-left p { font-size: 1.125rem; color: var(--outline); }

    .header-right { display: flex; align-items: center; gap: var(--spacing-md); }
    .user-chip { display: flex; align-items: center; gap: 0.75rem; padding: 0.5rem 1rem 0.5rem 0.5rem; background-color: white; border-radius: var(--rounded-full); border: 1px solid var(--surface-container); }
    .avatar { width: 32px; height: 32px; background-color: var(--primary-container); color: white; border-radius: var(--rounded-full); display: flex; align-items: center; justify-content: center; font-weight: 700; font-size: 0.75rem; }
    .details .name { font-size: 0.8125rem; font-weight: 700; color: var(--on-surface); }
    .details .role { font-size: 0.6875rem; color: var(--outline); line-height: 1; }
    
    .icon-btn { width: 40px; height: 40px; border-radius: var(--rounded-full); display: flex; align-items: center; justify-content: center; border: 1px solid var(--surface-container); background-color: white; color: var(--outline); cursor: pointer; transition: all 0.2s; }
    .icon-btn:hover { background-color: var(--error-container); color: var(--error); border-color: var(--error); }

    .dashboard-grid { display: grid; grid-template-columns: 1fr; gap: var(--spacing-lg); }

    @media (min-width: 1024px) {
        .dashboard-grid { grid-template-columns: repeat(12, 1fr); }
        .left-column { grid-column: span 8; }
        .right-column { grid-column: span 4; }
    }

    .section-header { margin-bottom: var(--spacing-lg); }
    .section-header h2 { font-size: 1.5rem; color: var(--on-surface); }

    .circles-grid { display: grid; grid-template-columns: 1fr; gap: var(--spacing-md); }
    @media (min-width: 640px) { .circles-grid { grid-template-columns: repeat(2, 1fr); } }

    .circle-card-link { text-decoration: none !important; color: inherit !important; }
    .circle-card {
        padding: var(--spacing-lg);
        border-radius: var(--rounded-lg);
        position: relative;
        overflow: hidden;
        box-shadow: 0 4px 12px rgba(0,0,0,0.05);
        transition: transform 0.2s;
    }
    .circle-card:hover { transform: translateY(-2px); }
    .circle-card.my-turn { background-color: var(--primary-container); color: white; }
    .circle-card.dark-card { background-color: var(--inverse-surface); color: white; }
    .circle-card:not(.my-turn):not(.dark-card) { background-color: white; border: 1px solid var(--surface-container); }

    .card-badge-row { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: var(--spacing-lg); }
    .turn-badge { background-color: #D4AF37; color: #3f0111; font-size: 0.75rem; font-weight: 700; padding: 0.25rem 0.75rem; border-radius: var(--rounded-full); }
    .type-icon { width: 40px; height: 40px; background-color: var(--secondary); border-radius: var(--rounded-default); display: flex; align-items: center; justify-content: center; }
    .type-icon.sadaqah { background-color: var(--tertiary-fixed-dim); color: var(--on-tertiary-fixed); }
    .more-btn { background: none; border: none; color: inherit; opacity: 0.6; }

    .circle-card h3 { font-size: 1.25rem; margin-bottom: 0.25rem; }
    .balance-row { display: flex; align-items: baseline; gap: 0.5rem; margin-bottom: var(--spacing-lg); }
    .amount { font-size: 1.75rem; font-weight: 500; }
    .label { font-size: 0.75rem; text-transform: uppercase; opacity: 0.7; }
    .card-footer { display: flex; gap: var(--spacing-md); font-size: 0.875rem; opacity: 0.9; }
    .footer-item { display: flex; align-items: center; gap: 0.25rem; }

    .create-circle-card {
        border: 2px dashed var(--outline-variant);
        border-radius: var(--rounded-lg);
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        gap: var(--spacing-sm);
        background: none;
        cursor: pointer;
        padding: var(--spacing-lg);
    }
    .add-icon { width: 48px; height: 48px; border-radius: var(--rounded-full); border: 2px solid var(--primary); color: var(--primary); display: flex; align-items: center; justify-content: center; }
    .create-circle-card span { font-weight: 600; color: var(--primary); }

    .activity-link-section { margin-top: var(--spacing-xl); }
    .activity-btn {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: var(--spacing-lg);
        border-radius: var(--rounded-lg);
        text-decoration: none;
        color: var(--on-surface);
        transition: background-color 0.2s;
    }
    .activity-btn:hover { background-color: var(--surface-container-low); }
    .btn-content { display: flex; gap: var(--spacing-md); align-items: center; }
    .btn-content .material-symbols-outlined { font-size: 32px; color: var(--primary); }
    .btn-title { font-weight: 700; font-size: 1rem; }
    .btn-subtitle { font-size: 0.875rem; color: var(--outline); }

    .pending-section h2 { font-size: 1.25rem; margin-bottom: var(--spacing-lg); }
    .actions-list { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .action-card { background-color: white; border-left: 4px solid var(--primary); padding: var(--spacing-md); border-radius: 0 var(--rounded-default) var(--rounded-default) 0; box-shadow: 0 2px 8px rgba(0,0,0,0.04); display: flex; gap: var(--spacing-md); }
    .action-card.swap { border-left-color: var(--secondary); }
    .action-card.emergency { border-left-color: var(--error); background-color: rgba(186, 26, 26, 0.02); }
    .action-icon { color: var(--primary); }
    .swap .action-icon { color: var(--secondary); }
    .emergency .action-icon { color: var(--error); }
    .action-title { font-weight: 700; font-size: 0.875rem; color: var(--on-surface); margin-bottom: 0.25rem; }
    .action-desc { font-size: 0.875rem; color: var(--outline); margin-bottom: 1rem; }
    .action-btns { display: flex; gap: var(--spacing-sm); }
    .btn-action { padding: 0.4rem 1rem; border-radius: var(--rounded-md); font-size: 0.75rem; font-weight: 700; cursor: pointer; border: none; }
    .btn-action.primary { background-color: var(--primary); color: white; }
    .btn-action.outline { border: 1px solid var(--outline); background: none; color: var(--outline); }
    .btn-action.secondary { background-color: var(--secondary); color: white; }
    .btn-action.error { background-color: var(--error); color: white; }
</style>
