import { keepPreviousData, useQuery } from "@tanstack/react-query";
import {
  getEmployees,
  getHeadTeacherAssignments,
  getTeachingAssignments,
} from "./api";

export function useEmployees({ page, pageSize, department, contract, search }) {
  return useQuery({
    queryKey: ["employees", { page, pageSize, department, contract, search }],
    queryFn: () => {
      return getEmployees(page, pageSize, department, contract, search);
    },
    placeholderData: keepPreviousData,
  });
}

export function useTA({ page, pageSize, department, teacher, semester }) {
  return useQuery({
    queryKey: [
      "teaching-assignments",
      { page, pageSize, department, teacher, semester },
    ],
    queryFn: () => {
      return getTeachingAssignments(
        page,
        pageSize,
        department,
        teacher,
        semester,
      );
    },
    placeholderData: keepPreviousData,
  });
}

export function useHTA({ page, pageSize, grade, teacher }) {
  return useQuery({
    queryKey: ["head-teacher-assignments", { page, pageSize, grade, teacher }],
    queryFn: () => {
      return getHeadTeacherAssignments(page, pageSize, grade, teacher);
    },
    placeholderData: keepPreviousData,
  });
}
