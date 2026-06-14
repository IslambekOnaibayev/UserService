# UserService

User management app: registration with email confirmation, JWT auth, block/unblock/delete users.

**Demo:** [youtu.be/zUF8r4NvgKs](https://youtu.be/zUF8r4NvgKs)

**Stack:** ASP.NET Core 10 · FastEndpoints · EF Core · PostgreSQL · React + Vite · Railway

---

## Local Development

```bash
# 1. Apply migrations
dotnet ef database update --project Infrastructure --startup-project WebAPI

# 2. Run backend (https://localhost:7057)
dotnet run --project WebAPI

# 3. Run frontend (http://localhost:3000)
cd WebAPI/ClientApp && npm install && npm run dev
```

> In development, emails are not sent — confirmation link is printed to the console.

---

## Production (Railway)

**Required environment variables:**

| Variable | Description |
|---|---|
| `DATABASE_URL` | `${{Postgres.DATABASE_URL}}` |
| `JwtSettings__SecretKey` | Random 32+ char secret (`openssl rand -hex 32`) |
| `Gmail__ClientId` | Google OAuth2 client ID |
| `Gmail__ClientSecret` | Google OAuth2 client secret |
| `Gmail__RefreshToken` | OAuth2 refresh token |
| `Mailserver__FromEmail` | Sender email (your Gmail) |
| `Mailserver__FromName` | Sender display name |
| `Mailserver__BaseUrl` | Public app URL |
| `PORT` | `8080` |

Email sender priority: **Gmail API → Resend → SMTP**

---

## Project Structure

```
UserService/
├── Core/            # Domain entities, interfaces
├── UseCases/        # Commands and queries
├── Infrastructure/  # EF Core, email senders, repositories
└── WebAPI/          # API endpoints, JWT, React SPA
    └── ClientApp/   # React + Vite frontend
```
