import { useState } from "react";
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
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Badge } from "@/components/ui/badge";
import { X, Check } from "lucide-react";
import { toast } from "react-hot-toast";

export default function TAModal({ open, setOpen }) {
  const [selectedTeacher, setSelectedTeacher] = useState(null);
  const [selectedSubject, setSelectedSubject] = useState(null);
  const [assignments, setAssignments] = useState([]);
  const [selectedClasses, setSelectedClasses] = useState([]);
  const [subjectSelections, setSubjectSelections] = useState({});

  // Mock data
  const teacherGroups = [
    {
      name: "Khoa học tự nhiên",
      teachers: [
        { id: 1, name: "Phạm Thị Duyên" },
        { id: 2, name: "Nguyễn Thị Hiền" },
        { id: 3, name: "Vũ Thị Thu Hoài" },
        { id: 4, name: "Nguyễn Thị Huyền" },
        { id: 5, name: "Nguyễn Hữu Luận" },
        { id: 6, name: "Trần Thị Tuyết Nhung" },
        { id: 7, name: "Phạm Thị Thuý" },
        { id: 8, name: "Nguyễn Ngọc Trang" },
        { id: 9, name: "Nguyễn Thành Trung" },
      ],
    },
    {
      name: "Khoa học xã hội",
      teachers: [
        { id: 10, name: "Trần Văn A" },
        { id: 11, name: "Lê Thị B" },
      ],
    },
    {
      name: "Toàn trường",
      teachers: [
        { id: 12, name: "Nguyễn Văn C" },
        { id: 13, name: "Phạm Thị D" },
      ],
    },
  ];

  const subjects = [
    {
      id: 1,
      name: "GDCD",
      periods: {
        6: { hk1: 1, hk2: 1 },
        7: { hk1: 1, hk2: 1 },
        8: { hk1: 1, hk2: 1 },
        9: { hk1: 1, hk2: 1 },
      },
    },
    {
      id: 2,
      name: "Tiếng Anh",
      periods: {
        6: { hk1: 3, hk2: 3 },
        7: { hk1: 3, hk2: 3 },
        8: { hk1: 3, hk2: 3 },
        9: { hk1: 4, hk2: 4 },
      },
    },
    {
      id: 3,
      name: "Toán",
      periods: {
        6: { hk1: 4, hk2: 4 },
        7: { hk1: 4, hk2: 4 },
        8: { hk1: 4, hk2: 4 },
        9: { hk1: 4, hk2: 4 },
      },
    },
    {
      id: 4,
      name: "Ngữ Văn",
      periods: {
        6: { hk1: 3, hk2: 3 },
        7: { hk1: 3, hk2: 3 },
        8: { hk1: 3, hk2: 3 },
        9: { hk1: 3, hk2: 3 },
      },
    },
  ];

  const allClasses = ["6A", "6B", "7A", "7B", "7C", "8A", "8B", "9A", "9B"];

  const toggleClassSelection = (className) => {
    setSelectedClasses((prev) =>
      prev.includes(className)
        ? prev.filter((c) => c !== className)
        : [...prev, className],
    );
  };

  const handleAddAssignment = () => {
    if (!selectedTeacher || !selectedSubject || selectedClasses.length === 0) {
      toast.error("Vui lòng chọn đầy đủ thông tin");
      return;
    }

    // Check for duplicate assignment
    const isDuplicate = assignments.some(
      (a) =>
        a.teacher.id === selectedTeacher.id &&
        a.subject.id === selectedSubject.id,
    );

    // Check for duplicate classes
    const duplicateClasses = selectedClasses.filter((className) =>
      assignments.some((a) => a.classes.includes(className)),
    );

    if (isDuplicate) {
      toast.error("Phân công này đã tồn tại");
      return;
    }

    if (duplicateClasses.length > 0) {
      toast.error(`Lớp ${duplicateClasses.join(", ")} đã được phân công`);
      return;
    }

    setAssignments((prev) => [
      ...prev,
      {
        id: Date.now(),
        teacher: selectedTeacher,
        subject: selectedSubject,
        classes: [...selectedClasses],
      },
    ]);

    toast.success("Thêm phân công thành công");
    setSelectedSubject(null);
    setSelectedClasses([]);
  };

  const handleSubjectChange = (value) => {
    const subject = subjects.find((s) => s.id === parseInt(value));

    // Save current selection if exists
    if (selectedSubject && selectedClasses.length > 0) {
      setSubjectSelections((prev) => ({
        ...prev,
        [selectedSubject.id]: selectedClasses,
      }));
    }

    // Set new subject and restore its previous selections
    setSelectedSubject(subject);
    setSelectedClasses(subjectSelections[subject.id] || []);
  };

  const removeAssignment = (id) => {
    setAssignments((prev) => prev.filter((a) => a.id !== id));
    toast.success("Xóa phân công thành công");
  };

  const calculateTotalPeriods = () => {
    return assignments.reduce(
      (acc, curr) => {
        const semesterTotals = curr.classes.reduce(
          (classAcc, className) => {
            const grade = parseInt(className);
            const periods = curr.subject.periods[grade];
            return {
              hk1: classAcc.hk1 + periods.hk1,
              hk2: classAcc.hk2 + periods.hk2,
            };
          },
          { hk1: 0, hk2: 0 },
        );
        return {
          hk1: acc.hk1 + semesterTotals.hk1,
          hk2: acc.hk2 + semesterTotals.hk2,
        };
      },
      { hk1: 0, hk2: 0 },
    );
  };
  const resetForm = () => {
    setSelectedTeacher(null);
    setSelectedSubject(null);
    setSelectedClasses([]);
    setSubjectSelections({});
    setAssignments([]);
    setOpen(false);
  };

  const handleSave = () => {
    console.log("Saved assignments:", assignments);
    toast.success("Lưu phân công thành công");
    resetForm();
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogContent className="flex h-[80vh] w-[80%] !max-w-[1400px]">
        <div className="w-1/3 border-r pr-4">
          <DialogHeader>
            <DialogTitle>Danh sách giáo viên</DialogTitle>
          </DialogHeader>

          <ScrollArea className="h-[calc(80vh-100px)]">
            {teacherGroups.map((group) => (
              <div key={group.name} className="mb-6">
                <h3 className="mb-2 font-semibold">{group.name}</h3>
                <div className="space-y-1">
                  {group.teachers.map((teacher) => (
                    <div
                      key={teacher.id}
                      className={`cursor-pointer rounded p-2 ${
                        selectedTeacher?.id === teacher.id
                          ? "bg-blue-100 font-medium"
                          : "hover:bg-gray-100"
                      }`}
                      onClick={() => {
                        setSelectedTeacher(teacher);
                        setSelectedSubject(null);
                        setSelectedClasses([]);
                      }}
                    >
                      {teacher.name}
                    </div>
                  ))}
                </div>
              </div>
            ))}
          </ScrollArea>
        </div>

        <div className="w-2/3 pl-4">
          {selectedTeacher ? (
            <>
              <DialogHeader>
                <DialogTitle>
                  Phân công giảng dạy cho {selectedTeacher.name}
                </DialogTitle>
              </DialogHeader>

              <div className="mt-4 space-y-4">
                {/* Subject Selection */}
                <div className="space-y-2">
                  <Label>Môn học</Label>
                  <Select
                    value={selectedSubject?.id?.toString()}
                    onValueChange={handleSubjectChange}
                  >
                    <SelectTrigger className="w-full">
                      <SelectValue>
                        {selectedSubject ? (
                          <div className="flex items-center justify-between gap-4">
                            <span>{selectedSubject.name}</span>
                          </div>
                        ) : (
                          "Chọn môn học"
                        )}
                      </SelectValue>
                    </SelectTrigger>
                    <SelectContent>
                      {subjects.map((subject) => (
                        <SelectItem
                          key={subject.id}
                          value={subject.id.toString()}
                        >
                          {subject.name}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                {/* Class Selection with Periods Info */}
                {selectedSubject && (
                  <div className="space-y-4">
                    <div className="space-y-2">
                      <div className="flex items-center justify-between">
                        <Label>Lớp học</Label>
                        <span className="text-muted-foreground text-sm">
                          Số tiết theo khối lớp
                        </span>
                      </div>
                      <div className="grid grid-cols-3 gap-2">
                        {allClasses.map((className) => {
                          const grade = parseInt(className);
                          const periods = selectedSubject.periods[grade];
                          const isAssigned = assignments.some(
                            (a) =>
                              a.subject.id === selectedSubject.id &&
                              a.classes.includes(className),
                          );
                          return (
                            <div
                              key={className}
                              className={`flex cursor-pointer flex-col rounded border p-2 ${
                                selectedClasses.includes(className)
                                  ? "border-blue-300 bg-blue-50"
                                  : ""
                              } ${isAssigned ? "cursor-not-allowed opacity-50" : ""}`}
                              onClick={() =>
                                !isAssigned && toggleClassSelection(className)
                              }
                            >
                              <div className="flex items-center">
                                {selectedClasses.includes(className) ? (
                                  <Check className="mr-2 h-4 w-4 text-blue-500" />
                                ) : (
                                  <div className="mr-2 h-4 w-4 rounded border" />
                                )}
                                <span
                                  className={isAssigned ? "line-through" : ""}
                                >
                                  {className}
                                </span>
                              </div>
                              <div className="text-muted-foreground mt-1 text-xs">
                                HK1: {periods.hk1} - HK2: {periods.hk2} tiết
                              </div>
                            </div>
                          );
                        })}
                      </div>
                    </div>

                    {selectedClasses.length > 0 && (
                      <div className="bg-muted/50 flex items-center justify-between rounded-lg border p-3">
                        <div>
                          <p className="font-medium">
                            {selectedSubject.name}
                            <span className="text-muted-foreground ml-2 text-sm">
                              ({selectedClasses.join(", ")})
                            </span>
                          </p>
                          <p className="text-muted-foreground text-sm">
                            {
                              selectedClasses.reduce(
                                (acc, className) => {
                                  const grade = parseInt(className);
                                  const periods =
                                    selectedSubject.periods[grade];
                                  return {
                                    hk1: acc.hk1 + periods.hk1,
                                    hk2: acc.hk2 + periods.hk2,
                                  };
                                },
                                { hk1: 0, hk2: 0 },
                              ).hk1
                            }{" "}
                            tiết HK1 •{" "}
                            {
                              selectedClasses.reduce(
                                (acc, className) => {
                                  const grade = parseInt(className);
                                  const periods =
                                    selectedSubject.periods[grade];
                                  return {
                                    hk1: acc.hk1 + periods.hk1,
                                    hk2: acc.hk2 + periods.hk2,
                                  };
                                },
                                { hk1: 0, hk2: 0 },
                              ).hk2
                            }{" "}
                            tiết HK2
                          </p>
                        </div>
                      </div>
                    )}
                  </div>
                )}

                {/* Add Button */}
                {selectedSubject && selectedClasses.length > 0 && (
                  <Button onClick={handleAddAssignment}>Thêm phân công</Button>
                )}

                {/* Assignments Summary */}
                {assignments.length > 0 && (
                  <div className="space-y-4 rounded-lg border p-4">
                    <h3 className="font-medium">Danh sách phân công</h3>
                    <div className="space-y-2">
                      {assignments.map((assignment) => {
                        const totalPerSemester = assignment.classes.reduce(
                          (acc, className) => {
                            const grade = parseInt(className);
                            const periods = assignment.subject.periods[grade];
                            return {
                              hk1: acc.hk1 + periods.hk1,
                              hk2: acc.hk2 + periods.hk2,
                            };
                          },
                          { hk1: 0, hk2: 0 },
                        );

                        return (
                          <div
                            key={assignment.id}
                            className="flex items-center justify-between rounded-lg border p-3"
                          >
                            <div>
                              <p className="font-medium">
                                {assignment.subject.name}
                                <span className="text-muted-foreground ml-2 text-sm font-normal">
                                  ({assignment.classes.join(", ")})
                                </span>
                              </p>
                              <p className="text-muted-foreground text-sm">
                                {totalPerSemester.hk1} tiết HK1 •{" "}
                                {totalPerSemester.hk2} tiết HK2
                              </p>
                            </div>
                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={() => removeAssignment(assignment.id)}
                            >
                              <X className="h-4 w-4" />
                            </Button>
                          </div>
                        );
                      })}
                    </div>

                    <div className="flex items-center justify-between border-t pt-4">
                      <p className="font-medium">Tổng số tiết/tuần:</p>
                      <Badge variant="secondary" className="px-3 py-1 text-sm">
                        {calculateTotalPeriods().hk1} tiết HK1 •{" "}
                        {calculateTotalPeriods().hk2} tiết HK2
                      </Badge>
                    </div>
                  </div>
                )}
              </div>
            </>
          ) : (
            <div className="flex h-full items-center justify-center">
              <p className="text-muted-foreground">
                Vui lòng chọn giáo viên để phân công
              </p>
            </div>
          )}

          <div className="mt-4 flex justify-end gap-2">
            <Button variant="outline" onClick={resetForm}>
              Đóng
            </Button>
            <Button
              onClick={handleSave}
              disabled={assignments.length === 0 || !selectedTeacher}
            >
              Lưu
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}
