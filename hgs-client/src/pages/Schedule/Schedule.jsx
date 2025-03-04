import React from "react";
import { scheduleData, teacherData, subjectData } from "./data";
import "./Schedule.scss";

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

    const allClasses = grades.flatMap((grade) =>
        Object.keys(scheduleData[grade] || {}).map((className) => ({ grade, className }))
    );

    return (
        <div>
            <h2 style={{ textAlign: "center" }}>Thời Khóa Biểu</h2>
            <div className="table-container">
                <table className="schedule-table">
                    <thead>
                        <tr>
                            <th className="sticky-col col-1">Thứ</th>
                            <th className="sticky-col col-2">Buổi</th>
                            <th className="sticky-col col-3">Tiết</th>
                            {allClasses.map(({ grade, className }) => (
                                <th key={`${grade}-${className}`}>Khối {grade} - {className}</th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {days.map((day) => {
                            let maxPeriodsInDay = 0;

                            sessions.forEach((session) => {
                                const maxPeriods = Math.max(
                                    ...allClasses.map(({ grade, className }) =>
                                        scheduleData[grade][className]?.[day]?.[session]?.length || 0
                                    )
                                );
                                maxPeriodsInDay += maxPeriods;
                            });

                            return (
                                <React.Fragment key={day}>
                                    {sessions.map((session, sessionIndex) => {
                                        const maxPeriods = Math.max(
                                            ...allClasses.map(({ grade, className }) =>
                                                scheduleData[grade][className]?.[day]?.[session]?.length || 0
                                            )
                                        );

                                        return [...Array(maxPeriods)].map((_, periodIndex) => (
                                            <tr key={`${day}-${session}-${periodIndex}`}>
                                                {sessionIndex === 0 && periodIndex === 0 && (
                                                    <td className="sticky-col col-1" rowSpan={maxPeriodsInDay}>{day}</td>
                                                )}
                                                {periodIndex === 0 && <td className="sticky-col col-2" rowSpan={maxPeriods}>{session}</td>}
                                                <td className="sticky-col col-3">Tiết {periodIndex + 1}</td>
                                                {allClasses.map(({ grade, className }) => {
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
