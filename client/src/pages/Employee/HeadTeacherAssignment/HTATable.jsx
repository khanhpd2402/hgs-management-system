import { useState } from "react";
import { Card } from "@/components/ui/card";
import {
  Table,
  TableHeader,
  TableRow,
  TableHead,
  TableBody,
  TableCell,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { Pencil, Trash } from "lucide-react";
import { useHTA } from "@/services/employee/queries";
import HTAHeader from "./HTAHeader";
import { Spinner } from "@/components/Spinner";

export default function HTATable() {
  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 5,
    grade: "",
    teacher: "",
  });

  const { data, isPending, error, isError, isFetching } = useHTA(filter);
  console.log(data);

  if (isPending) {
    return (
      <Card className="relative mt-6 flex min-h-[550px] items-center justify-center p-4">
        <Spinner size="medium" />
      </Card>
    );
  }

  if (isError) {
    return (
      <Card className="relative mt-6 flex min-h-[550px] items-center justify-center p-4">
        <div className="text-red-500">Lỗi khi tải dữ liệu</div>
      </Card>
    );
  }

  return (
    <Card className="relative mt-6 min-h-[550px] p-4">
      <HTAHeader type="employees" setFilter={setFilter} />

      <div className="relative min-h-[400px]">
        {isFetching && (
          <div className="absolute inset-0 z-10 flex items-center justify-center bg-white/70">
            <Spinner />
          </div>
        )}

        <div className="mt-4 overflow-x-auto">
          <Table className="w-full border border-gray-300">
            <TableHeader className="bg-gray-100 text-white">
              <TableRow>
                <TableHead className="border border-gray-300 text-center whitespace-nowrap">
                  STT
                </TableHead>
                <TableHead className="border border-gray-300 text-center whitespace-nowrap">
                  Lớp
                </TableHead>
                <TableHead className="border border-gray-300 text-center whitespace-nowrap">
                  Sĩ số
                </TableHead>
                <TableHead className="border border-gray-300 text-center whitespace-nowrap">
                  Giáo viên chủ nhiệm
                </TableHead>
                <TableHead className="border border-gray-300 text-center whitespace-nowrap">
                  Thao tác
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {data.length > 0 ? (
                data?.map((classItem) => (
                  <TableRow key={classItem.id}>
                    <TableCell className="border border-gray-300 text-center">
                      {classItem.id}
                    </TableCell>
                    <TableCell className="border border-gray-300 text-center">
                      {classItem.class}
                    </TableCell>
                    <TableCell className="border border-gray-300 text-center">
                      {classItem.student_number}
                    </TableCell>
                    <TableCell className="border border-gray-300 text-center">
                      {classItem.head_teacher}
                    </TableCell>
                    <TableCell className="border border-gray-300 text-center">
                      <div className="flex justify-center space-x-2">
                        <Button
                          variant="outline"
                          size="icon"
                          className="h-8 w-8 border-blue-500 text-blue-500"
                        >
                          <Pencil className="h-4 w-4" />
                        </Button>
                        <Button
                          variant="outline"
                          size="icon"
                          className="h-8 w-8 border-red-500 text-red-500"
                        >
                          <Trash className="h-4 w-4" />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell
                    colSpan={5}
                    className="h-16 text-center text-gray-500"
                  >
                    Không có dữ liệu
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      </div>
    </Card>
  );
}
