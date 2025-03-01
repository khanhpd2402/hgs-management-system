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

const grade = [
  { value: "6", label: "Khối 6" },
  { value: "7", label: "Khối 7" },
  { value: "8", label: "Khối 8" },
  { value: "9", label: "Khối 9" },
];

const teacher = [
  { value: "Physics", label: "Vật Lý" },
  { value: "Math", label: "Toán" },
];

HTAFilter.propTypes = {
  setFilter: PropTypes.func.isRequired,
};

export default function HTAFilter({ setFilter }) {
  const [open, setOpen] = useState(false);

  const [search, setSearch] = useState("");
  const [department, setDepartment] = useState("");
  const [contract, setContract] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    setFilter((options) => ({
      ...options,
      page: 1,

      search: search.trim(),
      department,
      contract,
    }));
    setOpen(false);
  };

  const handleReset = () => {
    setSearch("");
    setDepartment("");
    setContract("");
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
            {/* Lọc theo tổ bộ môn */}
            <Select onValueChange={setDepartment} value={department}>
              <SelectTrigger>
                <SelectValue placeholder="Chọn khối" />
              </SelectTrigger>
              <SelectContent>
                {grade.map((d) => (
                  <SelectItem key={d.value} value={d.value}>
                    {d.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            {/* Lọc theo hợp đồng lao động */}
            <Select onValueChange={setContract} value={contract}>
              <SelectTrigger>
                <SelectValue placeholder="Chọn giáo viên" />
              </SelectTrigger>
              <SelectContent>
                {teacher.map((c) => (
                  <SelectItem key={c.value} value={c.value}>
                    {c.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

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
