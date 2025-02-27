import React from "react";
import { scheduleData } from "./data";
import "./Schedule.scss";

// Hàm chuyển đổi dữ liệu
const transformScheduleData = (data) => {
    const days = ["Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy", "Chủ nhật"];
    const sessions = ["Sáng", "Chiều"];
    const transformedData = [];

    days.forEach((day) => {
        sessions.forEach((session) => {
            // Tìm số tiết tối đa trong buổi này
            let maxPeriods = 0;
            Object.values(data).forEach((block) => {
                Object.values(block).forEach((schedule) => {
                    const periods = schedule[day]?.[session]?.length || 0;
                    if (periods > maxPeriods) maxPeriods = periods;
                });
            });

            // Tạo các hàng cho từng tiết
            for (let period = 0; period < maxPeriods; period++) {
                const row = {
                    day,
                    session,
                    period: period + 1,
                };

                // Thêm môn học cho từng lớp
                Object.entries(data).forEach(([blockName, block]) => {
                    Object.keys(block).forEach((className) => {
                        row[className] = block[className][day]?.[session]?.[period] || "";
                    });
                });

                transformedData.push(row);
            }
        });
    });

    return transformedData;
};

const ScheduleTable = () => {
    const transformedData = transformScheduleData(scheduleData);
    const classNames = Array.from(
        new Set(
            transformedData.flatMap((row) => Object.keys(row)).filter((key) => !["day", "session", "period"].includes(key))
        )
    );

    // Tạo cấu trúc dữ liệu để xác định rowSpan cho "Thứ" và "Buổi"
    const daySessionCount = {};
    transformedData.forEach((row) => {
        const key = `${row.day}-${row.session}`;
        if (!daySessionCount[key]) {
            daySessionCount[key] = 0;
        }
        daySessionCount[key]++;
    });

    const renderedDays = new Set();
    const renderedSessions = new Set();

    return (
        <div className="schedule-container">
            <h1>Thời Khóa Biểu</h1>
            <table className="schedule-table">
                <thead>
                    <tr>
                        <th>Thứ</th>
                        <th>Buổi</th>
                        <th>Tiết</th>
                        {classNames.map((className) => (
                            <th key={className}>{className}</th>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {transformedData.map((row, index) => {
                        const daySessionKey = `${row.day}-${row.session}`;
                        const isFirstDayOccurrence = !renderedDays.has(row.day);
                        const isFirstSessionOccurrence = !renderedSessions.has(daySessionKey);

                        if (isFirstDayOccurrence) {
                            renderedDays.add(row.day);
                        }
                        if (isFirstSessionOccurrence) {
                            renderedSessions.add(daySessionKey);
                        }

                        // Xác định lớp CSS cho hàng dựa trên thứ
                        const dayIndex = ["Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy", "Chủ nhật"].indexOf(row.day);
                        const rowClass = row.day === "Chủ nhật" ? "sunday" : dayIndex % 2 === 0 ? "even-day" : "odd-day";

                        return (
                            <tr key={index} className={rowClass}>
                                {isFirstDayOccurrence && (
                                    <td rowSpan={Object.keys(daySessionCount).filter((key) => key.startsWith(row.day)).reduce((sum, key) => sum + daySessionCount[key], 0)}>
                                        {row.day}
                                    </td>
                                )}
                                {isFirstSessionOccurrence && (
                                    <td rowSpan={daySessionCount[daySessionKey]}>
                                        {row.session}
                                    </td>
                                )}
                                <td>{row.period}</td>
                                {classNames.map((className) => (
                                    <td key={className}>{row[className] || ""}</td>
                                ))}
                            </tr>
                        );
                    })}
                </tbody>
            </table>
        </div>
    );
};

export default ScheduleTable;
