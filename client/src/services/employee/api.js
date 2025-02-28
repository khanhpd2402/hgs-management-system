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
      `Employees?_limit=${limit}&_page=${page}&q=${encodeURIComponent(searchValue)}${queryString}`,
    )
  ).data;
};

// Sử dụng hàm với hai trường lọc
// getEmployees(1, 5, "IT", "Full-time", "John Doe");
