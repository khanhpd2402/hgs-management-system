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

export const getSubjectDetail = async (id) => {
  return (await axiosInstance.get(`Subjects/${id}`)).data;
};

export const getSubjectConfigues = async () => {
  return (await axiosInstance.get(`GradeLevelSubjects`)).data;
};

export const getSubjectConfigueDetail = async (id) => {
  return (await axiosInstance.get(`GradeLevelSubjects/subject/${id}`)).data;
};

export const updateSubject = async (id, data) => {
  return (await axiosInstance.put(`Subjects/${id}`, data)).data;
};

export const updateSubjectConfigue = async (id, data) => {
  return (await axiosInstance.put(`GradeLevelSubjects/${id}`, data)).data;
};

export const deleteSubject = async (id) => {
  return (await axiosInstance.delete(`Subjects/${id}`)).data;
};

export const deleteSubjectConfigue = async (id) => {
  return (await axiosInstance.delete(`GradeLevelSubjects/${id}`)).data;
};

//teaching assignment

export const getTeacherSubjects = async () => {
  return (await axiosInstance.get(`TeachingAssignment/filter-data`)).data;
};

export const getTeachingAssignments = async () => {
  return (await axiosInstance.get(`TeachingAssignment/all`)).data;
};

export const assignTeaching = async (data) => {
  return (await axiosInstance.post(`TeachingAssignment/create`, data)).data;
};
