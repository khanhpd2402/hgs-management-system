import { useState } from "react";
import {
  useSemestersByAcademicYear,
  useSubjects,
} from "@/services/common/queries";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Checkbox } from "@/components/ui/checkbox";
import DatePicker from "@/components/DatePicker";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { useAddGradeBatch } from "@/services/principal/mutation";
import { formatDate } from "@/helpers/formatDate";
import { PlusCircle } from "lucide-react";
import { useLayout } from "@/layouts/DefaultLayout/DefaultLayout";
import toast from "react-hot-toast";
import { cleanString } from "@/helpers/removeWhiteSpace";

export default function AddGradeBatch({ semester }) {
  const { currentYear } = useLayout();
  const semesterQuery = useSemestersByAcademicYear(currentYear?.academicYearID);
  const semesters = semesterQuery.data || [];
  const currentSemester = semesters?.find((s) => s.semesterID === semester);
  const semesterStartDate = currentSemester
    ? new Date(currentSemester.startDate)
    : null;
  const semesterEndDate = currentSemester
    ? new Date(currentSemester.endDate)
    : null;

  const gradeBatchMutation = useAddGradeBatch();
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [formData, setFormData] = useState({
    name: "",
    startDate: null,
    endDate: null,
    isLocked: false,
    gradeColumns: {
      regular: false,
      midterm: false,
      final: false,
    },
    regularColumnCount: 2,
    subjects: {},
  });
  const [errors, setErrors] = useState({});

  const validateName = (name) => {
    const cleanedName = cleanString(name.trim());
    if (cleanedName.length > 50) {
      return "Tên đợt không được vượt quá 30 ký tự";
    }
    if (!/^[\p{L}\p{N} ]*$/u.test(cleanedName)) {
      return "Tên đợt không được chứa ký tự đặc biệt";
    }
    if (!/[\p{L}]/u.test(cleanedName)) {
      return "Tên đợt phải chứa ít nhất 1 chữ cái";
    }
    return "";
  };

  const validateStartDate = (startDate, endDate) => {
    if (!semesterStartDate || !semesterEndDate) return "";
    if (!startDate) return "";
    if (startDate <= semesterStartDate)
      return "Ngày bắt đầu phải lớn hơn ngày bắt đầu học kỳ";
    if (startDate >= semesterEndDate)
      return "Ngày bắt đầu phải nhỏ hơn ngày kết thúc học kỳ";
    if (endDate && startDate >= endDate)
      return "Ngày bắt đầu phải nhỏ hơn ngày kết thúc đợt";
    return "";
  };

  const validateEndDate = (endDate, startDate) => {
    if (!semesterStartDate || !semesterEndDate) return "";
    if (!endDate) return "";
    if (endDate <= semesterStartDate)
      return "Ngày kết thúc phải lớn hơn ngày bắt đầu học kỳ";
    if (startDate && endDate <= startDate)
      return "Ngày kết thúc phải lớn hơn ngày bắt đầu đợt";
    if (endDate >= semesterEndDate)
      return "Ngày kết thúc phải nhỏ hơn ngày kết thúc học kỳ";
    return "";
  };

  const isFormValid = formData.name && formData.startDate && formData.endDate;

    // Validate name
    if (!formData.name.trim()) {
      newErrors.name = "Tên đợt không được để trống";
    }

    // Validate dates
    if (!formData.startDate) {
      newErrors.startDate = "Ngày bắt đầu không được để trống";
    }

    if (!formData.endDate) {
      newErrors.endDate = "Ngày kết thúc không được để trống";
    }

    if (
      formData.startDate &&
      formData.endDate &&
      formData.startDate > formData.endDate
    ) {
      newErrors.dateRange = "Ngày bắt đầu phải trước ngày kết thúc";
    }

    // Validate at least one grade column is selected
    const hasGradeColumn = Object.values(formData.gradeColumns).some(
      (value) => value,
    );
    if (!hasGradeColumn) {
      newErrors.gradeColumns = "Phải chọn ít nhất một cột điểm";
    }

    // Validate at least one subject is selected
    const hasSubject = Object.values(formData.subjects).some((value) => value);
    if (!hasSubject) {
      newErrors.subjects = "Phải chọn ít nhất một môn học";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = () => {
    const nameError = validateName(formData.name);
    if (nameError) {
      toast.error(nameError);
      return;
    }
      }

      if (formData.gradeColumns.midterm) {
        gradeTypes.push("DDGGK");
      }

      if (formData.gradeColumns.final) {
        gradeTypes.push("DDGCK");
      }
      console.log(formData.startDate, formData.endDate);

    const payload = {
      academicYearId: currentYear?.academicYearID,
      batchName: cleanString(formData.name.trim()),
      semesterId: semester,
      startDate: formatDate(formData.startDate),
      endDate: formatDate(formData.endDate),
      status: formData.isLocked ? "Không Hoạt Động" : "Hoạt Động",
    };

    console.log("Form data submitted:", payload);

    gradeBatchMutation.mutate(payload);

    setIsModalOpen(false);
    }
  };

  const resetForm = () => {
    setFormData({
      name: "",
      startDate: null,
      endDate: null,
      isLocked: false,
      gradeColumns: {
        regular: false,
        midterm: false,
        final: false,
      },
      regularColumnCount: 2, // Ensure this is set to 2 by default
      subjects: subjectsQuery.data
        ? subjectsQuery.data.reduce((acc, subject) => {
            acc[subject.subjectId] = false;
            return acc;
          }, {})
        : {},
    });
    setErrors({});
  };

  const openModal = () => {
    resetForm();
    setIsModalOpen(true);
  };

  // Map Vietnamese grade column names
  const gradeColumnLabels = {
    regular: "Thường xuyên",
    midterm: "Giữa học kỳ",
    final: "Cuối học kỳ",
  };
  const regularColumnOptions = [2, 3, 4];

  return (
    <div className="">
      <Button
        onClick={openModal}
        className="flex items-center gap-2 rounded-md bg-blue-600 text-white shadow-md transition-all hover:bg-blue-700 hover:shadow-lg"
      >
        <PlusCircle size={18} />
        <span>Thêm đợt nhập điểm</span>
      </Button>

      <Dialog open={isModalOpen} onOpenChange={setIsModalOpen}>
        <DialogContent className="max-h-[90vh] overflow-y-auto rounded-lg border-0 shadow-lg sm:max-w-[600px]">
          <DialogHeader className="">
            <DialogTitle className="text-xl font-bold">
              Thêm mới đợt nhập điểm
            </DialogTitle>
            <Button
              variant="ghost"
              className="ring-offset-background absolute top-4 right-4 rounded-sm opacity-70 transition-opacity hover:opacity-100"
              onClick={() => setIsModalOpen(false)}
            ></Button>
          </DialogHeader>

          <div className="grid gap-5 px-4 py-6">
            {/* Tên đợt */}
            <div className="grid grid-cols-4 items-center gap-4">
              <label htmlFor="name" className="font-medium text-gray-700">
                Tên đợt
              </label>
              <div className="col-span-3">
                <Input
                  id="name"
                  name="name"
                  value={formData.name}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      name: e.target.value,
                    }))
                  }
                  className={`focus:ring-opacity-50 rounded-md border-gray-300 focus:border-blue-500 focus:ring focus:ring-blue-200`}
                  placeholder="Nhập tên đợt nhập điểm"
                />
                {errors.name && (
                  <p className="mt-1 text-sm text-red-500">{errors.name}</p>
                )}
              </div>
            </div>

            {/* Thời gian */}
            <div className="grid grid-cols-4 items-center gap-4">
              <label className="font-medium text-gray-700">Từ ngày</label>
              <div className="col-span-3">
                <DatePicker
                  value={formData.startDate}
                  onSelect={(date) => {
                    const startDateError = validateStartDate(
                      formData.startDate,
                      formData.endDate,
                    );
                    if (startDateError) {
                      toast.error(startDateError);
                      return;
                    }
                    return setFormData((prev) => ({
                      ...prev,
                      startDate: date,
                    }));
                  }}
                  disabled={false}
                  className="w-full"
                />
                {errors.startDate && (
                  <p className="mt-1 text-sm text-red-500">
                    {errors.startDate}
                  </p>
                )}
              </div>
            </div>

            <div className="grid grid-cols-4 items-center gap-4">
              <label className="font-medium text-gray-700">Đến ngày</label>
              <div className="col-span-3">
                <DatePicker
                  value={formData.endDate}
                  onSelect={(date) => {
                    const endDateError = validateEndDate(
                      formData.endDate,
                      formData.startDate,
                    );
                    if (endDateError) {
                      toast.error(endDateError);
                      return;
                    }
                    return setFormData((prev) => ({
                      ...prev,
                      endDate: date,
                    }));
                  }}
                  disabled={false}
                  className="w-full"
                />
                {errors.endDate && (
                  <p className="mt-1 text-sm text-red-500">{errors.endDate}</p>
                )}
                {errors.dateRange && (
                  <p className="mt-1 text-sm text-red-500">
                    {errors.dateRange}
                  </p>
                )}
              </div>
            </div>

            {/* Khóa đợt */}
            <div className="grid grid-cols-4 items-center gap-4">
              <div className="col-span-4">
                <div className="flex items-center space-x-2 rounded-md bg-gray-50">
                  <Checkbox
                    id="isLocked"
                    checked={formData.isLocked}
                    onCheckedChange={(checked) =>
                      setFormData((prev) => ({ ...prev, isLocked: checked }))
                    }
                    className="h-5 w-5 text-blue-600"
                  />
                  <label
                    htmlFor="isLocked"
                    className="text-sm leading-none font-medium peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
                  >
                    Khóa đợt
                  </label>
                </div>
              </div>
            </div>

            {/* Các cột điểm */}
            <div className="grid grid-cols-4 gap-4">
              <label className="text-right font-medium text-gray-700">
                Các cột điểm của đợt
              </label>
              <div className="col-span-3">
                <div className="grid grid-cols-3 gap-4 rounded-md bg-gray-50 p-3">
                  {Object.entries(gradeColumnLabels).map(([key, label]) => (
                    <div key={key} className="flex items-center space-x-2">
                      <Checkbox
                        id={`grade-${key}`}
                        checked={formData.gradeColumns[key]}
                        onCheckedChange={() =>
                          handleCheckboxChange("gradeColumns", key)
                        }
                        className="h-5 w-5 text-blue-600"
                      />
                      <label
                        htmlFor={`grade-${key}`}
                        className="text-sm leading-none font-medium"
                      >
                        {label}
                      </label>
                    </div>
                  ))}
                </div>
                {formData.gradeColumns.regular && (
                  <div className="mt-3 rounded-md bg-blue-50 p-3 pl-6">
                    <label className="mb-2 block text-sm font-medium text-gray-700">
                      Số đầu điểm thường xuyên:
                    </label>
                    <div className="flex space-x-4">
                      {regularColumnOptions.map((count) => (
                        <div
                          key={count}
                          className="flex items-center space-x-2"
                        >
                          <input
                            type="radio"
                            id={`regular-count-${count}`}
                            name="regularColumnCount"
                            value={count}
                            checked={formData.regularColumnCount === count}
                            onChange={() =>
                              setFormData((prev) => ({
                                ...prev,
                                regularColumnCount: count,
                              }))
                            }
                            className="h-4 w-4 text-blue-600"
                          />
                          <label
                            htmlFor={`regular-count-${count}`}
                            className="text-sm leading-none"
                          >
                            {count} đầu điểm
                          </label>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
                {errors.gradeColumns && (
                  <p className="mt-1 text-sm text-red-500">
                    {errors.gradeColumns}
                  </p>
                )}
              </div>
            </div>

            {/* Môn học áp dụng */}
            <div className="grid grid-cols-4 gap-4">
              <label className="text-right font-medium text-gray-700">
                Môn học áp dụng
              </label>
              <div className="col-span-3">
                <div className="grid max-h-[200px] grid-cols-3 gap-2 overflow-y-auto rounded-md bg-gray-50 p-3">
                  {subjectsQuery.isLoading ? (
                    <p className="text-gray-500">
                      Đang tải danh sách môn học...
                    </p>
                  ) : subjectsQuery.isError ? (
                    <p className="text-red-500">
                      Lỗi khi tải danh sách môn học
                    </p>
                  ) : (
                    subjectsQuery.data?.map((subject) => (
                      <div
                        key={subject.subjectId}
                        className="flex items-center space-x-2 rounded p-1 transition-colors hover:bg-blue-50"
                      >
                        <Checkbox
                          id={`subject-${subject.subjectId}`}
                          checked={
                            formData.subjects[subject.subjectId] || false
                          }
                          onCheckedChange={() =>
                            handleCheckboxChange("subjects", subject.subjectId)
                          }
                          className="h-5 w-5 text-blue-600"
                        />
                        <label
                          htmlFor={`subject-${subject.subjectId}`}
                          className="text-sm leading-none font-medium"
                        >
                          {subject.subjectName}
                        </label>
                      </div>
                    ))
                  )}
                </div>
                {errors.subjects && (
                  <p className="mt-1 text-sm text-red-500">{errors.subjects}</p>
                )}
              </div>
            </div>
          </div>

          <DialogFooter className="flex justify-end gap-2 rounded-b-lg bg-gray-50">
            <Button
              variant="outline"
              onClick={() => setIsModalOpen(false)}
              className="border-gray-300 text-gray-700 hover:bg-gray-100"
            >
              Hủy bỏ
            </Button>
            <Button
              onClick={handleSubmit}
              className="bg-blue-600 text-white hover:bg-blue-700"
              disabled={!isFormValid}
            >
              Thêm
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
