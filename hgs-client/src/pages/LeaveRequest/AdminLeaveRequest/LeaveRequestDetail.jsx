import React, { useState, useEffect } from 'react'; // Đảm bảo import useState và useEffect
import { useParams, useNavigate } from 'react-router-dom';
import { Card, Descriptions, Button, message } from 'antd';
import axios from 'axios';
import dayjs from 'dayjs';

const LeaveRequestDetail = () => {
  const { id } = useParams(); 
  const navigate = useNavigate();
  const [leaveRequest, setLeaveRequest] = useState(null);
  const [loading, setLoading] = useState(false);

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

  const handleUpdateStatus = async (status) => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      const updatedRequest = {
        requestId: parseInt(requestId),
        teacherId: leaveRequest.teacherId,
        requestDate: leaveRequest.requestDate,
        leaveFromDate: leaveRequest.leaveFromDate,
        leaveToDate: leaveRequest.leaveToDate,
        reason: leaveRequest.reason,
        status: status // Approved hoặc Rejected
      };

      await axios.put(`https://localhost:8386/api/LeaveRequest/${requestId}`, updatedRequest, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });

      message.success(`Yêu cầu đã được ${status === 'Approved' ? 'phê duyệt' : 'từ chối'} thành công`);
      navigate('/system/leave-request'); // Quay lại danh sách sau khi cập nhật
    } catch (error) {
      console.error('Error updating leave request:', error);
      message.error('Cập nhật trạng thái thất bại');
    } finally {
      setLoading(false);
    }
  };

  if (!leaveRequest) {
    return <div>Loading...</div>;
  }

  return (
    <div style={{ padding: '24px' }}>
      <h1>Chi tiết yêu cầu nghỉ phép</h1>
      <Card loading={loading}>
        <Descriptions bordered column={1}>
          <Descriptions.Item label="ID Yêu cầu">{leaveRequest.requestId}</Descriptions.Item>
          <Descriptions.Item label="Mã giáo viên">{leaveRequest.teacherId}</Descriptions.Item>
          <Descriptions.Item label="Ngày yêu cầu">
            {dayjs(leaveRequest.requestDate).format('DD/MM/YYYY')}
          </Descriptions.Item>
          <Descriptions.Item label="Ngày bắt đầu nghỉ">
            {dayjs(leaveRequest.leaveFromDate).format('DD/MM/YYYY')}
          </Descriptions.Item>
          <Descriptions.Item label="Ngày kết thúc nghỉ">
            {dayjs(leaveRequest.leaveToDate).format('DD/MM/YYYY')}
          </Descriptions.Item>
          <Descriptions.Item label="Lý do">{leaveRequest.reason}</Descriptions.Item>
          <Descriptions.Item label="Trạng thái">
            <span style={{ color: leaveRequest.status === 'Pending' ? 'gold' : leaveRequest.status === 'Approved' ? 'green' : 'red' }}>
              {leaveRequest.status}
            </span>
          </Descriptions.Item>
        </Descriptions>
      </Card>

      {leaveRequest.status === 'Pending' && (
        <div style={{ marginTop: '20px' }}>
          <Button
            type="primary"
            onClick={() => handleUpdateStatus('Approved')}
            loading={loading}
            style={{ marginRight: '10px' }}
          >
            Phê duyệt
          </Button>
          <Button
            type="danger"
            onClick={() => handleUpdateStatus('Rejected')}
            loading={loading}
          >
            Từ chối
          </Button>
        </div>
      )}
    </div>
  );
};

export default LeaveRequestDetail;