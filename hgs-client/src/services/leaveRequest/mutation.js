import toast from "react-hot-toast";
import { useQueryClient } from "@tanstack/react-query";
import { useMutation } from "@tanstack/react-query";
import { createLeaveRequest } from "./api";

export const useCreateLeaveRequest = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createLeaveRequest,
    onSuccess: (data) => {
      toast.success(data?.message || "Yêu cầu nghỉ phép đã được gửi thành công!");
      queryClient.invalidateQueries({ queryKey: ['leaveRequests'] });
    },
    onError: (error) => {
      const errorMessage = error.response?.data?.message || 'Có lỗi xảy ra khi gửi yêu cầu nghỉ phép.';
      toast.error(errorMessage);
    },
  });
};
