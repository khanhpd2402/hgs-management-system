import React, { useState } from "react";
import toast, { Toaster } from "react-hot-toast";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { PlusCircle, Pencil, Trash2 } from "lucide-react";
import { useGradeLevels } from "@/services/common/queries";
import ClassModal from "./ClassModal";
import { useClassesWithStudentCount } from "@/services/principal/queries";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";

export default function ClassManagement() {
  const { currentYear } = useLayout();
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingClass, setEditingClass] = useState(null);

  const classesWithStudentQuery = useClassesWithStudentCount(
    currentYear?.academicYearID,
  );
  const gradelevelQuery = useGradeLevels();
  console.log(classesWithStudentQuery.data);

  // You should implement these handlers to call your API
  const handleSubmit = async (e) => {
    e.preventDefault();
    // Implement API call for add/edit here
    // After successful API call, close modal and show toast
    setIsDialogOpen(false);
    setEditingClass(null);
    toast.success(
      editingClass ? "Cập nhật lớp học thành công" : "Thêm lớp học thành công",
    );
    // Optionally: refetch classQuery here
  };

  const handleDelete = async (id) => {
    if (confirm("Bạn có chắc chắn muốn xóa lớp học này không?")) {
      // Implement API call for delete here
      toast.success("Xóa lớp học thành công");
      // Optionally: refetch classQuery here
    }
  };

  return (
    <div className="py-6">
      <Toaster position="top-right" />
      <div className="mb-4 flex items-center justify-between">
        <h2 className="mb-2 text-2xl font-bold">Quản Lý Lớp Học</h2>
        <Button
          className="flex items-center gap-2"
          onClick={() => {
            setEditingClass(null);
            setIsDialogOpen(true);
          }}
        >
          <PlusCircle className="h-4 w-4" />
          Thêm Lớp Học Mới
        </Button>
      </div>

      <ClassModal
        open={isDialogOpen}
        onOpenChange={setIsDialogOpen}
        onSubmit={handleSubmit}
        editingClass={editingClass}
        gradelevelQuery={gradelevelQuery}
      />

      <div className="overflow-x-auto">
        <Table className="min-w-full border border-gray-300">
          <TableHeader>
            <TableRow className="bg-gray-100">
              <TableHead className="border border-gray-300">Khối</TableHead>
              <TableHead className="border border-gray-300">Tên Lớp</TableHead>
              <TableHead className="border border-gray-300">
                Giáo Viên Chủ Nhiệm
              </TableHead>
              <TableHead className="border border-gray-300">
                Số Học Sinh
              </TableHead>
              <TableHead className="border border-gray-300">Thao Tác</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {classesWithStudentQuery?.data?.map((classItem) => (
              <TableRow key={classItem.id} className="border-b border-gray-300">
                <TableCell className="border border-gray-300">
                  {
                    gradelevelQuery?.data?.find(
                      (grade) => grade.id === classItem.gradeId,
                    )?.gradeName
                  }
                </TableCell>
                <TableCell className="border border-gray-300">
                  {classItem.className}
                </TableCell>
                <TableCell className="border border-gray-300">
                  {classItem.homeroomTeacherName}
                </TableCell>
                <TableCell className="border border-gray-300">
                  {classItem.studentCount}
                </TableCell>
                <TableCell className="border border-gray-300">
                  <div className="flex gap-2">
                    <Button
                      variant="outline"
                      size="icon"
                      onClick={() => {
                        setEditingClass(classItem);
                        setIsDialogOpen(true);
                      }}
                      title="Chỉnh sửa"
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="destructive"
                      size="icon"
                      onClick={() => handleDelete(classItem.id)}
                      title="Xóa"
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
