/**
 * Authentication plugin for Nuxt
 * Plugin xác thực cho Nuxt
 */
export default defineNuxtPlugin(() => {
  const authStore = useAuthStore()

  // Initialize authentication state on app start
  // Khởi tạo trạng thái xác thực khi ứng dụng khởi động
  if (process.client) {
    authStore.initAuth()
  }
})
