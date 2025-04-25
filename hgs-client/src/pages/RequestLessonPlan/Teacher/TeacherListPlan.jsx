import React, { useState, useEffect } from 'react';
import axios from 'axios';
import '../System/LessonPlanList.scss';
import { Table, Space, Tag, Select, Form, Card, Modal, Button, Descriptions } from 'antd';
import { Link } from 'react-router-dom';
import { EyeOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { jwtDecode } from 'jwt-decode';
import { useLessonPlanByTeacher } from 'c:/Users/trung/Downloads/hgs-client/src/services/lessonPlan/queries';

const TeacherListPlan = () => {
    const [currentPage, setCurrentPage] = useState(1);
    const [selectedStatus, setSelectedStatus] = useState('All');
    const [searchTerm, setSearchTerm] = useState('');
    const pageSize = 10;
    const [isModalVisible, setIsModalVisible] = useState(false);
    const [selectedRequest, setSelectedRequest] = useState(null);

    const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
    const decoded = jwtDecode(token);
    const teacherId = decoded?.teacherId;

    const { data, isLoading } = useLessonPlanByTeacher(teacherId, currentPage, pageSize);

    const filteredLessonPlans = data?.lessonPlans?.filter(plan => {
        const searchStr = searchTerm.toLowerCase();
        const matchesSearch = (
            plan.planId.toString().includes(searchStr) ||
            plan.teacherName?.toLowerCase().includes(searchStr) ||
            plan.subjectName?.toLowerCase().includes(searchStr) ||
            plan.planContent?.toLowerCase().includes(searchStr) ||
            plan.reviewerName?.toLowerCase().includes(searchStr) ||
            plan.feedback?.toLowerCase().includes(searchStr)
        );

        return matchesSearch && (selectedStatus === 'All' || plan.status === selectedStatus);
    }) || [];

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
            case 'Chờ duyệt':
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
            case 'Chờ duyệt':
                return 'Đang xử lý';
            case 'Đã duyệt':
                return 'Đã duyệt';
            case 'Từ chối':
                return 'Từ chối';
            default:
                return status;
        }
    };

    const showDetailModal = (record) => {
        setSelectedRequest(record);
        setIsModalVisible(true);
    };

    const getGoogleDriveEmbedUrl = (url) => {
        if (!url) return '';
        try {
            const urlObj = new URL(url);
            if (urlObj.hostname === 'drive.google.com') {
                // For folders
                if (url.includes('/folders/')) {
                    const folderId = url.match(/\/folders\/([^?/]+)/)?.[1];
                    if (folderId) {
                        return `https://drive.google.com/embeddedfolderview?id=${folderId}&amp;usp=sharing#list`;
                    }
                }
                // For files
                else if (url.includes('/file/d/')) {
                    const fileId = url.match(/\/file\/d\/([^/]+)/)?.[1];
                    if (fileId) {
                        return `https://drive.google.com/file/d/${fileId}/preview`;
                    }
                }
            }
            return url;
        } catch (error) {
            console.error('Invalid URL:', error);
            return '';
        }
    };

    const DetailModal = () => (
        <Modal
            title="Chi tiết giáo án"
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
                        <h2>Thông tin chi tiết </h2>
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
                                    src={getGoogleDriveEmbedUrl(selectedRequest.attachmentUrl)}
                                    title="File đính kèm"
                                    width="100%"
                                    height="600px"
                                    style={{
                                        border: '1px solid #d9d9d9',
                                        borderRadius: '4px'
                                    }}
                                    onError={(e) => {
                                        e.target.style.display = 'none';
                                        message.error('Không thể tải tệp đính kèm. Vui lòng mở trong tab mới.');
                                    }}
                                />
                                <div style={{ marginTop: '10px', textAlign: 'center' }}>
                                    <a
                                        href={selectedRequest.attachmentUrl}
                                        target="_blank"
                                        rel="noopener noreferrer"
                                    >
                                        <Button type="primary">Mở trong tab mới</Button>
                                    </a>
                                </div>
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
                    {record.status === 'Chờ duyệt' && (
                        <Link to={`/system/lesson-plan/add-document/${record.planId}`}>
                            <Button>Thêm tài liệu</Button>
                        </Link>
                    )}
                </Space>
            ),
        },
    ];

    if (isLoading) {
        return <div className="loading">Đang tải dữ liệu...</div>;
    }

    return (
        <div className="lesson-plan-list">

            <h2>Danh sách phân công làm giáo án</h2>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                <h2></h2>

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
                    <select
                        value={selectedStatus}
                        onChange={(e) => setSelectedStatus(e.target.value)}
                    >
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
                    loading={isLoading}
                    rowKey="planId"
                    pagination={{
                        pageSize: pageSize,
                        total: data?.totalCount || 0,
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

export default TeacherListPlan
