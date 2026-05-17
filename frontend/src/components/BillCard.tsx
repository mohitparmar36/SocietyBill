import React from "react";

interface BillCardProps {
  month: number;
  year: number;
  amount: number;
  dueDate: string;
  isPaid: boolean;
  onDownload: () => void;
}

const BillCard: React.FC<BillCardProps> = ({ month, year, amount, dueDate, isPaid, onDownload }) => (
  <div className="bg-white rounded shadow p-4 flex flex-col gap-2">
    <div className="font-medium">{month}/{year}</div>
    <div>Amount: ₹{amount}</div>
    <div>Due: {new Date(dueDate).toLocaleDateString()}</div>
    <div>Status: <span className={isPaid ? "text-green-600" : "text-red-600"}>{isPaid ? "Paid" : "Pending"}</span></div>
    <button className="mt-2 px-3 py-1 bg-blue-600 text-white rounded" onClick={onDownload}>Download PDF</button>
  </div>
);

export default BillCard;
