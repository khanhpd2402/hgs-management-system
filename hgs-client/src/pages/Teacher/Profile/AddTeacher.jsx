import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useForm, Controller } from "react-hook-form";
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
import { useCreateTeacher } from "@/services/teacher/mutation";
import { useNavigate } from "react-router-dom";

export default function AddTeacher() {
  const navigate = useNavigate();
  const { mutate, isPending: isCreating } = useCreateTeacher();

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
    dob: z
      .date()
      .nullable()
      .superRefine((val, ctx) => {
        if (val === null) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Vui lòng chọn ngày sinh",
          });
        }
      }),
    idcardNumber: z
      .string()
      .refine(
        (val) => !val || val.length === 9 || val.length === 12,
        "Số CMND/CCCD phải có 9 hoặc 12 chữ số",
      )
      .refine(
        (val) => !val || /^\d+$/.test(val),
        "Số CMND/CCCD chỉ được chứa chữ số",
      ),
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
    hiringDate: z
      .date()
      .nullable()
      .superRefine((val, ctx) => {
        if (val === null) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Vui lòng chọn ngày tuyển dụng",
          });
        }
      }),
    schoolJoinDate: z
      .date()
      .nullable()
      .superRefine((val, ctx) => {
        if (val === null) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Vui lòng chọn ngày vào trường",
          });
        }
      }),
    permanentEmploymentDate: z
      .date()
      .nullable()
      .superRefine((val, ctx) => {
        if (val === null) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Vui lòng chọn ngày vào biên chế",
          });
        }
      }),
    phoneNumber: z
      .string()
      .min(10, "Số điện thoại phải có ít nhất 10 chữ số")
      .max(11, "Số điện thoại không được quá 11 chữ số")
      .regex(
        /^(0[2-9]|84[2-9])\d{8}$/,
        "Số điện thoại không hợp lệ. Phải bắt đầu bằng 0 hoặc +84 và có 10-11 chữ số",
      ),
    email: z.string().email("Email không hợp lệ"),
  });

  const {
    register,
    handleSubmit,
    control,
    reset,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(teacherSchema),
    defaultValues: {
      fullName: "",
      position: "",
      department: "",
      gender: "Nam",
      dob: null,
      idcardNumber: "",
      hometown: "",
      ethnicity: "",
      religion: "",
      maritalStatus: "Độc thân",
      permanentAddress: "",
      additionalDuties: "",
      isHeadOfDepartment: false,
      employmentType: "Hợp đồng dài hạn",
      employmentStatus: "Đang làm việc",
      recruitmentAgency: "",
      insuranceNumber: "",
      hiringDate: null,
      schoolJoinDate: null,
      permanentEmploymentDate: null,
      phoneNumber: "",
      email: "",
    },
  });

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
    const data = mutate(processedData, {
      onSuccess: () => {
        reset();
      },
    });
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
            render={({ field }) => (
              <Select onValueChange={field.onChange} value={field.value || ""}>
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
        <h1 className="text-3xl font-bold">Thêm giáo viên mới</h1>
        <div className="flex gap-2">
          <Button
            type="button"
            variant="outline"
            onClick={() => navigate("/teacher/profile")}
          >
            Hủy
          </Button>
          <Button type="submit" disabled={isCreating}>
            {isCreating ? "Đang thêm..." : "Lưu giáo viên"}
          </Button>
        </div>
      </div>

      {/* Basic Info Card */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin cơ bản</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <div className="md:col-span-2">
            <FormField name="fullName" label="Họ và tên" />
          </div>
          <FormField name="position" label="Vị trí" />
          <FormField name="department" label="Bộ môn" />
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
          />
          <FormField name="dob" label="Ngày sinh" type="date" />
          <FormField name="phoneNumber" label="Số điện thoại" />{" "}
          <FormField name="email" label="Email" />
          <FormField name="idcardNumber" label="Số CMND/CCCD" />
          <FormField name="hometown" label="Quê quán" />
          <FormField name="ethnicity" label="Dân tộc" />
          <FormField name="religion" label="Tôn giáo" />
          <FormField
            name="maritalStatus"
            label="Tình trạng hôn nhân"
            type="select"
            options={["Độc thân", "Đã kết hôn", "Ly hôn", "Góa"]}
          />
          <FormField name="permanentAddress" label="Địa chỉ thường trú" />
        </CardContent>
      </Card>

      {/* Employment Information */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin công việc</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormField name="additionalDuties" label="Nhiệm vụ bổ sung" />
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
          />
          <FormField
            name="employmentStatus"
            label="Trạng thái"
            type="select"
            options={["Đang làm việc", "Nghỉ phép", "Đã nghỉ việc"]}
          />
          <FormField name="recruitmentAgency" label="Cơ quan tuyển dụng" />
          <FormField name="insuranceNumber" label="Số bảo hiểm" />
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

      {/* Submit buttons at bottom for convenience */}
      <div className="flex justify-end gap-2">
        <Button
          type="button"
          variant="outline"
          onClick={() => navigate("/teachers")}
        >
          Hủy
        </Button>
        <Button type="submit" disabled={isCreating} size="lg">
          {isCreating ? "Đang thêm..." : "Lưu giáo viên"}
        </Button>
      </div>
    </form>
  );
}
