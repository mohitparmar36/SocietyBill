import React, { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import api from "../api/axios";
import { Users, FileText, IndianRupee, Plus, AlertCircle, CheckCircle2, CircleDollarSign } from "lucide-react";
import { useToast } from "../hooks/useToast";
import { toastUtils } from "../lib/toastUtils";

const AdminDashboard: React.FC = () => {
  const queryClient = useQueryClient();
  const toast = useToast();
  const [activeTab, setActiveTab] = useState<"flats" | "bills">("flats");
  
  // Flat Form State
  const [flatNumber, setFlatNumber] = useState("");
  const [ownerName, setOwnerName] = useState("");
  const [email, setEmail] = useState("");
  
  // Bill Form State
  const [billAmount, setBillAmount] = useState<number | "">("");

  const { data: flats = [], isLoading: isLoadingFlats } = useQuery(["admin-flats"], async () => {
    const { data } = await api.get("/admin/flats");
    return data;
  });

  const { data: bills = [], isLoading: isLoadingBills } = useQuery(["admin-bills"], async () => {
    const { data } = await api.get("/admin/bills");
    return data;
  });

  const createFlat = useMutation(async (newFlat: any) => {
    const { data } = await api.post("/admin/flats", newFlat);
    return data;
  }, {
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin-flats"] });
      toast.success("Flat added successfully!", "The new flat has been registered.");
    },
    onError: (error: any) => {
      toastUtils.apiError(error);
    }
  });

  const generateBills = useMutation(async (amount: number) => {
    const promises = flats.map((flat: any) => 
      api.post("/admin/bills", {
        flatId: flat.id,
        amount,
        month: new Date().getMonth() + 1,
        year: new Date().getFullYear(),
        dueDate: new Date(new Date().setDate(new Date().getDate() + 15)).toISOString()
      })
    );
    await Promise.all(promises);
  }, {
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["admin-bills"] });
      setBillAmount("");
      toast.success("Bills generated successfully!", `Generated bills for ${flats.length} flats`);
    },
    onError: (error: any) => {
      toastUtils.apiError(error);
    }
  });

  const handleCreateFlat = (e: React.FormEvent) => {
    e.preventDefault();
    if (!flatNumber || !ownerName || !email) return;
    toast.loading("Adding flat...");
    createFlat.mutate({ flatNumber, ownerName, email });
    setFlatNumber("");
    setOwnerName("");
    setEmail("");
  };

  const totalOutstanding = bills.filter((b: any) => !b.isPaid).reduce((sum: number, b: any) => sum + b.amount, 0);
  const totalCollected = bills.filter((b: any) => b.isPaid).reduce((sum: number, b: any) => sum + b.amount, 0);

  return (
    <div className="w-full">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-slate-900 tracking-tight">Dashboard Overview</h1>
        <p className="text-slate-500 mt-1">Manage your flats, generate bills, and track payments.</p>
      </div>

      {/* Stat Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div className="glass-card p-6 flex flex-col">
          <div className="flex items-center gap-3 text-slate-500 mb-2 font-medium">
            <Users size={20} className="text-blue-500" /> Total Flats
          </div>
          <div className="text-3xl font-bold text-slate-900">{flats.length}</div>
        </div>
        <div className="glass-card p-6 flex flex-col">
          <div className="flex items-center gap-3 text-slate-500 mb-2 font-medium">
            <FileText size={20} className="text-indigo-500" /> Total Bills
          </div>
          <div className="text-3xl font-bold text-slate-900">{bills.length}</div>
        </div>
        <div className="glass-card p-6 flex flex-col">
          <div className="flex items-center gap-3 text-slate-500 mb-2 font-medium">
            <IndianRupee size={20} className="text-green-500" /> Collected
          </div>
          <div className="text-3xl font-bold text-slate-900">₹{totalCollected}</div>
        </div>
        <div className="glass-card p-6 flex flex-col border-l-4 border-l-red-500">
          <div className="flex items-center gap-3 text-slate-500 mb-2 font-medium">
            <AlertCircle size={20} className="text-red-500" /> Outstanding
          </div>
          <div className="text-3xl font-bold text-red-600">₹{totalOutstanding}</div>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex space-x-1 bg-slate-200/50 p-1 rounded-xl mb-6 w-max">
        <button
          onClick={() => setActiveTab("flats")}
          className={`flex items-center gap-2 px-6 py-2.5 rounded-lg text-sm font-medium transition-all ${activeTab === "flats" ? "bg-white text-slate-900 shadow-sm" : "text-slate-500 hover:text-slate-700"}`}
        >
          <Users size={16} /> Manage Flats
        </button>
        <button
          onClick={() => setActiveTab("bills")}
          className={`flex items-center gap-2 px-6 py-2.5 rounded-lg text-sm font-medium transition-all ${activeTab === "bills" ? "bg-white text-slate-900 shadow-sm" : "text-slate-500 hover:text-slate-700"}`}
        >
          <CircleDollarSign size={16} /> Billing
        </button>
      </div>

      {/* Tab Content: Flats */}
      {activeTab === "flats" && (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-1">
            <div className="glass-card p-6">
              <h2 className="text-xl font-bold text-slate-900 mb-6 flex items-center gap-2">
                <Plus size={20} className="text-blue-600" /> Add New Flat
              </h2>
              <form onSubmit={handleCreateFlat} className="flex flex-col gap-4">
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">Flat Number</label>
                  <input type="text" value={flatNumber} onChange={e => setFlatNumber(e.target.value)} className="input-field" placeholder="e.g. A-101" required />
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">Owner Name</label>
                  <input type="text" value={ownerName} onChange={e => setOwnerName(e.target.value)} className="input-field" placeholder="John Doe" required />
                </div>
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">Owner Email</label>
                  <input type="email" value={email} onChange={e => setEmail(e.target.value)} className="input-field" placeholder="john@example.com" required />
                </div>
                <button type="submit" disabled={createFlat.isLoading} className="btn-primary mt-2">
                  {createFlat.isLoading ? "Adding..." : "Add Flat"}
                </button>
              </form>
            </div>
          </div>
          
          <div className="lg:col-span-2">
            <div className="glass-card overflow-hidden">
              <div className="px-6 py-5 border-b border-slate-100 bg-white">
                <h2 className="text-lg font-bold text-slate-900">Registered Flats</h2>
              </div>
              <div className="overflow-x-auto bg-white">
                <table className="w-full text-left border-collapse">
                  <thead>
                    <tr className="bg-slate-50 text-slate-500 text-xs uppercase tracking-wider">
                      <th className="px-6 py-4 font-medium">Flat Number</th>
                      <th className="px-6 py-4 font-medium">Owner</th>
                      <th className="px-6 py-4 font-medium">Email</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-100">
                    {isLoadingFlats ? (
                      <tr><td colSpan={3} className="px-6 py-8 text-center text-slate-500">Loading flats...</td></tr>
                    ) : flats.length === 0 ? (
                      <tr><td colSpan={3} className="px-6 py-12 text-center text-slate-500">No flats added yet.</td></tr>
                    ) : (
                      flats.map((f: any) => (
                        <tr key={f.id} className="hover:bg-slate-50 transition-colors">
                          <td className="px-6 py-4 font-medium text-slate-900">{f.flatNumber}</td>
                          <td className="px-6 py-4 text-slate-600">{f.ownerName}</td>
                          <td className="px-6 py-4 text-slate-500">{f.email}</td>
                        </tr>
                      ))
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Tab Content: Bills */}
      {activeTab === "bills" && (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div className="lg:col-span-1">
            <div className="glass-card p-6 border-t-4 border-t-indigo-500">
              <h2 className="text-xl font-bold text-slate-900 mb-2">Mass Generation</h2>
              <p className="text-sm text-slate-500 mb-6">Generate bills for all registered flats for the current month.</p>
              
              <div className="flex flex-col gap-4">
                <div>
                  <label className="block text-sm font-medium text-slate-700 mb-1">Bill Amount (₹)</label>
                  <div className="relative">
                    <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                      <IndianRupee size={16} className="text-slate-400" />
                    </div>
                    <input 
                      type="number" value={billAmount} onChange={e => setBillAmount(Number(e.target.value))} 
                      className="input-field pl-9" placeholder="0.00" 
                    />
                  </div>
                </div>
                <button 
                  onClick={() => {
                    if (billAmount && Number(billAmount) > 0) {
                      toast.loading("Generating bills...");
                      generateBills.mutate(Number(billAmount));
                    }
                  }}
                  disabled={generateBills.isLoading || flats.length === 0}
                  className="w-full inline-flex items-center justify-center rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-medium text-white shadow-sm hover:bg-indigo-700 disabled:opacity-50 transition-colors"
                >
                  {generateBills.isLoading ? "Processing..." : "Generate Monthly Bills"}
                </button>
                {flats.length === 0 && (
                  <p className="text-xs text-red-500 text-center mt-2">You must add flats before generating bills.</p>
                )}
              </div>
            </div>
          </div>

          <div className="lg:col-span-2">
            <div className="glass-card overflow-hidden">
              <div className="px-6 py-5 border-b border-slate-100 bg-white">
                <h2 className="text-lg font-bold text-slate-900">Billing History</h2>
              </div>
              <div className="overflow-x-auto bg-white">
                <table className="w-full text-left border-collapse">
                  <thead>
                    <tr className="bg-slate-50 text-slate-500 text-xs uppercase tracking-wider">
                      <th className="px-6 py-4 font-medium">Flat Number</th>
                      <th className="px-6 py-4 font-medium">Period</th>
                      <th className="px-6 py-4 font-medium">Amount</th>
                      <th className="px-6 py-4 font-medium">Status</th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-100">
                    {isLoadingBills ? (
                      <tr><td colSpan={4} className="px-6 py-8 text-center text-slate-500">Loading bills...</td></tr>
                    ) : bills.length === 0 ? (
                      <tr><td colSpan={4} className="px-6 py-12 text-center text-slate-500">No bills generated yet.</td></tr>
                    ) : (
                      bills.map((b: any) => (
                        <tr key={b.id} className="hover:bg-slate-50 transition-colors">
                          <td className="px-6 py-4 font-medium text-slate-900">{b.flatNumber}</td>
                          <td className="px-6 py-4 text-slate-900 font-medium">{b.month}/{b.year}</td>
                          <td className="px-6 py-4 font-semibold text-slate-900">₹{b.amount}</td>
                          <td className="px-6 py-4">
                            {b.isPaid ? (
                              <span className="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 border border-green-200">
                                <CheckCircle2 size={12} /> Paid
                              </span>
                            ) : (
                              <span className="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-medium bg-amber-100 text-amber-800 border border-amber-200">
                                <AlertCircle size={12} /> Pending
                              </span>
                            )}
                          </td>
                        </tr>
                      ))
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
      )}

    </div>
  );
};

export default AdminDashboard;
