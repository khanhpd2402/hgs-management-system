import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import './ReviewDetail.scss';

const ReviewDetail = () => {
    const { planId } = useParams();
    const navigate = useNavigate();
    const [plan, setPlan] = useState(null);
    const [loading, setLoading] = useState(true);
    const [feedback, setFeedback] = useState('');
    const [status, setStatus] = useState('');

    useEffect(() => {
        fetchPlanDetail();
    }, [planId]);

    const fetchPlanDetail = async () => {
        try {
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            const response = await axios.get(
                `https://localhost:8386/api/LessonPlan/${planId}`,
                {
                    headers: { 'Authorization': `Bearer ${token}` }
                }
            );
            setPlan(response.data);
            setLoading(false);
        } catch (error) {
            console.error('Lỗi khi tải chi tiết:', error);
            setLoading(false);
        }
    };

    const handleSubmit = async () => {
        try {
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            await axios.post(
                'https://localhost:8386/api/LessonPlan/review',
                {
                    planId: parseInt(planId),
                    status: status,
                    feedback: feedback
                },
                {
                    headers: { 'Authorization': `Bearer ${token}` }
                }
            );
            alert('Phê duyệt thành công!');
            navigate('/review-list');
        } catch (error) {
            console.error('Lỗi khi phê duyệt:', error);
            alert('Có lỗi xảy ra khi phê duyệt');
        }
    };

    if (loading) {
        return <div className="loading">Đang tải dữ liệu...</div>;
    }

    return (
        <div className="review-detail">
            <h2>Chi tiết kế hoạch giảng dạy</h2>

            <div className="plan-info">
                <div className="info-group">
                    <label>ID:</label>
                    <span>{plan.planId}</span>
                </div>
                <div className="info-group">
                    <label>Giáo viên:</label>
                    <span>{plan.teacherName}</span>
                </div>
                <div className="info-group">
                    <label>Môn học:</label>
                    <span>{plan.subjectName}</span>
                </div>
                <div className="info-group">
                    <label>Nội dung:</label>
                    <p>{plan.planContent}</p>
                </div>
            </div>

            <div className="review-form">
                <div className="form-group">
                    <label>Trạng thái:</label>
                    <select
                        value={status}
                        onChange={(e) => setStatus(e.target.value)}
                        required
                    >
                        <option value="">Chọn trạng thái</option>
                        <option value="Approved">Phê duyệt</option>
                        <option value="Rejected">Từ chối</option>
                    </select>
                </div>

                <div className="form-group">
                    <label>Phản hồi:</label>
                    <textarea
                        value={feedback}
                        onChange={(e) => setFeedback(e.target.value)}
                        required
                    />
                </div>

                <div className="button-group">
                    <button onClick={() => navigate(-1)}>Quay lại</button>
                    <button
                        onClick={handleSubmit}
                        disabled={!status || !feedback}
                        className="submit-button"
                    >
                        Xác nhận
                    </button>
                </div>
            </div>
        </div>
    );
};

export default ReviewDetail;