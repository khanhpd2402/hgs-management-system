import { useQuery } from "@tanstack/react-query";
import {
  getAllSemesters,
  getClassById,
  getClassesWithStudentCount,
  getGradeBatch,
  getGradeBatches,
  getHomeroomTeachers,
  getSubjectConfigueDetail,
  getSubjectConfigues,
  getSubjectDetail,
  getTeacherSubjects,
  getTeachingAssignments,
  getTeachingAssignmentsByTeacher,
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

export function useTA(semesterID) {
  return useQuery({
    queryKey: ["teaching-assignments", { semesterID }],
    queryFn: () => {
      return getTeachingAssignments(semesterID);
    },
    enabled: !!semesterID,
  });
}

export function useTeachingAssignmentsByTeacher({ teacherId, semesterId }) {
  return useQuery({
    queryKey: ["teaching-assignments-by-teacher", { teacherId, semesterId }],
    queryFn: () => {
      return getTeachingAssignmentsByTeacher(teacherId, semesterId);
    },
    enabled: !!teacherId && !!semesterId,
  });
}

export function useHomeroomTeachers() {
  return useQuery({
    queryKey: ["hoomroom-teachers"],
    queryFn: getHomeroomTeachers,
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

export function useAllSemesters() {
  return useQuery({
    queryKey: ["all-semesters"],
    queryFn: () => {
      return getAllSemesters();
    },
  });
}

//class
export function useClass(id) {
  return useQuery({
    queryKey: ["class", id],
    queryFn: () => getClassById(id),
    enabled: !!id,
  });
}
