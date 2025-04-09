import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Card, Descriptions, Button, message, Steps, Form, Input, Select } from 'antd';
import axios from 'axios';
import dayjs from 'dayjs';
import SubstituteTeacherAssignment from './SubstituteTeacherAssignment';

const { Option } = Select;

const LeaveRequestDetail = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [leaveRequest, setLeaveRequest] = useState(null);
  const [loading, setLoading] = useState(false);
  const [currentStep, setCurrentStep] = useState(0);
  const [form] = Form.useForm();

  useEffect(() => {
    fetchLeaveRequestDetail();
  }, [id]);

  const fetchLeaveRequestDetail = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      const response = await axios.get(`https://localhost:8386/api/LeaveRequest/${id}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      setLeaveRequest(response.data);
    } catch (error) {
      console.error('Error fetching leave request detail:', error);
      message.error('Không thể tải chi tiết yêu cầu nghỉ phép');
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateStatus = async (values) => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      
      const updatedRequest = {
        requestId: leaveRequest.requestId,
        teacherId: leaveRequest.teacherId,
        requestDate: leaveRequest.requestDate,
        leaveFromDate: leaveRequest.leaveFromDate,
        leaveToDate: leaveRequest.leaveToDate,
        reason: values.reason,
        status: values.status
      };

      await axios.put(`https://localhost:8386/api/LeaveRequest/${id}`, updatedRequest, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      message.success(`Yêu cầu đã được ${values.status === 'Approved' ? 'phê duyệt' : 'từ chối'} thành công`);
      
      if (values.status === 'Approved') {
        setCurrentStep(2);
        await fetchLeaveRequestDetail();
      } else {
        navigate('/system/leave-request');
      }
    } catch (error) {
      console.error('Error updating leave request:', error);
      message.error('Cập nhật trạng thái thất bại');
    } finally {
      setLoading(false);
    }
  };

  const steps = [
    {
      title: 'Xem chi tiết',
      content: (
        <Card loading={loading}>
          <Descriptions bordered column={1}>
            <Descriptions.Item label="ID Yêu cầu">{leaveRequest?.requestId}</Descriptions.Item>
            <Descriptions.Item label="Mã giáo viên">{leaveRequest?.teacherId}</Descriptions.Item>
            <Descriptions.Item label="Ngày yêu cầu">
              {dayjs(leaveRequest?.requestDate).format('DD/MM/YYYY')}
            </Descriptions.Item>
            <Descriptions.Item label="Ngày bắt đầu nghỉ">
              {dayjs(leaveRequest?.leaveFromDate).format('DD/MM/YYYY')}
            </Descriptions.Item>
            <Descriptions.Item label="Ngày kết thúc nghỉ">
              {dayjs(leaveRequest?.leaveToDate).format('DD/MM/YYYY')}
            </Descriptions.Item>
            <Descriptions.Item label="Lý do">{leaveRequest?.reason}</Descriptions.Item>
            <Descriptions.Item label="Trạng thái">
              <span style={{ 
                color: leaveRequest?.status === 'Pending' ? 'gold' 
                     : leaveRequest?.status === 'Approved' ? 'green' 
                     : 'red' 
              }}>
                {leaveRequest?.status === 'Pending' ? 'Đang chờ duyệt'
                 : leaveRequest?.status === 'Approved' ? 'Đã phê duyệt'
                 : 'Đã từ chối'}
              </span>
            </Descriptions.Item>
          </Descriptions>
        </Card>
      )
    },
    {
      title: 'Cập nhật trạng thái',
      content: (
        <Card loading={loading}>
          <Form
            form={form}
            layout="vertical"
            onFinish={handleUpdateStatus}
            initialValues={{
              status: 'Approved',
              reason: leaveRequest?.reason
            }}
          >
            <Form.Item
              name="status"
              label="Trạng thái"
              rules={[{ required: true, message: 'Vui lòng chọn trạng thái!' }]}
            >
              <Select>
                <Option value="Approved">Phê duyệt</Option>
                <Option value="Rejected">Từ chối</Option>
              </Select>
            </Form.Item>

            <Form.Item
              name="reason"
              label="Lý do"
              rules={[{ required: true, message: 'Vui lòng nhập lý do!' }]}
            >
              <Input.TextArea rows={4} />
            </Form.Item>

            <Form.Item>
              <Button type="primary" htmlType="submit" loading={loading}>
                Cập nhật
              </Button>
            </Form.Item>
          </Form>
        </Card>
      )
    },
    {
      title: 'Phân công dạy thay',
      content: (
        <Card loading={loading}>
          {leaveRequest?.status === 'Approved' ? (
            <SubstituteTeacherAssignment leaveRequest={leaveRequest} />
          ) : (
            <div style={{ textAlign: 'center', padding: '24px' }}>
              Vui lòng phê duyệt yêu cầu nghỉ phép trước khi phân công giáo viên dạy thay
            </div>
          )}
        </Card>
      )
    }
  ];

  if (!leaveRequest) {
    return <div>Đang tải...</div>;
  }

  return (
    <div style={{ padding: '24px' }}>
      <Steps
        current={currentStep}
        items={steps.map(item => ({ title: item.title }))}
        style={{ marginBottom: '24px' }}
      />

      {steps[currentStep].content}

      <div style={{ marginTop: '24px' }}>
        {currentStep < steps.length - 1 && (
          <Button type="primary" onClick={() => setCurrentStep(currentStep + 1)}>
            Tiếp theo
          </Button>
        )}
        {currentStep > 0 && (
          <Button style={{ marginLeft: '8px' }} onClick={() => setCurrentStep(currentStep - 1)}>
            Quay lại
          </Button>
        )}
      </div>
    </div>
  );
};

export default LeaveRequestDetail;