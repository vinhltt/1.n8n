# Technology Context

## Core Technologies
- **Docker & Docker Compose**: Container orchestration
- **PostgreSQL 15**: Database storage
- **n8n**: Workflow automation platform
- **Node.js**: Runtime for Discord bot
- **ASP.NET Core**: Excel API service
- **Bash**: Backup scripting

## Infrastructure
- **TrueNAS**: Production server
- **Cloudflared**: Secure tunnel service
- **GitHub Actions**: CI/CD pipeline
- **Ubuntu**: Runner environment
- **SSH**: Secure remote access

## Development Tools
- **Git**: Version control
- **Docker**: Development environment
- **VSCode**: IDE configuration present

## Environment Variables
Từ `.env.template`:
```
# Database
POSTGRES_DB, POSTGRES_USER, POSTGRES_PASSWORD

# n8n
N8N_ENCRYPTION_KEY, N8N_BASIC_AUTH_PASSWORD
N8N_BACKUP_DIR_HOST, WEBHOOK_URL

# Discord
DISCORD_TOKEN, DISCORD_N8N_WEBHOOK_URL

# Network
IP_PREFIX=172.20.0

# Backup
N8N_RETENTION_DAYS=7
```

## GitHub Secrets Required
- TRUENAS_SSH_PRIVATE_KEY
- TRUENAS_SSH_HOSTNAME_THROUGH_CLOUDFLARED
- TRUENAS_USER
- POSTGRES_* variables
- N8N_* variables
- DISCORD_* variables
- DISCORD_WEBHOOK_URL

## Deployment Paths
- DEPLOY_PATH_ON_TRUENAS variable
- Branch-specific directories: deploy_${branch_name}