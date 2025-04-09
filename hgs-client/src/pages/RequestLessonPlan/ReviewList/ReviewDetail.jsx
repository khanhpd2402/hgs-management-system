import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { 
    Card, 
    Form, 
    Input, 
    Select, 
    Button, 
    Descriptions, 
    Space, 
    message, 
    Divider,
    Typography
} from 'antd';
import { 
    RollbackOutlined, 
    CheckCircleOutlined, 
    FileTextOutlined 
} from '@ant-design/icons';
import axios from 'axios';
import dayjs from 'dayjs';
import './ReviewDetail.scss';

const { TextArea } = Input;
const { Option } = Select;
const { Title } = Typography;

const ReviewDetail = () => {
    const { planId } = useParams();
    const navigate = useNavigate();
    const [form] = Form.useForm();
    const [plan, setPlan] = useState(null);
    const [loading, setLoading] = useState(true);
    const [submitting, setSubmitting] = useState(false);

    useEffect(() => {
        fetchPlanDetail();
    }, [planId]);

    const fetchPlanDetail = async () => {
        try {
            setLoading(true);
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            const response = await axios.get(
                `https://localhost:8386/api/LessonPlan/${planId}`,
                {
                    headers: { 'Authorization': `Bearer ${token}` }
                }
            );
            setPlan(response.data);
        } catch (error) {
            console.error('Lỗi khi tải chi tiết:', error);
            message.error('Không thể tải thông tin kế hoạch');
        } finally {
            setLoading(false);
        }
    };

    const handleSubmit = async (values) => {
        try {
            setSubmitting(true);
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            await axios.post(
                'https://localhost:8386/api/LessonPlan/review',
                {
                    planId: parseInt(planId),
                    status: values.status,
                    feedback: values.feedback
                },
                {
                    headers: { 'Authorization': `Bearer ${token}` }
                }
            );
            message.success('Phê duyệt kế hoạch thành công!');
            navigate('/review-list');
        } catch (error) {
            console.error('Lỗi khi phê duyệt:', error);
            message.error('Có lỗi xảy ra khi phê duyệt');
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <div className="review-detail-container">
            <Card 
                loading={loading}
                className="review-detail-card"
                title={
                    <Title level={2}>
                        <FileTextOutlined /> Chi tiết kế hoạch giảng dạy
                    </Title>
                }
            >
                {plan && (
                    <>
                        <div className="plan-info-section">
                            <Descriptions 
                                bordered 
                                column={{ xxl: 2, xl: 2, lg: 2, md: 1, sm: 1, xs: 1 }}
                            >
                                <Descriptions.Item label="ID kế hoạch">
                                    {plan.planId}
                                </Descriptions.Item>
                                <Descriptions.Item label="Giáo viên">
                                    {plan.teacherName}
                                </Descriptions.Item>
                                <Descriptions.Item label="Môn học">
                                    {plan.subjectName}
                                </Descriptions.Item>
                                <Descriptions.Item label="Ngày nộp">
                                    {dayjs(plan.submittedDate).format('DD/MM/YYYY HH:mm')}
                                </Descriptions.Item>
                                <Descriptions.Item label="Nội dung" span={2}>
                                    {plan.planContent}
                                </Descriptions.Item>
                            </Descriptions>
                        </div>

                        <Divider />

                        <div className="attachment-preview-section">
                            <Title level={3}>
                                <FileTextOutlined /> File đính kèm
                            </Title>
                            <div className="attachment-container">
                                {plan.attachmentUrl ? (
                                    <div className="preview-wrapper">
                                        <iframe
                                            src={plan.attachmentUrl}
                                            title="File Preview"
                                            className="file-preview"
                                        />
                                        <Button 
                                            type="primary"
                                            onClick={() => window.open(plan.attachmentUrl, '_blank')}
                                            className="open-new-tab-button"
                                        >
                                            Mở trong tab mới
                                        </Button>
                                    </div>
                                ) : (
                                    <Empty
                                        image={Empty.PRESENTED_IMAGE_SIMPLE}
                                        description="Không có file đính kèm"
                                    />
                                )}
                            </div>
                        </div>

                        <Divider />

                        <div className="review-form-section">
                            <Title level={3}>Phê duyệt kế hoạch</Title>
                            <Form
                                form={form}
                                layout="vertical"
                                onFinish={handleSubmit}
                                className="review-form"
                            >
                                <Form.Item
                                    name="status"
                                    label="Trạng thái"
                                    rules={[{ required: true, message: 'Vui lòng chọn trạng thái!' }]}
                                >
                                    <Select placeholder="Chọn trạng thái">
                                        <Option value="Approved">Phê duyệt</Option>
                                        <Option value="Rejected">Từ chối</Option>
                                    </Select>
                                </Form.Item>

                                <Form.Item
                                    name="feedback"
                                    label="Phản hồi"
                                    rules={[{ required: true, message: 'Vui lòng nhập phản hồi!' }]}
                                >
                                    <TextArea 
                                        rows={4} 
                                        placeholder="Nhập phản hồi của bạn..."
                                    />
                                </Form.Item>

                                <Form.Item>
                                    <Space size="middle" className="form-buttons">
                                        <Button 
                                            icon={<RollbackOutlined />}
                                            onClick={() => navigate(-1)}
                                        >
                                            Quay lại
                                        </Button>
                                        <Button
                                            type="primary"
                                            icon={<CheckCircleOutlined />}
                                            htmlType="submit"
                                            loading={submitting}
                                        >
                                            Xác nhận phê duyệt
                                        </Button>
                                    </Space>
                                </Form.Item>
                            </Form>
                        </div>
                    </>
                )}
            </Card>
        </div>
    );
};

export default ReviewDetail;