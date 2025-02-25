import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { getEmployees } from "./api";

export function useEmployees(page, limit, sort, order, search) {
  return useQuery({
    queryKey: ["employees", { page, limit, sort, order, search }],
    queryFn: () => {
      return getEmployees(page, limit, sort, order, search);
    },
    placeholderData: keepPreviousData,
  });
}
