import { useQuery } from "@tanstack/react-query";
import { getGradeBatch, getGradeBatches, getUsers } from "./api";

export function useGradeBatchs() {
  return useQuery({
    queryKey: ["grade-batchs"],
    queryFn: getGradeBatches,
  });
}

export function useGradeBatch(id) {
  return useQuery({
    queryKey: ["grade-batch", id],
    queryFn: () => getGradeBatch(id),
    enabled: !!id,
  });
}

// getUser
export function useUsers() {
  return useQuery({
    queryKey: ["users"],
    queryFn: getUsers,
  });
}
