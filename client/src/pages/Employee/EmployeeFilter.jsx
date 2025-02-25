import { useState } from "react";
import { useForm, Controller } from "react-hook-form";
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

export default function EmployeeFilter({ setSort, setSearch, setOrder }) {
  const [open, setOpen] = useState(false);
  const { control, handleSubmit, watch, reset } = useForm({
    defaultValues: { field: "", order: "", search: "" },
  });

  const selectedField = watch("field");

  const onSubmit = (data) => {
    setSort(data.field);
    setSearch(data.search.trim());
    setOrder(data.order);
    setOpen(false);
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
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <Controller
              name="field"
              control={control}
              render={({ field }) => (
                <Select onValueChange={field.onChange} value={field.value}>
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
              )}
            />

            <Controller
              name="order"
              control={control}
              render={({ field }) => (
                <Select
                  onValueChange={field.onChange}
                  value={field.value}
                  disabled={!selectedField}
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Chọn thứ tự" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="asc">Tăng dần</SelectItem>
                    <SelectItem value="desc">Giảm dần</SelectItem>
                  </SelectContent>
                </Select>
              )}
            />

            <Controller
              name="search"
              control={control}
              render={({ field }) => (
                <Input placeholder="Tìm kiếm" {...field} />
              )}
            />

            <div className="flex justify-end space-x-2">
              <Button type="button" variant="outline" onClick={() => reset()}>
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
