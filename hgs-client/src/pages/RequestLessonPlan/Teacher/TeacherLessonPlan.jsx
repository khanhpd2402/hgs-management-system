import React, { useState } from 'react';
import { Card } from "@/components/ui/card";
import {
    Table,
    TableHeader,
    TableRow,
    TableHead,
    TableBody,
    TableCell,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { Link } from 'react-router-dom';
import { EyeOutlined } from '@ant-design/icons';
import dayjs from 'dayjs';
import { jwtDecode } from 'jwt-decode';
import { useLessonPlanByTeacher } from '../../../services/lessonPlan/queries';
import { Modal, Space, Tag, Descriptions } from 'antd';

const TeacherLessonPlan = () => {
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

    return (
        <Card className="p-4">
            <div className="flex items-center justify-between mb-4">
                <h2 className="text-lg font-semibold">Danh sách phân công làm giáo án</h2>
                <Link to="/teacher/lesson-plan/create">
                    <Button variant="outline" className="bg-blue-600 text-white hover:bg-blue-700">
                        Tạo kế hoạch mới
                    </Button>
                </Link>
            </div>

            <div className="mb-4 flex gap-4">
                <div className="flex-1">
                    <input
                        type="text"
                        placeholder="Tìm kiếm..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="w-full px-3 py-2 border rounded-md"
                    />
                </div>
                <select
                    value={selectedStatus}
                    onChange={(e) => setSelectedStatus(e.target.value)}
                    className="px-3 py-2 border rounded-md"
                >
                    <option value="All">Tất cả</option>
                    <option value="Đang chờ">Đang xử lý</option>
                    <option value="Đã duyệt">Đã duyệt</option>
                    <option value="Từ chối">Từ chối</option>
                </select>
            </div>

            <div className="border rounded-lg overflow-hidden">
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead className="w-[100px]">ID</TableHead>
                            <TableHead>Môn học</TableHead>
                            <TableHead>Nội dung</TableHead>
                            <TableHead>Trạng thái</TableHead>
                            <TableHead>Ngày nộp</TableHead>
                            <TableHead>Người duyệt</TableHead>
                            <TableHead>Ngày duyệt</TableHead>
                            <TableHead>Phản hồi</TableHead>
                            <TableHead>Hành động</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {filteredLessonPlans.map((plan) => (
                            <TableRow key={plan.planId}>
                                <TableCell>{plan.planId}</TableCell>
                                <TableCell>{plan.subjectName}</TableCell>
                                <TableCell>{plan.planContent}</TableCell>
                                <TableCell>
                                    <span className={`status-${plan.status.toLowerCase()} ${getStatusClass(plan.status)}`}>
                                        {getStatusText(plan.status)}
                                    </span>
                                </TableCell>
                                <TableCell>{formatDate(plan.submittedDate)}</TableCell>
                                <TableCell>{plan.reviewerName}</TableCell>
                                <TableCell>{formatDate(plan.reviewedDate)}</TableCell>
                                <TableCell>{plan.feedback}</TableCell>
                                <TableCell>
                                    <Space>
                                        <Button
                                            variant="outline"
                                            size="sm"
                                            onClick={() => showDetailModal(plan)}
                                        >
                                            <EyeOutlined className="mr-2" />
                                            Xem chi tiết
                                        </Button>
                                        <Link to={`/system/lesson-plan/add-document/${plan.planId}`}>
                                            <Button variant="outline" size="sm">
                                                Thêm tài liệu
                                            </Button>
                                        </Link>
                                    </Space>
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </div>

            <DetailModal />
        </Card>
    );
};

export default TeacherLessonPlan;
