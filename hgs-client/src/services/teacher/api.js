import { axiosInstance } from "../axios";

export const getTeachers = async () => {
  // Gọi API với axiosInstance
  return (await axiosInstance.get(`teachers`)).data;
};

export const getTeacher = async (id) => {
  return (await axiosInstance.get(`teachers/${id}`)).data;
};

export const updateTeacher = async (id, data) => {
  return (await axiosInstance.put(`teachers/${id}`, data)).data;
};

export const createTeacher = async (data) => {
  return (await axiosInstance.post(`teachers`, data)).data;
};

export const deleteTeacher = async (id) => {
  return (await axiosInstance.delete(`teachers/${id}`)).data;
};

// export const getTeachingAssignments = async (
// ) => {

//   return (
//     await axiosInstance.get(
//       `teaching-assignment?_limit=${limit}&_page=${page}${queryString}`,
//     )
//   ).data;
// };

export const getHeadTeacherAssignments = async (page, limit, grade) => {
  // const filterParams = [];
  // if (grade) filterParams.push(`grade=${encodeURIComponent(grade)}`);

  // const queryString = filterParams.length ? `&${filterParams.join("&")}` : "";
  const encodeGrade = encodeURIComponent(grade);
  return (
    await axiosInstance.get(
      `head-teacher-assignment?_limit=${limit}&_page=${page}&class_like=${encodeGrade}`,
    )
  ).data;
};

export const importTeachers = async (fileExcel) => {
  console.log(fileExcel);
  return await axiosInstance.post("teachers/import", fileExcel);
};
