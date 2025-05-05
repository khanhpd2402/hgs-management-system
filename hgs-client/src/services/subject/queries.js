import { useQuery } from "@tanstack/react-query";
import { getSubjectByTeacher, getTeacherBySubject } from "./api";

export const useSubjectByTeacher = (teacherId) => {
  const query = useQuery({
    queryKey: ["subjects", teacherId],
    queryFn: () => getSubjectByTeacher(teacherId),
    enabled: !!teacherId,
  });

  return {
    ...query,
    subjects: query.data || [],
  };
};

export const useTeacherBySubject = (subjectId) => {
  const query = useQuery({
    queryKey: ["teachers", "subject", subjectId],
    queryFn: () => getTeacherBySubject(subjectId),
    enabled: !!subjectId,
  });

  return {
    ...query,
    teachers: query.data || [],
  };
};
