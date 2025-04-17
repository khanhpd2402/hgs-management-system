import React, { useState, useEffect } from 'react';
import { useScheduleTeacher } from '../../../services/schedule/queries';
import './ScheduleTeacher.scss';

const ScheduleTeacher = () => {
    const [teacherId, setTeacherId] = useState(null);
    const [effectiveDate, setEffectiveDate] = useState('');

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
        { name: 'Chiều', periods: [1, 2, 3] },
    ];

    const getSchedule = (day, shift, period) => {
        if (!scheduleData?.[0]?.details) return '';
        
        const schedule = scheduleData[0].details.find(
            (item) =>
                item.dayOfWeek === day &&
                item.periodId === period
        );

        if (schedule) {
            return `${schedule.className} - ${schedule.subjectName}`;
        }
        return '';
    };

    if (isLoading) {
        return <div>Đang tải...</div>;
    }

    const currentSchedule = scheduleData?.[0];

    return (
        <div className="schedule-teacher-container">
          

            {currentSchedule && (
                <div style={{ marginBottom: '20px' }}>
                    <h2>Thời Khóa Biểu Trong Tuần</h2>
                    <p><strong>Học kỳ:</strong> {currentSchedule.semesterId}</p>
                    <p><strong>Thời gian áp dụng:</strong> {new Date(currentSchedule.effectiveDate).toLocaleDateString('vi-VN')} - {new Date(currentSchedule.endDate).toLocaleDateString('vi-VN')}</p>
                    <p><strong>Trạng thái:</strong> {currentSchedule.status}</p>
                </div>
            )}

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
                                {daysOfWeek.map((day, idx) => (
                                    <td key={idx} className={idx % 2 === 0 ? 'even-column' : 'odd-column'}>
                                        {getSchedule(day, shift.name, period)}
                                    </td>
                                ))}
                            </tr>
                        ))
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default ScheduleTeacher;