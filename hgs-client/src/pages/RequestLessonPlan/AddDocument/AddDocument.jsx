import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { Card, Button, Descriptions, Space, Modal, Form, Input, message } from 'antd';
import { EditOutlined, EyeOutlined } from '@ant-design/icons';
import toast from 'react-hot-toast';

const AddDocument = () => {
    const [lessonPlan, setLessonPlan] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isUpdateModalVisible, setIsUpdateModalVisible] = useState(false);
    const [isPreviewVisible, setIsPreviewVisible] = useState(false);
    const [form] = Form.useForm();
    const { planId } = useParams();
    const navigate = useNavigate();

    const getToken = () => localStorage.getItem('token')?.replace(/^"|"$/g, '');

    const fetchLessonPlan = async () => {
        try {
            const response = await axios.get(`https://localhost:8386/api/LessonPlan/${planId}`, {
                headers: { Authorization: `Bearer ${getToken()}` },
            });
            setLessonPlan(response.data);
            setLoading(false);
        } catch (error) {
            console.error('Error fetching lesson plan:', error);
            message.error('Không thể tải chi tiết giáo án');
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchLessonPlan();
    }, [planId]);

    const formatDate = (dateString) =>
        dateString
            ? new Date(dateString).toLocaleDateString('vi-VN', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
                hour: '2-digit',
                minute: '2-digit',
            })
            : 'N/A';

    const getGoogleDriveEmbedUrl = (url) => {
        if (!url || !url.includes('drive.google.com')) return url;
        if (url.includes('/folders/')) {
            const folderId = url.match(/\/folders\/([^?/]+)/)?.[1];
            return folderId ? `https://drive.google.com/embeddedfolderview?id=${folderId}#list` : url;
        }
        if (url.includes('/file/d/')) {
            const fileId = url.match(/\/file\/d\/([^/]+)/)?.[1];
            return fileId ? `https://drive.google.com/file/d/${fileId}/preview` : url;
        }
        return url;
    };

    const handleUpdate = async (values) => {
        try {
            // Validate required fields
            if (!values.title || !values.planContent) {
                toast.error('Vui lòng điền đầy đủ thông tin bắt buộc');
                return;
            }

            // Validate dates
            const startDate = new Date(lessonPlan.startDate);
            const endDate = new Date(lessonPlan.endDate);
            const currentDate = new Date();


            if (isNaN(startDate) || isNaN(endDate)) {
                toast.error('Ngày bắt đầu hoặc ngày kết thúc không hợp lệ');
                return;
            }

            if (startDate >= endDate) {
                toast.error('Ngày kết thúc phải sau ngày bắt đầu');
                return;
            }



            if (currentDate < startDate || currentDate > endDate) {
                const formattedStart = formatDate(startDate);
                const formattedEnd = formatDate(endDate);
                toast.error(`Bạn chỉ có thể cập nhật giáo án từ ngày ${formattedStart} đến ${formattedEnd}`);
                return;
            }
            console.log("startDate: ", startDate)
            console.log("endDate: ", endDate)
            console.log("currentDate: ", currentDate)
            console.log("currentDate >= startDate: ", currentDate >= startDate)
            console.log("currentDate <= endDate: ", currentDate <= endDate)

            await axios.put(
                `https://localhost:8386/api/LessonPlan/${planId}/update`,
                values,
                {
                    headers: {
                        Authorization: `Bearer ${getToken()}`,
                        'Content-Type': 'application/json',
                    },
                }
            );

            toast.success('Cập nhật giáo án thành công!');
            setIsUpdateModalVisible(false);
            fetchLessonPlan();
        } catch (error) {
            console.error('Error updating lesson plan:', error);
            toast.error(error.response?.data?.message || 'Cập nhật giáo án thất bại');
        }
    };

    const showUpdateModal = () => {
        form.setFieldsValue({
            planContent: lessonPlan.planContent,
            title: lessonPlan.title,
            attachmentUrl: lessonPlan.attachmentUrl,
        });
        setIsUpdateModalVisible(true);
    };

    return (
        <div style={{ padding: '24px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
                <h2>Chi tiết giáo án</h2>
            </div>
            {loading ? (
                <div>Đang tải...</div>
            ) : lessonPlan ? (
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
                        <Descriptions.Item label="Người duyệt">{lessonPlan.reviewerName || 'N/A'}</Descriptions.Item>
                        <Descriptions.Item label="Ngày duyệt">{formatDate(lessonPlan.reviewedDate)}</Descriptions.Item>
                        <Descriptions.Item label="Phản hồi">{lessonPlan.feedback || 'Chưa có phản hồi'}</Descriptions.Item>
                        <Descriptions.Item label="Link tài liệu đính kèm">
                            <Space>
                                {lessonPlan.attachmentUrl ? (
                                    <>
                                        <a href={lessonPlan.attachmentUrl} target="_blank" rel="noopener noreferrer">
                                            Link drive
                                        </a>
                                        <Button
                                            type="primary"
                                            size="small"
                                            icon={<EyeOutlined />}
                                            style={{
                                                borderRadius: '6px',
                                                background: '#1890ff',
                                                boxShadow: '0 2px 4px rgba(24, 144, 255, 0.2)',
                                            }}
                                            onClick={() => setIsPreviewVisible(true)}
                                        >
                                            Xem nhanh
                                        </Button>
                                    </>
                                ) : (
                                    'Chưa có link'
                                )}
                            </Space>
                        </Descriptions.Item>
                    </Descriptions>

                    <div style={{ marginTop: '24px', display: 'flex', justifyContent: 'space-between' }}>
                        <Button type="primary" icon={<EditOutlined />} onClick={showUpdateModal}>
                            Cập nhật giáo án
                        </Button>
                        <Button onClick={() => navigate(-1)}>Trở về</Button>
                    </div>
                </Card>
            ) : (
                <div>Không tìm thấy giáo án</div>
            )}

            <Modal
                title="Cập nhật giáo án"
                open={isUpdateModalVisible}
                onCancel={() => setIsUpdateModalVisible(false)}
                footer={null}
            >
                <Form form={form} onFinish={handleUpdate} layout="vertical">
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
                    <Form.Item name="attachmentUrl" label="URL đính kèm">
                        <Input />
                    </Form.Item>
                    <Form.Item>
                        <Space>
                            <Button type="primary" htmlType="submit">
                                Cập nhật
                            </Button>
                            <Button onClick={() => setIsUpdateModalVisible(false)}>Hủy</Button>
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
                        sandbox="allow-same-origin allow-scripts allow-popups allow-forms"
                    />
                </div>
            </Modal>
        </div>
    );
};

export default AddDocument;