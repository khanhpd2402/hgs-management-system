import React, { useState } from "react";
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

export default function SubjectConfigForTeacher() {
  const [openUpdateModal, setOpenUpdateModal] = useState(false);
  const [selectedTeacher, setSelectedTeacher] = useState(null);

  const teacherSubjectQuery = useTeacherSubjects();
  const teachers = teacherSubjectQuery.data?.teachers || [];

  return (
    <div className="mt-6">
      <h2 className="mb-6 text-2xl font-bold">
        Cấu hình môn học cho giáo viên
      </h2>
      <div className="mt-2 text-sm text-gray-600">
        <span>
          Môn học có dấu
          <span className="text-red-500"> *</span> là môn dạy chính
        </span>
      </div>
      <Table className="mt-2 w-full rounded-lg bg-white shadow">
        <TableHeader>
          <TableRow>
            <TableHead className="w-16">STT</TableHead>
            <TableHead>Giáo viên</TableHead>
            <TableHead>Môn học có thể dạy</TableHead>
            <TableHead>Thao tác</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {teachers.map((teacher, idx) => (
            <TableRow key={teacher.teacherId}>
              <TableCell>{idx + 1}</TableCell>
              <TableCell>{teacher.fullName}</TableCell>
              <TableCell>
                {teacher?.subjects?.map((t, i) => (
                  <span key={t.subjectId || i}>
                    {t.subjectName}
                    {t.isMainSubject && <span className="text-red-500">*</span>}
                    {i < teacher.subjects.length - 1 && ", "}
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

      <UpdateTeacherSubjectModal
        open={openUpdateModal}
        onClose={(open) => {
          setOpenUpdateModal(open);
          if (!open) setSelectedTeacher(null);
        }}
        teacherId={selectedTeacher}
      />
    </div>
  );
}

// ... existing code ...
