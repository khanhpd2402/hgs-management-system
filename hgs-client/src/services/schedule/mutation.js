import { useMutation, useQueryClient } from "@tanstack/react-query";
import { getScheduleByTeacherId } from "./api";

export function useGetScheduleByTeacherId() {
  return useMutation({
    mutationFn: getScheduleByTeacherId,
  });
}
