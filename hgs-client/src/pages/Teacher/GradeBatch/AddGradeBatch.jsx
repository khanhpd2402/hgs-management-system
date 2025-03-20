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
  DialogTrigger,
  DialogFooter,
} from "@/components/ui/dialog";
import DatePicker from "@/components/DatePicker";
import { Card, CardContent } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { useSubjects } from "@/services/common/queries";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Controller } from "react-hook-form";
import { formatDate } from "@/helpers/formatDate";
import { useAddGradeBatch } from "@/services/principal/mutation";

const schema = z
  .object({
    batchName: z.string().min(1, "Tên đợt không được để trống"),
    startDate: z.date({ required_error: "Vui lòng chọn ngày bắt đầu" }),
    endDate: z.date({ required_error: "Vui lòng chọn ngày kết thúc" }),
    subjectIds: z.array(z.string()).min(1, "Vui lòng chọn ít nhất một môn học"),
  })
  .refine(
    (data) =>
      !data.startDate || !data.endDate || data.endDate >= data.startDate,
    {
      message: "Ngày kết thúc phải sau ngày bắt đầu",
      path: ["endDate"],
    },
  );
export default function GradeEntryForm() {
  const {
    register,
    handleSubmit,
    formState: { errors },
    control,
    setValue, // Add setValue here
    reset,
  } = useForm({
    resolver: zodResolver(schema),
  });

  const { data, isPending } = useSubjects();
  const addGradeBatchMutation = useAddGradeBatch();

  const [open, setOpen] = useState(false);
  const [assessmentTypes, setAssessmentTypes] = useState({
    frequent: { enabled: false, count: "" },
    midterm: { enabled: false },
    final: { enabled: false },
  });

  const [selectedSubjects, setSelectedSubjects] = useState([]);

  const handleScoreTypeChange = (type, value) => {
    setAssessmentTypes((prev) => ({
      ...prev,
      [type]: { ...prev[type], enabled: value },
    }));
  };

  const handleFrequentCountChange = (value) => {
    setAssessmentTypes((prev) => ({
      ...prev,
      frequent: { ...prev.frequent, count: value },
    }));
  };

  const handleSubjectToggle = (subjectId) => {
    const stringId = String(subjectId); // Ensure ID is a string
    setSelectedSubjects((prev) => {
      const newSelection = prev.includes(stringId)
        ? prev.filter((id) => id !== stringId)
        : [...prev, stringId];

      // Update the form value when selection changes
      setValue("subjectIds", newSelection, { shouldValidate: true });
      return newSelection;
    });
  };

  // In your useEffect, add a dependency array with setValue
  useEffect(() => {
    setValue("subjectIds", []);
  }, [setValue]);

  const onSubmit = (formData) => {
    // Transform assessmentTypes from object to array format
    const transformedAssessmentTypes = [];

    // Add frequent assessments based on count
    if (assessmentTypes.frequent.enabled && assessmentTypes.frequent.count) {
      const count = parseInt(assessmentTypes.frequent.count, 10);
      for (let i = 1; i <= count; i++) {
        transformedAssessmentTypes.push(`DDGTX${i}`);
      }
    }

    // Add midterm if enabled
    if (assessmentTypes.midterm.enabled) {
      transformedAssessmentTypes.push("DDGGK");
    }

    // Add final if enabled
    if (assessmentTypes.final.enabled) {
      transformedAssessmentTypes.push("DDGCK");
    }

    // Convert subjectIds from strings to numbers
    const numericSubjectIds = formData.subjectIds.map((id) => Number(id));

    // Create a complete batch object with all required data
    const newBatch = {
      gradeBatch: {
        batchName: formData.batchName,
        startDate: formatDate(formData.startDate),
        endDate: formatDate(formData.endDate),
      },
      assessmentTypes: transformedAssessmentTypes,
      subjectIds: numericSubjectIds,
    };
    console.log(newBatch);
    addGradeBatchMutation.mutate(newBatch);

    // Reset form and close dialog
    // reset();
    // setAssessmentTypes({
    //   frequent: { enabled: false, count: "" },
    //   midterm: { enabled: false },
    //   final: { enabled: false },
    // });
    // setSelectedSubjects([]);
    // setOpen(false);
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
                id="batchName"
                {...register("batchName")}
                placeholder="Nhập tên đợt"
                className={errors.name ? "border-red-500" : ""}
              />
              {errors.batchName && (
                <p className="text-xs text-red-500">
                  {errors.batchName.message}
                </p>
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
                      checked={assessmentTypes.frequent.enabled} // Changed from scoreTypes
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
                      value={assessmentTypes.frequent.count} // Changed from scoreTypes
                      onValueChange={handleFrequentCountChange}
                      disabled={!assessmentTypes.frequent.enabled} // Changed from scoreTypes
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
                      checked={assessmentTypes.midterm.enabled} // Changed from scoreTypes
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
                      checked={assessmentTypes.final.enabled} // Changed from scoreTypes
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

            <Card>
              <CardContent className="p-4">
                <Label className="mb-3 block font-medium">
                  Môn học áp dụng <span className="text-red-500">*</span>
                </Label>
                {isPending ? (
                  <div className="py-2 text-center">Đang tải...</div>
                ) : (
                  <ScrollArea className="h-[150px] pr-4">
                    <div className="space-y-2">
                      {data?.map((subject) => (
                        <div
                          key={subject.subjectId}
                          className="flex items-center gap-2"
                        >
                          <Checkbox
                            id={`subject-${subject.subjectId}`}
                            checked={selectedSubjects.includes(
                              String(subject.subjectId),
                            )}
                            onCheckedChange={() =>
                              handleSubjectToggle(subject.subjectId)
                            }
                          />
                          <Label
                            htmlFor={`subject-${subject.subjectId}`}
                            className="cursor-pointer font-normal"
                          >
                            {subject.subjectName}
                          </Label>
                        </div>
                      ))}
                    </div>
                  </ScrollArea>
                )}
                {errors.subjectIds && (
                  <p className="mt-1 text-xs text-red-500">
                    {errors.subjectIds.message}
                  </p>
                )}
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
