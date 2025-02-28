import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { getEmployees } from "./api";

export function useEmployees({ page, pageSize, department, contract, search }) {
  return useQuery({
    queryKey: ["employees", { page, pageSize, department, contract, search }],
    queryFn: () => {
      return getEmployees(page, pageSize, department, contract, search);
    },
    placeholderData: keepPreviousData,
  });
}
