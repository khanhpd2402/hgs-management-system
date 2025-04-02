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
import { useStudents } from "@/services/student/queries";
import StudentTableHeader from "./StudentTableHeader";
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
import { formatDateString } from "@/helpers/formatDate";

export default function StudentTable() {
  const queryClient = useQueryClient();
  const navigate = useNavigate();

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
    { id: "studentId", label: "ID", width: "80px" },
    { id: "fullName", label: "Họ và tên", width: "200px" },
    { id: "gender", label: "Giới tính", width: "120px" },
    { id: "ethnicity", label: "Dân tộc", width: "120px" },
    { id: "grade", label: "Khối", width: "100px" },
    { id: "class", label: "Lớp", width: "100px" },
    { id: "status", label: "Trạng thái", width: "150px" },
    { id: "dob", label: "Ngày sinh", width: "150px" },
  ];

  // Initialize with all columns visible
  const [visibleColumns, setVisibleColumns] = useState(
    allColumns.map((col) => ({ id: col.id, label: col.label })),
  );
  const academicYearId = JSON.parse(
    sessionStorage.getItem("currentAcademicYear"),
  ).academicYearID;

  const { data, isPending, error, isError } = useStudents(academicYearId);

  //phan trang
  const { page, pageSize, search } = filter;

  const filteredData =
    data?.students?.filter((teacher) => {
      // // Filter by department
      // if (department && teacher.department !== department) {
      //   return false;
      // }

      // // Filter by contract type
      // if (contract && teacher.employmentType !== contract) {
      //   return false;
      // }

      // Filter by search term (case insensitive)
      if (search) {
        const searchLower = search.toLowerCase();
        return (
          teacher.fullName?.toLowerCase().includes(searchLower) ||
          teacher.email?.toLowerCase().includes(searchLower) ||
          teacher.phoneNumber?.toLowerCase().includes(searchLower)
        );
      }

      return true;
    }) || [];

  const totalPages = Math.ceil(filteredData.length / pageSize);
  const currentData = filteredData.slice(
    (page - 1) * pageSize,
    page * pageSize,
  );

  const startIndex = filteredData.length === 0 ? 0 : (page - 1) * pageSize + 1;
  const endIndex = Math.min(page * pageSize, filteredData.length);

  // Save visible columns to localStorage
  useEffect(() => {
    localStorage.setItem(
      "studentTableVisibleColumns",
      JSON.stringify(visibleColumns),
    );
  }, [visibleColumns]);

  // Load visible columns from localStorage on component mount
  useEffect(() => {
    const savedColumns = localStorage.getItem("studentTableVisibleColumns");
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
        <p>{error.message || "Không thể tải dữ liệu học sinh"}</p>
        <button
          onClick={() => queryClient.invalidateQueries(["students"])}
          className="mt-2 rounded bg-blue-500 px-4 py-2 text-white hover:bg-blue-600"
        >
          Thử lại
        </button>
      </div>
    );
  }

  return (
    <Card className="p-4">
      <StudentTableHeader
        setFilter={setFilter}
        type="student"
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
                      visibleColumns.some((col) => col.id === column.id) && (
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
                {currentData && currentData.length > 0 ? (
                  currentData?.map((student) => (
                    <TableRow
                      key={student.studentId}
                      className="divide-x divide-gray-300"
                    >
                      {visibleColumns.some((col) => col.id === "actions") && (
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
                                    `/student/profile/${student.studentId}`,
                                  )
                                }
                              >
                                Xem hồ sơ
                              </DropdownMenuItem>
                            </DropdownMenuContent>
                          </DropdownMenu>
                        </TableCell>
                      )}

                      {visibleColumns.some((col) => col.id === "studentId") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {student.studentId}
                        </TableCell>
                      )}

                      {visibleColumns.some((col) => col.id === "fullName") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {student.fullName}
                        </TableCell>
                      )}

                      {visibleColumns.some((col) => col.id === "gender") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {student.gender}
                        </TableCell>
                      )}

                      {visibleColumns.some((col) => col.id === "ethnicity") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {student.ethnicity}
                        </TableCell>
                      )}

                      {visibleColumns.some((col) => col.id === "grade") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {student.grade}
                        </TableCell>
                      )}

                      {visibleColumns.some((col) => col.id === "class") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {student.className}
                        </TableCell>
                      )}

                      {visibleColumns.some((col) => col.id === "status") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {student.status}
                        </TableCell>
                      )}

                      {visibleColumns.some((col) => col.id === "dob") && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {formatDateString(student.dob)}
                        </TableCell>
                      )}

                      {visibleColumns.some(
                        (col) => col.id === "educationLevel",
                      ) && (
                        <TableCell className="h-16 border border-gray-300 text-center">
                          {student.educationLevel}
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
          totalItems={filteredData.length || 0}
          startIndex={startIndex}
          endIndex={endIndex}
        />

        <MyPagination
          totalPages={totalPages}
          currentPage={page}
          onPageChange={setFilter}
        />
      </div>
    </Card>
  );
}
