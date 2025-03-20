import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { getStudent, getStudents } from "./api";

export function useStudents({ page, pageSize, grade, classname, searchValue }) {
  return useQuery({
    queryKey: ["students", { page, pageSize, grade, classname, searchValue }],
    queryFn: () => {
      return getStudents(page, pageSize, grade, classname, searchValue);
    },
    placeholderData: keepPreviousData,
  });
}

export function useStudent(id) {
  return useQuery({
    queryKey: ["student", id],
    queryFn: () => {
      return getStudent(id);
    },
    enabled: !!id,
  });
}
