import { useState } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { authApi } from '@/api/client'
import { setToken } from '@/store/auth'
import { useAsync } from '@/hooks'
import { AuthLayout } from '@/components/AuthLayout'
import { Alert, Spinner } from '@/components/ui'

const EMAIL_MAX = 320

function validateLogin(email: string, password: string): string | null {
  if (!email.trim()) return 'Email is required'
  if (!email.includes('@') || !email.includes('.')) return 'Enter a valid email address'
  if (email.length > EMAIL_MAX) return `Email cannot exceed ${EMAIL_MAX} characters`
  if (!password) return 'Password is required'
  return null
}

export default function LoginPage() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const confirmed = searchParams.get('confirmed')
  const { loading, error, run } = useAsync<void>()
  const [email, setEmail]           = useState('')
  const [password, setPassword]     = useState('')
  const [clientError, setClientError] = useState<string | null>(null)

  async function handleSubmit(e: { preventDefault(): void }) {
    e.preventDefault()
    const msg = validateLogin(email, password)
    if (msg) { setClientError(msg); return }
    setClientError(null)
    await run(async () => {
      const res = await authApi.login(email, password)
      setToken(res.token)
      navigate('/users')
    })
  }

  function serverError(err: string | null) {
    if (!err) return null
    const lower = err.toLowerCase()
    if (lower.includes('unauthorized') || lower.includes('401'))
      return 'Invalid email or password'
    if (lower.includes('forbidden') || lower.includes('403') || lower.includes('blocked'))
      return 'Your account is blocked'
    return err
  }

  const displayError = clientError ?? serverError(error)

  return (
    <AuthLayout title="Sign in" subtitle="to UserService admin panel">
      {confirmed === 'true' && (
        <div className="mb-4">
          <Alert kind="success">Email confirmed! You can now sign in.</Alert>
        </div>
      )}
      {confirmed === 'false' && (
        <div className="mb-4">
          <Alert kind="error">Invalid or expired confirmation link.</Alert>
        </div>
      )}
      {displayError && (
        <div className="mb-4">
          <Alert kind="error">{displayError}</Alert>
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-5" noValidate>
        <div>
          <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">
            Email
          </label>
          <input
            type="email"
            value={email}
            onChange={e => { setEmail(e.target.value); setClientError(null) }}
            placeholder="you@example.com"
            maxLength={EMAIL_MAX}
            required
            className="input"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">
            Password
          </label>
          <input
            type="password"
            value={password}
            onChange={e => { setPassword(e.target.value); setClientError(null) }}
            placeholder="••••••••"
            required
            className="input"
          />
        </div>

        <button type="submit" disabled={loading} className="btn-primary">
          {loading ? (
            <span className="flex items-center justify-center gap-2">
              <Spinner size={4} /> Signing in…
            </span>
          ) : (
            'Sign in'
          )}
        </button>
      </form>

      <p className="mt-6 text-center text-sm text-gray-500 dark:text-gray-400">
        Don't have an account?{' '}
        <Link to="/register" className="font-medium text-indigo-600 dark:text-indigo-400 hover:underline">
          Register
        </Link>
      </p>
    </AuthLayout>
  )
}
