# UserService

A full-stack user management application built with **ASP.NET Core 10** and **React**. Supports user registration with email confirmation, JWT authentication, and admin operations (block, unblock, delete users).

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 10, FastEndpoints, EF Core 10 |
| Database | PostgreSQL |
| Auth | JWT Bearer |
| Email | Resend (production) / console log (development) |
| Frontend | React 18, TypeScript, Vite, TanStack Query |
| Deployment | Railway |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org)
- [PostgreSQL](https://www.postgresql.org/download/) running locally

---

## Local Development

### 1. Clone the repository

```bash
git clone <repository-url>
cd UserService
```

### 2. Configure the database

Make sure PostgreSQL is running locally. The default connection string in `appsettings.Development.json` expects:

```
Host=localhost;Port=5432;Database=userservice_dev;Username=postgres;Password=postgres
```

Edit `WebAPI/appsettings.Development.json` if your local PostgreSQL credentials differ.

### 3. Apply database migrations

```bash
dotnet ef database update --project Infrastructure --startup-project WebAPI
```

### 4. Run the backend

```bash
dotnet run --project WebAPI
```

The API will be available at `https://localhost:7057`.  
Swagger UI: `https://localhost:7057/swagger`

> **Email in development:** confirmation emails are not actually sent — the link is printed to the console log instead. Look for a line starting with `Not actually sending confirmation email`.

### 5. Run the frontend

```bash
cd WebAPI/ClientApp
npm install
npm run dev
```

The app will be available at `http://localhost:3000`. API requests are proxied to `https://localhost:7057`.

---

## Production Deployment (Railway)

### 1. Create a Railway project

1. Sign in at [railway.app](https://railway.app)
2. **New Project** → **Deploy from GitHub repo** → select this repository
3. Add a **PostgreSQL** plugin to the project

### 2. Set environment variables

In Railway → your service → **Variables**, add the following:

```
DATABASE_URL="${{Postgres.DATABASE_URL}}"
JwtSettings__SecretKey="<random 32+ character secret>"
Resend__ApiKey="<your Resend API key>"
Mailserver__FromEmail="onboarding@resend.dev"
Mailserver__FromName="UserService"
Mailserver__BaseUrl="https://<your-railway-domain>"
PORT="8080"
```

> Generate a JWT secret with: `openssl rand -hex 32`

### 3. Get a Resend API key

1. Sign up at [resend.com](https://resend.com)
2. Go to **API Keys** → **Create API Key**
3. Copy the key (starts with `re_`) and paste it into `Resend__ApiKey`

> **Note:** With the free `onboarding@resend.dev` sender, emails are delivered only to the Resend account owner's address. To send to any user, verify your own domain in Resend → **Domains**.

### 4. Deploy

Railway automatically builds and deploys on every push to `main`. The Dockerfile at the repository root handles the full build (frontend + backend in one image).

---

## Environment Variables Reference

| Variable | Description | Required |
|---|---|---|
| `DATABASE_URL` | PostgreSQL connection URI (set by Railway plugin) | Production |
| `JwtSettings__SecretKey` | Secret key for signing JWT tokens (32+ chars) | Production |
| `Resend__ApiKey` | Resend API key for sending emails | Production |
| `Mailserver__FromEmail` | Sender email address | Production |
| `Mailserver__FromName` | Sender display name | Production |
| `Mailserver__BaseUrl` | Public URL of the app (used in email links) | Production |
| `PORT` | Port Kestrel listens on (Railway sets this automatically) | Production |

---

## Project Structure

```
UserService/
├── Core/               # Domain entities, interfaces, events
├── UseCases/           # Application logic (commands, queries)
├── Infrastructure/     # EF Core, email senders, repositories
├── WebAPI/             # FastEndpoints, JWT auth, React SPA
│   └── ClientApp/      # React + Vite frontend
├── Dockerfile
└── docker-compose.yml
```

---

## Running with Docker Compose (local)

```bash
cp .env.example .env
# Edit .env and fill in your values
docker compose up --build
```

The app will be available at `http://localhost`.
