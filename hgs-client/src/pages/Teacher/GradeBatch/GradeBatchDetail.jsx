import { useEffect, useState } from "react";
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
  DialogFooter,
} from "@/components/ui/dialog";
import DatePicker from "@/components/DatePicker";
import { Card, CardContent } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { Controller } from "react-hook-form";

const schema = z
  .object({
    name: z.string().min(1, "Tên đợt không được để trống"),
    startDate: z.date({ required_error: "Vui lòng chọn ngày bắt đầu" }),
    endDate: z.date({ required_error: "Vui lòng chọn ngày kết thúc" }),
  })
  .refine(
    (data) =>
      !data.startDate || !data.endDate || data.endDate >= data.startDate,
    {
      message: "Ngày kết thúc phải sau ngày bắt đầu",
      path: ["endDate"],
    },
  );

export default function GradeBatchDetail({
  batch,
  open,
  onOpenChange,
  onSave,
}) {
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    control,
    reset,
  } = useForm({
    resolver: zodResolver(schema),
  });

  const [scoreTypes, setScoreTypes] = useState({
    frequent: { enabled: false, count: "" },
    midterm: { enabled: false },
    final: { enabled: false },
  });

  // Load batch data when component receives a batch or when dialog opens
  useEffect(() => {
    if (batch && open) {
      reset({
        name: batch.name,
        startDate: batch.startDate ? new Date(batch.startDate) : undefined,
        endDate: batch.endDate ? new Date(batch.endDate) : undefined,
      });
      setScoreTypes(batch.scoreTypes);
    }
  }, [batch, open, reset]);

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
    const updatedBatch = {
      ...batch,
      ...data,
      // Format dates for API
      startDate: data.startDate ? data.startDate.toISOString() : null,
      endDate: data.endDate ? data.endDate.toISOString() : null,
      scoreTypes,
    };

    onSave(updatedBatch);
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle className="text-xl font-semibold">
            Chi tiết đợt nhập điểm
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
              <Controller
                name="startDate"
                control={control}
                render={({ field }) => (
                  <DatePicker
                    id="startDate"
                    value={field.value}
                    onSelect={field.onChange}
                    className={errors.startDate ? "border-red-500" : ""}
                    disabled={false}
                  />
                )}
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
              <Controller
                name="endDate"
                control={control}
                render={({ field }) => (
                  <DatePicker
                    id="endDate"
                    value={field.value}
                    onSelect={field.onChange}
                    className={errors.endDate ? "border-red-500" : ""}
                    disabled={false}
                  />
                )}
              />
              {errors.endDate && (
                <p className="text-xs text-red-500">{errors.endDate.message}</p>
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
                  <Label htmlFor="final" className="cursor-pointer font-normal">
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
              onClick={() => onOpenChange(false)}
            >
              Hủy bỏ
            </Button>
            <Button type="submit">Lưu</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
