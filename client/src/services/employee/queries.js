import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { getEmployees } from "./api";

export function useEmployees(page, limit) {
  return useQuery({
    queryKey: ["employees", { page, limit }],
    queryFn: () => {
      return getEmployees(page, limit);
    },
    placeholderData: keepPreviousData,
  });
}
