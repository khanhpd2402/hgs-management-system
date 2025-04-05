import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import axios from "axios";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

// Schema với studentId và classId đảm bảo là kiểu number
const studentSchema = z.object({
  studentId: z.coerce.number().int().nonnegative(),
  fullName: z.string().min(1, "Full name is required"),
  dob: z.string().min(1, "Date of birth is required"),
  gender: z.string().min(1, "Gender is required"),
  classId: z.coerce.number().int().nonnegative(),
  admissionDate: z.string().min(1, "Admission date is required"),
  enrollmentType: z.string().min(1, "Enrollment type is required"),
  ethnicity: z.string().optional(),
  permanentAddress: z.string().optional(),
  birthPlace: z.string().optional(),
  religion: z.string().optional(),
  repeatingYear: z.boolean().default(false),
  idcardNumber: z.string().optional(),
  status: z.string().min(1, "Status is required"),
});

export default function AddStudentForm() {
  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(studentSchema),
    defaultValues: {
      studentId: 0,
      fullName: "",
      gender: "",
      classId: 0,
      enrollmentType: "",
      repeatingYear: false,
      status: "",
    },
  });

  const onSubmit = async (data) => {
    // Đảm bảo studentId và classId là number
    const formattedData = {
      ...data,
      studentId: Number(data.studentId),
      classId: Number(data.classId),
      // Đảm bảo các trường không bắt buộc có giá trị mặc định
      ethnicity: data.ethnicity || "",
      permanentAddress: data.permanentAddress || "",
      birthPlace: data.birthPlace || "",
      religion: data.religion || "",
      idcardNumber: data.idcardNumber || "",
    };

    console.log("Dữ liệu gửi đi:", formattedData);

    try {
      const response = await axios.post(
        "https://localhost:8386/api/student",
        formattedData,
        {
          headers: {
            "Content-Type": "application/json",
          },
        },
      );
      console.log("Thêm sinh viên thành công", response.data);
      alert("Thêm sinh viên thành công!");
    } catch (error) {
      console.error("Lỗi khi gửi biểu mẫu:", error);
      alert(
        "Lỗi khi thêm sinh viên: " +
          (error.response?.data?.message || error.message),
      );
    }
  };

  // ... existing code ...
  return (
    <Card className="mx-auto mt-10 max-w-4xl p-6 shadow-lg">
      <CardContent>
        <h2 className="mb-6 text-center text-2xl font-bold">
          Thêm Sinh Viên Mới
        </h2>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Thông tin cơ bản */}
          <div className="rounded-md bg-slate-50 p-4">
            <h3 className="mb-4 text-lg font-semibold text-slate-800">
              Thông tin cơ bản
            </h3>
            <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
              <div>
                <label className="mb-1 block text-sm font-medium">
                  Mã Sinh Viên <span className="text-red-500">*</span>
                </label>
                <Input
                  placeholder="Nhập mã sinh viên"
                  type="number"
                  className="focus:border-primary"
                  {...register("studentId", { valueAsNumber: true })}
                />
                {errors.studentId && (
                  <p className="text-sm text-red-500">
                    {errors.studentId.message}
                  </p>
                )}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">
                  Họ và Tên <span className="text-red-500">*</span>
                </label>
                <Input
                  placeholder="Nhập họ và tên"
                  className="focus:border-primary"
                  {...register("fullName")}
                />
                {errors.fullName && (
                  <p className="text-sm text-red-500">
                    {errors.fullName.message}
                  </p>
                )}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">
                  Ngày Sinh <span className="text-red-500">*</span>
                </label>
                <Input
                  type="date"
                  className="focus:border-primary"
                  {...register("dob")}
                />
                {errors.dob && (
                  <p className="text-sm text-red-500">{errors.dob.message}</p>
                )}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">
                  Giới Tính <span className="text-red-500">*</span>
                </label>
                <Select onValueChange={(value) => setValue("gender", value)}>
                  <SelectTrigger className="focus:border-primary">
                    <SelectValue placeholder="Chọn giới tính" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Male">Nam</SelectItem>
                    <SelectItem value="Female">Nữ</SelectItem>
                    <SelectItem value="Other">Khác</SelectItem>
                  </SelectContent>
                </Select>
                {errors.gender && (
                  <p className="text-sm text-red-500">
                    {errors.gender.message}
                  </p>
                )}
              </div>
            </div>
          </div>

          {/* Thông tin học tập */}
          <div className="rounded-md bg-slate-50 p-4">
            <h3 className="mb-4 text-lg font-semibold text-slate-800">
              Thông tin học tập
            </h3>
            <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
              <div>
                <label className="mb-1 block text-sm font-medium">
                  Mã Lớp <span className="text-red-500">*</span>
                </label>
                <Input
                  placeholder="Nhập mã lớp"
                  type="number"
                  className="focus:border-primary"
                  {...register("classId", { valueAsNumber: true })}
                />
                {errors.classId && (
                  <p className="text-sm text-red-500">
                    {errors.classId.message}
                  </p>
                )}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">
                  Ngày Nhập Học <span className="text-red-500">*</span>
                </label>
                <Input
                  type="date"
                  className="focus:border-primary"
                  {...register("admissionDate")}
                />
                {errors.admissionDate && (
                  <p className="text-sm text-red-500">
                    {errors.admissionDate.message}
                  </p>
                )}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">
                  Loại Hình Đào Tạo <span className="text-red-500">*</span>
                </label>
                <Input
                  placeholder="Nhập loại hình đào tạo"
                  className="focus:border-primary"
                  {...register("enrollmentType")}
                />
                {errors.enrollmentType && (
                  <p className="text-sm text-red-500">
                    {errors.enrollmentType.message}
                  </p>
                )}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">
                  Trạng Thái <span className="text-red-500">*</span>
                </label>
                <Select onValueChange={(value) => setValue("status", value)}>
                  <SelectTrigger className="focus:border-primary">
                    <SelectValue placeholder="Chọn trạng thái" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Active">Đang học</SelectItem>
                    <SelectItem value="Inactive">Nghỉ học</SelectItem>
                    <SelectItem value="Suspended">Đình chỉ</SelectItem>
                    <SelectItem value="Graduated">Đã tốt nghiệp</SelectItem>
                  </SelectContent>
                </Select>
                {errors.status && (
                  <p className="text-sm text-red-500">
                    {errors.status.message}
                  </p>
                )}
              </div>

              <div className="flex items-center">
                <label className="flex items-center space-x-2 rounded-md border border-gray-300 p-2">
                  <input
                    type="checkbox"
                    className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                    {...register("repeatingYear")}
                  />
                  <span className="text-sm font-medium">Lưu Ban</span>
                </label>
              </div>
            </div>
          </div>

          {/* Thông tin cá nhân */}
          <div className="rounded-md bg-slate-50 p-4">
            <h3 className="mb-4 text-lg font-semibold text-slate-800">
              Thông tin cá nhân
            </h3>
            <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
              <div>
                <label className="mb-1 block text-sm font-medium">
                  Dân Tộc
                </label>
                <Input
                  placeholder="Nhập dân tộc"
                  className="focus:border-primary"
                  {...register("ethnicity")}
                />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">
                  Tôn Giáo
                </label>
                <Input
                  placeholder="Nhập tôn giáo"
                  className="focus:border-primary"
                  {...register("religion")}
                />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">
                  Nơi Sinh
                </label>
                <Input
                  placeholder="Nhập nơi sinh"
                  className="focus:border-primary"
                  {...register("birthPlace")}
                />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">
                  Số CMND/CCCD
                </label>
                <Input
                  placeholder="Nhập số CMND/CCCD"
                  className="focus:border-primary"
                  {...register("idcardNumber")}
                />
              </div>

              <div className="md:col-span-2">
                <label className="mb-1 block text-sm font-medium">
                  Địa Chỉ Thường Trú
                </label>
                <Input
                  placeholder="Nhập địa chỉ thường trú"
                  className="focus:border-primary"
                  {...register("permanentAddress")}
                />
              </div>
            </div>
          </div>

          <div className="flex justify-end space-x-4">
            <Button type="button" variant="outline" className="w-32">
              Hủy
            </Button>
            <Button
              type="submit"
              className="w-32 bg-blue-600 hover:bg-blue-700"
            >
              Lưu
            </Button>
          </div>
        </form>
      </CardContent>
    </Card>
  );
  // ... existing code ...
}
