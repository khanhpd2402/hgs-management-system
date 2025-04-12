import { useQuery } from "@tanstack/react-query";
import {
  getAcademicYears,
  getClasses,
  getGradeLevels,
  getRoles,
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

export function useClasses() {
  return useQuery({
    queryKey: ["classes"],
    queryFn: () => {
      return getClasses();
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

export function useRoles() {
  return useQuery({
    queryKey: ["roles"],
    queryFn: () => {
      return getRoles();
    },
  });
}

export function useGradeLevels() {
  return useQuery({
    queryKey: ["gradeLevels"],
    queryFn: () => {
      return getGradeLevels();
    },
  });
}
