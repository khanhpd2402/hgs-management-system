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

export default function AttendanceTable() {
  const [date, setDate] = useState(new Date());
  const [grade, setGrade] = useState("");
  const [classroom, setClassroom] = useState("");
  const [session, setSession] = useState("");
  const [selectAll, setSelectAll] = useState(false);
  const [students, setStudents] = useState([
    {
      id: "1",
      name: "Vũ Mai Tuyết Anh",
      status: "KP",
    },
    {
      id: "2",
      name: "Vũ Hải Chính",
      status: "KP",
    },
    {
      id: "3",
      name: "Trần Thị Phương Dung",
      status: "KP",
    },
    {
      id: "4",
      name: "Nguyễn Hoàng Duy",
      status: "KP",
    },
    {
      id: "5",
      name: "Bùi Tiến Đại Dương",
      status: "KP",
    },
    {
      id: "6",
      name: "Nguyễn Hoàng Hải",
      status: "KP",
    },
    {
      id: "7",
      name: "Vũ Xuân Hải",
      status: "KP",
    },
    {
      id: "8",
      name: "Bùi Quang Huy",
      status: "KP",
    },
    {
      id: "9",
      name: "Phạm Quang Huy",
      status: "KP",
    },
    {
      id: "10",
      name: "Trịnh Gia Huy",
      status: "KP",
    },
    {
      id: "11",
      name: "Nguyễn Thị Thoại Khanh",
      status: "KP",
    },
    {
      id: "12",
      name: "Trần Xuân Kiên",
      status: "KP",
    },
    {
      id: "13",
      name: "Nguyễn Hoàng Long",
      status: "KP",
    },
    {
      id: "14",
      name: "Vũ Gia Lộc",
      status: "KP",
    },
    {
      id: "15",
      name: "Vũ Thị Trà My",
      status: "KP",
    },
    {
      id: "16",
      name: "Phạm Hoài Nam",
      status: "KP",
    },
    {
      id: "17",
      name: "Trần Thị Huyền Nga",
      status: "KP",
    },
    {
      id: "18",
      name: "Nguyễn Khánh Ngọc",
      status: "KP",
    },
    {
      id: "19",
      name: "Vũ Duy Phúc",
      status: "KP",
    },
    {
      id: "20",
      name: "Tống Thị Bích Phượng",
      status: "KP",
    },
    {
      id: "21",
      name: "Bùi Minh Quang",
      status: "KP",
    },
    {
      id: "22",
      name: "Nguyễn Hoàng Quân",
      status: "KP",
    },
    {
      id: "23",
      name: "Phạm Hoàng Quân",
      status: "KP",
    },
    {
      id: "24",
      name: "Vũ Đức Minh Quân",
      status: "KP",
    },
    {
      id: "25",
      name: "Nguyễn Phương Thanh",
      status: "KP",
    },
    {
      id: "26",
      name: "Trần Phương Thảo",
      status: "KP",
    },
    {
      id: "27",
      name: "Mai Đức Thiện",
      status: "KP",
    },
    {
      id: "28",
      name: "Nguyễn Thị Minh Thư",
      status: "KP",
    },
    {
      id: "29",
      name: "Phạm Nguyễn Minh Thư",
      status: "KP",
    },
    {
      id: "30",
      name: "Trần Thủy Tiên",
      status: "KP",
    },
    {
      id: "31",
      name: "Trần Duy Tiến",
      status: "KP",
    },
    {
      id: "32",
      name: "Lê Quang Trưởng",
      status: "KP",
    },
    {
      id: "33",
      name: "Nguyễn Thị Tươi",
      status: "KP",
    },
    {
      id: "34",
      name: "Nguyễn Thị Kiều Vy",
      status: "KP",
    },
    {
      id: "35",
      name: "Nguyễn Tường Vy",
      status: "KP",
    },
    {
      id: "36",
      name: "Bùi Hải Yến",
      status: "KP",
    },
    {
      id: "37",
      name: "Trần Đình Khánh Chi",
      status: "KP",
    },
  ]);

  const updateStatus = (id, status) => {
    setStudents(students.map((s) => (s.id === id ? { ...s, status } : s)));
  };

  const toggleSelectAll = () => {
    setSelectAll(!selectAll);
    setStudents(
      students.map((s) => ({
        ...s,
        status: selectAll ? "present" : "absent_excused",
      })),
    );
  };

  return (
    <Card className="relative mt-6 p-4">
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

      <div className="max-h-[600px] overflow-auto border border-gray-300">
        <Table className="min-w-full">
          <TableHeader className="bg-gray-100">
            <TableRow>
              <TableHead className="border border-gray-300 text-center whitespace-nowrap">
                ID
              </TableHead>
              <TableHead className="border border-gray-300 text-center whitespace-nowrap">
                Họ và Tên
              </TableHead>
              <TableHead className="border border-gray-300 text-center whitespace-nowrap">
                Trạng Thái
              </TableHead>
              <TableHead className="border border-gray-300 text-center whitespace-nowrap">
                Lí do
              </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {students.map((student) => (
              <TableRow key={student.id} className="divide-x divide-gray-300">
                <TableCell className="h-16 border border-gray-300 text-center whitespace-nowrap">
                  {student.id}
                </TableCell>
                <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                  {student.name}
                </TableCell>
                <TableCell className="h-16 border border-gray-300 text-center whitespace-nowrap">
                  <select
                    value={student.status}
                    onChange={(e) => updateStatus(student.id, e.target.value)}
                    className="rounded border px-2 py-1"
                  >
                    <option value="C">Có mặt</option>
                    <option value="P">Nghỉ có phép</option>
                    <option value="KP">Nghỉ không phép</option>
                    <option value="K">Lí do khác</option>
                  </select>
                </TableCell>
                <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                  <Input type="text" disabled={student.status !== "K"} />
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      <div className="mt-4 flex justify-end">
        <Button>Lưu điểm danh</Button>
      </div>
    </Card>
  );
}
