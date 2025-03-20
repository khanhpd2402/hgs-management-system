import { axiosInstance } from "../axios";

export const getStudents = async (
  page = 1,
  limit = 5,
  grade = "",
  classname = "",
  searchValue = "",
) => {
  // Tạo query string cho bộ lọc
  const filterParams = [];
  if (grade) filterParams.push(`grade=${grade}`);
  if (classname) filterParams.push(`class=${classname}`);

  return (await axiosInstance.get(`student?page=${page}`)).data;
};

export const getStudent = async (id) => {
  return (await axiosInstance.get(`student/${id}`)).data;
};

export const updateStudent = async (id, data) => {
  return (await axiosInstance.put(`student/${id}`, data)).data;
};
