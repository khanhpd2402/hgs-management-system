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

export default function EmployeeFilter({ setFilter }) {
  const [open, setOpen] = useState(false);
  const { control, handleSubmit, watch, reset, setValue } = useForm({
    defaultValues: { field: "", order: "", search: "" },
  });

  const selectedField = watch("field"); // Theo dõi giá trị của field

  const onSubmit = (data) => {
    setFilter((options) => ({
      ...options,
      page: 1,
      sort: data.field,
      order: data.order,
      search: data.search.trim(),
    }));

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
            {/* Chọn trường lọc */}
            <Controller
              name="field"
              control={control}
              defaultValue="" // Đảm bảo không bị undefined
              render={({ field }) => (
                <Select
                  onValueChange={(value) => {
                    field.onChange(value);
                    setValue("field", value); // Cập nhật state để tránh selectedField bị rỗng
                  }}
                  value={field.value || ""}
                >
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

            {/* Chọn thứ tự sắp xếp */}
            <Controller
              name="order"
              control={control}
              defaultValue="" // Đảm bảo không bị undefined
              render={({ field }) => (
                <Select
                  onValueChange={field.onChange}
                  value={field.value || ""}
                  disabled={!selectedField} // Chỉ chọn nếu đã chọn trường lọc
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

            {/* Ô tìm kiếm */}
            <Controller
              name="search"
              control={control}
              defaultValue=""
              render={({ field }) => (
                <Input placeholder="Tìm kiếm" {...field} />
              )}
            />

            <div className="flex justify-end space-x-2">
              <Button
                type="button"
                variant="outline"
                onClick={() => {
                  reset();
                  setValue("field", ""); // Reset giá trị field về rỗng
                }}
              >
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
