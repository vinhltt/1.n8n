import { ref, reactive, computed } from 'vue'
import { useAuthStore, type LoginCredentials, type SignupCredentials } from '~/stores/auth'

/**
 * Interface for form validation errors
 * Giao diện cho lỗi validation form
 */
interface ValidationErrors {
  email?: string
  password?: string
  confirmPassword?: string
  firstName?: string
  lastName?: string
  username?: string
  phoneNumber?: string
  terms?: string
  general?: string
}

/**
 * Composable for authentication functionality
 * Composable cho chức năng xác thực
 */
export const useAuth = () => {
  const authStore = useAuthStore()
  const router = useRouter()

  // Form state
  const loginForm = reactive<LoginCredentials>({
    email: '',
    password: '',
  })

  const signupForm = reactive<SignupCredentials>({
    email: '',
    username: '',
    firstName: '',
    lastName: '',
    fullName: '',
    phoneNumber: '',
    password: '',
    confirmPassword: '',
  })

  const errors = ref<ValidationErrors>({})
  const isLoading = ref(false)
  const rememberMe = ref(false)
  const agreeToTerms = ref(false)

  /**
   * Computed properties for auth state
   * Computed properties cho trạng thái auth
   */
  const isAuthenticated = computed(() => authStore.isLoggedIn)
  const user = computed(() => authStore.user)
  const userRoles = computed(() => authStore.userRoles)

  /**
   * Validate email format
   * Kiểm tra định dạng email
   */
  const isValidEmail = (email: string): boolean => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    return emailRegex.test(email)
  }

  /**
   * Validate login form
   * Kiểm tra form đăng nhập
   */
  const validateForm = (): boolean => {
    errors.value = {}

    if (!loginForm.email) {
      errors.value.email = 'Email is required'
    } else if (!isValidEmail(loginForm.email)) {
      errors.value.email = 'Please enter a valid email address'
    }

    if (!loginForm.password) {
      errors.value.password = 'Password is required'
    } else if (loginForm.password.length < 6) {
      errors.value.password = 'Password must be at least 6 characters'
    }

    return Object.keys(errors.value).length === 0
  }

  /**
   * Handle login form submission
   * Xử lý submit form đăng nhập
   */
  const handleLogin = async (): Promise<boolean> => {
    if (!validateForm()) {
      return false
    }

    isLoading.value = true
    errors.value = {}

    try {
      const success = await authStore.login(loginForm)

      if (success) {
        // Reset form
        loginForm.email = ''
        loginForm.password = ''

        // Redirect to dashboard after successful login
        await router.push('/dashboard')
        return true
      } else {
        errors.value.general = authStore.error || 'Login failed. Please check your credentials.'
        return false
      }
    } catch (error: any) {
      errors.value.general = error.message || 'An unexpected error occurred'
      return false
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Handle logout
   * Xử lý đăng xuất
   */
  const handleLogout = async (): Promise<void> => {
    try {
      await authStore.logout()
      await router.push('/auth/cover-login')
    } catch (error) {
      console.error('Logout error:', error)
    }
  }

  /**
   * Initialize authentication
   * Khởi tạo xác thực
   */
  const initAuth = (): void => {
    authStore.initAuth()
  }

  /**
   * Check if user has specific role
   * Kiểm tra người dùng có quyền cụ thể không
   */
  const hasRole = (role: string): boolean => {
    return authStore.hasRole(role)
  }

  /**
   * Check if user has any of the specified roles
   * Kiểm tra người dùng có bất kỳ quyền nào trong danh sách không
   */
  const hasAnyRole = (roles: string[]): boolean => {
    return roles.some(role => authStore.hasRole(role))
  }

  /**
   * Get authorization header for API calls
   * Lấy header authorization cho API calls
   */
  const getAuthHeader = (): Record<string, string> => {
    if (authStore.token) {
      return {
        'Authorization': `Bearer ${authStore.token}`
      }
    }
    return {}
  }

  /**
   * Clear form data
   * Xóa dữ liệu form
   */
  const clearForm = (): void => {
    loginForm.email = ''
    loginForm.password = ''
    errors.value = {}
    rememberMe.value = false
  }

  /**
   * Set form error
   * Đặt lỗi cho form
   */
  const setError = (field: keyof ValidationErrors, message: string): void => {
    errors.value[field] = message
  }

  /**
   * Clear specific error
   * Xóa lỗi cụ thể
   */
  const clearError = (field: keyof ValidationErrors): void => {
    if (errors.value[field]) {
      delete errors.value[field]
    }
  }

  /**
   * Clear all errors
   * Xóa tất cả lỗi
   */
  const clearErrors = (): void => {
    errors.value = {}
  }

  /**
   * Validate signup form
   * Kiểm tra form đăng ký
   */
  const validateSignupForm = (): boolean => {
    errors.value = {}

    validateBasicFields()
    validatePasswordFields()
    validateOptionalFields()
    validateTerms()

    return Object.keys(errors.value).length === 0
  }

  /**
   * Validate basic required fields
   */
  const validateBasicFields = (): void => {
    if (!signupForm.firstName.trim()) {
      errors.value.firstName = 'First name is required'
    }

    if (!signupForm.lastName.trim()) {
      errors.value.lastName = 'Last name is required'
    }

    if (!signupForm.username.trim()) {
      errors.value.username = 'Username is required'
    } else if (signupForm.username.length < 3) {
      errors.value.username = 'Username must be at least 3 characters'
    } else if (!/^\w+$/.test(signupForm.username)) {
      errors.value.username = 'Username can only contain letters, numbers, and underscores'
    }

    if (!signupForm.email.trim()) {
      errors.value.email = 'Email is required'
    } else if (!isValidEmail(signupForm.email)) {
      errors.value.email = 'Please enter a valid email address'
    }
  }

  /**
   * Validate password fields
   */
  const validatePasswordFields = (): void => {
    if (!signupForm.password) {
      errors.value.password = 'Password is required'
    } else if (signupForm.password.length < 6) {
      errors.value.password = 'Password must be at least 6 characters'
    } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(signupForm.password)) {
      errors.value.password = 'Password must contain uppercase, lowercase, and number'
    }

    if (!signupForm.confirmPassword) {
      errors.value.confirmPassword = 'Please confirm your password'
    } else if (signupForm.password !== signupForm.confirmPassword) {
      errors.value.confirmPassword = 'Passwords do not match'
    }
  }

  /**
   * Validate optional fields
   */
  const validateOptionalFields = (): void => {
    if (signupForm.phoneNumber && signupForm.phoneNumber.trim()) {
      const phoneRegex = /^[+]?[1-9]\d{0,15}$/
      if (!phoneRegex.test(signupForm.phoneNumber.replace(/\D/g, ''))) {
        errors.value.phoneNumber = 'Please enter a valid phone number'
      }
    }
  }

  /**
   * Validate terms agreement
   */
  const validateTerms = (): void => {
    if (!agreeToTerms.value) {
      errors.value.terms = 'You must agree to the terms and conditions'
    }
  }

  /**
   * Handle signup form submission
   * Xử lý submit form đăng ký
   */
  const handleSignup = async (): Promise<boolean> => {
    if (!validateSignupForm()) {
      return false
    }

    isLoading.value = true
    errors.value = {}

    try {
      // Update fullName before sending
      signupForm.fullName = `${signupForm.firstName.trim()} ${signupForm.lastName.trim()}`

      const success = await authStore.signup(signupForm)

      if (success) {
        // Reset form
        clearSignupForm()

        // Redirect to login page with success message
        await router.push('/auth/cover-login?signup=success')
        return true
      } else {
        errors.value.general = authStore.error || 'Registration failed. Please try again.'
        return false
      }
    } catch (error: any) {
      errors.value.general = error.message || 'An unexpected error occurred during registration'
      return false
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Clear signup form data
   * Xóa dữ liệu form đăng ký
   */
  const clearSignupForm = (): void => {
    signupForm.email = ''
    signupForm.username = ''
    signupForm.firstName = ''
    signupForm.lastName = ''
    signupForm.fullName = ''
    signupForm.phoneNumber = ''
    signupForm.password = ''
    signupForm.confirmPassword = ''
    agreeToTerms.value = false
    errors.value = {}
  }

  return {
    // Form state
    loginForm,
    signupForm,
    errors,
    isLoading,
    rememberMe,
    agreeToTerms,

    // Computed
    isAuthenticated,
    user,
    userRoles,

    // Login methods
    validateForm,
    handleLogin,

    // Signup methods
    validateSignupForm,
    handleSignup,
    clearSignupForm,

    // Common methods
    handleLogout,
    initAuth,
    hasRole,
    hasAnyRole,
    getAuthHeader,
    clearForm,
    setError,
    clearError,
    clearErrors,

    // Store references for direct access if needed
    authStore,
  }
}
