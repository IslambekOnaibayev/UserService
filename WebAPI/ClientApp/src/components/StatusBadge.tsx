import type { UserStatus } from '@/api/client'

const styles: Record<UserStatus, { badge: string; dot: string }> = {
  Active:     { badge: 'bg-green-100 dark:bg-green-950 text-green-700 dark:text-green-300',   dot: 'bg-green-500' },
  Unverified: { badge: 'bg-yellow-100 dark:bg-yellow-950 text-yellow-700 dark:text-yellow-300', dot: 'bg-yellow-500' },
  Blocked:    { badge: 'bg-red-100 dark:bg-red-950 text-red-700 dark:text-red-300',             dot: 'bg-red-500' },
}

export function StatusBadge({ status }: { status: UserStatus }) {
  const { badge, dot } = styles[status]
  return (
    <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${badge}`}>
      <span className={`w-1.5 h-1.5 rounded-full mr-1.5 ${dot}`} />
      {status}
    </span>
  )
}
