import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { getStudent, getStudents } from "./api";

export function useStudents(academicId) {
  return useQuery({
    queryKey: ["students", { academicId }],
    queryFn: () => {
      return getStudents(academicId);
    },
    placeholderData: keepPreviousData,
    enabled: !!academicId,
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
