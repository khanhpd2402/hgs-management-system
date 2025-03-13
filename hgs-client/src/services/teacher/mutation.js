import { useMutation } from "@tanstack/react-query";
import { importTeachers, updateTeacher } from "./api";

export function useImportTeachers() {
  return useMutation({
    mutationFn: (fileExcel) => importTeachers(fileExcel),
    onSettled: (data, error) => {
      if (error) {
        console.log(error);
      } else {
        console.log(data);
      }
    },
  });
}

export function useUpdateTeacher() {
  return useMutation({
    mutationFn: (data) => updateTeacher(data.id, data),
    onSuccess: () => {},
    onError: (error) => {
      console.log(error);
    },
  });
}
