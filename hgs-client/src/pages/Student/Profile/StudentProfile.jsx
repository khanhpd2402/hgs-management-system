import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Spinner } from "@/components/Spinner";
import DatePicker from "@/components/DatePicker";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { useParams } from "react-router";
import { useStudent } from "@/services/student/queries";
import { useUpdateStudent } from "@/services/student/mutation";
import { useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect } from "react";

export default function StudentProfile() {
  const { id } = useParams();
  const studentQuery = useStudent(id);
  const { mutate, isPending: isUpdating } = useUpdateStudent();

  console.log(studentQuery.data);

  const studentSchema = z.object({
    // Basic student info
    fullName: z
      .string()
      .min(1, "Họ và tên không được để trống")
      .regex(
        /^[\p{L}\s]+$/u,
        "Họ và tên không được chứa số hoặc ký tự đặc biệt",
      ),
    dob: z.date().nullable(),
    gender: z.enum(["Nam", "Nữ", "Khác"]).optional(),
    grade: z.string().min(1, "Khối lớp không được để trống"),
    className: z.string().min(1, "Lớp học không được để trống"),
    status: z.enum(["Đang học", "Bảo lưu", "Đã tốt nghiệp", "Đã nghỉ học"]),
    registrationNumber: z.string().optional(),
    admissionType: z.string().optional(),

    // Parent information - Father
    fatherName: z
      .string()
      .min(1, "Họ tên cha không được để trống")
      .regex(
        /^[\p{L}\s]+$/u,
        "Họ tên cha không được chứa số hoặc ký tự đặc biệt",
      ),
    fatherOccupation: z.string().optional(),
    fatherPhone: z
      .string()
      .regex(/^0\d{9}$/, "Số điện thoại phải có 10 số và bắt đầu bằng số 0")
      .optional(),

    // Parent information - Mother
    motherName: z
      .string()
      .min(1, "Họ tên mẹ không được để trống")
      .regex(
        /^[\p{L}\s]+$/u,
        "Họ tên mẹ không được chứa số hoặc ký tự đặc biệt",
      ),
    motherOccupation: z.string().optional(),
    motherPhone: z
      .string()
      .regex(/^0\d{9}$/, "Số điện thoại phải có 10 số và bắt đầu bằng số 0")
      .optional(),

    // Address information
    province: z
      .string()
      .min(1, "Tỉnh/TP không được để trống")
      .regex(/^[\p{L}\s]+$/u, "Tỉnh/TP không được chứa số hoặc ký tự đặc biệt"),
    district: z
      .string()
      .min(1, "Quận/huyện không được để trống")
      .regex(
        /^[\p{L}\s]+$/u,
        "Quận/huyện không được chứa số hoặc ký tự đặc biệt",
      ),
    ward: z
      .string()
      .min(1, "Xã/phường không được để trống")
      .regex(
        /^[\p{L}\s]+$/u,
        "Xã/phường không được chứa số hoặc ký tự đặc biệt",
      ),
    address: z.string().optional(),
  });

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    control,
    reset,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(studentSchema),
    defaultValues: {
      status: "Đang học",
      gender: "Nữ",
    },
  });

  // Initialize form when studentQuery.data is loaded
  useEffect(() => {
    if (studentQuery.data) {
      // Set form values from studentQuery.data
      Object.keys(studentQuery.data).forEach((key) => {
        // Handle date fields separately
        if (["dob"].includes(key) && studentQuery.data[key]) {
          try {
            setValue(key, new Date(studentQuery.data[key]));
          } catch (e) {
            console.error(`Error parsing date for ${key}:`, e);
            setValue(key, null);
          }
        } else {
          setValue(key, studentQuery.data[key]);
        }
      });
    }
  }, [studentQuery.data, setValue]);

  if (studentQuery.isPending) return <Spinner />;

  // Submit handler
  const onSubmit = (formData) => {
    const processedData = {
      ...formData,
      dob: formData.dob ? formData.dob.toISOString().split("T")[0] : null,
    };

    mutate({ id, data: processedData });
  };

  // Form field component
  const FormField = ({ name, label, type = "text", options }) => {
    const error = errors[name];

    // Select field
    if (type === "select" && Array.isArray(options)) {
      return (
        <div className="space-y-2">
          <Label htmlFor={name}>{label}</Label>
          <Controller
            name={name}
            control={control}
            defaultValue={watch(name) || ""}
            render={({ field }) => (
              <Select onValueChange={field.onChange} value={field.value}>
                <SelectTrigger>
                  <SelectValue placeholder={`Chọn ${label.toLowerCase()}`} />
                </SelectTrigger>
                <SelectContent>
                  {options.map((option) => (
                    <SelectItem key={option} value={option}>
                      {option}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
          {error && <p className="text-sm text-red-500">{error.message}</p>}
        </div>
      );
    }

    // Date field
    if (type === "date") {
      return (
        <div className="space-y-2">
          <Label htmlFor={name}>{label}</Label>
          <Controller
            name={name}
            control={control}
            render={({ field }) => (
              <DatePicker value={field.value} onSelect={field.onChange} />
            )}
          />
          {error && <p className="text-sm text-red-500">{error.message}</p>}
        </div>
      );
    }

    // Default text input
    return (
      <div className="space-y-2">
        <Label htmlFor={name}>{label}</Label>
        <Input id={name} {...register(name)} />
        {error && <p className="text-sm text-red-500">{error.message}</p>}
      </div>
    );
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="container mx-auto space-y-6 py-6"
    >
      {/* Header with title and save button */}
      <div className="flex items-center justify-between">
        <h1 className="text-3xl font-bold">Hồ sơ học sinh</h1>
        <Button type="submit" disabled={isUpdating}>
          {isUpdating ? "Đang lưu..." : "Cập nhật thông tin"}
        </Button>
      </div>

      {/* Basic Student Info Card */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin học sinh</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <div className="flex items-center gap-4 md:col-span-2">
            <Avatar className="h-20 w-20">
              <AvatarImage
                src="/placeholder-avatar.jpg"
                alt={studentQuery.data?.fullName}
              />
              <AvatarFallback className="text-2xl">
                {studentQuery.data?.fullName?.split(" ").pop()?.charAt(0) ||
                  "A"}
              </AvatarFallback>
            </Avatar>
            <div className="flex-1">
              <FormField
                name="fullName"
                label="Họ và tên"
                defaultValue={studentQuery.data?.fullName}
              />
            </div>
          </div>
          <FormField name="dob" label="Ngày sinh" type="date" />
          <FormField
            name="gender"
            label="Giới tính"
            type="select"
            options={["Nam", "Nữ", "Khác"]}
            defaultValue={studentQuery.data?.gender}
          />
          <FormField
            name="grade"
            label="Khối"
            defaultValue={studentQuery.data?.grade}
          />
          <FormField
            name="className"
            label="Lớp"
            defaultValue={studentQuery.data?.className}
          />
          <FormField
            name="status"
            label="Trạng thái"
            type="select"
            options={["Đang học", "Bảo lưu", "Đã tốt nghiệp", "Đã nghỉ học"]}
            defaultValue={studentQuery.data?.status}
          />
        </CardContent>
      </Card>

      {/* Registration Information */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin hồ sơ</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormField
            name="registrationNumber"
            label="Số đăng bộ"
            defaultValue={studentQuery.data?.registrationNumber}
          />
          <FormField
            name="admissionType"
            label="Hình thức trúng tuyển"
            defaultValue={studentQuery.data?.admissionType}
          />
        </CardContent>
      </Card>

      {/* Father Information */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin cha</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormField
            name="fatherName"
            label="Họ và tên"
            defaultValue={studentQuery.data?.fatherName}
          />
          <FormField
            name="fatherOccupation"
            label="Nghề nghiệp"
            defaultValue={studentQuery.data?.fatherOccupation}
          />
          <FormField
            name="fatherPhone"
            label="Số điện thoại"
            defaultValue={studentQuery.data?.fatherPhone}
          />
        </CardContent>
      </Card>

      {/* Mother Information */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin mẹ</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormField
            name="motherName"
            label="Họ và tên"
            defaultValue={studentQuery.data?.motherName}
          />
          <FormField
            name="motherOccupation"
            label="Nghề nghiệp"
            defaultValue={studentQuery.data?.motherOccupation}
          />
          <FormField
            name="motherPhone"
            label="Số điện thoại"
            defaultValue={studentQuery.data?.motherPhone}
          />
        </CardContent>
      </Card>

      {/* Address Information */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin địa chỉ và hộ khẩu</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormField
            name="province"
            label="Tỉnh/Thành phố"
            defaultValue={studentQuery.data?.province}
          />
          <FormField
            name="district"
            label="Quận/Huyện"
            defaultValue={studentQuery.data?.district}
          />
          <FormField
            name="ward"
            label="Xã/Phường"
            defaultValue={studentQuery.data?.ward}
          />
          <FormField
            name="address"
            label="Thôn/Xóm/Số nhà"
            defaultValue={studentQuery.data?.address}
          />
        </CardContent>
      </Card>

      {/* Submit button at bottom for convenience */}
      <div className="flex justify-end">
        <Button type="submit" disabled={isUpdating} size="lg">
          {isUpdating ? "Đang lưu..." : "Cập nhật thông tin"}
        </Button>
      </div>
    </form>
  );
}
