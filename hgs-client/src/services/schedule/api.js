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
