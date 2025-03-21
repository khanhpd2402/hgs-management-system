import { useState } from "react";
import { useSubjects } from "@/services/common/queries";
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

export default function AddGradeBatch({ semester }) {
  const subjectsQuery = useSubjects();
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

  // Initialize subjects from the query data
  useState(() => {
    if (subjectsQuery.data) {
      const subjectsState = {};
      subjectsQuery.data.forEach((subject) => {
        subjectsState[subject.subjectId] = false;
      });
      setFormData((prev) => ({
        ...prev,
        subjects: subjectsState,
      }));
    }
  }, [subjectsQuery.data]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));

    // Clear error when user types
    if (errors[name]) {
      setErrors((prev) => ({
        ...prev,
        [name]: null,
      }));
    }
  };

  const handleCheckboxChange = (category, name) => {
    setFormData((prev) => ({
      ...prev,
      [category]: {
        ...prev[category],
        [name]: !prev[category][name],
      },
    }));

    // Clear category error when user selects an option
    if (errors[category]) {
      setErrors((prev) => ({
        ...prev,
        [category]: null,
      }));
    }
  };

  const handleDateChange = (name, date) => {
    setFormData((prev) => ({
      ...prev,
      [name]: date,
    }));

    // Clear error when user selects a date
    if (errors[name]) {
      setErrors((prev) => ({
        ...prev,
        [name]: null,
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};

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
    if (validateForm()) {
      // Transform the form data to match the required API structure
      const selectedSubjectIds = Object.entries(formData.subjects)
        .filter(([_, isSelected]) => isSelected)
        .map(([subjectId]) => parseInt(subjectId));

      const gradeTypes = [];

      // Add grade types based on selections
      if (formData.gradeColumns.regular) {
        // For regular grades, add the count of columns
        for (let i = 1; i <= formData.regularColumnCount; i++) {
          gradeTypes.push(`DDGTX${i}`);
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
        gradeBatch: {
          batchName: formData.name,
          semesterId: semester, // You might need to get this from somewhere
          startDate: formatDate(formData.startDate),
          endDate: formatDate(formData.endDate),
          isActive: !formData.isLocked,
        },
        subjectIds: selectedSubjectIds,
        assessmentTypes: gradeTypes,
      };

      console.log("Form data submitted:", payload);
      // Here you would typically call an API to save the data
      // Example: createGradeBatch(payload);
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
    <div className="p-4">
      <Button onClick={openModal}>Thêm đợt nhập điểm</Button>

      <Dialog open={isModalOpen} onOpenChange={setIsModalOpen}>
        <DialogContent className="max-h-[90vh] overflow-y-auto sm:max-w-[600px]">
          <DialogHeader>
            <DialogTitle className="text-xl font-bold">
              Thêm mới đợt nhập điểm
            </DialogTitle>
            <Button
              variant="ghost"
              className="ring-offset-background absolute top-4 right-4 rounded-sm opacity-70 transition-opacity hover:opacity-100"
              onClick={() => setIsModalOpen(false)}
            ></Button>
          </DialogHeader>

          <div className="grid gap-4 py-4">
            {/* Tên đợt */}
            <div className="grid grid-cols-4 items-center gap-4">
              <label htmlFor="name" className="text-right font-medium">
                Tên đợt
              </label>
              <div className="col-span-3">
                <Input
                  id="name"
                  name="name"
                  value={formData.name}
                  onChange={handleInputChange}
                  className={errors.name ? "border-red-500" : ""}
                />
                {errors.name && (
                  <p className="mt-1 text-sm text-red-500">{errors.name}</p>
                )}
              </div>
            </div>

            {/* Thời gian */}
            <div className="grid grid-cols-4 items-center gap-4">
              <label className="text-right font-medium">Từ ngày</label>
              <div className="col-span-3">
                <DatePicker
                  value={formData.startDate}
                  onSelect={(date) => handleDateChange("startDate", date)}
                  disabled={false}
                />
                {errors.startDate && (
                  <p className="mt-1 text-sm text-red-500">
                    {errors.startDate}
                  </p>
                )}
              </div>
            </div>

            <div className="grid grid-cols-4 items-center gap-4">
              <label className="text-right font-medium">Đến ngày</label>
              <div className="col-span-3">
                <DatePicker
                  value={formData.endDate}
                  onSelect={(date) => handleDateChange("endDate", date)}
                  disabled={false}
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
                <div className="flex items-center space-x-2">
                  <Checkbox
                    id="isLocked"
                    checked={formData.isLocked}
                    onCheckedChange={(checked) =>
                      setFormData((prev) => ({ ...prev, isLocked: checked }))
                    }
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
              <label className="text-right font-medium">
                Các cột điểm của đợt
              </label>
              <div className="col-span-3">
                <div className="grid grid-cols-3 gap-4">
                  {Object.entries(gradeColumnLabels).map(([key, label]) => (
                    <div key={key} className="flex items-center space-x-2">
                      <Checkbox
                        id={`grade-${key}`}
                        checked={formData.gradeColumns[key]}
                        onCheckedChange={() =>
                          handleCheckboxChange("gradeColumns", key)
                        }
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
                  <div className="mt-3 pl-6">
                    <label className="mb-2 block text-sm font-medium">
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
                            className="h-4 w-4"
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
              <label className="text-right font-medium">Môn học áp dụng</label>
              <div className="col-span-3">
                <div className="grid grid-cols-3 gap-2">
                  {subjectsQuery.isLoading ? (
                    <p>Đang tải danh sách môn học...</p>
                  ) : subjectsQuery.isError ? (
                    <p>Lỗi khi tải danh sách môn học</p>
                  ) : (
                    subjectsQuery.data?.map((subject) => (
                      <div
                        key={subject.subjectId}
                        className="flex items-center space-x-2"
                      >
                        <Checkbox
                          id={`subject-${subject.subjectId}`}
                          checked={
                            formData.subjects[subject.subjectId] || false
                          }
                          onCheckedChange={() =>
                            handleCheckboxChange("subjects", subject.subjectId)
                          }
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

          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsModalOpen(false)}
              className="mr-2"
            >
              Hủy bỏ
            </Button>
            <Button onClick={handleSubmit}>Lưu</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
