import { axiosInstance } from "../axios";

export const getEmployees = async (page = 1, limit = 5) => {
  return (await axiosInstance.get(`Employees?_limit=${limit}&_page=${page}`))
    .data;
};
