import { axiosInstance } from "../axios";

export const addGradeBatch = async (data) => {
  return (await axiosInstance.post(`GradeBatch`, data)).data;
};

export const getGradeBatches = async () => {
  return (await axiosInstance.get(`GradeBatch`)).data;
};
