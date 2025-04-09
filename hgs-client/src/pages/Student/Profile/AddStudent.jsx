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
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { useCreateStudent } from "@/services/student/mutation";
import { useForm, Controller } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";
import { useClasses } from "@/services/common/queries";
import { formatDate } from "@/helpers/formatDate";
import { useState } from "react";
import { useStudents } from "@/services/student/queries";

export default function AddStudent() {
  const { currentYear } = useLayout();
  const classQuery = useClasses();
  const academicYearId = currentYear?.academicYearID;
  const { mutate, isPending: isUpdating } = useCreateStudent();

  // State để lưu trữ checkbox nào được chọn
  const [showFatherInfo, setShowFatherInfo] = useState(false);
  const [showMotherInfo, setShowMotherInfo] = useState(false);
  const [showGuardianInfo, setShowGuardianInfo] = useState(false);
  const [showSiblingsModal, setShowSiblingsModal] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const studentsQuery = useStudents(academicYearId);
  const filteredStudents = studentsQuery.data?.students?.filter((student) =>
    student.fullName.toLowerCase().includes(searchQuery.toLowerCase()),
  );

  const handleChangeSibling = (student) => {
    const currentValues = watch();
    const parentInfo = {};

    if (student?.parent?.fullNameFather) {
      setShowFatherInfo(true);
      Object.assign(parentInfo, {
        fullNameFather: student.parent.fullNameFather,
        yearOfBirthFather: student.parent.yearOfBirthFather
          ? new Date(student.parent.yearOfBirthFather)
          : null,
        occupationFather: student.parent.occupationFather || "",
        phoneNumberFather: student.parent.phoneNumberFather || "",
        emailFather: student.parent.emailFather || "",
        idcardNumberFather: student.parent.idcardNumberFather || "",
      });
    } else {
      setShowFatherInfo(false);
    }

    if (student?.parent?.fullNameMother) {
      setShowMotherInfo(true);
      Object.assign(parentInfo, {
        fullNameMother: student.parent.fullNameMother,
        yearOfBirthMother: student.parent.yearOfBirthMother
          ? new Date(student.parent.yearOfBirthMother)
          : null,
        occupationMother: student.parent.occupationMother || "",
        phoneNumberMother: student.parent.phoneNumberMother || "",
        emailMother: student.parent.emailMother || "",
        idcardNumberMother: student.parent.idcardNumberMother || "",
      });
    } else {
      setShowMotherInfo(false);
    }

    if (student?.parent?.fullNameGuardian) {
      setShowGuardianInfo(true);
      Object.assign(parentInfo, {
        fullNameGuardian: student.parent.fullNameGuardian,
        yearOfBirthGuardian: student.parent.yearOfBirthGuardian
          ? new Date(student.parent.yearOfBirthGuardian)
          : null,
        occupationGuardian: student.parent.occupationGuardian || "",
        phoneNumberGuardian: student.parent.phoneNumberGuardian || "",
        emailGuardian: student.parent.emailGuardian || "",
        idcardNumberGuardian: student.parent.idcardNumberGuardian || "",
      });
    } else {
      setShowGuardianInfo(false);
    }

    // Reset form with all parent info at once
    reset({
      ...currentValues,
      ...parentInfo,
    });
  };

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
    watch,
    control,
    reset,
    formState: { errors },
  } = useForm({
    resolver: zodResolver(studentSchema),
    defaultValues: {
      fullName: "",
      gender: "",
      enrollmentType: "",
      classId: 0,
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
    console.log("submit");
    // if (!showFatherInfo && !showMotherInfo && !showGuardianInfo) {
    //   return;
    // }
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
    mutate(formattedData, {
      onSuccess: () => {
        // Reset form về giá trị mặc định
        reset({
          fullName: "",
          gender: "",
          enrollmentType: "",
          classId: 0,
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
        });
        // Reset các state
        setShowFatherInfo(false);
        setShowMotherInfo(false);
        setShowGuardianInfo(false);
        setSearchQuery("");
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
            defaultValue={watch(name) || ""}
            render={({ field }) => (
              <Select onValueChange={field.onChange} value={field.value}>
                <SelectTrigger>
                  <SelectValue placeholder={`Chọn ${label?.toLowerCase()}`} />
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
    <>
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="container mx-auto space-y-6 py-6"
      >
        {/* Header with title and save button */}
        <div className="flex items-center justify-between">
          <h1 className="text-3xl font-bold">Hồ sơ học sinh</h1>
          <Button type="submit" disabled={isUpdating}>
            {isUpdating ? "Đang lưu..." : "Lưu"}
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
                        <SelectItem
                          key={c.classId}
                          value={c.classId.toString()}
                        >
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
              options={[
                "Đang học",
                "Bảo lưu",
                "Tốt nghiệp",
                "Nghỉ học",
                "Chuyển trường",
              ]}
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

        {/* Family Information Section */}
        <Card>
          <CardHeader>
            <CardTitle>Thông tin gia đình</CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* Checkbox selection */}
            <div className="flex flex-wrap gap-4">
              <div className="flex items-center space-x-2">
                <input
                  type="checkbox"
                  id="showFatherInfo"
                  checked={showFatherInfo}
                  onChange={(e) => setShowFatherInfo(e.target.checked)}
                  className="h-4 w-4 rounded border-gray-300"
                />
                <Label htmlFor="showFatherInfo">Thông tin cha</Label>
              </div>
              <div className="flex items-center space-x-2">
                <input
                  type="checkbox"
                  id="showMotherInfo"
                  checked={showMotherInfo}
                  onChange={(e) => setShowMotherInfo(e.target.checked)}
                  className="h-4 w-4 rounded border-gray-300"
                />
                <Label htmlFor="showMotherInfo">Thông tin mẹ</Label>
              </div>
              <div className="flex items-center space-x-2">
                <input
                  type="checkbox"
                  id="showGuardianInfo"
                  checked={showGuardianInfo}
                  onChange={(e) => setShowGuardianInfo(e.target.checked)}
                  className="h-4 w-4 rounded border-gray-300"
                />
                <Label htmlFor="showGuardianInfo">Thông tin người bảo hộ</Label>
              </div>
              <div className="flex items-center space-x-2">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => setShowSiblingsModal(true)}
                >
                  Có anh/chị/em đang học
                </Button>
              </div>
            </div>
            {errors.familyInfoRequired && (
              <p className="text-sm text-red-500">
                {errors.familyInfoRequired.message}
              </p>
            )}

            {/* Father Information - chỉ hiển thị khi checkbox được chọn */}
            {showFatherInfo && (
              <div className="rounded-lg border p-4">
                <h3 className="mb-4 text-lg font-semibold">Thông tin cha</h3>
                <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                  <FormField name="fullNameFather" label="Họ và tên" />
                  <FormField
                    name="yearOfBirthFather"
                    label="Năm sinh"
                    type="date"
                  />
                  <FormField name="occupationFather" label="Nghề nghiệp" />
                  <FormField name="phoneNumberFather" label="Số điện thoại" />
                  <FormField name="emailFather" label="Email" />
                  <FormField name="idcardNumberFather" label="CCCD/CMND" />
                </div>
              </div>
            )}

            {/* Mother Information - chỉ hiển thị khi checkbox được chọn */}
            {showMotherInfo && (
              <div className="rounded-lg border p-4">
                <h3 className="mb-4 text-lg font-semibold">Thông tin mẹ</h3>
                <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                  <FormField name="fullNameMother" label="Họ và tên" />
                  <FormField
                    name="yearOfBirthMother"
                    label="Năm sinh"
                    type="date"
                  />
                  <FormField name="occupationMother" label="Nghề nghiệp" />
                  <FormField name="phoneNumberMother" label="Số điện thoại" />
                  <FormField name="emailMother" label="Email" />
                  <FormField name="idcardNumberMother" label="CCCD/CMND" />
                </div>
              </div>
            )}

            {/* Guardian Information - chỉ hiển thị khi checkbox được chọn */}
            {showGuardianInfo && (
              <div className="rounded-lg border p-4">
                <h3 className="mb-4 text-lg font-semibold">
                  Thông tin người giám hộ
                </h3>
                <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                  <FormField name="fullNameGuardian" label="Họ và tên" />
                  <FormField
                    name="yearOfBirthGuardian"
                    label="Năm sinh"
                    type="date"
                  />
                  <FormField name="occupationGuardian" label="Nghề nghiệp" />
                  <FormField name="phoneNumberGuardian" label="Số điện thoại" />
                  <FormField name="emailGuardian" label="Email" />
                  <FormField name="idcardNumberGuardian" label="CCCD/CMND" />
                </div>
              </div>
            )}
          </CardContent>
        </Card>

        {/* Submit button at bottom for convenience */}
        <div className="flex justify-end">
          <Button type="submit" disabled={isUpdating} size="lg">
            {isUpdating ? "Đang lưu..." : "Lưu"}
          </Button>
        </div>
      </form>
      <Dialog open={showSiblingsModal} onOpenChange={setShowSiblingsModal}>
        <DialogContent className="!w-full !max-w-fit">
          <DialogHeader>
            <DialogTitle>Danh sách học sinh</DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <Input
              placeholder="Tìm kiếm theo tên..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="mt-2"
            />
            <div className="max-h-[400px] overflow-y-auto">
              <table className="w-full">
                <thead className="">
                  <tr className="bg-background sticky top-0 z-10 border-b">
                    <th className="p-2 text-left">Họ và tên</th>
                    <th className="p-2 text-left">Lớp</th>
                    <th className="p-2 text-left">Họ tên cha</th>
                    <th className="p-2 text-left">Họ tên mẹ</th>
                    <th className="p-2 text-left">Họ tên người bảo hộ</th>
                    <th className="p-2 text-left">Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredStudents && filteredStudents.length > 0 ? (
                    filteredStudents?.map((student) => (
                      <tr key={student.studentId} className="border-b">
                        <td className="w-50 p-2">{student.fullName}</td>
                        <td className="w-14 p-2">{student.className}</td>
                        <td className="w-50 p-2">
                          {student?.parent?.fullNameFather}
                        </td>
                        <td className="w-50 p-2">
                          {student?.parent?.fullNameMother}
                        </td>
                        <td className="w-50 p-2">
                          {student?.parent?.fullNameGuardian}
                        </td>
                        <td className="w-30 p-2">
                          <Button
                            type="button"
                            variant="outline"
                            size="sm"
                            onClick={() => {
                              setShowSiblingsModal(false);
                              handleChangeSibling(student);
                            }}
                          >
                            Chọn
                          </Button>
                        </td>
                      </tr>
                    ))
                  ) : (
                    <td colSpan={10}>Đang tải dữ liệu</td>
                  )}
                </tbody>
              </table>
            </div>
          </div>
        </DialogContent>
      </Dialog>
    </>
  );
}
