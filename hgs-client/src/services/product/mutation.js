import { useMutation, useQueryClient } from "@tanstack/react-query";
import { createTodo, deleteTodo, updateTodo } from "./api";

export function useCreateTodo() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data) => createTodo(data),
    onMutate: () => {
      console.log("Mutate");
    },
    onError: () => {
      console.log("error");
    },
    onSuccess: () => {
      console.log("success");
    },
    onSettled: async (data, error) => {
      console.log("settle");
      if (error) {
        console.log(error);
      } else {
        // await queryClient.invalidateQueries({ queryKey: ["todos"] });
        await queryClient.setQueryData(["todos"], (old) => [...old, data]);
      }
    },
  });
}

export function useUpdateTodo() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data) => updateTodo(data),
    onSettled: async (_, error, variables) => {
      if (error) {
        console.log(error);
      } else {
        await queryClient.invalidateQueries({ queryKey: ["todos"] });
        await queryClient.invalidateQueries({
          queryKey: ["todo", { id: variables.id }],
        });
      }
    },
  });
}

export function useDeleteTodo() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id) => deleteTodo(id),
    onSuccess: () => {
      console.log("delete successfully");
    },
    onSettled: async (_, error) => {
      if (error) {
        console.log(error);
      } else {
        await queryClient.invalidateQueries({ queryKey: ["todos"] });
      }
    },
  });
}
