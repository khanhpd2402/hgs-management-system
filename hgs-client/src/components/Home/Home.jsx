// ... existing code ...
import { AgCharts } from "ag-charts-react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  Tooltip,
  Legend,
  ResponsiveContainer,
  LabelList,
} from "recharts";
import { useHomeroomTeachers, useStats } from "@/services/principal/queries";
import { Card, CardContent, CardHeader, CardTitle } from "../ui/card";
import { Skeleton } from "../ui/skeleton";
import { User, Users, School, GraduationCap, Landmark } from "lucide-react";
import { jwtDecode } from "jwt-decode";
import { useScheduleTeacher } from "@/services/schedule/queries";
import {
  useExamStats,
  useHomeroomAttendanceInfo,
  useHomeroomClassInfo,
  useLessonPlanStats,
  useStudentAttendances,
} from "@/services/teacher/queries";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";
import { useSemestersByAcademicYear } from "@/services/common/queries";
import { formatDate, formatDateString } from "@/helpers/formatDate";
import {
  useListStudentById,
  useStudentListAttendances,
} from "@/services/student/queries";

// const userRole = "Trưởng bộ môn";
const barColors = {
  "Có phép": "dodgerblue",
  "Không phép": "mediumseagreen",
  "Chưa rõ": "gold",
};

export default function Home() {
  const { currentYear } = useLayout();
  const token = JSON.parse(localStorage.getItem("token"));
  const teacherId = jwtDecode(token)?.teacherId;
  const userRole = jwtDecode(token)?.role;
  let studentIds = [];
  //phuhuynh
  if (userRole == "Phụ huynh") {
    studentIds = jwtDecode(token)?.studentIds?.split(",");
  }
  //hieu truong, hieu pho
  const statsQuery = useStats(userRole);
  const stats = statsQuery.data || [];

  //tkb
  const scheduleQuery = useScheduleTeacher(teacherId);
  const schedules = scheduleQuery.data || [];
  //truong bo mon
  const examQuery = useExamStats(userRole);
  const examStats = examQuery.data || [];
  const lessonPlanQuery = useLessonPlanStats(userRole);
  const lessonPlanStats = lessonPlanQuery.data || [];

  //semester
  const semesterQuery = useSemestersByAcademicYear(currentYear?.academicYearID);
  const semesters = semesterQuery.data || [];
  const today = new Date();
  const todayStr = formatDate(today);
  const currentSemester = semesters.find((semester) => {
    const start = new Date(semester.startDate);
    const end = new Date(semester.endDate);
    return today >= start && today <= end;
  });
  // Lấy danh sách ngày trong tuần hiện tại

  //chu nhiem
  const homeroomTeacherQuery = useHomeroomTeachers();
  const homeroomTeachers = homeroomTeacherQuery.data || [];
  const isHomeroom = homeroomTeachers.some(
    (teacher) => teacher.teacherId == teacherId,
  );
  const classInfoQuery = useHomeroomClassInfo({
    isHomeroom,
    teacherId,
    semesterId: currentSemester?.semesterID,
  });
  const getMonday = (d) => {
    const date = new Date(d);
    const day = date.getDay();
    // Nếu là Chủ nhật (0) thì lùi về thứ 2 tuần hiện tại, còn lại lùi về thứ 2
    const diff = date.getDate() - day + (day === 0 ? -6 : 1);
    return new Date(date.setDate(diff));
  };
  const weekStart = formatDate(getMonday(today));
  const weekEnd = formatDate(
    new Date(new Date(weekStart).setDate(new Date(weekStart).getDate() + 6)),
  );
  const attendanceInfoQuery = useHomeroomAttendanceInfo({
    isHomeroom,
    teacherId,
    semesterId: currentSemester?.semesterID,
    weekStart,
  });
  const classInfo = classInfoQuery.data || [];
  const attendanceInfo = attendanceInfoQuery.data || [];
  const studentAttendanceQueries = useStudentListAttendances({
    studentIds,
    weekStart: formatDateString(weekStart),
  });
  const studentAttendances =
    studentAttendanceQueries
      .filter((q) => q.status === "success" && q.data)
      .map((q) => q.data) || [];
  const studentInfoQueries = useListStudentById({
    studentIds,
    academicYearId: currentYear?.academicYearID,
  });
  const studentInfos =
    studentInfoQueries
      .filter((q) => q.status === "success" && q.data)
      .map((q) => q.data) || [];
  console.log(studentInfos);
  console.log(studentAttendances);

  // console.log(classInfo);
  // console.log(attendanceInfo);
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

  const attendanceTodayPieData = (attendanceInfo, date, session) => {
    const statusMap = {
      C: "Có mặt",
      K: "Không phép",
      P: "Có phép",
      X: "Chưa rõ",
    };
    const records = attendanceInfo.filter(
      (item) => item.date === date && item.session === session,
    );
    const summary = {
      "Có mặt": 0,
      "Có phép": 0,
      "Không phép": 0,
      "Chưa rõ": 0,
    };
    records.forEach((item) => {
      const label = statusMap[item.status] || "Chưa rõ";
      summary[label]++;
    });
    return [
      { label: "Có mặt", value: summary["Có mặt"] },
      { label: "Có phép", value: summary["Có phép"] },
      { label: "Không phép", value: summary["Không phép"] },
      { label: "Chưa rõ", value: summary["Chưa rõ"] },
    ];
  };

  // Hàm tổng hợp điểm danh trong tuần
  function getWeeklyAttendanceGroupedBar(attendanceInfo, weekDates) {
    const statusMap = {
      C: "Có mặt",
      K: "Không phép",
      P: "Có phép",
      X: "Chưa rõ",
    };
    const sessions = ["Sáng", "Chiều"];
    const daysOfWeek = [
      "Thứ Hai",
      "Thứ Ba",
      "Thứ Tư",
      "Thứ Năm",
      "Thứ Sáu",
      "Thứ Bảy",
    ];
    let result = [];
    weekDates.forEach((date, idx) => {
      sessions.forEach((session) => {
        const records = attendanceInfo.filter(
          (item) => item.date === date && item.session === session,
        );
        const summary = {
          "Có mặt": 0,
          "Có phép": 0,
          "Không phép": 0,
          "Chưa rõ": 0,
        };
        records.forEach((item) => {
          const label = statusMap[item.status] || "Chưa rõ";
          summary[label]++;
        });
        result.push({
          daySession: `${daysOfWeek[idx] || date}`,
          "Có mặt": summary["Có mặt"],
          "Có phép": summary["Có phép"],
          "Không phép": summary["Không phép"],
          "Chưa rõ": summary["Chưa rõ"],
        });
      });
    });
    return result;
  }
  function getWeekDates(weekStart, weekEnd) {
    const dates = [];
    let current = new Date(weekStart);
    // Lấy tối đa 6 ngày (Thứ Hai đến Thứ Bảy)
    for (let i = 0; i < 6; i++) {
      dates.push(formatDate(current));
      current.setDate(current.getDate() + 1);
    }
    return dates;
  }
  const weekDates = getWeekDates(weekStart, weekEnd);
  // const attendanceTodayGroupedBar = getAttendanceTodayGroupedBar(
  //   attendanceInfo,
  //   todayStr,
  // );
  const attendanceTodayMorningPie = attendanceTodayPieData(
    attendanceInfo,
    todayStr,
    "Sáng",
  );
  const attendanceTodayAfternoonPie = attendanceTodayPieData(
    attendanceInfo,
    todayStr,
    "Chiều",
  );
  const weeklyAttendanceGroupedBar = getWeeklyAttendanceGroupedBar(
    attendanceInfo,
    weekDates,
  );

  const isLoading =
    scheduleQuery.isLoading ||
    statsQuery.isLoading ||
    scheduleQuery.isLoading ||
    examQuery.isLoading ||
    classInfoQuery.isLoading ||
    attendanceInfoQuery.isLoading ||
    studentAttendanceQueries.isLoading ||
    studentInfoQueries.isLoading ||
    lessonPlanQuery.isLoading;

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

  if (userRole == "Phụ huynh") {
    return (
      <>
        <h2 className="mt-4 text-2xl font-semibold">Tình trạng điểm danh</h2>
        <div className="mt-4 grid grid-cols-1 gap-6">
          {studentInfos.length > 0 ? studentInfos.map((student, idx) => {
            const attendances = studentAttendances[idx] || [];
            const weekDates = getWeekDates(weekStart, weekEnd);
            const statusMap = {
              C: "Có mặt",
              K: "Không phép",
              P: "Có phép",
              X: "Chưa rõ",
            };
            // Gom nhóm theo ngày và buổi, mỗi ngày/buổi chỉ lấy 1 trạng thái (nếu có)
            const weeklyData = weekDates.flatMap((date) =>
              ["Sáng", "Chiều"].map((session) => {
                const record = attendances.find(
                  (item) => item.date === date && item.session === session,
                );
                let status = "";
                if (record) {
                  status = statusMap[record.status];
                }
                return {
                  date,
                  session,
                  status,
                };
              }),
            );
            return (
              <Card key={student.studentId} className="border shadow-lg">
                <CardHeader>
                  <CardTitle>{student.fullName}</CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="overflow-x-auto">
                    <table className="min-w-full rounded border text-center shadow">
                      <thead>
                        <tr>
                          <th className="border px-2 py-2">Ngày</th>
                          <th className="border px-2 py-2">Buổi</th>
                          <th className="border px-2 py-2">Có mặt</th>
                          <th className="border px-2 py-2">Có phép</th>
                          <th className="border px-2 py-2">Không phép</th>
                          <th className="border px-2 py-2">Chưa rõ</th>
                        </tr>
                      </thead>
                      <tbody>
                        {weeklyData.map((row, i) => (
                          <tr key={i}>
                            <td className="border px-2 py-2">{row.date}</td>
                            <td className="border px-2 py-2">{row.session}</td>
                            <td className="border px-2 py-2">
                              {row.status === "Có mặt" ? "✔️" : ""}
                            </td>
                            <td className="border px-2 py-2">
                              {row.status === "Có phép" ? "✔️" : ""}
                            </td>
                            <td className="border px-2 py-2">
                              {row.status === "Không phép" ? "✔️" : ""}
                            </td>
                            <td className="border px-2 py-2">
                              {row.status === "Chưa rõ" ? "✔️" : ""}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </CardContent>
              </Card>
            );
          }) : "Không tìm thấy dữ liệu học sinh trong năm nay"}
        </div>
      </>
    );
  }

  return (
    <>
      {userRole === "Giáo viên" && (
        <>
          {isHomeroom && (
            <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
              <Card className="md:col-span- border shadow-lg">
                <CardHeader>
                  <CardTitle>
                    <div className="flex items-center gap-2">
                      <School className="text-blue-500" />
                      <span>Thông tin lớp chủ nhiệm</span>
                    </div>
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="flex flex-col items-center gap-4 py-4">
                    <div className="flex items-center gap-2">
                      <Landmark className="text-blue-500" />
                      <span className="text-lg font-semibold">Lớp:</span>
                      <span className="text-2xl font-bold text-blue-600">
                        {classInfo.className || "--"}
                      </span>
                    </div>
                    <div className="flex items-center gap-2">
                      <GraduationCap className="text-red-500" />
                      <span className="text-lg font-semibold">
                        Tổng số học sinh:
                      </span>
                      <span className="text-2xl font-bold text-red-600">
                        {classInfo.totalStudents ?? "--"}
                      </span>
                    </div>
                  </div>
                </CardContent>
              </Card>
              <Card className="border shadow-lg md:col-span-2">
                <CardHeader>
                  <CardTitle>
                    <div className="flex items-center gap-2">
                      <Users className="text-yellow-500" />
                      <span>Điểm danh hôm nay</span>
                    </div>
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  {attendanceInfoQuery.isLoading ? (
                    <Skeleton className="h-56 w-full" />
                  ) : (
                    <div className="flex flex-col items-center justify-center gap-8 md:flex-row">
                      <div>
                        <div style={{ width: 220, height: 220 }}>
                          <AgCharts
                            options={pieOptions(
                              attendanceTodayMorningPie,
                              "Buổi sáng",
                            )}
                          />
                        </div>
                      </div>
                      <div>
                        <div style={{ width: 220, height: 220 }}>
                          <AgCharts
                            options={pieOptions(
                              attendanceTodayAfternoonPie,
                              "Buổi chiều",
                            )}
                          />
                        </div>
                      </div>
                    </div>
                  )}
                </CardContent>
              </Card>
              <Card className="border shadow-lg md:col-span-3">
                <CardHeader>
                  <CardTitle>
                    <div className="flex items-center gap-2">
                      <Users className="text-green-500" />
                      <span>Điểm danh trong tuần</span>
                    </div>
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  {attendanceInfoQuery.isLoading ? (
                    <Skeleton className="h-56 w-full" />
                  ) : (
                    <div className="flex w-full justify-center">
                      <div style={{ width: "100%", height: 350 }}>
                        <ResponsiveContainer width="100%" height="100%">
                          <BarChart
                            data={weeklyAttendanceGroupedBar}
                            margin={{ top: 20, right: 30, left: 0, bottom: 20 }}
                          >
                            <XAxis
                              dataKey="daySession"
                              tick={({ x, y, payload }) => {
                                const [day, session] = payload.value.split("-");
                                return (
                                  <g transform={`translate(${x},${y})`}>
                                    <text
                                      textAnchor="middle"
                                      fontSize={12}
                                      fill="#333"
                                    >
                                      <tspan x="0" dy="16">
                                        {day}
                                      </tspan>
                                      <tspan x="0" dy="16">
                                        {session}
                                      </tspan>
                                    </text>
                                  </g>
                                );
                              }}
                            />
                            <YAxis />
                            <Tooltip
                              formatter={(value, name) => [value, name]}
                              contentStyle={{ fontSize: 14 }}
                            />
                            <Legend />
                            <Bar dataKey="Có mặt" stackId="a" fill="#3CB371">
                              <LabelList
                                dataKey="Có mặt"
                                position="top"
                                fill="#fff"
                                fontSize={0}
                                fontWeight="bold"
                              />
                            </Bar>
                            <Bar dataKey="Có phép" stackId="a" fill="#1E90FF">
                              <LabelList
                                dataKey="Có phép"
                                position="top"
                                fill="#fff"
                                fontSize={0}
                                fontWeight="bold"
                              />
                            </Bar>
                            <Bar
                              dataKey="Không phép"
                              stackId="a"
                              fill="#FF6347"
                            >
                              <LabelList
                                dataKey="Không phép"
                                position="top"
                                fill="#fff"
                                fontSize={0}
                                fontWeight="bold"
                              />
                            </Bar>
                            <Bar dataKey="Chưa rõ" stackId="a" fill="#FFD700">
                              <LabelList
                                dataKey="Chưa rõ"
                                position="top"
                                fill="#fff"
                                fontSize={0}
                                fontWeight="bold"
                              />
                            </Bar>
                          </BarChart>
                        </ResponsiveContainer>
                      </div>
                    </div>
                  )}
                </CardContent>
              </Card>
            </div>
          )}
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
                          <th
                            key={day}
                            className="border bg-gray-100 px-2 py-2"
                          >
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
                            <td
                              key={day}
                              className="border px-2 py-2 align-top"
                            >
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
        </>
      )}
    </>
  );
}
// ... existing code ...
