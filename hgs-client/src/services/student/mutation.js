import { useMutation, useQueryClient } from "@tanstack/react-query";
import toast from "react-hot-toast";

import { updateStudent } from "./api";

export function useUpdateStudent() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ id, data }) => updateStudent(id, data),
    onSettled: (data, error, inputData) => {
      if (error) {
        console.log(error);
        toast.error("Đã có lỗi xảy ra");
      } else {
        console.log(data);
        console.log(inputData);
        toast.success("Cập nhật thành công");
        queryClient.invalidateQueries("student", inputData.id);
      }
    },
  });
}
