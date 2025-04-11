import { useState } from "react";
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

import { Check, Search } from "lucide-react";
import { CreateSubjectModal } from "./CreateSubjectModal";

export default function SubjectManagement() {
  const [searchTerm, setSearchTerm] = useState("");

  // // Filter subjects based on search term
  // const filteredSubjects = subjects.filter((subject) =>
  //   subject.name.toLowerCase().includes(searchTerm.toLowerCase()),
  // );

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
            <CreateSubjectModal />
          </div>
        </CardHeader>
        <CardContent>
          {/* <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-50">Tên môn học</TableHead>
                <TableHead>Kiểu môn</TableHead>
                <TableHead className="text-center">Hệ số</TableHead>
                <TableHead className="text-center">Số tiết/tuần HK1</TableHead>
                <TableHead className="text-center">Số tiết/tuần HK2</TableHead>
                <TableHead className="text-center">ĐGTX-HK1</TableHead>
                <TableHead className="text-center">ĐGTX-HK2</TableHead>
                <TableHead className="text-center">ĐGgk</TableHead>
                <TableHead className="text-center">ĐGck</TableHead>
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
                  <TableCell className="text-center">
                    <Input
                      value={subject.dgtxhk1}
                      type="text"
                      className="mx-auto max-w-[80px] text-center"
                    />
                  </TableCell>
                  <TableCell className="text-center">
                    <Input
                      value={subject.dgtxhk2}
                      type="text"
                      className="mx-auto max-w-[80px] text-center"
                    />
                  </TableCell>
                  <TableCell className="text-center">
                    <Input
                      value={subject.dggk}
                      type="text"
                      className="mx-auto max-w-[80px] text-center"
                    />
                  </TableCell>
                  <TableCell className="text-center">
                    <Input
                      value={subject.dgck}
                      type="text"
                      className="mx-auto max-w-[80px] text-center"
                    />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table> */}
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
