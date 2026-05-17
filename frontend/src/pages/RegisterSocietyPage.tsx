import React, { useState } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { useNavigate } from "react-router-dom";
import { Building2, ArrowRight, Loader2 } from "lucide-react";

const RegisterSocietyPage: React.FC = () => {
  const { getAccessTokenSilently, loginWithRedirect } = useAuth0();
  const navigate = useNavigate();

  const [name, setName] = useState("");
  const [address, setAddress] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setErrorMsg(null);

    try {
      const token = await getAccessTokenSilently();
      const apiUrl = import.meta.env.VITE_API_URL || "http://localhost:5000/api";
      const response = await fetch(`${apiUrl}/societies/register`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({ name, address })
      });

      if (!response.ok) {
        throw new Error("Failed to register society.");
      }

      setSuccess(true);
      
      // Need to login again to refresh the token and claims
      setTimeout(() => {
        loginWithRedirect();
      }, 3000);

    } catch (err: any) {
      setErrorMsg(err.message || "An error occurred.");
    } finally {
      setIsLoading(false);
    }
  };

  if (success) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-slate-50 p-4">
        <div className="max-w-md w-full bg-white p-8 rounded-2xl shadow-xl text-center">
          <div className="w-16 h-16 bg-green-100 text-green-600 rounded-full flex items-center justify-center mx-auto mb-6">
            <Building2 size={32} />
          </div>
          <h2 className="text-2xl font-bold text-slate-900 mb-2">Society Registered!</h2>
          <p className="text-slate-600 mb-6">
            We are configuring your account. You will be redirected to log in again to apply your new Admin privileges...
          </p>
          <div className="flex justify-center">
            <Loader2 className="animate-spin text-blue-600" size={24} />
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50 p-4">
      <div className="max-w-md w-full bg-white p-8 sm:p-10 rounded-2xl shadow-xl shadow-slate-200/50">
        <div className="flex items-center gap-3 mb-8 justify-center">
          <div className="bg-blue-600 p-2.5 rounded-xl text-white shadow-lg shadow-blue-600/30">
            <Building2 size={24} />
          </div>
          <span className="text-2xl font-bold text-slate-900">SocietyBill</span>
        </div>

        <div className="text-center mb-8">
          <h1 className="text-2xl font-bold text-slate-900 mb-2">Create your Society</h1>
          <p className="text-slate-500 text-sm">
            You don't belong to any society yet. Let's get yours set up!
          </p>
        </div>

        {errorMsg && (
          <div className="mb-6 p-3 rounded-lg bg-red-50 border border-red-100 text-red-600 text-sm text-center">
            {errorMsg}
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <label htmlFor="name" className="block text-sm font-medium text-slate-700 mb-1">
              Society Name
            </label>
            <input
              id="name"
              type="text"
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="w-full px-4 py-3 rounded-xl border border-slate-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all bg-slate-50 focus:bg-white"
              placeholder="e.g. Green Valley Apartments"
            />
          </div>

          <div>
            <label htmlFor="address" className="block text-sm font-medium text-slate-700 mb-1">
              Address
            </label>
            <input
              id="address"
              type="text"
              required
              value={address}
              onChange={(e) => setAddress(e.target.value)}
              className="w-full px-4 py-3 rounded-xl border border-slate-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all bg-slate-50 focus:bg-white"
              placeholder="Full physical address"
            />
          </div>

          <button
            type="submit"
            disabled={isLoading}
            className="w-full mt-4 flex items-center justify-center gap-2 rounded-xl bg-blue-600 px-4 py-3.5 text-white font-medium shadow-lg shadow-blue-600/20 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all disabled:opacity-70 disabled:cursor-not-allowed"
          >
            {isLoading ? (
              <Loader2 className="animate-spin" size={20} />
            ) : (
              <>
                <span>Register Society</span>
                <ArrowRight size={18} />
              </>
            )}
          </button>
        </form>
      </div>
    </div>
  );
};

export default RegisterSocietyPage;
