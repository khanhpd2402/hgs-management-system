import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Select,
  SelectItem,
  SelectTrigger,
  SelectContent,
  SelectValue,
} from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DialogFooter,
} from "@/components/ui/dialog";
import DatePicker from "@/components/DatePicker";
import { Card, CardContent } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";

const schema = z.object({
  name: z.string().min(1, "Tên đợt không được để trống"),
  startDate: z.date({ required_error: "Vui lòng chọn ngày bắt đầu" }),
  endDate: z
    .date({ required_error: "Vui lòng chọn ngày kết thúc" })
    .refine(
      (date, ctx) => date >= ctx.parent.startDate,
      "Ngày kết thúc phải sau ngày bắt đầu",
    ),
});

export default function GradeEntryForm() {
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    reset,
  } = useForm({
    resolver: zodResolver(schema),
  });

  const [open, setOpen] = useState(false);
  const [scoreTypes, setScoreTypes] = useState({
    frequent: { enabled: false, count: "" },
    midterm: { enabled: false },
    final: { enabled: false },
  });

  const handleScoreTypeChange = (type, value) => {
    setScoreTypes((prev) => ({
      ...prev,
      [type]: { ...prev[type], enabled: value },
    }));
  };

  const handleFrequentCountChange = (value) => {
    setScoreTypes((prev) => ({
      ...prev,
      frequent: { ...prev.frequent, count: value },
    }));
  };

  const onSubmit = (data) => {
    console.log("Form data submitted:", {
      ...data,
      scoreTypes,
    });

    // Reset form and close dialog
    reset();
    setScoreTypes({
      frequent: { enabled: false, count: "" },
      midterm: { enabled: false },
      final: { enabled: false },
    });
    setOpen(false);
  };

  return (
    <div className="w-full max-w-2xl">
      <Dialog open={open} onOpenChange={setOpen}>
        <DialogTrigger asChild>
          <Button className="mb-4" size="sm">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="16"
              height="16"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
              strokeLinecap="round"
              strokeLinejoin="round"
              className="mr-2"
            >
              <path d="M12 5v14M5 12h14" />
            </svg>
            Thêm mới đợt nhập điểm
          </Button>
        </DialogTrigger>
        <DialogContent className="sm:max-w-[500px]">
          <DialogHeader>
            <DialogTitle className="text-xl font-semibold">
              Thêm mới đợt nhập điểm
            </DialogTitle>
            <Separator className="my-2" />
          </DialogHeader>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
            <div className="space-y-1">
              <Label htmlFor="name" className="font-medium">
                Tên đợt <span className="text-red-500">*</span>
              </Label>
              <Input
                id="name"
                {...register("name")}
                placeholder="Nhập tên đợt"
                className={errors.name ? "border-red-500" : ""}
              />
              {errors.name && (
                <p className="text-xs text-red-500">{errors.name.message}</p>
              )}
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-1">
                <Label htmlFor="startDate" className="font-medium">
                  Từ ngày <span className="text-red-500">*</span>
                </Label>
                <DatePicker
                  id="startDate"
                  onSelect={(date) => setValue("startDate", date)}
                  className={errors.startDate ? "border-red-500" : ""}
                />
                {errors.startDate && (
                  <p className="text-xs text-red-500">
                    {errors.startDate.message}
                  </p>
                )}
              </div>
              <div className="space-y-1">
                <Label htmlFor="endDate" className="font-medium">
                  Đến ngày <span className="text-red-500">*</span>
                </Label>
                <DatePicker
                  id="endDate"
                  onSelect={(date) => setValue("endDate", date)}
                  className={errors.endDate ? "border-red-500" : ""}
                />
                {errors.endDate && (
                  <p className="text-xs text-red-500">
                    {errors.endDate.message}
                  </p>
                )}
              </div>
            </div>

            <Card>
              <CardContent className="p-4">
                <Label className="mb-3 block font-medium">
                  Các cột điểm của đợt
                </Label>
                <div className="space-y-3">
                  <div className="flex items-center gap-3">
                    <Checkbox
                      id="frequent"
                      checked={scoreTypes.frequent.enabled}
                      onCheckedChange={(checked) =>
                        handleScoreTypeChange("frequent", checked)
                      }
                    />
                    <Label
                      htmlFor="frequent"
                      className="cursor-pointer font-normal"
                    >
                      Thường Xuyên
                    </Label>
                    <Select
                      value={scoreTypes.frequent.count}
                      onValueChange={handleFrequentCountChange}
                      disabled={!scoreTypes.frequent.enabled}
                      className="ml-auto"
                    >
                      <SelectTrigger className="w-[140px]">
                        <SelectValue placeholder="Số đầu điểm" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="2">2 đầu điểm</SelectItem>
                        <SelectItem value="3">3 đầu điểm</SelectItem>
                        <SelectItem value="4">4 đầu điểm</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>

                  <div className="flex items-center gap-3">
                    <Checkbox
                      id="midterm"
                      checked={scoreTypes.midterm.enabled}
                      onCheckedChange={(checked) =>
                        handleScoreTypeChange("midterm", checked)
                      }
                    />
                    <Label
                      htmlFor="midterm"
                      className="cursor-pointer font-normal"
                    >
                      Đánh giá giữa kỳ (ĐĐG GK)
                    </Label>
                  </div>

                  <div className="flex items-center gap-3">
                    <Checkbox
                      id="final"
                      checked={scoreTypes.final.enabled}
                      onCheckedChange={(checked) =>
                        handleScoreTypeChange("final", checked)
                      }
                    />
                    <Label
                      htmlFor="final"
                      className="cursor-pointer font-normal"
                    >
                      Đánh giá cuối kỳ (ĐĐG CK)
                    </Label>
                  </div>
                </div>
              </CardContent>
            </Card>

            <DialogFooter className="mt-6 flex justify-end gap-2">
              <Button
                type="button"
                variant="outline"
                onClick={() => setOpen(false)}
              >
                Hủy bỏ
              </Button>
              <Button type="submit">Lưu</Button>
            </DialogFooter>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
}
