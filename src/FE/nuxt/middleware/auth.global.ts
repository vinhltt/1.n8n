export default defineNuxtRouteMiddleware(async (to, from) => {
  // Skip auth check for login/signup pages and other auth routes
  // Bỏ qua kiểm tra auth cho trang login/signup và các route auth khác
  const publicRoutes = [
    '/auth/cover-login',
    '/auth/cover-signup',
    '/auth/boxed-signin',
    '/auth/boxed-signup',
    '/auth/cover-lockscreen',
    '/auth/boxed-lockscreen',
    '/auth/cover-password-reset',
    '/auth/boxed-password-reset'
  ]

  if (publicRoutes.includes(to.path)) {
    return
  }  // Identity SSO service configuration from runtime config
  // Cấu hình Identity SSO service từ runtime config
  const { $config } = useNuxtApp()
  const SSO_BASE_URL = $config.public.ssoBase
  const APP_BASE_URL = $config.public.appBase

  // Construct return URL properly for both client and server side
  // Xây dựng return URL đúng cách cho cả client và server side
  let currentUrl = ''

  if (import.meta.client) {
    // Client side: use window.location
    currentUrl = window.location.origin + to.fullPath
  } else {
    // Server side: try to get request info safely
    try {
      const event = useRequestEvent()
      if (event?.node?.req?.headers?.host) {
        const protocol = event.node.req.headers['x-forwarded-proto'] || 'http'
        currentUrl = `${protocol}://${event.node.req.headers.host}${to.fullPath}`
      } else {
        // Fallback for development - use configured app base
        currentUrl = `${APP_BASE_URL}${to.fullPath}`
      }
    } catch (error) {
      // Fallback if useRequestEvent fails - use configured app base
      currentUrl = `${APP_BASE_URL}${to.fullPath}`
    }
  }

  const loginUrl = `${SSO_BASE_URL}/auth/login?returnUrl=${encodeURIComponent(currentUrl)}`
  // Debug log to see what URL is being constructed
  console.log('🔍 Auth Middleware Debug:', {
    isClient: import.meta.client,
    ssoBase: SSO_BASE_URL,
    appBase: APP_BASE_URL,
    toPath: to.path,
    toFullPath: to.fullPath,
    currentUrl,
    loginUrl
  })

  // Check authentication status
  // Kiểm tra trạng thái xác thực
  const { $pinia } = useNuxtApp()

  if (!$pinia) {
    // If Pinia is not available, redirect to Identity SSO
    // Nếu Pinia không khả dụng, redirect về Identity SSO
    return await navigateTo(loginUrl, { external: true })
  }

  try {
    const { useAuthStore } = await import('@/stores/auth')
    const authStore = useAuthStore($pinia)

    // Initialize auth state if not already done
    // Khởi tạo trạng thái auth nếu chưa thực hiện
    if (!authStore.isAuthenticated && !authStore.isLoading) {
      await authStore.initAuth()
    }

    // If not authenticated, redirect to Identity SSO
    // Nếu chưa xác thực, redirect về Identity SSO  
    if (!authStore.isAuthenticated) {
      return await navigateTo(loginUrl, { external: true })
    }
  } catch (error) {
    // If auth store not available, redirect to Identity SSO
    // Nếu auth store không khả dụng, redirect về Identity SSO
    console.warn('Auth store not available, redirecting to Identity SSO:', error)
    return await navigateTo(loginUrl, { external: true })
  }
})
