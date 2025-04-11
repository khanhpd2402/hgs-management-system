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
import { Pencil, Search, Trash2 } from "lucide-react";
import { CreateSubjectModal } from "./CreateSubjectModal";
import { useSubjects } from "@/services/common/queries";
import { UpdateSubjectModal } from "./UpdateSubjectModal";

export default function SubjectManagement() {
  const [searchTerm, setSearchTerm] = useState("");
  const [open, setOpen] = useState(false);
  const [selectedSubjectId, setSelectedSubjectId] = useState(null);
  const subjectQuery = useSubjects();

  const filteredSubjects = subjectQuery?.data?.filter((subject) =>
    subject.subjectName.toLowerCase().includes(searchTerm.toLowerCase()),
  );
  // console.log(filteredSubjects);

  console.log(selectedSubjectId);
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
            <CreateSubjectModal />
          </div>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>STT</TableHead>
                <TableHead>Tên môn học</TableHead>
                <TableHead className="text-right">Thao tác</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredSubjects?.map((subject, index) => (
                <TableRow key={subject.subjectId}>
                  <TableCell className="w-16 font-medium">
                    {index + 1}
                  </TableCell>
                  <TableCell className="min-w-72">
                    {subject.subjectName}
                  </TableCell>
                  <TableCell className="text-right">
                    <div className="flex justify-end gap-2">
                      <Button
                        variant="outline"
                        size="icon"
                        className="cursor-pointer"
                        onClick={() => {
                          setOpen(true);
                          setSelectedSubjectId(subject.subjectId);
                        }}
                      >
                        <Pencil className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="destructive"
                        size="icon"
                        className="cursor-pointer"
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
      {selectedSubjectId && (
        <UpdateSubjectModal
          subjectId={selectedSubjectId}
          open={open}
          setOpen={setOpen}
          setSelectedSubjectId={setSelectedSubjectId}
        />
      )}
    </div>
  );
}
