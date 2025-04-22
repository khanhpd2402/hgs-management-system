import { useMutation } from "@tanstack/react-query";
import { createLessonPlan } from "./api";

export const useCreateLessonPlan = () => {
  return useMutation({
    mutationFn: (data) => createLessonPlan(data),
  });
};
