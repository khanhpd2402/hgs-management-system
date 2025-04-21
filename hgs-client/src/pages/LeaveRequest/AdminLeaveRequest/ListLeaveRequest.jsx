import React, { useState, useEffect, useRef } from 'react';
import { Table, Space, Tag, Select, Form, Card, Spin, Alert, Input } from 'antd';
import axios from 'axios';
import dayjs from 'dayjs';
import { Link } from 'react-router-dom';
import { useGetLeaveRequestByAdmin } from '../../../services/leaveRequest/queries';
import { useTeachers } from '../../../services/teacher/queries';

const { Option } = Select;

const ListLeaveRequest = () => {
  const [teachers, setTeachers] = useState([]);
  const [filteredData, setFilteredData] = useState([]);
  const [filters, setFilters] = useState({
    status: 'all',
    teacherId: 'all',
    searchTerm: ''
  });
  const debounceTimeoutRef = useRef(null);

  const { data: leaveRequestsData, isLoading: loadingLeaveRequests, error: errorLeaveRequests } = useGetLeaveRequestByAdmin();
  const { data: teachersData, isLoading: teachersLoading } = useTeachers();

  useEffect(() => {
    if (teachersData?.teachers) {
      const formattedTeachers = teachersData.teachers.map(teacher => ({
        teacherId: teacher.teacherId || teacher.id,
        fullName: teacher.fullName || `${teacher.firstName || ''} ${teacher.lastName || ''}`.trim(),
        dob: teacher.dob
      })).filter(teacher => teacher.teacherId && teacher.fullName);

      setTeachers(formattedTeachers);
    }
  }, [teachersData]);



  useEffect(() => {
    if (leaveRequestsData) {
      filterData(leaveRequestsData);
    } else {
      setFilteredData([]);
    }
  }, [filters, leaveRequestsData, teachers]);

  const filterData = (requests) => {
    const { status, teacherId, searchTerm } = filters;
    const lowerSearchTerm = searchTerm.toLowerCase();

    let result = [...requests];

    if (status !== 'all') {
      result = result.filter(item => item.status === status);
    }

    if (teacherId !== 'all') {
      result = result.filter(item => item.teacherId === teacherId);
    }

    if (lowerSearchTerm) {
      result = result.filter(item => {
        const teacherInfo = teachers.find(t => t.teacherId === item.teacherId);
        const teacherName = teacherInfo ? teacherInfo.fullName.toLowerCase() : '';
        const reason = item.reason ? item.reason.toLowerCase() : '';

        return teacherName.includes(lowerSearchTerm) || reason.includes(lowerSearchTerm);
      });
    }

    setFilteredData(result);
  };

  const fetchTeachers = async () => {
    const token = localStorage.getItem('token');
    
    if (!token) {
      console.error("Không tìm thấy token trong localStorage.");
      setTeachers([]); 
      return; 
    }

    const cleanedToken = token.replace(/^"|"$/g, '');
    console.log("Cleaned Token:", cleanedToken); 

    try {
      console.log("Đang gọi API để lấy danh sách giáo viên...");
      const response = await axios.get('https://localhost:8386/api/Teachers', {
        headers: {
          'accept': '*/*',
          'Authorization': `Bearer ${cleanedToken}`
        }
      });

      console.log("API Response Data:", response.data); 

      if (response.data && response.data.teachers && Array.isArray(response.data.teachers)) {
        const formattedTeachers = response.data.teachers.map(teacher => ({
          teacherId: teacher.teacherId || teacher.id, 
          fullName: teacher.fullName || `${teacher.firstName || ''} ${teacher.lastName || ''}`.trim(), 
          dob: teacher.dob 
        })).filter(teacher => teacher.teacherId && teacher.fullName); 

        console.log("Formatted Teachers Data:", formattedTeachers); 

        setTeachers(formattedTeachers);
        console.log("Đã cập nhật state teachers.");
      } else {
         console.error("Dữ liệu API không hợp lệ hoặc không chứa mảng 'teachers':", response.data);
         setTeachers([]); 
      }
    } catch (error) {
      console.error("Lỗi nghiêm trọng khi tải danh sách giáo viên:");
      if (error.response) {
        console.error("Status:", error.response.status);
        console.error("Data:", error.response.data);
        console.error("Headers:", error.response.headers);
      } else if (error.request) {
        console.error("Request Error: Không nhận được phản hồi từ server.", error.request);
      } else {
        console.error("Error setting up request:", error.message);
      }
      console.error("Full Error Object:", error.config);
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
          <div><strong>{teacher.fullName} - {teacher.teacherId}</strong></div>
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

  const handleSearchChange = (event) => {
    const { value } = event.target;

    if (debounceTimeoutRef.current) {
      clearTimeout(debounceTimeoutRef.current);
    }

    debounceTimeoutRef.current = setTimeout(() => {
      setFilters(prev => ({
        ...prev,
        searchTerm: value
      }));
    }, 500);
  };

  const handleSearchSubmit = (value) => {
    if (debounceTimeoutRef.current) {
      clearTimeout(debounceTimeoutRef.current);
    }
    setFilters(prev => ({
      ...prev,
      searchTerm: value
    }));
  };

  const FilterSection = () => (
    <Card style={{ marginBottom: 16 }}>
      <Form layout="inline" style={{ display: 'flex', flexWrap: 'wrap', gap: '16px' }}>
        <Form.Item label="Trạng thái">
          <Select
            style={{ width: 200 }}
            value={filters.status}
            onChange={(value) => handleFilterChange('status', value)}
          >
            <Option value="all">Tất cả trạng thái</Option>
            <Option value="Chờ Duyệt">Đang chờ duyệt</Option>
            <Option value="Đã Duyệt">Đã phê duyệt</Option>
            <Option value="Từ Chối">Đã từ chối</Option>
          </Select>
        </Form.Item>

        <Form.Item label="Giáo viên">
          <Select
            style={{ width: 200 }}
            value={filters.teacherId}
            onChange={(value) => handleFilterChange('teacherId', value)}
            showSearch
            optionFilterProp="children"
            filterOption={(input, option) =>
              (option?.children ?? '').toLowerCase().includes(input.toLowerCase())
            }
          >
            <Option value="all">Tất cả giáo viên</Option>
            {teachers.map(teacher => (
              <Option key={teacher.teacherId} value={teacher.teacherId}>
                {teacher.fullName} - {teacher.teacherId}
              </Option>
            ))}
          </Select>
        </Form.Item>

        <Form.Item label="Tìm kiếm">
          <Input.Search
            placeholder="Nhập tên GV hoặc lý do..."
            allowClear
            onChange={handleSearchChange}
            onSearch={handleSearchSubmit}
            style={{ width: 240 }}
            defaultValue={filters.searchTerm}
          />
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
          status === 'Chờ Duyệt' ? 'gold'
            : status === 'Đã Duyệt' ? 'green'
              : 'red'
        }>
          {status === 'Chờ Duyệt' ? 'Đang chờ duyệt'
            : status === 'Đã Duyệt' ? 'Đã phê duyệt'
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

  if (loadingLeaveRequests) {
    return <Spin tip="Đang tải dữ liệu..." />;
  }

  if (errorLeaveRequests) {
    return <Alert message="Lỗi" description={`Không thể tải danh sách yêu cầu nghỉ phép: ${errorLeaveRequests.message}`} type="error" showIcon />;
  }

  return (
    <div style={{ padding: '24px' }}>
      <h1>Danh sách yêu cầu nghỉ phép</h1>

      <FilterSection />

      <Table
        columns={columns}
        dataSource={filteredData}
        loading={loadingLeaveRequests}
        rowKey="requestId"
        pagination={{
          pageSize: 10,
          showTotal: (total, range) => `Hiển thị ${range[0]}-${range[1]} của ${total} yêu cầu`,
        }}
      />
    </div>
  );
};

export default ListLeaveRequest;
