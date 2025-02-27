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

const fields = [
  { value: "name", label: "Họ tên cán bộ" },
  { value: "code", label: "Mã cán bộ" },
  { value: "phone", label: "Số ĐTDD" },
  { value: "email", label: "Địa chỉ Email" },
  { value: "dob", label: "Ngày sinh" },
  { value: "id", label: "ID" },
];

EmployeeFilter.propTypes = {
  setFilter: PropTypes.func.isRequired,
};

export default function EmployeeFilter({ setFilter }) {
  const [open, setOpen] = useState(false);
  const [field, setField] = useState("");
  const [order, setOrder] = useState("");
  const [search, setSearch] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    setFilter((options) => ({
      ...options,
      page: 1,
      sort: field,
      order,
      search: search.trim(),
    }));
    setOpen(false);
  };

  const handleReset = () => {
    setField("");
    setOrder("");
    setSearch("");
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
            {/* Chọn trường lọc */}
            <Select onValueChange={setField} value={field}>
              <SelectTrigger>
                <SelectValue placeholder="Chọn trường lọc" />
              </SelectTrigger>
              <SelectContent>
                {fields.map((f) => (
                  <SelectItem key={f.value} value={f.value}>
                    {f.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            {/* Chọn thứ tự sắp xếp */}
            <Select onValueChange={setOrder} value={order} disabled={!field}>
              <SelectTrigger>
                <SelectValue placeholder="Chọn thứ tự" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="asc">Tăng dần</SelectItem>
                <SelectItem value="desc">Giảm dần</SelectItem>
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
