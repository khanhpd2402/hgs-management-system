import React, { useState, useEffect } from 'react';
import { Card, Table, Form, Select, Button, message, Input } from 'antd';
import dayjs from 'dayjs';
import { useScheduleTeacher } from '../../../services/schedule/queries';
import { useTeachers } from '../../../services/teacher/queries';
import toast from "react-hot-toast";

const { Option } = Select;

const getWeekdayName = (date) => {
  const weekdays = ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'];
  return weekdays[dayjs(date).day()];
};

const SubstituteTeacherAssignment = ({ leaveRequest }) => {
  const [loading, setLoading] = useState(false);
  const [filteredSchedules, setFilteredSchedules] = useState([]);
  const [form] = Form.useForm();
  const { data: teacherSchedule, isLoading: scheduleLoading } = useScheduleTeacher(leaveRequest?.teacherId);
  const { data: teachersData, isLoading: teachersLoading } = useTeachers();

  // Validate and create payload for API
  const createPayload = (record, teacherId, note) => ({
    timetableDetailId: Number(record.timetableDetailId),
    originalTeacherId: Number(leaveRequest.teacherId),
    substituteTeacherId: Number(teacherId),
    date: record.date,
    note: note || '',
  });

  // Save assignment via API
  const handleSaveAssignment = async (record, teacherId, note) => {
    try {
      const payload = createPayload(record, teacherId, note);

      // Validate payload
      const missingFields = Object.entries(payload).filter(([key, value]) => !value && key !== 'note');
      if (missingFields.length) {
        console.error('Missing required fields:', missingFields);
        message.error('Dữ liệu không hợp lệ, vui lòng kiểm tra lại!');
        return;
      }

      // Validate date
      const currentDate = dayjs().startOf('day');
      const assignmentDate = dayjs(payload.date);
      if (assignmentDate.isBefore(currentDate)) {
        console.error('Invalid date:', payload.date);
        message.error('Ngày dạy thay không thể là ngày trong quá khứ!');
        return;
      }

      const response = await fetch('https://localhost:8386/api/SubstituteTeachings', {
        method: 'POST',
        headers: {
          'accept': '*/*',
          'Authorization': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIzIiwiZW1haWwiOiJ0cmFudGhpYmluaEBoZ3NkYi5lZHUudm4iLCJuYW1lIjoidHJhbnRoaWJpbmgiLCJyb2xlIjoiR2nDoW8gdmnDqm4iLCJqdGkiOiI5OWU4OWVjMC1mZTI3LTQ1NjEtYTQ5Zi01NTUwZDA2OWZiMmMiLCJ0ZWFjaGVySWQiOiIyIiwiZXhwIjoxNzQ1NTA5MjQwLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo4Mzg2IiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6ODM4NiJ9.xBAmKzkZ20TqHtN9oQv3IvLqgE6t_-VPspwwGoH_-EI',
          'Content-Type': 'application/json;odata.metadata=minimal;odata.streaming=true',
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const errorText = await response.text();
        console.error('Server response:', errorText);
        throw new Error(`HTTP error! status: ${response.status}, message: ${errorText}`);
      }

      toast.success('Phân công giáo viên dạy thay thành công!')
      message.success('Phân công giáo viên dạy thay thành công!');
    } catch (error) {
      toast.error(`Giáo viên dạy thay không được trùng với giáo viên xin nghỉ`)
      console.error('Error saving assignment:', error);
      message.error(`Có lỗi khi lưu phân công: ${error.message}`);
    }
  };

  // Process schedules for display
  useEffect(() => {
    if (!teacherSchedule?.[0]?.details || !leaveRequest) return;

    const scheduleDetails = teacherSchedule[0].details;
    const startDate = dayjs(leaveRequest.leaveFromDate);
    const endDate = dayjs(leaveRequest.leaveToDate);
    const currentDate = dayjs().startOf('day');
    const schedules = [];

    let currentDateIterator = startDate;
    while (currentDateIterator.isSame(endDate, 'day') || currentDateIterator.isBefore(endDate, 'day')) {
      if (currentDateIterator.isBefore(currentDate)) {
        currentDateIterator = currentDateIterator.add(1, 'day');
        continue;
      }

      const weekday = getWeekdayName(currentDateIterator);
      const daySchedules = scheduleDetails.filter((schedule) => schedule.dayOfWeek === weekday);

      daySchedules.forEach((schedule) => {
        schedules.push({
          scheduleId: `${currentDateIterator.format('YYYY-MM-DD')}-${schedule.timetableDetailId}`,
          date: currentDateIterator.format('YYYY-MM-DD'),
          dayOfWeek: weekday,
          period: schedule.periodName,
          className: schedule.className,
          subject: schedule.subjectName,
          timetableDetailId: schedule.timetableDetailId,
        });
      });

      currentDateIterator = currentDateIterator.add(1, 'day');
    }

    setFilteredSchedules(schedules);
  }, [teacherSchedule, leaveRequest]);

  // Handle bulk assignment submission
  const handleAssignSubstitute = async (values) => {
    setLoading(true);
    try {
      const assignments = values.assignments || {};
      const notes = values.notes || {};

      await Promise.all(
        filteredSchedules.map(async (schedule) => {
          const teacherId = assignments[schedule.scheduleId];
          const note = notes[schedule.scheduleId];
          if (teacherId) {
            await handleSaveAssignment(schedule, teacherId, note);
          }
        })
      );

      toast.success('Lưu thành công')
      message.success('Đã lưu tất cả phân công thành công!');
      form.resetFields(['assignments', 'notes']);
    } catch (error) {
      message.error('Có lỗi xảy ra khi lưu phân công');
    } finally {
      setLoading(false);
    }
  };

  // Table columns configuration
  const columns = [
    {
      title: 'Ngày',
      dataIndex: 'date',
      key: 'date',
      render: (text, record) => `${dayjs(text).format('DD/MM/YYYY')} (${record.dayOfWeek})`,
    },
    { title: 'Tiết học', dataIndex: 'period', key: 'period' },
    { title: 'Lớp', dataIndex: 'className', key: 'className' },
    { title: 'Môn học', dataIndex: 'subject', key: 'subject' },
    {
      title: 'Giáo viên dạy thay',
      key: 'substituteTeacher',
      render: (_, record) => (
        <Form.Item
          name={['assignments', record.scheduleId]}
          style={{ margin: 0 }}
          rules={[{ required: true, message: 'Vui lòng chọn giáo viên!' }]}
        >
          <Select placeholder="Chọn giáo viên dạy thay" loading={teachersLoading}>
            {teachersData?.teachers?.map((teacher) => (
              <Option key={teacher.teacherId} value={teacher.teacherId}>
                {teacher.fullName} - {teacher.teacherId}
              </Option>
            ))}
          </Select>
        </Form.Item>
      ),
    },
    {
      title: 'Ghi chú',
      key: 'note',
      render: (_, record) => (
        <Form.Item name={['notes', record.scheduleId]} style={{ margin: 0 }}>
          <Input.TextArea placeholder="Nhập ghi chú" autoSize={{ minRows: 1, maxRows: 3 }} />
        </Form.Item>
      ),
    },
    {
      title: 'Lưu',
      key: 'save',
      render: (_, record) => (
        <Button
          type="primary"
          size="small"
          loading={loading}
          onClick={async () => {
            const teacherId = form.getFieldValue(['assignments', record.scheduleId]);
            const note = form.getFieldValue(['notes', record.scheduleId]);
            if (!teacherId) {
              message.error('Vui lòng chọn giáo viên trước khi lưu!');
              return;
            }
            await handleSaveAssignment(record, teacherId, note);
          }}
        >
          Lưu
        </Button>
      ),
    },
  ];

  return (
    <div>
      <Card title="Thông tin lịch dạy cần thay thế" style={{ marginBottom: 16 }}>
        <p>
          <strong>Thời gian nghỉ: </strong>
          {dayjs(leaveRequest?.leaveFromDate).format('DD/MM/YYYY')} ({getWeekdayName(leaveRequest?.leaveFromDate)}) -{' '}
          {dayjs(leaveRequest?.leaveToDate).format('DD/MM/YYYY')} ({getWeekdayName(leaveRequest?.leaveToDate)})
        </p>
        {dayjs(leaveRequest?.leaveFromDate).isBefore(dayjs().startOf('day')) && (
          <p style={{ color: 'red' }}>
            <strong>Lưu ý:</strong> Một số ngày trong thời gian nghỉ đã qua, chỉ có thể phân công từ hôm nay trở đi.
          </p>
        )}
      </Card>

      <Card title="Phân công giáo viên dạy thay">
        <Form form={form} onFinish={handleAssignSubstitute}>
          <Table
            dataSource={filteredSchedules}
            columns={columns}
            rowKey="scheduleId"
            pagination={false}
            loading={scheduleLoading}
          />
        </Form>
      </Card>
    </div>
  );
};

export default SubstituteTeacherAssignment;