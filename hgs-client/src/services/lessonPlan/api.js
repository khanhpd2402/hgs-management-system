import { axiosInstance } from "../axios";

export const createLessonPlan = async (data) => {
  return await axiosInstance.post("/LessonPlan/create", data);
};
