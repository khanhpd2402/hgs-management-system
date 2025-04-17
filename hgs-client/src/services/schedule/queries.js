import { useQuery } from "@tanstack/react-query";
import { getScheduleByTeacherId, getScheduleByStudent } from "./api";

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