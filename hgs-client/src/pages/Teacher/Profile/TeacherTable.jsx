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
import { useTeachers } from "@/services/teacher/queries";
import TeacherTableHeader from "./TeacherTableHeader";
import PaginationControls from "@/components/PaginationControls";
import { Spinner } from "@/components/Spinner";
import { useQueryClient } from "@tanstack/react-query";

export default function TeacherTable() {
  const queryClient = useQueryClient();

  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 5,
    sort: "",
    order: "asc",
    search: "",
  });

  const { data, isPending, error, isError, isFetching } = useTeachers(filter);

  console.log(data);

  const { page, pageSize } = filter;

  const startIndex = (page - 1) * pageSize + 1;
  const endIndex = Math.min(
    (page - 1) * pageSize + (data?.length || 0),
    startIndex + pageSize - 1,
  );

  // Tính toán hàng hiển thị
  if (isPending) {
    return (
      <Card className="relative mt-6 flex min-h-[550px] items-center justify-center p-4">
        <Spinner size="medium" />
      </Card>
    );
  }

  if (isError) {
    return (
      <div className="rounded border border-red-300 bg-red-50 p-4 text-red-500">
        <h3 className="font-bold">Đã xảy ra lỗi:</h3>
        <p>{error.message || "Không thể tải dữ liệu nhân viên"}</p>
        <button
          onClick={() => queryClient.invalidateQueries(["employees"])}
          className="mt-2 rounded bg-blue-500 px-4 py-2 text-white hover:bg-blue-600"
        >
          Thử lại
        </button>
      </div>
    );
  }

  return (
    <Card className="relative mt-6 p-4">
      <TeacherTableHeader setFilter={setFilter} type="teachers" />

      {/* Container chính không có overflow-x-auto */}
      <div className="max-h-[400px] overflow-auto">
        {isFetching && (
          <div className="absolute inset-0 z-10 flex items-center justify-center bg-white/70">
            <Spinner />
          </div>
        )}

        {/* Container cho bảng với overflow-x-auto */}
        <div className="min-w-max">
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
              {data.length > 0 ? (
                data.map((teacher) => (
                  <TableRow
                    key={teacher.id}
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
                      {teacher.id}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                      {teacher.name}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                      {teacher.phone}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                      {teacher.email}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-center whitespace-nowrap">
                      {teacher.status}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                      {teacher.dob}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                      {teacher.gender}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                      {teacher.ethnicity}
                    </TableCell>
                    <TableCell className="h-16border border-gray-300 text-left whitespace-nowrap">
                      {teacher.position}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                      {teacher.department}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                      {teacher.employmentType}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                      {teacher.additionalDuties}
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={14} className="text-xl">
                    Không có dữ liệu
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
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
    </Card>
  );
}
