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
    useEffect(() => {
        if (teacherSchedule && leaveRequest?.leaveFromDate && leaveRequest?.leaveToDate) {
            const startDate = dayjs(leaveRequest.leaveFromDate);
            const endDate = dayjs(leaveRequest.leaveToDate);
            const processedSchedules = [];

            let currentDate = startDate;
            while (currentDate.isSame(endDate) || currentDate.isBefore(endDate)) {
                const weekday = getWeekdayName(currentDate);
                const daySchedules = teacherSchedule.filter(schedule => schedule.dayOfWeek === weekday);

                daySchedules.forEach(schedule => {
                    processedSchedules.push({
                        scheduleId: `${currentDate.format('YYYY-MM-DD')}-${schedule.timetableDetailId}`,
                        date: currentDate.format('YYYY-MM-DD'),
                        dayOfWeek: schedule.dayOfWeek,
                        period: schedule.periodName,
                        className: schedule.className,
                        subject: schedule.subjectName,
                        timetableDetailId: schedule.timetableDetailId
                    });
                });

                currentDate = currentDate.add(1, 'day');
            }

            setFilteredSchedules(processedSchedules);
        }
    }, [teacherSchedule, leaveRequest?.leaveFromDate, leaveRequest?.leaveToDate]);

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
                <p><strong>Giáo viên nghỉ:</strong> {leaveRequest?.teacherId}</p>
                <p>
                    <strong>Thời gian nghỉ:</strong> {dayjs(leaveRequest?.leaveFromDate).format('DD/MM/YYYY')} ({getWeekdayName(leaveRequest?.leaveFromDate)})
                    - {dayjs(leaveRequest?.leaveToDate).format('DD/MM/YYYY')} ({getWeekdayName(leaveRequest?.leaveToDate)})
                </p>
                <div style={{ marginTop: 8 }}>
                    <strong>Các ngày nghỉ:</strong>
                    <ul style={{ marginTop: 4, marginBottom: 0 }}>
                        {(() => {
                            const days = [];
                            let currentDate = dayjs(leaveRequest?.leaveFromDate);
                            const endDate = dayjs(leaveRequest?.leaveToDate);

                            while (currentDate.isSame(endDate) || currentDate.isBefore(endDate)) {
                                days.push(
                                    <li key={currentDate.format('YYYY-MM-DD')}>
                                        {currentDate.format('DD/MM/YYYY')} ({getWeekdayName(currentDate)})
                                    </li>
                                );
                                currentDate = currentDate.add(1, 'day');
                            }

                            return days;
                        })()}
                    </ul>
                </div>
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
