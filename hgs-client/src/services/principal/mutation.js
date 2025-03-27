import { useMutation } from "@tanstack/react-query";
import { addGradeBatch } from "./api";
import toast from "react-hot-toast";
import { useQueryClient } from "@tanstack/react-query";

export function useAddGradeBatch() {
  const queryClient = useQueryClient();
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
        queryClient.invalidateQueries({ queryKey: ["grade-batchs"] });
        toast.success("Thêm thành công");
      }
    },
  });
}
