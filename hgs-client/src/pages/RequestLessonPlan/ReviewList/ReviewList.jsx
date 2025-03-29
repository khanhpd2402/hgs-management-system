import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import './ReviewList.scss';

const ReviewList = () => {
    const [lessonPlans, setLessonPlans] = useState([]);
    const [loading, setLoading] = useState(true);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const pageSize = 10;
    const navigate = useNavigate();

    const fetchProcessingPlans = async (page) => {
        try {
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            const response = await axios.get(
                `https://localhost:8386/api/LessonPlan/filter-by-status?status=Processing&pageNumber=${page}&pageSize=${pageSize}`,
                {
                    headers: { 'Authorization': `Bearer ${token}` }
                }
            );
            setLessonPlans(response.data.lessonPlans);
            setTotalPages(Math.ceil(response.data.totalCount / pageSize));
            setLoading(false);
        } catch (error) {
            console.error('Lỗi khi tải danh sách:', error);
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchProcessingPlans(currentPage);
    }, [currentPage]);

    const handleReviewClick = (planId) => {
        navigate(`/teacher/review-detail/${planId}`);
    };

    const formatDate = (dateString) => {
        if (!dateString) return 'N/A';
        return new Date(dateString).toLocaleDateString('vi-VN', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    };

    if (loading) {
        return <div className="loading">Đang tải dữ liệu...</div>;
    }

    return (
        <div className="review-list">
            <h2>Danh sách kế hoạch cần phê duyệt</h2>

            <div className="table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Giáo viên</th>
                            <th>Môn học</th>
                            <th>Nội dung</th>
                            <th>Ngày nộp</th>
                            <th>Thao tác</th>
                        </tr>
                    </thead>
                    <tbody>
                        {lessonPlans.map(plan => (
                            <tr key={plan.planId}>
                                <td>{plan.planId}</td>
                                <td>{plan.teacherName}</td>
                                <td>{plan.subjectName}</td>
                                <td className="content-cell">{plan.planContent}</td>
                                <td>{formatDate(plan.submittedDate)}</td>
                                <td>
                                    <button
                                        className="review-button"
                                        onClick={() => handleReviewClick(plan.planId)}
                                    >
                                        Phê duyệt
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            <div className="pagination">
                <button
                    onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
                    disabled={currentPage === 1}
                >
                    Trước
                </button>
                <span>Trang {currentPage} / {totalPages}</span>
                <button
                    onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
                    disabled={currentPage === totalPages}
                >
                    Sau
                </button>
            </div>
        </div>
    );
};

export default ReviewList;