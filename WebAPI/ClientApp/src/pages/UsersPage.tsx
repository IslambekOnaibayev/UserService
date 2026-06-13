import { useState, useEffect, useMemo } from 'react'
import { usersApi, type User } from '@/api/client'
import { Navbar } from '@/components/Navbar'
import { Toolbar } from '@/components/Toolbar'
import { StatusBadge } from '@/components/StatusBadge'
import { Alert } from '@/components/ui'

// Format date relative to now — matches the ТЗ "last seen" column
function relativeTime(dateStr: string | null): string {
  if (!dateStr) return 'Never'
  const diff = Date.now() - new Date(dateStr).getTime()
  const mins = Math.floor(diff / 60_000)
  if (mins < 1) return 'Just now'
  if (mins < 60) return `${mins}m ago`
  const hrs = Math.floor(mins / 60)
  if (hrs < 24) return `${hrs}h ago`
  const days = Math.floor(hrs / 24)
  if (days < 7) return `${days}d ago`
  return new Date(dateStr).toLocaleDateString()
}

type Toast = { msg: string; kind: 'success' | 'error' }

export default function UsersPage() {
  const [users, setUsers]         = useState<User[]>([])
  const [loading, setLoading]     = useState(false)
  const [selected, setSelected]   = useState<Set<string>>(new Set())
  const [filter, setFilter]       = useState('')
  const [toast, setToast]         = useState<Toast | null>(null)

  // Auto-dismiss toast after 4 s
  useEffect(() => {
    if (!toast) return
    const t = setTimeout(() => setToast(null), 4000)
    return () => clearTimeout(t)
  }, [toast])

  function showToast(msg: string, kind: Toast['kind'] = 'success') {
    setToast({ msg, kind })
  }

  async function load() {
    setLoading(true)
    try {
      const res = await usersApi.list()
      setUsers(res.users)
      setSelected(new Set()) // clear selection after refresh
    } catch {
      showToast('Failed to load users', 'error')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => { load() }, [])

  // Filter by name or email
  const filtered = useMemo(() => {
    const q = filter.toLowerCase()
    return q ? users.filter(u =>
      u.name.toLowerCase().includes(q) || u.email.toLowerCase().includes(q)
    ) : users
  }, [users, filter])

  // ─── Selection ──────────────────────────────────────────────────────────────

  const allChecked  = filtered.length > 0 && filtered.every(u => selected.has(u.id))
  const someChecked = filtered.some(u => selected.has(u.id)) && !allChecked

  function toggleAll() {
    if (allChecked) {
      setSelected(new Set())
    } else {
      setSelected(new Set(filtered.map(u => u.id)))
    }
  }

  function toggleOne(id: string) {
    setSelected(prev => {
      const next = new Set(prev)
      next.has(id) ? next.delete(id) : next.add(id)
      return next
    })
  }

  // ─── Toolbar actions ────────────────────────────────────────────────────────

  async function withRefresh(fn: () => Promise<void>, msg: string) {
    setLoading(true)
    try {
      await fn()
      showToast(msg)
      await load()
    } catch {
      showToast(`Failed: ${msg.toLowerCase()}`, 'error')
      setLoading(false)
    }
  }

  const ids = [...selected]

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-950 flex flex-col">
      <Navbar />

      <main className="flex-1 max-w-7xl mx-auto w-full px-4 sm:px-6 lg:px-8 py-8">

        {/* Page header */}
        <div className="mb-6 animate-fade-in">
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white">User Management</h1>
          <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
            {users.length} total · {selected.size} selected
          </p>
        </div>

        {/* Toast */}
        {toast && (
          <div className="mb-4 animate-slide-down">
            <Alert kind={toast.kind}>{toast.msg}</Alert>
          </div>
        )}

        {/* Card */}
        <div className="bg-white dark:bg-gray-900 rounded-2xl border border-gray-200 dark:border-gray-800
                        shadow-sm overflow-hidden animate-fade-in">

          <Toolbar
            selectedCount={selected.size}
            loading={loading}
            filter={filter}
            onFilterChange={setFilter}
            onBlock={() => withRefresh(() => usersApi.block(ids), 'Users blocked')}
            onUnblock={() => withRefresh(() => usersApi.unblock(ids), 'Users unblocked')}
            onDelete={() => withRefresh(() => usersApi.delete(ids), 'Users deleted')}
            onDeleteUnverified={() => withRefresh(() => usersApi.deleteUnverified(), 'Unverified users deleted')}
            onRefresh={load}
          />

          {/* Table */}
          <div className="overflow-x-auto">
            <table className="min-w-full">
              <thead>
                <tr className="border-b border-gray-200 dark:border-gray-800 bg-gray-50 dark:bg-gray-900/30">
                  {/* Select-all — checkbox only, no label (per ТЗ) */}
                  <th className="w-12 px-4 py-3">
                    <input
                      type="checkbox"
                      checked={allChecked}
                      ref={el => { if (el) el.indeterminate = someChecked }}
                      onChange={toggleAll}
                      className="w-4 h-4 rounded border-gray-300 dark:border-gray-600
                                 text-indigo-600 focus:ring-indigo-500 cursor-pointer"
                    />
                  </th>
                  <Th>Name</Th>
                  <Th>Email</Th>
                  <Th>Status</Th>
                  <Th>Last seen</Th>
                </tr>
              </thead>

              <tbody className="divide-y divide-gray-100 dark:divide-gray-800">
                {loading && users.length === 0 ? (
                  // Skeleton rows while first load
                  Array.from({ length: 5 }).map((_, i) => (
                    <tr key={i}>
                      <td className="px-4 py-4">
                        <div className="w-4 h-4 bg-gray-200 dark:bg-gray-700 rounded animate-pulse" />
                      </td>
                      {[32, 48, 16, 28].map((w, j) => (
                        <td key={j} className="px-4 py-4">
                          <div className={`h-4 w-${w} bg-gray-200 dark:bg-gray-700 rounded animate-pulse`} />
                        </td>
                      ))}
                    </tr>
                  ))
                ) : filtered.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="px-4 py-16 text-center">
                      <svg className="mx-auto w-12 h-12 text-gray-300 dark:text-gray-700 mb-3"
                           fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5}
                              d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857
                                 M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857
                                 m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z" />
                      </svg>
                      <p className="text-sm text-gray-400 dark:text-gray-600">No users found</p>
                    </td>
                  </tr>
                ) : (
                  filtered.map(user => {
                    const isSelected = selected.has(user.id)
                    return (
                      <tr
                        key={user.id}
                        className={`transition-colors duration-150 cursor-pointer
                          ${isSelected
                            ? 'bg-indigo-50 dark:bg-indigo-950/40'
                            : 'hover:bg-gray-50 dark:hover:bg-gray-800/50'}`}
                        onClick={() => toggleOne(user.id)}
                      >
                        <td className="px-4 py-3.5" onClick={e => e.stopPropagation()}>
                          <input
                            type="checkbox"
                            checked={isSelected}
                            onChange={() => toggleOne(user.id)}
                            className="w-4 h-4 rounded border-gray-300 dark:border-gray-600
                                       text-indigo-600 focus:ring-indigo-500 cursor-pointer"
                          />
                        </td>

                        {/* Name — struck-through if blocked */}
                        <td className="px-4 py-3.5">
                          <span className={`text-sm font-medium ${
                            user.status === 'Blocked'
                              ? 'line-through text-gray-400 dark:text-gray-600'
                              : 'text-gray-900 dark:text-white'
                          }`}>
                            {user.name}
                          </span>
                        </td>

                        <td className="px-4 py-3.5">
                          <span className="text-sm text-gray-600 dark:text-gray-400">{user.email}</span>
                        </td>

                        <td className="px-4 py-3.5">
                          <StatusBadge status={user.status} />
                        </td>

                        <td className="px-4 py-3.5">
                          <span className="text-sm text-gray-500 dark:text-gray-400">
                            {relativeTime(user.lastLoginTime ?? user.lastActivityTime)}
                          </span>
                        </td>
                      </tr>
                    )
                  })
                )}
              </tbody>
            </table>
          </div>

          {/* Footer */}
          <div className="px-4 py-3 border-t border-gray-100 dark:border-gray-800
                          bg-gray-50 dark:bg-gray-900/30 flex items-center justify-between
                          text-xs text-gray-400 dark:text-gray-600">
            <span>Sorted by last login time ↓</span>
            <span>{filtered.length} of {users.length} users</span>
          </div>
        </div>
      </main>
    </div>
  )
}

function Th({ children }: { children: React.ReactNode }) {
  return (
    <th className="px-4 py-3 text-left text-xs font-semibold
                   text-gray-500 dark:text-gray-400 uppercase tracking-wider">
      {children}
    </th>
  )
}
