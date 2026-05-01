<script lang="ts">
    let requests = [
        { id: 1, requester: 'Amina Mansour', amount: '200,000 DZD', reason: 'Medical emergency - sudden surgery required.', status: 'Pending', votes: '3/8' },
        { id: 2, requester: 'Youssef Brahimi', amount: '150,000 DZD', reason: 'Home repair after flooding.', status: 'Approved', votes: '8/8' }
    ];
</script>

<header class="page-header">
    <div class="header-content">
        <h1>Emergency Requests</h1>
        <p class="desc">Review and submit requests for urgent community support.</p>
    </div>
    <button class="btn btn-error">
        <span class="material-symbols-outlined">emergency</span>
        New Request
    </button>
</header>

<div class="requests-list">
    {#each requests as req}
        <div class="glass-card request-card" class:pending={req.status === 'Pending'}>
            <div class="request-header">
                <div class="user-info">
                    <div class="avatar-stub">{req.requester.substring(0,2)}</div>
                    <div>
                        <p class="name">{req.requester}</p>
                        <p class="amount">{req.amount}</p>
                    </div>
                </div>
                <span class="status-badge" class:pending-badge={req.status === 'Pending'} class:approved-badge={req.status === 'Approved'}>
                    {req.status}
                </span>
            </div>
            <div class="request-body">
                <p class="reason-label">Reason for Request:</p>
                <p class="reason-text">{req.reason}</p>
            </div>
            <div class="request-footer">
                <div class="vote-info">
                    <span class="material-symbols-outlined">how_to_vote</span>
                    <span>Community Votes: <strong>{req.votes}</strong></span>
                </div>
                {#if req.status === 'Pending'}
                    <div class="action-btns">
                        <button class="btn-vote approve">Approve</button>
                        <button class="btn-vote reject">Reject</button>
                    </div>
                {/if}
            </div>
        </div>
    {/each}
</div>

<style>
    .page-header { display: flex; justify-content: space-between; align-items: flex-end; margin-bottom: var(--spacing-xl); }
    h1 { font-size: 2rem; color: var(--primary); margin-bottom: 0.25rem; }
    .desc { color: var(--outline); }

    .btn {
        padding: 0.75rem 1.5rem;
        border-radius: var(--rounded-md);
        font-weight: 700;
        display: flex;
        align-items: center;
        gap: 0.5rem;
        cursor: pointer;
        border: none;
    }

    .btn-error { background-color: var(--error); color: white; }

    .requests-list { display: flex; flex-direction: column; gap: var(--spacing-lg); }
    .request-card { padding: var(--spacing-lg); border-radius: var(--rounded-xl); }
    .request-card.pending { border-left: 4px solid var(--error); }

    .request-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: var(--spacing-md); }
    .user-info { display: flex; gap: 1rem; align-items: center; }
    .avatar-stub { width: 40px; height: 40px; border-radius: var(--rounded-full); background-color: var(--surface-container); display: flex; align-items: center; justify-content: center; font-weight: 700; color: var(--primary); }
    
    .user-info .name { font-weight: 700; color: var(--on-surface); }
    .user-info .amount { font-size: 1.125rem; font-weight: 700; color: var(--primary); }

    .status-badge { font-size: 0.75rem; font-weight: 700; padding: 0.25rem 0.75rem; border-radius: var(--rounded-full); }
    .pending-badge { background-color: var(--error-container); color: var(--error); }
    .approved-badge { background-color: rgba(0, 109, 56, 0.1); color: var(--secondary); }

    .request-body { margin-bottom: var(--spacing-lg); }
    .reason-label { font-size: 0.75rem; text-transform: uppercase; font-weight: 700; color: var(--outline); margin-bottom: 0.25rem; }
    .reason-text { color: var(--on-surface-variant); line-height: 1.5; }

    .request-footer { display: flex; justify-content: space-between; align-items: center; padding-top: var(--spacing-md); border-top: 1px solid var(--surface-container); }
    .vote-info { display: flex; align-items: center; gap: 0.5rem; font-size: 0.875rem; color: var(--outline); }
    .vote-info strong { color: var(--on-surface); }

    .action-btns { display: flex; gap: var(--spacing-sm); }
    .btn-vote { padding: 0.4rem 1rem; border-radius: var(--rounded-md); font-size: 0.75rem; font-weight: 700; cursor: pointer; border: none; }
    .btn-vote.approve { background-color: var(--primary); color: white; }
    .btn-vote.reject { background-color: var(--surface-container); color: var(--outline); }
</style>
