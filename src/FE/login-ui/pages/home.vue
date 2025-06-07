<template>
    <div>
        <!-- Redirect to dashboard if authenticated -->
        <div class="flex items-center justify-center min-h-screen">
            <div class="text-center">
                <div class="animate-spin inline-block w-8 h-8 border-4 border-primary border-t-transparent rounded-full mb-4"></div>
                <p class="text-gray-600 dark:text-gray-300">Redirecting to dashboard...</p>
            </div>
        </div>
    </div>
</template>

<script lang="ts" setup>
import { onMounted } from 'vue'

useHead({ title: 'Home - VRISTO' })

definePageMeta({
  middleware: 'auth'
})

const router = useRouter()

onMounted(async () => {
  // Redirect to dashboard
  await router.push('/dashboard')
})
</script>
