<template>
    <div>
        <div class="absolute inset-0">
            <img src="/assets/images/auth/bg-gradient.png" alt="image" class="h-full w-full object-cover" />
        </div>
        <div
            class="relative flex min-h-screen items-center justify-center bg-[url(/assets/images/auth/map.png)] bg-cover bg-center bg-no-repeat px-6 py-10 dark:bg-[#060818] sm:px-16"
        >
            <img src="/assets/images/auth/coming-soon-object1.png" alt="image" class="absolute left-0 top-1/2 h-full max-h-[893px] -translate-y-1/2" />
            <img src="/assets/images/auth/coming-soon-object2.png" alt="image" class="absolute left-24 top-0 h-40 md:left-[30%]" />
            <img src="/assets/images/auth/coming-soon-object3.png" alt="image" class="absolute right-0 top-0 h-[300px]" />
            <img src="/assets/images/auth/polygon-object.svg" alt="image" class="absolute bottom-0 end-[28%]" />
            <div
                class="relative flex w-full max-w-[1502px] flex-col justify-between overflow-hidden rounded-md bg-white/60 backdrop-blur-lg dark:bg-black/50 lg:min-h-[758px] lg:flex-row lg:gap-10 xl:gap-0"
            >
                <div
                    class="relative hidden w-full items-center justify-center bg-[linear-gradient(225deg,rgba(239,18,98,1)_0%,rgba(67,97,238,1)_100%)] p-5 lg:inline-flex lg:max-w-[835px] xl:-ms-28 ltr:xl:skew-x-[14deg] rtl:xl:skew-x-[-14deg]"
                >
                    <div
                        class="absolute inset-y-0 w-8 from-primary/10 via-transparent to-transparent ltr:-right-10 ltr:bg-gradient-to-r rtl:-left-10 rtl:bg-gradient-to-l xl:w-16 ltr:xl:-right-20 rtl:xl:-left-20"
                    ></div>
                    <div class="ltr:xl:-skew-x-[14deg] rtl:xl:skew-x-[14deg]">
                        <NuxtLink to="/" class="ms-10 block w-48 lg:w-72">
                            <img src="/assets/images/auth/logo-white.svg" alt="Logo" class="w-full" />
                        </NuxtLink>
                        <div class="mt-24 hidden w-full max-w-[430px] lg:block">
                            <img src="/assets/images/auth/register.svg" alt="Cover Image" class="w-full" />
                        </div>
                    </div>
                </div>
                <div class="relative flex w-full flex-col items-center justify-center gap-6 px-4 pb-16 pt-6 sm:px-6 lg:max-w-[667px]">
                    <div class="flex w-full max-w-[440px] items-center gap-2 lg:absolute lg:end-6 lg:top-6 lg:max-w-full">
                        <NuxtLink to="/" class="block w-8 lg:hidden">
                            <img src="/assets/images/logo.svg" alt="Logo" class="mx-auto w-10" />
                        </NuxtLink>
                        <div class="dropdown ms-auto w-max">
                            <client-only>
                                <Popper :placement="store.rtlClass === 'rtl' ? 'bottom-start' : 'bottom-end'" offsetDistance="8">
                                    <button
                                        type="button"
                                        class="flex items-center gap-2.5 rounded-lg border border-white-dark/30 bg-white px-2 py-1.5 text-white-dark hover:border-primary hover:text-primary dark:bg-black"
                                    >
                                        <div>
                                            <img :src="currentFlag" alt="image" class="h-5 w-5 rounded-full object-cover" />
                                        </div>
                                        <div class="text-base font-bold uppercase">{{ store.locale }}</div>
                                        <span class="shrink-0">
                                            <icon-caret-down />
                                        </span>
                                    </button>
                                    <template #content="{ close }">
                                        <ul class="grid w-[280px] grid-cols-2 gap-2 !p-2 font-semibold text-dark dark:text-white-dark dark:text-white-light/90">
                                            <li>
                                                <button
                                                    type="button"
                                                    class="flex w-full items-center gap-2 rounded-lg p-2 hover:bg-primary/10 ltr:text-left rtl:text-right"
                                                    @click="store.changeLocale('en'), close()"
                                                >
                                                    <img src="/assets/images/flags/US.svg" alt="US Flag" class="h-5 w-5 rounded-full object-cover" />
                                                    <span>English</span>
                                                </button>
                                            </li>
                                            <li>
                                                <button
                                                    type="button"
                                                    class="flex w-full items-center gap-2 rounded-lg p-2 hover:bg-primary/10 ltr:text-left rtl:text-right"
                                                    @click="store.changeLocale('da'), close()"
                                                >
                                                    <img src="/assets/images/flags/DK.svg" alt="Denmark Flag" class="h-5 w-5 rounded-full object-cover" />
                                                    <span>Danish</span>
                                                </button>
                                            </li>
                                        </ul>
                                    </template>
                                </Popper>
                            </client-only>
                        </div>
                    </div>
                    <div class="w-full max-w-[440px] lg:mt-16">
                        <div class="mb-10">
                            <h1 class="text-3xl font-extrabold uppercase !leading-snug text-primary md:text-4xl">Sign up</h1>
                            <p class="text-base font-bold leading-normal text-white-dark">Enter your details to create your account</p>
                        </div>
                        <form class="space-y-5 dark:text-white" @submit.prevent="handleSignup">
                            <!-- First Name & Last Name -->
                            <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
                                <div>
                                    <label for="firstName">First Name</label>
                                    <div class="relative text-white-dark">
                                        <input
                                            id="firstName"
                                            v-model="signupForm.firstName"
                                            type="text"
                                            placeholder="Enter First Name"
                                            class="form-input ps-10 placeholder:text-white-dark"
                                            :class="{ 'border-red-500': errors.firstName }"
                                            :disabled="isLoading"
                                        />
                                        <span class="absolute start-4 top-1/2 -translate-y-1/2">
                                            <icon-user :fill="true" />
                                        </span>
                                    </div>
                                    <p v-if="errors.firstName" class="mt-1 text-sm text-red-600">{{ errors.firstName }}</p>
                                </div>
                                <div>
                                    <label for="lastName">Last Name</label>
                                    <div class="relative text-white-dark">
                                        <input
                                            id="lastName"
                                            v-model="signupForm.lastName"
                                            type="text"
                                            placeholder="Enter Last Name"
                                            class="form-input ps-10 placeholder:text-white-dark"
                                            :class="{ 'border-red-500': errors.lastName }"
                                            :disabled="isLoading"
                                        />
                                        <span class="absolute start-4 top-1/2 -translate-y-1/2">
                                            <icon-user :fill="true" />
                                        </span>
                                    </div>
                                    <p v-if="errors.lastName" class="mt-1 text-sm text-red-600">{{ errors.lastName }}</p>
                                </div>
                            </div>

                            <!-- Username -->
                            <div>
                                <label for="username">Username</label>
                                <div class="relative text-white-dark">
                                    <input
                                        id="username"
                                        v-model="signupForm.username"
                                        type="text"
                                        placeholder="Enter Username"
                                        class="form-input ps-10 placeholder:text-white-dark"
                                        :class="{ 'border-red-500': errors.username }"
                                        :disabled="isLoading"
                                    />
                                    <span class="absolute start-4 top-1/2 -translate-y-1/2">
                                        <icon-user :fill="true" />
                                    </span>
                                </div>
                                <p v-if="errors.username" class="mt-1 text-sm text-red-600">{{ errors.username }}</p>
                            </div>

                            <!-- Email -->
                            <div>
                                <label for="email">Email</label>
                                <div class="relative text-white-dark">
                                    <input
                                        id="email"
                                        v-model="signupForm.email"
                                        type="email"
                                        placeholder="Enter Email"
                                        class="form-input ps-10 placeholder:text-white-dark"
                                        :class="{ 'border-red-500': errors.email }"
                                        :disabled="isLoading"
                                    />
                                    <span class="absolute start-4 top-1/2 -translate-y-1/2">
                                        <icon-mail :fill="true" />
                                    </span>
                                </div>
                                <p v-if="errors.email" class="mt-1 text-sm text-red-600">{{ errors.email }}</p>
                            </div>

                            <!-- Phone Number (Optional) -->
                            <div>
                                <label for="phoneNumber">Phone Number (Optional)</label>
                                <div class="relative text-white-dark">
                                    <input
                                        id="phoneNumber"
                                        v-model="signupForm.phoneNumber"
                                        type="tel"
                                        placeholder="Enter Phone Number"
                                        class="form-input ps-10 placeholder:text-white-dark"
                                        :class="{ 'border-red-500': errors.phoneNumber }"
                                        :disabled="isLoading"
                                    />
                                    <span class="absolute start-4 top-1/2 -translate-y-1/2">
                                        <icon-phone :fill="true" />
                                    </span>
                                </div>
                                <p v-if="errors.phoneNumber" class="mt-1 text-sm text-red-600">{{ errors.phoneNumber }}</p>
                            </div>

                            <!-- Password -->
                            <div>
                                <label for="password">Password</label>
                                <div class="relative text-white-dark">
                                    <input
                                        id="password"
                                        v-model="signupForm.password"
                                        type="password"
                                        placeholder="Enter Password"
                                        class="form-input ps-10 placeholder:text-white-dark"
                                        :class="{ 'border-red-500': errors.password }"
                                        :disabled="isLoading"
                                    />
                                    <span class="absolute start-4 top-1/2 -translate-y-1/2">
                                        <icon-lock-dots :fill="true" />
                                    </span>
                                </div>
                                <p v-if="errors.password" class="mt-1 text-sm text-red-600">{{ errors.password }}</p>
                            </div>

                            <!-- Confirm Password -->
                            <div>
                                <label for="confirmPassword">Confirm Password</label>
                                <div class="relative text-white-dark">
                                    <input
                                        id="confirmPassword"
                                        v-model="signupForm.confirmPassword"
                                        type="password"
                                        placeholder="Confirm Password"
                                        class="form-input ps-10 placeholder:text-white-dark"
                                        :class="{ 'border-red-500': errors.confirmPassword }"
                                        :disabled="isLoading"
                                    />
                                    <span class="absolute start-4 top-1/2 -translate-y-1/2">
                                        <icon-lock-dots :fill="true" />
                                    </span>
                                </div>
                                <p v-if="errors.confirmPassword" class="mt-1 text-sm text-red-600">{{ errors.confirmPassword }}</p>
                            </div>

                            <!-- Terms and Conditions -->
                            <div>
                                <label class="flex cursor-pointer items-center">
                                    <input
                                        v-model="agreeToTerms"
                                        type="checkbox"
                                        class="form-checkbox bg-white dark:bg-black"
                                        :class="{ 'border-red-500': errors.terms }"
                                    />
                                    <span class="text-white-dark">I agree to the Terms and Conditions</span>
                                </label>
                                <p v-if="errors.terms" class="mt-1 text-sm text-red-600">{{ errors.terms }}</p>
                            </div>

                            <!-- Error Display -->
                            <div v-if="errors.general" class="p-3 text-sm text-red-600 bg-red-50 border border-red-200 rounded-md dark:bg-red-900/20 dark:border-red-800">
                                {{ errors.general }}
                            </div>

                            <!-- Submit Button -->
                            <button
                                type="submit"
                                class="btn btn-gradient !mt-6 w-full border-0 uppercase shadow-[0_10px_20px_-10px_rgba(67,97,238,0.44)]"
                                :disabled="isLoading"
                            >
                                <span v-if="isLoading" class="animate-spin inline-block w-4 h-4 border-2 border-white border-t-transparent rounded-full mr-2"></span>
                                {{ isLoading ? 'Creating Account...' : 'Create Account' }}
                            </button>
                        </form>

                        <div class="relative my-7 text-center md:mb-9">
                            <span class="absolute inset-x-0 top-1/2 h-px w-full -translate-y-1/2 bg-white-light dark:bg-white-dark"></span>
                            <span class="relative bg-white px-2 font-bold uppercase text-white-dark dark:bg-dark dark:text-white-light">or</span>
                        </div>
                        <div class="mb-10 md:mb-[60px]">
                            <ul class="flex justify-center gap-3.5">
                                <li>
                                    <NuxtLink
                                        to="javascript:;"
                                        class="inline-flex h-8 w-8 items-center justify-center rounded-full p-0 transition hover:scale-110"
                                        style="background: linear-gradient(135deg, rgba(239, 18, 98, 1) 0%, rgba(67, 97, 238, 1) 100%)"
                                    >
                                        <icon-instagram />
                                    </NuxtLink>
                                </li>
                                <li>
                                    <NuxtLink
                                        to="javascript:;"
                                        class="inline-flex h-8 w-8 items-center justify-center rounded-full p-0 transition hover:scale-110"
                                        style="background: linear-gradient(135deg, rgba(239, 18, 98, 1) 0%, rgba(67, 97, 238, 1) 100%)"
                                    >
                                        <icon-facebook />
                                    </NuxtLink>
                                </li>
                                <li>
                                    <NuxtLink
                                        to="javascript:;"
                                        class="inline-flex h-8 w-8 items-center justify-center rounded-full p-0 transition hover:scale-110"
                                        style="background: linear-gradient(135deg, rgba(239, 18, 98, 1) 0%, rgba(67, 97, 238, 1) 100%)"
                                    >
                                        <icon-twitter />
                                    </NuxtLink>
                                </li>
                                <li>
                                    <NuxtLink
                                        to="javascript:;"
                                        class="inline-flex h-8 w-8 items-center justify-center rounded-full p-0 transition hover:scale-110"
                                        style="background: linear-gradient(135deg, rgba(239, 18, 98, 1) 0%, rgba(67, 97, 238, 1) 100%)"
                                    >
                                        <icon-google />
                                    </NuxtLink>
                                </li>
                            </ul>
                        </div>
                        <p class="text-center text-white-dark dark:text-white">
                            Already have an account?
                            <NuxtLink to="/auth/cover-login" class="uppercase text-primary underline transition hover:text-black dark:hover:text-white"
                                >SIGN IN</NuxtLink
                            >
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { useAppStore } from '@/stores/index'
import { useAuth } from '~/composables/useAuth'

// Define page meta for layout and middleware
definePageMeta({
    layout: 'auth-layout',
    middleware: 'guest',
})

// App store for theme and locale management
const store = useAppStore()

// Auth composable
const { isLoading, errors, signupForm, agreeToTerms, handleSignup } = useAuth()

// Computed for flag display
const currentFlag = computed(() => {
    return store.locale === 'en' ? '/assets/images/flags/US.svg' : '/assets/images/flags/DK.svg'
})

// Meta tags
useHead({
    title: 'Sign Up - Create Account',
    meta: [
        { name: 'description', content: 'Create your account to get started with our platform.' }
    ]
})
</script>

<style scoped>
.form-input {
    @apply block w-full rounded-md border border-white-light bg-white px-3 py-2 text-sm placeholder-gray-400 shadow-sm transition-colors duration-200 focus:border-primary focus:outline-none focus:ring-1 focus:ring-primary dark:border-[#17263C] dark:bg-[#121E32] dark:text-white;
}

.form-checkbox {
    @apply h-4 w-4 rounded border-gray-300 text-primary focus:ring-primary;
}
</style>
