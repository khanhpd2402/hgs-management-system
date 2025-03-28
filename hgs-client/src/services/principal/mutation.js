import { useMutation } from "@tanstack/react-query";
import { addGradeBatch, changeUserStatus, resetUserPassword } from "./api";
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

export function useChangeStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ userId, status }) => {
      return changeUserStatus(userId, status);
    },
    onSettled: (data, error) => {
      if (error) {
        console.log(error);
        toast.error("Thay đổi trạng thái thất bại");
      } else {
        console.log(data);
        queryClient.invalidateQueries({ queryKey: ["users"] });
        // toast.success("Thay đổi trạng thái thành công");
      }
    },
  });
}

export function useResetPassword() {
  return useMutation({
    mutationFn: ({ userId, newPassword }) => {
      return resetUserPassword(userId, newPassword);
    },
    onSettled: (data, error) => {
      if (error) {
        console.log(error);
        toast.error("Đặt lại mật khẩu thất bại");
      } else {
        console.log(data);
        toast.success("Đặt lại mật khẩu thành công");
      }
    },
  });
}
