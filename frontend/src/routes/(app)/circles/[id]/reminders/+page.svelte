<script lang="ts">
    let reminders = [
        { id: 1, recipient: 'Zubair Khan', amount: '15,000 DZD', status: 'Sent', date: '2 hours ago', tone: 'Gentle' },
        { id: 2, recipient: 'Fatima Al-Sayed', amount: '15,000 DZD', status: 'Pending', date: '-', tone: 'Urgent' }
    ];
</script>

<header class="page-header">
    <h1>Payment Reminders</h1>
    <p class="desc">Send warm and respectful reminders to members whose turn it is to contribute.</p>
</header>

<div class="glass-card reminders-container">
    <div class="reminders-list">
        <div class="list-header">
            <span>Recipient</span>
            <span>Due Amount</span>
            <span>Tone</span>
            <span>Status</span>
            <span></span>
        </div>
        {#each reminders as item}
            <div class="list-row">
                <div class="name-cell">
                    <div class="avatar-stub">{item.recipient[0]}</div>
                    <span class="name">{item.recipient}</span>
                </div>
                <span class="amount">{item.amount}</span>
                <span class="tone-badge" class:urgent={item.tone === 'Urgent'}>{item.tone}</span>
                <div class="status-info">
                    <span class="status">{item.status}</span>
                    <span class="time">{item.date}</span>
                </div>
                <div class="actions">
                    <button class="btn btn-sm" class:btn-primary={item.status === 'Pending'} disabled={item.status === 'Sent'}>
                        {item.status === 'Sent' ? 'Sent' : 'Send Reminder'}
                    </button>
                </div>
            </div>
        {/each}
    </div>
</div>

<style>
    .page-header { margin-bottom: var(--spacing-xl); }
    h1 { font-size: 2rem; color: var(--primary); margin-bottom: 0.25rem; }
    .desc { color: var(--outline); }

    .reminders-container { border-radius: var(--rounded-xl); overflow: hidden; }
    .reminders-list { display: flex; flex-direction: column; }
    
    .list-header { display: grid; grid-template-columns: 2fr 1fr 1fr 1fr 1.5fr; padding: 1rem 1.5rem; background-color: var(--surface-container-low); font-size: 0.75rem; font-weight: 700; color: var(--outline); text-transform: uppercase; letter-spacing: 0.05em; }
    
    .list-row { display: grid; grid-template-columns: 2fr 1fr 1fr 1fr 1.5fr; padding: 1.25rem 1.5rem; align-items: center; border-bottom: 1px solid var(--surface-container); transition: background-color 0.2s; }
    .list-row:hover { background-color: var(--surface-container-lowest); }
    
    .name-cell { display: flex; align-items: center; gap: 1rem; }
    .avatar-stub { width: 32px; height: 32px; border-radius: var(--rounded-full); background-color: var(--primary-container); color: var(--on-primary-container); display: flex; align-items: center; justify-content: center; font-weight: 700; font-size: 0.75rem; }
    .name { font-weight: 600; color: var(--on-surface); }
    
    .amount { font-weight: 700; color: var(--primary); }
    .tone-badge { font-size: 0.75rem; font-weight: 700; padding: 0.25rem 0.5rem; border-radius: var(--rounded-sm); background-color: rgba(201, 168, 76, 0.1); color: var(--gold-accent); width: fit-content; }
    .tone-badge.urgent { background-color: var(--error-container); color: var(--error); }
    
    .status-info { display: flex; flex-direction: column; }
    .status { font-weight: 600; font-size: 0.875rem; color: var(--on-surface); }
    .time { font-size: 0.75rem; color: var(--outline); }

    .btn { padding: 0.5rem 1rem; border-radius: var(--rounded-md); font-weight: 700; border: 1px solid var(--surface-container); cursor: pointer; font-size: 0.75rem; }
    .btn-primary { background-color: var(--primary); color: white; border: none; }
    .btn:disabled { opacity: 0.5; cursor: not-allowed; }
</style>
