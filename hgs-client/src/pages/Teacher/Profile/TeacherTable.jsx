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
import {
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DropdownMenu } from "@radix-ui/react-dropdown-menu";
import { useNavigate } from "react-router";

export default function TeacherTable() {
  const queryClient = useQueryClient();
  const navigate = useNavigate();

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
    <Card className="p-4">
      <TeacherTableHeader setFilter={setFilter} type="teachers" />

      {/* Container chính với overflow-auto để cuộn ngang */}
      <div className="max-h-[400px] overflow-auto border border-gray-200">
        {/* Bảng sẽ có chiều rộng cố định lớn để hiển thị đầy đủ nội dung */}
        <div>
          <Table className="w-full border-collapse">
            <TableHeader className="bg-gray-100">
              <TableRow>
                <TableHead className="w-[100px] border border-gray-300 p-0 text-center">
                  <Checkbox />
                </TableHead>
                <TableHead className="w-[100px] border border-gray-300 text-center">
                  Thao tác
                </TableHead>

                <TableHead className="w-[200px] border border-gray-300 text-center">
                  Họ tên cán bộ
                </TableHead>
                <TableHead className="w-[150px] border border-gray-300 text-center">
                  Số ĐTDD
                </TableHead>
                <TableHead className="w-[200px] border border-gray-300 text-center">
                  Địa chỉ Email
                </TableHead>
                <TableHead className="w-[120px] border border-gray-300 text-center">
                  Trạng thái
                </TableHead>
                <TableHead className="w-[150px] border border-gray-300 text-center">
                  Ngày sinh
                </TableHead>
                <TableHead className="w-[120px] border border-gray-300 text-center">
                  Giới tính
                </TableHead>
                <TableHead className="w-[120px] border border-gray-300 text-center">
                  Dân tộc
                </TableHead>
                <TableHead className="w-[150px] border border-gray-300 text-center">
                  Chức vụ
                </TableHead>
                <TableHead className="w-[150px] border border-gray-300 text-center">
                  Tổ bộ môn
                </TableHead>
                <TableHead className="w-[180px] border border-gray-300 text-center">
                  Hình thức hợp đồng
                </TableHead>
                <TableHead className="w-[180px] border border-gray-300 text-center">
                  Vị trí làm việc
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {data.length > 0 ? (
                data.map((teacher) => (
                  <TableRow
                    key={teacher.teacherId}
                    className="divide-x divide-gray-300"
                  >
                    <TableCell className="h-16 border border-gray-300 p-0 text-center">
                      <Checkbox />
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-center">
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button variant="outline" size="icon">
                            <Settings className="h-4 w-4" />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                          <DropdownMenuItem
                            onClick={() =>
                              navigate(`/teacher/profile/${teacher.teacherId}`)
                            }
                          >
                            Xem hồ sơ
                          </DropdownMenuItem>
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </TableCell>

                    <TableCell className="h-16 border border-gray-300 text-center">
                      {teacher.fullName}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-center">
                      {teacher.phone}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-center">
                      {teacher.email}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-center">
                      {teacher.status}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-center">
                      {teacher.dob}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-center">
                      {teacher.gender}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-center">
                      {teacher.ethnicity}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-center">
                      {teacher.position}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-center">
                      {teacher.department}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-left">
                      {teacher.employmentType}
                    </TableCell>
                    <TableCell className="h-16 border border-gray-300 text-center">
                      {teacher.additionalDuties}
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={14} className="p-4 text-center">
                    Không có dữ liệu
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      </div>
      <div className="mt-4 flex flex-col items-center justify-between gap-4 sm:flex-row">
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
