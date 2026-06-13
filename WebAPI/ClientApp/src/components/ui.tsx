import { toggleTheme } from '@/store/theme'
import { useTheme } from '@/hooks'

export function Spinner({ size = 4 }: { size?: number }) {
  return (
    <svg
      className={`animate-spin w-${size} h-${size}`}
      fill="none"
      viewBox="0 0 24 24"
    >
      <circle
        className="opacity-25"
        cx="12" cy="12" r="10"
        stroke="currentColor" strokeWidth="4"
      />
      <path
        className="opacity-75"
        fill="currentColor"
        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
      />
    </svg>
  )
}

type AlertKind = 'error' | 'success' | 'info'

const alertStyles: Record<AlertKind, string> = {
  error:   'bg-red-50 dark:bg-red-950 border-red-200 dark:border-red-800 text-red-700 dark:text-red-300',
  success: 'bg-green-50 dark:bg-green-950 border-green-200 dark:border-green-800 text-green-700 dark:text-green-300',
  info:    'bg-blue-50 dark:bg-blue-950 border-blue-200 dark:border-blue-800 text-blue-700 dark:text-blue-300',
}

export function Alert({ kind = 'error', children }: { kind?: AlertKind; children: React.ReactNode }) {
  return (
    <div className={`p-3 rounded-lg border text-sm flex items-start gap-2 animate-fade-in ${alertStyles[kind]}`}>
      {kind === 'error' && (
        <svg className="w-4 h-4 shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
      )}
      {kind === 'success' && (
        <svg className="w-4 h-4 shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
        </svg>
      )}
      <span>{children}</span>
    </div>
  )
}

export function ThemeToggle({ className = '' }: { className?: string }) {
  const dark = useTheme()

  return (
    <button
      onClick={toggleTheme}
      title={dark ? 'Switch to light mode' : 'Switch to dark mode'}
      className={`p-2 rounded-lg text-gray-400 hover:text-gray-600 dark:hover:text-gray-200
                  hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors duration-200 ${className}`}
    >
      {dark ? (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364-6.364l-.707.707
                   M6.343 17.657l-.707.707M17.657 17.657l-.707-.707
                   M6.343 6.343l-.707-.707M12 5a7 7 0 000 14 7 7 0 000-14z" />
        </svg>
      ) : (
        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
        </svg>
      )}
    </button>
  )
}

export function Logo() {
  return (
    <div className="flex items-center gap-2.5">
      <div className="w-8 h-8 rounded-lg bg-indigo-600 flex items-center justify-center shadow-md shadow-indigo-500/30">
        <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197" />
        </svg>
      </div>
      <span className="font-bold text-gray-900 dark:text-white tracking-tight">UserService</span>
    </div>
  )
}
