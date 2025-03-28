import React, { useEffect, useState } from 'react';
import { jwtDecode } from 'jwt-decode';
import axios from 'axios';
import './UploadLessonPlan.scss';

const UploadLessonPlan = () => {
    const [subjects, setSubjects] = useState([]); // Danh sách môn học
    const [formData, setFormData] = useState({
        teacherId: "",
        subjectId: "",
        planContent: "",
        semesterId: "",
        title: "",
        attachmentUrl: "",
        grade: "",
        groupBy: ""
    });

    useEffect(() => {
        const token = localStorage.getItem('token'); // Lấy token từ localStorage
        if (token) {
            try {
                const decodedToken = jwtDecode(token);
                console.log("Token:", token);
                console.log("Decoded Token:", decodedToken);

                // Lấy teacherId từ token (giá trị `sub`)
                const teacherId = decodedToken.sub;

                // Cập nhật formData với teacherId
                setFormData(prevState => ({
                    ...prevState,
                    teacherId: teacherId
                }));
            } catch (error) {
                console.error("Lỗi khi giải mã token:", error);
            }
        } else {
            console.warn("Không tìm thấy token trong localStorage");
        }

        // Gọi API lấy danh sách môn học
        axios.get("https://localhost:8386/api/Subjects")
            .then(response => setSubjects(response.data))
            .catch(error => console.error("Lỗi khi lấy danh sách môn học:", error));
    }, []);

    // Xử lý thay đổi input
    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prevState => ({
            ...prevState,
            [name]: value
        }));
    };

    // Gửi form
    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post("https://localhost:8386/api/LessonPlan/upload", formData, {
                headers: { "Content-Type": "application/json" }
            });
            console.log("Kế hoạch giảng dạy đã tải lên thành công:", response.data);
            alert("Tải lên thành công!");
        } catch (error) {
            console.error("Lỗi khi tải lên kế hoạch giảng dạy:", error);
            alert("Tải lên thất bại!");
        }
    };

    return (
        <div className="upload-lesson-plan">
            <h1>Tải lên giảng dạy</h1>
            <form onSubmit={handleSubmit}>
                <div className="form-group">
                    <div className="form-field">
                        <label>Tiêu đề:</label>
                        <input
                            type="text"
                            name="title"
                            value={formData.title}
                            onChange={handleChange}
                            required
                        />
                    </div>
                </div>

                <div className="form-group">
                    <div className="form-field">
                        <label>Nội dung kế hoạch:</label>
                        <textarea
                            name="planContent"
                            value={formData.planContent}
                            onChange={handleChange}
                            required
                            rows="4"
                        />
                    </div>
                </div>

                <div className="form-group">
                    <div className="form-field">
                        <label>Link tài liệu đính kèm:</label>
                        <input
                            type="text"
                            name="attachmentUrl"
                            value={formData.attachmentUrl}
                            onChange={handleChange}
                            required
                        />
                    </div>
                </div>

                <div className="form-group">
                    <div className="form-field">
                        <label>Học kỳ:</label>
                        <select name="semesterId" value={formData.semesterId} onChange={handleChange} required>
                            <option value="">-- Chọn học kỳ --</option>
                            <option value="1">Học kỳ 1</option>
                            <option value="2">Học kỳ 2</option>
                        </select>
                    </div>
                    <div className="form-field">
                        <label>Khối:</label>
                        <select name="grade" onChange={handleChange} required>
                            <option value="">-- Tất cả --</option>
                            <option value="10">Khối 10</option>
                            <option value="11">Khối 11</option>
                            <option value="12">Khối 12</option>
                        </select>
                    </div>
                </div>

                <div className="form-group">
                    <div className="form-field">
                        <label>Nhóm theo:</label>
                        <select name="groupBy" onChange={handleChange}>
                            <option value="teacher">Giáo viên</option>
                            <option value="subject">Môn học</option>
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

                <div className="buttons-container">
                    <button type="submit">Tải lên</button>
                </div>
            </form>
        </div>
    );
}

export default UploadLessonPlan;
