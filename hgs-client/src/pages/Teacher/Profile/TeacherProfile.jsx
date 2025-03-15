import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";

import { data, useParams } from "react-router";
import { useTeacher } from "@/services/teacher/queries";
import { Spinner } from "@/components/Spinner";
import { useUpdateTeacher } from "@/services/teacher/mutation";
import { useForm, Controller } from "react-hook-form";
import { Button } from "@/components/ui/button";
import { useEffect } from "react";
import DatePicker from "@/components/DatePicker";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

export default function TeacherProfile() {
  const { id } = useParams();
  const teacherQuery = useTeacher(id);
  const { mutate, isPending: isUpdating } = useUpdateTeacher();

  const teacherSchema = z.object({
    // Basic info
    fullName: z
      .string()
      .min(1, "Họ và tên không được để trống")
      .regex(
        /^[\p{L}\s]+$/u,
        "Họ và tên không được chứa số hoặc ký tự đặc biệt",
      ),
    position: z.string().optional(),
    department: z.string().optional(),

    // Personal information
    gender: z.enum(["Nam", "Nữ", "Khác"]).optional(),
    dob: z.date().nullable(),
    idcardNumber: z
      .string()
      .length(12, "Số CMND/CCCD phải có đúng 13 chữ số")
      .regex(/^\d{12}$/, "Số CMND/CCCD chỉ được chứa chữ số"),
    hometown: z
      .string()
      .min(1, "Quê quán không được để trống")
      .regex(
        /^[\p{L}\s]+$/u,
        "Quê quán không được chứa số hoặc ký tự đặc biệt",
      ),
    ethnicity: z
      .string()
      .min(1, "Dân tộc không được để trống")
      .regex(/^[\p{L}\s]+$/u, "Dân tộc không được chứa số hoặc ký tự đặc biệt"),
    religion: z
      .string()
      .min(1, "Tôn giáo không được để trống")
      .regex(
        /^[\p{L}\s]+$/u,
        "Tôn giáo không được chứa số hoặc ký tự đặc biệt",
      ),
    maritalStatus: z
      .enum(["Độc thân", "Đã kết hôn", "Ly hôn", "Góa"])
      .optional(),
    permanentAddress: z
      .string()
      .min(1, "Địa chỉ không được để trống")
      .regex(/^[\p{L}\d\s,]+$/u, "Địa chỉ không được chứa ký tự đặc biệt"),

    // Employment information
    additionalDuties: z.string().optional(),
    isHeadOfDepartment: z.boolean().optional().default(false),
    employmentType: z
      .enum(["Biên chế", "Hợp đồng dài hạn", "Hợp đồng ngắn hạn", "Cơ hữu"])
      .optional(),
    employmentStatus: z
      .enum(["Đang làm việc", "Nghỉ phép", "Đã nghỉ việc"])
      .optional(),
    recruitmentAgency: z.string().optional(),
    insuranceNumber: z.string().optional(),

    // Employment dates
    hiringDate: z.date().nullable(),
    schoolJoinDate: z.date().nullable(),
    permanentEmploymentDate: z.date().nullable(),
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
    resolver: zodResolver(teacherSchema),
  });

  // Initialize form when teacherQuery.data is loaded
  useEffect(() => {
    if (teacherQuery.data) {
      // Set form values from teacherQuery.data
      Object.keys(teacherQuery.data).forEach((key) => {
        // Handle date fields separately
        if (
          [
            "dob",
            "hiringDate",
            "schoolJoinDate",
            "permanentEmploymentDate",
          ].includes(key) &&
          teacherQuery.data[key]
        ) {
          try {
            setValue(key, new Date(teacherQuery.data[key]));
          } catch (e) {
            console.error(`Error parsing date for ${key}:`, e);
            setValue(key, null);
          }
        } else {
          setValue(key, teacherQuery.data[key]);
        }
      });
    }
  }, [teacherQuery.data, setValue]);

  if (teacherQuery.isPending) return <Spinner />;

  // Submit handler
  const onSubmit = (formData) => {
    const processedData = {
      ...formData,
      dob: formData.dob ? formData.dob.toISOString().split("T")[0] : null,
      hiringDate: formData.hiringDate
        ? formData.hiringDate.toISOString().split("T")[0]
        : null,
      schoolJoinDate: formData.schoolJoinDate
        ? formData.schoolJoinDate.toISOString().split("T")[0]
        : null,
      permanentEmploymentDate: formData.permanentEmploymentDate
        ? formData.permanentEmploymentDate.toISOString().split("T")[0]
        : null,
    };

    mutate({ id, data: processedData });
  };

  // Form field component
  const FormField = ({ name, label, type = "text", options }) => {
    const error = errors[name]; // Lấy lỗi của field

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

    // Boolean field (Switch)
    if (type === "boolean") {
      return (
        <div className="flex items-center justify-between">
          <Label htmlFor={name}>{label}</Label>
          <Controller
            name={name}
            control={control}
            defaultValue={watch(name) || false}
            render={({ field }) => (
              <Switch
                id={name}
                checked={field.value}
                onCheckedChange={field.onChange}
              />
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
        <h1 className="text-3xl font-bold">Hồ sơ giáo viên</h1>
        <Button type="submit" disabled={isUpdating}>
          {isUpdating ? "Đang lưu..." : "Cập nhật thông tin"}
        </Button>
      </div>

      {/* Basic Info Card */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin cơ bản</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <div className="flex items-center gap-4 md:col-span-2">
            <Avatar className="h-20 w-20">
              <AvatarImage
                src="/placeholder-avatar.jpg"
                alt={teacherQuery.data?.fullName}
              />
              <AvatarFallback className="text-2xl">
                {teacherQuery.data?.fullName?.split(" ").pop()?.charAt(0) ||
                  "A"}
              </AvatarFallback>
            </Avatar>
            <div className="flex-1">
              <FormField
                name="fullName"
                label="Họ và tên"
                defaultValue={teacherQuery.data?.fullName}
              />
            </div>
          </div>
          <FormField
            name="position"
            label="Vị trí"
            defaultValue={teacherQuery.data?.position}
          />
          <FormField
            name="department"
            label="Bộ môn"
            defaultValue={teacherQuery.data?.department}
          />
        </CardContent>
      </Card>

      {/* Personal Information */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin cá nhân</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormField
            name="gender"
            label="Giới tính"
            type="select"
            options={["Nam", "Nữ", "Khác"]}
            defaultValue={teacherQuery.data?.gender}
          />
          <FormField name="dob" label="Ngày sinh" type="date" />
          <FormField
            name="idcardNumber"
            label="Số CMND/CCCD"
            defaultValue={teacherQuery.data?.idcardNumber}
          />
          <FormField
            name="hometown"
            label="Quê quán"
            defaultValue={teacherQuery.data?.hometown}
          />
          <FormField
            name="ethnicity"
            label="Dân tộc"
            defaultValue={teacherQuery.data?.ethnicity}
          />
          <FormField
            name="religion"
            label="Tôn giáo"
            defaultValue={teacherQuery.data?.religion}
          />
          <FormField
            name="maritalStatus"
            label="Tình trạng hôn nhân"
            type="select"
            options={["Độc thân", "Đã kết hôn", "Ly hôn", "Góa"]}
            defaultValue={teacherQuery.data?.maritalStatus}
          />
          <FormField
            name="permanentAddress"
            label="Địa chỉ thường trú"
            defaultValue={teacherQuery.data?.permanentAddress}
          />
        </CardContent>
      </Card>

      {/* Employment Information */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin công việc</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormField
            name="additionalDuties"
            label="Nhiệm vụ bổ sung"
            defaultValue={teacherQuery.data?.additionalDuties}
          />
          <FormField
            name="isHeadOfDepartment"
            label="Tổ trưởng bộ môn"
            type="boolean"
          />
          <FormField
            name="employmentType"
            label="Loại hợp đồng"
            type="select"
            options={[
              "Biên chế",
              "Hợp đồng dài hạn",
              "Hợp đồng ngắn hạn",
              "Cơ hữu",
            ]}
            defaultValue={teacherQuery.data?.employmentType}
          />
          <FormField
            name="employmentStatus"
            label="Trạng thái"
            type="select"
            defaultValue={["Đang làm việc", "Nghỉ phép", "Đã nghỉ việc"]}
          />
          <FormField
            name="recruitmentAgency"
            label="Cơ quan tuyển dụng"
            defaultValue={teacherQuery.data?.recruitmentAgency}
          />
          <FormField
            name="insuranceNumber"
            label="Số bảo hiểm"
            defaultValue={teacherQuery.data?.insuranceNumber}
          />
        </CardContent>
      </Card>

      {/* Employment Dates */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin ngày tháng</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
            <FormField name="hiringDate" label="Ngày tuyển dụng" type="date" />
            <FormField
              name="schoolJoinDate"
              label="Ngày vào trường"
              type="date"
            />
            <FormField
              name="permanentEmploymentDate"
              label="Ngày vào biên chế"
              type="date"
            />
          </div>
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
