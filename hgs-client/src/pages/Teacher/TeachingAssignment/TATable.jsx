import { useEffect, useState } from "react";
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

import { Spinner } from "@/components/Spinner";
import TAModal from "./TAModal";
import ExportExcel from "@/components/excel/ExportExcel";
import ExcelImportModal from "@/components/excel/ExcelImportModal";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";
import {
  useClasses,
  useSemestersByAcademicYear,
} from "@/services/common/queries";
import { cn } from "@/lib/utils";
import { useSubjectConfigue, useTA } from "@/services/principal/queries";
import MyPagination from "@/components/MyPagination";
import PaginationControls from "@/components/PaginationControls";
import { Settings } from "lucide-react";
import UpdateTAModal from "./UpdateTAModal";

export default function TATable() {
  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 5,
  });
  const { currentYear } = useLayout();
  const [semester, setSemester] = useState(null);
  const TAQuery = useTA();
  const classQuery = useClasses();
  const subjectConfigQuery = useSubjectConfigue();
  // console.log(semester);
  // console.log(subjectConfigQuery.data);
  // console.log(TAQuery.data);
  // console.log(classQuery.data);

  const { data = [], isPending, error, isError, isFetching } = TAQuery;
  const semesterQuery = useSemestersByAcademicYear(currentYear?.academicYearID);
  const semesters = semesterQuery.data || [];
  const [openModal, setOpenModal] = useState(false);
  const [openUpdateModal, setOpenUpdateModal] = useState(false);

  useEffect(() => {
    if (semesters?.length > 0) {
      setSemester(semesters[0]);
    }
  }, [semesters, currentYear]);

  // const { data, isPending, error, isError, isFetching } = useTA(filter);

  const groupedData = Object.values(
    data.reduce((acc, curr) => {
      if (!acc[curr.teacherId]) {
        acc[curr.teacherId] = {
          teacherName: curr.teacherName,
          subjects: {},
        };
      }
      if (!acc[curr.teacherId].subjects[curr.subjectName]) {
        acc[curr.teacherId].subjects[curr.subjectName] = {
          subjectName: curr.subjectName,
          subjectId: curr.subjectId, // <-- Add this line
          classes: [],
          semesters: new Set(),
        };
      }
      acc[curr.teacherId].subjects[curr.subjectName].classes.push(
        curr.className,
      );
      acc[curr.teacherId].subjects[curr.subjectName].semesters.add(
        curr.semesterName,
      );
      return acc;
    }, {}),
  );

  // Tính toán hàng hiển thị
  if (isPending) {
    return (
      <Card className="relative mt-6 flex min-h-[550px] items-center justify-center p-4">
        <Spinner size="medium" />
      </Card>
    );
  }

  // if (isError) {
  //   return (
  //     <Card className="relative mt-6 flex items-center justify-center p-4">
  //       <div className="text-red-500">Lỗi khi tải dữ liệu</div>
  //     </Card>
  //   );
  // }

  //Phân trang
  const page = filter.page;
  const pageSize = filter.pageSize;
  const totalItems = groupedData.length;
  const totalPages = Math.ceil(totalItems / pageSize);
  const startIndex = totalItems === 0 ? 0 : (page - 1) * pageSize + 1;
  const endIndex = Math.min(page * pageSize, totalItems);
  const paginatedTeachers = groupedData.slice(
    (page - 1) * pageSize,
    page * pageSize,
  );

  console.log(groupedData);

  return (
    <div className="relative mt-6 p-4">
      <div className="mb-4 flex items-center justify-between border-b pb-4">
        <div className="flex items-center gap-4">
          <h2 className="!mb-0 text-lg font-semibold">Phân công giảng dạy</h2>
          <div className="bg-muted text-muted-foreground inline-flex h-10 items-center justify-center rounded-lg p-1">
            {semesters?.map((sem) => (
              <button
                key={sem.semesterID}
                className={cn(
                  // Change color for active/inactive semester
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
        <div className="flex items-center gap-2">
          <ExcelImportModal type="teachingAssingment" />
          <ExportExcel type="teachingAssingment" fileName="PCDG.xlsx" />
          <Button
            onClick={() => setOpenModal(true)}
            className="flex items-center gap-2 rounded-md bg-blue-600 text-white shadow-md transition-all hover:bg-blue-700 hover:shadow-lg"
          >
            Thêm phân công giảng dạy
          </Button>
        </div>
      </div>

      {/* Add Modal */}
      <TAModal
        open={openModal}
        onOpenChange={setOpenModal}
        semester={semester}
      />

      {/* Update Modal */}
      <UpdateTAModal
        open={openUpdateModal}
        onOpenChange={setOpenUpdateModal}
        semester={semester}
      />

      {/* Container chính không có overflow-x-auto */}
      <div className="relative">
        {isFetching && (
          <div className="absolute inset-0 z-10 flex items-center justify-center bg-white/70">
            <Spinner />
          </div>
        )}

        {/* Container cho bảng với overflow-x-auto */}
        <div className="max-h-[500px] overflow-auto">
          <div className="min-w-max">
            <Table
              className="w-full border border-gray-300"
              // style={{ minWidth: "1500px" }}
            >
              <TableHeader className="bg-gray-100">
                <TableRow className="sticky top-0 z-10">
                  <TableHead className="w-10 border border-gray-300 text-center whitespace-nowrap">
                    Thao tác
                  </TableHead>
                  <TableHead className="w-50 border border-gray-300 text-center whitespace-nowrap">
                    Họ tên cán bộ
                  </TableHead>
                  <TableHead className="w-50 border border-gray-300 text-center whitespace-nowrap">
                    Môn học
                  </TableHead>
                  <TableHead className="w-40 border border-gray-300 text-center whitespace-nowrap">
                    Lớp phân công
                  </TableHead>
                  <TableHead className="w-20 border border-gray-300 text-center whitespace-nowrap">
                    Số tiết
                  </TableHead>
                  <TableHead className="w-20 border border-gray-300 text-center whitespace-nowrap">
                    Số tiết định mức
                  </TableHead>
                  <TableHead className="w-20 border border-gray-300 text-center whitespace-nowrap">
                    Trạng thái
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {paginatedTeachers.map((teacher) => {
                  const subjectEntries = Object.values(teacher.subjects);

                  // Calculate total assigned periods for the teacher (all subjects)
                  let totalAssignedPeriods = 0;
                  subjectEntries.forEach((subject) => {
                    const classInfoArr = subject.classes.map((className) => {
                      const classObj = classQuery.data?.find(
                        (cls) => cls.className === className,
                      );
                      return classObj
                        ? {
                            className: classObj.className,
                            gradeLevelId: classObj.gradeLevelId,
                          }
                        : { className, gradeLevelId: 0 };
                    });

                    let totalHKI = 0;
                    let totalHKII = 0;
                    classInfoArr.forEach(({ gradeLevelId }) => {
                      const subjectConfig = subjectConfigQuery.data?.find(
                        (cfg) =>
                          cfg.subjectId === subject.subjectId &&
                          cfg.gradeLevelId === gradeLevelId,
                      );
                      if (subjectConfig) {
                        totalHKI +=
                          Number(subjectConfig.periodsPerWeekHKI) || 0;
                        totalHKII +=
                          Number(subjectConfig.periodsPerWeekHKII) || 0;
                      }
                    });

                    if (semester?.semesterName === "Học kỳ 1") {
                      totalAssignedPeriods += totalHKI;
                    } else if (semester?.semesterName === "Học kỳ 2") {
                      totalAssignedPeriods += totalHKII;
                    }
                  });

                  const standardPeriods = 19;
                  let status = "";
                  if (totalAssignedPeriods > standardPeriods) {
                    status = "Thừa";
                  } else if (totalAssignedPeriods < standardPeriods) {
                    status = "Thiếu";
                  } else {
                    status = "Đủ";
                  }

                  return subjectEntries.map((subject, idx) => {
                    // Build an array of { className, gradeLevelId }
                    const classInfoArr = subject.classes
                      .map((className) => {
                        const classObj = classQuery.data?.find(
                          (cls) => cls.className === className,
                        );
                        return classObj
                          ? {
                              className: classObj.className,
                              gradeLevelId: classObj.gradeLevelId,
                            }
                          : { className, gradeLevelId: 0 };
                      })
                      .sort((a, b) => {
                        if (a.gradeLevelId !== b.gradeLevelId) {
                          return a.gradeLevelId - b.gradeLevelId;
                        }
                        return a.className.localeCompare(b.className, "vi");
                      });

                    const sortedClassNames = classInfoArr.map(
                      (c) => c.className,
                    );

                    return (
                      <TableRow key={teacher.teacherName + subject.subjectName}>
                        {idx === 0 && (
                          <TableCell
                            rowSpan={subjectEntries.length}
                            className="h-14 border border-gray-300 text-center whitespace-nowrap"
                          >
                            <Button
                              variant="outline"
                              size="icon"
                              onClick={() => setOpenUpdateModal(true)}
                            >
                              <Settings className="h-4 w-4" />
                            </Button>
                          </TableCell>
                        )}
                        {idx === 0 && (
                          <TableCell
                            rowSpan={subjectEntries.length}
                            className="h-14 border border-gray-300 text-center whitespace-nowrap"
                          >
                            {teacher.teacherName}
                          </TableCell>
                        )}
                        <TableCell className="h-14 border border-gray-300 text-center whitespace-nowrap">
                          {subject.subjectName}
                        </TableCell>
                        <TableCell className="h-14 border border-gray-300 text-center whitespace-nowrap">
                          {sortedClassNames.join(", ")}
                        </TableCell>
                        {idx === 0 && (
                          <>
                            <TableCell
                              rowSpan={subjectEntries.length}
                              className="h-14 border border-gray-300 text-center whitespace-nowrap"
                            >
                              {totalAssignedPeriods}
                            </TableCell>
                            <TableCell
                              rowSpan={subjectEntries.length}
                              className="h-14 border border-gray-300 text-center whitespace-nowrap"
                            >
                              {standardPeriods}
                            </TableCell>
                            <TableCell
                              rowSpan={subjectEntries.length}
                              className="h-14 border border-gray-300 text-center whitespace-nowrap"
                            >
                              {status}
                            </TableCell>
                          </>
                        )}
                      </TableRow>
                    );
                  });
                })}
              </TableBody>
            </Table>
          </div>
        </div>

        <div className="mt-4 flex items-center justify-between">
          <PaginationControls
            pageSize={pageSize}
            setFilter={setFilter}
            totalItems={totalItems}
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
    </div>
  );
}
