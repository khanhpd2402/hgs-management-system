import React, { useState, useEffect } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";

export default function UpdateTAModal({ open, onOpenChange }) {
  const [selectedSubject, setSelectedSubject] = useState("english");
  const [subjectAssignments, setSubjectAssignments] = useState({
    english: ["7A", "7B", "7C"],
    math: [],
    physics: [],
    chemistry: [],
    biology: [],
  });

  const handleSubjectChange = (value) => {
    setSelectedSubject(value);
  };

  const handleClassToggle = (className) => {
    setSubjectAssignments((prev) => ({
      ...prev,
      [selectedSubject]: prev[selectedSubject].includes(className)
        ? prev[selectedSubject].filter((c) => c !== className)
        : [...prev[selectedSubject], className],
    }));
  };

  const getAssignmentString = () => {
    const assignments = [];
    const subjectNames = {
      english: "Tiếng Anh",
      math: "Toán",
      physics: "Vật Lý",
      chemistry: "Hóa Học",
      biology: "Sinh Học",
    };

    Object.entries(subjectAssignments).forEach(([subject, classes]) => {
      if (classes.length > 0) {
        assignments.push(
          `${subjectNames[subject]}(${classes.sort().join(", ")})`,
        );
      }
    });

    return assignments.join(" - ");
  };

  const handleSave = () => {
    const data = {
      subject: selectedSubject,
      assignedClasses: selectedClasses,
      periodsHK1: 3,
      periodsHK2: 3,
    };
    console.log("Saving data:", data);
    // Add your API call here
    onOpenChange(false);
  };

  const handleClose = () => {
    onOpenChange(false);
  };

  const calculateTotalPeriods = () => {
    let totalHK1 = 0;
    let totalHK2 = 0;

    Object.values(subjectAssignments).forEach((classes) => {
      totalHK1 += classes.length * 3; // 3 periods per class in HK1
      totalHK2 += classes.length * 3; // 3 periods per class in HK2
    });

    return `HK1: ${totalHK1}, HK2: ${totalHK2}`;
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="!max-w-4xl">
        <DialogHeader>
          <DialogTitle>
            Cập nhật phân công giảng dạy - Vương Thị Ngọc Anh
          </DialogTitle>
        </DialogHeader>

        <div className="grid gap-4">
          <div className="grid grid-cols-3 gap-4">
            <div>
              <label className="text-sm font-medium">Giáo viên</label>
              <Input value="Vương Thị Ngọc Anh" disabled className="mt-1" />
            </div>
            <div>
              <label className="text-sm font-medium">Môn học</label>
              <Select
                value={selectedSubject}
                onValueChange={handleSubjectChange}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Chọn môn học" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="english">Tiếng Anh</SelectItem>
                  <SelectItem value="math">Toán</SelectItem>
                  <SelectItem value="physics">Vật Lý</SelectItem>
                  <SelectItem value="chemistry">Hóa Học</SelectItem>
                  <SelectItem value="biology">Sinh Học</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          <div>
            <div>
              <label className="text-sm font-medium">Danh sách phân công</label>
              <div className="flex gap-2">
                <Input
                  value={getAssignmentString()}
                  disabled
                  className="mt-1 disabled:opacity-100"
                />
              </div>
              <div>
                <label className="text-sm font-medium">Tổng số tiết</label>

                <div>
                  <Input
                    value={calculateTotalPeriods()}
                    disabled
                    className="mt-1 w-64 disabled:opacity-100"
                  />
                </div>
              </div>
            </div>
          </div>

          <div className="rounded-lg border">
            <table className="w-full">
              <thead className="bg-slate-100">
                <tr>
                  <th className="w-16 p-2"></th>
                  <th className="p-2 text-left">Tên lớp</th>
                  <th className="p-2 text-center">Số tiết/tuần HK1</th>
                  <th className="p-2 text-center">Số tiết/tuần HK2</th>
                </tr>
              </thead>
              <tbody>
                {["6A", "6B", "7A", "7B", "7C", "8A", "8B", "9A", "9B"].map(
                  (className) => (
                    <tr key={className} className="border-t">
                      <td className="p-2 text-center">
                        <Checkbox
                          checked={subjectAssignments[selectedSubject].includes(
                            className,
                          )}
                          onCheckedChange={() => handleClassToggle(className)}
                        />
                      </td>
                      <td className="p-2">{className}</td>
                      <td className="p-2 text-center">3</td>
                      <td className="p-2 text-center">3</td>
                    </tr>
                  ),
                )}
              </tbody>
            </table>
          </div>
        </div>

        <div className="mt-4 flex justify-end gap-2">
          <Button variant="outline" onClick={handleClose}>
            Đóng
          </Button>
          <Button onClick={handleSave}>Lưu</Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
