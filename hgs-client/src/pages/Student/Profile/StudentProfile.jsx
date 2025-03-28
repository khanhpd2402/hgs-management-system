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
import { formatDate } from "@/helpers/formatDate";

export default function StudentProfile() {
  const { id } = useParams();
  const { currentYear } = useLayout();
  const academicYearId = currentYear?.academicYearID;
  const studentQuery = useStudent({ id, academicYearId });
  const { mutate, isPending: isUpdating } = useUpdateStudent();

  console.log(studentQuery.data);

  const studentSchema = z.object({
    studentId: z.coerce.number().int().nonnegative(),
    fullName: z.string().min(1, "Vui lòng nhập họ và tên"),
    dob: z.string().min(1, "Date of birth is required"),
    gender: z.string().min(1, "Vui lòng chọn giới tính"),
    classId: z.coerce.number().int().nonnegative(),
    admissionDate: z.string().min(1, "Vui lòng nhập ngày nhập học"),
    enrollmentType: z.string().min(1, "Vui lòng nhập phương thức trúng tuyển"),
    ethnicity: z.string().optional(),
    permanentAddress: z.string().optional(),
    birthPlace: z.string().optional(),
    religion: z.string().optional(),
    repeatingYear: z.boolean().default(false),
    idcardNumber: zz
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
    status: z.string().min(1, "Vui lòng chọn trạng thái học sinh"),

    // Father information
    fullNameFather: z.string().optional(),
    yearOfBirthFather: z.string().optional(),
    occupationFather: z.string().optional(),
    phoneNumberFather: z.string().optional(),
    emailFather: z.string().optional(),
    idcardNumberFather: z.string().optional(),

    // Mother information
    fullNameMother: z.string().optional(),
    yearOfBirthMother: z.string().optional(),
    occupationMother: z.string().optional(),
    phoneNumberMother: z.string().optional(),
    emailMother: z.string().optional(),
    idcardNumberMother: z.string().optional(),

    // Guardian information
    fullNameGuardian: z.string().optional(),
    yearOfBirthGuardian: z.string().optional(),
    occupationGuardian: z.string().optional(),
    phoneNumberGuardian: z.string().optional(),
    emailGuardian: z.string().optional(),
    idcardNumberGuardian: z.string().optional(),
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
      {studentQuery.data?.parents.map((parent, index) => (
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
