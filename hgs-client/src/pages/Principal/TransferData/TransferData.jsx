import React, { useState } from "react";
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
import toast from "react-hot-toast";
import { Checkbox } from "@/components/ui/checkbox";

export default function TransferData() {
  // State cho loading, modal, dữ liệu lớp, học sinh, chế độ chuyển
  const [loading, setLoading] = useState(false);
  const [mode, setMode] = useState("class"); // "class" hoặc "student"
  const [open, setOpen] = useState(false);
  const [fromClass, setFromClass] = useState("");
  const [toClass, setToClass] = useState("");
  const [selectedStudents, setSelectedStudents] = useState([]);
  const [students, setStudents] = useState([
    // Dữ liệu mẫu, thực tế lấy từ API
    { id: 1, name: "Nguyễn Văn A", class: "6A" },
    { id: 2, name: "Trần Thị B", class: "6A" },
    { id: 3, name: "Lê Văn C", class: "6B" },
  ]);
  const [classes, setClasses] = useState([
    // Dữ liệu mẫu, thực tế lấy từ API
    { id: "6A", name: "6A" },
    { id: "6B", name: "6B" },
    { id: "7A", name: "7A" },
    { id: "7B", name: "7B" },
  ]);
  // Dữ liệu mẫu danh sách lớp
  const [classList, setClassList] = useState([
    { id: "6A", name: "6A", size: 40, qualified: 38, toClass: "" },
    { id: "6B", name: "6B", size: 42, qualified: 40, toClass: "" },
    { id: "7A", name: "7A", size: 39, qualified: 37, toClass: "" },
  ]);

  const [grade, setGrade] = useState("");
  const [className, setClassName] = useState("");

  // Xử lý chuyển dữ liệu
  const handleTransfer = () => {
    setLoading(true);
    setTimeout(() => {
      setLoading(false);
      setOpen(false);
      toast.success("Chuyển dữ liệu thành công!");
    }, 1500);
  };

  // UI loading skeleton
  if (loading) {
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
            <SelectItem value="6A">6A</SelectItem>
            <SelectItem value="7A">7A</SelectItem>
          </SelectContent>
        </Select>
        <Button onClick={() => setOpen(true)} className="ml-auto">
          Thực hiện chuyển dữ liệu
        </Button>
      </div>

      {/* Bảng danh sách các lớp */}
      {mode === "class" && (
        <div className="mt-4">
          <p className="font-semibold">Danh sách các lớp</p>
          <Card className="mt-4 border shadow">
            <CardContent className="p-0">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead className="w-12 text-center">STT</TableHead>
                    <TableHead className="text-center">Lớp hiện tại</TableHead>
                    <TableHead className="text-center">Sĩ số</TableHead>
                    <TableHead className="text-center">
                      Đủ điều kiện lên lớp
                    </TableHead>
                    <TableHead className="text-center">
                      Lớp chuyển đến
                    </TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {classList.map((cls, idx) => (
                    <TableRow key={cls.id}>
                      <TableCell className="text-center">{idx + 1}</TableCell>
                      <TableCell className="text-center">{cls.name}</TableCell>
                      <TableCell className="text-center">{cls.size}</TableCell>
                      <TableCell className="text-center">
                        {cls.qualified}
                      </TableCell>
                      <TableCell className="text-center">
                        <Select
                          value={cls.toClass}
                          onValueChange={(val) => {
                            setClassList((prev) =>
                              prev.map((c) =>
                                c.id === cls.id ? { ...c, toClass: val } : c,
                              ),
                            );
                          }}
                        >
                          <SelectTrigger className="w-[120px]">
                            <SelectValue placeholder="Chọn lớp" />
                          </SelectTrigger>
                          <SelectContent>
                            {classes
                              .filter((c) => c.id !== cls.id)
                              .map((c) => (
                                <SelectItem key={c.id} value={c.id}>
                                  {c.name}
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

      {mode === "student" && (
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
                    {students.map((student) => (
                      <TableRow key={student.id}>
                        <TableCell>
                          <Checkbox
                            checked={selectedStudents.some(
                              (s) => s.id === student.id,
                            )}
                            onCheckedChange={(checked) => {
                              if (checked) {
                                setSelectedStudents([
                                  ...selectedStudents,
                                  { ...student, toClass: "" },
                                ]);
                              } else {
                                setSelectedStudents(
                                  selectedStudents.filter(
                                    (s) => s.id !== student.id,
                                  ),
                                );
                              }
                            }}
                          />
                        </TableCell>
                        <TableCell>{student.name}</TableCell>
                        <TableCell>
                          <Select
                            value={
                              selectedStudents.find((s) => s.id === student.id)
                                ?.toClass || ""
                            }
                            onValueChange={(val) => {
                              setSelectedStudents((prev) =>
                                prev.map((s) =>
                                  s.id === student.id
                                    ? { ...s, toClass: val }
                                    : s,
                                ),
                              );
                            }}
                            disabled={
                              !selectedStudents.some((s) => s.id === student.id)
                            }
                          >
                            <SelectTrigger className="w-[120px]">
                              <SelectValue placeholder="Chọn lớp" />
                            </SelectTrigger>
                            <SelectContent>
                              {classes.map((c) => (
                                <SelectItem key={c.id} value={c.id}>
                                  {c.name}
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
      <Dialog open={open} onOpenChange={setOpen}>
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
      </Dialog>
    </div>
  );
}
