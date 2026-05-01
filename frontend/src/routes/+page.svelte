<div class="landing-container">
    <div class="geometric-pattern"></div>
    <div class="content">
        <div class="logo">SalamCircle</div>
        <h1>Welcome to SalamCircle</h1>
        <p>Ethical Prosperity, Shared Success.</p>
        <div class="actions">
            <a href="/login" class="btn btn-primary">Login</a>
            <a href="/register" class="btn btn-secondary">Register</a>
        </div>
    </div>
</div>

<style>
    .landing-container {
        display: flex;
        align-items: center;
        justify-content: center;
        height: 100vh;
        text-align: center;
        background-color: var(--surface);
        position: relative;
        overflow: hidden;
    }

    .geometric-pattern {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        opacity: 0.05;
        background-image: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M30 0l2.598 5h-5.196L30 0zm0 60l-2.598-5h5.196L30 60zM0 30l5-2.598v5.196L0 30zm60 0l-5 2.598v-5.196L60 30zM15 15l2.598 5h-5.196L15 15zm30 30l-2.598-5h5.196L45 45zm0-30l-2.598 5h5.196L45 15zM15 45l2.598-5h-5.196L15 45z' fill='%23005127' fill-rule='evenodd'/%3E%3C/svg%3E");
    }

    .content {
        position: relative;
        z-index: 1;
        max-width: 600px;
        padding: var(--spacing-lg);
    }

    .logo {
        font-size: 1.5rem;
        font-weight: 800;
        color: var(--primary);
        margin-bottom: var(--spacing-lg);
        letter-spacing: -0.03em;
    }

    h1 {
        font-size: 3.5rem;
        margin-bottom: var(--spacing-md);
        color: var(--primary);
        line-height: 1.1;
    }

    p {
        font-size: 1.25rem;
        color: var(--on-surface-variant);
        margin-bottom: var(--spacing-xl);
    }

    .actions {
        display: flex;
        gap: var(--spacing-md);
        justify-content: center;
    }

    .btn {
        padding: 0.875rem 2.5rem;
        border-radius: var(--rounded-default);
        font-weight: 700;
        text-decoration: none;
        transition: all 0.2s;
        font-size: 1rem;
        display: inline-flex;
        align-items: center;
    }

    .btn-primary {
        background-color: var(--primary);
        color: white;
    }

    .btn-primary:hover {
        background-color: var(--primary-container);
        color: white;
        text-decoration: none;
        transform: translateY(-1px);
        box-shadow: 0 4px 12px rgba(0, 81, 39, 0.15);
    }

    .btn-secondary {
        background-color: white;
        border: 1.5px solid var(--primary);
        color: var(--primary);
    }

    .btn-secondary:hover {
        background-color: var(--surface-container-low);
        text-decoration: none;
        transform: translateY(-1px);
    }
</style>
