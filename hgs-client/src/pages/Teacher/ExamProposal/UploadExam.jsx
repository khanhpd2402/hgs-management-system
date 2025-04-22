import { useEffect, useState } from "react";
import UploadExamModal from "./UploadExamModal";
import {
  useGradeLevels,
  useSemestersByAcademicYear,
} from "@/services/common/queries";
import { Card, CardContent } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";
import { cn } from "@/lib/utils";

function UploadExam() {
  //phan trang
  const { currentYear } = useLayout();
  const [semester, setSemester] = useState(null);
  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 5,
  });
  const semesterQuery = useSemestersByAcademicYear(currentYear?.academicYearID);
  const semesters = semesterQuery.data || [];
  const [exams, setExams] = useState([
    {
      id: 1,
      grade: "10",
      subject: "Toán",
      title: "Đề thi giữa kỳ 1",
      filename: "de_giua_ky_1.pdf",
    },
    {
      id: 2,
      grade: "11",
      subject: "Vật lý",
      title: "Đề thi cuối kỳ",
      filename: "de_cuoi_ky.pdf",
    },
  ]);

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

  const gradeLevelQuery = useGradeLevels();
  const gradeLevels = gradeLevelQuery.data || [];
  const isLoading = gradeLevelQuery.isLoading;

  // Handler to add a new exam from the upload form
  const handleAddExam = (exam) => {
    setExams([
      ...exams,
      {
        ...exam,
        id: exams.length + 1,
      },
    ]);
  };

  const { page, pageSize } = filter;

  useEffect(() => {
    if (semesters?.length > 0) {
      setSemester(semesters[0]);
    }
  }, [semesters, currentYear]);

  return (
    <div>
      <div className="flex items-center justify-between">
        <div className="flex items-center gap-4">
          <h2 className="mt-4 mb-6 text-3xl font-bold">Đề thi đã tải lên</h2>
          <div className="bg-muted text-muted-foreground inline-flex h-10 items-center justify-center rounded-lg p-1">
            {semesters?.map((sem) => (
              <button
                key={sem.semesterID}
                className={cn(
                  "ring-offset-background focus-visible:ring-ring inline-flex items-center justify-center rounded-md px-8 py-1.5 text-sm font-medium whitespace-nowrap transition-all focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:outline-none disabled:pointer-events-none disabled:opacity-50",
                  semester?.semesterID === sem.semesterID
                    ? "bg-blue-600 text-white shadow-sm" // Active: blue background, white text
                    : "bg-gray-200 text-gray-700 hover:bg-blue-100", // Inactive: gray background, blue hover
                )}
                onClick={() => setSemester(sem)}
              >
                {sem.semesterName}
              </button>
            ))}
          </div>
        </div>
        <UploadExamModal semester={semester} />
      </div>
      <Card className="mt-2 border shadow-md">
        <CardContent className="p-0">
          <div className="overflow-x-auto">
            <table className="min-w-full rounded-lg border border-gray-200 text-sm shadow-sm">
              <thead>
                <tr className="bg-gray-100">
                  <th className="border px-4 py-2">STT</th>
                  <th className="border px-4 py-2">Khối lớp</th>
                  <th className="border px-4 py-2">Môn học</th>
                  <th className="border px-4 py-2">Tiêu đề</th>
                  <th className="border px-4 py-2">Tên file</th>
                  <th className="border px-4 py-2">Trạng thái</th>
                </tr>
              </thead>
              <tbody>
                {isLoading
                  ? Array.from({ length: 5 }).map((_, idx) => (
                      <tr
                        key={idx}
                        className={idx % 2 === 0 ? "bg-blue-50" : ""}
                      >
                        <td className="border px-4 py-2 text-center">
                          <Skeleton className="h-4 w-8" />
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
                      </tr>
                    ))
                  : exams.map((exam, idx) => (
                      <tr
                        key={exam.id}
                        className={idx % 2 === 1 ? "bg-blue-50" : ""}
                      >
                        <td className="border px-4 py-2 text-center">
                          {idx + 1}
                        </td>
                        <td className="border px-4 py-2 text-center">
                          {exam.grade}
                        </td>
                        <td className="border px-4 py-2 text-center">
                          {exam.subject}
                        </td>
                        <td className="border px-4 py-2">{exam.title}</td>
                        <td className="border px-4 py-2">{exam.filename}</td>
                        <td className="border px-4 py-2">
                          <span
                            className={`inline-block rounded-full px-2 py-1 text-xs font-semibold ${getStatusBackgroundColor("Từ chối")}`}
                          >
                            Đang chờ duyệt
                          </span>
                        </td>
                      </tr>
                    ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

export default UploadExam;
