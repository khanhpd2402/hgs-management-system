import { useMutation, useQueryClient } from "@tanstack/react-query";
import {
  getScheduleByTeacherId,
  getScheduleByStudent,
  deleteTimeTableDetail,
  updateTimeTableDetail,
  createTimeTableDetail,
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

export function useUpdateTimeTableDetail() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: updateTimeTableDetail,
    onSuccess: () => {
      queryClient.invalidateQueries(["timetable"]);
      toast.success("Cập nhật thời khóa biểu thành công");
    },
    onError: (error) => {
      console.error("Lỗi khi cập nhật thời khóa biểu:", error);
      toast.error("Có lỗi xảy ra khi cập nhật thời khóa biểu");
    },
  });
}

export function useCreateTimeTableDetail() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: createTimeTableDetail,
    onSuccess: () => {
      queryClient.invalidateQueries(["timetable"]);
      toast.success("Tạo thời khóa biểu thành công");
    },
    onError: (error) => {
      console.error("Lỗi khi tạo thời khóa biểu:", error);
      toast.error("Có lỗi xảy ra khi tạo thời khóa biểu");
    },
  });
}
