import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  getScheduleByTeacherId,
  getScheduleByStudent,
  deleteTimeTableDetail,
} from "./api";
import { toast } from "react-toastify";

export function useGetScheduleByTeacherId() {
  return useMutation({
    mutationFn: getScheduleByTeacherId,
  });
}

export function useGetScheduleByStudent() {
  return useMutation({
    mutationFn: getScheduleByStudent,
  });
}

export function useDeleteTimeTableDetail() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: deleteTimeTableDetail,
    onSuccess: () => {
      queryClient.invalidateQueries(["timetable"]);
      toast.success("Xóa thời khóa biểu thành công");
    },
    onError: (error) => {
      console.error("Lỗi khi xóa thời khóa biểu:", error);
      toast.error("Có lỗi xảy ra khi xóa thời khóa biểu");
    },
  });
}
