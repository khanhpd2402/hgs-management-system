// ... existing code ...
import { AgCharts } from "ag-charts-react";
import { useHomeroomTeachers, useStats } from "@/services/principal/queries";
import { Card, CardContent, CardHeader, CardTitle } from "../ui/card";
import { Skeleton } from "../ui/skeleton";
import { User, Users, School, GraduationCap } from "lucide-react";
import { jwtDecode } from "jwt-decode";
import { useScheduleTeacher } from "@/services/schedule/queries";
import { useExamStats, useLessonPlanStats } from "@/services/teacher/queries";

// const userRole = "Trưởng bộ môn";
const barColors = {
  "Có phép": "dodgerblue",
  "Không phép": "mediumseagreen",
  "Chưa rõ": "gold",
};

export default function Home() {
  const statsQuery = useStats();
  const token = JSON.parse(localStorage.getItem("token"));
  const teacherId = jwtDecode(token)?.teacherId;
  const userRole = jwtDecode(token)?.role;
  const scheduleQuery = useScheduleTeacher(teacherId);
  const stats = statsQuery.data || [];
  const schedules = scheduleQuery.data || [];
  const examQuery = useExamStats();
  const examStats = examQuery.data || [];
  const lessonPlanQuery = useLessonPlanStats();
  const lessonPlanStats = lessonPlanQuery.data || [];
  const homeroomTeacherQuery = useHomeroomTeachers();
  const homeroomTeachers = homeroomTeacherQuery.data || [];
  const isHomeroom = homeroomTeachers.some(
    (teacher) => teacher.teacherId === teacherId,
  );
  // console.log(homeroomTeachers);
  // console.log(userRole);
  // console.log(examStats);
  // console.log(lessonPlanStats);
  // console.log(stats);

  // Dữ liệu cho biểu đồ tròn giới tính học sinh
  const studentGenderData = [
    { label: "Nam", value: stats?.maleStudents || 0 },
    { label: "Nữ", value: stats?.femaleStudents || 0 },
  ];
  // Dữ liệu cho biểu đồ tròn giới tính giáo viên
  const teacherGenderData = [
    { label: "Nam", value: stats?.maleTeachers || 0 },
    { label: "Nữ", value: stats?.femaleTeachers || 0 },
  ];
  // Dữ liệu cho biểu đồ cột tổng hợp

  // Dữ liệu cho biểu đồ cột điểm danh
  const absentBarData = [
    {
      label: "Có phép",
      value: stats?.attendanceSummary?.permissionAbsent || 4,
    },
    {
      label: "Không phép",
      value: stats?.attendanceSummary?.absentWithoutPermission || 4,
    },
    { label: "Chưa rõ", value: stats?.attendanceSummary?.unknownAbsent || 3 },
  ];

  const examStatsOptions = {
    data: [
      { label: "Đã duyệt", value: examStats.submittedCount || 0 },
      { label: "Từ chối", value: examStats.pendingCount || 0 },
      { label: "Chờ duyệt", value: examStats.waitingForApprovalCount || 0 },
    ],
    title: { text: "Trạng thái các đề thi", fontSize: 16 },
    series: [
      {
        type: "pie",
        angleKey: "value",
        labelKey: "label",
        innerRadiusRatio: 0.5,
        calloutLabel: { enabled: true },
        sectorLabelKey: "value",
        tooltip: {
          renderer: ({ datum }) => ({
            title: datum.label,
            content: `${datum.value}`,
          }),
        },
      },
    ],
    legend: { position: "bottom" },
    height: 220,
    width: "100%",
    padding: { top: 20, bottom: 20, left: 0, right: 0 },
  };

  const lessonPlanStatsOptions = {
    data: [
      { label: "Đã duyệt", value: lessonPlanStats.submittedCount || 0 },
      { label: "Chờ xử lý", value: lessonPlanStats.pendingCount || 0 },
      {
        label: "Chờ duyệt",
        value: lessonPlanStats.waitingForApprovalCount || 0,
      },
      { label: "Từ chối", value: lessonPlanStats.rejectedCount || 0 },
    ],
    title: { text: "Trạng thái giáo án", fontSize: 16 },
    series: [
      {
        type: "pie",
        angleKey: "value",
        labelKey: "label",
        innerRadiusRatio: 0.5,
        calloutLabel: { enabled: true },
        sectorLabelKey: "value",
        tooltip: {
          renderer: ({ datum }) => ({
            title: datum.label,
            content: `${datum.value}`,
          }),
        },
      },
    ],
    legend: { position: "bottom" },
    height: 220,
    width: "100%",
    padding: { top: 20, bottom: 20, left: 0, right: 0 },
  };

  // Cấu hình ag-charts
  const pieOptions = (data, title) => ({
    data,
    title: { text: title, fontSize: 16 },
    series: [
      {
        type: "pie",
        angleKey: "value",
        labelKey: "label",
        innerRadiusRatio: 0.5,
        calloutLabel: { enabled: true },
        sectorLabelKey: "value",
        tooltip: {
          renderer: ({ datum }) => ({
            title: datum.label,
            // content: `${datum.label}: ${datum.value}`,
          }),
        },
      },
    ],
    legend: { position: "bottom" },
    height: 220,
    width: "100%",
    padding: { top: 20, bottom: 20, left: 0, right: 0 },
  });

  const barOptions = (data, title) => ({
    data,
    title: { text: title, fontSize: 16 },
    series: [
      {
        type: "bar",
        xKey: "label",
        yKey: "value",
        label: { enabled: true },
        itemStyler: (params) => {
          return {
            fill: barColors[params.datumIndex % barColors.length],
          };
        },
      },
    ],
    legend: { enabled: false },
    height: 220,
    width: "100%",
    padding: { top: 20, bottom: 20, left: 0, right: 0 },
    axes: [
      { type: "category", position: "bottom" },
      { type: "number", position: "left", nice: true, min: 0 },
    ],
  });

  const daysOfWeek = [
    "Thứ Hai",
    "Thứ Ba",
    "Thứ Tư",
    "Thứ Năm",
    "Thứ Sáu",
    "Thứ Bảy",
  ];

  const morningPeriods = [1, 2, 3, 4, 5];
  const afternoonPeriods = [6, 7, 8];
  const allLessons = schedules.flatMap((item) => item.details || []);

  function renderCell(lessons, day, period) {
    const item = lessons.find(
      (s) => s.dayOfWeek === day && s.periodId === period,
    );
    if (!item) return null;
    return (
      <div className="font-semibold">
        {item.className} - {item.subjectName}
        {/* <div className="text-xs text-gray-500">{item.teacherName}</div> */}
        {/* <div className="font-semibold">{item.className}</div> */}
      </div>
    );
  }

  const isLoading =
    scheduleQuery.isLoading ||
    statsQuery.isLoading ||
    scheduleQuery.isLoading ||
    examQuery.isLoading;

  if (userRole == "Hiệu trưởng" || userRole == "Hiệu phó") {
    return (
      <>
        <Card className="my-6 border shadow-lg">
          <CardHeader>
            <CardTitle>Thống kê</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col gap-10 md:flex-row">
              {/* Box Lớp */}
              <div className="flex items-center gap-4">
                <div className="rounded-full bg-violet-100 p-4">
                  <School className="h-8 w-8 text-violet-500" />
                </div>
                <div>
                  <div className="text-2xl font-bold">
                    {stats?.activeClasses ?? (
                      <Skeleton className="inline-block h-6 w-10" />
                    )}
                  </div>
                  <div className="text-sm text-gray-500">Lớp</div>
                </div>
              </div>
              {/* Box Giáo viên */}
              <div className="flex items-center gap-4">
                <div className="rounded-full bg-cyan-100 p-4">
                  <User className="h-8 w-8 text-cyan-500" />
                </div>
                <div>
                  <div className="text-2xl font-bold">
                    {stats?.totalTeachers ?? (
                      <Skeleton className="inline-block h-6 w-10" />
                    )}
                  </div>
                  <div className="text-sm text-gray-500">Giáo viên</div>
                </div>
              </div>
              {/* Box Học sinh */}
              <div className="flex items-center gap-4">
                <div className="rounded-full bg-red-100 p-4">
                  <GraduationCap className="h-8 w-8 text-red-500" />
                </div>
                <div>
                  <div className="text-2xl font-bold">
                    {stats?.totalStudents ?? (
                      <Skeleton className="inline-block h-6 w-10" />
                    )}
                  </div>
                  <div className="text-sm text-gray-500">Học sinh</div>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
        <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
          <Card className="border shadow-lg">
            <CardHeader>
              <CardTitle>
                <div className="flex items-center gap-2">
                  <Users className="text-blue-500" /> Học sinh
                </div>
              </CardTitle>
            </CardHeader>
            <CardContent>
              {statsQuery.isLoading ? (
                <Skeleton className="h-56 w-full" />
              ) : (
                <div className="flex w-full justify-center">
                  <div style={{ width: "100%", maxWidth: 350 }}>
                    <AgCharts
                      options={pieOptions(
                        studentGenderData,
                        "Tỉ lệ học sinh Nam/Nữ",
                      )}
                    />
                  </div>
                </div>
              )}
              <div className="mt-4 text-center font-semibold">
                Tổng số học sinh:{" "}
                {stats?.totalStudents ?? (
                  <Skeleton className="inline-block h-4 w-16" />
                )}
              </div>
            </CardContent>
          </Card>

          <Card className="border shadow-lg">
            <CardHeader>
              <CardTitle>
                <div className="flex items-center gap-2">
                  <User className="text-green-500" /> Giáo viên
                </div>
              </CardTitle>
            </CardHeader>
            <CardContent>
              {statsQuery.isLoading ? (
                <Skeleton className="h-56 w-full" />
              ) : (
                <div className="flex w-full justify-center">
                  <div style={{ width: "100%", maxWidth: 350 }}>
                    <AgCharts
                      options={pieOptions(
                        teacherGenderData,
                        "Tỉ lệ giáo viên Nam/Nữ",
                      )}
                    />
                  </div>
                </div>
              )}
              <div className="mt-4 text-center font-semibold">
                Tổng số giáo viên:{" "}
                {stats?.totalTeachers ?? (
                  <Skeleton className="inline-block h-4 w-16" />
                )}
              </div>
            </CardContent>
          </Card>

          <Card className="col-span-1 border shadow-lg md:col-span-2">
            <CardHeader>
              <CardTitle>
                <div className="flex items-center gap-2">
                  <School className="text-yellow-500" /> Thống kê điểm danh
                </div>
              </CardTitle>
            </CardHeader>
            <CardContent>
              {statsQuery.isLoading ? (
                <Skeleton className="h-56 w-full" />
              ) : (
                <div className="flex w-full justify-center">
                  <div style={{ width: "100%", maxWidth: 500 }}>
                    <AgCharts
                      options={barOptions(absentBarData, "Thống kê điểm danh")}
                    />
                  </div>
                </div>
              )}
            </CardContent>
          </Card>
        </div>
      </>
    );
  }

  if (userRole == "Trưởng bộ môn") {
    return (
      <div className="mt-4 grid grid-cols-1 gap-6 md:grid-cols-2">
        <Card className="border shadow-lg">
          <CardHeader>
            <CardTitle>
              <div className="flex items-center gap-2">
                <School className="text-blue-500" /> Thống kê đề thi
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            {examQuery.isLoading ? (
              <Skeleton className="h-56 w-full" />
            ) : (
              <div className="flex w-full justify-center">
                <div style={{ width: "100%", maxWidth: 350 }}>
                  <AgCharts options={examStatsOptions} />
                </div>
              </div>
            )}
            <div className="mt-4 text-center font-semibold">
              Tổng số đề thi: {examStats.totalExamProposals ?? 0}
            </div>
          </CardContent>
        </Card>
        <Card className="border shadow-lg">
          <CardHeader>
            <CardTitle>
              <div className="flex items-center gap-2">
                <School className="text-green-500" /> Thống kê giáo án
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            {lessonPlanQuery.isLoading ? (
              <Skeleton className="h-56 w-full" />
            ) : (
              <div className="flex w-full justify-center">
                <div style={{ width: "100%", maxWidth: 350 }}>
                  <AgCharts options={lessonPlanStatsOptions} />
                </div>
              </div>
            )}
            <div className="mt-4 text-center font-semibold">
              Tổng số giáo án:{" "}
              {lessonPlanStats.totalLessonPlans ?? (
                <Skeleton className="inline-block h-4 w-16" />
              )}
            </div>
          </CardContent>
        </Card>
        <Card className="col-span-2 my-6 border shadow-lg">
          <CardHeader>
            <CardTitle>Thời khoá biểu</CardTitle>
          </CardHeader>
          <CardContent>
            {isLoading ? (
              <Skeleton className="h-96 w-full" />
            ) : (
              <div className="overflow-x-auto">
                <table className="min-w-full rounded border text-center shadow">
                  <thead>
                    <tr>
                      <th className="border bg-gray-100 px-2 py-2">Buổi</th>
                      <th className="border bg-gray-100 px-2 py-2">Tiết</th>
                      {daysOfWeek.map((day) => (
                        <th key={day} className="border bg-gray-100 px-2 py-2">
                          {day}
                        </th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {/* Buổi sáng */}
                    {morningPeriods.map((period, idx) => (
                      <tr key={period}>
                        {idx === 0 && (
                          <td
                            className="border bg-blue-50 px-2 py-2 font-bold"
                            rowSpan={morningPeriods.length}
                            style={{ verticalAlign: "middle" }}
                          >
                            Sáng
                          </td>
                        )}
                        <td className="border bg-blue-50 px-2 py-2 font-bold">
                          Tiết {period}
                        </td>
                        {daysOfWeek.map((day) => (
                          <td
                            key={day}
                            className="align-center border px-2 py-2"
                          >
                            {renderCell(allLessons, day, period) || (
                              <span className="text-gray-300"></span>
                            )}
                          </td>
                        ))}
                      </tr>
                    ))}
                    {/* Buổi chiều */}
                    {afternoonPeriods.map((period, idx) => (
                      <tr key={period}>
                        {idx === 0 && (
                          <td
                            className="border bg-yellow-50 px-2 py-2 font-bold"
                            rowSpan={afternoonPeriods.length}
                            style={{ verticalAlign: "middle" }}
                          >
                            Chiều
                          </td>
                        )}
                        <td className="border bg-yellow-50 px-2 py-2 font-bold">
                          Tiết {period}
                        </td>
                        {daysOfWeek.map((day) => (
                          <td key={day} className="border px-2 py-2 align-top">
                            {renderCell(allLessons, day, period) || (
                              <span className="text-gray-300"></span>
                            )}
                          </td>
                        ))}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <>
      {userRole === "Giáo viên" && (
        <Card className="my-6 border shadow-lg">
          <CardHeader>
            <CardTitle>Thời khoá biểu</CardTitle>
          </CardHeader>
          <CardContent>
            {isLoading ? (
              <Skeleton className="h-96 w-full" />
            ) : (
              <div className="overflow-x-auto">
                <table className="min-w-full rounded border text-center shadow">
                  <thead>
                    <tr>
                      <th className="border bg-gray-100 px-2 py-2">Buổi</th>
                      <th className="border bg-gray-100 px-2 py-2">Tiết</th>
                      {daysOfWeek.map((day) => (
                        <th key={day} className="border bg-gray-100 px-2 py-2">
                          {day}
                        </th>
                      ))}
                    </tr>
                  </thead>
                  <tbody>
                    {/* Buổi sáng */}
                    {morningPeriods.map((period, idx) => (
                      <tr key={period}>
                        {idx === 0 && (
                          <td
                            className="border bg-blue-50 px-2 py-2 font-bold"
                            rowSpan={morningPeriods.length}
                            style={{ verticalAlign: "middle" }}
                          >
                            Sáng
                          </td>
                        )}
                        <td className="border bg-blue-50 px-2 py-2 font-bold">
                          Tiết {period}
                        </td>
                        {daysOfWeek.map((day) => (
                          <td
                            key={day}
                            className="align-center border px-2 py-2"
                          >
                            {renderCell(allLessons, day, period) || (
                              <span className="text-gray-300"></span>
                            )}
                          </td>
                        ))}
                      </tr>
                    ))}
                    {/* Buổi chiều */}
                    {afternoonPeriods.map((period, idx) => (
                      <tr key={period}>
                        {idx === 0 && (
                          <td
                            className="border bg-yellow-50 px-2 py-2 font-bold"
                            rowSpan={afternoonPeriods.length}
                            style={{ verticalAlign: "middle" }}
                          >
                            Chiều
                          </td>
                        )}
                        <td className="border bg-yellow-50 px-2 py-2 font-bold">
                          Tiết {period}
                        </td>
                        {daysOfWeek.map((day) => (
                          <td key={day} className="border px-2 py-2 align-top">
                            {renderCell(allLessons, day, period) || (
                              <span className="text-gray-300"></span>
                            )}
                          </td>
                        ))}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </CardContent>
        </Card>
      )}
    </>
  );
}
// ... existing code ...
