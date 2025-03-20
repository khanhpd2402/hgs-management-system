import { useForm } from "react-hook-form";
import {
  useCreateTodo,
  useDeleteTodo,
  useUpdateTodo,
} from "../services/product/mutation";
import { useTodos } from "../services/product/queries";

const Todos = () => {
  //   const todoIdsQuery = useTodoIds();
  const todosQueries = useTodos();
  const createTodoMutation = useCreateTodo();
  const updateTodoMutation = useUpdateTodo();
  const deleteTodoMutation = useDeleteTodo();

  const { register, handleSubmit } = useForm();

  const handleCreateTodoSubmit = (data) => {
    createTodoMutation.mutate(data);
  };

  const handlearkAsDoneSubmit = (data) => {
    if (data) {
      updateTodoMutation.mutate({ ...data, checked: !data.checked });
    }
  };

  const handleDeleteTodoSubmit = async (id) => {
    await deleteTodoMutation.mutateAsync(id);
    console.log("success");
  };

  return (
    <>
      <form onSubmit={handleSubmit(handleCreateTodoSubmit)}>
        <h4>New Todo:</h4>
        <input {...register("title")} placeholder="Title" className="" />
        <br />
        <input {...register("description")} placeholder="description" />
        <br />
        <input
          type="submit"
          disabled={createTodoMutation.isPending}
          value={createTodoMutation.isPending ? "Creating..." : "Create Todo"}
        />
      </form>
      <ul>
        {todosQueries?.data &&
          todosQueries?.data?.map((data) => (
            <li key={data?.id}>
              Id: {data?.id} <br />
              <span>
                <strong>Title: </strong> {data?.title} <br />
                <strong>Description: </strong> {data?.description}
              </span>
              <div>
                <button
                  onClick={() => handlearkAsDoneSubmit(data)}
                  disabled={
                    updateTodoMutation.isPending &&
                    updateTodoMutation.variables.id === data.id
                  }
                >
                  {data?.checked ? "Done" : "Mark as done"}
                </button>
                {data && data.id && (
                  <button onClick={() => handleDeleteTodoSubmit(data.id)}>
                    Delete
                  </button>
                )}
              </div>
            </li>
          ))}
      </ul>
    </>
  );
};

export default Todos;
