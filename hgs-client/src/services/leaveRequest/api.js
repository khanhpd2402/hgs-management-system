import { axiosInstance } from "../axios";

export const getLeaveRequests = async () => {
  return (await axiosInstance.get(`LeaveRequest`)).data;
};

export const createLeaveRequest = async (data) => {
  return (await axiosInstance.post(`LeaveRequest`, data)).data;
};

export const getLeaveRequest = async (id) => {
  return (await axiosInstance.get(`LeaveRequest/${id}`)).data;
};

export const getLeaveRequestByTeacherId = async (teacherId) => {
    const token = JSON.parse(localStorage.getItem("token"));

    return (await axiosInstance.get(`LeaveRequest/by-teacher/${teacherId}`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    })).data;
};


