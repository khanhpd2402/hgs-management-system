import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import { Card, Button, Descriptions, Space, Upload, message, Modal, Form, Input } from 'antd';
import { UploadOutlined, EditOutlined, EyeOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast'; // Add this import

const AddDocument = () => {
    const [lessonPlan, setLessonPlan] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isUpdateModalVisible, setIsUpdateModalVisible] = useState(false);
    const [isPreviewVisible, setIsPreviewVisible] = useState(false);
    const [form] = Form.useForm();
    const { planId } = useParams();
    const navigate = useNavigate();

    const fetchLessonPlan = async () => {
        try {
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            const response = await axios.get(`https://localhost:8386/api/LessonPlan/${planId}`, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            setLessonPlan(response.data);
            setLoading(false);
        } catch (error) {
            console.error('Error fetching lesson plan:', error);
            message.error('Failed to load lesson plan details');
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchLessonPlan();
    }, [planId]);

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

    const handleUpdate = async (values) => {
        try {
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            await axios.put(`https://localhost:8386/api/LessonPlan/${planId}/update`, values, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            toast.success('Cập nhật thành công!');
            setIsUpdateModalVisible(false);
            fetchLessonPlan();
        } catch (error) {
            console.error('Error updating lesson plan:', error);
            toast.error('Cập nhật thất bại');
        }
    };

    const showUpdateModal = () => {
        form.setFieldsValue({
            planContent: lessonPlan.planContent,
            title: lessonPlan.title,
            attachmentUrl: lessonPlan.attachmentUrl
        });
        setIsUpdateModalVisible(true);
    };
    const getGoogleDriveEmbedUrl = (url) => {
        if (!url) return '';

        if (url.includes('drive.google.com')) {
            // Handle folder URLs
            if (url.includes('/folders/')) {
                const folderId = url.match(/\/folders\/([^?/]+)/)?.[1];
                if (folderId) {
                    return `https://drive.google.com/embeddedfolderview?id=${folderId}#list`;
                }
            }
            // Handle file URLs
            else if (url.includes('/file/d/')) {
                const fileId = url.match(/\/file\/d\/([^/]+)/)?.[1];
                if (fileId) {
                    return `https://drive.google.com/file/d/${fileId}/preview`;
                }
            }
        }
        return url;
    };
    return (
        <div style={{ padding: '24px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>

                    <h2>Chi tiết kế hoạch giáo án</h2>
                </div>

            </div>
            {lessonPlan && (
                <Card>
                    <Descriptions bordered column={2}>
                        <Descriptions.Item label="ID Kế hoạch">{lessonPlan.planId}</Descriptions.Item>
                        <Descriptions.Item label="Giáo viên">{lessonPlan.teacherName}</Descriptions.Item>
                        <Descriptions.Item label="Môn học">{lessonPlan.subjectName}</Descriptions.Item>
                        <Descriptions.Item label="Trạng thái">{lessonPlan.status}</Descriptions.Item>
                        <Descriptions.Item label="Tiêu đề">{lessonPlan.title}</Descriptions.Item>
                        <Descriptions.Item label="Nội dung">{lessonPlan.planContent}</Descriptions.Item>
                        <Descriptions.Item label="Ngày bắt đầu">{formatDate(lessonPlan.startDate)}</Descriptions.Item>
                        <Descriptions.Item label="Ngày kết thúc">{formatDate(lessonPlan.endDate)}</Descriptions.Item>
                        <Descriptions.Item label="Ngày nộp">{formatDate(lessonPlan.submittedDate)}</Descriptions.Item>
                        <Descriptions.Item label="Người duyệt">{lessonPlan.reviewerName}</Descriptions.Item>
                        <Descriptions.Item label="Ngày duyệt">{formatDate(lessonPlan.reviewedDate)}</Descriptions.Item>
                        <Descriptions.Item label="Phản hồi">{lessonPlan.feedback || 'Chưa có phản hồi'}</Descriptions.Item>
                        <Descriptions.Item label="Link tài liệu đính kèm">
                            <Space>
                                <a href={lessonPlan.attachmentUrl} target="_blank" rel="noopener noreferrer">
                                    Link drive tại tại đây
                                </a>
                                <Button
                                    type="primary"
                                    size="small"
                                    icon={<EyeOutlined />}
                                    style={{
                                        borderRadius: '6px',
                                        background: '#1890ff',
                                        boxShadow: '0 2px 4px rgba(24, 144, 255, 0.2)',
                                        display: 'flex',
                                        alignItems: 'center',
                                        gap: '4px'
                                    }}
                                    onClick={() => setIsPreviewVisible(true)}
                                >
                                    Xem nhanh
                                </Button>
                            </Space>
                        </Descriptions.Item>
                    </Descriptions>

                    <div style={{ marginTop: '24px' }}>
                        <Space direction="vertical" style={{ width: '100%' }}>
                            <Upload
                                action={`https://localhost:8386/api/LessonPlan/${planId}/document`}
                                headers={{
                                    Authorization: `Bearer ${localStorage.getItem('token')?.replace(/^"|"$/g, '')}`,
                                }}
                                onChange={(info) => {
                                    if (info.file.status === 'done') {
                                        toast.success('Tải lên tài liệu thành công!');
                                        fetchLessonPlan();
                                    } else if (info.file.status === 'error') {
                                        toast.error('Tải lên tài liệu thất bại');
                                    }
                                }}
                            >
                            </Upload>
                        </Space>
                        <div style={{ marginTop: '24px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                            <Button type="primary" icon={<UploadOutlined />} onClick={showUpdateModal}>
                                Upload tài liệu
                            </Button>
                            <Button onClick={() => navigate(-1)}>
                                Trở về
                            </Button>
                        </div>
                    </div>
                </Card>
            )}
            <Modal
                title="Cập nhật kế hoạch giáo án"
                open={isUpdateModalVisible}
                onCancel={() => setIsUpdateModalVisible(false)}
                footer={null}
            >
                <Form
                    form={form}
                    onFinish={handleUpdate}
                    layout="vertical"
                >
                    <Form.Item
                        name="title"
                        label="Tiêu đề"
                        rules={[{ required: true, message: 'Vui lòng nhập tiêu đề' }]}
                    >
                        <Input />
                    </Form.Item>

                    <Form.Item
                        name="planContent"
                        label="Nội dung"
                        rules={[{ required: true, message: 'Vui lòng nhập nội dung' }]}
                    >
                        <Input.TextArea rows={4} />
                    </Form.Item>

                    <Form.Item
                        name="attachmentUrl"
                        label="URL đính kèm"
                    >
                        <Input />
                    </Form.Item>

                    <Form.Item>
                        <Space>
                            <Button type="primary" htmlType="submit">
                                Cập nhật
                            </Button>
                            <Button onClick={() => setIsUpdateModalVisible(false)}>
                                Hủy
                            </Button>
                        </Space>
                    </Form.Item>
                </Form>
            </Modal>
            <Modal
                title="Xem tài liệu"
                open={isPreviewVisible}
                onCancel={() => setIsPreviewVisible(false)}
                width="90%"
                style={{ top: 20 }}
                footer={null}
            >
                <div style={{ height: '80vh' }}>
                    <iframe
                        src={getGoogleDriveEmbedUrl(lessonPlan?.attachmentUrl)}
                        width="100%"
                        height="100%"
                        frameBorder="0"
                        allowFullScreen
                        allow="autoplay"
                        sandbox="allow-same-origin allow-scripts allow-popups allow-forms"
                    />
                </div>
            </Modal>
        </div>
    );
};

export default AddDocument;
