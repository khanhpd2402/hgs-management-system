import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  createTeacher,
  deleteTeacher,
  importTeachers,
  updateTeacher,
} from "./api";

import toast from "react-hot-toast";

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

export function useDeleteTeacher() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (id) => deleteTeacher(id),
    onSettled: (data, error) => {
      if (error) {
        console.log(error);
        console.log("đã có lỗi xảy ra");
      } else {
        console.log(data);
        console.log("xóa thành công");
        toast.success("Xóa thành công");
        queryClient.invalidateQueries("teachers");
      }
    },
  });
}
