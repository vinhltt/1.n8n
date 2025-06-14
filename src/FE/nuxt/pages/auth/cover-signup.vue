<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'

// Set page layout
definePageMeta({
  layout: 'auth-layout'
})

// Reactive form data
const form = reactive({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  confirmPassword: '',
  agreeToTerms: false
})

// Form state
const loading = ref(false)
const error = ref('')
const success = ref('')

// Auth store
const authStore = useAuthStore()

// Password visibility toggles
const showPassword = ref(false)
const showConfirmPassword = ref(false)

// Form validation
const isValidEmail = computed(() => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(form.email)
})

const isValidPassword = computed(() => {
  return form.password.length >= 8
})

const passwordsMatch = computed(() => {
  return form.password === form.confirmPassword
})

const isFormValid = computed(() => {
  return form.firstName.trim() && 
         form.lastName.trim() && 
         isValidEmail.value && 
         isValidPassword.value && 
         passwordsMatch.value && 
         form.agreeToTerms
})

// Handle form submission
const handleSignup = async () => {
  if (!isFormValid.value) {
    error.value = 'Please fill in all required fields correctly.'
    return
  }

  loading.value = true
  error.value = ''

  try {
    // Register user
    const response = await $fetch('/api/auth/register', {
      method: 'POST',
      body: {
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
        password: form.password
      }
    })

    success.value = 'Account created successfully! Redirecting to login...'
    
    // Redirect to login page with success message after 2 seconds
    setTimeout(() => {
      navigateTo('/auth/cover-login?signup=success')
    }, 2000)

  } catch (err: any) {
    error.value = err.data?.message || 'Registration failed. Please try again.'
  } finally {
    loading.value = false
  }
}

// Clear error when user types
watch([() => form.email, () => form.password, () => form.confirmPassword], () => {
  error.value = ''
})
</script>

<template>
  <div class="flex min-h-screen">
    <!-- Left Side - Form -->
    <div class="flex flex-1 flex-col justify-center px-4 py-12 sm:px-6 lg:flex-none lg:px-20 xl:px-24">
      <div class="mx-auto w-full max-w-sm lg:w-96">
        <!-- Logo and Title -->
        <div class="text-center">
          <NuxtLink to="/" class="inline-block">
            <img src="/assets/images/logo.svg" alt="logo" class="mx-auto w-10" />
          </NuxtLink>
          <h2 class="mt-6 text-3xl font-bold tracking-tight text-black dark:text-white">
            Create your account
          </h2>
          <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">
            Already have an account?
            <NuxtLink 
              to="/auth/cover-login" 
              class="font-medium text-primary hover:underline"
            >
              Sign in here
            </NuxtLink>
          </p>
        </div>

        <!-- Success Message -->
        <div v-if="success" class="mt-6 rounded-md bg-green-50 p-4">
          <div class="flex">
            <icon-circle-check class="h-5 w-5 text-green-400" />
            <div class="ml-3">
              <p class="text-sm font-medium text-green-800">{{ success }}</p>
            </div>
          </div>
        </div>

        <!-- Error Message -->
        <div v-if="error" class="mt-6 rounded-md bg-red-50 p-4">
          <div class="flex">
            <icon-x-circle class="h-5 w-5 text-red-400" />
            <div class="ml-3">
              <p class="text-sm font-medium text-red-800">{{ error }}</p>
            </div>
          </div>
        </div>

        <!-- Signup Form -->
        <form @submit.prevent="handleSignup" class="mt-8 space-y-6">
          <!-- Name Fields -->
          <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <div>
              <label for="firstName" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                First Name *
              </label>
              <div class="mt-1">
                <input
                  id="firstName"
                  v-model="form.firstName"
                  type="text"
                  required
                  class="form-input"
                  placeholder="Enter your first name"
                />
              </div>
            </div>

            <div>
              <label for="lastName" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
                Last Name *
              </label>
              <div class="mt-1">
                <input
                  id="lastName"
                  v-model="form.lastName"
                  type="text"
                  required
                  class="form-input"
                  placeholder="Enter your last name"
                />
              </div>
            </div>
          </div>

          <!-- Email -->
          <div>
            <label for="email" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Email Address *
            </label>
            <div class="mt-1">
              <input
                id="email"
                v-model="form.email"
                type="email"
                required
                class="form-input"
                :class="{ 'border-red-500': form.email && !isValidEmail }"
                placeholder="Enter your email"
              />
            </div>
            <p v-if="form.email && !isValidEmail" class="mt-1 text-sm text-red-600">
              Please enter a valid email address
            </p>
          </div>

          <!-- Password -->
          <div>
            <label for="password" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Password *
            </label>
            <div class="mt-1 relative">
              <input
                id="password"
                v-model="form.password"
                :type="showPassword ? 'text' : 'password'"
                required
                class="form-input pr-10"
                :class="{ 'border-red-500': form.password && !isValidPassword }"
                placeholder="Enter your password"
              />
              <button
                type="button"
                @click="showPassword = !showPassword"
                class="absolute inset-y-0 right-0 flex items-center pr-3"
              >
                <icon-eye v-if="showPassword" class="h-5 w-5 text-gray-400" />
                <icon-eye v-else class="h-5 w-5 text-gray-400" />
              </button>
            </div>
            <p v-if="form.password && !isValidPassword" class="mt-1 text-sm text-red-600">
              Password must be at least 8 characters long
            </p>
          </div>

          <!-- Confirm Password -->
          <div>
            <label for="confirmPassword" class="block text-sm font-medium text-gray-700 dark:text-gray-300">
              Confirm Password *
            </label>
            <div class="mt-1 relative">
              <input
                id="confirmPassword"
                v-model="form.confirmPassword"
                :type="showConfirmPassword ? 'text' : 'password'"
                required
                class="form-input pr-10"
                :class="{ 'border-red-500': form.confirmPassword && !passwordsMatch }"
                placeholder="Confirm your password"
              />
              <button
                type="button"
                @click="showConfirmPassword = !showConfirmPassword"
                class="absolute inset-y-0 right-0 flex items-center pr-3"
              >
                <icon-eye v-if="showConfirmPassword" class="h-5 w-5 text-gray-400" />
                <icon-eye v-else class="h-5 w-5 text-gray-400" />
              </button>
            </div>
            <p v-if="form.confirmPassword && !passwordsMatch" class="mt-1 text-sm text-red-600">
              Passwords do not match
            </p>
          </div>

          <!-- Terms and Conditions -->
          <div class="flex items-center">
            <input
              id="agreeToTerms"
              v-model="form.agreeToTerms"
              type="checkbox"
              class="h-4 w-4 rounded border-gray-300 text-primary focus:ring-primary"
            />
            <label for="agreeToTerms" class="ml-2 block text-sm text-gray-700 dark:text-gray-300">
              I agree to the 
              <a href="#" class="text-primary hover:underline">Terms of Service</a> 
              and 
              <a href="#" class="text-primary hover:underline">Privacy Policy</a>
            </label>
          </div>

          <!-- Submit Button -->
          <div>
            <button
              type="submit"
              :disabled="!isFormValid || loading"
              class="btn btn-primary w-full"
              :class="{ 'opacity-50 cursor-not-allowed': !isFormValid || loading }"
            >
              <icon-loader v-if="loading" class="animate-spin mr-2" />
              {{ loading ? 'Creating Account...' : 'Create Account' }}
            </button>
          </div>
        </form>

        <!-- Social Login -->
        <div class="mt-6">
          <div class="relative">
            <div class="absolute inset-0 flex items-center">
              <div class="w-full border-t border-gray-300 dark:border-gray-600" />
            </div>
            <div class="relative flex justify-center text-sm">
              <span class="bg-white dark:bg-gray-900 px-2 text-gray-500">Or continue with</span>
            </div>
          </div>

          <div class="mt-6">
            <button
              type="button"
              class="btn btn-outline-dark w-full"
            >
              <icon-google class="mr-2" />
              Sign up with Google
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Right Side - Image/Branding -->
    <div class="relative hidden w-0 flex-1 lg:block">
      <div class="absolute inset-0 bg-gradient-to-br from-primary to-primary/80">
        <div class="flex h-full items-center justify-center p-12">
          <div class="text-center text-white">
            <h1 class="text-4xl font-bold mb-4">Join Our Community</h1>
            <p class="text-xl opacity-90 mb-8">
              Take control of your finances with our powerful tools and insights.
            </p>
            <div class="grid grid-cols-1 gap-4 text-left max-w-md">
              <div class="flex items-center">
                <icon-circle-check class="mr-3 h-5 w-5" />
                <span>Track expenses and income</span>
              </div>
              <div class="flex items-center">
                <icon-circle-check class="mr-3 h-5 w-5" />
                <span>Set and achieve financial goals</span>
              </div>
              <div class="flex items-center">
                <icon-circle-check class="mr-3 h-5 w-5" />
                <span>Get personalized insights</span>
              </div>
              <div class="flex items-center">
                <icon-circle-check class="mr-3 h-5 w-5" />
                <span>Secure and private</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.form-input {
  @apply block w-full appearance-none rounded-md border border-gray-300 px-3 py-2 placeholder-gray-400 shadow-sm focus:border-primary focus:outline-none focus:ring-primary dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-300;
}

.btn {
  @apply inline-flex items-center justify-center rounded-md border px-4 py-2 text-sm font-medium transition-colors focus:outline-none focus:ring-2 focus:ring-offset-2;
}

.btn-primary {
  @apply border-transparent bg-primary text-white hover:bg-primary/80 focus:ring-primary;
}

.btn-outline-dark {
  @apply border-gray-300 bg-white text-gray-700 hover:bg-gray-50 focus:ring-gray-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:hover:bg-gray-600;
}
</style>
