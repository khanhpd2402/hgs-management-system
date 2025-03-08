import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { getEmployees } from "./api";

export function useEmployees({ page, pageSize, sort, order, search }) {
  return useQuery({
    queryKey: ["employees", { page, pageSize, sort, order, search }],
    queryFn: () => {
      return getEmployees(page, pageSize, sort, order, search);
    },
    placeholderData: keepPreviousData,
  });
}
