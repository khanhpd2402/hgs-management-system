import { axiosInstance } from "../axios";
import axios from "axios";

export const sendNotification = async (data) => {
  try {
    const response = await axiosInstance.post("/Notification/send", data);
    return response.data;
  } catch (error) {
    console.error("Lỗi khi gửi thông báo:", error);
    throw error;
  }
};
