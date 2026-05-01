<script lang="ts">
    import '../../app.css';
    let { children } = $props();
</script>

<div class="app-container">
    <main class="main-content">
        {@render children()}
    </main>
</div>

<style>
    .app-container {
        min-height: 100vh;
        background-color: var(--surface);
    }
    .main-content {
        min-height: 100vh;
        padding: var(--spacing-lg);
    }

    @media (min-width: 768px) {
        .main-content {
            padding: var(--spacing-xl);
        }
    }
</style>
