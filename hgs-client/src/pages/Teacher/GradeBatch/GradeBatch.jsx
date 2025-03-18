import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import DatePicker from "@/components/DatePicker";

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
  } = useForm({
    resolver: zodResolver(schema),
  });

  const [scores, setScores] = useState(Array(8).fill(false));
  const [scoreValues, setScoreValues] = useState(Array(8).fill(""));

  const handleCheckboxChange = (index) => {
    const newScores = [...scores];
    newScores[index] = !newScores[index];
    setScores(newScores);
  };

  const handleInputChange = (index, value) => {
    const newScoreValues = [...scoreValues];
    newScoreValues[index] = value;
    setScoreValues(newScoreValues);
  };

  const onSubmit = (data) => {
    console.log("Form data submitted:", data, scoreValues);
  };

  return (
    <div className="w-full max-w-2xl rounded-xl bg-white p-6 shadow-lg">
      <Dialog>
        <DialogTrigger asChild>
          <Button>Thêm mới</Button>
        </DialogTrigger>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Thêm mới đợt nhập điểm</DialogTitle>
          </DialogHeader>
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div>
              <Label className="font-medium">Tên đợt *</Label>
              <Input {...register("name")} placeholder="Nhập tên đợt" />
              {errors.name && (
                <p className="text-sm text-red-500">{errors.name.message}</p>
              )}
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label className="font-medium">Từ ngày *</Label>
                <DatePicker onSelect={(date) => setValue("startDate", date)} />
                {errors.startDate && (
                  <p className="text-sm text-red-500">
                    {errors.startDate.message}
                  </p>
                )}
              </div>
              <div>
                <Label className="font-medium">Đến ngày *</Label>
                <DatePicker onSelect={(date) => setValue("endDate", date)} />
                {errors.endDate && (
                  <p className="text-sm text-red-500">
                    {errors.endDate.message}
                  </p>
                )}
              </div>
            </div>

            <div>
              <Label className="font-medium">Các cột điểm của đợt</Label>
              <div className="mt-2 space-y-2">
                <div className="flex items-center gap-2">
                  <span>Thường Xuyên</span>
                  {[...Array(8)].map((_, i) => (
                    <div key={i} className="flex items-center gap-2">
                      <Checkbox
                        checked={scores[i]}
                        onCheckedChange={() => handleCheckboxChange(i)}
                      />
                      {scores[i] && (
                        <Input
                          type="number"
                          value={scoreValues[i]}
                          onChange={(e) => handleInputChange(i, e.target.value)}
                          className="w-16"
                        />
                      )}
                    </div>
                  ))}
                </div>
                <div className="flex items-center gap-2">
                  <Checkbox /> <span>ĐĐG GK</span>
                </div>
                <div className="flex items-center gap-2">
                  <Checkbox /> <span>ĐĐG CK</span>
                </div>
              </div>
            </div>
            <div className="flex justify-end gap-2">
              <Button type="button" variant="outline">
                Hủy bỏ
              </Button>
              <Button type="submit">Lưu</Button>
            </div>
          </form>
        </DialogContent>
      </Dialog>
    </div>
  );
}
