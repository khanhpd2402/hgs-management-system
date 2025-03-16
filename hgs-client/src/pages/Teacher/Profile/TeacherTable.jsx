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
import { DropdownMenu } from "@/components/ui/dropdown-menu";
import { useNavigate } from "react-router";
import ConfirmDeleteModal from "@/components/modal/ConfirmDeleteModal";
import { useDeleteTeacher } from "@/services/teacher/mutation";

export default function TeacherTable() {
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const [isModalOpen, setModalOpen] = useState(false);

  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 5,
    sort: "",
    order: "asc",
    search: "",
  });

  // Define all available columns
  const allColumns = [
    { id: "actions", label: "Thao tác", width: "100px" },
    { id: "fullName", label: "Họ tên cán bộ", width: "200px" },
    { id: "phone", label: "Số ĐTDD", width: "150px" },
    { id: "email", label: "Địa chỉ Email", width: "200px" },
    { id: "status", label: "Trạng thái", width: "120px" },
    { id: "dob", label: "Ngày sinh", width: "150px" },
    { id: "gender", label: "Giới tính", width: "120px" },
    { id: "ethnicity", label: "Dân tộc", width: "120px" },
    { id: "position", label: "Chức vụ", width: "150px" },
    { id: "department", label: "Tổ bộ môn", width: "150px" },
    { id: "employmentType", label: "Hình thức hợp đồng", width: "180px" },
    { id: "additionalDuties", label: "Vị trí làm việc", width: "180px" },
  ];

  // Initialize with all columns visible
  const [visibleColumns, setVisibleColumns] = useState(
    allColumns.map((col) => col.id),
  );

  const { data, isPending, error, isError } = useTeachers(filter);
  const teacherMutation = useDeleteTeacher();

  const { page, pageSize } = filter;

  const startIndex = (page - 1) * pageSize + 1;
  const endIndex = Math.min(
    (page - 1) * pageSize + (data?.length || 0),
    startIndex + pageSize - 1,
  );

  // Save visible columns to localStorage
  useEffect(() => {
    localStorage.setItem(
      "teacherTableVisibleColumns",
      JSON.stringify(visibleColumns),
    );
  }, [visibleColumns]);

  // Load visible columns from localStorage on component mount
  useEffect(() => {
    const savedColumns = localStorage.getItem("teacherTableVisibleColumns");
    if (savedColumns) {
      setVisibleColumns(JSON.parse(savedColumns));
    }
  }, []);

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
      <TeacherTableHeader
        setFilter={setFilter}
        type="teachers"
        setVisibleColumns={setVisibleColumns}
        visibleColumns={visibleColumns}
        columns={allColumns}
      />

      <div className="max-h-[400px] overflow-auto border border-gray-200">
        <div className="min-w-max">
          <div>
            <Table className="w-full border-collapse">
              <TableHeader className="bg-gray-100">
                <TableRow>
                  {allColumns.map(
                    (column) =>
                      visibleColumns.includes(column.id) && (
                        <TableHead
                          key={column.id}
                          className={`w-[${column.width}] border border-gray-300 text-center`}
                        >
                          {column.label}
                        </TableHead>
                      ),
                  )}
                </TableRow>
              </TableHeader>
              <TableBody>
                {data && data.length > 0 ? (
                  data.map((teacher) => (
                    <TableRow
                      key={teacher.teacherId}
                      className="divide-x divide-gray-300"
                    >
                      {visibleColumns.includes("actions") && (
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
                                  navigate(
                                    `/teacher/profile/${teacher.teacherId}`,
                                  )
                                }
                              >
                                Xem hồ sơ
                              </DropdownMenuItem>
                              <DropdownMenuItem
                                onClick={(e) => {
                                  e.stopPropagation();
                                  setModalOpen(true);
                                }}
                              >
                                Xóa
                              </DropdownMenuItem>
                            </DropdownMenuContent>
                          </DropdownMenu>
                          <ConfirmDeleteModal
                            open={isModalOpen}
                            onClose={() => setModalOpen(false)}
                            onConfirm={() => {
                              teacherMutation.mutate(teacher.teacherId);
                              setModalOpen(false);
                            }}
                            title="Xác nhận xóa"
                            description={`Bạn có chắc chắn muốn xóa giáo viên ${teacher.fullName}?`}
                          />
                        </TableCell>
                      )}

                      {visibleColumns.includes("fullName") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {teacher.fullName}
                        </TableCell>
                      )}

                      {visibleColumns.includes("phone") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {teacher.phone}
                        </TableCell>
                      )}

                      {visibleColumns.includes("email") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {teacher.email}
                        </TableCell>
                      )}

                      {visibleColumns.includes("status") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {teacher.status}
                        </TableCell>
                      )}

                      {visibleColumns.includes("dob") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {teacher.dob}
                        </TableCell>
                      )}

                      {visibleColumns.includes("gender") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {teacher.gender}
                        </TableCell>
                      )}

                      {visibleColumns.includes("ethnicity") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {teacher.ethnicity}
                        </TableCell>
                      )}

                      {visibleColumns.includes("position") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {teacher.position}
                        </TableCell>
                      )}

                      {visibleColumns.includes("department") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {teacher.department}
                        </TableCell>
                      )}

                      {visibleColumns.includes("employmentType") && (
                        <TableCell className="h-16 border border-gray-300 text-left">
                          {teacher.employmentType}
                        </TableCell>
                      )}

                      {visibleColumns.includes("additionalDuties") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {teacher.additionalDuties}
                        </TableCell>
                      )}
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell
                      colSpan={visibleColumns.length}
                      className="p-4 text-center"
                    >
                      Không có dữ liệu
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </div>
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
