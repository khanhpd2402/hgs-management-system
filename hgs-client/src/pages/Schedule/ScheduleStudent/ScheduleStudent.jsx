import React, { useState, useEffect } from 'react';
import { useScheduleStudent, useGetStudentNameAndClass } from '../../../services/schedule/queries';
import { Select } from 'antd';
import { useAcademicYears } from '../../../services/common/queries';
import { getStudentNameAndClass, getSemesterByYear } from '../../../services/schedule/api';
import '../ScheduleTeacher/ScheduleTeacher.scss';

const ScheduleStudent = () => {
    const [studentId, setStudentId] = useState(null);
    const [academicYearId, setAcademicYearId] = useState(null);
    const [semesterId, setSemesterId] = useState(null);
    const [studentList, setStudentList] = useState([]);
    const [selectedStudent, setSelectedStudent] = useState(null);
    const [semesters, setSemesters] = useState([]);

    const { data: academicYears, isLoading: academicYearsLoading } = useAcademicYears();
    const { data: scheduleData, isLoading } = useScheduleStudent(studentId, semesterId);

    // Lấy danh sách học kỳ khi năm học thay đổi
    useEffect(() => {
        const fetchSemesters = async () => {
            if (academicYearId) {
                try {
                    const semesterData = await getSemesterByYear(academicYearId);
                    setSemesters(semesterData || []);
                    setSemesterId(semesterData[0]?.semesterID || null);
                } catch (error) {
                    console.error('Lỗi khi lấy dữ liệu học kỳ:', error);
                    setSemesters([]);
                    setSemesterId(null);
                }
            } else {
                setSemesters([]);
                setSemesterId(null);
            }
        };
        fetchSemesters();
    }, [academicYearId]);

    // Lấy danh sách học sinh khi component mount
    useEffect(() => {
        const fetchStudents = async () => {
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            if (!token || !academicYearId) {
                console.warn('Token or academicYearId is missing');
                setStudentList([]);
                setStudentId(null);
                setSelectedStudent(null);
                return;
            }

            try {
                const tokenParts = token.split('.');
                if (tokenParts.length !== 3) {
                    throw new Error('Invalid token format');
                }

                const payload = JSON.parse(atob(tokenParts[1]));
                if (!payload.studentIds) {
                    console.warn('studentIds not found in token payload');
                    setStudentList([]);
                    setStudentId(null);
                    setSelectedStudent(null);
                    return;
                }
                console.log("paload", payload)

                const studentIdList = payload.studentIds.split(',');

                const students = await Promise.all(
                    studentIdList.map(id =>
                        getStudentNameAndClass(id, academicYearId)
                    )
                );

                const formattedStudents = students.map(student => ({
                    value: student.studentId,
                    label: `${student.fullName} - Lớp ${student.className}`,
                    studentInfo: student
                }));

                setStudentList(formattedStudents);
                setStudentId(formattedStudents[0]?.value || null);
                setSelectedStudent(formattedStudents[0]?.studentInfo || null);
            } catch (error) {
                console.error('Error fetching student details:', error);
                setStudentList([]);
                setStudentId(null);
                setSelectedStudent(null);
            }
        };

        fetchStudents();
    }, [academicYearId]);

    // Cập nhật thông tin học sinh từ danh sách đã lưu
    const handleStudentChange = (value) => {
        setStudentId(value);
        const selectedStudentInfo = studentList.find(student => student.value === value)?.studentInfo;
        setSelectedStudent(selectedStudentInfo);
    };

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

    if (isLoading || academicYearsLoading) {
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
                        <label>Năm học</label>
                        <Select
                            value={academicYearId}
                            onChange={(value) => setAcademicYearId(value)}
                            options={academicYears?.map(year => ({
                                value: year.academicYearID,
                                label: `${year.yearName} -- ${year.academicYearID}`
                            })) || []}
                            style={{ width: 300 }}
                            placeholder="Chọn năm học"
                            disabled={academicYearsLoading}
                        />
                    </div>
                    <div className="filter-item">
                        <label>Học kỳ</label>
                        <Select
                            value={semesterId}
                            onChange={(value) => setSemesterId(value)}
                            options={semesters.map(semester => ({
                                value: semester.semesterID,
                                label: `${semester.semesterName} -- ${semester.semesterID}`
                            }))}
                            style={{ width: 300 }}
                            placeholder="Chọn học kỳ"
                            disabled={!academicYearId || !semesters.length}
                        />
                    </div>
                    <div className="filter-item">
                        <label>Chọn học sinh</label>
                        <Select
                            value={studentId}
                            onChange={handleStudentChange}
                            options={studentList}
                            style={{ width: 300 }}
                            placeholder="Chọn học sinh"
                            disabled={!studentList.length}
                        />
                    </div>
                </div>
            </div>

            {currentSchedule && selectedStudent && (
                <div className="schedule-header">
                    <h2>Thời Khóa Biểu - {selectedStudent.fullName} - Lớp {selectedStudent.className}</h2>
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