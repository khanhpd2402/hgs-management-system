import React, { useState } from 'react';
import { useScheduleStudent } from '../../../services/schedule/queries';
import '../ScheduleTeacher/ScheduleTeacher.scss';

const ScheduleStudent = () => {
    const [studentId] = useState(646);
    const [semesterId, setSemesterId] = useState(1);

    const { data: scheduleData, isLoading } = useScheduleStudent(studentId, semesterId);

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
        { name: 'Chiều', periods: [6, 7, 8] }
    ];

    const getSchedule = (day, periodId) => {
        if (!scheduleData?.[0]?.details) return null;
        
        const schedule = scheduleData[0].details.find(
            (item) =>
                item.dayOfWeek === day &&
                item.periodId === periodId
        );

        if (schedule) {
            return (
                <div className="schedule-cell-content">
                    <strong>{schedule.subjectName}</strong>
                    <div>GV: {schedule.teacherName}</div>
                </div>
            );
        }
        return null;
    };

    const getPeriodName = (periodId) => {
        if (!scheduleData?.[0]?.details) return `Tiết ${periodId}`;
        
        const period = scheduleData[0].details.find(
            (item) => item.periodId === periodId
        );
        
        return period?.periodName || `Tiết ${periodId}`;
    };

    if (isLoading) {
        return (
            <div className="schedule-teacher-container">
                <div className="loading-container">
                    <div className="loading-text">Đang tải...</div>
                </div>
            </div>
        );
    }

    const currentSchedule = scheduleData?.[0];

    return (
        <div className="schedule-teacher-container">
            <div className="filter-section">
                <div className="filter-row">
                    <div className="filter-item">
                        <label>Học kỳ</label>
                        <select 
                            value={semesterId} 
                            onChange={(e) => setSemesterId(Number(e.target.value))}
                            className="semester-select"
                        >
                            <option value={1}>Học kỳ 1</option>
                            <option value={2}>Học kỳ 2</option>
                        </select>
                    </div>
                </div>
            </div>

            {currentSchedule && (
                <div className="schedule-header">
                    <h2>Thời Khóa Biểu Lớp {currentSchedule.details?.[0]?.className}</h2>
                    <div className="schedule-info">
                        <p><strong>Học kỳ:</strong> {currentSchedule.semesterId}</p>
                        <p><strong>Thời gian áp dụng:</strong> {new Date(currentSchedule.effectiveDate).toLocaleDateString('vi-VN')} - {new Date(currentSchedule.endDate).toLocaleDateString('vi-VN')}</p>
                        <p><strong>Trạng thái:</strong> {currentSchedule.status}</p>
                    </div>
                </div>
            )}

            <table className="schedule-teacher-table">
                <thead>
                    <tr>
                        <th>Buổi</th>
                        <th>Tiết</th>
                        {daysOfWeek.map((day, index) => (
                            <th
                                key={`header-${day}-${index}`}
                                className={index % 2 === 0 ? 'even-column' : 'odd-column'}
                            >
                                {day}
                            </th>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {shifts.map((shift) =>
                        shift.periods.map((period, periodIndex) => (
                            <tr key={`${shift.name}-${period}-${periodIndex}`}>
                                {periodIndex === 0 && (
                                    <td rowSpan={shift.periods.length} className="shift-cell">
                                        {shift.name}
                                    </td>
                                )}
                                <td className="period-cell">{getPeriodName(period)}</td>
                                {daysOfWeek.map((day, dayIndex) => (
                                    <td 
                                        key={`${day}-${period}-${dayIndex}`}
                                        className={`schedule-cell ${dayIndex % 2 === 0 ? 'even-column' : 'odd-column'}`}
                                    >
                                        {getSchedule(day, period)}
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

export default ScheduleStudent;
