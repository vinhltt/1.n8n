import { apiService, API_CONFIG } from './api'
import type { LoginCredentials, AuthResponse, User } from '~/stores/auth'

/**
 * Authentication service for API calls
 * Dịch vụ xác thực cho các lời gọi API
 */
export class AuthService {
  /**
   * Login with credentials - tries real API first, falls back to mock
   * Đăng nhập với thông tin đăng nhập - thử API thật trước, fallback sang mock
   */
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    console.log('🔐 Starting login process for:', credentials.email)

    try {
      // Check if backend API is available
      console.log('🔍 Checking API health...')
      const isApiAvailable = await this.checkApiHealth()
      console.log('🔍 API health check result:', isApiAvailable)

      if (isApiAvailable) {
        // Try real API login
        console.log('🌐 Attempting real API login...')
        const loginRequest = {
          UsernameOrEmail: credentials.email,
          Password: credentials.password
        }

        console.log('📤 Sending login request to:', API_CONFIG.ENDPOINTS.LOGIN)
        const response = await apiService.post<AuthResponse>(
          API_CONFIG.ENDPOINTS.LOGIN,
          loginRequest
        )
        console.log('✅ Real API login successful:', response)
        return response
      } else {
        // Fallback to mock login
        console.log('🎭 Backend API not available, using mock login')
        return await this.mockLogin(credentials)
      }
    } catch (error) {
      console.error('❌ Login API error, falling back to mock:', error)
      // Fallback to mock if API fails
      return await this.mockLogin(credentials)
    }
  }

  /**
   * Logout user
   * Đăng xuất người dùng
   */
  async logout(token: string): Promise<void> {
    try {
      await apiService.post(API_CONFIG.ENDPOINTS.LOGOUT, null, { token })
    } catch (error) {
      console.error('Logout API error:', error)
      // Don't throw error for logout, just log it
    }
  }

  /**
   * Refresh authentication token
   * Làm mới token xác thực
   */
  async refreshToken(refreshToken: string): Promise<AuthResponse> {
    try {
      const response = await apiService.post<AuthResponse>(
        API_CONFIG.ENDPOINTS.REFRESH,
        { refreshToken }
      )
      return response
    } catch (error) {
      console.error('Token refresh API error:', error)
      throw new Error('Token refresh failed')
    }
  }

  /**
   * Get user profile
   * Lấy hồ sơ người dùng
   */
  async getUserProfile(token: string): Promise<User> {
    try {
      const response = await apiService.get<User>(
        API_CONFIG.ENDPOINTS.USER_PROFILE,
        { token }
      )
      return response
    } catch (error) {
      console.error('Get user profile API error:', error)
      throw new Error('Failed to get user profile')
    }
  }

  /**
   * Sign up with user credentials
   * Đăng ký với thông tin người dùng
   */
  async signup(credentials: any): Promise<AuthResponse> {
    console.log('📝 Starting signup process for:', credentials.email)

    try {
      // Check if backend API is available
      console.log('🔍 Checking API health...')
      const isApiAvailable = await this.checkApiHealth()
      console.log('🔍 API health check result:', isApiAvailable)

      if (isApiAvailable) {
        // Try real API signup
        console.log('🌐 Attempting real API signup...')
        const signupRequest = {
          email: credentials.email,
          username: credentials.username,
          fullName: credentials.fullName,
          firstName: credentials.firstName,
          lastName: credentials.lastName,
          phoneNumber: credentials.phoneNumber || '',
          password: credentials.password
        }

        console.log('📤 Sending signup request to:', '/api/auth/register')
        const response = await apiService.post<any>(
          '/api/auth/register',
          signupRequest
        )
        console.log('✅ Real API signup successful:', response)

        // Auto login after successful signup
        return await this.login({
          email: credentials.email,
          password: credentials.password
        })
      } else {
        // Fallback to mock signup
        console.log('🎭 Backend API not available, using mock signup')
        return await this.mockSignup(credentials)
      }
    } catch (error) {
      console.error('❌ Signup API error, falling back to mock:', error)
      // Fallback to mock if API fails
      return await this.mockSignup(credentials)
    }
  }

  /**
   * Mock login for development/demo
   * Đăng nhập giả lập cho phát triển/demo
   */
  async mockLogin(credentials: LoginCredentials): Promise<AuthResponse> {
    console.log('🎭 Running mock login for:', credentials.email)

    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 1000))

    // Check demo credentials - both mock and real test user
    if ((credentials.email === 'admin@example.com' && credentials.password === 'password') ||
        (credentials.email === 'test@example.com' && credentials.password === 'Test123!')) {

      const mockResponse = {
        token: 'mock-jwt-token-' + Date.now(),
        user: {
          id: credentials.email === 'test@example.com' ? 'test-user-id' : '1',
          email: credentials.email,
          username: credentials.email === 'test@example.com' ? 'testuser' : 'admin',
          fullName: credentials.email === 'test@example.com' ? 'Test User' : 'Administrator',
          roles: ['User'],
          isActive: true,
        },
      }

      console.log('🎭 Mock login successful:', mockResponse)
      return mockResponse
    }

    console.log('🎭 Mock login failed - invalid credentials')
    throw new Error('Invalid credentials')
  }

  /**
   * Mock signup for development/demo
   * Đăng ký giả lập cho phát triển/demo
   */
  async mockSignup(credentials: any): Promise<AuthResponse> {
    console.log('🎭 Running mock signup for:', credentials.email)

    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 1000))

    // Check if email is already "registered" in mock
    if (credentials.email === 'admin@example.com') {
      throw new Error('Email already exists')
    }

    const mockResponse = {
      token: 'mock-jwt-token-' + Date.now(),
      user: {
        id: 'mock-user-' + Date.now(),
        email: credentials.email,
        username: credentials.username,
        fullName: credentials.fullName,
        roles: ['User'],
        isActive: true,
      },
    }

    console.log('🎭 Mock signup successful:', mockResponse)
    return mockResponse
  }

  /**
   * Check if backend API is available
   * Kiểm tra xem backend API có sẵn không
   */
  async checkApiHealth(): Promise<boolean> {
    try {
      console.log('🏥 Testing API connectivity to:', API_CONFIG.BASE_URL)
      // Try to reach the login endpoint with a test request
      const testResponse = await fetch(`${API_CONFIG.BASE_URL}/api/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          UsernameOrEmail: 'health-check-test',
          Password: 'health-check-test'
        })
      })

      console.log('🏥 API connectivity test response status:', testResponse.status)

      // API is available if we get any response (even 400/401 is fine, means API is running)
      return testResponse.status < 500
    } catch (error) {
      console.warn('🏥 Backend API not available:', error)
      return false
    }
  }
}

// Default auth service instance
export const authService = new AuthService()
