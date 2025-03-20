import { useMutation } from "@tanstack/react-query";
import { addGradeBatch } from "./api";
import toast from "react-hot-toast";

export function useAddGradeBatch() {
  return useMutation({
    mutationFn: (data) => {
      return addGradeBatch(data);
    },
    onSettled: (data, error) => {
      if (error) {
        console.log(error);
        toast.error("Thêm thất bại");
      } else {
        console.log(data);
        toast.success("Thêm thành công");
      }
    },
  });
}
