import React, { useEffect, useState, useCallback } from 'react';
import { jwtDecode } from 'jwt-decode';
import axios from 'axios';
import './UploadLessonPlan.scss';

const API_URL = 'https://localhost:8386/api';
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
    attachmentUrl: '',
    startDate: '',
    endDate: ''
};

const UploadLessonPlan = () => {
    const [subjects, setSubjects] = useState([]);
    const [form, setForm] = useState(INITIAL_FORM);
    const [previewUrl, setPreviewUrl] = useState(null);
    const [previewType, setPreviewType] = useState(null);

    // ============= Helper ==================
    const extractGoogleId = (url) => url.match(/[-\w]{25,}/)?.[0];

    const getPreviewUrl = useCallback((url) => {
        if (!url) return null;

        if (url.includes('drive.google.com')) {
            const id = extractGoogleId(url);
            if (!id) return null;

            if (url.includes('/folders/')) {
                setPreviewType(PREVIEW_TYPES.FOLDER);
                return `https://drive.google.com/embeddedfolderview?id=${id}#list`;
            }

            setPreviewType(PREVIEW_TYPES.GOOGLE_FILE);
            return `https://drive.google.com/file/d/${id}/preview`;
        }

        const ext = url.split('.').pop().toLowerCase();
        switch (ext) {
            case 'pdf':
                setPreviewType(PREVIEW_TYPES.PDF);
                return url;
            case 'doc':
            case 'docx':
                setPreviewType(PREVIEW_TYPES.GOOGLE_DOCS);
                return `https://docs.google.com/viewer?url=${encodeURIComponent(url)}&embedded=true`;
            default:
                setPreviewType(PREVIEW_TYPES.UNKNOWN);
                return url;
        }
    }, []);

    // ============= Event Handlers ==================
    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    };

    const handlePreview = () => {
        if (!form.attachmentUrl) return alert('Vui lòng nhập URL tài liệu!');
        const url = getPreviewUrl(form.attachmentUrl);
        if (!url) return alert('URL không hợp lệ!');
        setPreviewUrl(url);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
        if (!token) return alert('Vui lòng đăng nhập để tiếp tục!');

        console.log("token", token)

        try {
            const decoded = jwtDecode(token);
            const teacherId = decoded?.teacherId;  // Changed to lowercase teacherId

            if (!teacherId) {
                throw new Error('Không tìm thấy thông tin giáo viên');
            }

            const payload = {
                teacherId,
                subjectId: parseInt(form.subjectId),
                semesterId: parseInt(form.semesterId),
                title: form.title,
                planContent: form.planContent,
                startDate: new Date(form.startDate).toISOString(),
                endDate: new Date(form.endDate).toISOString()
            };
            console.log("payload", payload)

            await axios.post(`${API_URL}/LessonPlan/create`, payload, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            alert('Tải lên thành công!');
            setForm(INITIAL_FORM);
        } catch (error) {
            const msg = error.response?.status === 401
                ? 'Phiên đăng nhập đã hết hạn!'
                : `Tải lên thất bại: ${error.response?.data || 'Lỗi hệ thống'}`;
            alert(msg);
        }
    };

    const fetchSubjects = async () => {
        try {
            const res = await axios.get(`${API_URL}/Subjects`);
            setSubjects(res.data);
        } catch (err) {
            console.error('Lỗi lấy danh sách môn học:', err);
        }
    };

    // ============= useEffect ==================
    useEffect(() => {
        fetchSubjects();
    }, []);

    return (
        <div className="upload-lesson-plan">
            <h1>Tải lên kế hoạch giảng dạy</h1>
            <form onSubmit={handleSubmit}>
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
                    { name: 'attachmentUrl', label: 'Link tài liệu đính kèm', type: 'text' }
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
                    <button type="submit">Tải lên</button>
                    <button type="button" className="btn-preview" onClick={handlePreview}>Xem trước</button>
                </div>
            </form>

            {previewUrl && (
                <div className="modal-overlay">
                    <div className="modal-content">
                        <h3>Xem trước tài liệu</h3>
                        <button onClick={() => setPreviewUrl(null)} className="close-preview">Đóng</button>
                        <iframe src={previewUrl} title={`${previewType} Preview`} />
                    </div>
                </div>
            )}
        </div>
    );
};

export default UploadLessonPlan;
