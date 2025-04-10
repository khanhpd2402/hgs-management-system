import React, { useState } from 'react';
import { Form, Input, Button, DatePicker } from 'antd';
import toast from 'react-hot-toast';
import axios from 'axios';
import dayjs from 'dayjs';
import isSameOrAfter from 'dayjs/plugin/isSameOrAfter';
import './CreateTeacherLeaveRequest.scss';
import { Link, useNavigate } from 'react-router-dom';

dayjs.extend(isSameOrAfter);

const CreateTeacherLeaveRequest = () => {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);
  const [startDate, setStartDate] = useState(null);
  const navigate = useNavigate();

  const handleStartDateChange = (date) => {
    setStartDate(date);
    form.setFieldsValue({ leaveToDate: null });
  };

  const disabledStartDate = (current) => {
    return current && current < dayjs().startOf('day');
  };

  const disabledEndDate = (current) => {
    if (!startDate) {
      return true;
    }
    return current && current < startDate.startOf('day');
  };

  const onFinish = async (values) => {
    setLoading(true);

    const payload = {
      teacherId: 1,
      leaveFromDate: values.leaveFromDate.format('YYYY-MM-DD'),
      leaveToDate: values.leaveToDate.format('YYYY-MM-DD'),
      reason: values.reason,
    };

    try {
      const token = localStorage.getItem('token');
      const response = await axios.post(
        'https://localhost:8386/api/LeaveRequest',
        payload,
        {
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        }
      );
      console.log('API call successful.');
      toast.success('Yêu cầu nghỉ phép đã được gửi thành công!');
      form.resetFields();
      setStartDate(null);

      setTimeout(() => {
        navigate('/teacher/leave-request');
      }, 2000);

    } catch (error) {
      console.error('Error creating leave request:', error);
      const errorMessage = error.response?.data?.message || 'Có lỗi xảy ra khi gửi yêu cầu nghỉ phép.';
      toast.error(errorMessage);
    } finally {
      if (!(payload.status === 200 || payload.status === 201)) {
        setLoading(false);
      }
    }
  };

  return (
    <div className="create-teacher-leave-request">
      <h1>Tạo yêu cầu nghỉ phép mới</h1>
      <Form
        form={form}
        layout="vertical"
        onFinish={onFinish}
      >
        <div className="date-picker-row">
          <Form.Item
            label="Ngày bắt đầu nghỉ"
            name="leaveFromDate"
            rules={[{ required: true, message: 'Vui lòng chọn ngày bắt đầu!' }]}
          >
            <DatePicker
              format="DD/MM/YYYY"
              disabledDate={disabledStartDate}
              onChange={handleStartDateChange}
              placeholder="Chọn ngày bắt đầu"
            />
          </Form.Item>

          <Form.Item
            label="Ngày kết thúc nghỉ"
            name="leaveToDate"
            rules={[
              { required: true, message: 'Vui lòng chọn ngày kết thúc!' },
            ]}
          >
            <DatePicker
              format="DD/MM/YYYY"
              disabledDate={disabledEndDate}
              disabled={!startDate}
              placeholder="Chọn ngày kết thúc"
            />
          </Form.Item>
        </div>

        <Form.Item
          label="Lý do"
          name="reason"
          rules={[{ required: true, message: 'Vui lòng nhập lý do!' }]}
        >
          <Input.TextArea rows={4} placeholder="Nhập lý do nghỉ phép" />
        </Form.Item>

        <Form.Item>
          <div className="button-row">
            <Button type="primary" htmlType="submit" loading={loading}>
              Gửi yêu cầu
            </Button>
            <Link to="/teacher/leave-request">
              <Button type="default" disabled={loading}>
                Quay lại
              </Button>
            </Link>
          </div>
        </Form.Item>
      </Form>
    </div>
  );
};

export default CreateTeacherLeaveRequest;