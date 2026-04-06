import { Navigate, Outlet } from 'react-router-dom';

export const ProtectedRoute = () => {
  const isAuthenticated = !!localStorage.getItem('token'); // Check your login logic
  return isAuthenticated ? <Outlet /> : <Navigate to="/" replace />;
};
