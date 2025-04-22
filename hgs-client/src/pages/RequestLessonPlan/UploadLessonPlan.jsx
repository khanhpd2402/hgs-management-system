import React, { useEffect, useState, useCallback } from 'react';
import { jwtDecode } from 'jwt-decode';
import './UploadLessonPlan.scss';
import { useSubjectByTeacher } from '../../services/subject/queries';
import { useSemestersByAcademicYear } from '../../services/common/queries';
import { useCreateLessonPlan } from '../../services/lessonPlan/mutations';
import toast from "react-hot-toast";

const PREVIEW_TYPES = {
    FOLDER: 'folder',
    PDF: 'pdf',
    GOOGLE_DOCS: 'google-docs',
    GOOGLE_FILE: 'google-file',
    UNKNOWN: 'unknown'
};

const INITIAL_FORM = {
    subjectId: '',
    semesterId: '',
    title: '',
    planContent: '',
    startDate: '',
    endDate: ''
};

const UploadLessonPlan = () => {
    const [form, setForm] = useState(INITIAL_FORM);


    // Get teacherId from token
    const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
    const teacherId = token ? jwtDecode(token)?.teacherId : null;

    // Pass academicYearId 1 explicitly
    const { data: semesters, isLoading: semestersLoading } = useSemestersByAcademicYear(1);
    const { subjects, isLoading: subjectsLoading } = useSubjectByTeacher(teacherId);




    // ============= Event Handlers ==================
    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    };


    const createLessonPlanMutation = useCreateLessonPlan();

    const handleSubmit = async (e) => {
        e.preventDefault();
        const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
        if (!token) {
            createLessonPlanMutation.onError({ response: { data: 'Vui lòng đăng nhập để tiếp tục!' } });
            return;
        }

        const decoded = jwtDecode(token);
        const teacherId = decoded?.teacherId;

        if (!teacherId) {
            createLessonPlanMutation.onError({ response: { data: 'Không tìm thấy thông tin giáo viên' } });
            return;
        }

        // Validate dates
        const startDateTime = new Date(form.startDate);
        const endDateTime = new Date(form.endDate);

        if (startDateTime >= endDateTime) {

            toast.message("Ngày kết thúc phải sau ngày bắt đầu");
            return;
        }

        const payload = {
            teacherId: parseInt(teacherId),
            subjectId: parseInt(form.subjectId),
            semesterId: parseInt(form.semesterId),
            title: form.title.trim(),
            planContent: form.planContent.trim(),
            startDate: startDateTime.toISOString(),
            endDate: endDateTime.toISOString()
        };

        try {
            await createLessonPlanMutation.mutateAsync(payload);
            setForm(INITIAL_FORM);
            toast.message("Tạo kế hoạch giảng dạy thành công!");

        } catch (error) {
            console.error('Error creating lesson plan:', error);
            const errorMessage = error.response?.data?.message || 'Có lỗi xảy ra khi tạo kế hoạch giảng dạy';
            toast.error(errorMessage);

        }
    };



    return (
        <div className="upload-lesson-plan">
            <h1>Tải lên kế hoạch giáo án</h1>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <div className="form-field">
                        <label>Môn học:</label>
                        <select
                            name="subjectId"
                            value={form.subjectId}
                            onChange={handleChange}
                            required
                            disabled={subjectsLoading}
                        >
                            <option value="">Chọn môn học</option>
                            {subjects.map(subject => (
                                <option key={subject.subjectId} value={subject.subjectId}>
                                    {subject.subjectName}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="form-field">
                        <label>Học kỳ:</label>
                        <select
                            name="semesterId"
                            value={form.semesterId}
                            onChange={handleChange}
                            required
                            disabled={semestersLoading}
                        >
                            <option value="">Chọn học kỳ</option>
                            {semesters?.map(semester => (
                                <option key={semester.semesterID} value={semester.semesterID}>
                                    {semester.semesterName}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>

                <div className="form-group">
                    <div className="form-field">
                        <label>Ngày bắt đầu:</label>
                        <input type="datetime-local" name="startDate" value={form.startDate} onChange={handleChange} required />
                    </div>
                    <div className="form-field">
                        <label>Ngày kết thúc:</label>
                        <input type="datetime-local" name="endDate" value={form.endDate} onChange={handleChange} required />
                    </div>
                </div>

                {[
                    { name: 'title', label: 'Tiêu đề', type: 'text' },
                    { name: 'planContent', label: 'Nội dung kế hoạch', type: 'textarea' },
                ].map(({ name, label, type }) => (
                    <div className="form-group" key={name}>
                        <div className="form-field">
                            <label>{label}:</label>
                            {type === 'textarea' ? (
                                <textarea name={name} value={form[name]} onChange={handleChange} required rows="4" />
                            ) : (
                                <input type="text" name={name} value={form[name]} onChange={handleChange} required />
                            )}
                        </div>
                    </div>
                ))}

                <div className="buttons-container">
                    <button type="button" className="btn-back" onClick={() => window.history.back()}>Trở lại danh sách</button>
                    <div className="right-buttons">
                        <button type="submit">Tải lên</button>
                    </div>
                </div>

            </form>


        </div>
    );
};

export default UploadLessonPlan;
