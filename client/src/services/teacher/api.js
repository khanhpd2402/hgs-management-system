import { axiosInstance } from "../axios";

export const getEmployees = async (
  page = 1,
  limit = 5,
  department = "",
  contract = "",
  searchValue = "",
) => {
  // Tạo query string cho bộ lọc
  const filterParams = [];
  if (department)
    filterParams.push(`department=${encodeURIComponent(department)}`);
  if (contract) filterParams.push(`contract=${encodeURIComponent(contract)}`);

  const queryString = filterParams.length ? `&${filterParams.join("&")}` : "";

  console.log(queryString);

  return (
    await axiosInstance.get(
      `teachers?_limit=${limit}&_page=${page}&q=${encodeURIComponent(searchValue)}${queryString}`,
    )
  ).data;
};

export const getTeachingAssignments = async (
  page,
  limit,
  department,
  teacher,
  semester,
) => {
  const filterParams = [];
  if (department)
    filterParams.push(`department=${encodeURIComponent(department)}`);
  if (teacher) filterParams.push(`teacher=${encodeURIComponent(teacher)}`);
  if (semester) filterParams.push(`semester=${encodeURIComponent(semester)}`);

  const queryString = filterParams.length ? `&${filterParams.join("&")}` : "";
  return (
    await axiosInstance.get(
      `teaching-assignment?_limit=${limit}&_page=${page}${queryString}`,
    )
  ).data;
};

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
