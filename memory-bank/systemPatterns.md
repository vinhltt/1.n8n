# System Architecture & Patterns

## Container Architecture
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Discord Bot   │    │       n8n       │    │   Excel API    │
│   (172.20.0.5)  │◄──►│   (172.20.0.4)  │◄──►│  (172.20.0.2)   │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │
                                ▼
                       ┌─────────────────┐
                       │   PostgreSQL    │
                       │   (172.20.0.3)  │
                       └─────────────────┘
```

## Docker Compose Pattern
- **Static IP assignment**: Mỗi service có IP cố định
- **Health checks**: PostgreSQL có health check
- **Dependencies**: n8n chờ PostgreSQL ready
- **Volumes**: Named volumes cho data persistence

## Backup Architecture
```
GitHub Actions (Ubuntu Runner)
           │
           ▼ SSH over Cloudflared
    TrueNAS Server
           │
           ▼ Docker Exec
    PostgreSQL Container ──► pg_dump ──► .sql.gz
           │
           ▼ Docker Run
    Volume Archive ──► tar ──► .tar.gz
```

## Network Pattern
- **Bridge network**: Custom subnet 172.20.0.0/24
- **Gateway**: 172.20.0.1
- **Static IPs**: Predictable container addressing
- **Port mapping**: External access to services

## Security Patterns
- **SSH Keys**: RSA 4096-bit for TrueNAS access
- **Cloudflared**: Zero-trust tunnel
- **Environment separation**: Branch-based deployment
- **Secret management**: GitHub secrets for credentials

## Data Patterns
- **PostgreSQL**: Primary data storage
- **Docker volumes**: File system persistence
- **Backup retention**: 7-day rotation policy
- **Logging**: Comprehensive backup logging