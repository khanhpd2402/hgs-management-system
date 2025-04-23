import { useEffect, useState } from "react";
import {
  useGradeLevels,
  useSemestersByAcademicYear,
  useSubjects,
} from "@/services/common/queries";
import { Card, CardContent } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import PaginationControls from "@/components/PaginationControls";
import MyPagination from "@/components/MyPagination";
import toast from "react-hot-toast";
import { useExams } from "@/services/principal/queries";
import { useTeachers } from "@/services/teacher/queries";
// Giả sử có hook này, bạn cần thay bằng hook thật từ services/principal/queries
// import { useAllExamProposals, useUpdateExamStatus } from "@/services/principal/queries";

export default function ExamManagement() {
  const { currentYear } = useLayout();
  const [semester, setSemester] = useState(null);
  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 5,
  });
  const [statusFilter, setStatusFilter] = useState("all");
  const [gradeFilter, setGradeFilter] = useState("all");
  const [subjectFilter, setSubjectFilter] = useState("all");
  const [selectedExam, setSelectedExam] = useState(null);
  const [openModal, setOpenModal] = useState(false);
  const [newStatus, setNewStatus] = useState("");

  const semesterQuery = useSemestersByAcademicYear(currentYear?.academicYearID);
  const semesters = semesterQuery.data || [];
  const teacherQuery = useTeachers();
  const teachers = teacherQuery.data?.teachers || [];
  const examQuery = useExams(); // placeholder
  const allExams = examQuery.data || [];
  const subjectQuery = useSubjects();
  const subjects = subjectQuery.data || [];
  const gradeLevelQuery = useGradeLevels();
  const gradeLevels = gradeLevelQuery.data || [];
  const isLoading = examQuery.isLoading || gradeLevelQuery.isLoading;

  const getStatusBackgroundColor = (status) => {
    switch (status) {
      case "Đã duyệt":
        return "bg-green-100 text-green-800";
      case "Chờ duyệt":
        return "bg-yellow-100 text-yellow-800";
      case "Từ chối":
        return "bg-red-100 text-red-800";
      default:
        return "bg-gray-100 text-gray-800";
    }
  };

  console.log(allExams);

  const filteredExams = allExams.filter((exam) => {
    let match = true;
    if (statusFilter !== "all" && exam.status !== statusFilter) match = false;
    if (gradeFilter !== "all" && exam.grade !== gradeFilter) match = false;
    if (subjectFilter !== "all" && exam.subjectId !== subjectFilter)
      match = false;
    if (semester && exam.semesterId !== semester.semesterID) match = false;
    return match;
  });

  const { page, pageSize } = filter;
  const totalPages = Math.ceil(filteredExams.length / pageSize);
  const currentData = filteredExams.slice(
    (page - 1) * pageSize,
    page * pageSize,
  );

  const startIndex = filteredExams.length === 0 ? 0 : (page - 1) * pageSize + 1;
  const endIndex = Math.min(page * pageSize, filteredExams.length);

  useEffect(() => {
    if (semesters?.length > 0) {
      setSemester(semesters[0]);
    }
  }, [semesters, currentYear]);

  // const updateExamStatus = useUpdateExamStatus(); // TODO: mutation đổi trạng thái

  const handleChangeStatus = (exam, status) => {
    setSelectedExam(exam);
    setNewStatus(status);
    setOpenModal(true);
  };

  const handleConfirmChangeStatus = () => {
    // TODO: Gọi mutation đổi trạng thái, ví dụ:
    // updateExamStatus.mutate({ id: selectedExam.proposalId, status: newStatus }, {
    //   onSuccess: () => {
    //     toast.success("Cập nhật trạng thái thành công!");
    //     setOpenModal(false);
    //   },
    //   onError: () => {
    //     toast.error("Có lỗi xảy ra!");
    //   }
    // });
    setOpenModal(false);
    toast.success("Đã cập nhật trạng thái (demo)");
  };

  return (
    <div>
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <h2 className="mt-4 mb-6 text-3xl font-bold">Quản lý đề thi</h2>
          <div className="bg-muted text-muted-foreground inline-flex h-10 items-center justify-center rounded-lg">
            {semesters?.map((sem) => (
              <button
                key={sem.semesterID}
                className={cn(
                  "ring-offset-background focus-visible:ring-ring inline-flex items-center justify-center rounded-md px-8 py-1.5 text-sm font-medium whitespace-nowrap transition-all focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:outline-none disabled:pointer-events-none disabled:opacity-50",
                  semester?.semesterID === sem.semesterID
                    ? "bg-blue-600 text-white shadow-sm"
                    : "bg-gray-200 text-gray-700 hover:bg-blue-100",
                )}
                onClick={() => setSemester(sem)}
              >
                {sem.semesterName}
              </button>
            ))}
          </div>
          <div className="flex items-center gap-4">
            <Select value={statusFilter} onValueChange={setStatusFilter}>
              <SelectTrigger className="w-40">
                <SelectValue placeholder="Lọc trạng thái" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả trạng thái</SelectItem>
                <SelectItem value="Đã duyệt">Đã duyệt</SelectItem>
                <SelectItem value="Chờ duyệt">Chờ duyệt</SelectItem>
                <SelectItem value="Từ chối">Từ chối</SelectItem>
              </SelectContent>
            </Select>
            <Select value={gradeFilter} onValueChange={setGradeFilter}>
              <SelectTrigger className="w-40">
                <SelectValue placeholder="Lọc khối lớp" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả khối</SelectItem>
                {gradeLevels.map((grade) => (
                  <SelectItem
                    value={grade.gradeLevelId}
                    key={grade.gradeLevelId}
                  >
                    {grade.gradeName}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            <Select value={subjectFilter} onValueChange={setSubjectFilter}>
              <SelectTrigger className="w-40">
                <SelectValue placeholder="Lọc môn học" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả môn</SelectItem>
                {subjects.map((subject) => (
                  <SelectItem value={subject.subjectID} key={subject.subjectID}>
                    {subject.subjectName}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>
      </div>
      <Card className="mt-2 border shadow-md">
        <CardContent className="p-0">
          <div className="overflow-x-auto">
            <table className="min-w-full rounded-lg border border-gray-200 text-sm shadow-sm">
              <thead>
                <tr className="bg-gray-100">
                  <th className="w-16 border px-4 py-2">STT</th>
                  <th className="w-40 border px-4 py-2">Người gửi</th>
                  <th className="w-20 border px-4 py-2">Khối lớp</th>
                  <th className="w-32 border px-4 py-2">Môn học</th>
                  <th className="w-48 border px-4 py-2">Tiêu đề</th>
                  <th className="w-40 border px-4 py-2">File tài liệu</th>
                  <th className="w-25 border px-4 py-2">Trạng thái</th>
                </tr>
              </thead>
              <tbody>
                {isLoading ? (
                  Array.from({ length: 5 }).map((_, idx) => (
                    <tr key={idx} className={idx % 2 === 0 ? "bg-blue-50" : ""}>
                      <td className="border px-4 py-2 text-center">
                        <Skeleton className="h-4 w-8" />
                      </td>
                      <td className="border px-4 py-2 text-center">
                        <Skeleton className="h-4 w-12" />
                      </td>
                      <td className="border px-4 py-2 text-center">
                        <Skeleton className="h-4 w-12" />
                      </td>
                      <td className="border px-4 py-2 text-center">
                        <Skeleton className="h-4 w-16" />
                      </td>
                      <td className="border px-4 py-2">
                        <Skeleton className="h-4 w-32" />
                      </td>
                      <td className="border px-4 py-2">
                        <Skeleton className="h-4 w-32" />
                      </td>
                      <td className="border px-4 py-2">
                        <Skeleton className="h-4 w-20" />
                      </td>
                      <td className="border px-4 py-2">
                        <Skeleton className="h-4 w-20" />
                      </td>
                    </tr>
                  ))
                ) : currentData?.length > 0 ? (
                  currentData?.map((exam, idx) => (
                    <tr
                      key={exam.proposalId}
                      className={idx % 2 === 1 ? "bg-blue-50" : ""}
                    >
                      <td className="border px-4 py-2 text-center">
                        {idx + 1}
                      </td>
                      <td className="border px-4 py-2 text-center">
                        {
                          teachers.find((t) => t.teacherId === exam.createdBy)
                            ?.fullName
                        }
                      </td>
                      <td className="border px-4 py-2 text-center">
                        {
                          gradeLevels.find(
                            (grade) => grade.gradeLevelId === exam.grade,
                          )?.gradeName
                        }
                      </td>
                      <td className="border px-4 py-2 text-center">
                        {
                          subjects.find(
                            (sub) => sub.subjectID === exam.subjectId,
                          )?.subjectName
                        }
                      </td>
                      <td className="border px-4 py-2">{exam.title}</td>
                      <td className="border px-4 py-2">
                        {exam.fileUrl ? (
                          <a
                            href={exam.fileUrl}
                            download
                            className="text-green-600 underline hover:text-green-800"
                          >
                            Tải xuống
                          </a>
                        ) : (
                          <span className="text-gray-400 italic">
                            Không có file
                          </span>
                        )}
                      </td>
                      <td className="border px-4 py-2 text-center">
                        <span
                          className={`inline-block rounded-full px-2 py-1 text-xs font-semibold ${getStatusBackgroundColor(exam.status)}`}
                        >
                          {exam.status}
                        </span>
                      </td>
                    </tr>
                  ))
                ) : (
                  <tr>
                    <td colSpan={7} className="p-4 text-left">
                      Không có dữ liệu
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
      <div className="mt-4 flex items-center justify-between">
        <PaginationControls
          pageSize={pageSize}
          setFilter={setFilter}
          totalItems={filteredExams.length || 0}
          startIndex={startIndex}
          endIndex={endIndex}
        />
        <MyPagination
          totalPages={totalPages}
          currentPage={page}
          onPageChange={setFilter}
        />
      </div>
      {/* Modal xác nhận đổi trạng thái */}
      {openModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/30">
          <div className="w-full max-w-sm rounded-lg bg-white p-6 shadow-lg">
            <h3 className="mb-4 text-lg font-semibold text-gray-800">
              Xác nhận đổi trạng thái
            </h3>
            <p>
              Bạn có chắc muốn đổi trạng thái đề thi thành{" "}
              <span className="font-bold">{newStatus}</span>?
            </p>
            <div className="mt-6 flex justify-end gap-2">
              <Button variant="outline" onClick={() => setOpenModal(false)}>
                Hủy
              </Button>
              <Button
                className="bg-blue-600 text-white hover:bg-blue-700"
                onClick={handleConfirmChangeStatus}
              >
                Xác nhận
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
