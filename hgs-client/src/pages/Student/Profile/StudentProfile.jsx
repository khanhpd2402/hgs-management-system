import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
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
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";

export default function StudentProfile() {
  const { id } = useParams();
  const { currentYear } = useLayout();
  const academicYearId = currentYear?.academicYearID || null;
  const studentQuery = useStudent({ id, academicYearId });
  const { mutate, isPending: isUpdating } = useUpdateStudent();

  const [showFatherInfo, setShowFatherInfo] = useState(false);
  const [showMotherInfo, setShowMotherInfo] = useState(false);
  const [showGuardianInfo, setShowGuardianInfo] = useState(false);

  const studentSchema = z
    .object({
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
        .min(1, "Số CCCD không được để trống")
        .refine(
          (val) => val.length === 12,
          "Số CCCD phải có chính xác 12 chữ số",
        )
        .refine((val) => /^\d+$/.test(val), "Số CCCD chỉ được chứa chữ số"),

      status: z.string().min(1, "Vui lòng chọn trạng thái"),

      // Father information
      fullNameFather: z
        .string()
        .min(showFatherInfo ? 1 : 0, "Họ và tên không được để trống"),
      yearOfBirthFather: z
        .union([z.date(), z.null()])
        .refine((val) => !showFatherInfo || val !== null, {
          message: "Vui lòng chọn ngày sinh",
        }),
      occupationFather: z
        .string()
        .min(showFatherInfo ? 1 : 0, "Nghề nghiệp không được để trống"),
      phoneNumberFather: z
        .string()
        .min(showFatherInfo ? 1 : 0, "Số điện thoại không được để trống")
        .refine((val) => !showFatherInfo || /^(0|\+84)[0-9]{9,10}$/.test(val), {
          message: "Số điện thoại không hợp lệ",
        }),
      emailFather: z
        .string()
        .email("Email không hợp lệ")
        .optional()
        .or(z.literal("")),
      idcardNumberFather: z
        .string()
        .superRefine((val, ctx) => {
          // Chỉ validate nếu showGuardianInfo là true
          if (showFatherInfo) {
            // Kiểm tra không được để trống
            if (!val || val.trim().length === 0) {
              ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Số CCCD không được để trống",
              });
            }
            // Kiểm tra độ dài 12 ký tự
            else if (val.length !== 12) {
              ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Số CCCD phải có chính xác 12 chữ số",
              });
            }
            // Kiểm tra chỉ chứa chữ số
            else if (!/^\d+$/.test(val)) {
              ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Số CCCD chỉ được chứa chữ số",
              });
            }
          }
          return z.NEVER;
        })
        .optional(),

      // Mother information
      fullNameMother: z
        .string()
        .min(showMotherInfo ? 1 : 0, "Họ và tên không được để trống"),
      yearOfBirthMother: z
        .union([z.date(), z.null()])
        .refine((val) => !showMotherInfo || val !== null, {
          message: "Vui lòng chọn ngày sinh",
        }),
      occupationMother: z
        .string()
        .min(showMotherInfo ? 1 : 0, "Nghề nghiệp không được để trống"),
      phoneNumberMother: z
        .string()
        .min(showMotherInfo ? 1 : 0, "Số điện thoại không được để trống")
        .refine((val) => !showMotherInfo || /^(0|\+84)[0-9]{9,10}$/.test(val), {
          message: "Số điện thoại không hợp lệ",
        }),
      idcardNumberMother: z
        .string()
        .superRefine((val, ctx) => {
          // Chỉ validate nếu showGuardianInfo là true
          if (showMotherInfo) {
            // Kiểm tra không được để trống
            if (!val || val.trim().length === 0) {
              ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Số CCCD không được để trống",
              });
            }
            // Kiểm tra độ dài 12 ký tự
            else if (val.length !== 12) {
              ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Số CCCD phải có chính xác 12 chữ số",
              });
            }
            // Kiểm tra chỉ chứa chữ số
            else if (!/^\d+$/.test(val)) {
              ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Số CCCD chỉ được chứa chữ số",
              });
            }
          }
          return z.NEVER;
        })
        .optional(),

      // Guardian information
      fullNameGuardian: z
        .string()
        .min(showGuardianInfo ? 1 : 0, "Họ và tên không được để trống"),
      yearOfBirthGuardian: z
        .union([z.date(), z.null()])
        .refine((val) => !showGuardianInfo || val !== null, {
          message: "Vui lòng chọn ngày sinh",
        }),
      occupationGuardian: z
        .string()
        .min(showGuardianInfo ? 1 : 0, "Nghề nghiệp không được để trống"),
      phoneNumberGuardian: z
        .string()
        .min(showGuardianInfo ? 1 : 0, "Số điện thoại không được để trống")
        .refine(
          (val) => !showGuardianInfo || /^(0|\+84)[0-9]{9,10}$/.test(val),
          {
            message: "Số điện thoại không hợp lệ",
          },
        ),
      emailGuardian: z
        .string()
        .email("Email không hợp lệ")
        .optional()
        .or(z.literal("")),
      idcardNumberGuardian: z
        .string()
        .superRefine((val, ctx) => {
          // Chỉ validate nếu showGuardianInfo là true
          if (showGuardianInfo) {
            // Kiểm tra không được để trống
            if (!val || val.trim().length === 0) {
              ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Số CCCD không được để trống",
              });
            }
            // Kiểm tra độ dài 12 ký tự
            else if (val.length !== 12) {
              ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Số CCCD phải có chính xác 12 chữ số",
              });
            }
            // Kiểm tra chỉ chứa chữ số
            else if (!/^\d+$/.test(val)) {
              ctx.addIssue({
                code: z.ZodIssueCode.custom,
                message: "Số CCCD chỉ được chứa chữ số",
              });
            }
          }
          return z.NEVER;
        })
        .optional(),
    })
    .superRefine((val, ctx) => {
      // Kiểm tra ít nhất 1 checkbox được chọn
      if (!showFatherInfo && !showMotherInfo && !showGuardianInfo) {
        ctx.addIssue({
          code: z.ZodIssueCode.custom,
          message:
            "Vui lòng chọn ít nhất 1 người thân (cha, mẹ hoặc người giám hộ)",
          path: ["familyInfoRequired"], // Tạo một path giả để hiển thị lỗi
        });
      }
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
      studentId: 0,
      fullName: "",
      gender: "",
      classId: 0,
      enrollmentType: "",
      repeatingYear: false,
      status: "",
      // Father information
      fullNameFather: "",
      yearOfBirthFather: "",
      occupationFather: "",
      phoneNumberFather: "",
      emailFather: "",
      idcardNumberFather: "",
      // Mother information
      fullNameMother: "",
      yearOfBirthMother: "",
      occupationMother: "",
      phoneNumberMother: "",
      emailMother: "",
      idcardNumberMother: "",
      // Guardian information
      fullNameGuardian: "",
      yearOfBirthGuardian: "",
      occupationGuardian: "",
      phoneNumberGuardian: "",
      emailGuardian: "",
      idcardNumberGuardian: "",
    },
  });

  // Initialize form when studentQuery.data is loaded
  useEffect(() => {
    if (studentQuery.data) {
      // Set form values from studentQuery.data
      Object.keys(studentQuery.data).forEach((key) => {
        // Handle date fields separately
        if (["dob", "admissionDate"].includes(key) && studentQuery.data[key]) {
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

      // Handle parent dates
      if (studentQuery.data.parents) {
        studentQuery.data.parents.forEach((parent, index) => {
          if (parent.dob) {
            try {
              setValue(`parents.${index}.dob`, new Date(parent.dob));
            } catch (e) {
              console.error(`Error parsing parent date of birth:`, e);
              setValue(`parents.${index}.dob`, null);
            }
          }
        });
      }
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
                  {options?.map((option) => (
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
            name="ethnicity"
            label="Dân tộc"
            defaultValue={studentQuery.data?.ethnicity}
          />
          <FormField
            name="religion"
            label="Tôn giáo"
            defaultValue={studentQuery.data?.religion}
          />
          <FormField
            name="idcardNumber"
            label="CCCD/CMND"
            defaultValue={studentQuery.data?.idcardNumber}
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
          <FormField
            name="enrollmentType"
            label="Hình thức trúng tuyển"
            defaultValue={studentQuery.data?.enrollmentType}
          />
          <FormField
            type="date"
            name="admissionDate"
            label="Ngày nhập học"
            defaultValue={studentQuery.data?.admissionDate}
          />
          <FormField
            name="birthPlace"
            label="Nơi sinh"
            defaultValue={studentQuery.data?.birthPlace}
          />
          <FormField
            name="permanentAddress"
            label="Địa chỉ thường trú"
            defaultValue={studentQuery.data?.permanentAddress}
          />
        </CardContent>
      </Card>

      {/* Father Information */}
      {studentQuery.data?.parents?.map((parent, index) => (
        <Card key={parent.parentId || index}>
          <CardHeader>
            <CardTitle>Thông tin {parent.relationship.toLowerCase()}</CardTitle>
          </CardHeader>
          <CardContent className="grid grid-cols-1 gap-4 md:grid-cols-2">
            <FormField
              name={`parents.${index}.fullName`}
              label="Họ và tên"
              defaultValue={parent.fullName}
            />
            <FormField
              name={`parents.${index}.occupation`}
              label="Nghề nghiệp"
              defaultValue={parent.occupation}
            />
            <FormField
              name={`parents.${index}.phoneNumber`}
              label="Số điện thoại"
              defaultValue={parent.phoneNumber}
            />
            <FormField
              name={`parents.${index}.dob`}
              label="Ngày sinh"
              type="date"
              defaultValue={parent.dob}
            />
            <FormField
              name={`parents.${index}.email`}
              label="Email"
              defaultValue={parent.email}
            />
          </CardContent>
        </Card>
      ))}

      {/* Submit button at bottom for convenience */}
      <div className="flex justify-end">
        <Button type="submit" disabled={isUpdating} size="lg">
          {isUpdating ? "Đang lưu..." : "Cập nhật thông tin"}
        </Button>
      </div>
    </form>
  );
}
