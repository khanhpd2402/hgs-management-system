import { axiosInstance } from "../axios";

export const createLessonPlan = async (data) => {
  return (
    await axiosInstance.post("/api/LessonPlan/create", data)
  ).data;
};