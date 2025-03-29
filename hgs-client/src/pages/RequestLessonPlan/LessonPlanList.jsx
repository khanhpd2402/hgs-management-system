import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './LessonPlanList.scss';

const LessonPlanList = () => {
    const [lessonPlans, setLessonPlans] = useState([]);
    const [loading, setLoading] = useState(true);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [selectedStatus, setSelectedStatus] = useState('All');
    const [searchTerm, setSearchTerm] = useState('');
    const pageSize = 10;

    const fetchLessonPlans = async (page, status) => {
        try {
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            const url = status === 'All'
                ? `https://localhost:8386/api/LessonPlan/all?pageNumber=${page}&pageSize=${pageSize}`
                : `https://localhost:8386/api/LessonPlan/filter-by-status?status=${status}&pageNumber=${page}&pageSize=${pageSize}`;

            const response = await axios.get(url, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            setLessonPlans(response.data.lessonPlans);
            setTotalPages(Math.ceil(response.data.totalCount / pageSize));
            setLoading(false);
        } catch (error) {
            console.error('Lỗi khi tải danh sách:', error);
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchLessonPlans(currentPage, selectedStatus);
    }, [currentPage, selectedStatus]);

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

    const getStatusClass = (status) => {
        switch (status) {
            case 'Processing':
                return 'status-processing';
            case 'Approved':
                return 'status-approved';
            case 'Rejected':
                return 'status-rejected';
            default:
                return '';
        }
    };

    const getStatusText = (status) => {
        switch (status) {
            case 'Processing':
                return 'Đang xử lý';
            case 'Approved':
                return 'Đã duyệt';
            case 'Rejected':
                return 'Từ chối';
            default:
                return status;
        }
    };

    const filteredLessonPlans = lessonPlans.filter(plan => {
        const searchStr = searchTerm.toLowerCase();
        return (
            plan.planId.toString().includes(searchStr) ||
            plan.teacherName?.toLowerCase().includes(searchStr) ||
            plan.subjectName?.toLowerCase().includes(searchStr) ||
            plan.planContent?.toLowerCase().includes(searchStr) ||
            plan.reviewerName?.toLowerCase().includes(searchStr) ||
            plan.feedback?.toLowerCase().includes(searchStr)
        );
    });

    if (loading) {
        return <div className="loading">Đang tải dữ liệu...</div>;
    }

    return (
        <div className="lesson-plan-list">
            <h2>Danh sách kế hoạch giảng dạy</h2>

            <div className="filters-section">
                <div className="search-container">
                    <input
                        type="text"
                        placeholder="Tìm kiếm..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="search-input"
                    />
                </div>
                <div className="filter-container">
                    <select
                        value={selectedStatus}
                        onChange={(e) => {
                            setSelectedStatus(e.target.value);
                            setCurrentPage(1);
                        }}
                    >
                        <option value="All">Tất cả</option>
                        <option value="Processing">Đang xử lý</option>
                        <option value="Approved">Đã duyệt</option>
                        <option value="Rejected">Từ chối</option>
                    </select>
                </div>
            </div>

            <div className="table-container">
                <table>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Giáo viên</th>
                            <th>Môn học</th>
                            <th>Nội dung</th>
                            <th>Trạng thái</th>
                            <th>Ngày nộp</th>
                            <th>Người duyệt</th>
                            <th>Ngày duyệt</th>
                            <th>Phản hồi</th>
                        </tr>
                    </thead>
                    <tbody>
                        {filteredLessonPlans.map(plan => (
                            <tr key={plan.planId}>
                                <td>{plan.planId}</td>
                                <td>{plan.teacherName}</td>
                                <td>{plan.subjectName}</td>
                                <td className="content-cell">{plan.planContent}</td>
                                <td>
                                    <span className={`status-badge ${getStatusClass(plan.status)}`}>
                                        {getStatusText(plan.status)}
                                    </span>
                                </td>
                                <td>{formatDate(plan.submittedDate)}</td>
                                <td>{plan.reviewerName}</td>
                                <td>{formatDate(plan.reviewedDate)}</td>
                                <td>{plan.feedback || 'Chưa có phản hồi'}</td>
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

export default LessonPlanList;