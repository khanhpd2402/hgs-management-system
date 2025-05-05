import { axiosInstance } from "../axios";

export const getStudents = async (academicId) => {
  // Tạo query string cho bộ lọc

  return (await axiosInstance.get(`student/${academicId}`)).data;
};

export const getStudent = async (id, academicId) => {
  return (await axiosInstance.get(`student/${id}/${academicId}`)).data;
};

export const updateStudent = async (id, data) => {
  return (await axiosInstance.put(`student/${id}`, data)).data;
};

export const createStudent = async (data) => {
  return (await axiosInstance.post(`student`, data)).data;
};

export const getStudentAttendance = async (studentId, weekStart) => {
  return (
    await axiosInstance.get(
      `Attendance/student/${studentId}/week?weekStart=${weekStart}`,
    )
  ).data;
};

export const getStudentById = async (studentId, academicId) => {
  return (await axiosInstance.get(`student/${studentId}/${academicId}`)).data;
};
