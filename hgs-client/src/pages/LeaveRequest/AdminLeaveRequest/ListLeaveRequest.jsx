import React, { useState, useEffect } from 'react';
import { Table, Space, Tag } from 'antd';
import axios from 'axios';
import dayjs from 'dayjs';
import { Link } from 'react-router-dom';

const ListLeaveRequest = () => {
  const [leaveRequests, setLeaveRequests] = useState([]);
  const [loading, setLoading] = useState(false);
  const [teachers, setTeachers] = useState([]);

  useEffect(() => {
    fetchLeaveRequests();
    fetchTeachers();
  }, []);

  const fetchLeaveRequests = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      const response = await axios.get('https://localhost:8386/api/LeaveRequest', {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      setLeaveRequests(response.data);
    } catch (error) {
      console.error('Error fetching leave requests:', error);
    } finally {
      setLoading(false);
    }
  };

  const fetchTeachers = async () => {
    try {
      const token = localStorage.getItem('token');
      const response = await axios.get('https://localhost:8386/api/Teachers', {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      const teachersList = response.data.teachers || response.data || [];
      setTeachers(teachersList);
    } catch (error) {
      console.error('Error fetching teachers:', error);
      setTeachers([]);
    }
  };

  const getTeacherInfo = (teacherId) => {
    if (!Array.isArray(teachers)) {
      return `ID: ${teacherId}`;
    }

    const teacher = teachers.find(t => t.teacherId === teacherId);
    if (teacher) {
      return (
        <div>
          <div><strong>{teacher.fullName}</strong></div>
          <div style={{ fontSize: '12px', color: '#666' }}>
            {dayjs(teacher.dob).format('DD/MM/YYYY')}
          </div>
        </div>
      );
    }
    return `ID: ${teacherId}`;
  };

  const columns = [
    {
      title: 'ID',
      dataIndex: 'requestId',
      key: 'requestId',
    },
    {
      title: 'Mã giáo viên',
      dataIndex: 'teacherId',
      key: 'teacherId',
      render: (teacherId) => getTeacherInfo(teacherId),
    },
    {
      title: 'Ngày yêu cầu',
      dataIndex: 'requestDate',
      key: 'requestDate',
      render: (date) => dayjs(date).format('DD/MM/YYYY'),
    },
    {
      title: 'Ngày bắt đầu nghỉ',
      dataIndex: 'leaveFromDate',
      key: 'leaveFromDate',
      render: (date) => dayjs(date).format('DD/MM/YYYY'),
    },
    {
      title: 'Ngày kết thúc nghỉ',
      dataIndex: 'leaveToDate',
      key: 'leaveToDate',
      render: (date) => dayjs(date).format('DD/MM/YYYY'),
    },
    {
      title: 'Lý do',
      dataIndex: 'reason',
      key: 'reason',
    },
    {
      title: 'Trạng thái',
      dataIndex: 'status',
      key: 'status',
      render: (status) => (
        <Tag color={status === 'Pending' ? 'gold' : status === 'Approved' ? 'green' : 'red'}>
          {status}
        </Tag>
      ),
    },
    {
      title: 'Hành động',
      dataIndex: 'action',
      key: 'action',
      render: (_, record) => (
        <Link to={`/system/leave-request/${record.requestId}`}>Xem chi tiết</Link>
      ),
    },
  ];

  return (
    <div style={{ padding: '24px' }}>
      <h1>Danh sách yêu cầu nghỉ phép</h1>
      <Table
        columns={columns}
        dataSource={leaveRequests}
        loading={loading}
        rowKey="requestId"
        pagination={{
          pageSize: 10,
          showTotal: (total) => `Tổng số ${total} yêu cầu`,
        }}
      />
    </div>
  );
};

export default ListLeaveRequest;
