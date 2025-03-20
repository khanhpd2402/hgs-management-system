import React from "react";
import { format } from "date-fns";
import { vi } from "date-fns/locale";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { CalendarIcon } from "lucide-react";
import { cn } from "@/lib/utils";
import { Switch } from "@/components/ui/switch";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Spinner } from "@/components/Spinner";

// Form schema with validation
const formSchema = z.object({
  fullName: z.string().min(2, { message: "Họ tên phải có ít nhất 2 ký tự" }),
  gender: z.string().min(1, { message: "Vui lòng chọn giới tính" }),
  dob: z
    .date()
    .max(new Date(), { message: "Ngày sinh không thể lớn hơn ngày hiện tại" })
    .optional(),
  idcardNumber: z
    .string()
    .regex(/^\d{9}$|^\d{12}$|^\d{13}$/, {
      message: "Số CMND/CCCD phải có 9, 12 hoặc 13 số",
    })
    .optional(),
  hometown: z.string().optional(),
  ethnicity: z.string().optional(),
  religion: z.string().optional(),
  maritalStatus: z.string().optional(),
  permanentAddress: z.string().optional(),
  position: z.string().optional(),
  department: z.string().optional(),
  additionalDuties: z.string().optional(),
  isHeadOfDepartment: z.boolean().optional(),
  employmentType: z.string().optional(),
  employmentStatus: z.string().optional(),
  recruitmentAgency: z.string().optional(),
  insuranceNumber: z.string().optional(),
  hiringDate: z.date().optional(),
  schoolJoinDate: z.date().optional(),
  permanentEmploymentDate: z.date().optional(),
});

export default function UpdateForm({ data, onCancel, onSubmit, isMutating }) {
  const form = useForm({
    resolver: zodResolver(formSchema),
    defaultValues: {
      fullName: data.fullName || "",
      gender: data.gender || "",
      dob: data.dob ? new Date(data.dob) : undefined,
      idcardNumber: data.idcardNumber || "",
      hometown: data.hometown || "",
      ethnicity: data.ethnicity || "",
      religion: data.religion || "",
      maritalStatus: data.maritalStatus || "",
      permanentAddress: data.permanentAddress || "",
      position: data.position || "",
      department: data.department || "",
      additionalDuties: data.additionalDuties || "",
      isHeadOfDepartment: data.isHeadOfDepartment || false,
      employmentType: data.employmentType || "",
      employmentStatus: data.employmentStatus || "",
      recruitmentAgency: data.recruitmentAgency || "",
      insuranceNumber: data.insuranceNumber || "",
      hiringDate: data.hiringDate ? new Date(data.hiringDate) : undefined,
      schoolJoinDate: data.schoolJoinDate
        ? new Date(data.schoolJoinDate)
        : undefined,
      permanentEmploymentDate: data.permanentEmploymentDate
        ? new Date(data.permanentEmploymentDate)
        : undefined,
    },
  });

  // Format date function
  const formatDate = (dateString) => {
    if (!dateString) return "";
    try {
      return format(new Date(dateString), "dd/MM/yyyy", { locale: vi });
    } catch (error) {
      return dateString;
    }
  };

  // Info item component for consistent styling
  const FormItem = ({ label, name, type = "text", options = [] }) => {
    if (type === "date") {
      return (
        <FormField
          control={form.control}
          name={name}
          render={({ field }) => (
            <FormItem className="flex flex-col py-2 sm:flex-row">
              <FormLabel className="w-full font-medium text-gray-500 sm:w-1/3">
                {label}:
              </FormLabel>
              <div className="w-full sm:w-2/3">
                <Popover>
                  <PopoverTrigger asChild>
                    <FormControl>
                      <Button
                        variant={"outline"}
                        className={cn(
                          "w-full pl-3 text-left font-normal",
                          !field.value && "text-muted-foreground",
                        )}
                      >
                        {field.value ? (
                          format(field.value, "dd/MM/yyyy")
                        ) : (
                          <span>Chọn ngày</span>
                        )}
                        <CalendarIcon className="ml-auto h-4 w-4 opacity-50" />
                      </Button>
                    </FormControl>
                  </PopoverTrigger>
                  <PopoverContent className="w-auto p-0" align="start">
                    <Calendar
                      mode="single"
                      selected={field.value}
                      onSelect={field.onChange}
                      disabled={(date) =>
                        name === "dob" ? date > new Date() : false
                      }
                      initialFocus
                    />
                  </PopoverContent>
                </Popover>
                <FormMessage />
              </div>
            </FormItem>
          )}
        />
      );
    } else if (type === "select") {
      return (
        <FormField
          control={form.control}
          name={name}
          render={({ field }) => (
            <FormItem className="flex flex-col py-2 sm:flex-row">
              <FormLabel className="w-full font-medium text-gray-500 sm:w-1/3">
                {label}:
              </FormLabel>
              <div className="w-full sm:w-2/3">
                <Select
                  onValueChange={field.onChange}
                  defaultValue={field.value}
                >
                  <FormControl>
                    <SelectTrigger>
                      <SelectValue placeholder="Chọn..." />
                    </SelectTrigger>
                  </FormControl>
                  <SelectContent>
                    {options.map((option) => (
                      <SelectItem key={option.value} value={option.value}>
                        {option.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                <FormMessage />
              </div>
            </FormItem>
          )}
        />
      );
    } else if (type === "switch") {
      return (
        <FormField
          control={form.control}
          name={name}
          render={({ field }) => (
            <FormItem className="flex flex-col py-2 sm:flex-row">
              <FormLabel className="w-full font-medium text-gray-500 sm:w-1/3">
                {label}:
              </FormLabel>
              <div className="flex w-full items-center sm:w-2/3">
                <FormControl>
                  <Switch
                    checked={field.value}
                    onCheckedChange={field.onChange}
                  />
                </FormControl>
                <span className="ml-2">{field.value ? "Có" : "Không"}</span>
                <FormMessage />
              </div>
            </FormItem>
          )}
        />
      );
    } else {
      return (
        <FormField
          control={form.control}
          name={name}
          render={({ field }) => (
            <FormItem className="flex flex-col py-2 sm:flex-row">
              <FormLabel className="w-full font-medium text-gray-500 sm:w-1/3">
                {label}:
              </FormLabel>
              <div className="w-full sm:w-2/3">
                <FormControl>
                  <Input {...field} />
                </FormControl>
                <FormMessage />
              </div>
            </FormItem>
          )}
        />
      );
    }
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
        {/* Personal Information */}
        <Card>
          <CardHeader>
            <CardTitle>Thông tin cá nhân</CardTitle>
          </CardHeader>
          <CardContent className="grid grid-cols-1 gap-x-6 lg:grid-cols-2">
            <div>
              <FormItem label="Họ và tên" name="fullName" />
              <FormItem
                label="Giới tính"
                name="gender"
                type="select"
                options={[
                  { value: "Nam", label: "Nam" },
                  { value: "Nữ", label: "Nữ" },
                  { value: "Khác", label: "Khác" },
                ]}
              />
              <FormItem label="Ngày sinh" name="dob" type="date" />
              <FormItem label="Số CMND/CCCD" name="idcardNumber" />
              <FormItem label="Quê quán" name="hometown" />
            </div>
            <div>
              <FormItem label="Dân tộc" name="ethnicity" />
              <FormItem label="Tôn giáo" name="religion" />
              <FormItem
                label="Tình trạng hôn nhân"
                name="maritalStatus"
                type="select"
                options={[
                  { value: "Độc thân", label: "Độc thân" },
                  { value: "Đã kết hôn", label: "Đã kết hôn" },
                  { value: "Ly hôn", label: "Ly hôn" },
                  { value: "Góa", label: "Góa" },
                ]}
              />
              <FormItem label="Địa chỉ thường trú" name="permanentAddress" />
            </div>
          </CardContent>
        </Card>

        {/* Employment Information */}
        <Card>
          <CardHeader>
            <CardTitle>Thông tin công việc</CardTitle>
          </CardHeader>
          <CardContent className="grid grid-cols-1 gap-x-6 lg:grid-cols-2">
            <div>
              <FormItem label="Vị trí" name="position" />
              <FormItem label="Bộ môn" name="department" />
              <FormItem label="Nhiệm vụ bổ sung" name="additionalDuties" />
              <FormItem
                label="Tổ trưởng bộ môn"
                name="isHeadOfDepartment"
                type="switch"
              />
            </div>
            <div>
              <FormItem
                label="Loại hợp đồng"
                name="employmentType"
                type="select"
                options={[
                  { value: "Toàn thời gian", label: "Toàn thời gian" },
                  { value: "Bán thời gian", label: "Bán thời gian" },
                  { value: "Hợp đồng", label: "Hợp đồng" },
                  { value: "Biên chế", label: "Biên chế" },
                ]}
              />
              <FormItem
                label="Trạng thái"
                name="employmentStatus"
                type="select"
                options={[
                  { value: "Đang làm việc", label: "Đang làm việc" },
                  { value: "Nghỉ phép", label: "Nghỉ phép" },
                  { value: "Đã nghỉ việc", label: "Đã nghỉ việc" },
                ]}
              />
              <FormItem label="Cơ quan tuyển dụng" name="recruitmentAgency" />
              <FormItem label="Số bảo hiểm" name="insuranceNumber" />
            </div>
          </CardContent>
        </Card>

        {/* Employment Dates */}
        <Card>
          <CardHeader>
            <CardTitle>Thông tin ngày tháng</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
              <div>
                <FormItem
                  label="Ngày tuyển dụng"
                  name="hiringDate"
                  type="date"
                />
              </div>
              <div>
                <FormItem
                  label="Ngày vào trường"
                  name="schoolJoinDate"
                  type="date"
                />
              </div>
              <div>
                <FormItem
                  label="Ngày vào biên chế"
                  name="permanentEmploymentDate"
                  type="date"
                />
              </div>
            </div>
          </CardContent>
        </Card>

        <div className="flex justify-end gap-2">
          <Button type="button" variant="outline" onClick={onCancel}>
            Hủy
          </Button>
          <Button type="submit" disabled={isMutating}>
            {isMutating ? <Spinner size="sm" /> : "Lưu"}
          </Button>
        </div>
      </form>
    </Form>
  );
}
