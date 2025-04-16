import { useQuery } from "@tanstack/react-query";
import { getScheduleByTeacherId } from "./api";

export function useScheduleTeacher(teacherId) {
    return useQuery({
        queryKey: ["schedule", teacherId],
        queryFn: () => getScheduleByTeacherId(teacherId),
        enabled: !!teacherId,
    });
}
