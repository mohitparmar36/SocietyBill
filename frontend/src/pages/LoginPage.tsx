import React, { useEffect } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { useNavigate } from "react-router-dom";
import { Building2, ShieldCheck, ArrowRight, Activity } from "lucide-react";

const LoginPage: React.FC = () => {
  const { loginWithRedirect, isAuthenticated, user, isLoading } = useAuth0();
  const navigate = useNavigate();

  const [errorMsg, setErrorMsg] = React.useState<string | null>(null);

  useEffect(() => {
    if (!isLoading && isAuthenticated && user) {
      const roles = user["https://societybill.com/roles"] || [];
      const hasAdmin = Array.isArray(roles) ? (roles.includes("SocietyAdmin") || roles.includes("Admin")) : (roles === "SocietyAdmin" || roles === "Admin");
      const hasResident = Array.isArray(roles) ? roles.includes("Resident") : roles === "Resident";
      
      if (hasAdmin) {
        navigate("/admin");
      } else if (hasResident) {
        navigate("/resident");
      } else {
        navigate("/register-society");
      }
    }
  }, [isLoading, isAuthenticated, user, navigate]);

  if (isLoading) return null;
  if (isAuthenticated && !errorMsg) return null; 

  return (
    <div className="min-h-screen flex w-full bg-slate-50">
      
      {/* Left Column - Form/Action */}
      <div className="w-full lg:w-5/12 flex flex-col justify-center items-center p-8 sm:p-12 lg:p-16 relative z-10 bg-white shadow-[20px_0_40px_-15px_rgba(0,0,0,0.05)]">
        <div className="w-full max-w-md">
          {/* Logo */}
          <div className="flex items-center gap-3 mb-12">
            <div className="bg-blue-600 p-2.5 rounded-xl text-white shadow-lg shadow-blue-600/30">
              <Building2 size={28} />
            </div>
            <span className="text-3xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-blue-700 to-indigo-700 tracking-tight">
              SocietyBill
            </span>
          </div>

          <div className="mb-10">
            <h1 className="text-4xl font-bold text-slate-900 mb-4 tracking-tight">
              Welcome back
            </h1>
            <p className="text-lg text-slate-500 font-light">
              Manage your society effortlessly. Access bills, track payments, and manage flats all in one place.
            </p>
          </div>

          {errorMsg && (
            <div className="mb-8 p-4 rounded-xl bg-red-50 border border-red-100 flex items-start gap-3">
              <div className="text-red-500 mt-0.5"><Activity size={20} /></div>
              <div className="text-red-800 text-sm font-medium leading-relaxed">{errorMsg}</div>
            </div>
          )}

          <div className="space-y-6">
            <button
              className="w-full group relative flex items-center justify-center gap-3 rounded-xl bg-slate-900 px-6 py-4 text-white font-medium shadow-xl shadow-slate-900/20 transition-all hover:bg-slate-800 hover:shadow-slate-900/30 focus:outline-none focus:ring-2 focus:ring-slate-900 focus:ring-offset-2 active:scale-[0.98]"
              onClick={() => loginWithRedirect()}
            >
              <ShieldCheck size={20} className="text-slate-300 group-hover:text-white transition-colors" />
              <span>Continue with Auth0</span>
              <ArrowRight size={18} className="absolute right-6 opacity-0 -translate-x-4 transition-all group-hover:opacity-100 group-hover:translate-x-0" />
            </button>
            <p className="text-center text-sm text-slate-400">
              Secure, identity-based authentication.
            </p>
          </div>
        </div>
      </div>

      {/* Right Column - Visual/Hero */}
      <div className="hidden lg:flex w-7/12 relative overflow-hidden bg-gradient-to-br from-blue-600 via-indigo-700 to-slate-900">
        {/* Decorative elements */}
        <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxjaXJjbGUgY3g9IjMiIGN5PSIzIiByPSIzIiBmaWxsPSIjZmZmIiBmaWxsLW9wYWNpdHk9IjAuMSIvPjwvZz48L3N2Zz4=')]"></div>
        <div className="absolute -top-[30%] -right-[10%] w-[70%] h-[70%] rounded-full bg-blue-400/20 blur-[120px]"></div>
        <div className="absolute -bottom-[20%] -left-[10%] w-[60%] h-[60%] rounded-full bg-indigo-500/20 blur-[100px]"></div>

        {/* Floating cards showcase */}
        <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-2xl">
          <div className="glass-card bg-white/10 border-white/10 p-8 transform -rotate-3 hover:rotate-0 transition-transform duration-500 shadow-2xl">
            <div className="flex justify-between items-center mb-6">
              <div className="h-4 w-32 bg-white/20 rounded-full"></div>
              <div className="h-8 w-24 bg-green-400/20 text-green-300 flex items-center justify-center rounded-full text-sm font-semibold">PAID</div>
            </div>
            <div className="space-y-4">
              <div className="h-3 w-3/4 bg-white/20 rounded-full"></div>
              <div className="h-3 w-1/2 bg-white/20 rounded-full"></div>
              <div className="h-3 w-5/6 bg-white/20 rounded-full"></div>
            </div>
            <div className="mt-8 flex justify-between items-end">
              <div className="h-10 w-28 bg-white/30 rounded-lg"></div>
              <div className="h-12 w-12 rounded-full bg-white/20"></div>
            </div>
          </div>
        </div>
        
        <div className="absolute bottom-12 left-12 right-12 text-center text-white/80 font-medium">
          "The modern standard for society management and billing."
        </div>
      </div>
      
    </div>
  );
};

export default LoginPage;
