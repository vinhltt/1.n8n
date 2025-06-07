import { useAuthStore } from '~/stores/auth'

/**
 * Guest middleware
 * Middleware khách - chỉ cho phép truy cập khi chưa đăng nhập
 */
export default defineNuxtRouteMiddleware((to) => {
  const authStore = useAuthStore()

  // Skip middleware on server-side rendering
  // Bỏ qua middleware trên server-side rendering
  if (process.server) return

  // Redirect to dashboard if already authenticated
  // Chuyển hướng đến dashboard nếu đã xác thực
  if (authStore.isLoggedIn) {
    return navigateTo('/')
  }
})
