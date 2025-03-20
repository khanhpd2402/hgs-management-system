import { axiosInstance } from "../axios";

export const getSubjects = async () => {
  const response = await axiosInstance.get("/subjects");
  return response.data;
};
