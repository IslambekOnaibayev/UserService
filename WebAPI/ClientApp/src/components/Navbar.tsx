import { useNavigate } from 'react-router-dom'
import { clearToken, getCurrentUserEmail } from '@/store/auth'
import { Logo, ThemeToggle } from './ui'

export function Navbar() {
  const navigate = useNavigate()
  const email = getCurrentUserEmail()

  function logout() {
    clearToken()
    navigate('/login')
  }

  return (
    <nav className="bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-800 shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          <Logo />
          <div className="flex items-center gap-3">
            {email && (
              <span className="text-sm text-gray-500 dark:text-gray-400 hidden sm:block">
                {email}
              </span>
            )}
            <ThemeToggle />
            <button
              onClick={logout}
              className="flex items-center gap-2 px-3 py-2 rounded-lg text-sm font-medium
                         text-gray-500 dark:text-gray-400 hover:text-red-600 dark:hover:text-red-400
                         hover:bg-red-50 dark:hover:bg-red-950 transition-colors duration-200 ml-1"
            >
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                      d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7
                         a3 3 0 013-3h4a3 3 0 013 3v1" />
              </svg>
              Logout
            </button>
          </div>
        </div>
      </div>
    </nav>
  )
}
