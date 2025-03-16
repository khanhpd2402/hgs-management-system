import { axiosInstance } from "../axios";

export const getTeachers = async (
  page = 1,
  limit = 5,
  department = "",
  contract = "",
  searchValue = "",
) => {
  // Tạo mảng chứa các tham số OData
  const filterParams = [];

  if (department) {
    filterParams.push(`Department eq '${encodeURIComponent(department)}'`);
  }
  if (contract) {
    filterParams.push(`ContractType eq '${encodeURIComponent(contract)}'`);
  }
  if (searchValue) {
    filterParams.push(`contains(Name, '${encodeURIComponent(searchValue)}')`);
  }

  // Gộp các điều kiện lọc lại thành một chuỗi OData
  const filterQuery =
    filterParams.length > 0 ? `$filter=${filterParams.join(" and ")}` : "";

  // Tính toán `$skip` để phân trang
  const skip = (page - 1) * limit;

  // Tạo URL với OData query
  const queryString = [
    filterQuery,
    `$top=${limit}`,
    `$skip=${skip}`,
    `$orderby=fullName asc`, // Sắp xếp theo tên
  ]
    .filter(Boolean) // Loại bỏ giá trị rỗng
    .join("&");

  // Gọi API với axiosInstance
  return (await axiosInstance.get(`teachers?${queryString}`)).data;
};

export const getTeacher = async (id) => {
  return (await axiosInstance.get(`teachers/${id}`)).data;
};

export const updateTeacher = async (id, data) => {
  return (await axiosInstance.put(`teachers/${id}`, data)).data;
};

export const createTeacher = async (data) => {
  console.log(data);
  return (await axiosInstance.post(`teachers`, data)).data;
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
