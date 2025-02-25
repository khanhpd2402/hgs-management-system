import { axiosInstance } from "../axios";

export const getEmployees = async (
  page = 1,
  limit = 5,
  sort = "",
  order = "asc",
  search = "",
) => {
  return (
    await axiosInstance.get(
      `Employees?_limit=${limit}&_page=${page}&_sort=${sort}&_order=${order}&q=${search}`,
    )
  ).data;
};
