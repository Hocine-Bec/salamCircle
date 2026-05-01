<script lang="ts">
    import { goto } from '$app/navigation';
    import AuthLayout from '$lib/components/AuthLayout.svelte';
    
    let fullName = $state('');
    let phone = $state('');
    let password = $state('');
    let showPassword = $state(false);
    let agreeTerms = $state(false);
    let error = $state('');

    async function handleRegister(e: SubmitEvent) {
        e.preventDefault();
        error = '';
        
        if (!fullName || !phone || !password) {
            error = 'Please fill in all fields.';
            return;
        }

        if (password.length < 8) {
            error = 'Password must be at least 8 characters.';
            return;
        }

        if (!agreeTerms) {
            error = 'You must agree to the Terms of Service and Privacy Policy.';
            return;
        }

        // Simulated Register
        console.log('Registering with:', { fullName, phone, password, agreeTerms });
        goto('/dashboard');
    }
</script>

<AuthLayout 
    title="Create Your Account" 
    subtitle="Join our community of ethical investors today."
    sideTitle="Ethical Prosperity"
    sideSubtitle="Built on trust. Powered by community. Rooted in Islam."
>
    {#if error}
        <div class="alert alert-error">
            <div class="alert-icon">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>
            </div>
            <div class="alert-content">
                <strong>Registration Failed</strong>
                <p>{error}</p>
            </div>
        </div>
    {/if}

    <form onsubmit={handleRegister} class="auth-form">
        <div class="form-group">
            <label for="fullName">Full Name</label>
            <div class="input-wrapper">
                <div class="input-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/><circle cx="12" cy="7" r="4"/></svg>
                </div>
                <input 
                    type="text" 
                    id="fullName" 
                    bind:value={fullName} 
                    placeholder="Enter your full name" 
                />
            </div>
        </div>

        <div class="form-group">
            <label for="phone">Phone Number</label>
            <div class="input-wrapper">
                <div class="input-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect x="5" y="2" width="14" height="20" rx="2" ry="2"/><line x1="12" y1="18" x2="12.01" y2="18"/></svg>
                </div>
                <input 
                    type="tel" 
                    id="phone" 
                    bind:value={phone} 
                    placeholder="+1 (555) 000-0000" 
                />
            </div>
        </div>

        <div class="form-group">
            <label for="password">Password</label>
            <div class="input-wrapper">
                <div class="input-icon">
                    <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="11" width="18" height="11" rx="2" ry="2"/><path d="M7 11V7a5 5 0 0 1 10 0v4"/></svg>
                </div>
                <input 
                    type={showPassword ? 'text' : 'password'} 
                    id="password" 
                    bind:value={password} 
                    placeholder="At least 8 characters" 
                />
                <button 
                    type="button" 
                    class="toggle-password" 
                    onclick={() => showPassword = !showPassword}
                    aria-label={showPassword ? 'Hide password' : 'Show password'}
                >
                    {#if showPassword}
                        <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/><line x1="1" y1="1" x2="23" y2="23"/></svg>
                    {:else}
                        <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>
                    {/if}
                </button>
            </div>
        </div>

        <div class="form-options">
            <label class="checkbox-container">
                <input type="checkbox" bind:checked={agreeTerms} />
                <span class="checkmark"></span>
                I agree to the <a href="/terms">Terms of Service</a> and <a href="/privacy">Privacy Policy</a>.
            </label>
        </div>

        <button type="submit" class="btn btn-primary btn-block">Register</button>

        <p class="auth-footer">
            Already have an account? <a href="/login">Login here</a>
        </p>
    </form>
</AuthLayout>

<style>
    .auth-form {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-lg);
    }

    .form-group {
        display: flex;
        flex-direction: column;
        gap: var(--spacing-xs);
    }

    .form-group label {
        font-size: 0.875rem;
        font-weight: 600;
        color: var(--on-surface);
    }

    .input-wrapper {
        position: relative;
        display: flex;
        align-items: center;
    }

    .input-icon {
        position: absolute;
        left: 1rem;
        color: var(--outline);
        display: flex;
        align-items: center;
        justify-content: center;
    }

    input {
        width: 100%;
        padding: 0.875rem 1rem 0.875rem 3rem;
        border: 1.5px solid transparent;
        border-radius: var(--rounded-default);
        font-size: 1rem;
        transition: all 0.2s;
        background-color: var(--surface-bright);
        color: var(--on-surface);
    }

    input::placeholder {
        color: var(--outline);
        opacity: 0.6;
    }

    input:focus {
        outline: none;
        border-color: var(--primary);
        background-color: white;
        box-shadow: 0 0 0 4px rgba(0, 81, 39, 0.05);
    }

    .toggle-password {
        position: absolute;
        right: 1rem;
        background: none;
        border: none;
        color: var(--outline);
        cursor: pointer;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: 0.5rem;
    }

    .toggle-password:hover {
        color: var(--on-surface);
    }

    .form-options {
        display: flex;
        align-items: center;
    }

    .checkbox-container {
        display: flex;
        align-items: flex-start;
        gap: var(--spacing-sm);
        font-size: 0.875rem;
        color: var(--on-surface-variant);
        cursor: pointer;
        user-select: none;
        line-height: 1.4;
    }

    .checkbox-container input {
        width: 18px;
        height: 18px;
        margin-top: 2px;
        cursor: pointer;
        accent-color: var(--primary);
    }

    .btn {
        padding: 0.875rem 1.5rem;
        border-radius: var(--rounded-default);
        font-weight: 700;
        cursor: pointer;
        transition: all 0.2s;
        border: none;
        font-size: 1rem;
        display: inline-flex;
        align-items: center;
        justify-content: center;
    }

    .btn-primary {
        background-color: var(--primary);
        color: white;
    }

    .btn-primary:hover {
        background-color: var(--primary-container);
        color: white;
        transform: translateY(-1px);
        box-shadow: 0 4px 12px rgba(0, 81, 39, 0.15);
    }

    .btn-block {
        width: 100%;
    }

    .auth-footer {
        text-align: center;
        font-size: 0.875rem;
        color: var(--on-surface-variant);
        margin-top: var(--spacing-sm);
    }

    .alert {
        display: flex;
        gap: var(--spacing-md);
        padding: var(--spacing-md);
        border-radius: var(--rounded-default);
        margin-bottom: var(--spacing-lg);
        font-size: 0.875rem;
        border: 1px solid var(--error-container);
    }

    .alert-error {
        background-color: var(--error-container);
        color: var(--on-error-container);
    }

    .alert-icon {
        flex-shrink: 0;
        color: var(--error);
    }

    .alert-content strong {
        display: block;
        margin-bottom: 2px;
        font-weight: 700;
    }
</style>
