import { useState } from "react";
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

export default function AttendanceTable() {
  const [date, setDate] = useState(new Date());
  const [grade, setGrade] = useState("");
  const [classroom, setClassroom] = useState("");
  const [session, setSession] = useState("");
  const [selectAll, setSelectAll] = useState(false);
  const [viewMode, setViewMode] = useState("day"); // State để chọn chế độ xem

  const daysInMonth = new Date(
    date.getFullYear(),
    date.getMonth() + 1,
    0,
  ).getDate();

  const [students, setStudents] = useState(
    data.map((s) => ({
      ...s,
      attendance: Array(daysInMonth).fill("-"), // Mặc định "Có mặt"
    })),
  );

  const updateStatus = (id, status) => {
    setStudents(students.map((s) => (s.id === id ? { ...s, status } : s)));
  };

  const toggleSelectAll = () => {
    setSelectAll(!selectAll);
    setStudents(
      students.map((s) => ({
        ...s,
        status: selectAll && "C",
      })),
    );
  };

  const updateAttendance = (id, dayIndex, status) => {
    setStudents((prev) =>
      prev.map((s) =>
        s.id === id
          ? {
              ...s,
              attendance: s.attendance.map((att, i) =>
                i === dayIndex ? status : att,
              ),
            }
          : s,
      ),
    );
  };

  console.log(students);

  return (
    <Card className="mt-6 p-4">
      <AttendanceHeader
        date={date}
        setDate={(newDate) => {
          const today = new Date();
          today.setHours(0, 0, 0, 0);
          if (newDate <= today) {
            setDate(newDate);
          }
        }}
        grade={grade}
        setGrade={setGrade}
        classroom={classroom}
        setClassroom={setClassroom}
        session={session}
        setSession={setSession}
        selectAll={selectAll}
        toggleSelectAll={toggleSelectAll}
      />

      {/* Dropdown chọn chế độ xem */}
      <div className="mb-4 flex justify-end">
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

      {/* Hiển thị dữ liệu phù hợp với chế độ xem */}
      <div className="relative">
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
                  {students.map((student) => (
                    <TableRow key={student.id} className="hover:bg-gray-50">
                      <TableCell className="h-16 border border-gray-200 text-center">
                        {student.id}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-200 font-medium">
                        {student.name}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-200 text-center">
                        <select
                          value={student.attendance}
                          onChange={(e) =>
                            updateStatus(student.id, e.target.value)
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
                    <TableRow key={student.id}>
                      <TableCell>{index + 1}</TableCell>
                      <TableCell>{student.name}</TableCell>
                      {[...Array(7)].map((_, i) => (
                        <TableCell
                          key={i}
                          className="border border-gray-300 text-center"
                        >
                          <Input
                            type="text"
                            value={student.attendance[i]}
                            onChange={(e) =>
                              updateAttendance(
                                student.id,
                                i,
                                e.target.value.toUpperCase(),
                              )
                            }
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
                            onChange={(e) =>
                              updateAttendance(
                                student.id,
                                i,
                                e.target.value.toUpperCase(),
                              )
                            }
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
          onClick={() => {
            // Add your save logic here
            console.log("Saving attendance data:", students);
          }}
        >
          Lưu điểm danh
        </Button>
      </div>
    </Card>
  );
}
