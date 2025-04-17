import { axiosInstance } from "../axios";

export const getScheduleByTeacherId = async (teacherId) => {
    const response = await axiosInstance.get(`Timetables/teacher/${teacherId}`);
    return response.data;
};


export const getScheduleByStudent = async ({ studentId, semesterId }) => {
    const response = await axiosInstance.get(`Timetables/student/${studentId}/semester/${semesterId}`);
    return response.data;
};