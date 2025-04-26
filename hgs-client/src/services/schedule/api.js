import { axiosInstance } from "../axios";

export const getScheduleByTeacherId = async (teacherId) => {
  const response = await axiosInstance.get(`Timetables/teacher/${teacherId}`);
  return response.data;
};

export const getScheduleByStudent = async ({ studentId, semesterId }) => {
  const response = await axiosInstance.get(
    `Timetables/student/${studentId}/semester/${semesterId}`,
  );
  return response.data;
};

export const getTimetableForPrincipal = async (timetableId) => {
  const response = await axiosInstance.get(
    `Timetables/TimetablesForPrincipal/${timetableId}`,
  );
  return response.data;
};

export const createSubstituteTeaching = async (payload) => {
  const response = await axiosInstance.post("SubstituteTeachings", payload);
  return response.data;
};

export const getSubstituteTeachings = async (
  timetableDetailId,
  originalTeacherId,
  date,
) => {
  const response = await axiosInstance.get(
    `SubstituteTeachings?timetableDetailId=${timetableDetailId}&OriginalTeacherId=${originalTeacherId}&date=${date}`,
  );
  return response.data;
};

export const getTimetiableSubstituteSubstituteForTeacher = async (
  teacherId,
  date,
) => {
  const response = await axiosInstance.get(
    `SubstituteTeachings?SubstituteTeacherId=${teacherId}&date=${date.format("YYYY-MM-DD")}`,
  );
  return response.data;
};
