import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './LessonPlanList.scss';
import { Table, Space, Tag, Select, Form, Card, Modal, Button, Descriptions } from 'antd';
import { Link } from 'react-router-dom';
import { EyeOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { jwtDecode } from 'jwt-decode';
const { Option } = Select;
const TeacherLessonPlan = () => {
    const [lessonPlans, setLessonPlans] = useState([]);
    const [loading, setLoading] = useState(true);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [selectedStatus, setSelectedStatus] = useState('All');
    const [searchTerm, setSearchTerm] = useState('');
    const pageSize = 10;
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [selectedRequest, setSelectedRequest] = useState(null);

    const fetchLessonPlans = async (page) => {
        try {
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            const decoded = jwtDecode(token);
            const teacherId = decoded?.teacherId;

            const url = `https://localhost:8386/api/LessonPlan/teacher/${teacherId}?pageNumber=${page}&pageSize=${pageSize}`;

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
            case 'Đang chờ':
                return 'status-processing';
            case 'Đã duyệt':
                return 'status-approved';
            case 'Từ chối':
                return 'status-rejected';
            default:
                return '';
        }
    };

    const getStatusText = (status) => {
        switch (status) {
            case 'Đang chờ':
                return 'Đang xử lý';
            case 'Đã duyệt':
                return 'Đã duyệt';
            case 'Từ chối':
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

    const showDetailModal = (record) => {
        setSelectedRequest(record);
        setIsModalVisible(true);
    };

    const DetailModal = () => (
        <Modal
            title="Chi tiết kế hoạch giáo án"
            open={isModalVisible}
            onCancel={() => setIsModalVisible(false)}
            width={1000}
            style={{
                top: 20,
                maxHeight: 'calc(100vh - 40px)',
                overflow: 'auto'
            }}
            footer={[
                <Button key="close" onClick={() => setIsModalVisible(false)}>
                    Đóng
                </Button>
            ]}
        >
            {selectedRequest && (
                <div className="lesson-plan-detail">
                    <div className="detail-section">
                        <h2>Thông tin kế hoạch</h2>
                        <Card bordered={false}>
                            <Descriptions bordered column={2}>
                                <Descriptions.Item label="ID Kế hoạch" span={2}>
                                    {selectedRequest.planId}
                                </Descriptions.Item>

                                <Descriptions.Item label="Môn học" span={2}>
                                    {selectedRequest.subjectName}
                                </Descriptions.Item>
                                <Descriptions.Item label="Nội dung" span={2}>
                                    {selectedRequest.planContent}
                                </Descriptions.Item>
                                <Descriptions.Item label="Trạng thái">
                                    <Tag className={`status-${selectedRequest.status.toLowerCase().replace(' ', '-')}`}>
                                        {getStatusText(selectedRequest.status)}
                                    </Tag>
                                </Descriptions.Item>
                                <Descriptions.Item label="Ngày nộp">
                                    {formatDate(selectedRequest.submittedDate)}
                                </Descriptions.Item>
                                <Descriptions.Item label="Người duyệt">
                                    {selectedRequest.reviewerName}
                                </Descriptions.Item>
                                <Descriptions.Item label="Ngày duyệt">
                                    {formatDate(selectedRequest.reviewedDate)}
                                </Descriptions.Item>
                                <Descriptions.Item label="Phản hồi">
                                    {selectedRequest.feedback || 'Chưa có phản hồi'}
                                </Descriptions.Item>
                            </Descriptions>
                        </Card>
                    </div>

                    {selectedRequest.attachmentUrl && (
                        <div className="attachment-section">
                            <h2>File đính kèm</h2>
                            <div className="file-preview">
                                <iframe
                                    src={selectedRequest.attachmentUrl}
                                    title="File đính kèm"
                                    width="100%"
                                    height="600px"
                                    style={{
                                        border: '1px solid #d9d9d9',
                                        borderRadius: '4px'
                                    }}
                                />
                            </div>
                        </div>
                    )}
                </div>
            )}
        </Modal>
    );

    const columns = [
        {
            title: 'ID',
            dataIndex: 'planId',
            key: 'planId',
        },
        {
            title: 'Môn học',
            dataIndex: 'subjectName',
            key: 'subjectName',
        },
        {
            title: 'Nội dung',
            dataIndex: 'planContent',
            key: 'planContent',
        },
        {
            title: 'Trạng thái',
            dataIndex: 'status',
            key: 'status',
            render: (status) => (
                <span className={`status-${status.toLowerCase()} ${getStatusClass(status)}`}>
                    {getStatusText(status)}
                </span>
            ),
        },
        {
            title: 'Ngày nộp',
            dataIndex: 'submittedDate',
            key: 'submittedDate',
            render: (date) => formatDate(date),
        },
        {
            title: 'Người duyệt',
            dataIndex: 'reviewerName',
            key: 'reviewerName',
        },
        {
            title: 'Ngày duyệt',
            dataIndex: 'reviewedDate',
            key: 'reviewedDate',
            render: (date) => formatDate(date),
        },
        {
            title: 'Phản hồi',
            dataIndex: 'feedback',
            key: 'feedback',
        },
        {
            title: 'Hành động',
            key: 'action',
            render: (_, record) => (
                <Space>
                    <Button
                        type="primary"
                        icon={<EyeOutlined />}
                        onClick={() => showDetailModal(record)}
                    >
                        Xem chi tiết
                    </Button>
                    <Link to={`/system/lesson-plan/add-document/${record.planId}`}>
                        <Button>Thêm tài liệu</Button>
                    </Link>
                </Space>
            ),
        },
    ];

    if (loading) {
        return <div className="loading">Đang tải dữ liệu...</div>;
    }

    return (
        <div className="lesson-plan-list">

            <h2>Danh sách kế hoạch giáo án</h2>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                <h2></h2>
                <Link to="/teacher/lesson-plan/create">
                    <Button type="primary">
                        Tạo kế hoạch mới
                    </Button>
                </Link>
            </div>
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
                    <select>
                        <option value="All">Tất cả</option>
                        <option value="Đang chờ">Đang xử lý</option>
                        <option value="Đã duyệt">Đã duyệt</option>
                        <option value="Từ chối">Từ chối</option>
                    </select>
                </div>
            </div>

            <div className="table-container">
                <Table
                    columns={columns}
                    dataSource={filteredLessonPlans}
                    loading={loading}
                    rowKey="planId"
                    pagination={{
                        pageSize: pageSize,
                        total: totalPages * pageSize,
                        current: currentPage,
                        onChange: (page) => {
                            setCurrentPage(page);
                        },
                    }}
                />
            </div>

            <DetailModal />
        </div>
    );
};

export default TeacherLessonPlan
