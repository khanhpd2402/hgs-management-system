import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import DatePicker from "@/components/DatePicker";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { useCreateStudent } from "@/services/student/mutation";
import { useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";
import { useClasses } from "@/services/common/queries";
import { formatDate } from "@/helpers/formatDate";

export default function AddStudent() {
  const { currentYear } = useLayout();
  const classQuery = useClasses();
  const academicYearId = currentYear?.academicYearID;
  const { mutate, isPending: isUpdating } = useCreateStudent();

  const studentSchema = z.object({
    fullName: z.string().min(1, "Họ và tên không được để trống"),
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
    gender: z.string().min(1, "Vui lòng chọn giới tính"),
    classId: z.coerce
      .number()
      .int()
      .refine((val) => val > 0, {
        message: "Vui lòng chọn lớp",
      }),
    admissionDate: z
      .date()
      .nullable()
      .superRefine((val, ctx) => {
        if (val === null) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Vui lòng chọn ngày nhập học",
          });
        }
      }),
    enrollmentType: z
      .string()
      .min(1, "Hình thức trúng tuyển không được để trống"),
    ethnicity: z.string().optional(),
    permanentAddress: z.string().min(1, "Họ và tên không được để trống"),
    birthPlace: z.string().min(1, "Họ và tên không được để trống"),
    religion: z.string().optional(),
    repeatingYear: z.boolean().default(false),
    idcardNumber: z
      .string()
      .optional()
      .refine(
        (val) => !val || val.length === 8 || val.length === 12,
        "Số CMND/CCCD phải có 8 hoặc 12 chữ số",
      )
      .refine(
        (val) => !val || /^\d+$/.test(val),
        "Số CMND/CCCD chỉ được chứa chữ số",
      ),
    status: z.string().min(1, "Vui lòng chọn trạng thái"),

    // Father information
    fullNameFather: z.string().optional(),
    yearOfBirthFather: z
      .union([z.date(), z.null()])
      .optional()
      .superRefine((val, ctx) => {
        if (val === null) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Vui lòng chọn ngày sinh",
          });
        }
      }),
    occupationFather: z.string().optional(),
    phoneNumberFather: z.string().optional(),
    emailFather: z
      .string()
      .email("Email không hợp lệ")
      .optional()
      .or(z.literal("")),
    idcardNumberFather: z
      .string()
      .optional()
      .refine(
        (val) => !val || val.length === 9 || val.length === 12,
        "Số CMND/CCCD phải có 9 hoặc 12 chữ số",
      )
      .refine(
        (val) => !val || /^\d+$/.test(val),
        "Số CMND/CCCD chỉ được chứa chữ số",
      ),

    // Mother information
    fullNameMother: z.string().optional(),
    yearOfBirthMother: z
      .date()
      .optional()
      .superRefine((val, ctx) => {
        if (val === null) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Vui lòng chọn ngày sinh",
          });
        }
      }),
    occupationMother: z.string().optional(),
    phoneNumberMother: z
      .string()
      .optional()
      .refine((val) => !val || /^(0|\+84)[0-9]{9,10}$/.test(val), {
        message: "Số điện thoại không hợp lệ",
      }),
    emailMother: z
      .string()
      .email("Email không hợp lệ")
      .optional()
      .or(z.literal("")),
    idcardNumberMother: z
      .string()
      .optional()
      .refine(
        (val) => !val || val.length === 9 || val.length === 12,
        "Số CMND/CCCD phải có 9 hoặc 12 chữ số",
      )
      .refine(
        (val) => !val || /^\d+$/.test(val),
        "Số CMND/CCCD chỉ được chứa chữ số",
      ),

    // Guardian information
    fullNameGuardian: z.string().optional(),
    yearOfBirthGuardian: z
      .date()
      .optional()
      .superRefine((val, ctx) => {
        if (val === null) {
          ctx.addIssue({
            code: z.ZodIssueCode.custom,
            message: "Vui lòng chọn ngày sinh",
          });
        }
      }),
    occupationGuardian: z.string().optional(),
    phoneNumberGuardian: z
      .string()
      .optional()
      .refine((val) => !val || /^(0|\+84)[0-9]{9,10}$/.test(val), {
        message: "Số điện thoại không hợp lệ",
      }),
    emailGuardian: z
      .string()
      .email("Email không hợp lệ")
      .optional()
      .or(z.literal("")),
    idcardNumberGuardian: z
      .string()
      .optional()
      .refine(
        (val) => !val || val.length === 9 || val.length === 12,
        "Số CMND/CCCD phải có 9 hoặc 12 chữ số",
      )
      .refine(
        (val) => !val || /^\d+$/.test(val),
        "Số CMND/CCCD chỉ được chứa chữ số",
      ),
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
      classId: null,
      fullName: "",
      gender: "",
      enrollmentType: "",
      dob: null,
      admissionDate: null,

      permanentAddress: "",
      repeatingYear: false,
      status: "",
      // Father information
      fullNameFather: "",
      occupationFather: "",
      phoneNumberFather: "",
      emailFather: "",
      idcardNumberFather: "",
      yearOfBirthFather: null,
      // Mother information
      fullNameMother: "",
      occupationMother: "",
      phoneNumberMother: "",
      emailMother: "",
      yearOfBirthMother: null,
      // Guardian information
      fullNameGuardian: "",
      occupationGuardian: "",
      phoneNumberGuardian: "",
      emailGuardian: "",
      yearOfBirthGuardian: null,
    },
  });

  // Submit handler
  const onSubmit = (formData) => {
    const formattedData = {
      ...formData,
      dob: formData.dob ? formatDate(formData.dob) : null,
      admissionDate: formData.admissionDate
        ? formatDate(formData.admissionDate)
        : null,
      yearOfBirthFather: formData.yearOfBirthFather
        ? formatDate(formData.yearOfBirthFather)
        : null,
      yearOfBirthMother: formData.yearOfBirthMother
        ? formatDate(formData.yearOfBirthMother)
        : null,
      yearOfBirthGuardian: formData.yearOfBirthGuardian
        ? formatDate(formData.yearOfBirthGuardian)
        : null,
    };
    console.log(formattedData);
    mutate(formattedData);
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
            <div className="flex-1">
              <FormField name="fullName" label="Họ và tên" />
            </div>
          </div>
          <FormField name="dob" label="Ngày sinh" type="date" />
          <FormField
            name="gender"
            label="Giới tính"
            type="select"
            options={["Nam", "Nữ", "Khác"]}
          />
          <FormField name="ethnicity" label="Dân tộc" />
          <FormField name="religion" label="Tôn giáo" />
          <FormField name="idcardNumber" label="CCCD/CMND" />
          <div className="space-y-2">
            <Label htmlFor="classId">Lớp</Label>
            <Controller
              name="classId"
              control={control}
              render={({ field }) => (
                <Select
                  onValueChange={(value) => field.onChange(parseInt(value))}
                  value={
                    field.value && field.value !== 0
                      ? field.value.toString()
                      : undefined
                  }
                >
                  <SelectTrigger>
                    <SelectValue placeholder="Chọn lớp" />
                  </SelectTrigger>
                  <SelectContent>
                    {classQuery.data?.map((c) => (
                      <SelectItem key={c.classId} value={c.classId.toString()}>
                        {c.className}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              )}
            />
            {errors.classId && (
              <p className="text-sm text-red-500">{errors.classId.message}</p>
            )}
          </div>
          <FormField
            name="status"
            label="Trạng thái"
            type="select"
            options={["Đang học", "Bảo lưu", "Đã tốt nghiệp", "Đã nghỉ học"]}
          />
          <FormField name="enrollmentType" label="Hình thức trúng tuyển" />
          <FormField name="admissionDate" label="Ngày nhập học" type="date" />
          <FormField name="birthPlace" label="Nơi sinh" />
          <FormField name="permanentAddress" label="Địa chỉ thường trú" />
          <div className="flex items-center space-x-2">
            <input
              type="checkbox"
              id="repeatingYear"
              {...register("repeatingYear")}
              className="h-4 w-4 rounded border-gray-300"
            />
            <Label htmlFor="repeatingYear">Học sinh lưu ban</Label>
          </div>
        </CardContent>
      </Card>

      {/* Father Information */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin cha</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormField name="fullNameFather" label="Họ và tên" />
          <FormField name="yearOfBirthFather" label="Năm sinh" type="date" />
          <FormField name="occupationFather" label="Nghề nghiệp" />
          <FormField name="phoneNumberFather" label="Số điện thoại" />
          <FormField name="emailFather" label="Email" />
          <FormField name="idcardNumberFather" label="CCCD/CMND" />
        </CardContent>
      </Card>

      {/* Mother Information */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin mẹ</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormField name="fullNameMother" label="Họ và tên" />
          <FormField name="yearOfBirthMother" label="Năm sinh" type="date" />
          <FormField name="occupationMother" label="Nghề nghiệp" />
          <FormField name="phoneNumberMother" label="Số điện thoại" />
          <FormField name="emailMother" label="Email" />
          <FormField name="idcardNumberMother" label="CCCD/CMND" />
        </CardContent>
      </Card>

      {/* Guardian Information */}
      <Card>
        <CardHeader>
          <CardTitle>Thông tin người giám hộ</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
          <FormField name="fullNameGuardian" label="Họ và tên" />
          <FormField name="yearOfBirthGuardian" label="Năm sinh" type="date" />
          <FormField name="occupationGuardian" label="Nghề nghiệp" />
          <FormField name="phoneNumberGuardian" label="Số điện thoại" />
          <FormField name="emailGuardian" label="Email" />
          <FormField name="idcardNumberGuardian" label="CCCD/CMND" />
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
