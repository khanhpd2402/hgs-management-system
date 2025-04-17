import { useMutation, useQueryClient } from "@tanstack/react-query";
import { getScheduleByTeacherId, getScheduleByStudent } from "./api";

export function useGetScheduleByTeacherId() {
  return useMutation({
    mutationFn: getScheduleByTeacherId,
  });
}

export function useGetScheduleByStudent() {
  return useMutation({
    mutationFn: getScheduleByStudent,
  });
}
