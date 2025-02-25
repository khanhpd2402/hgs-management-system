import { useState } from "react";
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
import PaginationControls from "@/components/PaginationControls"; // Import component mới

export default function EmployeeTable() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(5); // Chọn số lượng hiển thị mỗi trang

  const { data, isPending, error, isError } = useEmployees(page, pageSize);

  // Tính toán phạm vi hiển thị dữ liệu
  const startIndex = (page - 1) * pageSize + 1;
  const endIndex = (page - 1) * pageSize + data?.length;

  return (
    <>
      {isPending ? (
        <div>Loading...</div>
      ) : isError ? (
        <div>Error {error.message}</div>
      ) : (
        <>
          <EmployeeTableHeader type="employees" />
          <Card className="p-4">
            <Table className="w-full table-fixed border border-gray-300">
              <TableHeader className="bg-gray-100">
                <TableRow>
                  <TableHead className="w-12 border border-gray-300 text-center">
                    <Checkbox />
                  </TableHead>
                  <TableHead className="w-20 border border-gray-300 text-center">
                    Thao tác
                  </TableHead>
                  <TableHead className="w-16 border border-gray-300 text-center">
                    ID
                  </TableHead>
                  <TableHead className="w-48 border border-gray-300 text-left">
                    Họ tên cán bộ
                  </TableHead>
                  <TableHead className="w-36 border border-gray-300 text-left">
                    Mã cán bộ
                  </TableHead>
                  <TableHead className="w-36 border border-gray-300 text-left">
                    Số ĐTDD
                  </TableHead>
                  <TableHead className="w-48 border border-gray-300 text-left">
                    Địa chỉ Email
                  </TableHead>
                  <TableHead className="w-32 border border-gray-300 text-center">
                    Trạng thái
                  </TableHead>
                  <TableHead className="w-32 border border-gray-300 text-left">
                    Ngày sinh
                  </TableHead>
                  <TableHead className="w-20 border border-gray-300 text-left">
                    Giới tính
                  </TableHead>
                  <TableHead className="w-16 border border-gray-300 text-left">
                    Dân tộc
                  </TableHead>
                  <TableHead className="w-24 border border-gray-300 text-left">
                    Chức vụ
                  </TableHead>
                  <TableHead className="w-32 border border-gray-300 text-left">
                    Tổ bộ môn
                  </TableHead>
                  <TableHead className="w-48 border border-gray-300 text-left">
                    Hình thức hợp đồng
                  </TableHead>
                  <TableHead className="w-32 border border-gray-300 text-left">
                    Vị trí làm việc
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {data?.map((employee) => (
                  <TableRow
                    key={employee.id}
                    className="divide-x divide-gray-300"
                  >
                    <TableCell className="w-12 border border-gray-300 text-center">
                      <Checkbox />
                    </TableCell>
                    <TableCell className="w-20 border border-gray-300 text-center">
                      <Button variant="outline" size="icon">
                        <Settings className="h-4 w-4" />
                      </Button>
                    </TableCell>
                    <TableCell className="w-16 border border-gray-300 text-center">
                      {employee.id}
                    </TableCell>
                    <TableCell className="w-48 border border-gray-300 text-left break-words">
                      {employee.name}
                    </TableCell>
                    <TableCell className="w-36 border border-gray-300 text-left break-words">
                      {employee.code}
                    </TableCell>
                    <TableCell className="w-36 border border-gray-300 text-left">
                      {employee.phone}
                    </TableCell>
                    <TableCell className="w-48 border border-gray-300 text-left break-words">
                      {employee.email}
                    </TableCell>
                    <TableCell className="w-32 border border-gray-300 text-center">
                      {employee.status}
                    </TableCell>
                    <TableCell className="w-32 border border-gray-300 text-left">
                      {employee.dob}
                    </TableCell>
                    <TableCell className="w-20 border border-gray-300 text-left">
                      {employee.gender}
                    </TableCell>
                    <TableCell className="w-16 border border-gray-300 text-left">
                      {employee.ethnicity}
                    </TableCell>
                    <TableCell className="w-24 border border-gray-300 text-left">
                      {employee.position}
                    </TableCell>
                    <TableCell className="w-32 border border-gray-300 text-left break-words">
                      {employee.department}
                    </TableCell>
                    <TableCell className="w-48 border border-gray-300 text-left break-words">
                      {employee.contractType}
                    </TableCell>
                    <TableCell className="w-32 border border-gray-300 text-left break-words">
                      {employee.workLocation}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>

            {/* Chọn số lượng hiển thị + Tổng số item + Phân trang */}
            <div className="mt-4 flex items-center justify-between">
              <PaginationControls
                pageSize={pageSize}
                setPageSize={setPageSize}
                totalItems={data.length}
                startIndex={startIndex}
                endIndex={endIndex}
              />

              <MyPagination
                totalPages={6}
                currentPage={page}
                onPageChange={setPage}
              />
            </div>
          </Card>
        </>
      )}
    </>
  );
}
