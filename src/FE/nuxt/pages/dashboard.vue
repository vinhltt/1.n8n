<template>
  <div>
    <!-- Page Header -->
    <div class="mb-6">
      <h1 class="text-3xl font-bold text-black dark:text-white">
        Dashboard
      </h1>
      <p class="text-gray-500 dark:text-gray-400 mt-2">
        Welcome back! Here's your financial overview.
      </p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
      <div 
        v-for="stat in stats" 
        :key="stat.title"
        class="panel p-6"
      >
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-lg font-semibold text-black dark:text-white mb-1">
              {{ stat.title }}
            </h3>
            <p class="text-2xl font-bold text-primary">
              {{ stat.value }}
            </p>
          </div>
          <div class="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center">
            <component 
              :is="`icon-${stat.icon}`" 
              class="text-primary"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Recent Transactions -->
      <div class="panel">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-xl font-bold text-black dark:text-white">
            Recent Transactions
          </h2>
          <NuxtLink 
            to="/apps/transactions" 
            class="text-primary hover:underline"
          >
            View All
          </NuxtLink>
        </div>
        <div class="space-y-3">
          <div class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-800 rounded">
            <div class="flex items-center">
              <div class="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center mr-3">
                <icon-shopping-cart class="text-primary" />
              </div>
              <div>
                <p class="font-medium text-black dark:text-white">Grocery Store</p>
                <p class="text-sm text-gray-500">June 10, 2025</p>
              </div>
            </div>
            <span class="text-red-500 font-medium">-$85.32</span>
          </div>
          
          <div class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-800 rounded">
            <div class="flex items-center">
              <div class="w-10 h-10 bg-success/10 rounded-full flex items-center justify-center mr-3">
                <icon-trending-up class="text-success" />
              </div>
              <div>
                <p class="font-medium text-black dark:text-white">Salary Deposit</p>
                <p class="text-sm text-gray-500">June 8, 2025</p>
              </div>
            </div>
            <span class="text-green-500 font-medium">+$3,200.00</span>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="panel">
        <h2 class="text-xl font-bold text-black dark:text-white mb-4">
          Quick Actions
        </h2>
        <div class="grid grid-cols-2 gap-4">
          <NuxtLink 
            to="/apps/transactions"
            class="flex flex-col items-center p-4 bg-primary/10 rounded-lg hover:bg-primary/20 transition-colors"
          >
            <icon-plus-circle class="text-primary mb-2" />
            <span class="text-sm font-medium text-black dark:text-white">Add Transaction</span>
          </NuxtLink>
          
          <NuxtLink 
            to="/apps/recurring-transactions"
            class="flex flex-col items-center p-4 bg-warning/10 rounded-lg hover:bg-warning/20 transition-colors"
          >
            <icon-refresh class="text-warning mb-2" />
            <span class="text-sm font-medium text-black dark:text-white">Recurring</span>
          </NuxtLink>
          
          <NuxtLink 
            to="/finance"
            class="flex flex-col items-center p-4 bg-info/10 rounded-lg hover:bg-info/20 transition-colors"
          >
            <icon-chart-square class="text-info mb-2" />
            <span class="text-sm font-medium text-black dark:text-white">Analytics</span>
          </NuxtLink>
          
          <NuxtLink 
            to="/users"
            class="flex flex-col items-center p-4 bg-success/10 rounded-lg hover:bg-success/20 transition-colors"
          >
            <icon-settings class="text-success mb-2" />
            <span class="text-sm font-medium text-black dark:text-white">Settings</span>
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Set page metadata
definePageMeta({
  layout: 'default',
  title: 'Dashboard'
})

// Sample dashboard data
const stats = ref([
  {
    title: 'Total Balance',
    value: '$24,000',
    icon: 'dollar-sign-circle'
  },
  {
    title: 'Monthly Income',
    value: '$5,400',
    icon: 'trending-up'
  },
  {
    title: 'Monthly Expenses',
    value: '$3,200',
    icon: 'arrow-wave-left-up'
  },
  {
    title: 'Savings Goal',
    value: '75%',
    icon: 'award'
  }
])
</script>

<style scoped>
.panel {
  @apply bg-white dark:bg-[#1b2e4b] rounded-lg p-6 shadow-sm border border-gray-200 dark:border-gray-700;
}
</style>
