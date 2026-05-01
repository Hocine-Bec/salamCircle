<script lang="ts">
    let queue = [
        { id: '01', name: 'Zubair Khan', date: 'January 2026', status: 'Completed' },
        { id: '02', name: 'Fatima Al-Sayed', date: 'February 2026', status: 'Completed' },
        { id: '03', name: 'Omar Farooq', date: 'March 2026', status: 'Completed' },
        { id: '04', name: 'Maryam Jamil', date: 'April 2026', status: 'Current', isUser: true, estimated: '1,250,000 DZD' },
        { id: '05', name: 'Ali Hassan', date: 'May 2026', status: 'Upcoming' },
        { id: '06', name: 'Zainab Ahmed', date: 'June 2026', status: 'Upcoming' }
    ];
</script>

<header class="page-header">
    <h1>Contribution Queue</h1>
    <p class="desc">The scheduled rotation for the current cycle year.</p>
</header>

<div class="glass-card queue-card">
    <div class="queue-list">
        {#each queue as item}
            <div class="queue-item" class:current={item.status === 'Current'}>
                <div class="item-left">
                    <span class="rank" class:gold={item.status === 'Current'}>{item.id}</span>
                    <div class="info">
                        <p class="name" class:highlight={item.status === 'Current'}>{item.name} {#if item.isUser} (YOU){/if}</p>
                        <p class="date">{item.date}</p>
                    </div>
                </div>
                <div class="item-right">
                    <span class="status-badge" class:completed={item.status === 'Completed'} class:current-badge={item.status === 'Current'}>
                        {item.status}
                    </span>
                    {#if item.estimated}
                        <p class="est">Est: {item.estimated}</p>
                    {/if}
                </div>
            </div>
        {/each}
    </div>
</div>

<style>
    .page-header { margin-bottom: var(--spacing-xl); }
    h1 { font-size: 2rem; color: var(--primary); margin-bottom: 0.25rem; }
    .desc { color: var(--outline); }

    .queue-card { border-radius: var(--rounded-xl); overflow: hidden; }
    .queue-list { display: flex; flex-direction: column; }
    
    .queue-item {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 1.5rem;
        border-bottom: 1px solid var(--surface-container);
        transition: background-color 0.2s;
    }

    .queue-item:hover { background-color: var(--surface-container-lowest); }
    .queue-item.current { background-color: rgba(201, 168, 76, 0.05); }

    .item-left { display: flex; align-items: center; gap: 1.5rem; }
    .rank { font-size: 1.5rem; font-weight: 700; color: var(--outline-variant); width: 2.5rem; }
    .rank.gold { color: var(--gold-accent); }

    .info .name { font-weight: 700; color: var(--on-surface); }
    .info .name.highlight { color: var(--primary); font-size: 1.125rem; }
    .info .date { font-size: 0.875rem; color: var(--outline); }

    .item-right { text-align: right; }
    .status-badge {
        font-size: 0.75rem;
        font-weight: 700;
        padding: 0.25rem 0.75rem;
        border-radius: var(--rounded-full);
        background-color: var(--surface-container-low);
        color: var(--outline);
    }
    .status-badge.completed { background-color: rgba(0, 109, 56, 0.1); color: var(--secondary); }
    .status-badge.current-badge { background-color: var(--gold-accent); color: white; }

    .est { font-size: 0.75rem; font-weight: 700; color: var(--gold-accent); margin-top: 0.25rem; }
</style>
