import { useMutation } from "@tanstack/react-query";
import { createLessonPlan } from "./api";
import toast from "react-hot-toast";

export const useCreateLessonPlan = () => {
  return useMutation({
    mutationFn: createLessonPlan,
    onError: (error) => {
      const msg =
        error.response?.status === 401
          ? "Phiên đăng nhập đã hết hạn!"
          : `Tải lên thất bại: ${error.response?.data || "Lỗi hệ thống"}`;
      toast.error(msg);
    },
    onSuccess: () => {},
  });
};
