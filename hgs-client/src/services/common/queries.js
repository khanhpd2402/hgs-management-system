import { useQuery } from "@tanstack/react-query";
import { getSubjects } from "./api";

export function useSubjects() {
  return useQuery({
    queryKey: ["subjects"],
    queryFn: () => {
      return getSubjects();
    },
  });
}
