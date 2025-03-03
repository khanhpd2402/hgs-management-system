import React, { useState } from "react";
import { scheduleData } from "./data";
import "./Schedule.scss";

const transformScheduleData = (data, selectedBlock, selectedClass, selectedSession) => {
    const days = ["Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy", "Chủ nhật"];
    const sessions = ["Sáng", "Chiều"];
    const transformedData = [];
    const daySessionCount = {};

    days.forEach((day) => {
        sessions.forEach((session) => {
            if (selectedSession && session !== selectedSession) return;
            let maxPeriods = 0;
            Object.values(data).forEach((block) => {
                Object.values(block).forEach((schedule) => {
                    const periods = schedule[day]?.[session]?.length || 0;
                    if (periods > maxPeriods) maxPeriods = periods;
                });
            });

            for (let period = 0; period < maxPeriods; period++) {
                const row = { day, session, period: period + 1 };
                Object.entries(data).forEach(([blockName, block]) => {
                    if (selectedBlock && blockName !== selectedBlock) return;
                    Object.keys(block).forEach((className) => {
                        if (selectedClass && className !== selectedClass) return;
                        row[className] = block[className][day]?.[session]?.[period] || "";
                    });
                });
                transformedData.push(row);
                daySessionCount[`${day}-${session}`] = (daySessionCount[`${day}-${session}`] || 0) + 1;
            }
        });
    });

    return { transformedData, daySessionCount };
};

const ScheduleTable = () => {
    const [selectedBlock, setSelectedBlock] = useState("");
    const [selectedClass, setSelectedClass] = useState("");
    const [selectedSession, setSelectedSession] = useState("");
    const [isFiltersVisible, setIsFiltersVisible] = useState(true); // State to toggle filters visibility

    const { transformedData, daySessionCount } = transformScheduleData(scheduleData, selectedBlock, selectedClass, selectedSession);
    const blocks = Object.keys(scheduleData);
    const classes = selectedBlock ? Object.keys(scheduleData[selectedBlock]) : [];

    const classNames = Array.from(new Set(transformedData.flatMap(row => Object.keys(row)).filter(key => !["day", "session", "period"].includes(key))));

    const renderedDays = new Set();
    const renderedSessions = new Set();

    const getDayClass = (day) => {
        const evenDays = ["Thứ hai", "Thứ tư", "Thứ sáu", "Chủ nhật"];
        return evenDays.includes(day) ? "even-day" : "odd-day";
    };

    return (
        <div className="schedule-wrapper">
            {/* Bộ lọc trên cùng */}
            <div className="filters">
                <h2 className="filter-header" onClick={() => setIsFiltersVisible(!isFiltersVisible)}>
                    Bộ Lọc
                </h2>
                {isFiltersVisible && (
                    <div className="filter-grid">
                        <div className="filter-item">
                            <label>Học kỳ</label>
                            <select>
                                <option value="">Chọn Học kỳ</option>
                                <option value="Học kỳ I">Học kỳ I</option>
                                <option value="Học kỳ II">Học kỳ II</option>
                            </select>
                        </div>

                        <div className="filter-item">
                            <label>Ngày áp dụng</label>
                            <input type="date" />
                        </div>

                        <div className="filter-item">
                            <label>Giáo viên</label>
                            <input type="text" placeholder="Nhập tên giáo viên" />
                        </div>

                        <div className="filter-item">
                            <label>Khối</label>
                            <select onChange={(e) => {
                                setSelectedBlock(e.target.value);
                                setSelectedClass("");
                            }}>
                                <option value="">Chọn Khối</option>
                                {blocks.map((block) => (
                                    <option key={block} value={block}>{block}</option>
                                ))}
                            </select>
                        </div>

                        <div className="filter-item">
                            <label>Lớp</label>
                            <select onChange={(e) => setSelectedClass(e.target.value)} value={selectedClass}>
                                <option value="">Chọn Lớp</option>
                                {classes.map((cls) => (
                                    <option key={cls} value={cls}>{cls}</option>
                                ))}
                            </select>
                        </div>

                        <div className="filter-item">
                            <label>Môn học</label>
                            <select>
                                <option value="">Chọn Môn học</option>
                                <option value="Toán">Toán</option>
                                <option value="Văn">Văn</option>
                                <option value="Anh">Anh</option>
                            </select>
                        </div>

                        <div className="filter-item">
                            <label>Buổi</label>
                            <select>
                                <option value="">Chọn Buổi</option>
                                <option value="Sáng">Sáng</option>
                                <option value="Chiều">Chiều</option>
                            </select>
                        </div>
                    </div>
                )}
            </div>

            {/* Thời khóa biểu bên dưới */}
            <div className="table-container">
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

                            if (isFirstDayOccurrence) renderedDays.add(row.day);
                            if (isFirstSessionOccurrence) renderedSessions.add(daySessionKey);

                            return (
                                <tr key={index} className={getDayClass(row.day)}>
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
        </div>
    );
};

export default ScheduleTable;
