import React, { useState, useEffect } from 'react';
import { useScheduleTeacher, useGetTimetiableSubstituteSubstituteForTeacher } from '../../../services/schedule/queries';
import { DatePicker } from 'antd';
import dayjs from 'dayjs';
import './ScheduleTeacher.scss';

const ScheduleTeacher = () => {
    const [teacherId, setTeacherId] = useState(null);
    const [selectedDate, setSelectedDate] = useState(dayjs());
    const [weekDays, setWeekDays] = useState(() => {
        const startOfWeek = dayjs().startOf('week');
        return Array.from({ length: 7 }, (_, i) => startOfWeek.add(i, 'day'));
    });

    // Update weekDays when selectedDate changes
    useEffect(() => {
        const effectiveDate = selectedDate || dayjs(); // Default to current date if null
        const startOfWeek = effectiveDate.startOf('week');
        const days = Array.from({ length: 7 }, (_, i) => startOfWeek.add(i, 'day'));
        setWeekDays(days);
    }, [selectedDate]);

    // Extract teacherId from JWT token
    useEffect(() => {
        const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
        if (token) {
            const tokenParts = token.split('.');
            if (tokenParts.length === 3) {
                const payload = JSON.parse(atob(tokenParts[1]));
                setTeacherId(payload.teacherId);
            }
        }
    }, []);

    const { data: scheduleData, isLoading } = useScheduleTeacher(teacherId);

    // Fetch substitute data for each day of the week (exactly 7 queries)
    const queries = [
        useGetTimetiableSubstituteSubstituteForTeacher(teacherId, weekDays[0], {
            skip: !teacherId || !weekDays[0],
        }),
        useGetTimetiableSubstituteSubstituteForTeacher(teacherId, weekDays[1], {
            skip: !teacherId || !weekDays[1],
        }),
        useGetTimetiableSubstituteSubstituteForTeacher(teacherId, weekDays[2], {
            skip: !teacherId || !weekDays[2],
        }),
        useGetTimetiableSubstituteSubstituteForTeacher(teacherId, weekDays[3], {
            skip: !teacherId || !weekDays[3],
        }),
        useGetTimetiableSubstituteSubstituteForTeacher(teacherId, weekDays[4], {
            skip: !teacherId || !weekDays[4],
        }),
        useGetTimetiableSubstituteSubstituteForTeacher(teacherId, weekDays[5], {
            skip: !teacherId || !weekDays[5],
        }),
        useGetTimetiableSubstituteSubstituteForTeacher(teacherId, weekDays[6], {
            skip: !teacherId || !weekDays[6],
        }),
    ];

    // Aggregate substitute data
    const substituteData = queries
        .flatMap((query) => query.data || [])
        .filter(Boolean);

    const daysOfWeek = [
        'Thứ Hai',
        'Thứ Ba',
        'Thứ Tư',
        'Thứ Năm',
        'Thứ Sáu',
        'Thứ Bảy',
        'Chủ Nhật',
    ];

    const shifts = [
        { name: 'Sáng', periods: [1, 2, 3, 4, 5] },
        { name: 'Chiều', periods: [6, 7, 8] },
    ];

    const getSchedule = (day, shift, period) => {
        if (!scheduleData?.[0]?.details) return '';

        // Find the corresponding date for the day of the week
        const dayIndex = daysOfWeek.indexOf(day);
        const targetDate = weekDays[dayIndex];

        // Check for substitute class on the specific date and period
        const substituteClass = substituteData.find(
            (sub) =>
                dayjs(sub.date).isSame(targetDate, 'day') && sub.periodId === period
        );

        if (substituteClass) {
            return {
                content: `${substituteClass.className} - ${substituteClass.subjectName}\n(Dạy thay)`,
                isSubstitute: true,
            };
        }

        const schedule = scheduleData[0].details.find(
            (item) =>
                item.dayOfWeek === day &&
                item.periodId === period
        );

        if (schedule) {
            return {
                content: `${schedule.className} - ${schedule.subjectName}`,
                isSubstitute: false,
            };
        }
        return '';
    };

    if (isLoading) {
        return <div>Đang tải...</div>;
    }

    const currentSchedule = scheduleData?.[0];

    return (
        <div className="schedule-teacher-container">
            <div className="schedule-header">
                <div className="date-picker-section">
                    <label>Chọn ngày:</label>
                    <DatePicker
                        value={selectedDate}
                        onChange={(date) => setSelectedDate(date || dayjs())} // Default to current date if null
                        format="DD/MM/YYYY"
                        placeholder="Chọn ngày"
                    />
                </div>

                {currentSchedule && (
                    <div className="schedule-info">
                        <h2>Thời Khóa Biểu Trong Tuần</h2>
                        <p><strong>Học kỳ:</strong> {currentSchedule.semesterId}</p>
                        <p>
                            <strong>Thời gian áp dụng:</strong>{' '}
                            {new Date(currentSchedule.effectiveDate).toLocaleDateString('vi-VN')} -{' '}
                            {new Date(currentSchedule.endDate).toLocaleDateString('vi-VN')}
                        </p>
                        <p><strong>Trạng thái:</strong> {currentSchedule.status}</p>
                    </div>
                )}
            </div>

            <table className="schedule-teacher-table">
                <thead>
                    <tr>
                        <th>Buổi</th>
                        <th>Tiết</th>
                        {daysOfWeek.map((day, index) => (
                            <th
                                style={{ backgroundColor: '#727cf5', color: 'white' }}
                                key={index}
                                className={index % 2 === 0 ? 'even-column' : 'odd-column'}
                            >
                                {day}
                            </th>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {shifts.map((shift) =>
                        shift.periods.map((period, index) => (
                            <tr key={`${shift.name}-${period}`}>
                                {index === 0 && (
                                    <td rowSpan={shift.periods.length} className="shift-cell">
                                        {shift.name}
                                    </td>
                                )}
                                <td>{period}</td>
                                {daysOfWeek.map((day, idx) => {
                                    const scheduleInfo = getSchedule(day, shift.name, period);
                                    return (
                                        <td
                                            key={idx}
                                            className={`${idx % 2 === 0 ? 'even-column' : 'odd-column'} ${scheduleInfo.isSubstitute ? 'substitute-class' : ''
                                                }`}
                                        >
                                            {scheduleInfo.content || ''}
                                        </td>
                                    );
                                })}
                            </tr>
                        ))
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default ScheduleTeacher;