import { axiosInstance } from "../axios";

export const getStudents = async (
  page = 1,
  limit = 5,
  grade = "",
  classname = "",
  searchValue = "",
) => {
  // Tạo query string cho bộ lọc
  const filterParams = [];
  if (grade) filterParams.push(`grade=${grade}`);
  if (classname) filterParams.push(`class=${classname}`);

  const formattedSearchValue = searchValue;

  const queryString = filterParams.length ? `&${filterParams.join("&")}` : "";
  console.log(
    `students?_limit=${limit}&_page=${page}&q=${formattedSearchValue}${queryString}`,
  );

  return (
    await axiosInstance.get(
      `students?_limit=${limit}&_page=${page}&q=${formattedSearchValue}${queryString}`,
    )
  ).data;
};
