// Centralized API client — all requests go through here.
// Token is read from localStorage on every call (no stale closures).

const BASE = import.meta.env.VITE_API_URL ?? ''

function getToken(): string | null {
  return localStorage.getItem('auth_token')
}

function clearToken(): void {
  localStorage.removeItem('auth_token')
  window.dispatchEvent(new Event('auth-changed'))
}

async function request<T>(
  method: string,
  path: string,
  body?: unknown,
): Promise<T> {
  const token = getToken()
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  }
  if (token) headers['Authorization'] = `Bearer ${token}`

  const res = await fetch(`${BASE}${path}`, {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  })

  if (res.status === 204 || res.status === 201 && method === 'DELETE') {
    return undefined as T
  }

  const data = await res.json().catch(() => ({}))

  if (!res.ok) {
    if (res.status === 401) {
      clearToken()
      window.location.replace('/login')
    }
    const err = Object.assign(new Error(data?.title ?? 'Request failed'), {
      status: res.status,
      data,
    })
    throw err
  }

  return data as T
}

// ─── Auth ────────────────────────────────────────────────────────────────────

export interface LoginResponse {
  token: string
  userId: string
}

export interface RegisterResponse {
  userId: string
  message: string
}

export const authApi = {
  login: (email: string, password: string) =>
    request<LoginResponse>('POST', '/api/auth/login', { email, password }),

  register: (name: string, email: string, password: string) =>
    request<RegisterResponse>('POST', '/api/auth/register', { name, email, password }),
}

// ─── Users ───────────────────────────────────────────────────────────────────

export type UserStatus = 'Unverified' | 'Active' | 'Blocked'

export interface User {
  id: string
  name: string
  email: string
  registrationTime: string
  lastLoginTime: string | null
  lastActivityTime: string | null
  status: UserStatus
}

export interface UsersResponse {
  users: User[]
}

export const usersApi = {
  list: () => request<UsersResponse>('GET', '/api/users'),
  block: (userIds: string[]) => request<void>('POST', '/api/users/block', { userIds }),
  unblock: (userIds: string[]) => request<void>('POST', '/api/users/unblock', { userIds }),
  delete: (userIds: string[]) => request<void>('DELETE', '/api/users', { userIds }),
  deleteUnverified: () => request<void>('DELETE', '/api/users/unverified'),
}
