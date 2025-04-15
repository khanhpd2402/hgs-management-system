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
import { Settings } from "lucide-react";
import { Checkbox } from "@/components/ui/checkbox";
import MyPagination from "@/components/MyPagination";
import { useTA } from "@/services/teacher/queries";
import TAHeader from "./TAHeader";
import PaginationControls from "@/components/PaginationControls";
import { Spinner } from "@/components/Spinner";
import TAModal from "./TAModal";

export default function TATable() {
  const [filter, setFilter] = useState({
    page: 1,
    pageSize: 5,
    sort: "",
    order: "asc",
    search: "",
  });
  const [openModal, setOpenModal] = useState(false);

  const mockData = [
    {
      id: 1,
      name: "Vương Thị Ngọc Anh",
      subject: "Tiếng Anh",
      assignedClass: "7A, 7B, 7C",
      standardHours: 18,
      assignedHours: 15,
    },
    {
      id: 2,
      name: "Lê Thị Hằng",
      subject: "Toán",
      assignedClass: "6A, 6B",
      standardHours: 18,
      assignedHours: 12,
    },
    {
      id: 3,
      name: "Lê Thị Hằng",
      subject: "Tin học",
      assignedClass: "6B, 6A",
      standardHours: 18,
      assignedHours: 12,
    },
    {
      id: 4,
      name: "Vũ Thị Thu Hoài",
      subject: "Giáo dục địa phương",
      assignedClass: "6A, 6B",
      standardHours: 18,
      assignedHours: 16,
    },
    {
      id: 5,
      name: "Vũ Thị Thu Hoài",
      subject: "Công nghệ",
      assignedClass: "7C, 7A, 7B",
      standardHours: 18,
      assignedHours: 16,
    },
    {
      id: 6,
      name: "Vũ Thị Thu Hoài",
      subject: "HD TNT, hướng nghiệp",
      assignedClass: "6B",
      standardHours: 18,
      assignedHours: 16,
    },
  ];

  // const { data, isPending, error, isError, isFetching } = useTA(filter);
  const { data, isPending, error, isError, isFetching } = {
    data: mockData,
    isPending: false,
    error: null,
    isError: false,
    isFetching: false,
  };

  const groupedData = data.reduce((acc, curr) => {
    if (!acc[curr.name]) {
      acc[curr.name] = {
        subjects: [],
        rowSpan: 0,
      };
    }
    acc[curr.name].subjects.push(curr);
    acc[curr.name].rowSpan++;
    return acc;
  }, {});
  const { page, pageSize } = filter;

  const startIndex = (page - 1) * pageSize + 1;
  const endIndex = Math.min(
    (page - 1) * pageSize + (data?.length || 0),
    startIndex + pageSize - 1,
  );

  // Tính toán hàng hiển thị
  if (isPending) {
    return (
      <Card className="relative mt-6 flex min-h-[550px] items-center justify-center p-4">
        <Spinner size="medium" />
      </Card>
    );
  }

  // if (isError) {
  //   return (
  //     <Card className="relative mt-6 flex items-center justify-center p-4">
  //       <div className="text-red-500">Lỗi khi tải dữ liệu</div>
  //     </Card>
  //   );
  // }

  return (
    <Card className="relative mt-6 p-4">
      <div className="mb-4 flex items-center justify-between">
        <TAHeader type="employees" setFilter={setFilter} />
        <Button
          onClick={() => setOpenModal(true)}
          className="flex items-center gap-2"
        >
          Thêm phân công giảng dạy
        </Button>
      </div>

      {/* Add Modal */}
      <TAModal open={openModal} onOpenChange={setOpenModal} />

      {/* Container chính không có overflow-x-auto */}
      <div className="relative">
        {isFetching && (
          <div className="absolute inset-0 z-10 flex items-center justify-center bg-white/70">
            <Spinner />
          </div>
        )}

        {/* Container cho bảng với overflow-x-auto */}
        <div className="max-h-[800px] overflow-auto">
          <div className="min-w-max">
            <Table
              className="w-full border border-gray-300"
              style={{ minWidth: "1500px" }}
            >
              <TableHeader className="bg-gray-100">
                <TableRow>
                  <TableHead className="w-50 border border-gray-300 text-left whitespace-nowrap">
                    Họ tên cán bộ
                  </TableHead>
                  <TableHead className="w-50 border border-gray-300 text-left whitespace-nowrap">
                    Môn học
                  </TableHead>
                  <TableHead className="w-40 border border-gray-300 text-center whitespace-nowrap">
                    Lớp phân công
                  </TableHead>
                  <TableHead className="w-20 border border-gray-300 text-center whitespace-nowrap">
                    Số tiết định mức trên tuần
                  </TableHead>
                  <TableHead className="w-20 border border-gray-300 text-center whitespace-nowrap">
                    Số tiết đã phân công
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {Object.entries(groupedData).map(([teacherName, data]) =>
                  data.subjects.map((subject, index) => (
                    <TableRow
                      key={subject.id}
                      className="divide-x divide-gray-300"
                    >
                      {index === 0 && (
                        <>
                          <TableCell
                            className="h-16 border border-gray-300 text-left whitespace-nowrap"
                            rowSpan={data.rowSpan}
                          >
                            {teacherName}
                          </TableCell>
                        </>
                      )}
                      <TableCell className="h-16 border border-gray-300 text-left whitespace-nowrap">
                        {subject.subject}
                      </TableCell>
                      <TableCell className="h-16 border border-gray-300 text-center whitespace-nowrap">
                        {subject.assignedClass}
                      </TableCell>
                      {index === 0 && (
                        <TableCell
                          className="h-16 border border-gray-300 text-center whitespace-nowrap"
                          rowSpan={data.rowSpan}
                        >
                          {subject.standardHours}
                        </TableCell>
                      )}
                      {index === 0 && (
                        <TableCell
                          className="h-16 border border-gray-300 text-center whitespace-nowrap"
                          rowSpan={data.rowSpan}
                        >
                          {subject.assignedHours}
                        </TableCell>
                      )}
                    </TableRow>
                  )),
                )}
              </TableBody>
            </Table>
          </div>
        </div>

        <div className="mt-4 flex items-center justify-between">
          <PaginationControls
            pageSize={pageSize}
            setFilter={setFilter}
            totalItems={data?.length || 0}
            startIndex={startIndex}
            endIndex={endIndex}
          />

          <MyPagination
            totalPages={6}
            currentPage={page}
            onPageChange={setFilter}
          />
        </div>
      </div>
    </Card>
  );
}
