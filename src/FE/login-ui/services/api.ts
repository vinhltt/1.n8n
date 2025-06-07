/**
 * API configuration and service
 * Cấu hình và dịch vụ API
 */

// API Configuration
export const API_CONFIG = {
  BASE_URL: process.env.NODE_ENV === 'production'
    ? 'https://your-api-domain.com'
    : 'http://localhost:5227', // Backend Identity API URL
  ENDPOINTS: {
    LOGIN: '/api/auth/login',
    LOGOUT: '/api/auth/logout',
    REFRESH: '/api/auth/refresh',
    USER_PROFILE: '/api/users/profile',
  },
  TIMEOUT: 10000, // 10 seconds
}

/**
 * API service for making HTTP requests
 * Dịch vụ API để thực hiện các yêu cầu HTTP
 */
export class ApiService {
  private baseURL: string

  constructor(baseURL: string = API_CONFIG.BASE_URL) {
    this.baseURL = baseURL
  }

  /**
   * Make API request with authentication header
   * Thực hiện yêu cầu API với header xác thực
   */
  async request<T>(
    endpoint: string,
    options: RequestInit & {
      token?: string,
      params?: Record<string, string>
    } = {}
  ): Promise<T> {
    const { token, params, ...fetchOptions } = options

    // Build URL with query parameters
    const url = new URL(endpoint, this.baseURL)
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        url.searchParams.append(key, value)
      })
    }

    // Default headers
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
      ...((fetchOptions.headers as Record<string, string>) || {})
    }

    // Add authorization header if token is provided
    if (token) {
      headers['Authorization'] = `Bearer ${token}`
    }

    const config: RequestInit = {
      ...fetchOptions,
      headers,
    }

    try {
      const response = await fetch(url.toString(), config)

      if (!response.ok) {
        const errorText = await response.text()
        throw new Error(`HTTP ${response.status}: ${errorText}`)
      }

      // Handle empty responses
      const contentType = response.headers.get('content-type')
      if (!contentType || !contentType.includes('application/json')) {
        return null as T
      }

      return await response.json() as T
    } catch (error) {
      if (error instanceof Error) {
        throw error
      }
      throw new Error('Network error occurred')
    }
  }

  /**
   * GET request
   */
  async get<T>(endpoint: string, options?: { token?: string, params?: Record<string, string> }): Promise<T> {
    return this.request<T>(endpoint, { ...options, method: 'GET' })
  }

  /**
   * POST request
   */
  async post<T>(endpoint: string, data?: any, options?: { token?: string }): Promise<T> {
    return this.request<T>(endpoint, {
      ...options,
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    })
  }

  /**
   * PUT request
   */
  async put<T>(endpoint: string, data?: any, options?: { token?: string }): Promise<T> {
    return this.request<T>(endpoint, {
      ...options,
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    })
  }

  /**
   * DELETE request
   */
  async delete<T>(endpoint: string, options?: { token?: string }): Promise<T> {
    return this.request<T>(endpoint, { ...options, method: 'DELETE' })
  }
}

// Default API service instance
export const apiService = new ApiService()
