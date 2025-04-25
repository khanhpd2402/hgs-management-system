import { useEffect, useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem,
} from "@/components/ui/select";
import { Checkbox } from "@/components/ui/checkbox";
import { useClasses } from "@/services/common/queries";
import { usePreviousYearStudents } from "@/services/principal/queries";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";

export default function TransferData() {
  const { currentYear } = useLayout();
  const [mode, setMode] = useState("class"); // "class" hoặc "student"
  const [open, setOpen] = useState(false);

  const [selectedStudents, setSelectedStudents] = useState([]);

  const classQuery = useClasses();
  const classes = classQuery.data;
  const studentQuery = usePreviousYearStudents(currentYear?.academicYearID);
  const students = studentQuery.data;

  const [grade, setGrade] = useState("");
  const [className, setClassName] = useState("");
  const [classList, setClassList] = useState([]);

  useEffect(() => {
    if (mode === "class" && grade && classes) {
      const filtered = classes
        .filter((c) => c.className[0] == grade)
        .map((c) => ({
          ...c,
          toClass: "",
        }));

      setClassList(filtered);
    }
  }, [mode, grade, classes]);
  console.log(selectedStudents);

  // Xử lý chuyển dữ liệu
  const handleTransfer = () => {};

  // UI loading skeleton
  if (classQuery.isLoading) {
    return (
      <Card className="mt-6 border shadow">
        <CardHeader>
          <CardTitle>Chuyển dữ liệu học sinh</CardTitle>
        </CardHeader>
        <CardContent>
          <Skeleton className="mb-4 h-10 w-1/3" />
          <Skeleton className="mb-2 h-8 w-full" />
          <Skeleton className="mb-2 h-8 w-full" />
          <Skeleton className="h-8 w-full" />
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="mt-6">
      <h2 className="text-2xl font-bold">
        Chuyển dữ liệu học sinh qua năm mới
      </h2>
      <div className="mt-4 flex gap-4">
        <Select
          value={mode}
          onValueChange={(val) => {
            setMode(val);
            setGrade("");
            setClassName("");
          }}
        >
          <SelectTrigger className="w-[200px]">
            <SelectValue placeholder="Chọn chế độ chuyển" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="class">Chuyển cả lớp</SelectItem>
            <SelectItem value="student">Chuyển lẻ học sinh</SelectItem>
          </SelectContent>
        </Select>
        <Select
          value={grade}
          onValueChange={(val) => {
            setGrade(val);
            setClassName("");
          }}
        >
          <SelectTrigger className="w-[200px]">
            <SelectValue placeholder="Chọn khối" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="6">Khối 6</SelectItem>
            <SelectItem value="7">Khối 7</SelectItem>
            <SelectItem value="8">Khối 8</SelectItem>
            <SelectItem value="9">Khối 9</SelectItem>
          </SelectContent>
        </Select>
        <Select
          value={className}
          onValueChange={setClassName}
          disabled={mode !== "student" || !grade}
        >
          <SelectTrigger className="w-[200px]">
            <SelectValue placeholder="Chọn lớp" />
          </SelectTrigger>
          <SelectContent>
            {classes.map((item) => {
              if (item.className[0] == grade) {
                return (
                  <SelectItem key={item.classId} value={item.className}>
                    {item.className}
                  </SelectItem>
                );
              }
            })}
          </SelectContent>
        </Select>
        <Button onClick={() => setOpen(true)} className="ml-auto">
          Thực hiện chuyển dữ liệu
        </Button>
      </div>

      {/* Bảng danh sách các lớp */}
      {mode === "class" && grade && (
        <div className="mt-4">
          <p className="font-semibold">Danh sách các lớp</p>
          <Card className="mt-4 border shadow">
            <CardContent className="p-0">
              <Table>
                <TableHeader>{/* ... existing code ... */}</TableHeader>
                <TableBody>
                  {classList.map((cls, idx) => (
                    <TableRow key={cls.classId}>
                      <TableCell className="text-center">{idx + 1}</TableCell>
                      <TableCell className="text-center">
                        {cls.className}
                      </TableCell>
                      <TableCell className="text-center">
                        {
                          students.filter((s) => s.className == cls.className)
                            .length
                        }
                      </TableCell>
                      <TableCell className="text-center">
                        {cls.qualified}
                      </TableCell>
                      <TableCell className="text-center">
                        <Select
                          value={cls.toClass}
                          onValueChange={(val) => {
                            setClassList((prev) =>
                              prev.map((c) =>
                                c.className === cls.className
                                  ? { ...c, toClass: val }
                                  : c,
                              ),
                            );
                          }}
                        >
                          <SelectTrigger className="w-[120px]">
                            <SelectValue placeholder="Chọn lớp" />
                          </SelectTrigger>
                          <SelectContent>
                            {classes
                              .filter(
                                (c) =>
                                  c.className[0] == +grade + 1 &&
                                  c.status === "Hoạt động",
                              )
                              .map((c) => (
                                <SelectItem key={c.classId} value={c.className}>
                                  {c.className}
                                </SelectItem>
                              ))}
                          </SelectContent>
                        </Select>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        </div>
      )}

      {mode === "student" && grade && className && (
        <div>
          <div className="mt-4 font-medium">Chọn học sinh cần chuyển:</div>
          <div className="overflow-x-auto">
            <Card className="mt-4 border shadow">
              <CardContent className="p-0">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead className="w-12"></TableHead>
                      <TableHead>Họ và tên</TableHead>
                      <TableHead>Lớp mới</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {students
                      .filter((s) => s.className === className)
                      .map((student) => (
                        <TableRow key={student.studentId}>
                          <TableCell>
                            <Checkbox
                              checked={selectedStudents.some(
                                (s) => s.studentId === student.studentId,
                              )}
                              onCheckedChange={(checked) => {
                                if (checked) {
                                  setSelectedStudents([
                                    ...selectedStudents,
                                    {
                                      studentId: student.studentId,
                                      classId: student.classId,
                                      toClass: "",
                                    },
                                  ]);
                                } else {
                                  setSelectedStudents(
                                    selectedStudents.filter(
                                      (s) => s.studentId !== student.studentId,
                                    ),
                                  );
                                }
                              }}
                            />
                          </TableCell>
                          <TableCell>{student.studentName}</TableCell>
                          <TableCell>
                            <Select
                              value={
                                selectedStudents.find(
                                  (s) => s.studentId === student.studentId,
                                )?.toClass || ""
                              }
                              onValueChange={(val) => {
                                setSelectedStudents((prev) =>
                                  prev.map((s) =>
                                    s.studentId === student.studentId
                                      ? { ...s, toClass: val }
                                      : s,
                                  ),
                                );
                              }}
                              disabled={
                                !selectedStudents.some(
                                  (s) => s.studentId === student.studentId,
                                )
                              }
                            >
                              <SelectTrigger className="w-[120px]">
                                <SelectValue placeholder="Chọn lớp" />
                              </SelectTrigger>
                              <SelectContent>
                                {classes
                                  .filter(
                                    (c) =>
                                      c.status === "Hoạt động" &&
                                      c.className[0] == +grade + 1,
                                  )
                                  .map((c) => (
                                    <SelectItem
                                      key={c.classId}
                                      value={c.className}
                                    >
                                      {c.className}
                                    </SelectItem>
                                  ))}
                              </SelectContent>
                            </Select>
                          </TableCell>
                        </TableRow>
                      ))}
                  </TableBody>
                </Table>
              </CardContent>
            </Card>
          </div>
        </div>
      )}

      {/* Chuyển cả lớp */}

      {/* Modal xác nhận chuyển */}
      {/* <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Xác nhận chuyển dữ liệu</DialogTitle>
          </DialogHeader>
          <div>
            {mode === "class" ? (
              <div>
                Bạn có chắc chắn muốn chuyển <b>toàn bộ học sinh</b> từ lớp{" "}
                <b>{fromClass}</b> sang lớp <b>{toClass}</b>?
              </div>
            ) : (
              <div>
                Bạn có chắc chắn muốn chuyển <b>{selectedStudents.length}</b>{" "}
                học sinh đã chọn sang các lớp mới?
              </div>
            )}
          </div>
          <DialogFooter>
            <Button variant="outline" onClick={() => setOpen(false)}>
              Hủy
            </Button>
            <Button onClick={handleTransfer}>Xác nhận</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog> */}
    </div>
  );
}
