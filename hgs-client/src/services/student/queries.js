import { keepPreviousData, useQueries, useQuery } from "@tanstack/react-query";
import {
  getStudent,
  getStudentAttendance,
  getStudentById,
  getStudents,
} from "./api";

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

export function useStudent({ id, academicYearId }) {
  return useQuery({
    queryKey: ["student", { id, academicYearId }],
    queryFn: () => {
      return getStudent(id, academicYearId);
    },
    enabled: !!id && !!academicYearId,
  });
}

export function useStudentListAttendances({ studentIds, weekStart }) {
  return useQueries({
    queries: studentIds.map((id) => ({
      queryKey: ["student-attendance", { id }],
      queryFn: () => {
        return getStudentAttendance(id, weekStart);
      },
      enabled: !!id,
    })),
  });
}

export function useListStudentById({ studentIds, academicYearId }) {
  return useQueries({
    queries: studentIds.map((id) => ({
      queryKey: ["student", { id, academicYearId }],
      queryFn: () => {
        return getStudentById(id, academicYearId);
      },
      enabled: !!id && !!academicYearId,
    })),
  });
}
