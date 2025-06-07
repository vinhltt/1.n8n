<template>
  <div>
    <!-- Auth Header -->
    <AuthHeader />

    <!-- Main Content -->
    <div class="min-h-screen bg-gray-50 dark:bg-dark py-8">
      <div class="max-w-4xl mx-auto px-4">
        <!-- Welcome Section -->
        <div class="bg-white dark:bg-black rounded-lg shadow-sm p-6 mb-6">
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">
            Dashboard
          </h1>
          <p class="text-gray-600 dark:text-gray-300">
            Welcome to your authenticated dashboard!
          </p>
        </div>

        <!-- User Information Card -->
        <div class="bg-white dark:bg-black rounded-lg shadow-sm p-6 mb-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">
            User Information
          </h2>
          <div v-if="user" class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">ID</label>
              <p class="mt-1 text-sm text-gray-900 dark:text-white">{{ user.id }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Username</label>
              <p class="mt-1 text-sm text-gray-900 dark:text-white">{{ user.username }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Email</label>
              <p class="mt-1 text-sm text-gray-900 dark:text-white">{{ user.email }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Full Name</label>
              <p class="mt-1 text-sm text-gray-900 dark:text-white">{{ user.fullName || 'Not set' }}</p>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Status</label>
              <span :class="user.isActive ? 'text-green-600' : 'text-red-600'" class="mt-1 text-sm font-medium">
                {{ user.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Roles</label>
              <div class="mt-1 flex flex-wrap gap-1">
                <span
                  v-for="role in user.roles"
                  :key="role"
                  class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200"
                >
                  {{ role }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Authentication Status Card -->
        <div class="bg-white dark:bg-black rounded-lg shadow-sm p-6 mb-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">
            Authentication Status
          </h2>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div class="text-center p-4 bg-green-50 dark:bg-green-900/20 rounded-lg">
              <div class="text-2xl text-green-600 mb-2">✓</div>
              <div class="text-sm font-medium text-green-800 dark:text-green-200">Authenticated</div>
              <div class="text-xs text-green-600 dark:text-green-400">{{ isAuthenticated ? 'Yes' : 'No' }}</div>
            </div>
            <div class="text-center p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
              <div class="text-2xl text-blue-600 mb-2">🔑</div>
              <div class="text-sm font-medium text-blue-800 dark:text-blue-200">Token Status</div>
              <div class="text-xs text-blue-600 dark:text-blue-400">{{ authStore.token ? 'Valid' : 'None' }}</div>
            </div>
            <div class="text-center p-4 bg-purple-50 dark:bg-purple-900/20 rounded-lg">
              <div class="text-2xl text-purple-600 mb-2">👤</div>
              <div class="text-sm font-medium text-purple-800 dark:text-purple-200">Session</div>
              <div class="text-xs text-purple-600 dark:text-purple-400">Active</div>
            </div>
          </div>
        </div>

        <!-- Actions Card -->
        <div class="bg-white dark:bg-black rounded-lg shadow-sm p-6">
          <h2 class="text-xl font-semibold text-gray-900 dark:text-white mb-4">
            Quick Actions
          </h2>
          <div class="flex flex-wrap gap-3">
            <button
              @click="showTokenDetails = !showTokenDetails"
              class="btn btn-outline-primary"
            >
              {{ showTokenDetails ? 'Hide' : 'Show' }} Token Details
            </button>
            <button
              @click="clearErrors"
              class="btn btn-outline-secondary"
            >
              Clear Errors
            </button>
            <NuxtLink
              to="/auth/cover-login"
              class="btn btn-outline-info"
            >
              Go to Login Page
            </NuxtLink>
          </div>

          <!-- Token Details -->
          <div v-if="showTokenDetails && authStore.token" class="mt-6 p-4 bg-gray-50 dark:bg-gray-800 rounded-lg">
            <h3 class="text-sm font-medium text-gray-900 dark:text-white mb-2">Token Information</h3>
            <div class="text-xs font-mono text-gray-600 dark:text-gray-400 break-all">
              {{ authStore.token }}
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue'
import { useAuth } from '~/composables/useAuth'

// Set page title and middleware
useHead({ title: 'Dashboard - Authenticated' })
definePageMeta({
  middleware: 'auth'
})

// Authentication composable
const {
  user,
  isAuthenticated,
  authStore,
  clearErrors
} = useAuth()

// Local state
const showTokenDetails = ref(false)
</script>
