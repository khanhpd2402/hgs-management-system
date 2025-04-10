import { useQuery } from "@tanstack/react-query";
import { getLeaveRequestByTeacherId, getLeaveRequestByAdmin } from "./api";

export const useGetLeaveRequestByTeacherId = (teacherId) => {
  return useQuery({
    queryKey: ['leaveRequests', 'teacher', teacherId],
    queryFn: () => getLeaveRequestByTeacherId(teacherId),
    enabled: !!teacherId,
  });
};

export const useGetLeaveRequestByAdmin = () => {
  return useQuery({
    queryKey: ['leaveRequests', 'admin'],
    queryFn: getLeaveRequestByAdmin,
  });
};


