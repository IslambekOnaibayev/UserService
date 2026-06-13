import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from './hooks'
import LoginPage    from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import UsersPage    from './pages/UsersPage'

// Simple guard — redirects to /login if not authenticated
function RequireAuth({ children }: { children: React.ReactNode }) {
  const loggedIn = useAuth()
  return loggedIn ? <>{children}</> : <Navigate to="/login" replace />
}

export default function App() {
  return (
    <Routes>
      <Route path="/login"    element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route
        path="/users"
        element={
          <RequireAuth>
            <UsersPage />
          </RequireAuth>
        }
      />
      <Route path="*" element={<Navigate to="/users" replace />} />
    </Routes>
  )
}
