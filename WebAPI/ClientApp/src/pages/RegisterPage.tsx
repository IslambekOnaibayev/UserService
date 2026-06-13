import { useState } from 'react'
import { Link } from 'react-router-dom'
import { authApi } from '@/api/client'
import { useAsync } from '@/hooks'
import { AuthLayout } from '@/components/AuthLayout'
import { Alert, Spinner } from '@/components/ui'

const NAME_MIN = 2
const NAME_MAX = 100
const EMAIL_MAX = 320

function validateRegister(name: string, email: string, password: string): string | null {
  const n = name.trim()
  if (!n) return 'Full name is required'
  if (n.length < NAME_MIN) return `Full name must be at least ${NAME_MIN} characters`
  if (n.length > NAME_MAX) return `Full name cannot exceed ${NAME_MAX} characters`
  if (!email.trim()) return 'Email is required'
  if (!email.includes('@') || !email.includes('.')) return 'Enter a valid email address'
  if (email.length > EMAIL_MAX) return `Email cannot exceed ${EMAIL_MAX} characters`
  if (!password) return 'Password is required'
  return null
}

export default function RegisterPage() {
  const { loading, error, run } = useAsync<void>()
  const [name, setName]             = useState('')
  const [email, setEmail]           = useState('')
  const [password, setPassword]     = useState('')
  const [done, setDone]             = useState(false)
  const [clientError, setClientError] = useState<string | null>(null)

  async function handleSubmit(e: { preventDefault(): void }) {
    e.preventDefault()
    const msg = validateRegister(name, email, password)
    if (msg) { setClientError(msg); return }
    setClientError(null)
    await run(async () => {
      await authApi.register(name, email, password)
      setDone(true)
    })
  }

  function serverError(err: string | null) {
    if (!err) return null
    if (err.includes('409') || err.toLowerCase().includes('conflict'))
      return 'This email is already registered'
    return err
  }

  const displayError = clientError ?? serverError(error)

  if (done) {
    return (
      <AuthLayout title="All done!" subtitle="One more step">
        <Alert kind="success">
          <p className="font-medium">Registration successful!</p>
          <p className="mt-1 text-sm">Check your email to confirm your account.</p>
        </Alert>
        <Link
          to="/login"
          className="btn-primary mt-5 flex items-center justify-center"
        >
          Go to sign in
        </Link>
      </AuthLayout>
    )
  }

  return (
    <AuthLayout title="Create account" subtitle="Join UserService today">
      {displayError && (
        <div className="mb-4">
          <Alert kind="error">{displayError}</Alert>
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-5" noValidate>
        <div>
          <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">
            Full name
          </label>
          <input
            type="text"
            value={name}
            onChange={e => { setName(e.target.value); setClientError(null) }}
            placeholder="John Doe"
            minLength={NAME_MIN}
            maxLength={NAME_MAX}
            required
            className="input"
          />
        </div>

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
            <span className="ml-1 font-normal text-gray-400">(any length)</span>
          </label>
          <input
            type="password"
            value={password}
            onChange={e => { setPassword(e.target.value); setClientError(null) }}
            placeholder="anything works"
            required
            className="input"
          />
        </div>

        <button type="submit" disabled={loading} className="btn-primary">
          {loading ? (
            <span className="flex items-center justify-center gap-2">
              <Spinner size={4} /> Creating account…
            </span>
          ) : (
            'Create account'
          )}
        </button>
      </form>

      <p className="mt-6 text-center text-sm text-gray-500 dark:text-gray-400">
        Already have an account?{' '}
        <Link to="/login" className="font-medium text-indigo-600 dark:text-indigo-400 hover:underline">
          Sign in
        </Link>
      </p>
    </AuthLayout>
  )
}
