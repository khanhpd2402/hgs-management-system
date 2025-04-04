import React, { useState, useEffect } from 'react';
import { Table, Space, Tag, Select, Form, Card } from 'antd';
import axios from 'axios';
import dayjs from 'dayjs';
import { Link } from 'react-router-dom';

const { Option } = Select;

const ListLeaveRequest = () => {
  const [leaveRequests, setLeaveRequests] = useState([]);
  const [loading, setLoading] = useState(false);
  const [teachers, setTeachers] = useState([]);
  const [filteredData, setFilteredData] = useState([]);
  const [filters, setFilters] = useState({
    status: 'all',
    teacherId: 'all'
  });

  useEffect(() => {
    fetchLeaveRequests();
    fetchTeachers();
  }, []);

  useEffect(() => {
    filterData();
  }, [filters, leaveRequests]);

  const filterData = () => {
    let result = [...leaveRequests];

    if (filters.status !== 'all') {
      result = result.filter(item => item.status === filters.status);
    }

    if (filters.teacherId !== 'all') {
      result = result.filter(item => item.teacherId === filters.teacherId);
    }

    setFilteredData(result);
  };

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

  const handleFilterChange = (filterName, value) => {
    setFilters(prev => ({
      ...prev,
      [filterName]: value
    }));
  };

  const FilterSection = () => (
    <Card style={{ marginBottom: 16 }}>
      <Form layout="inline">
        <Form.Item label="Trạng thái">
          <Select
            style={{ width: 200 }}
            value={filters.status}
            onChange={(value) => handleFilterChange('status', value)}
          >
            <Option value="all">Tất cả trạng thái</Option>
            <Option value="Pending">Đang chờ duyệt</Option>
            <Option value="Approved">Đã phê duyệt</Option>
            <Option value="Rejected">Đã từ chối</Option>
          </Select>
        </Form.Item>

        <Form.Item label="Giáo viên">
          <Select
            style={{ width: 200 }}
            value={filters.teacherId}
            onChange={(value) => handleFilterChange('teacherId', value)}
            showSearch
            optionFilterProp="children"
          >
            <Option value="all">Tất cả giáo viên</Option>
            {teachers.map(teacher => (
              <Option key={teacher.teacherId} value={teacher.teacherId}>
                {teacher.fullName} - {teacher.teacherId}
              </Option>
            ))}
          </Select>
        </Form.Item>
      </Form>
    </Card>
  );

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
        <Tag color={
          status === 'Pending' ? 'gold' 
          : status === 'Approved' ? 'green' 
          : 'red'
        }>
          {status === 'Pending' ? 'Đang chờ duyệt'
           : status === 'Approved' ? 'Đã phê duyệt'
           : 'Đã từ chối'}
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
      
      <FilterSection />

      <Table
        columns={columns}
        dataSource={filteredData}
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
