import { writable } from 'svelte/store';

export const userRole = writable<'imam' | 'member'>('member'); // Default to imam for testing
export const currentUser = writable({
    name: 'Ahmed Ali',
    avatar: 'AA',
    phone: '+1 (555) 123-4567'
});
