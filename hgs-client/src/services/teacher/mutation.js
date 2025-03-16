import { useMutation } from "@tanstack/react-query";
import { createTeacher, importTeachers, updateTeacher } from "./api";

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
    mutationFn: ({ id, data }) => updateTeacher(id, data),

    onSettled: (data, error) => {
      if (error) {
        console.log(error);
        console.log("đã có lỗi xảy ra");
      } else {
        console.log(data);
        console.log("cập nhật thành công");
      }
    },
  });
}

export function useCreateTeacher() {
  return useMutation({
    mutationFn: (data) => createTeacher(data),
    onSettled: (data, error) => {
      if (error) {
        console.log(error);
        console.log("đã có lỗi xảy ra");
      } else {
        console.log(data);
        console.log("tạo mới thành công");
      }
    },
  });
}
