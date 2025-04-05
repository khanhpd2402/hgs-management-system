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

const INITIAL_FORM_STATE = {
    subjectId: '',
    planContent: '',
    semesterId: '',
    title: '',
    attachmentUrl: ''
};

const UploadLessonPlan = () => {
    const [subjects, setSubjects] = useState([]);
    const [formData, setFormData] = useState(INITIAL_FORM_STATE);
    const [previewUrl, setPreviewUrl] = useState(null);
    const [previewType, setPreviewType] = useState(null);

    const getGoogleDriveUrl = (url) => {
        const idMatch = url.match(/[-\w]{25,}/);
        if (!idMatch) return null;

        if (url.includes('/folders/')) {
            setPreviewType(PREVIEW_TYPES.FOLDER);
            return `https://drive.google.com/embeddedfolderview?id=${idMatch[0]}#list`;
        }

        setPreviewType(PREVIEW_TYPES.GOOGLE_FILE);
        return `https://drive.google.com/file/d/${idMatch[0]}/preview`;
    };

    const getFilePreviewUrl = (url) => {
        const extension = url.split('.').pop().toLowerCase();
        switch (extension) {
            case 'pdf':
                setPreviewType(PREVIEW_TYPES.PDF);
                return url;
            case 'doc':
            case 'docx':
                setPreviewType(PREVIEW_TYPES.GOOGLE_DOCS);
                return `https://docs.google.com/viewer?url=${encodeURIComponent(url)}&embedded=true`;
            case 'xls':
            case 'xlsx':
                setPreviewType(PREVIEW_TYPES.GOOGLE_DOCS);
                return `https://docs.google.com/spreadsheets/d/e/${url}/preview`;
            default:
                setPreviewType(PREVIEW_TYPES.UNKNOWN);
                return url;
        }
    };

    const getPreviewUrl = useCallback((url) => {
        if (!url) return null;
        return url.includes('drive.google.com')
            ? getGoogleDriveUrl(url)
            : getFilePreviewUrl(url);
    }, []);

    const handlePreview = () => {
        if (!formData.attachmentUrl) {
            alert('Vui lòng nhập URL tài liệu trước khi xem trước!');
            return;
        }
        const url = getPreviewUrl(formData.attachmentUrl);
        setPreviewUrl(url);
    };

    const fetchSubjects = async () => {
        try {
            const response = await axios.get(`${API_URL}/Subjects`);
            setSubjects(response.data);
        } catch (error) {
            console.error("Lỗi khi lấy danh sách môn học:", error);
        }
    };

    useEffect(() => {
        fetchSubjects();
    }, []);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const token = localStorage.getItem('token');

        if (!token) {
            alert('Vui lòng đăng nhập để thực hiện chức năng này!');
            return;
        }

        const requestData = {
            ...formData,
            subjectId: parseInt(formData.subjectId),
            semesterId: parseInt(formData.semesterId)
        };
        console.log("Request Data:", requestData);

        try {
            await axios.post(
                `${API_URL}/LessonPlan/upload`,
                requestData,
                {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }
                }
            );

            alert("Tải lên thành công!");
            setFormData(INITIAL_FORM_STATE);
        } catch (error) {
            const errorMessage = error.response?.status === 401
                ? "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!"
                : `Tải lên thất bại! ${error.response?.data || 'Vui lòng thử lại sau.'}`;
            alert(errorMessage);
        }
    };

    const renderPreviewIframe = () => (
        <iframe
            src={previewUrl}
            title={`${previewType} Preview`}
        />
    );

    return (
        <div className="upload-lesson-plan">
            <h1>Tải lên giảng dạy</h1>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <div className="form-field">
                        <label>Học kỳ:</label>
                        <select name="semesterId" value={formData.semesterId} onChange={handleChange} required>
                            <option value="">-- Chọn học kỳ --</option>
                            <option value="2">Học kỳ 1</option>
                            <option value="3">Học kỳ 2</option>
                        </select>
                    </div>
                    <div className="form-field">
                        <label>Môn học:</label>
                        <select name="subjectId" value={formData.subjectId} onChange={handleChange} required>
                            <option value="">-- Tất cả --</option>
                            {subjects.map(subject => (
                                <option key={subject.subjectId} value={subject.subjectId}>
                                    {subject.subjectName}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>

                {['title', 'planContent', 'attachmentUrl'].map(field => (
                    <div className="form-group" key={field}>
                        <div className="form-field">
                            <label>{field === 'title' ? 'Tiêu đề' :
                                field === 'planContent' ? 'Nội dung kế hoạch' :
                                    'Link tài liệu đính kèm'}:</label>
                            {field === 'planContent' ? (
                                <textarea
                                    name={field}
                                    value={formData[field]}
                                    onChange={handleChange}
                                    required
                                    rows="4"
                                />
                            ) : (
                                <input
                                    type="text"
                                    name={field}
                                    value={formData[field]}
                                    onChange={handleChange}
                                    required
                                />
                            )}
                        </div>
                    </div>
                ))}

                <div className="buttons-container">
                    <button type="submit">Tải lên</button>
                    <button type="button" className='btn-preview' onClick={handlePreview}>
                        Xem trước
                    </button>
                </div>
            </form>

            {previewUrl && (
                <div className="modal-overlay">
                    <div className="modal-content">
                        <h3>Xem trước tài liệu</h3>
                        <button onClick={() => setPreviewUrl(null)} className="close-preview">
                            Đóng
                        </button>
                        {renderPreviewIframe()}
                    </div>
                </div>
            )}
        </div>
    );
};

export default UploadLessonPlan;