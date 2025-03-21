import { axiosInstance } from "../axios";

export const getSubjects = async () => {
  const response = await axiosInstance.get("/subjects");
  return response.data;
};

export const getAcademicYears = async () => {
  const response = await axiosInstance.get("/AcademicYear");
  return response.data;
};

export const getSemestersByAcademicYear = async (academicYearId) => {
  const response = await axiosInstance.get(`/semester/${academicYearId}`);
  return response.data;
};
