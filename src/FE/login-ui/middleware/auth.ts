import { useAuthStore } from '~/stores/auth'

/**
 * Authentication middleware
 * Middleware xác thực - bảo vệ các route yêu cầu đăng nhập
 */
export default defineNuxtRouteMiddleware((to) => {
  const authStore = useAuthStore()

  // Skip middleware on server-side rendering
  // Bỏ qua middleware trên server-side rendering
  if (process.server) return

  // Redirect to login if not authenticated
  // Chuyển hướng đến trang đăng nhập nếu chưa xác thực
  if (!authStore.isLoggedIn) {
    return navigateTo('/auth/cover-login')
  }
})
