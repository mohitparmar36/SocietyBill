import React, { useState } from "react";
import { useAuth0 } from "@auth0/auth0-react";
import { useResidentBills } from "../hooks/useResidentBills";
import api from "../api/axios";
import { Download, AlertCircle, CheckCircle2, FileText, Calendar, IndianRupee } from "lucide-react";

const ResidentDashboard: React.FC = () => {
  const { user } = useAuth0();
  const [year, setYear] = useState(new Date().getFullYear());
  const { data: bills = [], isLoading } = useResidentBills(year);

  const totalOutstanding = bills.filter((b: any) => !b.isPaid).reduce((sum: number, b: any) => sum + b.amount, 0);
  const nextDueDate = bills.filter((b: any) => !b.isPaid).sort((a: any, b: any) => new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime())[0]?.dueDate;

  const handleDownload = async (url: string, filename: string) => {
    try {
      const response = await api.get(url, { responseType: "blob" });
      const blobUrl = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement("a");
      link.href = blobUrl;
      link.setAttribute("download", filename);
      document.body.appendChild(link);
      link.click();
      link.remove();
    } catch (error) {
      console.error("Download failed", error);
      alert("Failed to download file. Please try again.");
    }
  };

  const downloadYearlyStatement = () => {
    handleDownload(`/member/yearly-statement/${year}`, `yearly_statement_${year}.pdf`);
  };

  return (
    <div className="w-full">
      
      {/* Header Section */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-slate-900 tracking-tight">Welcome, {user?.name?.split("@")[0]}</h1>
        <p className="text-slate-500 mt-1">Here is your account summary and billing history.</p>
      </div>

      {/* Account Summary */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
        <div className={`glass-card p-8 border-t-4 ${totalOutstanding > 0 ? "border-t-red-500" : "border-t-green-500"}`}>
          <div className="flex justify-between items-start">
            <div>
              <div className="text-slate-500 font-medium mb-2 flex items-center gap-2">
                <IndianRupee size={18} /> Total Outstanding Amount
              </div>
              <div className={`text-4xl font-bold ${totalOutstanding > 0 ? "text-red-600" : "text-green-600"}`}>
                ₹{totalOutstanding.toFixed(2)}
              </div>
            </div>
            {totalOutstanding > 0 && (
              <div className="bg-red-50 text-red-600 p-3 rounded-full">
                <AlertCircle size={28} />
              </div>
            )}
            {totalOutstanding === 0 && (
              <div className="bg-green-50 text-green-600 p-3 rounded-full">
                <CheckCircle2 size={28} />
              </div>
            )}
          </div>
          {totalOutstanding > 0 && nextDueDate && (
            <div className="mt-6 pt-4 border-t border-slate-100 flex items-center gap-2 text-sm text-slate-600">
              <Calendar size={16} className="text-slate-400" />
              <span>Next payment due on <strong className="text-slate-900">{new Date(nextDueDate).toLocaleDateString()}</strong></span>
            </div>
          )}
        </div>
        
        <div className="glass-card p-8 flex flex-col justify-center items-center text-center">
          <div className="bg-blue-50 p-4 rounded-full text-blue-600 mb-4">
            <FileText size={32} />
          </div>
          <h3 className="text-lg font-bold text-slate-900 mb-2">Yearly Statement</h3>
          <p className="text-sm text-slate-500 mb-6">Download a consolidated PDF statement of all your transactions for the year.</p>
          <div className="flex items-center gap-3 w-full max-w-xs">
            <select 
              value={year} 
              onChange={e => setYear(Number(e.target.value))}
              className="input-field w-1/3"
            >
              {[new Date().getFullYear(), new Date().getFullYear() - 1, new Date().getFullYear() - 2].map(y => (
                <option key={y} value={y}>{y}</option>
              ))}
            </select>
            <button 
              onClick={downloadYearlyStatement}
              className="btn-primary w-2/3 gap-2"
            >
              <Download size={16} /> Download
            </button>
          </div>
        </div>
      </div>

      {/* Bills Table */}
      <div className="glass-card overflow-hidden">
        <div className="px-6 py-5 border-b border-slate-100 bg-white flex justify-between items-center">
          <h2 className="text-lg font-bold text-slate-900">Billing History ({year})</h2>
        </div>
        <div className="overflow-x-auto bg-white">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-50 text-slate-500 text-xs uppercase tracking-wider">
                <th className="px-6 py-4 font-medium">Billing Period</th>
                <th className="px-6 py-4 font-medium">Due Date</th>
                <th className="px-6 py-4 font-medium">Amount</th>
                <th className="px-6 py-4 font-medium">Status</th>
                <th className="px-6 py-4 font-medium text-right">Invoice</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {isLoading ? (
                <tr><td colSpan={5} className="px-6 py-12 text-center text-slate-500">Loading your bills...</td></tr>
              ) : bills.length === 0 ? (
                <tr>
                  <td colSpan={5} className="px-6 py-16 text-center">
                    <div className="flex flex-col items-center justify-center text-slate-400">
                      <FileText size={48} className="mb-4 text-slate-200" />
                      <p className="text-lg font-medium text-slate-600">No bills found</p>
                      <p className="text-sm">You do not have any bills for the selected year.</p>
                    </div>
                  </td>
                </tr>
              ) : (
                bills.map((bill: any) => (
                  <tr key={bill.id} className="hover:bg-slate-50 transition-colors">
                    <td className="px-6 py-4 text-slate-900 font-medium">
                      {new Date(bill.year, bill.month - 1).toLocaleString('default', { month: 'long' })} {bill.year}
                    </td>
                    <td className="px-6 py-4 text-slate-500">{new Date(bill.dueDate).toLocaleDateString()}</td>
                    <td className="px-6 py-4 font-semibold text-slate-900">₹{bill.amount.toFixed(2)}</td>
                    <td className="px-6 py-4">
                      {bill.isPaid ? (
                        <span className="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 border border-green-200">
                          <CheckCircle2 size={12} /> Paid
                        </span>
                      ) : (
                        <span className="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-medium bg-red-100 text-red-800 border border-red-200">
                          <AlertCircle size={12} /> Pending
                        </span>
                      )}
                    </td>
                    <td className="px-6 py-4 text-right">
                      <button
                        onClick={() => handleDownload(`/member/download/${bill.id}`, `bill_${bill.month}_${bill.year}.pdf`)}
                        className="inline-flex items-center gap-2 px-3 py-1.5 text-sm font-medium text-blue-600 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors"
                      >
                        <Download size={14} /> PDF
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

    </div>
  );
};

export default ResidentDashboard;
