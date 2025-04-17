import React, { useState } from 'react';
import { useTimetablesForPrincipal } from '../../../services/schedule/queries';
import { useTeachers } from '../../../services/teacher/queries';
import { useSubjects } from '@/services/common/queries';
import './Schedule.scss';

const Schedule = () => {
    // Add daysOfWeek array at the top
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

    const formatDate = (dateString) => {
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    };

    const [selectedGrade, setSelectedGrade] = useState('');
    const [selectedClass, setSelectedClass] = useState('');

    // Add new state variables
    const [tempTeacher, setTempTeacher] = useState('');
    const [tempGrade, setTempGrade] = useState('');
    const [tempClass, setTempClass] = useState('');
    const [tempSubject, setTempSubject] = useState('');
    const [tempSession, setTempSession] = useState('');

    // Add dummy data (replace with actual data from your API)
    // Remove this line since we're getting teacherData from useTeachers hook
    // const teacherData = []; // Remove this line
    const subjectData = []; // Replace with actual subject data
    const grades = ['6', '7', '8', '9']; // Replace with actual grades

    // Add handler functions
    // Add filtered state
    const [filteredSchedule, setFilteredSchedule] = useState(null);

    const handleSearch = () => {
        if (!scheduleData?.[0]?.details) return;

        let filtered = scheduleData[0].details;

        if (tempTeacher) {
            filtered = filtered.filter(item => {
                const teacherIdFromFilter = parseInt(tempTeacher);
                const teacherIdFromItem = parseInt(item.teacherId);
                return teacherIdFromItem === teacherIdFromFilter;
            });
        }

        if (tempGrade) {
            filtered = filtered.filter(item => {
                const classGrade = item.className.charAt(0);
                return classGrade === tempGrade;
            });
        }

        if (tempClass) {
            filtered = filtered.filter(item => item.className === tempClass);
        }

        if (tempSubject) {
            filtered = filtered.filter(item => {
                const subjectIdFromFilter = parseInt(tempSubject);
                const subjectIdFromItem = parseInt(item.subjectId);
                return subjectIdFromItem === subjectIdFromFilter;
            });
        }

        if (tempSession) {
            const morningPeriods = [1, 2, 3, 4, 5];
            const afternoonPeriods = [6, 7, 8];
            filtered = filtered.filter(item => {
                const periodId = parseInt(item.periodId);
                return tempSession === 'Morning' 
                    ? morningPeriods.includes(periodId)
                    : afternoonPeriods.includes(periodId);
            });
        }

        setFilteredSchedule(filtered.length > 0 ? {
            ...scheduleData[0],
            details: filtered,
            selectedSession: tempSession
        } : null);
    };

    // Update handleReset function
    const handleReset = () => {
        setTempTeacher('');
        setTempGrade('');
        setTempClass('');
        setTempSubject('');
        setTempSession('');
        setFilteredSchedule(null);
    };

    // Update getSchedule function
    const getSchedule = (day, periodId, className) => {
        const scheduleToUse = filteredSchedule || scheduleData?.[0];
        if (!scheduleToUse?.details) return null;

        const schedule = scheduleToUse.details.find(
            item =>
                item.dayOfWeek === day &&
                item.periodId === periodId &&
                item.className === className
        );

        if (schedule) {
            return (
                <div className="schedule-cell">
                    <div className="subject">{schedule.subjectName}</div>
                    <div className="teacher">{schedule.teacherName}</div>
                </div>
            );
        }
        return null;
    };

    // Update the hook destructuring to include isLoading
    const { data: scheduleData, isLoading: scheduleLoading } = useTimetablesForPrincipal(1);
    const { data: teachersResponse = { teachers: [] }, isLoading: teachersLoading } = useTeachers();
    const { data: subjects = [], isLoading: subjectsLoading } = useSubjects();

    const teachers = Array.isArray(teachersResponse) ? teachersResponse : teachersResponse.teachers || [];

    // Update the loading check
    if (scheduleLoading || teachersLoading || subjectsLoading) {
        return <div className="loading">Đang tải...</div>;
    }

    const currentSchedule = scheduleData?.[0];

    // Move getUniqueClasses function before its usage
    const getUniqueClasses = () => {
        if (!scheduleData?.[0]?.details) return [];
        const classes = scheduleData[0].details.map(detail => detail.className);
        return [...new Set(classes)].sort();
    };
    const getFilteredClasses = () => {
        if (!scheduleData?.[0]?.details) return [];
        const allClasses = [...new Set(scheduleData[0].details.map(detail => detail.className))].sort();

        if (tempGrade) {
            return allClasses.filter(className => className.startsWith(tempGrade));
        }
        return allClasses;
    };

    const classes = getUniqueClasses();

    const getPeriodName = (periodId) => {
        if (!scheduleData?.[0]?.details) return `Tiết ${periodId}`;
        const period = scheduleData[0].details.find(item => item.periodId === periodId);
        return period?.periodName || `Tiết ${periodId}`;
    };

    const getClassesByGrade = () => {
        if (!classes.length) return {};
        return classes.reduce((acc, className) => {
            const grade = className.charAt(0);
            if (!acc[grade]) acc[grade] = [];
            acc[grade].push(className);
            return acc;
        }, {});
    };

    // Modify the shifts based on tempSession
    const getFilteredShifts = () => {
        if (!filteredSchedule?.selectedSession) return shifts;
        return shifts.filter(shift =>
            (filteredSchedule.selectedSession === 'Morning' && shift.name === 'Sáng') ||
            (filteredSchedule.selectedSession === 'Afternoon' && shift.name === 'Chiều')
        );
    };



    return (
        <div className="schedule-container">


            {/* Add the new filter container here */}
            <div className="filter-container">
                {/* Row 1: Semester, Application Date, Teacher */}
                <div className="filter-row">
                    <div className="filter-column">
                        <label>Học kỳ</label>
                        <input
                            type="text"
                            value={scheduleData?.[0]?.semesterId === 1 ? "Học kỳ I" : "Học kỳ II"}
                            readOnly
                        />
                    </div>

                    <div className="filter-column">
                        <label>Ngày áp dụng</label>
                        <input
                            type="text"
                            value={
                                scheduleData?.[0]?.effectiveDate && scheduleData?.[0]?.endDate
                                    ? `Từ ${formatDate(scheduleData[0].effectiveDate)} đến ${formatDate(scheduleData[0].endDate)}`
                                    : ""
                            } readOnly
                        />
                    </div>



                    <div className="filter-column">
                        <label>Giáo viên</label>
                        <select onChange={(e) => setTempTeacher(e.target.value)} value={tempTeacher}>
                            <option value="">Chọn giáo viên</option>
                            {teachers && teachers.map((teacher) => (
                                <option key={teacher.teacherId} value={teacher.teacherId}>
                                    {teacher.fullName}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>

                {/* Row 2: Grade, Class, Subject */}
                <div className="filter-row">
                    <div className="filter-column">
                        <label>Khối</label>
                        <select onChange={(e) => setTempGrade(e.target.value)} value={tempGrade}>
                            <option value="">-- Lựa chọn --</option>
                            {grades.map((grade) => (
                                <option key={grade} value={grade}>Khối {grade}</option>
                            ))}
                        </select>
                    </div>

                    <div className="filter-column">
                        <label>Lớp</label>
                        <select
                            onChange={(e) => setTempClass(e.target.value)}
                            value={tempClass}
                            disabled={!tempGrade}
                        >
                            <option value="">-- Lựa chọn --</option>
                            {getFilteredClasses().map(className => (
                                <option key={className} value={className}>{className}</option>
                            ))}
                        </select>
                    </div>

                    <div className="filter-column">
                        <label>Môn học</label>
                        <select onChange={(e) => setTempSubject(e.target.value)} value={tempSubject}>
                            <option value="">-- Lựa chọn --</option>
                            {subjects && subjects.map((subject) => (
                                <option key={subject.subjectId} value={subject.subjectId}>
                                    {subject.subjectName}
                                </option>
                            ))}
                        </select>
                    </div>
                </div>

                {/* Row 3: Session selection */}
                <div className="filter-row">
                    <div className="filter-column">
                        <label>Chọn buổi</label>
                        <select onChange={(e) => setTempSession(e.target.value)} value={tempSession}>
                            <option value="">Chọn buổi</option>
                            <option value="Morning">Sáng</option>
                            <option value="Afternoon">Chiều</option>
                        </select>
                    </div>
                </div>

                {/* Search and reset buttons */}
                <div className="filter-row">
                    <div className="filter-column search-button">
                        <button onClick={handleSearch}>Tìm kiếm</button>
                        <button onClick={handleReset} className="reset-button">Reset</button>
                    </div>
                </div>
            </div>

            <div className="table-container">
                <table className="schedule-table">
                    <thead>
                        <tr>
                            <th className="sticky-header col-1" colSpan="3">Lịch học</th>
                            {!selectedClass && Object.entries(getClassesByGrade()).map(([grade, gradeClasses]) => (
                                <th key={grade} colSpan={gradeClasses.length}>Khối {grade}</th>
                            ))}
                            {selectedClass && <th>Khối {selectedClass.charAt(0)}</th>}
                        </tr>
                        <tr>
                            <th className="sticky-col col-1">Thứ</th>
                            <th className="sticky-col col-2">Buổi</th>
                            <th className="sticky-col col-3">Tiết</th>
                            {selectedClass ? [selectedClass] : classes.map(className => (
                                <th key={className}>{className}</th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {daysOfWeek.map((day, dayIndex) => {
                            const shiftsToShow = getFilteredShifts();
                            const totalPeriods = shiftsToShow.reduce((sum, shift) => sum + shift.periods.length, 0);
                            return shiftsToShow.map((shift, shiftIndex) =>
                                shift.periods.map((period, periodIndex) => (
                                    <tr
                                        key={`${day}-${shift.name}-${period}`}
                                        className={dayIndex % 2 === 0 ? 'even-day' : 'odd-day'}
                                    >
                                        {shiftIndex === 0 && periodIndex === 0 && (
                                            <td className="sticky-col col-1" rowSpan={totalPeriods}>{day}</td>
                                        )}
                                        {periodIndex === 0 && (
                                            <td className="sticky-col col-2" rowSpan={shift.periods.length}>{shift.name}</td>
                                        )}
                                        <td className="sticky-col col-3">Tiết {period}</td>
                                        {(selectedClass ? [selectedClass] : classes).map(className => (
                                            <td key={className}>
                                                {getSchedule(day, period, className)}
                                            </td>
                                        ))}
                                    </tr>
                                ))
                            );
                        })}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default Schedule;
