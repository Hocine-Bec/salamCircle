<script lang="ts">
    import { page } from '$app/state';
    import { userRole, currentUser } from '$lib/stores/auth';
    import { goto } from '$app/navigation';
    
    let { children } = $props();
    let circleId = $derived(page.params.id);
    let currentPath = $derived(page.url.pathname);

    // Sidebar items logic based on role
    const getNavItems = (role: string) => {
        if (role === 'imam') {
            return [
                { label: 'Overview', icon: 'grid_view', path: '' },
                { label: 'Members', icon: 'group', path: '/members' },
                { label: 'Transparency Logs', icon: 'list_alt', path: '/logs' },
                { label: 'Reminders', icon: 'notifications_active', path: '/reminders' },
                { label: 'Queue', icon: 'reorder', path: '/queue' },
                { label: 'Emergency Requests', icon: 'emergency', path: '/emergency' }
            ];
        } else {
            return [
                { label: 'Overview', icon: 'grid_view', path: '' },
                { label: 'Transparency Logs', icon: 'list_alt', path: '/logs' },
                { label: 'Swap Requests', icon: 'swap_horiz', path: '/swaps' },
                { label: 'Queue', icon: 'reorder', path: '/queue' },
                { label: 'Contribution', icon: 'payments', path: '/contribution' }
            ];
        }
    };

    let navItems = $derived(getNavItems($userRole));

    function isActive(itemPath: string) {
        const fullPath = `/circles/${circleId}${itemPath}`;
        if (itemPath === '') return currentPath === fullPath;
        return currentPath.startsWith(fullPath);
    }

    function handleLogout() {
        goto('/login');
    }
</script>

<div class="circle-layout">
    <aside class="sidebar">
        <div class="sidebar-header">
            <div class="logo">SalamCircle</div>
            <a href="/dashboard" class="back-link">
                <span class="material-symbols-outlined">arrow_back</span>
                Back to Dashboard
            </a>
        </div>

        <nav class="sidebar-nav">
            {#each navItems as item}
                <a 
                    href="/circles/{circleId}{item.path}" 
                    class="nav-item" 
                    class:active={isActive(item.path)}
                >
                    <span class="material-symbols-outlined">{item.icon}</span>
                    {item.label}
                </a>
            {/each}
        </nav>

        <div class="sidebar-footer">
            <div class="user-profile">
                <div class="avatar-circle">{$currentUser.name.split(' ').map(n => n[0]).join('')}</div>
                <div class="user-details">
                    <p class="name">{$currentUser.name}</p>
                    <p class="role">{$userRole === 'imam' ? 'Imam' : 'Member'}</p>
                </div>
            </div>
            <button class="logout-btn" onclick={handleLogout}>
                <span class="material-symbols-outlined">logout</span>
                Logout
            </button>
        </div>
    </aside>

    <main class="content-area geometric-pattern">
        {@render children()}
    </main>
</div>

<style>
    .circle-layout {
        display: flex;
        min-height: 100vh;
        background-color: var(--surface);
    }

    .sidebar {
        width: 280px;
        background-color: white;
        border-right: 1px solid var(--surface-container);
        display: flex;
        flex-direction: column;
        position: fixed;
        height: 100vh;
        z-index: 100;
    }

    .sidebar-header {
        padding: var(--spacing-lg);
        border-bottom: 1px solid var(--surface-container);
    }

    .logo {
        font-size: 1.5rem;
        font-weight: 800;
        color: var(--primary);
        letter-spacing: -0.03em;
        margin-bottom: 1rem;
    }

    .back-link {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        font-size: 0.8125rem;
        color: var(--outline);
        font-weight: 600;
        text-decoration: none;
        transition: color 0.2s;
    }

    .back-link:hover {
        color: var(--primary);
        text-decoration: none;
    }

    .sidebar-nav {
        flex: 1;
        padding: var(--spacing-md) var(--spacing-sm);
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
    }

    .nav-item {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        padding: 0.75rem 1rem;
        border-radius: var(--rounded-default);
        color: var(--on-surface-variant);
        font-weight: 600;
        font-size: 0.9375rem;
        transition: all 0.2s;
        text-decoration: none;
    }

    .nav-item:hover {
        background-color: var(--surface-container-low);
        color: var(--primary);
    }

    .nav-item.active {
        background-color: var(--primary);
        color: white;
    }

    .nav-item .material-symbols-outlined {
        font-size: 20px;
    }

    .sidebar-footer {
        padding: var(--spacing-lg);
        border-top: 1px solid var(--surface-container);
        display: flex;
        flex-direction: column;
        gap: 1rem;
    }

    .user-profile {
        display: flex;
        align-items: center;
        gap: 0.75rem;
    }

    .avatar-circle {
        width: 40px;
        height: 40px;
        background-color: var(--primary-container);
        color: white;
        border-radius: var(--rounded-full);
        display: flex;
        align-items: center;
        justify-content: center;
        font-weight: 700;
        font-size: 0.875rem;
    }

    .user-details .name {
        font-size: 0.875rem;
        font-weight: 700;
        color: var(--on-surface);
    }

    .user-details .role {
        font-size: 0.75rem;
        color: var(--outline);
    }

    .logout-btn {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        width: 100%;
        padding: 0.625rem 1rem;
        background: none;
        border: 1px solid var(--outline-variant);
        border-radius: var(--rounded-default);
        color: var(--error);
        font-weight: 600;
        font-size: 0.875rem;
        cursor: pointer;
        transition: all 0.2s;
    }

    .logout-btn:hover {
        background-color: var(--error-container);
        border-color: var(--error);
    }

    .content-area {
        flex: 1;
        margin-left: 280px;
        padding: var(--spacing-xl);
    }

    @media (max-width: 900px) {
        .sidebar { width: 80px; }
        .content-area { margin-left: 80px; }
        .logo, .back-link span:not(.material-symbols-outlined), .nav-item span:not(.material-symbols-outlined), .user-details, .logout-btn span:not(.material-symbols-outlined) {
            display: none;
        }
        .logout-btn { justify-content: center; padding: 0.625rem; }
    }
</style>
