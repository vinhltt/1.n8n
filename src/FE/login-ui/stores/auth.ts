import { defineStore } from 'pinia'

/**
 * Interface for user data structure
 * Giao diện cho cấu trúc dữ liệu người dùng
 */
export interface User {
  id: string
  email: string
  username: string
  fullName?: string
  roles?: string[]
  isActive: boolean
}

/**
 * Interface for login credentials
 * Giao diện cho thông tin đăng nhập
 */
export interface LoginCredentials {
  email: string
  password: string
}

/**
 * Interface for signup credentials
 * Giao diện cho thông tin đăng ký
 */
export interface SignupCredentials {
  email: string
  username: string
  firstName: string
  lastName: string
  fullName: string
  phoneNumber?: string
  password: string
  confirmPassword: string
}

/**
 * Interface for authentication response
 * Giao diện cho phản hồi xác thực
 */
export interface AuthResponse {
  token: string
  user: User
  refreshToken?: string
}

/**
 * Interface for signup credentials
 * Giao diện cho thông tin đăng ký
 */
export interface SignupCredentials {
  email: string
  username: string
  firstName: string
  lastName: string
  fullName: string
  phoneNumber?: string
  password: string
  confirmPassword: string
}

/**
 * Authentication store using Pinia
 * Store xác thực sử dụng Pinia
 */
export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null as User | null,
    token: null as string | null,
    refreshToken: null as string | null,
    isAuthenticated: false,
    isLoading: false,
    error: null as string | null,
  }),

  getters: {
    /**
     * Check if user is logged in
     * Kiểm tra người dùng đã đăng nhập chưa
     */
    isLoggedIn: (state) => !!state.token && !!state.user,

    /**
     * Get user roles
     * Lấy quyền của người dùng
     */
    userRoles: (state) => state.user?.roles || [],

    /**
     * Check if user has specific role
     * Kiểm tra người dùng có quyền cụ thể không
     */
    hasRole: (state) => (role: string) => state.user?.roles?.includes(role) || false,
  },

  actions: {
    /**
     * Initialize authentication state from storage
     * Khởi tạo trạng thái xác thực từ storage
     */
    initAuth() {
      if (process.client) {
        try {
          const token = localStorage.getItem('auth.token')
          const user = localStorage.getItem('auth.user')
          const refreshToken = localStorage.getItem('auth.refreshToken')

          if (token && user) {
            this.token = token
            this.user = JSON.parse(user)
            this.refreshToken = refreshToken
            this.isAuthenticated = true
          }
        } catch (error) {
          console.error('Error initializing auth:', error)
          this.clearAuth()
        }
      }
    },    /**
     * Login with email and password
     * Đăng nhập bằng email và mật khẩu
     */
    async login(credentials: LoginCredentials): Promise<boolean> {
      this.isLoading = true
      this.error = null

      try {
        // Import auth service dynamically to avoid circular dependency
        const { authService } = await import('~/services/auth')

        // Try real API first, fallback to mock
        let response: AuthResponse
        try {
          const isApiAvailable = await authService.checkApiHealth()
          if (isApiAvailable) {
            response = await authService.login(credentials)
          } else {
            response = await authService.mockLogin(credentials)
          }
        } catch (apiError) {
          // Fallback to mock login for demo
          response = await authService.mockLogin(credentials)
        }

        this.setAuthData(response)
        return true
      } catch (error: any) {
        this.error = error.message || 'Login failed'
        return false
      } finally {
        this.isLoading = false
      }
    },

    /**
     * Sign up with user credentials
     * Đăng ký với thông tin người dùng
     */
    async signup(credentials: SignupCredentials): Promise<boolean> {
      this.isLoading = true
      this.error = null

      try {
        // Import auth service dynamically to avoid circular dependency
        const { authService } = await import('~/services/auth')

        // Call signup API
        const response = await authService.signup(credentials)

        // Auto login after successful signup
        this.setAuthData(response)
        return true
      } catch (error: any) {
        this.error = error.message || 'Signup failed'
        return false
      } finally {
        this.isLoading = false
      }
    },

    /**
     * Set authentication data and persist to storage
     * Đặt dữ liệu xác thực và lưu vào storage
     */
    setAuthData(authData: AuthResponse) {
      this.token = authData.token
      this.user = authData.user
      this.refreshToken = authData.refreshToken || null
      this.isAuthenticated = true

      if (process.client) {
        localStorage.setItem('auth.token', authData.token)
        localStorage.setItem('auth.user', JSON.stringify(authData.user))
        if (authData.refreshToken) {
          localStorage.setItem('auth.refreshToken', authData.refreshToken)
        }
      }
    },

    /**
     * Logout and clear authentication data
     * Đăng xuất và xóa dữ liệu xác thực
     */
    async logout() {
      this.isLoading = true

      try {
        // TODO: Call logout API if needed
        // TODO: Gọi API đăng xuất nếu cần
        /*
        await $fetch('/api/auth/logout', {
          method: 'POST',
          headers: {
            Authorization: `Bearer ${this.token}`,
          },
        })
        */

        this.clearAuth()
      } catch (error) {
        console.error('Logout error:', error)
        // Clear auth data even if API call fails
        // Xóa dữ liệu xác thực ngay cả khi gọi API thất bại
        this.clearAuth()
      } finally {
        this.isLoading = false
      }
    },

    /**
     * Clear all authentication data
     * Xóa tất cả dữ liệu xác thực
     */
    clearAuth() {
      this.user = null
      this.token = null
      this.refreshToken = null
      this.isAuthenticated = false
      this.error = null

      if (process.client) {
        localStorage.removeItem('auth.token')
        localStorage.removeItem('auth.user')
        localStorage.removeItem('auth.refreshToken')
      }
    },

    /**
     * Refresh authentication token
     * Làm mới token xác thực
     */
    async refreshAuthToken(): Promise<boolean> {
      if (!this.refreshToken) {
        return false
      }

      try {
        // TODO: Implement token refresh
        // TODO: Triển khai làm mới token
        /*
        const response = await $fetch<AuthResponse>('/api/auth/refresh', {
          method: 'POST',
          body: { refreshToken: this.refreshToken },
        })

        this.setAuthData(response)
        return true
        */

        return false
      } catch (error) {
        console.error('Token refresh failed:', error)
        this.clearAuth()
        return false
      }
    },

    /**
     * Update user profile
     * Cập nhật hồ sơ người dùng
     */
    updateUser(userData: Partial<User>) {
      if (this.user) {
        this.user = { ...this.user, ...userData }

        if (process.client) {
          localStorage.setItem('auth.user', JSON.stringify(this.user))
        }
      }
    },
  },
})
