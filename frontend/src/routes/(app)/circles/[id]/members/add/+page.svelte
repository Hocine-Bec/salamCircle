<script lang="ts">
    import { page } from '$app/state';
    let circleId = $derived(page.params.id);

    let phone = $state('');
    let autoAssign = $state(true);
    let queuePosition = $state('Next Available');
</script>

<header class="page-header">
    <div class="header-left">
        <a href="/circles/{circleId}/members" class="back-link">
            <span class="material-symbols-outlined">arrow_back</span>
            Back to Members
        </a>
        <h1>Add New Member</h1>
        <p class="desc">Invite a registered community member by their phone number.</p>
    </div>
</header>

<div class="glass-card form-container">
    <form class="add-member-form">
        <div class="form-group">
            <label for="phone">Phone Number</label>
            <div class="input-wrapper">
                <span class="material-symbols-outlined input-icon">phone</span>
                <input type="tel" id="phone" bind:value={phone} placeholder="+1 (555) 000-0000" />
            </div>
            <p class="input-hint">The user must have a registered SalamCircle account.</p>
        </div>

        <div class="form-group">
            <label for="queue-pos">Queue Position</label>
            <div class="input-wrapper">
                <span class="material-symbols-outlined input-icon">reorder</span>
                <input type="text" id="queue-pos" bind:value={queuePosition} disabled />
            </div>
        </div>

        <div class="form-group">
            <label class="checkbox-container">
                <input type="checkbox" bind:checked={autoAssign} />
                <span class="checkmark"></span>
                Auto-assign next available position in queue
            </label>
        </div>

        <div class="form-actions">
            <button type="button" class="btn btn-primary">Send Invitation</button>
            <a href="/circles/{circleId}/members" class="btn btn-ghost">Cancel</a>
        </div>
    </form>
</div>

<style>
    .page-header { margin-bottom: var(--spacing-xl); }
    .back-link { display: flex; align-items: center; gap: 0.5rem; color: var(--outline); font-weight: 600; text-decoration: none; margin-bottom: 1rem; font-size: 0.875rem; }
    .back-link:hover { color: var(--primary); }
    h1 { font-size: 2rem; color: var(--primary); margin-bottom: 0.25rem; }
    .desc { color: var(--outline); }

    .form-container { max-width: 600px; padding: var(--spacing-xl); border-radius: var(--rounded-xl); }
    .add-member-form { display: flex; flex-direction: column; gap: 2rem; }
    
    .form-group { display: flex; flex-direction: column; gap: 0.5rem; }
    .form-group label { font-size: 0.875rem; font-weight: 700; color: var(--on-surface); }
    
    .input-wrapper { position: relative; display: flex; align-items: center; }
    .input-icon { position: absolute; left: 1rem; color: var(--outline); font-size: 20px; }
    
    input { width: 100%; padding: 0.875rem 1rem 0.875rem 3rem; border: 1.5px solid var(--surface-container); border-radius: var(--rounded-default); font-size: 1rem; background-color: var(--surface-bright); transition: all 0.2s; }
    input:focus { outline: none; border-color: var(--primary); background-color: white; box-shadow: 0 0 0 4px rgba(0, 81, 39, 0.05); }
    input:disabled { background-color: var(--surface-container-low); cursor: not-allowed; opacity: 0.7; }

    .input-hint { font-size: 0.75rem; color: var(--outline); }

    .checkbox-container { display: flex; align-items: center; gap: 0.75rem; font-size: 0.875rem; color: var(--on-surface-variant); cursor: pointer; user-select: none; }
    .checkbox-container input { width: 18px; height: 18px; margin: 0; accent-color: var(--primary); }

    .form-actions { display: flex; gap: 1rem; margin-top: 1rem; }
    .btn { padding: 0.875rem 2rem; border-radius: var(--rounded-md); font-weight: 700; border: none; cursor: pointer; transition: all 0.2s; font-size: 0.875rem; text-decoration: none; display: flex; align-items: center; justify-content: center; }
    .btn-primary { background-color: var(--primary); color: white; box-shadow: 0 4px 12px rgba(0, 81, 39, 0.15); }
    .btn-primary:hover { background-color: #15552d; transform: translateY(-1px); }
    .btn-ghost { color: var(--outline); border: 1px solid var(--surface-container); }
    .btn-ghost:hover { background-color: var(--surface-container-low); }
</style>
