import { z } from "zod";

export const studentSchema = z.object({
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
    .refine((val) => val.length === 12, "Số CCCD phải có chính xác 12 chữ số")
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
    .min(1, "Số CCCD không được để trống")
    .refine((val) => val.length === 12, "Số CCCD phải có chính xác 12 chữ số")
    .refine((val) => /^\d+$/.test(val), "Số CCCD chỉ được chứa chữ số"),

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
    .min(1, "Số CCCD không được để trống")
    .refine((val) => val.length === 12, "Số CCCD phải có chính xác 12 chữ số")
    .refine((val) => /^\d+$/.test(val), "Số CCCD chỉ được chứa chữ số"),

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
    .refine((val) => !showGuardianInfo || /^(0|\+84)[0-9]{9,10}$/.test(val), {
      message: "Số điện thoại không hợp lệ",
    }),
  emailGuardian: z
    .string()
    .email("Email không hợp lệ")
    .optional()
    .or(z.literal("")),
  idcardNumberGuardian: z
    .string()
    .min(1, "Số CCCD không được để trống")
    .refine((val) => val.length === 12, "Số CCCD phải có chính xác 12 chữ số")
    .refine((val) => /^\d+$/.test(val), "Số CCCD chỉ được chứa chữ số"),
});
