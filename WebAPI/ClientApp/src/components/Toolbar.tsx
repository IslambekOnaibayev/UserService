interface Props {
  selectedCount: number
  loading: boolean
  onBlock: () => void
  onUnblock: () => void
  onDelete: () => void
  onDeleteUnverified: () => void
  onRefresh: () => void
  filter: string
  onFilterChange: (v: string) => void
}

// Toolbar never disappears — buttons become disabled when nothing is selected.
export function Toolbar({
  selectedCount, loading,
  onBlock, onUnblock, onDelete, onDeleteUnverified, onRefresh,
  filter, onFilterChange,
}: Props) {
  const hasSelection = selectedCount > 0
  const busy = loading

  return (
    <div className="flex flex-wrap items-center justify-between gap-3 px-4 py-3
                    border-b border-gray-200 dark:border-gray-800
                    bg-gray-50 dark:bg-gray-900/50">

      {/* Action buttons */}
      <div className="flex items-center gap-2">

        {/* Block — text button per spec */}
        <ToolbarBtn
          onClick={onBlock}
          disabled={!hasSelection || busy}
          variant="amber"
          title="Block selected users"
        >
          <BlockIcon />
          Block
        </ToolbarBtn>

        {/* Unblock — icon only per spec */}
        <ToolbarIconBtn onClick={onUnblock} disabled={!hasSelection || busy} variant="green" title="Unblock selected">
          <UnlockIcon />
        </ToolbarIconBtn>

        <div className="w-px h-6 bg-gray-200 dark:bg-gray-700" />

        {/* Delete selected — icon only */}
        <ToolbarIconBtn onClick={onDelete} disabled={!hasSelection || busy} variant="red" title="Delete selected">
          <TrashIcon />
        </ToolbarIconBtn>

        {/* Delete unverified — icon only */}
        <ToolbarIconBtn onClick={onDeleteUnverified} disabled={busy} variant="orange" title="Delete all unverified users">
          <WarnIcon />
        </ToolbarIconBtn>

        {/* Refresh */}
        <ToolbarIconBtn onClick={onRefresh} disabled={busy} variant="gray" title="Refresh">
          <RefreshIcon spinning={busy} />
        </ToolbarIconBtn>
      </div>

      {/* Filter */}
      <div className="relative">
        <svg className="absolute left-2.5 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400 pointer-events-none"
             fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
                d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
        </svg>
        <input
          type="text"
          value={filter}
          onChange={e => onFilterChange(e.target.value)}
          placeholder="Filter…"
          className="pl-8 pr-3 py-1.5 rounded-lg border text-sm w-48
                     bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700
                     text-gray-900 dark:text-white placeholder-gray-400 dark:placeholder-gray-500
                     focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
        />
      </div>
    </div>
  )
}

// ─── Small button helpers ─────────────────────────────────────────────────────

type Variant = 'amber' | 'green' | 'red' | 'orange' | 'gray'

const enabledVariant: Record<Variant, string> = {
  amber:  'border-amber-300 bg-amber-50 text-amber-700 hover:bg-amber-100 dark:border-amber-700 dark:bg-amber-950 dark:text-amber-300 dark:hover:bg-amber-900',
  green:  'border-green-300 bg-green-50 text-green-700 hover:bg-green-100 dark:border-green-700 dark:bg-green-950 dark:text-green-300 dark:hover:bg-green-900',
  red:    'border-red-300 bg-red-50 text-red-600 hover:bg-red-100 dark:border-red-700 dark:bg-red-950 dark:text-red-400 dark:hover:bg-red-900',
  orange: 'border-orange-300 bg-orange-50 text-orange-600 hover:bg-orange-100 dark:border-orange-700 dark:bg-orange-950 dark:text-orange-400 dark:hover:bg-orange-900',
  gray:   'border-gray-200 bg-white text-gray-500 hover:bg-gray-100 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-gray-700',
}

const disabledClass =
  'border-gray-200 bg-gray-50 text-gray-400 cursor-not-allowed dark:border-gray-700 dark:bg-gray-800 dark:text-gray-600'

function ToolbarBtn({ onClick, disabled, variant, title, children }: {
  onClick: () => void; disabled: boolean; variant: Variant; title: string; children: React.ReactNode
}) {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      title={title}
      className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm font-medium border
                  transition-all duration-200 ${disabled ? disabledClass : enabledVariant[variant]}`}
    >
      {children}
    </button>
  )
}

function ToolbarIconBtn({ onClick, disabled, variant, title, children }: {
  onClick: () => void; disabled: boolean; variant: Variant; title: string; children: React.ReactNode
}) {
  return (
    <button
      onClick={onClick}
      disabled={disabled}
      title={title}
      className={`p-2 rounded-lg border transition-all duration-200
                  ${disabled ? disabledClass : enabledVariant[variant]}`}
    >
      {children}
    </button>
  )
}

// ─── Icons ────────────────────────────────────────────────────────────────────

function BlockIcon() {
  return (
    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
            d="M18.364 18.364A9 9 0 005.636 5.636m12.728 12.728A9 9 0 015.636 5.636m12.728 12.728L5.636 5.636" />
    </svg>
  )
}

function UnlockIcon() {
  return (
    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
            d="M8 11V7a4 4 0 118 0m-4 8v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2z" />
    </svg>
  )
}

function TrashIcon() {
  return (
    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
            d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
    </svg>
  )
}

function WarnIcon() {
  return (
    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
            d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
    </svg>
  )
}

function RefreshIcon({ spinning }: { spinning: boolean }) {
  return (
    <svg className={`w-4 h-4 ${spinning ? 'animate-spin' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2}
            d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
    </svg>
  )
}
