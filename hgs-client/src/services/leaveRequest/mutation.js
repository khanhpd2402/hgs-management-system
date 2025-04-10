import toast from "react-hot-toast";
import { useQueryClient } from "@tanstack/react-query";
import { useMutation } from "@tanstack/react-query";
import { createLeaveRequest } from "./api";

export function useCreateLeaveRequest() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createLeaveRequest,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["leave-requests"] });
      toast.success("Yêu cầu nghỉ phép đã được gửi thành công!");
    },
    onError: () => {
      toast.error("Gửi yêu cầu nghỉ phép thất bại!");
    },
  });
}   
