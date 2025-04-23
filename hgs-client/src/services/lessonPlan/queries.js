import { useQuery } from "@tanstack/react-query";
import { getLessonPlanByTeacher } from "./api";

export function useLessonPlanByTeacher(teacherId, pageNumber, pageSize) {
  return useQuery({
    queryKey: ["lessonPlansByTeacher", teacherId, pageNumber, pageSize],
    queryFn: () => getLessonPlanByTeacher(teacherId, pageNumber, pageSize),
    enabled: !!teacherId,
    keepPreviousData: true,
    staleTime: 5 * 60 * 1000,
  });
}
