import React, { useState, useMemo } from "react";
import { scheduleData, teacherData, subjectData } from "./data";
import "./Schedule.scss";
import ExportSchedule from "./ExportSchedule";

const getTeacherName = (teacher_id) => {
    const teacher = teacherData.find((t) => t.teacher_id === parseInt(teacher_id));
    return teacher ? teacher.teacher_name : "Unknown";
};

const getSubjectName = (subject_Id) => {
    const subject = subjectData.find((s) => s.subject_Id === parseInt(subject_Id));
    return subject ? subject.subject_name : "Unknown";
};

const ScheduleTable = () => {
    const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    const sessions = ["Morning", "Afternoon"];
    const grades = Object.keys(scheduleData);

    const [selectedGrade, setSelectedGrade] = useState("");
    const [selectedClass, setSelectedClass] = useState("");
    const [selectedTeacher, setSelectedTeacher] = useState("");
    const [selectedSubject, setSelectedSubject] = useState("");
    const [selectedSession, setSelectedSession] = useState("");

    const allClasses = useMemo(() => {
        return grades.flatMap((grade) =>
            Object.keys(scheduleData[grade] || {}).map((className) => ({ grade, className }))
        );
    }, [grades]);

    const filteredClasses = useMemo(() => {
        return allClasses.filter(({ grade, className }) =>
            (!selectedGrade || grade === selectedGrade) &&
            (!selectedClass || className === selectedClass)
        );
    }, [allClasses, selectedGrade, selectedClass]);

    const filteredScheduleData = useMemo(() => {
        let filteredData = scheduleData;

        if (selectedGrade) {
            filteredData = Object.fromEntries(
                Object.entries(filteredData).filter(([grade]) => grade === selectedGrade)
            );
        }

        if (selectedClass) {
            filteredData = Object.fromEntries(
                Object.entries(filteredData).map(([grade, classes]) => [
                    grade,
                    Object.fromEntries(
                        Object.entries(classes).filter(([className]) => className === selectedClass)
                    ),
                ])
            );
        }

        if (selectedTeacher) {
            filteredData = Object.fromEntries(
                Object.entries(filteredData).map(([grade, classes]) => [
                    grade,
                    Object.fromEntries(
                        Object.entries(classes).map(([className, days]) => [
                            className,
                            Object.fromEntries(
                                Object.entries(days).map(([day, sessions]) => [
                                    day,
                                    Object.fromEntries(
                                        Object.entries(sessions).map(([session, periods]) => [
                                            session,
                                            periods.filter((period) => period.teacher_id === selectedTeacher),
                                        ])
                                    ),
                                ])
                            ),
                        ])
                    ),
                ])
            );
        }

        if (selectedSubject) {
            filteredData = Object.fromEntries(
                Object.entries(filteredData).map(([grade, classes]) => [
                    grade,
                    Object.fromEntries(
                        Object.entries(classes).map(([className, days]) => [
                            className,
                            Object.fromEntries(
                                Object.entries(days).map(([day, sessions]) => [
                                    day,
                                    Object.fromEntries(
                                        Object.entries(sessions).map(([session, periods]) => [
                                            session,
                                            periods.filter((period) => period.subject_Id === selectedSubject),
                                        ])
                                    ),
                                ])
                            ),
                        ])
                    ),
                ])
            );
        }

        if (selectedSession) {
            filteredData = Object.fromEntries(
                Object.entries(filteredData).map(([grade, classes]) => [
                    grade,
                    Object.fromEntries(
                        Object.entries(classes).map(([className, days]) => [
                            className,
                            Object.fromEntries(
                                Object.entries(days).map(([day, sessions]) => [
                                    day,
                                    Object.fromEntries(
                                        Object.entries(sessions).map(([session, periods]) => [
                                            session,
                                            session === selectedSession ? periods : [],
                                        ])
                                    ),
                                ])
                            ),
                        ])
                    ),
                ])
            );
        }

        return filteredData;
    }, [selectedGrade, selectedClass, selectedTeacher, selectedSubject, selectedSession]);

    const handleSearch = () => {
        // No need to filter here anymore as filtering is already handled by useMemo
    };

    return (
        <div>
            <h2 style={{ textAlign: "center" }}>Thời Khóa Biểu</h2>

            <div className="filter-container">
                {/* Hàng 1: Học kỳ, Ngày áp dụng, Giáo viên */}
                <div className="filter-row">
                    <div className="filter-column">
                        <label>Học kỳ</label>
                        <input type="text" value="Học kỳ II" readOnly />
                    </div>

                    <div className="filter-column">
                        <label>Ngày áp dụng</label>
                        <input type="text" value="20/01/2025" readOnly />
                    </div>

                    <div className="filter-column">
                        <label>Giáo viên</label>
                        <select onChange={(e) => setSelectedTeacher(e.target.value)} value={selectedTeacher}>
                            <option value="">Chọn giáo viên</option>
                            {teacherData.map((teacher) => (
                                <option key={teacher.teacher_id} value={teacher.teacher_id}>{teacher.teacher_name}</option>
                            ))}
                        </select>
                    </div>
                </div>

                {/* Hàng 2: Khối, Lớp, Môn học */}
                <div className="filter-row">
                    <div className="filter-column">
                        <label>Khối</label>
                        <select onChange={(e) => setSelectedGrade(e.target.value)} value={selectedGrade}>
                            <option value="">-- Lựa chọn --</option>
                            {grades.map((grade) => (
                                <option key={grade} value={grade}>Khối {grade}</option>
                            ))}
                        </select>
                    </div>

                    <div className="filter-column">
                        <label>Lớp</label>
                        <select onChange={(e) => setSelectedClass(e.target.value)} value={selectedClass} disabled={!selectedGrade}>
                            <option value="">-- Lựa chọn --</option>
                            {selectedGrade && Object.keys(scheduleData[selectedGrade]).map((className) => (
                                <option key={className} value={className}>{className}</option>
                            ))}
                        </select>
                    </div>

                    <div className="filter-column">
                        <label>Môn học</label>
                        <select onChange={(e) => setSelectedSubject(e.target.value)} value={selectedSubject}>
                            <option value="">-- Lựa chọn --</option>
                            {subjectData.map((subject) => (
                                <option key={subject.subject_Id} value={subject.subject_Id}>{subject.subject_name}</option>
                            ))}
                        </select>
                    </div>
                </div>

                {/* Hàng 3: Chọn buổi */}
                <div className="filter-row">
                    <div className="filter-column">
                        <label>Chọn buổi</label>
                        <select onChange={(e) => setSelectedSession(e.target.value)} value={selectedSession}>
                            <option value="">Chọn buổi</option>
                            <option value="Morning">Sáng</option>
                            <option value="Afternoon">Chiều</option>
                        </select>
                    </div>
                </div>

                {/* Nút tìm kiếm */}
                <div className="filter-row">
                    <div className="filter-column search-button">
                        <button onClick={handleSearch}>Tìm kiếm</button>
                    </div>
                </div>
            </div>







            <ExportSchedule
                selectedGrade={selectedGrade}
                selectedClass={selectedClass}
                filteredClasses={filteredClasses}
                scheduleData={filteredScheduleData}
                days={days}
                sessions={sessions}
                getSubjectName={getSubjectName}
                getTeacherName={getTeacherName}
            />

            <div className="table-container">
                <table className="schedule-table">
                    <thead>
                        <tr>
                            <th className="sticky-header" colSpan={3}>Lịch học</th>
                        </tr>
                        <tr>
                            <th className="sticky-col col-1">Thứ</th>
                            <th className="sticky-col col-2">Buổi</th>
                            <th className="sticky-col col-3">Tiết</th>
                            {filteredClasses.map(({ grade, className }) => (
                                <th key={`${grade}-${className}`}>Khối {grade} - {className}</th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {days.map((day, dayIndex) => {
                            let maxPeriodsInDay = 0;

                            sessions.forEach((session) => {
                                const maxPeriods = Math.max(
                                    ...filteredClasses.map(({ grade, className }) =>
                                        filteredScheduleData[grade][className]?.[day]?.[session]?.length || 0
                                    )
                                );
                                maxPeriodsInDay += maxPeriods;
                            });

                            const rowClass = dayIndex % 2 === 0 ? "even-day" : "odd-day";

                            return (
                                <React.Fragment key={day}>
                                    {sessions.map((session, sessionIndex) => {
                                        const maxPeriods = Math.max(
                                            ...filteredClasses.map(({ grade, className }) =>
                                                filteredScheduleData[grade][className]?.[day]?.[session]?.length || 0
                                            )
                                        );

                                        return [...Array(maxPeriods)].map((_, periodIndex) => {
                                            let shouldDisplay = true;

                                            if (selectedTeacher) {
                                                const period = filteredScheduleData[selectedGrade]?.[selectedClass]?.[day]?.[session]?.[periodIndex];
                                                if (period && parseInt(period.teacher_id) !== parseInt(selectedTeacher)) {
                                                    shouldDisplay = false;
                                                }
                                            }

                                            if (selectedSubject) {
                                                const period = filteredScheduleData[selectedGrade]?.[selectedClass]?.[day]?.[session]?.[periodIndex];
                                                if (period && parseInt(period.subject_Id) !== parseInt(selectedSubject)) {
                                                    shouldDisplay = false;
                                                }
                                            }

                                            if (selectedSession && selectedSession !== session) {
                                                shouldDisplay = false;
                                            }

                                            if (selectedGrade && selectedClass) {
                                                if (!filteredScheduleData[selectedGrade][selectedClass][day][session][periodIndex]) {
                                                    shouldDisplay = false;
                                                }
                                            }

                                            if (!shouldDisplay) {
                                                return null; // Skip rendering if filters don't match
                                            }

                                            return (
                                                <tr key={`${day}-${session}-${periodIndex}`} className={rowClass}>
                                                    {sessionIndex === 0 && periodIndex === 0 && (
                                                        <td className="sticky-col col-1" rowSpan={maxPeriodsInDay}>{day}</td>
                                                    )}
                                                    {periodIndex === 0 && <td className="sticky-col col-2" rowSpan={maxPeriods}>{session}</td>}
                                                    <td className="sticky-col col-3">Tiết {periodIndex + 1}</td>
                                                    {filteredClasses.map(({ grade, className }) => {
                                                        const period = filteredScheduleData[grade][className]?.[day]?.[session]?.[periodIndex];
                                                        return (
                                                            <td key={`${grade}-${className}-${day}-${session}-${periodIndex}`}>
                                                                {period
                                                                    ? `${getSubjectName(period.subject_Id)} - ${getTeacherName(period.teacher_id)}`
                                                                    : " "}
                                                            </td>
                                                        );
                                                    })}
                                                </tr>
                                            );
                                        });
                                    })}
                                </React.Fragment>
                            );
                        })}
                    </tbody>
                </table>
            </div>
        </div>
    );
};

export default ScheduleTable;
