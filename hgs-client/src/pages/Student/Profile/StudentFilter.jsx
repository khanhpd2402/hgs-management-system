import { useState } from "react";
import PropTypes from "prop-types";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { DialogDescription } from "@radix-ui/react-dialog";
import { useClasses } from "@/services/common/queries";

const grades = [
  { value: "6", label: "Khối 6" },
  { value: "7", label: "Khối 7" },
  { value: "8", label: "Khối 8" },
  { value: "9", label: "Khối 9" },
];

StudentFilter.propTypes = {
  setFilter: PropTypes.func.isRequired,
};

export default function StudentFilter({ setFilter }) {
  const [open, setOpen] = useState(false);
  const [search, setSearch] = useState("");
  const [grade, setGrade] = useState("");
  const [className, setClassname] = useState("");

  const { data } = useClasses();
  const currentClasses = data?.filter((c) => c.grade == grade);

  const handleGradeChange = (value) => {
    setGrade(value);
    setClassname(""); // Reset lớp khi chọn khối mới
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setFilter((options) => ({
      ...options,
      page: 1,
      search: search.trim(),
      grade,
      className,
    }));
    setOpen(false);
  };

  const handleReset = () => {
    setSearch("");
    setGrade("");
    setClassname("");
  };

  return (
    <>
      <Button onClick={() => setOpen(true)}>Lọc</Button>
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Bộ lọc</DialogTitle>
            <DialogDescription></DialogDescription>
          </DialogHeader>
          <form onSubmit={handleSubmit} className="space-y-4">
            {/* Lọc theo khối */}
            <Select onValueChange={handleGradeChange} value={grade}>
              <SelectTrigger>
                <SelectValue placeholder="Chọn khối" />
              </SelectTrigger>
              <SelectContent>
                {grades.map((d) => (
                  <SelectItem key={d.value} value={d.value}>
                    {d.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            {/* Lọc theo lớp */}
            <Select
              onValueChange={setClassname}
              value={className}
              disabled={!grade}
            >
              <SelectTrigger>
                <SelectValue placeholder="Chọn lớp" />
              </SelectTrigger>
              <SelectContent>
                {currentClasses &&
                  currentClasses.length > 0 &&
                  currentClasses.map((c) => (
                    <SelectItem key={c.classId} value={c.className}>
                      {c.className}
                    </SelectItem>
                  ))}
              </SelectContent>
            </Select>

            {/* Ô tìm kiếm */}
            <Input
              placeholder="Tìm kiếm"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />

            <div className="flex justify-end space-x-2">
              <Button type="button" variant="outline" onClick={handleReset}>
                Reset
              </Button>
              <Button type="submit">Xác nhận</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </>
  );
}
