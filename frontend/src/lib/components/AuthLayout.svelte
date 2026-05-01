<script lang="ts">
    interface Props {
        children: any;
        title: string;
        subtitle: string;
        sideTitle: string;
        sideSubtitle: string;
        footerText?: string;
    }
    let { children, title, subtitle, sideTitle, sideSubtitle }: Props = $props();
</script>

<div class="auth-container">
    <div class="side-panel">
        <div class="logo">SalamCircle</div>
        <div class="side-content">
            <h1>{sideTitle}</h1>
            <p>{sideSubtitle}</p>
        </div>
        <div class="side-footer">
            <span class="badge">100% SHARIA COMPLIANT</span>
        </div>
        <!-- Subtle Islamic Geometric Pattern Watermark -->
        <div class="watermark-pattern"></div>
    </div>
    
    <div class="form-panel">
        <div class="form-container">
            <h2>{title}</h2>
            <p class="subtitle">{subtitle}</p>
            
            {@render children()}
        </div>
    </div>
</div>

<style>
    .auth-container {
        display: flex;
        min-height: 100vh;
        width: 100%;
        background-color: var(--surface);
    }

    .side-panel {
        flex: 1.2;
        background-color: var(--primary);
        color: var(--on-primary);
        padding: var(--spacing-xl);
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        position: relative;
        overflow: hidden;
    }

    .watermark-pattern {
        position: absolute;
        bottom: -50px;
        right: -50px;
        width: 400px;
        height: 400px;
        opacity: 0.03;
        pointer-events: none;
        background-image: url('data:image/svg+xml;utf8,<svg width="100" height="100" viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg"><path d="M50 0 L61 39 L100 50 L61 61 L50 100 L39 61 L0 50 L39 39 Z" fill="white"/></svg>');
        background-size: 100px 100px;
    }

    .logo {
        font-size: 1.5rem;
        font-weight: 800;
        z-index: 1;
        letter-spacing: -0.03em;
    }

    .side-content {
        z-index: 1;
        max-width: 460px;
    }

    .side-content h1 {
        font-size: 3.5rem;
        line-height: 1.05;
        margin-bottom: var(--spacing-lg);
        font-weight: 800;
    }

    .side-content p {
        font-size: 1.25rem;
        opacity: 0.85;
        font-weight: 400;
    }

    .side-footer {
        z-index: 1;
    }

    .badge {
        font-size: 0.75rem;
        letter-spacing: 0.1em;
        font-weight: 700;
        opacity: 0.6;
        border-bottom: 1px solid rgba(255,255,255,0.2);
        padding-bottom: var(--spacing-xs);
    }

    .form-panel {
        flex: 1;
        display: flex;
        align-items: center;
        justify-content: center;
        padding: var(--spacing-lg);
    }

    .form-container {
        width: 100%;
        max-width: 420px;
        background-color: var(--surface-container-lowest);
        padding: var(--spacing-xl);
        border-radius: var(--rounded-lg);
        box-shadow: 0px 4px 12px rgba(0, 81, 39, 0.04);
    }

    .form-container h2 {
        font-size: 2rem;
        margin-bottom: var(--spacing-xs);
        color: var(--on-surface);
        font-weight: 700;
    }

    .subtitle {
        color: var(--on-surface-variant);
        margin-bottom: var(--spacing-xl);
        font-size: 1rem;
    }

    @media (max-width: 1024px) {
        .side-panel {
            display: none;
        }
    }
</style>
