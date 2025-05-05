import { useMutation, useQueryClient } from "@tanstack/react-query";
import { sendNotification } from "./api";
import toast from "react-hot-toast";

export const useSendNotification = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data) => sendNotification(data),
    onSuccess: () => {
      toast.success("Gửi thông báo thành công!");
      // Có thể thêm invalidate cache nếu cần
      // queryClient.invalidateQueries(["notifications"]);
    },
    onError: (error) => {
      console.error("Lỗi khi gửi thông báo:", error);
      toast.error(
        error.response?.data?.message ||
          "Có lỗi xảy ra khi gửi thông báo. Vui lòng thử lại!",
      );
    },
  });
};
