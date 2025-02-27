import { useState, useEffect } from "react";
import { Card } from "@/components/ui/card";
import {
  Table,
  TableHeader,
  TableRow,
  TableHead,
  TableBody,
  TableCell,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { Settings } from "lucide-react";
import { Checkbox } from "@/components/ui/checkbox";
import MyPagination from "@/components/MyPagination";
import { useEmployees } from "@/services/employee/queries";
import EmployeeTableHeader from "./EmployeeTableHeader";
import PaginationControls from "@/components/PaginationControls";
import { Spinner } from "@/components/Spinner";

export default function EmployeeTable() {
  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 5,
    sort: "",
    order: "asc",
    search: "",
  });

  const { data, isPending, error, isError, isFetching } = useEmployees(filter);
  const { page, pageSize } = filter;

  const [placeholderRows, setPlaceholderRows] = useState([]);

  useEffect(() => {
    setPlaceholderRows(Array(pageSize).fill(null));
  }, [pageSize]);

  const startIndex = (page - 1) * pageSize + 1;
  const endIndex = Math.min(
    (page - 1) * pageSize + (data?.length || 0),
    startIndex + pageSize - 1,
  );

  // Tính toán hàng hiển thị
  const displayRows = data || [];
  const emptyRowsCount = Math.max(0, pageSize - displayRows.length);
  const emptyRows = emptyRowsCount > 0 ? Array(emptyRowsCount).fill(null) : [];

  if (isPending) {
    return (
      <Card className="relative mt-6 flex min-h-[550px] items-center justify-center p-4">
        <Spinner size="medium" />
      </Card>
    );
  }

  if (isError) {
    return (
      <Card className="relative mt-6 flex min-h-[550px] items-center justify-center p-4">
        <div className="text-red-500">Lỗi khi tải dữ liệu</div>
      </Card>
    );
  }

  return (
    <Card className="relative mt-6 min-h-[550px] p-4">
      <EmployeeTableHeader type="employees" setFilter={setFilter} />

      {/* Container chính không có overflow-x-auto */}
      <div className="relative min-h-[400px]">
        {isFetching && (
          <div className="absolute inset-0 z-10 flex items-center justify-center bg-white/70">
            <Spinner />
          </div>
        )}

        {/* Container cho bảng với overflow-x-auto */}
        <div className="overflow-x-auto">
          <Table
            className="w-full border border-gray-300"
            style={{ minWidth: "1500px" }}
          >
            <TableHeader className="bg-gray-100">
              <TableRow>
                <TableHead className="w-12 border border-gray-300 p-0 text-center whitespace-nowrap">
                  <Checkbox />
                </TableHead>
                <TableHead className="border border-gray-300 text-center whitespace-nowrap">
                  Thao tác
                </TableHead>
                <TableHead className="w-12 border border-gray-300 text-center whitespace-nowrap">
                  ID
                </TableHead>
                <TableHead className="border border-gray-300 text-left whitespace-nowrap">
                  Họ tên cán bộ
                </TableHead>

                <TableHead className="border border-gray-300 text-left whitespace-nowrap">
                  Số ĐTDD
                </TableHead>
                <TableHead className="border border-gray-300 text-left whitespace-nowrap">
                  Địa chỉ Email
                </TableHead>
                <TableHead className="border border-gray-300 text-center whitespace-nowrap">
                  Trạng thái
                </TableHead>
                <TableHead className="border border-gray-300 text-left whitespace-nowrap">
                  Ngày sinh
                </TableHead>
                <TableHead className="border border-gray-300 text-left whitespace-nowrap">
                  Giới tính
                </TableHead>
                <TableHead className="border border-gray-300 text-left whitespace-nowrap">
                  Dân tộc
                </TableHead>
                <TableHead className="border border-gray-300 text-left whitespace-nowrap">
                  Chức vụ
                </TableHead>
                <TableHead className="border border-gray-300 text-left whitespace-nowrap">
                  Tổ bộ môn
                </TableHead>
                <TableHead className="border border-gray-300 text-left whitespace-nowrap">
                  Hình thức hợp đồng
                </TableHead>
                <TableHead className="border border-gray-300 text-left whitespace-nowrap">
                  Vị trí làm việc
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {/* Dữ liệu thực */}
              {displayRows.map((employee) => (
                <TableRow
                  key={employee.id}
                  className="divide-x divide-gray-300"
                >
                  <TableCell className="h-16 border border-gray-300 p-0 text-center whitespace-nowrap">
                    <Checkbox />
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-center whitespace-nowrap">
                    <Button variant="outline" size="icon">
                      <Settings className="h-4 w-4" />
                    </Button>
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-center whitespace-nowrap">
                    {employee.id}
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                    {employee.name}
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                    {employee.phone}
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                    {employee.email}
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-center whitespace-nowrap">
                    {employee.status}
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                    {employee.dob}
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                    {employee.gender}
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                    {employee.ethnicity}
                  </TableCell>
                  <TableCell className="h-16border border-gray-300 text-left whitespace-nowrap">
                    {employee.position}
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                    {employee.department}
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                    {employee.contractType}
                  </TableCell>
                  <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                    {employee.workLocation}
                  </TableCell>
                </TableRow>
              ))}

              {/* Thêm hàng trống để duy trì chiều cao */}
              {emptyRows.map((_, index) => (
                <TableRow
                  key={`empty-${index}`}
                  className="divide-x divide-gray-300"
                >
                  <TableCell
                    className="h-16 border border-gray-300"
                    colSpan={15}
                  ></TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>

        <div className="mt-4 flex items-center justify-between">
          <PaginationControls
            pageSize={pageSize}
            setFilter={setFilter}
            totalItems={data?.length || 0}
            startIndex={startIndex}
            endIndex={endIndex}
          />

          <MyPagination
            totalPages={6}
            currentPage={page}
            onPageChange={setFilter}
          />
        </div>
      </div>
    </Card>
  );
}
