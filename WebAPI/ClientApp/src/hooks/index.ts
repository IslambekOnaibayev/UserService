import { useState, useEffect, useCallback } from 'react'
import { isLoggedIn } from '@/store/auth'
import { isDark } from '@/store/theme'

// Re-renders whenever auth-changed fires
export function useAuth() {
  const [loggedIn, setLoggedIn] = useState(isLoggedIn)

  useEffect(() => {
    const handler = () => setLoggedIn(isLoggedIn())
    window.addEventListener('auth-changed', handler)
    return () => window.removeEventListener('auth-changed', handler)
  }, [])

  return loggedIn
}

// Re-renders whenever theme-changed fires
export function useTheme() {
  const [dark, setDark] = useState(isDark)

  useEffect(() => {
    const handler = () => setDark(isDark())
    window.addEventListener('theme-changed', handler)
    return () => window.removeEventListener('theme-changed', handler)
  }, [])

  return dark
}

// Generic async action with loading + error state
export function useAsync<T>() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const run = useCallback(async (fn: () => Promise<T>): Promise<T | null> => {
    setLoading(true)
    setError(null)
    try {
      const result = await fn()
      return result
    } catch (e: unknown) {
      const msg =
        (e as { data?: { title?: string }; message?: string })?.data?.title ??
        (e as { message?: string })?.message ??
        'Something went wrong'
      setError(msg)
      return null
    } finally {
      setLoading(false)
    }
  }, [])

  return { loading, error, run }
}
