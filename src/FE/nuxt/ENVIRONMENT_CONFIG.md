# Environment Configuration Guide

## Tổng quan
Authentication middleware đã được cập nhật để sử dụng environment-based configuration thay vì hard-coded endpoints. Điều này cho phép deploy linh hoạt across different environments.

## Environment Variables

### Required Variables
```bash
# SSO Service Base URL (Identity SSO service endpoint)
NUXT_PUBLIC_SSO_BASE=https://localhost:5001

# Application Base URL (for returnUrl construction)  
NUXT_PUBLIC_APP_BASE=http://localhost:3000

# API Base URL (for API calls)
NUXT_PUBLIC_API_BASE=https://localhost:7293
```

## Configuration Files

### Development (.env.local)
```bash
NUXT_PUBLIC_API_BASE=https://localhost:7293
NUXT_PUBLIC_SSO_BASE=https://localhost:5001
NUXT_PUBLIC_APP_BASE=http://localhost:3000
```

### Staging Example
```bash
NUXT_PUBLIC_API_BASE=https://api-staging.yourdomain.com
NUXT_PUBLIC_SSO_BASE=https://sso-staging.yourdomain.com
NUXT_PUBLIC_APP_BASE=https://app-staging.yourdomain.com
```

### Production Example
```bash
NUXT_PUBLIC_API_BASE=https://api.yourdomain.com
NUXT_PUBLIC_SSO_BASE=https://sso.yourdomain.com
NUXT_PUBLIC_APP_BASE=https://app.yourdomain.com
```

## How It Works

### Middleware Logic
```typescript
// Get configuration from runtime config
const { $config } = useNuxtApp()
const SSO_BASE_URL = $config.public.ssoBase
const APP_BASE_URL = $config.public.appBase

// Construct returnUrl dynamically
if (import.meta.client) {
  // Client-side: use actual window.location
  currentUrl = window.location.origin + to.fullPath
} else {
  // Server-side: use configured app base URL
  currentUrl = `${APP_BASE_URL}${to.fullPath}`
}

// Create SSO login URL with returnUrl
const loginUrl = `${SSO_BASE_URL}/auth/login?returnUrl=${encodeURIComponent(currentUrl)}`
```

### Benefits
- ✅ **Environment Flexibility**: Different endpoints per environment
- ✅ **Production Ready**: No hard-coded localhost URLs
- ✅ **Proper URL Construction**: Dynamic returnUrl based on actual request
- ✅ **Clean Configuration**: Centralized in .env files
- ✅ **Fallback Support**: Graceful fallbacks for development

## Setup Instructions

### 1. Copy Example Configuration
```bash
cp .env.example .env.local
```

### 2. Update Variables
Edit `.env.local` with your environment-specific URLs:
```bash
NUXT_PUBLIC_SSO_BASE=https://your-sso-service.com
NUXT_PUBLIC_APP_BASE=https://your-app.com
```

### 3. Verify Configuration
Check server logs for debug output:
```
🔍 Auth Middleware Debug: {
  ssoBase: 'https://your-sso-service.com',
  appBase: 'https://your-app.com',
  currentUrl: 'https://your-app.com/dashboard',
  loginUrl: 'https://your-sso-service.com/auth/login?returnUrl=...'
}
```

## Testing

### Local Testing
```bash
# Test middleware redirect
curl -v http://localhost:3000/dashboard

# Should return 302 redirect to:
# https://localhost:5001/auth/login?returnUrl=http%3A%2F%2Flocalhost%3A3000%2Fdashboard
```

### Production Testing  
```bash
# Test with production URLs
curl -v https://app.yourdomain.com/dashboard

# Should redirect to:
# https://sso.yourdomain.com/auth/login?returnUrl=https%3A%2F%2Fapp.yourdomain.com%2Fdashboard
```

## Security Notes

- ✅ `.env.local` excluded from git via `.gitignore`
- ✅ Use HTTPS URLs in production
- ✅ ReturnUrl properly URL-encoded
- ✅ Backend validates trusted return URLs

## Troubleshooting

### Common Issues
1. **Empty ReturnUrl**: Check `NUXT_PUBLIC_APP_BASE` configuration
2. **Wrong Redirect**: Verify `NUXT_PUBLIC_SSO_BASE` points to correct SSO service
3. **Server Errors**: Ensure all environment variables are set

### Debug Mode
Enable debug logs by checking browser console and server logs for:
```
🔍 Auth Middleware Debug: { ... }
```
