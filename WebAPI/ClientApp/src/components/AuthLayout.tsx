import { ThemeToggle, Logo } from './ui'

interface Props {
  title: string
  subtitle: string
  children: React.ReactNode
}

export function AuthLayout({ title, subtitle, children }: Props) {
  return (
    <div className="min-h-screen flex items-center justify-center px-4 bg-gray-50 dark:bg-gray-950">
      <ThemeToggle className="fixed top-4 right-4" />

      <div className="w-full max-w-sm animate-fade-in">
        <div className="bg-white dark:bg-gray-900 rounded-2xl shadow-xl
                        border border-gray-100 dark:border-gray-800 p-8">
          {/* Header */}
          <div className="text-center mb-8">
            <div className="flex justify-center mb-4">
              <Logo />
            </div>
            <h1 className="text-2xl font-bold text-gray-900 dark:text-white">{title}</h1>
            <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">{subtitle}</p>
          </div>

          {children}
        </div>
      </div>
    </div>
  )
}
