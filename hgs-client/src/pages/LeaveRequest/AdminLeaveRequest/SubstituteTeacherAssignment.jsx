import React, { useState, useEffect } from 'react';
import { Card, Table, Form, Select, Button, message, Input } from 'antd';
import dayjs from 'dayjs';
import axios from 'axios';
import { useScheduleTeacher } from '../../../services/schedule/queries';
import { useTeachers } from '../../../services/teacher/queries';
import { useSubstituteTeacher } from '../../../services/leaveRequest/mutation'
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
  const { data: teachersData, isLoading: teachersLoading } = useTeachers();
  const [filteredSchedules, setFilteredSchedules] = useState([]);
  const substituteTeacherMutation = useSubstituteTeacher();

  const handleSaveAssignment = async (record, teacherId, note) => {
    try {
      const payload = {
        timetableDetailId: record.timetableDetailId,
        originalTeacherId: leaveRequest.teacherId,
        substituteTeacherId: teacherId,
        date: record.date,
        note: note || ''
      };

      await substituteTeacherMutation.mutateAsync(payload);
      message.success('Đã lưu phân công cho tiết học này!');
    } catch (error) {
      console.error('Error saving assignment:', error);
      message.error('Lỗi khi lưu phân công: ' + (error.response?.data?.message || error.message));
    }
  };


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


  const handleAssignSubstitute = async (values) => {
    try {
      setLoading(true);
      const assignments = values.assignments || {};
      const notes = values.notes || {};

      // Save assignments one by one
      for (const schedule of filteredSchedules) {
        const teacherId = assignments[schedule.scheduleId];
        const note = notes[schedule.scheduleId];

        if (teacherId) {
          await handleSaveAssignment(schedule, teacherId, note);
        }
      }

      message.success('Đã lưu tất cả phân công thành công!');
      form.resetFields(['assignments', 'notes']);
    } catch (error) {
      message.error('Có lỗi xảy ra khi lưu phân công');
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
            loading={teachersLoading}
          >
            {teachersData?.teachers?.map(teacher => (
              <Option
                key={teacher.teacherId}
                value={teacher.teacherId}
              >
                {teacher.fullName} - {teacher.teacherId}
              </Option>
            ))}
          </Select>
        </Form.Item>
      )
    },
    {
      title: 'Ghi chú',
      key: 'note',
      render: (_, record) => (
        <Form.Item
          name={['notes', record.scheduleId]}
          style={{ margin: 0 }}
        >
          <Input.TextArea
            placeholder="Nhập ghi chú"
            autoSize={{ minRows: 1, maxRows: 3 }}
          />
        </Form.Item>
      )
    },
    {
      title: 'Lưu',
      key: 'save',
      render: (_, record) => (
        <Button
          type="primary"
          size="small"
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
          {/* <Form.Item style={{ marginTop: 16, textAlign: 'right' }}>
            <Button type="primary" htmlType="submit" loading={loading}>
              Lưu phân công
            </Button>
          </Form.Item> */}
        </Form>
      </Card>
    </div>
  );
};

export default SubstituteTeacherAssignment;
