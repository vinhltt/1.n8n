<template>
  <div class="bg-white dark:bg-dark border-b border-gray-200 dark:border-gray-700 px-4 py-3">
    <div class="flex items-center justify-between">
      <!-- Left: Logo/Title -->
      <div class="flex items-center space-x-4">
        <NuxtLink to="/" class="flex items-center">
          <img src="/assets/images/logo.svg" alt="Logo" class="w-8 h-8" />
          <span class="ml-2 text-xl font-bold text-primary">VRISTO</span>
        </NuxtLink>
      </div>

      <!-- Right: User Info & Logout -->
      <div class="flex items-center space-x-4">
        <!-- User Info -->
        <div v-if="user" class="text-sm">
          <span class="text-gray-600 dark:text-gray-300">Welcome, </span>
          <span class="font-semibold text-primary">{{ user.fullName || user.username }}</span>
          <div class="text-xs text-gray-500">{{ user.email }}</div>
        </div>

        <!-- Logout Button -->
        <button
          @click="handleLogout"
          :disabled="isLoading"
          class="btn btn-outline-danger btn-sm"
        >
          <span v-if="isLoading" class="animate-spin inline-block w-3 h-3 border border-white border-t-transparent rounded-full mr-1"></span>
          {{ isLoading ? 'Logging out...' : 'Logout' }}
        </button>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { useAuth } from '~/composables/useAuth'

const { user, isLoading, handleLogout } = useAuth()
</script>
