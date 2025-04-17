import React, { useState } from "react";
import toast, { Toaster } from "react-hot-toast";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { PlusCircle, Pencil, Trash2 } from "lucide-react";
import { useClasses, useGradeLevels } from "@/services/common/queries";

export default function ClassManagement() {
  const [classes, setClasses] = useState([]);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [editingClass, setEditingClass] = useState(null);

  const classQuery = useClasses();
  const gradelevelQuery = useGradeLevels();

  const handleSubmit = (e) => {
    e.preventDefault();
    const formData = new FormData(e.target);
    const classData = {
      id: editingClass ? editingClass.id : Date.now(),
      className: formData.get("className"),
      teacherName: formData.get("teacherName"),
      gradeId: formData.get("gradeId"),
      studentCount: editingClass ? editingClass.studentCount : 0,
    };

    if (editingClass) {
      setClasses(
        classes.map((c) => (c.id === editingClass.id ? classData : c)),
      );
      toast.success("Cập nhật lớp học thành công");
    } else {
      setClasses([...classes, classData]);
      toast.success("Thêm lớp học thành công");
    }
    setIsDialogOpen(false);
    setEditingClass(null);
  };

  const handleDelete = (id) => {
    if (confirm("Bạn có chắc chắn muốn xóa lớp học này không?")) {
      setClasses(classes.filter((c) => c.id !== id));
      toast.success("Xóa lớp học thành công");
    }
  };

  return (
    <div className="p-6">
      <Toaster position="top-right" />
      <Card>
        <CardHeader>
          <CardTitle className="text-2xl font-bold">Quản Lý Lớp Học</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="mb-4">
            <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
              <DialogTrigger asChild>
                <Button className="flex items-center gap-2">
                  <PlusCircle className="h-4 w-4" />
                  Thêm Lớp Học Mới
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>
                    {editingClass ? "Chỉnh Sửa Lớp Học" : "Thêm Lớp Học Mới"}
                  </DialogTitle>
                </DialogHeader>
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div className="grid w-full gap-2">
                    <Label htmlFor="gradeId">Khối</Label>
                    <select
                      id="gradeId"
                      name="gradeId"
                      className="border-input bg-background ring-offset-background placeholder:text-muted-foreground focus-visible:ring-ring flex h-10 w-full rounded-md border px-3 py-2 text-sm file:border-0 file:bg-transparent file:text-sm file:font-medium focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:outline-none disabled:cursor-not-allowed disabled:opacity-50"
                      defaultValue={editingClass?.gradeId}
                      required
                    >
                      <option value="">Chọn khối</option>
                      {gradelevelQuery?.data?.map((grade) => (
                        <option key={grade.id} value={grade.id}>
                          {grade.gradeName}
                        </option>
                      ))}
                    </select>
                  </div>
                  <div className="grid w-full gap-2">
                    <Label htmlFor="className">Tên Lớp</Label>
                    <Input
                      id="className"
                      name="className"
                      defaultValue={editingClass?.className}
                      required
                    />
                  </div>
                  <div className="grid w-full gap-2">
                    <Label htmlFor="teacherName">Giáo Viên Chủ Nhiệm</Label>
                    <Input
                      id="teacherName"
                      name="teacherName"
                      defaultValue={editingClass?.teacherName}
                      required
                    />
                  </div>
                  <Button type="submit" className="w-full">
                    {editingClass ? "Cập Nhật" : "Thêm"} Lớp Học
                  </Button>
                </form>
              </DialogContent>
            </Dialog>
          </div>

          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Khối</TableHead>
                <TableHead>Tên Lớp</TableHead>
                <TableHead>Giáo Viên Chủ Nhiệm</TableHead>
                <TableHead>Số Học Sinh</TableHead>
                <TableHead>Thao Tác</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {classQuery?.data?.map((classItem) => (
                <TableRow key={classItem.id}>
                  <TableCell>
                    {
                      gradelevelQuery?.data?.find(
                        (grade) => grade.id === classItem.gradeId,
                      )?.gradeName
                    }
                  </TableCell>
                  <TableCell>{classItem.className}</TableCell>
                  <TableCell>{classItem.teacherName}</TableCell>
                  <TableCell>{classItem.studentCount}</TableCell>
                  <TableCell>
                    <div className="flex gap-2">
                      <Button
                        variant="outline"
                        size="icon"
                        onClick={() => {
                          setEditingClass(classItem);
                          setIsDialogOpen(true);
                        }}
                        title="Chỉnh sửa"
                      >
                        <Pencil className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="destructive"
                        size="icon"
                        onClick={() => handleDelete(classItem.id)}
                        title="Xóa"
                      >
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    </div>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  );
}
