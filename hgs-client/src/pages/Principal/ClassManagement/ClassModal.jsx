import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";

export default function ClassModal({
  open,
  onOpenChange,
  onSubmit,
  editingClass,
  gradelevelQuery,
}) {
  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            {editingClass ? "Chỉnh Sửa Lớp Học" : "Thêm Lớp Học Mới"}
          </DialogTitle>
        </DialogHeader>
        <form onSubmit={onSubmit} className="space-y-4">
          <div className="grid w-full gap-2">
            <Label htmlFor="gradeId">Khối</Label>
            <select
              id="gradeId"
              name="gradeId"
              className="border-input bg-background ring-offset-background placeholder:text-muted-foreground focus-visible:ring-ring flex h-10 w-full rounded-md border px-3 py-2 text-sm file:border-0 file:bg-transparent file:text-sm file:font-medium focus-visible:ring-2 focus-visible:ring-offset-2 focus-visible:outline-none disabled:cursor-not-allowed disabled:opacity-50"
              defaultValue={editingClass?.gradeId}
              required
            >
              <option value="">Chọn khối</option>
              {gradelevelQuery?.data?.map((grade) => (
                <option key={grade.id} value={grade.id}>
                  {grade.gradeName}
                </option>
              ))}
            </select>
          </div>
          <div className="grid w-full gap-2">
            <Label htmlFor="className">Tên Lớp</Label>
            <Input
              id="className"
              name="className"
              defaultValue={editingClass?.className}
              required
            />
          </div>
          <div className="grid w-full gap-2">
            <Label htmlFor="teacherName">Giáo Viên Chủ Nhiệm</Label>
            <Input
              id="teacherName"
              name="teacherName"
              defaultValue={editingClass?.teacherName}
              required
            />
          </div>
          <Button type="submit" className="w-full">
            {editingClass ? "Cập Nhật" : "Thêm"} Lớp Học
          </Button>
        </form>
      </DialogContent>
    </Dialog>
  );
}
