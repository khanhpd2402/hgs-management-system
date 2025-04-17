import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem,
} from "@/components/ui/select";
import { Card } from "@/components/ui/card";
import { Pencil, Settings } from "lucide-react";

const mockData = [
  {
    id: 1,
    year: "2024-2025",
    current: true,
    startHK1: "01/08/2024",
    endHK1: "11/01/2025",
    startHK2: "12/01/2025",
    endHK2: "25/05/2025",
  },
  {
    id: 2,
    year: "2023-2024",
    current: false,
    startHK1: "06/09/2023",
    endHK1: "15/01/2024",
    startHK2: "16/01/2024",
    endHK2: "25/05/2024",
  },
  // ... add more mock data as needed ...
];

const PAGE_SIZE_OPTIONS = [10, 20, 50];

const AcademicYearManagement = () => {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(PAGE_SIZE_OPTIONS[0]);
  const totalItems = mockData.length;
  const totalPages = Math.ceil(totalItems / pageSize);

  const paginatedData = mockData.slice((page - 1) * pageSize, page * pageSize);

  return (
    <>
      <div className="py-6">
        <h1 className="text-2xl font-bold">Quản lý năm học</h1>
      </div>
      <div className="overflow-x-auto rounded-lg border">
        <table className="min-w-full text-sm">
          <thead>
            <tr className="sticky top-0 z-10 bg-slate-100">
              <th className="border px-2 py-2 text-center">STT</th>
              <th className="border px-2 py-2 text-center">Năm học</th>
              <th className="border px-2 py-2 text-center">
                Ngày bắt đầu học kì I
              </th>
              <th className="border px-2 py-2 text-center">
                Ngày kết thúc học kì I
              </th>
              <th className="border px-2 py-2 text-center">
                Ngày bắt đầu học kì II
              </th>
              <th className="border px-2 py-2 text-center">
                Ngày kết thúc học kì II
              </th>
              <th className="border px-2 py-2 text-center">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {paginatedData.map((row, idx) => (
              <tr key={row.id}>
                <td className="border px-2 py-2 text-center">
                  {(page - 1) * pageSize + idx + 1}
                </td>
                <td className="border px-2 py-2 text-center">
                  {row.year}

                  {row.current && (
                    <span className="ml-1 font-bold text-green-400"> </span>
                  )}
                </td>
                <td className="border px-2 py-2 text-center">{row.startHK1}</td>
                <td className="border px-2 py-2 text-center">{row.endHK1}</td>
                <td className="border px-2 py-2 text-center">{row.startHK2}</td>
                <td className="border px-2 py-2 text-center">{row.endHK2}</td>
                <td className="border px-2 py-2 text-center">
                  <Button variant="outline" size="icon">
                    <Settings className="h-4 w-4" />
                  </Button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  );
};

export default AcademicYearManagement;
