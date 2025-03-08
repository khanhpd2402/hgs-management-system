import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { getStudents } from "./api";

export function useStudents({ page, pageSize, grade, classname, searchValue }) {
  return useQuery({
    queryKey: ["students", { page, pageSize, grade, classname, searchValue }],
    queryFn: () => {
      return getStudents(page, pageSize, grade, classname, searchValue);
    },
    placeholderData: keepPreviousData,
  });
}
