import React, { useState } from 'react';
import { scheduleData } from './data';
import './ScheduleTeacher.scss';

const ScheduleTeacher = () => {
    const [selectedTeacher, setSelectedTeacher] = useState('');
    const [effectiveDate, setEffectiveDate] = useState('');

    const teachers = [...new Set(scheduleData.map(item => item.teacherName))];

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
        const schedule = scheduleData.find(
            (item) =>
                item.dayOfWeek === day &&
                item.shift === shift &&
                item.period === period &&
                (selectedTeacher ? item.teacherName === selectedTeacher : true)
        );
        return schedule ? `${schedule.className} - ${schedule.subjectName}` : '';
    };

    return (
        <div className="schedule-teacher-container">
            <div className="filter-section">
                <div className="filter-row">
                    <div className="filter-item">
                        <label>Giáo viên</label>
                        <select
                            value={selectedTeacher}
                            onChange={(e) => setSelectedTeacher(e.target.value)}
                        >
                            <option value="">-- Lựa chọn --</option>
                            {teachers.map((teacher, index) => (
                                <option key={index} value={teacher}>
                                    {teacher}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className="filter-item">
                        <label>Ngày áp dụng</label>
                        <input
                            type="date"
                            value={effectiveDate}
                            onChange={(e) => setEffectiveDate(e.target.value)}
                        />
                    </div>
                </div>

                <div className="search-button-container">
                    <button style={{ width: "150px" }} className="search-button">Tìm kiếm</button>
                </div>
            </div>

            <h2>Thời Khóa Biểu Trong Tuần</h2>
            <table className="schedule-teacher-table">
                <thead>
                    <tr>
                        <th>Buổi</th>
                        <th>Tiết</th>
                        {daysOfWeek.map((day, index) => (
                            <th
                                style={{ backgroundColor: '#727cf5' }}
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