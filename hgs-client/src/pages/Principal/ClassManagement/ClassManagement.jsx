import { useState } from "react";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { PlusCircle, Settings } from "lucide-react";
import { useGradeLevels } from "@/services/common/queries";
import ClassModal from "./ClassModal";
import { useClassesWithStudentCount } from "@/services/principal/queries";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";
import UpdateClassModal from "./UpdateClassModal";

export default function ClassManagement() {
  const { currentYear } = useLayout();
  const [openCreateModal, setOpenCreateModal] = useState(false);
  const [openUpdateModal, setOpenUpdateModal] = useState(false);
  const [selectedClass, setSelectedClass] = useState(null);
  const classesWithStudentQuery = useClassesWithStudentCount(
    currentYear?.academicYearID,
  );
  const gradelevelQuery = useGradeLevels();

  // You should implement these handlers to call your API

  return (
    <div className="py-6">
      <div className="mb-4 flex items-center justify-between">
        <h2 className="mb-2 text-2xl font-bold">Quản Lý Lớp Học</h2>
        <Button
          className="flex items-center gap-2 bg-blue-600 text-white shadow-md transition-all hover:bg-blue-700 hover:shadow-lg"
          onClick={() => {
            setOpenCreateModal(true);
          }}
        >
          <PlusCircle className="h-4 w-4" />
          Thêm Lớp Học Mới
        </Button>
      </div>

      <ClassModal
        open={openCreateModal}
        onOpenChange={setOpenCreateModal}
        gradelevelQuery={gradelevelQuery}
        currentYear={currentYear}
      />

      <UpdateClassModal
        open={openUpdateModal && selectedClass}
        onOpenChange={(open) => {
          setOpenUpdateModal(open);
          if (!open) setSelectedClass(null);
        }}
        classId={selectedClass?.classId}
        currentYear={currentYear}
      />

      <div className="overflow-x-auto">
        <Table className="min-w-full border border-gray-300">
          <TableHeader>
            <TableRow className="bg-gray-100">
              <TableHead className="border border-gray-300">Khối</TableHead>
              <TableHead className="border border-gray-300">Tên Lớp</TableHead>
              <TableHead className="border border-gray-300">GVCN HK1</TableHead>
              <TableHead className="border border-gray-300">GVCN HK2</TableHead>
              <TableHead className="border border-gray-300">
                Số Học Sinh
              </TableHead>
              <TableHead className="border border-gray-300">
                Trạng thái
              </TableHead>
              <TableHead className="border border-gray-300">Thao Tác</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {classesWithStudentQuery?.data?.map((classItem) => (
              <TableRow
                key={classItem.classId}
                className="border-b border-gray-300"
              >
                <TableCell className="border border-gray-300">
                  {
                    gradelevelQuery?.data?.find(
                      (grade) => grade.gradeLevelId === classItem.gradeLevelId,
                    )?.gradeName
                  }
                </TableCell>
                <TableCell className="border border-gray-300">
                  {classItem.className}
                </TableCell>
                <TableCell className="border border-gray-300">
                  {
                    classItem.homeroomTeachers?.find(
                      (teacher) => teacher.semesterName === "Học kỳ 1",
                    )?.teacherName
                  }
                </TableCell>
                <TableCell className="border border-gray-300">
                  {
                    classItem.homeroomTeachers?.find(
                      (teacher) => teacher.semesterName === "Học kỳ 2",
                    )?.teacherName
                  }
                </TableCell>
                <TableCell className="border border-gray-300">
                  {classItem.studentCount}
                </TableCell>
                <TableCell className="border border-gray-300">
                  {classItem.status}
                </TableCell>
                <TableCell className="border border-gray-300">
                  <div className="flex gap-2">
                    <Button
                      variant="outline"
                      size="icon"
                      onClick={() => {
                        setSelectedClass(classItem);
                        setOpenUpdateModal(true);
                      }}
                      title="Chỉnh sửa"
                    >
                      <Settings className="h-4 w-4" />
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
