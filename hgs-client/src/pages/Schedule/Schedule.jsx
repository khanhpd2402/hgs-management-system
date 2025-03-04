import React, { useState } from "react";
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

    // 🌟 State để lưu khối và lớp được chọn
    const [selectedGrade, setSelectedGrade] = useState("");
    const [selectedClass, setSelectedClass] = useState("");

    // Lấy danh sách tất cả các lớp
    const allClasses = grades.flatMap((grade) =>
        Object.keys(scheduleData[grade] || {}).map((className) => ({ grade, className }))
    );

    // Lọc danh sách lớp dựa theo lựa chọn của người dùng
    const filteredClasses = allClasses.filter(({ grade, className }) =>
        (!selectedGrade || grade === selectedGrade) &&
        (!selectedClass || className === selectedClass)
    );

    return (
        <div>
            <h2 style={{ textAlign: "center" }}>Thời Khóa Biểu</h2>

            {/* 🎯 Bộ lọc chọn khối và lớp */}
            <div className="filter-container">
                <select onChange={(e) => setSelectedGrade(e.target.value)} value={selectedGrade}>
                    <option value="">Chọn khối</option>
                    {grades.map((grade) => (
                        <option key={grade} value={grade}>Khối {grade}</option>
                    ))}
                </select>

                <select onChange={(e) => setSelectedClass(e.target.value)} value={selectedClass} disabled={!selectedGrade}>
                    <option value="">Chọn lớp</option>
                    {selectedGrade &&
                        Object.keys(scheduleData[selectedGrade]).map((className) => (
                            <option key={className} value={className}>{className}</option>
                        ))}
                </select>
            </div>
            <ExportSchedule
                selectedGrade={selectedGrade}
                selectedClass={selectedClass}
                filteredClasses={filteredClasses}
                scheduleData={scheduleData}
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
                                        scheduleData[grade][className]?.[day]?.[session]?.length || 0
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
                                                scheduleData[grade][className]?.[day]?.[session]?.length || 0
                                            )
                                        );

                                        return [...Array(maxPeriods)].map((_, periodIndex) => (
                                            <tr key={`${day}-${session}-${periodIndex}`} className={rowClass}>
                                                {sessionIndex === 0 && periodIndex === 0 && (
                                                    <td className="sticky-col col-1" rowSpan={maxPeriodsInDay}>{day}</td>
                                                )}
                                                {periodIndex === 0 && <td className="sticky-col col-2" rowSpan={maxPeriods}>{session}</td>}
                                                <td className="sticky-col col-3">Tiết {periodIndex + 1}</td>
                                                {filteredClasses.map(({ grade, className }) => {
                                                    const period = scheduleData[grade][className]?.[day]?.[session]?.[periodIndex];
                                                    return (
                                                        <td key={`${grade}-${className}-${day}-${session}-${periodIndex}`}>
                                                            {period
                                                                ? `${getSubjectName(period.subject_Id)} - ${getTeacherName(period.teacher_id)}`
                                                                : " "}
                                                        </td>
                                                    );
                                                })}
                                            </tr>
                                        ));
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
