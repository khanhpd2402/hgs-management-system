import { useEffect, useState } from "react";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import AttendanceHeader from "./AttendanceHeader";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { data } from "./data";
import {
  useHomeroomTeachers,
  useTeachingAssignmentsByTeacher,
} from "@/services/principal/queries";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";
import {
  useClasses,
  useSemestersByAcademicYear,
} from "@/services/common/queries";
import { cn } from "@/lib/utils";
import { jwtDecode } from "jwt-decode";
import { useStudentByClass } from "@/services/teacher/queries";
import { formatDate } from "@/helpers/formatDate";

export default function AttendanceTable() {
  const { currentYear } = useLayout();
  const semesterQuery = useSemestersByAcademicYear(currentYear?.academicYearID);
  const semesters = semesterQuery.data;
  const [semester, setSemester] = useState(null);
  const [classroom, setClassroom] = useState("");

  const token = JSON.parse(localStorage.getItem("token"));
  const teacherId = jwtDecode(token).teacherId;
  const teachingAssignmentQuery = useTeachingAssignmentsByTeacher({
    teacherId,
    semesterId: semester?.semesterID,
  });

  const teachingAssignments = teachingAssignmentQuery.data || [];
  const classQuery = useClasses();
  const classes = classQuery.data || [];
  const homeroomTeacherQuery = useHomeroomTeachers();
  const homeroomTeachers = homeroomTeacherQuery.data || [];
  const studentQuery = useStudentByClass({
    classId: classroom,
    semesterId: semester?.semesterID,
  });
  const students = studentQuery.data?.students || [];
  // console.log(students);
  // console.log(teachingAssignments);

  const [date, setDate] = useState(new Date());
  const [studentsData, setStudentsData] = useState([]);
  const [session, setSession] = useState("");
  const [viewMode, setViewMode] = useState("day");

  console.log(classQuery.error);

  const daysInMonth = new Date(
    date.getFullYear(),
    date.getMonth() + 1,
    0,
  ).getDate();

  const getMonday = (d) => {
    const date = new Date(d);
    const day = date.getDay();
    console.log(day);
    // Nếu là Chủ nhật (0) thì lùi về thứ 2 tuần hiện tại, còn lại lùi về thứ 2
    const diff = date.getDate() - day + (day === 0 ? -6 : 1);
    return new Date(date.setDate(diff));
  };

  const handleSubmit = () => {
    const monday = getMonday(date);
    const payload = studentsData.map((student) => ({
      classId: classroom,
      date: formatDate(monday),
      session: session,
      status: student.status || "C",
      note: student.note || "",
      studentId: student.studentId,
    }));
    console.log(payload);
  };

  useEffect(() => {
    if (semesters?.length > 0) {
      const now = new Date();
      const found = semesters.find(
        (sem) => new Date(sem.startDate) <= now && now <= new Date(sem.endDate),
      );
      setSemester(found || semesters[0]);
    }
  }, [semesters, currentYear]);

  useEffect(() => {
    if (students.length > 0) {
      setStudentsData(
        students.map((student) => ({
          ...student,
          status: student.status || "C",
          note: student.note || "",
        })),
      );
    } else {
      setStudentsData([]);
    }
  }, [students]);

  const handleStatusChange = (studentId, value) => {
    setStudentsData((prev) =>
      prev.map((s) =>
        s.studentId === studentId ? { ...s, status: value } : s,
      ),
    );
  };

  // Hàm xử lý thay đổi lý do
  const handleNoteChange = (studentId, value) => {
    setStudentsData((prev) =>
      prev.map((s) => (s.studentId === studentId ? { ...s, note: value } : s)),
    );
  };

  return (
    <Card className="mt-6 p-4">
      <div className="flex items-center">
        <div>
          <Select value={classroom} onValueChange={setClassroom}>
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="Chọn lớp" />
            </SelectTrigger>
            <SelectContent>
              {classes
                .filter((c) => {
                  const inTeaching = teachingAssignments.some(
                    (ta) => ta.classId === c.classId,
                  );
                  const inHomeroom = homeroomTeachers.some(
                    (ht) =>
                      ht.classId === c.classId &&
                      ht.teacherId == teacherId &&
                      semester?.semesterID === ht.semesterId,
                  );
                  return inTeaching || inHomeroom;
                })
                .map((c) => (
                  <SelectItem key={c.classId} value={c.classId}>
                    {c.className}
                  </SelectItem>
                ))}
            </SelectContent>
          </Select>
        </div>
        <div className="ml-4">
          <Select value={session} onValueChange={setSession}>
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="Chọn buổi" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="Sáng">Buổi sáng</SelectItem>
              <SelectItem value="Chiều">Buổi chiều</SelectItem>
            </SelectContent>
          </Select>
        </div>
        <div className="bg-muted text-muted-foreground ml-auto inline-flex h-10 items-center justify-center rounded-lg">
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

        {/* Dropdown chọn chế độ xem */}
        <div className="ml-4">
          <Select value={viewMode} onValueChange={setViewMode}>
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="Chế độ xem" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="day">Xem theo ngày</SelectItem>
              <SelectItem value="week">Xem theo tuần</SelectItem>
              <SelectItem value="month">Xem theo tháng</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>

      {/* Hiển thị dữ liệu phù hợp với chế độ xem */}
      <div className="relative mt-4">
        <div className="max-h-[500px] overflow-auto rounded-md border border-gray-200 shadow-sm">
          <div className="min-w-max">
            {viewMode === "day" && (
              <Table className="w-full">
                <TableHeader className="bg-gray-50">
                  <TableRow>
                    <TableHead className="min-w-[80px] border border-gray-200 text-center font-medium text-gray-700">
                      ID
                    </TableHead>
                    <TableHead className="min-w-[200px] border border-gray-200 text-center font-medium text-gray-700">
                      Họ và Tên
                    </TableHead>
                    <TableHead className="min-w-[150px] border border-gray-200 text-center font-medium text-gray-700">
                      Trạng Thái
                    </TableHead>
                    <TableHead className="min-w-[250px] border border-gray-200 text-center font-medium text-gray-700">
                      Lí do
                    </TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {students.map((student, index) => (
                    <TableRow
                      key={student.studentId}
                      className="hover:bg-gray-50"
                    >
                      <TableCell className="h-16 border border-gray-200 text-center">
                        {index + 1}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-200 font-medium">
                        {student.fullName}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-200 text-center">
                        <select
                          value={student.attendance}
                          onChange={(e) =>
                            handleStatusChange(
                              student.studentId,
                              e.target.value,
                            )
                          }
                          className="rounded-md border border-gray-300 px-3 py-1.5 text-sm shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500"
                        >
                          <option value="C">Có mặt</option>
                          <option value="P">Nghỉ có phép</option>
                          <option value="KP">Nghỉ không phép</option>
                          <option value="K">Lí do khác</option>
                        </select>
                      </TableCell>
                      <TableCell className="h-16 border border-gray-200">
                        <Input
                          type="text"
                          onChange={(e) =>
                            handleNoteChange(student.studentId, e.target.value)
                          }
                          disabled={student.status !== "K"}
                          className="disabled:bg-gray-100"
                        />
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
            {viewMode === "week" && (
              <Table className="w-full">
                <TableHeader className="bg-gray-100">
                  <TableRow>
                    <TableHead className="text-center">STT</TableHead>
                    <TableHead className="text-center">Họ và tên</TableHead>
                    {[...Array(6)].map((_, i) => (
                      <TableHead
                        key={i}
                        className="text-center"
                      >{`Thứ ${i + 2}`}</TableHead>
                    ))}
                    <TableHead className="text-center">Chủ nhật</TableHead>
                    <TableHead className="text-center">
                      Tổng ngày nghỉ
                    </TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {students.map((student, index) => (
                    <TableRow key={student.studentId}>
                      <TableCell>{index + 1}</TableCell>
                      <TableCell>{student.fullName}</TableCell>
                      {[...Array(7)].map((_, i) => (
                        <TableCell
                          key={i}
                          className="border border-gray-300 text-center"
                        >
                          <Input
                            type="text"
                            value={student.attendance[i]}
                            maxLength={2} // Giới hạn ký tự nhập vào
                            className="w-[50px] text-center"
                          />
                        </TableCell>
                      ))}
                      <TableCell className="border border-gray-300 text-center">
                        {
                          student.attendance.filter(
                            (status) => status === "P" || status === "KP",
                          ).length
                        }
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
            {viewMode === "month" && (
              <Table className="w-full border-collapse border-gray-300">
                <TableHeader className="bg-gray-100">
                  <TableRow>
                    <TableHead className="border border-gray-300">
                      STT
                    </TableHead>
                    <TableHead className="min-w-[200px] border border-gray-300">
                      Họ và tên
                    </TableHead>
                    {[...Array(daysInMonth)].map((_, i) => (
                      <TableHead
                        key={i}
                        className="min-w-[25px] border border-gray-300 text-center"
                      >{`${i + 1}`}</TableHead>
                    ))}
                    <TableHead className="border border-gray-300">
                      Tổng ngày nghỉ
                    </TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {students.map((student, index) => (
                    <TableRow key={student.id}>
                      <TableCell className="border border-gray-300 text-center">
                        {index + 1}
                      </TableCell>
                      <TableCell className="border border-gray-300">
                        {student.name}
                      </TableCell>
                      {[...Array(daysInMonth)].map((_, i) => (
                        <TableCell
                          key={i}
                          className="border border-gray-300 text-center"
                        >
                          <Input
                            type="text"
                            value={student.attendance[i]}
                            maxLength={2} // Giới hạn ký tự nhập vào
                            className="w-[50px] text-center"
                          />
                        </TableCell>
                      ))}
                      <TableCell className="border border-gray-300 text-center">
                        {
                          student.attendance.filter(
                            (status) => status === "P" || status === "KP",
                          ).length
                        }
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            )}
          </div>
        </div>
        {/* Thanh cuộn ngang luôn hiển thị */}
      </div>

      <div className="mt-6 flex justify-end">
        <Button
          className="min-w-[150px] bg-blue-600 hover:bg-blue-700 focus-visible:ring-blue-500"
          onClick={handleSubmit}
        >
          Lưu điểm danh
        </Button>
      </div>
    </Card>
  );
}
