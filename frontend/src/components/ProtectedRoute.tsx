import React from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { Navigate } from "react-router-dom";

interface ProtectedRouteProps {
  role: string;
  children: React.ReactNode;
}

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ role, children }) => {
  const { isAuthenticated, user, loginWithRedirect } = useAuth0();
  if (!isAuthenticated) {
    loginWithRedirect();
    return null;
  }
  const roles = user?.["https://societybill.com/roles"] || [];
  
  // Backwards compatibility for early testing where "Admin" was used instead of "SocietyAdmin"
  const checkRole = role === "SocietyAdmin" ? ["SocietyAdmin", "Admin"] : [role];
  const hasRole = Array.isArray(roles) 
    ? checkRole.some(r => roles.includes(r)) 
    : checkRole.includes(roles as string);

  if (!hasRole) {
    return (
      <div className="flex flex-col items-center justify-center min-h-screen text-center p-6">
        <h2 className="text-2xl font-bold text-red-600 mb-2">Unauthorized Access</h2>
        <p className="text-gray-700">You do not have the necessary '{role}' role to view this page.</p>
        <p className="text-gray-500 mt-2 text-sm">Please contact the administrator or verify your Auth0 setup.</p>
      </div>
    );
  }
  return <>{children}</>;
};

export default ProtectedRoute;
