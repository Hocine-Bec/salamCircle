<script lang="ts">
    import { page } from '$app/state';
    let circleId = $derived(page.params.id);
</script>

<div class="success-container">
    <div class="success-card glass-card">
        <div class="icon-circle">
            <span class="material-symbols-outlined">check_circle</span>
        </div>
        <h1>Contribution Successful!</h1>
        <p class="desc">JazakAllah Khair for your commitment to the community. Your contribution has been recorded in the transparency log.</p>
        
        <div class="details-box">
            <div class="detail-row">
                <span>Amount Paid:</span>
                <strong>15,000 DZD</strong>
            </div>
            <div class="detail-row">
                <span>Transaction ID:</span>
                <span class="mono">#SC-2026-4412</span>
            </div>
        </div>

        <div class="actions">
            <a href="/circles/{circleId}" class="btn btn-primary btn-block">Back to Overview</a>
            <a href="/circles/{circleId}/logs" class="btn btn-ghost btn-block">View Transparency Log</a>
        </div>
    </div>
</div>

<style>
    .success-container { display: flex; align-items: center; justify-content: center; min-height: 80vh; }
    .success-card { max-width: 450px; padding: var(--spacing-xl); border-radius: var(--rounded-xl); text-align: center; }
    
    .icon-circle { width: 80px; height: 80px; background-color: rgba(0, 109, 56, 0.1); color: var(--secondary); border-radius: var(--rounded-full); display: flex; align-items: center; justify-content: center; margin: 0 auto 1.5rem; }
    .icon-circle .material-symbols-outlined { font-size: 48px; }

    h1 { font-size: 1.75rem; color: var(--primary); margin-bottom: 0.75rem; }
    .desc { color: var(--outline); font-size: 0.9375rem; margin-bottom: 2rem; line-height: 1.6; }

    .details-box { background-color: var(--surface-container-low); padding: 1.25rem; border-radius: var(--rounded-lg); margin-bottom: 2rem; }
    .detail-row { display: flex; justify-content: space-between; font-size: 0.875rem; margin-bottom: 0.5rem; }
    .detail-row:last-child { margin-bottom: 0; }
    .detail-row span { color: var(--outline); }
    .detail-row strong { color: var(--on-surface); }
    .mono { font-family: monospace; color: var(--primary); font-weight: 600; }

    .actions { display: flex; flex-direction: column; gap: 0.75rem; }
    .btn-block { width: 100%; }
    .btn { padding: 0.875rem; border-radius: var(--rounded-md); font-weight: 700; text-decoration: none; display: flex; align-items: center; justify-content: center; transition: all 0.2s; }
    .btn-primary { background-color: var(--primary); color: white; }
    .btn-ghost { color: var(--outline); border: 1px solid var(--surface-container); }
    .btn-ghost:hover { background-color: var(--surface-container-low); }
</style>
