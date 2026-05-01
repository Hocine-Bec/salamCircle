<script lang="ts">
    import { page } from '$app/state';
    import { goto } from '$app/navigation';
    let circleId = $derived(page.params.id);

    let contributionAmount = '15,000 DZD';
    let cycleMonth = 'April 2026';

    function handleConfirm() {
        goto(`/circles/${circleId}/contribution/success`);
    }
</script>

<header class="page-header">
    <h1>Monthly Contribution</h1>
    <p class="desc">Fulfill your commitment to the community fund for this cycle.</p>
</header>

<div class="glass-card contribution-card">
    <div class="summary-section">
        <div class="summary-item">
            <span class="label">Circle</span>
            <span class="value">Family Hajj Fund</span>
        </div>
        <div class="summary-item">
            <span class="label">Cycle Month</span>
            <span class="value">{cycleMonth}</span>
        </div>
        <div class="summary-item total">
            <span class="label">Contribution Amount</span>
            <span class="value">{contributionAmount}</span>
        </div>
    </div>

    <div class="payment-method">
        <p class="section-title">Payment Method</p>
        <div class="method-card active">
            <span class="material-symbols-outlined">account_balance_wallet</span>
            <div class="method-info">
                <p class="method-name">Salam Wallet</p>
                <p class="method-desc">Balance: 1,450,000 DZD</p>
            </div>
            <span class="material-symbols-outlined check">check_circle</span>
        </div>
    </div>

    <div class="actions">
        <button class="btn btn-primary btn-block" onclick={handleConfirm}>Confirm Payment</button>
        <p class="security-note">
            <span class="material-symbols-outlined">shield</span>
            Secure Sharia-Compliant Transaction
        </p>
    </div>
</div>

<style>
    .page-header { margin-bottom: var(--spacing-xl); }
    h1 { font-size: 2rem; color: var(--primary); margin-bottom: 0.25rem; }
    .desc { color: var(--outline); }

    .contribution-card { max-width: 500px; padding: var(--spacing-xl); border-radius: var(--rounded-xl); }
    
    .summary-section { display: flex; flex-direction: column; gap: 1rem; margin-bottom: 2rem; padding-bottom: 2rem; border-bottom: 1px solid var(--surface-container); }
    .summary-item { display: flex; justify-content: space-between; align-items: center; }
    .summary-item .label { color: var(--outline); font-size: 0.875rem; }
    .summary-item .value { font-weight: 700; color: var(--on-surface); }
    .summary-item.total .value { font-size: 1.25rem; color: var(--primary); }

    .payment-method { margin-bottom: 2rem; }
    .section-title { font-size: 0.75rem; font-weight: 700; text-transform: uppercase; color: var(--outline); margin-bottom: 1rem; letter-spacing: 0.05em; }
    
    .method-card { display: flex; align-items: center; gap: 1rem; padding: 1rem; border-radius: var(--rounded-lg); border: 2px solid var(--primary); background-color: rgba(0, 81, 39, 0.02); position: relative; }
    .method-card .material-symbols-outlined:first-child { font-size: 32px; color: var(--primary); }
    .method-info .method-name { font-weight: 700; color: var(--on-surface); }
    .method-info .method-desc { font-size: 0.75rem; color: var(--outline); }
    .method-card .check { position: absolute; right: 1rem; color: var(--primary); }

    .actions { display: flex; flex-direction: column; gap: 1rem; align-items: center; }
    .btn-block { width: 100%; }
    .btn { padding: 1rem; border-radius: var(--rounded-md); font-weight: 700; border: none; cursor: pointer; transition: all 0.2s; font-size: 1rem; }
    .btn-primary { background-color: var(--primary); color: white; }
    .btn-primary:hover { background-color: #15552d; transform: translateY(-1px); }

    .security-note { display: flex; align-items: center; gap: 0.5rem; font-size: 0.75rem; color: var(--secondary); font-weight: 600; }
    .security-note .material-symbols-outlined { font-size: 16px; }
</style>
