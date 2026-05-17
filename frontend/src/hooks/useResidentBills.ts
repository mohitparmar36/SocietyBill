import { useQuery } from "@tanstack/react-query";
import api from "../api/axios";

export const useResidentBills = (year: number) => {
  return useQuery([
    "resident-bills",
    year
  ], async () => {
    const { data } = await api.get(`/member/my-bills?year=${year}`);
    return data;
  });
};
