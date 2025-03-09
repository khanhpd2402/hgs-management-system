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

  return (
    <Card className="mx-auto mt-10 max-w-lg p-4 shadow-lg">
      <CardContent>
        <h2 className="mb-4 text-xl font-bold">Thêm Sinh Viên</h2>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label className="mb-1 block text-sm">Mã Sinh Viên</label>
            <Input
              placeholder="Mã Sinh Viên"
              type="number"
              {...register("studentId", { valueAsNumber: true })}
            />
            {errors.studentId && (
              <p className="text-sm text-red-500">{errors.studentId.message}</p>
            )}
          </div>

          <div>
            <label className="mb-1 block text-sm">Họ và Tên</label>
            <Input placeholder="Họ và Tên" {...register("fullName")} />
            {errors.fullName && (
              <p className="text-sm text-red-500">{errors.fullName.message}</p>
            )}
          </div>

          <div>
            <label className="mb-1 block text-sm">Ngày Sinh</label>
            <Input type="date" {...register("dob")} />
            {errors.dob && (
              <p className="text-sm text-red-500">{errors.dob.message}</p>
            )}
          </div>

          <div>
            <label className="mb-1 block text-sm">Giới Tính</label>
            <Select onValueChange={(value) => setValue("gender", value)}>
              <SelectTrigger>
                <SelectValue placeholder="Chọn Giới Tính" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Male">Nam</SelectItem>
                <SelectItem value="Female">Nữ</SelectItem>
                <SelectItem value="Other">Khác</SelectItem>
              </SelectContent>
            </Select>
            {errors.gender && (
              <p className="text-sm text-red-500">{errors.gender.message}</p>
            )}
          </div>

          <div>
            <label className="mb-1 block text-sm">Mã Lớp</label>
            <Input
              placeholder="Mã Lớp"
              type="number"
              {...register("classId", { valueAsNumber: true })}
            />
            {errors.classId && (
              <p className="text-sm text-red-500">{errors.classId.message}</p>
            )}
          </div>

          <div>
            <label className="mb-1 block text-sm">Ngày Nhập Học</label>
            <Input type="date" {...register("admissionDate")} />
            {errors.admissionDate && (
              <p className="text-sm text-red-500">
                {errors.admissionDate.message}
              </p>
            )}
          </div>

          <div>
            <label className="mb-1 block text-sm">Loại Hình Đào Tạo</label>
            <Input
              placeholder="Loại Hình Đào Tạo"
              {...register("enrollmentType")}
            />
            {errors.enrollmentType && (
              <p className="text-sm text-red-500">
                {errors.enrollmentType.message}
              </p>
            )}
          </div>

          <div>
            <label className="mb-1 block text-sm">Dân Tộc</label>
            <Input placeholder="Dân Tộc" {...register("ethnicity")} />
          </div>

          <div>
            <label className="mb-1 block text-sm">Địa Chỉ Thường Trú</label>
            <Input
              placeholder="Địa Chỉ Thường Trú"
              {...register("permanentAddress")}
            />
          </div>

          <div>
            <label className="mb-1 block text-sm">Nơi Sinh</label>
            <Input placeholder="Nơi Sinh" {...register("birthPlace")} />
          </div>

          <div>
            <label className="mb-1 block text-sm">Tôn Giáo</label>
            <Input placeholder="Tôn Giáo" {...register("religion")} />
          </div>

          <div>
            <label className="flex items-center space-x-2">
              <input type="checkbox" {...register("repeatingYear")} />
              <span>Lưu Ban</span>
            </label>
          </div>

          <div>
            <label className="mb-1 block text-sm">Số CMND/CCCD</label>
            <Input placeholder="Số CMND/CCCD" {...register("idcardNumber")} />
          </div>

          <div>
            <label className="mb-1 block text-sm">Trạng Thái</label>
            <Select onValueChange={(value) => setValue("status", value)}>
              <SelectTrigger>
                <SelectValue placeholder="Chọn Trạng Thái" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Active">Đang học</SelectItem>
                <SelectItem value="Inactive">Nghỉ học</SelectItem>
                <SelectItem value="Suspended">Đình chỉ</SelectItem>
                <SelectItem value="Graduated">Đã tốt nghiệp</SelectItem>
              </SelectContent>
            </Select>
            {errors.status && (
              <p className="text-sm text-red-500">{errors.status.message}</p>
            )}
          </div>

          <Button type="submit" className="w-full">
            Lưu
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
