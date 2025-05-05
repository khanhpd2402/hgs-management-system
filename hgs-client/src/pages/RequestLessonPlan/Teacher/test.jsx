import React, { useState, useMemo, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import './UploadLessonPlan.scss';
import { useSubjects, useAcademicYears, useSemestersByAcademicYear } from '../../../services/common/queries';
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
    { name: 'title', label: 'Tiêu đề:', type: 'text' },
    { name: 'planContent', label: 'Nội dung:', type: 'textarea', rows: 4 },
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

    // Direct API call for teachers by subject
    const [teachersBySubject, setTeachersBySubject] = useState(null);
    const [teachersLoading, setTeachersLoading] = useState(false);

    const fetchTeachersBySubject = async (subjectId) => {
        if (!subjectId) return;

        setTeachersLoading(true);
        try {
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            const response = await fetch(`https://localhost:8386/api/TeacherSubject/${subjectId}`, {
                headers: {
                    'accept': '*/*',
                    'Authorization': `Bearer ${token}`
                }
            });
            const data = await response.json();
            setTeachersBySubject([data]); // Wrap in array since the API returns a single object
        } catch (error) {
            console.error('Error fetching teachers:', error);
            toast.error('Không thể tải danh sách giáo viên');
        } finally {
            setTeachersLoading(false);
        }
    };

    // Reset semesterId when academicYearId changes
    useEffect(() => {
        if (form.academicYearId) {
            setForm(prev => ({ ...prev, semesterId: '' }));
        }
    }, [form.academicYearId]);

    // Event Handlers
    const handleChange = (e) => {
        const { name, value } = e.target;
        if (name === 'subjectId') {
            const numericValue = parseInt(value);
            setForm(prev => ({
                ...prev,
                [name]: numericValue,
                teacherId: '' // Reset teacherId when subject changes
            }));
            fetchTeachersBySubject(numericValue);
        } else if (name === 'academicYearId') {
            const numericValue = parseInt(value);
            setForm(prev => ({
                ...prev,
                [name]: numericValue,
                semesterId: '' // Reset semesterId when academic year changes
            }));
        } else {
            setForm(prev => ({ ...prev, [name]: value }));
        }
    };

    const createLessonPlanMutation = useCreateLessonPlan();

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!token || !decodedTeacherId) {
            toast.error('Vui lòng đăng nhập để tiếp tục!');
            return;
        }

        // Validate dates
        const startDate = new Date(form.startDate);
        const endDate = new Date(form.endDate);
        if (startDate >= endDate) {
            toast.error('Ngày kết thúc phải sau ngày bắt đầu');
            return;
        }

        const payload = {
            teacherId: parseInt(form.teacherId),
            subjectId: parseInt(form.subjectId),
            semesterId: parseInt(form.semesterId),
            title: form.title.trim(),
            planContent: form.planContent.trim(),
            startDate: startDate.toISOString(),
            endDate: endDate.toISOString(),
        };

        try {
            setIsSubmitting(true);
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
    const RenderSelect = ({ name, label, value, options, loading, placeholder, disabled }) => (
        <div className="form-field">
            <label>{label}</label>
            <select
                name={name}
                value={value || ''}
                onChange={handleChange}
                required
                disabled={loading || disabled}
                className="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50"
            >
                <option value="">{placeholder}</option>
                {options?.map((option) => (
                    <option key={option.id} value={option.id}>
                        {option.name}
                    </option>
                ))}
            </select>
            {disabled && !loading && name !== 'academicYearId' && (
                <span className="text-sm text-gray-500">
                    {name === 'semesterId' ? 'Vui lòng chọn năm học trước' : 'Vui lòng chọn môn học trước'}
                </span>
            )}
        </div>
    );

    // Reusable Input Component
    const FormInput = ({ name, label, type, value, rows }) => (
        <div className="form-field">
            <label>{label}</label>
            {type === 'textarea' ? (
                <textarea
                    name={name}
                    value={value || ''}
                    onChange={handleChange}
                    rows={rows}
                    className="w-full p-2 border rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                />
            ) : (
                <Input
                    type={type}
                    name={name}
                    value={value || ''}
                    onChange={handleChange}
                    placeholder={type === 'datetime-local' ? 'mm/dd/yyyy --:-- --' : undefined}
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
                    <RenderSelect
                        name="academicYearId"
                        label="Chọn năm học:"
                        value={form.academicYearId}
                        options={academicYears?.map(year => ({
                            id: year.academicYearID,
                            name: `${year.yearName} -- ${year.academicYearID}`,
                        }))}
                        loading={academicYearsLoading}
                        placeholder="Chọn năm học"
                    />
                    <RenderSelect
                        name="semesterId"
                        label="Chọn học kỳ:"
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
                                <RenderSelect
                                    name="subjectId"
                                    label="Chọn môn học:"
                                    value={form.subjectId}
                                    options={subjects?.map(subject => ({
                                        id: subject.subjectID,
                                        name: `${subject.subjectName} -- ${subject.subjectID}`,
                                    }))}
                                    loading={subjectsLoading}
                                    placeholder="Chọn môn học"
                                />
                            </div>
                            <div style={{ flex: 1 }}>
                                <RenderSelect
                                    name="teacherId"
                                    label="Chọn giáo viên:"
                                    value={form.teacherId}
                                    options={teachersBySubject?.map(teacher => ({
                                        id: teacher.teacherId,
                                        name: `${teacher.teacherName} -- ${teacher.teacherId}`,
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
                        label="Ngày bắt đầu:"
                        type="datetime-local"
                        value={form.startDate}
                    />
                    <FormInput
                        name="endDate"
                        label="Ngày kết thúc:"
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
                    <button type="submit" disabled={isSubmitting}>
                        {isSubmitting ? 'Đang xử lý...' : 'Phân công làm giáo án'}
                    </button>
                </div>
            </form>
        </div>
    );
};

export default UploadLessonPlan;