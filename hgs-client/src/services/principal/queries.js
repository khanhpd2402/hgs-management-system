import { useQuery } from "@tanstack/react-query";
import {
  getClassesWithStudentCount,
  getGradeBatch,
  getGradeBatches,
  getSubjectConfigueDetail,
  getSubjectConfigues,
  getSubjectDetail,
  getTeacherSubjects,
  getTeachingAssignments,
  getUsers,
} from "./api";

export function useGradeBatchs() {
  return useQuery({
    queryKey: ["grade-batchs"],
    queryFn: getGradeBatches,
  });
}

export function useGradeBatch(id) {
  return useQuery({
    queryKey: ["grade-batch", id],
    queryFn: () => getGradeBatch(id),
    enabled: !!id,
  });
}

// getUser
export function useUsers() {
  return useQuery({
    queryKey: ["users"],
    queryFn: getUsers,
  });
}

export function useSubjectDetail(id) {
  return useQuery({
    queryKey: ["subject", { id }],
    queryFn: () => getSubjectDetail(id),
    enabled: !!id,
  });
}

export function useSubjectConfigue() {
  return useQuery({
    queryKey: ["subjectConfig"],
    queryFn: getSubjectConfigues,
  });
}

export function useSubjectConfigueDetail(id, options) {
  return useQuery({
    queryKey: ["subjectConfig", { id }],
    queryFn: () => getSubjectConfigueDetail(id),
    ...options,
    enabled: !!id && options?.enabled,
  });
}

//teaching assignment

export function useTeacherSubjects() {
  return useQuery({
    queryKey: ["teacherSubjects"],
    queryFn: getTeacherSubjects,
  });
}

export function useTA() {
  return useQuery({
    queryKey: ["teaching-assignments"],
    queryFn: () => {
      return getTeachingAssignments();
    },
  });
}
export function useClassesWithStudentCount(academicYearId) {
  return useQuery({
    queryKey: ["classes-with-student-count", academicYearId],
    queryFn: () => {
      return getClassesWithStudentCount(academicYearId);
    },
    enabled: !!academicYearId,
  });
}
