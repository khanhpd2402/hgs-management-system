import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import toast from "react-hot-toast";
import { PlusCircle } from "lucide-react";

// Add schema definition
const subjectSchema = z.object({
  subjectName: z.string().min(1, "Tên môn học không được để trống"),
  gradesData: z.array(
    z.object({
      khoi: z.number(),
      soTietHocKy1: z.number().nullable(),
      soTietHocKy2: z.number().nullable(),
      DDGTXHK1: z.number().nullable(),
      DDGTXHK2: z.number().nullable(),
      DDGGK: z.number().nullable(),
      DDCK: z.number().nullable(),
    }),
  ),
});

export const CreateSubjectModal = () => {
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    trigger,
    watch,
  } = useForm({
    resolver: zodResolver(subjectSchema),
    defaultValues: {
      subjectName: "",
      gradesData: [
        {
          khoi: 6,
          soTietHocKy1: 4,
          soTietHocKy2: 4,
          DDGTXHK1: 4,
          DDGTXHK2: 4,
          DDGGK: 1,
          DDCK: 1,
        },
        {
          khoi: 7,
          soTietHocKy1: 4,
          soTietHocKy2: 4,
          DDGTXHK1: 4,
          DDGTXHK2: 4,
          DDGGK: 1,
          DDCK: 1,
        },
        {
          khoi: 8,
          soTietHocKy1: 4,
          soTietHocKy2: 4,
          DDGTXHK1: 4,
          DDGTXHK2: 4,
          DDGGK: 1,
          DDCK: 1,
        },
        {
          khoi: 9,
          soTietHocKy1: 4,
          soTietHocKy2: 4,
          DDGTXHK1: 4,
          DDGTXHK2: 4,
          DDGGK: 1,
          DDCK: 1,
        },
      ],
    },
  });

  const gradesData = watch("gradesData");

  const [enabledGrades, setEnabledGrades] = useState({
    6: true,
    7: true,
    8: true,
    9: true,
  });

  const handleGradeToggle = (khoi) => {
    setEnabledGrades((prev) => ({
      ...prev,
      [khoi]: !prev[khoi],
    }));
    // Trigger validation when toggling grades
    trigger("gradesData");
  };

  const handleNumericInput = (e, index, field) => {
    const value = e.target.value.replace(/[^0-9]/g, "");
    setValue(
      `gradesData.${index}.${field}`,
      value === "" ? null : parseInt(value) || 0,
    );
    trigger("gradesData");
  };

  // Modify onSubmit to include validation
  const onSubmit = (data) => {
    const hasNullInEnabledGrades = data.gradesData.some((grade, index) => {
      if (!enabledGrades[grade.khoi]) return false;
      return Object.entries(grade).some(([key, value]) => {
        return key !== "khoi" && value === null;
      });
    });

    if (hasNullInEnabledGrades) {
      toast.error("Vui lòng điền đầy đủ thông tin cho các khối được chọn");
      return;
    }

    const filteredData = {
      ...data,
      gradesData: data.gradesData.filter((grade) => enabledGrades[grade.khoi]),
    };

    console.log(filteredData);
    // Submit data to server
  };

  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button variant="default">
          <PlusCircle className="h-4 w-4" />
          Tạo môn học
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-4xl">
        <DialogHeader>
          <DialogTitle>Tạo môn học mới</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit(onSubmit)} className="mt-4 space-y-4">
          <div className="space-y-2">
            <Label htmlFor="subject-name">Tên môn học</Label>
            <Input
              id="subject-name"
              className="h-10 w-full px-3 py-2 text-sm"
              {...register("subjectName", {
                required: "Tên môn học không được để trống",
              })}
            />
            {errors.subjectName && (
              <p className="text-sm text-red-500">
                {errors.subjectName.message}
              </p>
            )}
          </div>

          <div className="overflow-x-auto">
            <table className="w-full border-collapse border text-sm">
              <thead>
                <tr className="border-b">
                  <th className="border-r p-2 text-left">Số khối áp dụng</th>
                  {gradesData &&
                    gradesData.map((grade) => (
                      <th
                        key={grade.khoi}
                        className={`border-r p-2 text-center last:border-r-0 ${
                          !enabledGrades[grade.khoi] ? "bg-gray-100" : ""
                        }`}
                      >
                        <label className="flex cursor-pointer items-center justify-center gap-2 select-none">
                          Khối {grade.khoi}
                          <input
                            type="checkbox"
                            checked={enabledGrades[grade.khoi]}
                            onChange={() => handleGradeToggle(grade.khoi)}
                            className="h-4 w-4"
                          />
                        </label>
                      </th>
                    ))}
                </tr>
              </thead>
              <tbody>
                {/* Example for one row, apply to all rows */}
                <tr className="border-b">
                  <td className="border-r p-2">Số tiết HK1</td>
                  {gradesData.map((grade, index) => (
                    <td
                      key={`${grade.khoi}-hk1`}
                      className={`border-r p-2 text-center last:border-r-0 ${
                        !enabledGrades[grade.khoi] ? "bg-gray-100" : ""
                      }`}
                    >
                      <input
                        type="text"
                        className={`h-10 w-20 rounded-md px-3 py-2 text-center text-sm focus-visible:outline-none ${
                          errors.gradesData?.[index]?.soTietHocKy1 ||
                          (enabledGrades[grade.khoi] &&
                            !grade.soTietHocKy1 &&
                            grade.soTietHocKy1 !== 0)
                            ? "border-2 border-red-500"
                            : ""
                        }`}
                        value={grade.soTietHocKy1 ?? ""}
                        onChange={(e) =>
                          handleNumericInput(e, index, "soTietHocKy1")
                        }
                        disabled={!enabledGrades[grade.khoi]}
                        maxLength={2}
                        inputMode="numeric"
                        pattern="[0-9]*"
                      />
                    </td>
                  ))}
                </tr>
                <tr className="border-b">
                  <td className="border-r p-2">Số tiết HK2</td>
                  {gradesData.map((grade, index) => (
                    <td
                      key={`${grade.khoi}-hk2`}
                      className={`border-r p-2 text-center last:border-r-0 ${
                        !enabledGrades[grade.khoi] ? "bg-gray-100" : ""
                      }`}
                    >
                      <input
                        type="text"
                        className={`h-10 w-20 rounded-md px-3 py-2 text-center text-sm focus-visible:outline-none ${
                          errors.gradesData?.[index]?.soTietHocKy2 ||
                          (enabledGrades[grade.khoi] &&
                            !grade.soTietHocKy2 &&
                            grade.soTietHocKy2 !== 0)
                            ? "border-2 border-red-500"
                            : ""
                        }`}
                        value={grade.soTietHocKy2 ?? ""}
                        onChange={(e) =>
                          handleNumericInput(e, index, "soTietHocKy2")
                        }
                        disabled={!enabledGrades[grade.khoi]}
                        maxLength={2}
                        inputMode="numeric"
                        pattern="[0-9]*"
                      />
                    </td>
                  ))}
                </tr>
                <tr className="border-b">
                  <td className="border-r p-2">Số điểm ĐGTX HK1</td>
                  {gradesData.map((grade, index) => (
                    <td
                      key={`${grade.khoi}-dgtx1`}
                      className={`border-r p-2 text-center last:border-r-0 ${
                        !enabledGrades[grade.khoi] ? "bg-gray-100" : ""
                      }`}
                    >
                      <input
                        type="text"
                        className={`h-10 w-20 rounded-md px-3 py-2 text-center text-sm focus-visible:outline-none ${
                          errors.gradesData?.[index]?.DDGTXHK1 ||
                          (enabledGrades[grade.khoi] &&
                            !grade.DDGTXHK1 &&
                            grade.DDGTXHK1 !== 0)
                            ? "border-2 border-red-500"
                            : ""
                        }`}
                        value={grade.DDGTXHK1 ?? ""}
                        onChange={(e) =>
                          handleNumericInput(e, index, "DDGTXHK1")
                        }
                        disabled={!enabledGrades[grade.khoi]}
                        maxLength={2}
                        inputMode="numeric"
                        pattern="[0-9]*"
                      />
                    </td>
                  ))}
                </tr>
                <tr className="border-b">
                  <td className="border-r p-2">Số điểm ĐGTX HK2</td>
                  {gradesData.map((grade, index) => (
                    <td
                      key={`${grade.khoi}-dgtx2`}
                      className={`border-r p-2 text-center last:border-r-0 ${
                        !enabledGrades[grade.khoi] ? "bg-gray-100" : ""
                      }`}
                    >
                      <input
                        type="text"
                        className={`h-10 w-20 rounded-md px-3 py-2 text-center text-sm focus-visible:outline-none ${
                          errors.gradesData?.[index]?.DDGTXHK1 ||
                          (enabledGrades[grade.khoi] &&
                            !grade.DDGTXHK2 &&
                            grade.DDGTXHK2 !== 0)
                            ? "border-2 border-red-500"
                            : ""
                        }`}
                        value={grade.DDGTXHK2 ?? ""}
                        onChange={(e) =>
                          handleNumericInput(e, index, "DDGTXHK2")
                        }
                        disabled={!enabledGrades[grade.khoi]}
                        maxLength={2}
                        inputMode="numeric"
                        pattern="[0-9]*"
                      />
                    </td>
                  ))}
                </tr>
                <tr className="border-b">
                  <td className="border-r p-2">Số điểm ĐGGK</td>
                  {gradesData.map((grade, index) => (
                    <td
                      key={`${grade.khoi}-dggk`}
                      className={`border-r p-2 text-center last:border-r-0 ${
                        !enabledGrades[grade.khoi] ? "bg-gray-100" : ""
                      }`}
                    >
                      <input
                        type="text"
                        className={`h-10 w-20 rounded-md px-3 py-2 text-center text-sm focus-visible:outline-none ${
                          errors.gradesData?.[index]?.DDGGK ||
                          (enabledGrades[grade.khoi] &&
                            !grade.DDGGK &&
                            grade.DDGGK !== 0)
                            ? "border-2 border-red-500"
                            : ""
                        }`}
                        value={grade.DDGGK ?? ""}
                        onChange={(e) => handleNumericInput(e, index, "DDGGK")}
                        disabled={!enabledGrades[grade.khoi]}
                        maxLength={2}
                        inputMode="numeric"
                        pattern="[0-9]*"
                      />
                    </td>
                  ))}
                </tr>
                <tr className="border-b">
                  <td className="border-r p-2">Số điểm ĐGCK</td>
                  {gradesData.map((grade, index) => (
                    <td
                      key={`${grade.khoi}-dgck`}
                      className={`border-r p-2 text-center last:border-r-0 ${
                        !enabledGrades[grade.khoi] ? "bg-gray-100" : ""
                      }`}
                    >
                      <input
                        type="text"
                        className={`h-10 w-20 rounded-md px-3 py-2 text-center text-sm focus-visible:outline-none ${
                          errors.gradesData?.[index]?.DDCK ||
                          (enabledGrades[grade.khoi] &&
                            !grade.DDCK &&
                            grade.DDCK !== 0)
                            ? "border-2 border-red-500"
                            : ""
                        }`}
                        value={grade.DDCK ?? ""}
                        onChange={(e) => handleNumericInput(e, index, "DDCK")}
                        disabled={!enabledGrades[grade.khoi]}
                        maxLength={2}
                        inputMode="numeric"
                        pattern="[0-9]*"
                      />
                    </td>
                  ))}
                </tr>
              </tbody>
            </table>
            {Object.values(errors.gradesData || {}).some((error) => error) && (
              <p className="mt-2 text-sm text-red-500">
                Vui lòng điền đầy đủ thông tin cho các khối được chọn
              </p>
            )}
          </div>

          <div className="flex justify-end">
            <Button type="submit">Lưu môn học</Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
};
