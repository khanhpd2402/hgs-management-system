import { axiosInstance } from "../axios";

export const addGradeBatch = async (data) => {
  return (await axiosInstance.post(`GradeBatch`, data)).data;
};
