const THEME_KEY = 'theme'

export function isDark(): boolean {
  return document.documentElement.classList.contains('dark')
}

export function toggleTheme(): void {
  const dark = isDark()
  document.documentElement.classList.toggle('dark', !dark)
  localStorage.setItem(THEME_KEY, dark ? 'light' : 'dark')
  window.dispatchEvent(new Event('theme-changed'))
}
