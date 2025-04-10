import { axiosInstance } from "../axios";

export const getLeaveRequestByTeacherId = async (teacherId) => {
    const token = JSON.parse(localStorage.getItem("token"));

    return (await axiosInstance.get(`LeaveRequest/by-teacher/${teacherId}`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    })).data;
};

export const createLeaveRequest = async (data) => {
    const token = JSON.parse(localStorage.getItem("token"));

    return (await axiosInstance.post(`LeaveRequest`, data, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    })).data;
};

export const getLeaveRequestByAdmin = async () => {
    const token = JSON.parse(localStorage.getItem("token"));

    return (await axiosInstance.get(`LeaveRequest`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    })).data;
};


