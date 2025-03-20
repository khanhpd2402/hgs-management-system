import { keepPreviousData, useQuery } from "@tanstack/react-query";
import {
  getTeachers,
  getHeadTeacherAssignments,
  getTeachingAssignments,
  getTeacher,
} from "./api";

export function useTeachers() {
  return useQuery({
    queryKey: ["teachers"],
    queryFn: () => {
      return getTeachers();
    },
    placeholderData: keepPreviousData,
    // throwOnError: true,
  });
}

export function useTeacher(id) {
  return useQuery({
    queryKey: ["teacher", id],
    queryFn: () => {
      return getTeacher(id);
    },
    enabled: !!id,
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

export function useHTA({ page = 1, pageSize = 5, grade = "" }) {
  return useQuery({
    queryKey: ["head-teacher-assignments", { page, pageSize, grade }],
    queryFn: () => {
      return getHeadTeacherAssignments(page, pageSize, grade);
    },
    placeholderData: keepPreviousData,
  });
}
