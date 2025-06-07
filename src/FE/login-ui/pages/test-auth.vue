<template>
  <div class="container mx-auto px-4 py-8 max-w-4xl">
    <h1 class="text-3xl font-bold mb-8 text-center">Authentication Testing Dashboard</h1>

    <!-- Status Display -->
    <div class="mb-8 p-4 bg-gray-100 rounded-lg">
      <h2 class="text-xl font-semibold mb-2">Connection Status</h2>
      <div class="flex items-center space-x-4">
        <div class="flex items-center">
          <div
            :class="[
              'w-3 h-3 rounded-full mr-2',
              apiStatus.backend ? 'bg-green-500' : 'bg-red-500'
            ]"
          ></div>
          <span>Backend API ({{baseUrl}})</span>
        </div>
        <button
          @click="checkApiStatus"
          class="px-3 py-1 bg-blue-500 text-white rounded text-sm hover:bg-blue-600"
          :disabled="loading.status"
        >
          {{loading.status ? 'Checking...' : 'Check Status'}}
        </button>
      </div>
    </div>

    <!-- Registration Test -->
    <div class="mb-8 p-6 border rounded-lg">
      <h2 class="text-2xl font-semibold mb-4">Registration Test</h2>
      <form @submit.prevent="testRegistration" class="space-y-4">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label for="reg-email" class="block text-sm font-medium mb-1">Email</label>
            <input
              id="reg-email"
              v-model="registrationForm.email"
              type="email"
              required
              class="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="test@example.com"
            >
          </div>
          <div>
            <label for="reg-username" class="block text-sm font-medium mb-1">Username</label>
            <input
              id="reg-username"
              v-model="registrationForm.username"
              type="text"
              required
              class="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="testuser"
            >
          </div>
          <div>
            <label for="reg-fullname" class="block text-sm font-medium mb-1">Full Name</label>
            <input
              id="reg-fullname"
              v-model="registrationForm.fullName"
              type="text"
              required
              class="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Test User"
            >
          </div>
          <div>
            <label for="reg-firstname" class="block text-sm font-medium mb-1">First Name</label>
            <input
              id="reg-firstname"
              v-model="registrationForm.firstName"
              type="text"
              required
              class="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Test"
            >
          </div>
          <div>
            <label for="reg-lastname" class="block text-sm font-medium mb-1">Last Name</label>
            <input
              id="reg-lastname"
              v-model="registrationForm.lastName"
              type="text"
              required
              class="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="User"
            >
          </div>
          <div>
            <label for="reg-phone" class="block text-sm font-medium mb-1">Phone Number</label>
            <input
              id="reg-phone"
              v-model="registrationForm.phoneNumber"
              type="tel"
              class="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="+1234567890"
            >
          </div>
          <div class="md:col-span-2">
            <label for="reg-password" class="block text-sm font-medium mb-1">Password</label>
            <input
              id="reg-password"
              v-model="registrationForm.password"
              type="password"
              required
              class="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Password123!"
            >
          </div>
        </div>
        <div class="flex space-x-4">
          <button
            type="submit"
            :disabled="loading.registration"
            class="px-6 py-2 bg-green-600 text-white rounded hover:bg-green-700 disabled:opacity-50"
          >
            {{loading.registration ? 'Registering...' : 'Test Registration'}}
          </button>
          <button
            type="button"
            @click="fillSampleData"
            class="px-6 py-2 bg-gray-600 text-white rounded hover:bg-gray-700"
          >
            Fill Sample Data
          </button>
        </div>
      </form>
    </div>

    <!-- Login Test -->
    <div class="mb-8 p-6 border rounded-lg">
      <h2 class="text-2xl font-semibold mb-4">Login Test</h2>
      <form @submit.prevent="testLogin" class="space-y-4">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label for="login-email" class="block text-sm font-medium mb-1">Email/Username</label>
            <input
              id="login-email"
              v-model="loginForm.email"
              type="text"
              required
              class="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="test@example.com or testuser"
            >
          </div>
          <div>
            <label for="login-password" class="block text-sm font-medium mb-1">Password</label>
            <input
              id="login-password"
              v-model="loginForm.password"
              type="password"
              required
              class="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
              placeholder="Password123!"
            >
          </div>
        </div>
        <div class="flex space-x-4">
          <button
            type="submit"
            :disabled="loading.login"
            class="px-6 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
          >
            {{loading.login ? 'Logging in...' : 'Test Login'}}
          </button>
          <button
            type="button"
            @click="copyFromRegistration"
            class="px-6 py-2 bg-gray-600 text-white rounded hover:bg-gray-700"
          >
            Copy from Registration
          </button>
        </div>
      </form>
    </div>

    <!-- Results Display -->
    <div v-if="results.length > 0" class="space-y-4">
      <h2 class="text-2xl font-semibold">Test Results</h2>
      <div
        v-for="(result, index) in results"
        :key="index"
        :class="[
          'p-4 rounded-lg',
          result.success ? 'bg-green-50 border-green-200' : 'bg-red-50 border-red-200'
        ]"
        class="border"
      >
        <div class="flex justify-between items-start mb-2">
          <h3 :class="[
            'font-semibold',
            result.success ? 'text-green-800' : 'text-red-800'
          ]">
            {{result.operation}} - {{result.success ? 'SUCCESS' : 'FAILED'}}
          </h3>
          <span class="text-sm text-gray-500">{{result.timestamp}}</span>
        </div>
        <div class="space-y-2">
          <div v-if="result.url" class="text-sm">
            <strong>URL:</strong> <code class="bg-gray-100 px-1 rounded">{{result.url}}</code>
          </div>
          <div v-if="result.statusCode" class="text-sm">
            <strong>Status:</strong> {{result.statusCode}}
          </div>
          <div v-if="result.message" class="text-sm">
            <strong>Message:</strong> {{result.message}}
          </div>
          <div v-if="result.data" class="text-sm">
            <strong>Response Data:</strong>
            <pre class="bg-gray-100 p-2 rounded mt-1 text-xs overflow-auto">{{JSON.stringify(result.data, null, 2)}}</pre>
          </div>
          <div v-if="result.error" class="text-sm">
            <strong>Error:</strong> {{result.error}}
          </div>
        </div>
      </div>
    </div>

    <!-- Clear Results -->
    <div v-if="results.length > 0" class="mt-6 text-center">
      <button
        @click="clearResults"
        class="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
      >
        Clear Results
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Base URL from your API config
const baseUrl = 'http://localhost:5227'

// Reactive data
const apiStatus = ref({
  backend: false
})

const loading = ref({
  status: false,
  registration: false,
  login: false
})

const registrationForm = ref({
  email: '',
  username: '',
  fullName: '',
  firstName: '',
  lastName: '',
  phoneNumber: '',
  password: ''
})

const loginForm = ref({
  email: '',
  password: ''
})

const results = ref<Array<{
  operation: string
  success: boolean
  timestamp: string
  url?: string
  statusCode?: number
  message?: string
  data?: any
  error?: string
}>>([])

// Methods
const addResult = (result: any) => {
  results.value.unshift({
    ...result,
    timestamp: new Date().toLocaleTimeString()
  })
}

const checkApiStatus = async () => {
  loading.value.status = true
  try {
    const response = await fetch(`${baseUrl}/api/auth/health`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    })

    apiStatus.value.backend = response.ok

    addResult({
      operation: 'API Health Check',
      success: response.ok,
      url: `${baseUrl}/api/auth/health`,
      statusCode: response.status,
      message: response.ok ? 'API is healthy' : 'API is not responding'
    })
  } catch (error) {
    apiStatus.value.backend = false
    addResult({
      operation: 'API Health Check',
      success: false,
      url: `${baseUrl}/api/auth/health`,
      error: error instanceof Error ? error.message : 'Unknown error'
    })
  } finally {
    loading.value.status = false
  }
}

const testRegistration = async () => {
  loading.value.registration = true
  try {
    const response = await fetch(`${baseUrl}/api/auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        email: registrationForm.value.email,
        username: registrationForm.value.username,
        fullName: registrationForm.value.fullName,
        firstName: registrationForm.value.firstName,
        lastName: registrationForm.value.lastName,
        phoneNumber: registrationForm.value.phoneNumber,
        password: registrationForm.value.password
      })
    })

    const data = await response.json()

    addResult({
      operation: 'User Registration',
      success: response.ok,
      url: `${baseUrl}/api/auth/register`,
      statusCode: response.status,
      message: response.ok ? 'User registered successfully' : 'Registration failed',
      data: data
    })

    // If registration successful, copy credentials to login form
    if (response.ok) {
      loginForm.value.email = registrationForm.value.email
      loginForm.value.password = registrationForm.value.password
    }
  } catch (error) {
    addResult({
      operation: 'User Registration',
      success: false,
      url: `${baseUrl}/api/auth/register`,
      error: error instanceof Error ? error.message : 'Unknown error'
    })
  } finally {
    loading.value.registration = false
  }
}

const testLogin = async () => {
  loading.value.login = true
  try {
    const response = await fetch(`${baseUrl}/api/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({
        usernameOrEmail: loginForm.value.email,
        password: loginForm.value.password
      })
    })

    const data = await response.json()

    addResult({
      operation: 'User Login',
      success: response.ok,
      url: `${baseUrl}/api/auth/login`,
      statusCode: response.status,
      message: response.ok ? 'Login successful' : 'Login failed',
      data: data
    })
  } catch (error) {
    addResult({
      operation: 'User Login',
      success: false,
      url: `${baseUrl}/api/auth/login`,
      error: error instanceof Error ? error.message : 'Unknown error'
    })
  } finally {
    loading.value.login = false
  }
}

const fillSampleData = () => {
  const timestamp = Date.now()
  registrationForm.value = {
    email: `test${timestamp}@example.com`,
    username: `testuser${timestamp}`,
    fullName: 'Test User',
    firstName: 'Test',
    lastName: 'User',
    phoneNumber: '+1234567890',
    password: 'Password123!'
  }
}

const copyFromRegistration = () => {
  loginForm.value.email = registrationForm.value.email
  loginForm.value.password = registrationForm.value.password
}

const clearResults = () => {
  results.value = []
}

// Check API status on mount
onMounted(() => {
  checkApiStatus()
})
</script>

<style scoped>
/* Custom styles if needed */
pre {
  white-space: pre-wrap;
  word-break: break-word;
}
</style>
