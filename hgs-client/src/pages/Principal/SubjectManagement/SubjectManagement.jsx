import React, { useState } from "react";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Check, PlusCircle, Search } from "lucide-react";

export default function SubjectManagement() {
  const [subjects, setSubjects] = useState([
    {
      id: 1,
      name: "Toán",
      type: "Tính điểm",
      coefficient: 2,
      periodsHK1: 5,
      periodsHK2: 5,
    },
    {
      id: 2,
      name: "Vật lý",
      type: "Tính điểm",
      coefficient: 1,
      periodsHK1: 3,
      periodsHK2: 3,
    },
    {
      id: 3,
      name: "Hóa học",
      type: "Tính điểm",
      coefficient: 1,
      periodsHK1: 2,
      periodsHK2: 2,
    },
    {
      id: 4,
      name: "Sinh học",
      type: "Tính điểm",
      coefficient: 1,
      periodsHK1: 2,
      periodsHK2: 2,
    },
    {
      id: 5,
      name: "Tiếng Anh",
      type: "Tính điểm",
      coefficient: 1,
      periodsHK1: 3,
      periodsHK2: 3,
    },
  ]);

  const [searchTerm, setSearchTerm] = useState("");

  // Filter subjects based on search term
  const filteredSubjects = subjects.filter((subject) =>
    subject.name.toLowerCase().includes(searchTerm.toLowerCase()),
  );

  return (
    <div className="container mx-auto py-6">
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <div>
            <CardTitle className="text-2xl">Quản lý môn học</CardTitle>
            <CardDescription>
              Quản lý danh sách các môn học trong trường
            </CardDescription>
          </div>
          <div className="flex items-center gap-4">
            <div className="relative">
              <Search className="text-muted-foreground absolute top-2.5 left-2.5 h-4 w-4" />
              <Input
                type="search"
                placeholder="Tìm kiếm môn học..."
                className="w-64 pl-8"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
            </div>
            <Button className="flex items-center gap-1" variant="default">
              <Check className="h-4 w-4" />
              Lưu thay đổi
            </Button>
            <Dialog>
              <DialogTrigger asChild>
                <Button className="flex items-center gap-1">
                  <PlusCircle className="h-4 w-4" />
                  Thêm môn học
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Thêm môn học mới</DialogTitle>
                  <DialogDescription>
                    Điền thông tin môn học mới vào form dưới đây
                  </DialogDescription>
                </DialogHeader>
                <div className="grid gap-4 py-4">
                  <div className="grid grid-cols-4 items-center gap-4">
                    <Label htmlFor="name">Tên môn học</Label>
                    <Input id="name" className="col-span-3" />
                  </div>
                  <div className="grid grid-cols-4 items-center gap-4">
                    <Label htmlFor="type">Kiểu môn</Label>
                    <Select>
                      <SelectTrigger className="col-span-3">
                        <SelectValue placeholder="Chọn kiểu môn" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="Tính điểm">Tính điểm</SelectItem>
                        <SelectItem value="Nhận xét">Nhận xét</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                  <div className="grid grid-cols-4 items-center gap-4">
                    <Label htmlFor="coefficient" className="text-right">
                      Hệ số
                    </Label>
                    <Input
                      id="coefficient"
                      type="text"
                      className="col-span-3"
                    />
                  </div>
                  <div className="grid grid-cols-4 items-center gap-4">
                    <Label htmlFor="periodsHK1" className="text-left">
                      Số tiết/tuần HK1
                    </Label>
                    <Input id="periodsHK1" type="text" className="col-span-3" />
                  </div>
                  <div className="grid grid-cols-4 items-center gap-4">
                    <Label htmlFor="periodsHK2" className="text-left">
                      Số tiết/tuần HK2
                    </Label>
                    <Input id="periodsHK2" type="text" className="col-span-3" />
                  </div>
                </div>
                <DialogFooter>
                  <Button type="button">Thêm môn học</Button>
                </DialogFooter>
              </DialogContent>
            </Dialog>
          </div>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-50">Tên môn học</TableHead>
                <TableHead>Kiểu môn</TableHead>
                <TableHead className="text-center">Hệ số</TableHead>
                <TableHead className="text-center">Số tiết/tuần HK1</TableHead>
                <TableHead className="text-center">Số tiết/tuần HK2</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredSubjects.map((subject) => (
                <TableRow key={subject.id}>
                  <TableCell className="font-medium">
                    <Input
                      value={subject.name}
                      onChange={(e) => {
                        const updatedSubjects = [...subjects];
                        const index = updatedSubjects.findIndex(
                          (s) => s.id === subject.id,
                        );
                        updatedSubjects[index] = {
                          ...updatedSubjects[index],
                          name: e.target.value,
                        };
                        setSubjects(updatedSubjects);
                      }}
                      className="max-w-[200px]"
                    />
                  </TableCell>
                  <TableCell>
                    <Select value={subject.type}>
                      <SelectTrigger className="max-w-[150px]">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="Tính điểm">Tính điểm</SelectItem>
                        <SelectItem value="Nhận xét">Nhận xét</SelectItem>
                      </SelectContent>
                    </Select>
                  </TableCell>
                  <TableCell className="text-center">
                    <Input
                      value={subject.coefficient}
                      type="text"
                      className="mx-auto max-w-[80px] text-center"
                    />
                  </TableCell>
                  <TableCell className="text-center">
                    <Input
                      value={subject.periodsHK1}
                      type="text"
                      className="mx-auto max-w-[80px] text-center"
                    />
                  </TableCell>
                  <TableCell className="text-center">
                    <Input
                      value={subject.periodsHK2}
                      type="text"
                      className="mx-auto max-w-[80px] text-center"
                    />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <div className="mt-4 flex justify-end">
            <Button type="button" className="flex items-center gap-1">
              <Check className="h-4 w-4" />
              Lưu thay đổi
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
