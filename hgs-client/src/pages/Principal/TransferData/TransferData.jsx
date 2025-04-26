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
  const [selectedClasses, setSelectedClasses] = useState([]);
  const [selectedStudents, setSelectedStudents] = useState([]);
  const [pendingGrade, setPendingGrade] = useState(""); // Khối chờ xác nhận
  const [confirmChangeGrade, setConfirmChangeGrade] = useState(false);
  const [pendingClassName, setPendingClassName] = useState(""); // Lớp chờ xác nhận
  const [confirmChangeClass, setConfirmChangeClass] = useState(false);

  const classQuery = useClasses();
  const classes = classQuery.data || [];
  const studentQuery = usePreviousYearStudents(currentYear?.academicYearID);
  const students = studentQuery.data || [];

  const [grade, setGrade] = useState("");
  const [className, setClassName] = useState("");
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
            if (
              (mode === "class" && selectedClasses.length > 0) ||
              (mode === "student" && selectedStudents.length > 0)
            ) {
              setPendingGrade(val);
              setConfirmChangeGrade(true);
            } else {
              setGrade(val);
              setClassName("");
              setSelectedClasses([]);
              setSelectedStudents([]);
            }
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
          onValueChange={(val) => {
            if (mode === "student" && selectedStudents.length > 0) {
              setPendingClassName(val);
              setConfirmChangeClass(true);
            } else {
              setClassName(val);
              setSelectedStudents([]);
            }
          }}
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
                <TableHeader>
                  <TableRow>
                    <TableHead className="w-12 text-center">
                      <Checkbox
                        checked={
                          classes.filter((c) => c.className[0] == grade)
                            .length > 0 &&
                          selectedClasses.length ===
                            classes.filter((c) => c.className[0] == grade)
                              .length
                        }
                        indeterminate={
                          selectedClasses.length > 0 &&
                          selectedClasses.length <
                            classes.filter((c) => c.className[0] == grade)
                              .length
                        }
                        onCheckedChange={(checked) => {
                          if (checked) {
                            setSelectedClasses(
                              classes
                                .filter((c) => c.className[0] == grade)
                                .map((cls) => ({
                                  classId: cls.classId,
                                  targetClassId: "",
                                })),
                            );
                          } else {
                            setSelectedClasses([]);
                          }
                        }}
                      />
                    </TableHead>
                    <TableHead className="w-50 text-center">STT</TableHead>
                    <TableHead className="w-50 text-center">
                      Lớp hiện tại
                    </TableHead>
                    <TableHead className="w-50 text-center">
                      Số học sinh
                    </TableHead>
                    <TableHead className="w-auto">Lớp chuyển đến</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {classes
                    .filter((c) => c.className[0] == grade)
                    .map((cls, idx) => (
                      <TableRow key={cls.classId}>
                        <TableCell className="text-center">
                          <Checkbox
                            checked={selectedClasses.some(
                              (s) => s.classId === cls.classId,
                            )}
                            onCheckedChange={(checked) => {
                              if (checked) {
                                setSelectedClasses([
                                  ...selectedClasses,
                                  { classId: cls.classId, targetClassId: "" },
                                ]);
                              } else {
                                setSelectedClasses(
                                  selectedClasses.filter(
                                    (s) => s.classId !== cls.classId,
                                  ),
                                );
                              }
                            }}
                          />
                        </TableCell>
                        <TableCell className="text-center">{idx + 1}</TableCell>
                        <TableCell className="text-center">
                          {cls.className}
                        </TableCell>
                        <TableCell className="text-center">
                          {
                            students?.filter(
                              (s) => s.className == cls.className,
                            ).length
                          }
                        </TableCell>
                        <TableCell className="text-center">
                          <Select
                            value={
                              selectedClasses.find(
                                (s) => s.classId === cls.classId,
                              )?.targetClassId || ""
                            }
                            onValueChange={(val) => {
                              setSelectedClasses((prev) =>
                                prev.map((s) =>
                                  s.classId === cls.classId
                                    ? { ...s, targetClassId: val }
                                    : s,
                                ),
                              );
                            }}
                            disabled={
                              !selectedClasses.some(
                                (s) => s.classId === cls.classId,
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
                                    c.className[0] == +grade + 1 &&
                                    c.status === "Hoạt động",
                                )
                                .map((c) => (
                                  <SelectItem key={c.classId} value={c.classId}>
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
                                      value={c.classId}
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
      <Dialog open={confirmChangeGrade} onOpenChange={setConfirmChangeGrade}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle className="mb-4 text-2xl">
              Xác nhận đổi khối
            </DialogTitle>
          </DialogHeader>
          <div>
            {mode === "class"
              ? "Bạn đang có lớp được chọn để chuyển dữ liệu."
              : "Bạn đang có học sinh được chọn để chuyển dữ liệu."}
            <br />
            Nếu đổi khối, danh sách {mode === "class" ? "lớp" : "học sinh"} đã
            chọn sẽ bị xóa.
            <br />
            Bạn có chắc chắn muốn đổi khối không?
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setConfirmChangeGrade(false)}
            >
              Hủy
            </Button>
            <Button
              onClick={() => {
                setGrade(pendingGrade);
                setClassName("");
                setSelectedClasses([]);
                setSelectedStudents([]);
                setConfirmChangeGrade(false);
              }}
            >
              Xác nhận
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Dialog open={confirmChangeClass} onOpenChange={setConfirmChangeClass}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle className="mb-4 text-2xl">
              Xác nhận đổi lớp
            </DialogTitle>
          </DialogHeader>
          <div>
            Bạn đang có học sinh được chọn để chuyển dữ liệu.
            <br />
            Nếu đổi lớp, danh sách học sinh đã chọn sẽ bị xóa.
            <br />
            Bạn có chắc chắn muốn đổi lớp không?
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setConfirmChangeClass(false)}
            >
              Hủy
            </Button>
            <Button
              onClick={() => {
                setClassName(pendingClassName);
                setSelectedStudents([]);
                setConfirmChangeClass(false);
              }}
            >
              Xác nhận
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
