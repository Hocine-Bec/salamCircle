<script lang="ts">
    let swaps = [
        { id: 1, requester: 'Maryam Jamil', from: 'September 2026', to: 'July 2026', status: 'Open', desc: 'Need funds earlier for school fees.' },
        { id: 2, requester: 'Zubair Khan', from: 'January 2026', to: 'March 2026', status: 'Completed', desc: 'Family travel plans changed.' }
    ];
</script>

<header class="page-header">
    <div class="header-content">
        <h1>Swap Requests</h1>
        <p class="desc">Exchange your contribution turn with another member if needed.</p>
    </div>
    <button class="btn btn-secondary-outline">
        <span class="material-symbols-outlined">swap_horiz</span>
        Post Swap Request
    </button>
</header>

<div class="swaps-list">
    {#each swaps as swap}
        <div class="glass-card swap-card">
            <div class="swap-header">
                <div class="requester">
                    <div class="avatar-stub">{swap.requester[0]}</div>
                    <span class="name">{swap.requester}</span>
                </div>
                <span class="status-badge" class:completed={swap.status === 'Completed'}>{swap.status}</span>
            </div>
            <div class="swap-path">
                <div class="month-box">
                    <span class="label">Current</span>
                    <span class="month">{swap.from}</span>
                </div>
                <span class="material-symbols-outlined arrow">arrow_forward</span>
                <div class="month-box">
                    <span class="label">Requesting</span>
                    <span class="month">{swap.to}</span>
                </div>
            </div>
            <p class="reason">"{swap.desc}"</p>
            {#if swap.status === 'Open'}
                <div class="actions">
                    <button class="btn btn-primary btn-sm">Offer Swap</button>
                </div>
            {/if}
        </div>
    {/each}
</div>

<style>
    .page-header { display: flex; justify-content: space-between; align-items: flex-end; margin-bottom: var(--spacing-xl); gap: 1rem; }
    h1 { font-size: 2rem; color: var(--primary); margin-bottom: 0.25rem; }
    .desc { color: var(--outline); }

    .btn { padding: 0.75rem 1.5rem; border-radius: var(--rounded-md); font-weight: 700; display: flex; align-items: center; gap: 0.5rem; cursor: pointer; border: none; font-size: 0.875rem; }
    .btn-primary { background-color: var(--primary); color: white; }
    .btn-secondary-outline { background-color: white; border: 1px solid var(--outline-variant); color: var(--on-surface-variant); }
    .btn-sm { padding: 0.5rem 1rem; font-size: 0.75rem; }

    .swaps-list { display: grid; grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); gap: var(--spacing-lg); }
    .swap-card { padding: var(--spacing-lg); border-radius: var(--rounded-xl); }
    
    .swap-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .requester { display: flex; align-items: center; gap: 0.75rem; }
    .avatar-stub { width: 32px; height: 32px; border-radius: var(--rounded-full); background-color: var(--surface-container); color: var(--primary); display: flex; align-items: center; justify-content: center; font-weight: 700; font-size: 0.75rem; }
    .name { font-weight: 700; color: var(--on-surface); }

    .status-badge { font-size: 0.7rem; font-weight: 700; padding: 0.25rem 0.625rem; border-radius: var(--rounded-full); background-color: rgba(201, 168, 76, 0.1); color: var(--gold-accent); text-transform: uppercase; }
    .status-badge.completed { background-color: rgba(0, 109, 56, 0.1); color: var(--secondary); }

    .swap-path { display: flex; align-items: center; justify-content: center; gap: 1.5rem; margin-bottom: 1.5rem; background-color: var(--surface-container-low); padding: 1rem; border-radius: var(--rounded-lg); }
    .month-box { display: flex; flex-direction: column; align-items: center; }
    .month-box .label { font-size: 0.625rem; text-transform: uppercase; font-weight: 700; color: var(--outline); margin-bottom: 0.25rem; }
    .month-box .month { font-weight: 700; color: var(--primary); }
    .arrow { color: var(--outline-variant); }

    .reason { font-style: italic; color: var(--on-surface-variant); font-size: 0.875rem; text-align: center; margin-bottom: 1.5rem; }
    .actions { display: flex; justify-content: center; }
</style>
