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
  return (await axiosInstance.post(`user/${userId}/change-status`, { status }))
    .data;
};

export const resetUserPassword = async (userId, newPassword) => {
  return await axiosInstance.post(`user/${userId}/admin-change-password`, {
    newPassword,
  });
};

export const createSubject = async (data) => {
  return (await axiosInstance.post(`Subjects`, data)).data;
};

export const configueSubject = async (data) => {
  return (await axiosInstance.post(`GradeLevelSubjects`, data)).data;
};

export const getSubjectDetails = async (id) => {
  console.log(id);
  return (await axiosInstance.get(`GradeLevelSubjects/subject/${id}`)).data;
};
