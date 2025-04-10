import { useQuery } from "@tanstack/react-query";
import { getLeaveRequests } from "./api";

export function useLeaveRequests() {
  return useQuery({
    queryKey: ["leave-requests"],
    queryFn: getLeaveRequests,
  });
}

export function useLeaveRequest(id) {
  return useQuery({
    queryKey: ["leave-request", id],
    queryFn: () => getLeaveRequest(id),
  });
}