import React from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { Building2, LogOut, User as UserIcon } from "lucide-react";

const Layout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { user, isAuthenticated, logout } = useAuth0();

  return (
    <div className="min-h-screen flex flex-col">
      {/* Top Navigation */}
      <header className="sticky top-0 z-50 w-full glass border-b border-slate-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
          
          {/* Logo / Brand */}
          <div className="flex items-center gap-2">
            <div className="bg-blue-600 p-2 rounded-lg text-white">
              <Building2 size={20} />
            </div>
            <span className="text-xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-blue-700 to-indigo-700 tracking-tight">
              SocietyBill
            </span>
          </div>

          {/* User Controls */}
          {isAuthenticated && user && (
            <div className="flex items-center gap-4">
              <div className="flex items-center gap-3 hidden sm:flex">
                <div className="flex flex-col items-end">
                  <span className="text-sm font-semibold text-slate-800">{user.name}</span>
                  <span className="text-xs text-slate-500">{user.email}</span>
                </div>
                {user.picture ? (
                  <img src={user.picture} alt={user.name} className="h-9 w-9 rounded-full border border-slate-200 shadow-sm" />
                ) : (
                  <div className="h-9 w-9 rounded-full bg-slate-100 border border-slate-200 flex items-center justify-center text-slate-500">
                    <UserIcon size={18} />
                  </div>
                )}
              </div>
              <div className="h-8 w-px bg-slate-200 hidden sm:block"></div>
              <button
                onClick={() => logout({ logoutParams: { returnTo: window.location.origin } })}
                className="flex items-center gap-2 text-sm font-medium text-slate-600 hover:text-red-600 transition-colors p-2 rounded-lg hover:bg-red-50"
                title="Log Out"
              >
                <LogOut size={18} />
                <span className="hidden sm:inline">Log Out</span>
              </button>
            </div>
          )}
        </div>
      </header>

      {/* Main Content Area */}
      <main className="flex-1 w-full max-w-7xl mx-auto p-4 sm:p-6 lg:p-8">
        {children}
      </main>
      
      {/* Footer */}
      <footer className="w-full border-t border-slate-200 py-6 text-center text-sm text-slate-500 bg-white">
        &copy; {new Date().getFullYear()} SocietyBill. All rights reserved.
      </footer>
    </div>
  );
};

export default Layout;
