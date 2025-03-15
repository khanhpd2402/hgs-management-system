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
import { useStudents } from "@/services/student/queries";
import StudentTableHeader from "./StudentTableHeader";
import PaginationControls from "@/components/PaginationControls"; // Import component mới
import { Spinner } from "@/components/Spinner";

export default function StudentTable() {
  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 5,
    grade: "",
    class: "asc",
    searchValue: "",
  }); // Chọn số lượng hiển thị mặt trang

  const { data, isPending, error, isError, isFetching } = useStudents(filter);
  console.log(data);

  const { page, pageSize } = filter;

  const startIndex = (page - 1) * pageSize + 1;
  const endIndex = (page - 1) * pageSize + data?.length;

  if (isPending) return <Spinner size="medium" />;
  if (isError) return "Error";

  return (
    <>
      <Card className="relative mt-6 p-4">
        <StudentTableHeader type="student" setFilter={setFilter} />
        <div className="max-h-[500px] overflow-x-auto">
          <div className="min-w-max">
            <Table className="w-full border border-gray-300">
              <TableHeader className="bg-gray-100">
                <TableRow>
                  <TableHead className="w-10 border border-gray-300 p-0 text-center">
                    <Checkbox />
                  </TableHead>
                  <TableHead className="w-10 border border-gray-300 text-center">
                    Thao tác
                  </TableHead>
                  <TableHead className="w-12 border border-gray-300 text-center">
                    ID
                  </TableHead>
                  <TableHead className="w-40 border border-gray-300 text-center">
                    Họ và tên
                  </TableHead>
                  <TableHead className="w-20 border border-gray-300 text-center">
                    GIới tính
                  </TableHead>
                  <TableHead className="w-16 border border-gray-300 text-center">
                    Dân tộc
                  </TableHead>
                  <TableHead className="w-16 border border-gray-300 text-center">
                    Khối
                  </TableHead>
                  <TableHead className="w-16 border border-gray-300 text-center">
                    Lớp
                  </TableHead>
                  <TableHead className="w-28 border border-gray-300 text-center">
                    Trạng thái
                  </TableHead>
                  <TableHead className="w-28 border border-gray-300 text-center">
                    Ngày sinh
                  </TableHead>
                  <TableHead className="w-32 border border-gray-300 text-center">
                    Cấp học
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {data &&
                  data?.map((student) => (
                    <TableRow
                      key={student.id}
                      className="divide-x divide-gray-300"
                    >
                      <TableCell className="h-16 border border-gray-300 p-0 text-center">
                        <Checkbox />
                      </TableCell>
                      <TableCell className="h-16 border border-gray-300 text-center">
                        <Button variant="outline" size="icon">
                          <Settings className="h-4 w-4" />
                        </Button>
                      </TableCell>
                      <TableCell className="h-16 border border-gray-300 text-center">
                        {student.id}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-300 text-center">
                        {student.fullName}
                      </TableCell>

                      <TableCell className="h-16 border border-gray-300 text-center">
                        {student.gender}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-300 text-center">
                        {student.ethnicity}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-300 text-center">
                        {student.grade}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-300 text-center">
                        {student.class}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-300 text-center">
                        {student.status}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-300 text-center">
                        {student.dob}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-300 text-center">
                        {student.educationLevel}
                      </TableCell>
                    </TableRow>
                  ))}
              </TableBody>
            </Table>
          </div>
          <div className="mt-4 flex items-center justify-between">
            <PaginationControls
              pageSize={pageSize}
              setFilter={setFilter}
              totalItems={data.length}
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
    </>
  );
}
