import React from "react";
import { scheduleData } from "./data";
import "./Schedule.scss";

const ScheduleTable = () => {
    const classNames = Object.keys(scheduleData);
    const days = ["Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy", "Chủ nhật"];

    return (
        <div className="schedule-container">
            <h1>Thời Khóa Biểu</h1>
            <table className="schedule-table">
                <thead>
                    <tr>
                        <th>Thứ</th>
                        <th>Buổi</th>
                        <th>Tiết</th>
                        {classNames.map(className => (
                            <th key={className}>{className}</th>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {days.map((day, dayIndex) => {
                        const morningPeriods = Math.max(...classNames.map(className => scheduleData[className]?.[day]?.["Sáng"]?.length || 0));
                        const afternoonPeriods = Math.max(...classNames.map(className => scheduleData[className]?.[day]?.["Chiều"]?.length || 0));
                        const totalPeriods = morningPeriods + afternoonPeriods;

                        return Array.from({ length: totalPeriods }).map((_, periodIndex) => {
                            let currentSession = periodIndex < morningPeriods ? "Sáng" : "Chiều";
                            let sessionPeriodIndex = periodIndex < morningPeriods ? periodIndex : periodIndex - morningPeriods;

                            return (
                                <tr key={`${day}-${periodIndex}`} className={day === "Chủ nhật" ? "sunday" : (dayIndex % 2 === 1 ? "even-day" : "odd-day")}>
                                    {periodIndex === 0 && <td rowSpan={totalPeriods}>{day}</td>}
                                    {sessionPeriodIndex === 0 && <td rowSpan={currentSession === "Sáng" ? morningPeriods : afternoonPeriods}>{currentSession}</td>}
                                    <td>{sessionPeriodIndex + 1}</td>
                                    {classNames.map(className => (
                                        <td key={className}>{scheduleData[className]?.[day]?.[currentSession]?.[sessionPeriodIndex] || ""}</td>
                                    ))}
                                </tr>
                            );
                        });
                    })}
                </tbody>
            </table>
        </div>
    );
};

export default ScheduleTable;
