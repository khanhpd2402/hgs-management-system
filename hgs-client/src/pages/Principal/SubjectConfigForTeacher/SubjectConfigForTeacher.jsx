import { useState } from "react";
import {
  Table,
  TableHeader,
  TableBody,
  TableRow,
  TableHead,
  TableCell,
} from "@/components/ui/table";
import { useTeacherSubjects } from "@/services/principal/queries";
import { Button } from "@/components/ui/button";
import { Settings } from "lucide-react";
import UpdateTeacherSubjectModal from "./UpdateTeacherSubjectModal";
import PaginationControls from "@/components/PaginationControls";
import MyPagination from "@/components/MyPagination";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

export default function SubjectConfigForTeacher() {
  const [openUpdateModal, setOpenUpdateModal] = useState(false);
  const [selectedTeacher, setSelectedTeacher] = useState(null);

  const teacherSubjectQuery = useTeacherSubjects();
  const teachers = teacherSubjectQuery.data?.teachers || [];

  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 5,
    teacher: null,
  });

  const filteredData = filter.teacher
    ? teachers.filter((t) => t.teacherId === Number(filter.teacher))
    : teachers;

  const { page, pageSize } = filter;

  const totalPages = Math.ceil(filteredData?.length / pageSize);
  const currentData = filteredData.slice(
    (page - 1) * pageSize,
    page * pageSize,
  );

  const startIndex = filteredData.length === 0 ? 0 : (page - 1) * pageSize + 1;
  const endIndex = Math.min(page * pageSize, filteredData.length);

  return (
    <div className="mt-6">
      <div className="flex items-center justify-between">
        <h2 className="mb-6 text-2xl font-bold">
          Cấu hình môn học cho giáo viên
        </h2>
        <Select
          value={filter.teacher ?? ""}
          onValueChange={(value) =>
            setFilter((prev) => ({
              ...prev,
              teacher: value === "all" ? null : value,
              page: 1, // reset to first page when filter changes
            }))
          }
        >
          <SelectTrigger className="w-56">
            <SelectValue placeholder="Lọc theo giáo viên" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">Tất cả</SelectItem>
            {teachers.map((teacher) => (
              <SelectItem
                value={teacher.teacherId + ""}
                key={teacher.teacherId}
              >
                {teacher.fullName}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="mt-2 text-sm text-gray-600">
        <span>
          Môn học có dấu
          <span className="text-red-500"> *</span> là môn dạy chính
        </span>
      </div>
      <div className="max-h-[400px] overflow-auto">
        <Table className="mt-2 w-full rounded-lg bg-white shadow">
          <TableHeader>
            <TableRow>
              <TableHead className="w-16">STT</TableHead>
              <TableHead className="w-96">Giáo viên</TableHead>
              <TableHead>Môn học có thể dạy</TableHead>
              <TableHead className="w-32">Thao tác</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {currentData.map((teacher, idx) => (
              <TableRow key={teacher.teacherId}>
                <TableCell>{idx + 1}</TableCell>
                <TableCell>{teacher.fullName}</TableCell>
                <TableCell>
                  {teacher?.subjects
                    ?.slice() // create a shallow copy to avoid mutating original data
                    .sort(
                      (a, b) =>
                        (b.isMainSubject ? 1 : 0) - (a.isMainSubject ? 1 : 0),
                    )
                    .map((t, i, arr) => (
                      <span key={t.subjectId || i}>
                        {t.subjectName}
                        {t.isMainSubject && (
                          <span className="text-red-500">*</span>
                        )}
                        {i < arr.length - 1 && ", "}
                      </span>
                    ))}
                </TableCell>
                <TableCell>
                  <Button
                    variant="outline"
                    size="icon"
                    onClick={() => {
                      setOpenUpdateModal(true);
                      setSelectedTeacher(teacher.teacherId);
                    }}
                  >
                    <Settings className="h-4 w-4" />
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      <UpdateTeacherSubjectModal
        open={openUpdateModal}
        onClose={(open) => {
          setOpenUpdateModal(open);
          if (!open) setSelectedTeacher(null);
        }}
        teacherId={selectedTeacher}
      />
      <div className="mt-4 flex flex-col items-center justify-between gap-4 sm:flex-row">
        <PaginationControls
          pageSize={pageSize}
          setFilter={setFilter}
          totalItems={teachers?.length || 0}
          startIndex={startIndex}
          endIndex={endIndex}
        />

        <MyPagination
          totalPages={totalPages}
          currentPage={page}
          onPageChange={setFilter}
        />
      </div>
    </div>
  );
}

// ... existing code ...
