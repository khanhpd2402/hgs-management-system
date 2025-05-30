import React, { useState, useMemo, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import './UploadLessonPlan.scss';
import { useSubjects, useAcademicYears, useSemestersByAcademicYear } from '../../../services/common/queries';
import { useTeacherBySubject } from '../../../services/subject/queries';
import { useCreateLessonPlan } from '../../../services/lessonPlan/mutations';
import { Input } from "@/components/ui/input";

const INITIAL_FORM = {
    academicYearId: '',
    subjectId: '',
    semesterId: '',
    teacherId: '',
    title: '',
    planContent: '',
    startDate: '',
    endDate: '',
};

const FORM_FIELDS = [
    { name: 'title', label: 'Tiêu đề', type: 'text', required: true },
    { name: 'planContent', label: 'Nội dung', type: 'textarea', rows: 4, required: true },
];

const UploadLessonPlan = () => {
    const [form, setForm] = useState(INITIAL_FORM);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const navigate = useNavigate();

    // Get teacherId from token
    const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
    const decodedTeacherId = useMemo(() => token && jwtDecode(token)?.teacherId, [token]);

    // Queries
    const { data: academicYears, isLoading: academicYearsLoading } = useAcademicYears();
    const { data: semesters, isLoading: semestersLoading } = useSemestersByAcademicYear(form.academicYearId);
    const { data: subjects, isLoading: subjectsLoading } = useSubjects();
    const { data: teacherData, isLoading: teachersLoading } = useTeacherBySubject(form.subjectId);
    const teachers = useMemo(
        () => (Array.isArray(teacherData) ? teacherData : teacherData ? [teacherData] : []),
        [teacherData]
    );

    const createLessonPlanMutation = useCreateLessonPlan();

    // Reset semesterId when academicYearId changes
    useEffect(() => {
        if (form.academicYearId) {
            setForm(prev => ({ ...prev, semesterId: '' }));
        }
    }, [form.academicYearId]);

    // Validate form field
    const validateField = (name, value) => {
        if (['academicYearId', 'semesterId', 'subjectId', 'teacherId'].includes(name)) {
            if (!value) return `Vui lòng chọn ${name === 'academicYearId' ? 'năm học' : name === 'semesterId' ? 'học kỳ' : name === 'subjectId' ? 'môn học' : 'giáo viên'}`;
        } else if (name === 'title') {
            if (!value.trim()) return 'Tiêu đề không được để trống';
            if (value.length > 100) return 'Tiêu đề không được vượt quá 100 ký tự';
        } else if (name === 'planContent') {
            if (!value.trim()) return 'Nội dung không được để trống';
            if (value.length > 1000) return 'Nội dung không được vượt quá 1000 ký tự';
        } else if (name === 'startDate') {
            if (!value) return 'Vui lòng chọn ngày bắt đầu';
        } else if (name === 'endDate') {
            if (!value) return 'Vui lòng chọn ngày kết thúc';
            if (form.startDate && new Date(value) <= new Date(form.startDate)) {
                return 'Ngày kết thúc phải sau ngày bắt đầu';
            }
        }
        return '';
    };

    // Validate entire form
    const validateForm = () => {
        for (const key of Object.keys(form)) {
            const error = validateField(key, form[key]);
            if (error) {
                toast.error(error);
                return false;
            }
        }
        return true;
    };

    // Handle input change
    const handleChange = (e) => {
        const { name, value } = e.target;
        const newValue = ['academicYearId', 'subjectId', 'semesterId', 'teacherId'].includes(name)
            ? value ? parseInt(value) : ''
            : value;

        setForm(prev => ({
            ...prev,
            [name]: newValue,
            ...(name === 'subjectId' ? { teacherId: '' } : {}),
            ...(name === 'academicYearId' ? { semesterId: '' } : {}),
        }));
    };

    // Handle form submit
    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!token || !decodedTeacherId) {
            toast.error('Vui lòng đăng nhập để tiếp tục!');
            return;
        }

        if (!validateForm()) {
            return;
        }

        setIsSubmitting(true);
        const payload = {
            teacherId: form.teacherId,
            subjectId: form.subjectId,
            semesterId: form.semesterId,
            title: form.title.trim(),
            planContent: form.planContent.trim(),
            startDate: new Date(form.startDate).toISOString(),
            endDate: new Date(form.endDate).toISOString(),
        };

        try {
            await createLessonPlanMutation.mutateAsync(payload);
            setForm(INITIAL_FORM);
            const toastId = toast.success('Tạo lịch phân công giáo án thành công!');
            setTimeout(() => {
                toast.dismiss(toastId);
                navigate(-1);
            }, 2100);
        } catch (error) {
            toast.error(error.response?.data?.message || 'Có lỗi xảy ra khi tạo giáo án');
        } finally {
            setIsSubmitting(false);
        }
    };

    // Reusable Select Component
    const FormSelect = ({ name, label, value, options, loading, placeholder, disabled }) => (
        <div className="form-field">
            <label>{label}:</label>
            <select
                name={name}
                value={value || ''}
                onChange={handleChange}
                disabled={loading || disabled}
                required
            >
                <option value="">{placeholder}</option>
                {options?.map(option => (
                    <option key={option.id} value={option.id}>
                        {option.name}
                    </option>
                ))}
            </select>
        </div>
    );

    // Reusable Input Component
    const FormInput = ({ name, label, type, value, rows }) => (
        <div className="form-field">
            <label>{label}:</label>
            {type === 'textarea' ? (
                <textarea
                    name={name}
                    value={value || ''}
                    onChange={handleChange}
                    required
                    rows={rows}
                    className="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
            ) : (
                <Input
                    type={type}
                    name={name}
                    value={value || ''}
                    onChange={handleChange}
                    required
                    className="w-full focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
            )}
        </div>
    );

    return (
        <div className="upload-lesson-plan">
            <h1>Phân công làm giáo án</h1>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <FormSelect
                        name="academicYearId"
                        label="Năm học"
                        value={form.academicYearId}
                        options={academicYears?.map(year => ({
                            id: year.academicYearID,
                            name: `${year.yearName} -- ${year.academicYearID}`,
                        }))}
                        loading={academicYearsLoading}
                        placeholder="Chọn năm học"
                    />
                    <FormSelect
                        name="semesterId"
                        label="Học kỳ"
                        value={form.semesterId}
                        options={semesters?.map(semester => ({
                            id: semester.semesterID,
                            name: semester.semesterName,
                        }))}
                        loading={semestersLoading}
                        disabled={!form.academicYearId}
                        placeholder="Chọn học kỳ"
                    />
                </div>

                <div className="form-group">
                    <div className="form-field">
                        <div style={{ display: 'flex', gap: '1rem' }}>
                            <div style={{ flex: 1 }}>
                                <FormSelect
                                    name="subjectId"
                                    label="Môn học"
                                    value={form.subjectId}
                                    options={subjects?.map(subject => ({
                                        id: subject.subjectID,
                                        name: subject.subjectName,
                                    }))}
                                    loading={subjectsLoading}
                                    placeholder="Chọn môn học"
                                />
                            </div>
                            <div style={{ flex: 1 }}>
                                <FormSelect
                                    name="teacherId"
                                    label="Chọn giáo viên làm giáo án"
                                    value={form.teacherId}
                                    options={teachers?.map(teacher => ({
                                        id: teacher.teacherId,
                                        name: teacher.teacherName,
                                    }))}
                                    loading={teachersLoading}
                                    disabled={!form.subjectId}
                                    placeholder="Chọn giáo viên"
                                />
                            </div>
                        </div>
                    </div>
                </div>

                <div className="form-group">
                    <FormInput
                        name="startDate"
                        label="Ngày bắt đầu"
                        type="datetime-local"
                        value={form.startDate}
                    />
                    <FormInput
                        name="endDate"
                        label="Ngày kết thúc"
                        type="datetime-local"
                        value={form.endDate}
                    />
                </div>


                {FORM_FIELDS.map(({ name, label, type, rows }) => (
                    <div className="form-group" key={name}>
                        <div className="form-field">
                            <label>{label}:</label>
                            {type === 'textarea' ? (
                                <textarea
                                    name={name}
                                    value={form[name]}
                                    onChange={handleChange}
                                    required
                                    rows={rows}
                                />
                            ) : (
                                <input
                                    type="text"
                                    name={name}
                                    value={form[name]}
                                    onChange={handleChange}
                                    required
                                />
                            )}
                        </div>
                    </div>
                ))}
                <div className="buttons-container">
                    <button
                        type="button"
                        className="btn-back"
                        onClick={() => navigate(-1)}
                        disabled={isSubmitting}
                    >
                        Trở lại danh sách
                    </button>
                    <button
                        type="submit"
                        disabled={isSubmitting}
                    >
                        {isSubmitting ? 'Đang xử lý...' : 'Phân công làm giáo án'}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default UploadLessonPlan;