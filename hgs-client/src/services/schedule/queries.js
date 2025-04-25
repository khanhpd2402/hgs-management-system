import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  getScheduleByTeacherId,
  getScheduleByStudent,
  getTimetableForPrincipal,
  createSubstituteTeaching,
  getSubstituteTeachings,
} from "./api";

export function useScheduleTeacher(teacherId) {
  return useQuery({
    queryKey: ["schedule", teacherId],
    queryFn: () => getScheduleByTeacherId(teacherId),
    enabled: !!teacherId,
  });
}
export function useScheduleStudent(studentId, semesterId) {
  return useQuery({
    queryKey: ["schedule", "student", studentId, semesterId],
    queryFn: () => getScheduleByStudent({ studentId, semesterId }),
    enabled: !!studentId && !!semesterId,
  });
}
export function useTimetableForPrincipal(timetableId) {
  return useQuery({
    queryKey: ["schedule", "principal", timetableId],
    queryFn: () => getTimetableForPrincipal(timetableId),
    enabled: !!timetableId,
  });
}

export function useCreateSubstituteTeaching() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createSubstituteTeaching,
    onSuccess: () => {
      queryClient.invalidateQueries("assignedTeacher");
    },
  });
}

export function useGetSubstituteTeachings(
  timetableDetailId,
  originalTeacherId,
  date,
) {
  return useQuery({
    queryKey: [
      "substituteTeachings",
      timetableDetailId,
      originalTeacherId,
      date,
    ],
    queryFn: () =>
      getSubstituteTeachings(timetableDetailId, originalTeacherId, date),
    enabled: !!timetableDetailId && !!originalTeacherId && !!date,
  });
}
