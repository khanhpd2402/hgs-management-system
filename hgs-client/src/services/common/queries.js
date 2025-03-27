import { useQuery } from "@tanstack/react-query";
import {
  getAcademicYears,
  getSemestersByAcademicYear,
  getSubjects,
} from "./api";

export function useSubjects() {
  return useQuery({
    queryKey: ["subjects"],
    queryFn: () => {
      return getSubjects();
    },
  });
}

export function useAcademicYears() {
  return useQuery({
    queryKey: ["AcademicYear"],
    queryFn: () => {
      return getAcademicYears();
    },
  });
}

export function useSemestersByAcademicYear(academicYearId) {
  return useQuery({
    queryKey: ["semesters", academicYearId],
    queryFn: () => {
      return getSemestersByAcademicYear(academicYearId);
    },
    enabled: !!academicYearId,
  });
}
