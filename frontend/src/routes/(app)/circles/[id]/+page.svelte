<script lang="ts">
    import { page } from '$app/state';
    import { userRole } from '$lib/stores/auth';
    
    let circleId = $derived(page.params.id);

    let circle = {
        name: 'Family Hajj Fund',
        description: 'A collective journey towards the sacred pilgrimage.',
        memberCount: 8,
        maxMembers: 12,
        imam: 'Sheikh Abdullah',
        balance: '1,250,000 DZD',
        growth: '+12% this month',
        paidCount: 2,
        nextRecipient: 'Ahmed Ali',
        disbursementDate: 'July 2024'
    };
</script>

<header class="overview-header">
    <div class="header-content">
        <div class="badge-row">
            <span class="status-badge">Active Circle</span>
            <span class="imam-info">
                <span class="material-symbols-outlined">mosque</span>
                Imam: {circle.imam}
            </span>
        </div>
        <h1>{circle.name}</h1>
        <p class="desc">{circle.description}</p>
    </div>
    <div class="header-actions">
        {#if $userRole === 'imam'}
            <a href="/circles/{circleId}/reminders" class="btn btn-secondary-outline">
                <span class="material-symbols-outlined">mail</span>
                Send Reminder
            </a>
            <a href="/circles/{circleId}/emergency" class="btn btn-secondary-outline">
                <span class="material-symbols-outlined">fact_check</span>
                Review Requests
            </a>
            <a href="/circles/{circleId}/members" class="btn btn-primary">
                <span class="material-symbols-outlined">group</span>
                Manage Members
            </a>
        {:else}
            <a href="/circles/{circleId}/contribution" class="btn btn-primary">
                <span class="material-symbols-outlined">payments</span>
                Contribute Now
            </a>
        {/if}
    </div>
</header>

<div class="stats-grid">
    <div class="glass-card stat-card gold-border">
        <p class="stat-label">Total Balance</p>
        <h3 class="stat-value gold-text">{circle.balance}</h3>
        <div class="stat-trend">
            <span class="material-symbols-outlined">trending_up</span>
            <span>{circle.growth}</span>
        </div>
    </div>
    <div class="glass-card stat-card">
        <p class="stat-label">Members</p>
        <h3 class="stat-value">{circle.memberCount} <span class="muted">/ {circle.maxMembers}</span></h3>
        <div class="member-avatars">
            <div class="avatar-sm" style="background-color: #047857;"></div>
            <div class="avatar-sm" style="background-color: #0f766e;"></div>
            <div class="avatar-sm" style="background-color: #1d4ed8;"></div>
            <div class="avatar-sm more-badge">+{circle.memberCount - 3}</div>
        </div>
    </div>
    <div class="glass-card stat-card">
        <p class="stat-label">Monthly Contributors</p>
        <h3 class="stat-value">{circle.paidCount} <span class="muted">Paid</span></h3>
        <div class="progress-bar">
            <div class="progress-fill" style="width: 25%"></div>
        </div>
    </div>
    <div class="glass-card stat-card">
        <p class="stat-label">Next Recipient</p>
        <h3 class="stat-value">{circle.nextRecipient}</h3>
        <p class="disbursement-note">Expected: {circle.disbursementDate}</p>
    </div>
</div>

<section class="overview-details">
    <div class="glass-card info-panel">
        <h4>About this Circle</h4>
        <p>This Takaful circle was created to support community members in their journey to Hajj. All contributions are managed transparently and verified by the Imam.</p>
        <div class="info-list">
            <div class="info-item">
                <span class="material-symbols-outlined">calendar_today</span>
                <span>Created on: January 12, 2026</span>
            </div>
            <div class="info-item">
                <span class="material-symbols-outlined">account_balance</span>
                <span>Minimum Contribution: 15,000 DZD</span>
            </div>
            <div class="info-item">
                <span class="material-symbols-outlined">verified</span>
                <span>Sharia Compliant: 100% Verified</span>
            </div>
        </div>
    </div>
</section>

<style>
    .overview-header { display: flex; justify-content: space-between; align-items: flex-end; margin-bottom: var(--spacing-xl); gap: 1rem; flex-wrap: wrap; }
    .badge-row { display: flex; align-items: center; gap: var(--spacing-md); margin-bottom: var(--spacing-sm); }
    .status-badge { padding: 0.25rem 0.75rem; background-color: rgba(27, 107, 58, 0.1); color: var(--primary); border-radius: var(--rounded-full); font-size: 0.625rem; font-weight: 700; text-transform: uppercase; letter-spacing: 0.1em; border: 1px solid rgba(27, 107, 58, 0.2); }
    .imam-info { display: flex; align-items: center; gap: 0.25rem; font-size: 0.75rem; color: var(--outline); }
    .imam-info .material-symbols-outlined { font-size: 0.875rem; }

    h1 { font-size: 2.25rem; color: var(--primary); margin-bottom: 0.25rem; }
    .desc { color: var(--outline); font-size: 1rem; }

    .header-actions { display: flex; gap: var(--spacing-sm); flex-wrap: wrap; }
    .btn { padding: 0.75rem 1.5rem; border-radius: var(--rounded-md); font-weight: 700; display: flex; align-items: center; gap: 0.5rem; cursor: pointer; transition: all 0.2s; border: 1px solid transparent; font-size: 0.875rem; text-decoration: none; }
    .btn-primary { background-color: var(--primary); color: white; box-shadow: 0 4px 12px rgba(27, 107, 58, 0.15); }
    .btn-secondary-outline { background-color: white; border-color: var(--outline-variant); color: var(--on-surface-variant); }
    .btn:hover { transform: translateY(-1px); }

    .stats-grid { display: grid; grid-template-columns: repeat(1, 1fr); gap: var(--spacing-lg); margin-bottom: var(--spacing-xl); }
    @media (min-width: 640px) { .stats-grid { grid-template-columns: repeat(2, 1fr); } }
    @media (min-width: 1200px) { .stats-grid { grid-template-columns: repeat(4, 1fr); } }

    .stat-card { padding: var(--spacing-lg); border-radius: var(--rounded-lg); }
    .gold-border { border-left: 4px solid var(--gold-accent); }
    .stat-label { font-size: 0.625rem; text-transform: uppercase; font-weight: 600; color: var(--outline); margin-bottom: 0.5rem; }
    .stat-value { font-size: 1.75rem; font-weight: 500; color: var(--on-surface); }
    .stat-value .muted { font-size: 1.125rem; color: var(--outline); font-weight: 400; }
    .gold-text { color: var(--gold-accent); }
    .stat-trend { display: flex; align-items: center; gap: 0.25rem; font-size: 0.75rem; font-weight: 600; color: var(--secondary); margin-top: 0.5rem; }

    .member-avatars { display: flex; margin-top: 0.75rem; margin-left: 0.5rem; }
    .avatar-sm { width: 1.75rem; height: 1.75rem; border-radius: var(--rounded-full); border: 2px solid white; margin-left: -0.5rem; }
    .more-badge { background-color: var(--surface-container); display: flex; align-items: center; justify-content: center; font-size: 0.5rem; font-weight: 700; color: var(--outline); }

    .progress-bar { width: 100%; height: 0.5rem; background-color: var(--surface-container-high); border-radius: var(--rounded-full); margin-top: 1rem; overflow: hidden; }
    .progress-fill { height: 100%; background-color: var(--secondary); border-radius: var(--rounded-full); }
    .disbursement-note { font-size: 0.75rem; font-weight: 600; color: var(--gold-accent); margin-top: 0.5rem; }

    .info-panel { padding: var(--spacing-xl); border-radius: var(--rounded-xl); }
    .info-panel h4 { font-size: 1.25rem; color: var(--primary); margin-bottom: var(--spacing-md); }
    .info-panel p { color: var(--on-surface-variant); margin-bottom: var(--spacing-lg); line-height: 1.6; }
    .info-list { display: flex; flex-direction: column; gap: var(--spacing-md); }
    .info-item { display: flex; align-items: center; gap: 0.75rem; color: var(--on-surface); font-weight: 500; font-size: 0.875rem; }
    .info-item .material-symbols-outlined { color: var(--primary); font-size: 20px; }
</style>
