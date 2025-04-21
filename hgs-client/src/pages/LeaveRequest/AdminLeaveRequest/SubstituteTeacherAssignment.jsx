import React, { useState, useEffect } from 'react';
import { Card, Table, Form, Select, Button, message } from 'antd';
import dayjs from 'dayjs';
import { useScheduleTeacher } from '../../../services/schedule/queries';

const { Option } = Select;

const getWeekdayName = (date) => {
  const weekdays = {
    0: 'Chủ Nhật',
    1: 'Thứ Hai',
    2: 'Thứ Ba',
    3: 'Thứ Tư',
    4: 'Thứ Năm',
    5: 'Thứ Sáu',
    6: 'Thứ Bảy'
  };
  return weekdays[dayjs(date).day()];
};

const SubstituteTeacherAssignment = ({ leaveRequest }) => {
  const [loading, setLoading] = useState(false);
  const [form] = Form.useForm();
  const { data: teacherSchedule, isLoading: scheduleLoading } = useScheduleTeacher(leaveRequest?.teacherId);
  const [filteredSchedules, setFilteredSchedules] = useState([]);


  // Process teacher schedule when data changes
  // Add this mapping function at the top of the component
  const mapWeekdayToAPI = (weekday) => {
    const weekdayMapping = {
      'Chủ Nhật': 'Chủ Nhật',
      'Thứ Hai': 'Thứ Hai',
      'Thứ Ba': 'Thứ Ba',
      'Thứ Tư': 'Thứ Tư',
      'Thứ Năm': 'Thứ Năm',
      'Thứ Sáu': 'Thứ Sáu',
      'Thứ Bảy': 'Thứ Bảy'
    };
    return weekdayMapping[weekday];
  };

  // Update the useEffect that processes the schedule
  // Add console logs to debug the data
  // First, add this console log outside useEffect to track initial data
  console.log('Initial render:', { teacherSchedule, leaveRequest });

  // Then update the useEffect
  useEffect(() => {
    if (!teacherSchedule?.[0]?.details || !leaveRequest) {
      return;
    }

    const scheduleDetails = teacherSchedule[0].details;
    const startDate = dayjs(leaveRequest.leaveFromDate);
    const endDate = dayjs(leaveRequest.leaveToDate);
    const processedSchedules = [];

    let currentDate = startDate;
    while (currentDate.isSame(endDate) || currentDate.isBefore(endDate)) {
      const weekday = getWeekdayName(currentDate);

      // Get all schedules for the current weekday
      const daySchedules = scheduleDetails.filter(schedule => {
        return schedule.dayOfWeek === weekday;
      });

      // Add each schedule for this day
      daySchedules.forEach(schedule => {
        processedSchedules.push({
          scheduleId: `${currentDate.format('YYYY-MM-DD')}-${schedule.timetableDetailId}`,
          date: currentDate.format('YYYY-MM-DD'),
          dayOfWeek: weekday,
          period: schedule.periodName,
          className: schedule.className,
          subject: schedule.subjectName,
          timetableDetailId: schedule.timetableDetailId
        });
      });

      currentDate = currentDate.add(1, 'day');
    }

    setFilteredSchedules(processedSchedules);
  }, [teacherSchedule, leaveRequest]);

  const mockTeachers = [
    { id: 'T001', name: 'Nguyễn Văn A' },
    { id: 'T002', name: 'Trần Thị B' },
    { id: 'T003', name: 'Lê Văn C' },
    { id: 'T004', name: 'Phạm Thị D' }
  ];

  const handleAssignSubstitute = async (values) => {
    try {
      setLoading(true);
      await new Promise(resolve => setTimeout(resolve, 1000));
      console.log('Dữ liệu phân công:', values);
      message.success('Phân công giáo viên dạy thay thành công');
      form.resetFields(['assignments']);
    } catch (error) {
      message.error('Phân công giáo viên dạy thay thất bại');
    } finally {
      setLoading(false);
    }
  };

  const columns = [
    {
      title: 'Ngày',
      dataIndex: 'date',
      key: 'date',
      render: (text, record) => `${dayjs(text).format('DD/MM/YYYY')} (${record.dayOfWeek})`
    },
    {
      title: 'Tiết học',
      dataIndex: 'period',
      key: 'period'
    },
    {
      title: 'Lớp',
      dataIndex: 'className',
      key: 'className'
    },
    {
      title: 'Môn học',
      dataIndex: 'subject',
      key: 'subject'
    },
    {
      title: 'Giáo viên dạy thay',
      key: 'substituteTeacher',
      render: (_, record) => (
        <Form.Item
          name={['assignments', record.scheduleId]}
          style={{ margin: 0 }}
          rules={[{ required: true, message: 'Vui lòng chọn giáo viên!' }]}
        >
          <Select
            placeholder="Chọn giáo viên dạy thay"
            optionFilterProp="children"
          >
            {mockTeachers.map(teacher => (
              <Option key={teacher.id} value={teacher.id}>
                {teacher.name}
              </Option>
            ))}
          </Select>
        </Form.Item>
      )
    }
  ];
  console.log("filteredSchedules", filteredSchedules)

  return (
    <div>
      <Card title="Thông tin lịch dạy cần thay thế" style={{ marginBottom: 16 }}>
        <p>
          <strong>Thời gian nghỉ:</strong> {dayjs(leaveRequest?.leaveFromDate).format('DD/MM/YYYY')} ({getWeekdayName(leaveRequest?.leaveFromDate)})
          - {dayjs(leaveRequest?.leaveToDate).format('DD/MM/YYYY')} ({getWeekdayName(leaveRequest?.leaveToDate)})
        </p>

      </Card>

      <Card title="Phân công giáo viên dạy thay">
        <Form form={form} onFinish={handleAssignSubstitute}>
          <Table
            dataSource={filteredSchedules}
            columns={columns}
            rowKey={record => record.scheduleId}
            pagination={false}
            loading={scheduleLoading}
          />
          <Form.Item style={{ marginTop: 16, textAlign: 'right' }}>
            <Button type="primary" htmlType="submit" loading={loading}>
              Lưu phân công
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
};

export default SubstituteTeacherAssignment;
