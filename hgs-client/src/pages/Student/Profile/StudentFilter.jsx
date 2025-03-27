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

const grades = [
  { value: "Khối 6", label: "Khối 6" },
  { value: "Khối 7", label: "Khối 7" },
  { value: "Khối 8", label: "Khối 8" },
  { value: "Khối 9", label: "Khối 9" },
];

const classMap = {
  "Khối 6": ["6A", "6B"],
  "Khối 7": ["7A", "7B", "7C"],
  "Khối 8": ["8A", "8B"],
  "Khối 9": ["9A", "9B"],
};

StudentFilter.propTypes = {
  setFilter: PropTypes.func.isRequired,
};

export default function StudentFilter({ setFilter }) {
  const [open, setOpen] = useState(false);
  const [search, setSearch] = useState("");
  const [grade, setGrade] = useState("");
  const [classname, setClassname] = useState("");

  const handleGradeChange = (value) => {
    setGrade(value);
    setClassname(""); // Reset lớp khi chọn khối mới
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    setFilter((options) => ({
      ...options,
      page: 1,
      searchValue: search.trim(),
      grade,
      classname,
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
              value={classname}
              disabled={!grade}
            >
              <SelectTrigger>
                <SelectValue placeholder="Chọn lớp" />
              </SelectTrigger>
              <SelectContent>
                {grade &&
                  classMap[grade].map((c) => (
                    <SelectItem key={c} value={c}>
                      {c}
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
