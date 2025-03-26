import { axiosInstance } from "../axios";

export const addGradeBatch = async (data) => {
  return (await axiosInstance.post(`GradeBatch`, data)).data;
};

export const getGradeBatches = async () => {
  return (await axiosInstance.get(`GradeBatch`)).data;
};

export const getGradeBatch = async (id) => {
  return (await axiosInstance.get(`GradeBatch/${id}`)).data;
};

export const getUsers = async () => {
  const token = JSON.parse(localStorage.getItem("token"));

  return (
    await axiosInstance.get(`user`, {
      headers: { Authorization: `Bearer ${token}` },
    })
  ).data;
};

export const changeUserStatus = async (userId, status) => {
  const token = JSON.parse(localStorage.getItem("token"));

  return (
    await axiosInstance.post(
      `user/${userId}/change-status`,
      { status },
      {
        headers: { Authorization: `Bearer ${token}` },
      },
    )
  ).data;
};
