import { useQuery } from "@tanstack/react-query";
import { getGradeBatches } from "./api";

export function useGradeBatchs() {
  return useQuery({
    queryKey: ["grade-batchs"],
    queryFn: getGradeBatches,
  });
}
