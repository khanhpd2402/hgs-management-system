import { useQuery } from "@tanstack/react-query";
import { getScheduleByTeacherId, getScheduleByStudent, getTimetablesForPrincipal } from "./api";

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

export function useTimetablesForPrincipal(semesterId) {
    return useQuery({
        queryKey: ["timetables", "principal", semesterId],
        queryFn: () => getTimetablesForPrincipal(semesterId),
        enabled: !!semesterId,
    });
}