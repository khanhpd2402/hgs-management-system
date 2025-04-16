import { axiosInstance } from "../axios";

export const getScheduleByTeacherId = async (teacherId) => {
    const response = await axiosInstance.get(`Timetables/teacher/${teacherId}`);
    return response.data;
};


