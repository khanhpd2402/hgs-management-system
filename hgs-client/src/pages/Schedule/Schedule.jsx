import React, { useState, useMemo } from "react";
import { scheduleData, teacherData, subjectData } from "./data";
import "./Schedule.scss";
import ExportSchedule from "./ExportSchedule";

const getTeacherName = (teacher_id) => {
    const teacher = teacherData.find((t) => t.teacher_id === parseInt(teacher_id));
    return teacher ? teacher.teacher_name : "";
};

const getSubjectName = (subject_Id) => {
    const subject = subjectData.find((s) => s.subject_Id === parseInt(subject_Id));
    return subject ? subject.subject_name : "";
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


    const [tempTeacher, setTempTeacher] = useState("");
    const [tempGrade, setTempGrade] = useState("");
    const [tempClass, setTempClass] = useState("");
    const [tempSubject, setTempSubject] = useState("");
    const [tempSession, setTempSession] = useState("");

    const [showTeacherName, setShowTeacherName] = useState(true);
    const [originalScheduleData, setOriginalScheduleData] = useState(scheduleData);


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
        let filteredData = originalScheduleData;

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
                                            session === selectedSession
                                                ? periods.length > 0
                                                    ? periods
                                                    : [{ subject_Id: "", teacher_id: "" }]
                                                : [],

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
    }, [selectedGrade, selectedClass, selectedTeacher, selectedSubject, selectedSession, originalScheduleData]);


    const handleSearch = () => {
        setOriginalScheduleData(scheduleData);
        if (tempTeacher === null && tempGrade === null && tempSubject === null && tempSession === null && tempClass === null && temp) {
            setShowTeacherName(true);
            return;
        }

        setSelectedTeacher(tempTeacher);
        setSelectedGrade(tempGrade);
        setSelectedClass(tempClass);
        setSelectedSubject(tempSubject);
        setSelectedSession(tempSession);
    };
    const handleReset = () => {
        setTempTeacher("");
        setTempGrade("");
        setTempClass("");
        setTempSubject("");
        setTempSession("");

        setSelectedTeacher("");
        setSelectedGrade("");
        setSelectedClass("");
        setSelectedSubject("");
        setSelectedSession("");

        setOriginalScheduleData(scheduleData);
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
                        <select onChange={(e) => setTempTeacher(e.target.value)} value={tempTeacher}>
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
                        <select onChange={(e) => setTempGrade(e.target.value)} value={tempGrade}>
                            <option value="">-- Lựa chọn --</option>
                            {grades.map((grade) => (
                                <option key={grade} value={grade}>Khối {grade}</option>
                            ))}
                        </select>
                    </div>

                    <div className="filter-column">
                        <label>Lớp</label>
                        <select onChange={(e) => setTempClass(e.target.value)} value={tempClass} disabled={!tempGrade}>
                            <option value="">-- Lựa chọn --</option>
                            {tempGrade && Object.keys(scheduleData[tempGrade]).map((className) => (
                                <option key={className} value={className}>{className}</option>
                            ))}
                        </select>
                    </div>

                    <div className="filter-column">
                        <label>Môn học</label>
                        <select onChange={(e) => setTempSubject(e.target.value)} value={tempSubject}>
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
                        <select onChange={(e) => setTempSession(e.target.value)} value={tempSession}>
                            <option value="">Chọn buổi</option>
                            <option value="Morning">Sáng</option>
                            <option value="Afternoon">Chiều</option>
                        </select>
                    </div>
                </div>

                {/* Nút tìm kiếm và nút reset */}
                <div className="filter-row">
                    <div className="filter-column search-button">
                        <button onClick={handleSearch}>Tìm kiếm</button>
                        <button onClick={handleReset} className="reset-button">Reset</button>
                    </div>
                </div>



            </div>



            <div className="filter-row" style={{ display: 'flex', justifyContent: 'flex-end', alignItems: 'center' }}>
                <div className="filter-column" style={{ marginRight: '10px' }}>
                    <button onClick={() => setShowTeacherName(!showTeacherName)}>
                        {showTeacherName ? "Ẩn tên giáo viên" : "Hiển thị tên giáo viên"}
                    </button>
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
                    showTeacherName={showTeacherName} // Truyền prop vào
                />
            </div>






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
                            let displayedSessions = sessions.filter(session => {
                                if (selectedSession) {
                                    return session === selectedSession;
                                }
                                return true;
                            });

                            let totalDisplayedPeriods = 0;
                            displayedSessions.forEach(session => {
                                let maxPeriods = Math.max(
                                    ...filteredClasses.map(({ grade, className }) =>
                                        filteredScheduleData[grade][className]?.[day]?.[session]?.length || 0
                                    )
                                );
                                totalDisplayedPeriods += maxPeriods;
                            });

                            const rowClass = dayIndex % 2 === 0 ? "even-day" : "odd-day";

                            let periodCounter = 0;

                            return (
                                <React.Fragment key={day}>
                                    {displayedSessions.map((session, sessionIndex) => {
                                        let maxPeriods = Math.max(
                                            ...filteredClasses.map(({ grade, className }) =>
                                                filteredScheduleData[grade][className]?.[day]?.[session]?.length || 0
                                            )
                                        );

                                        return Array.from({ length: maxPeriods }).map((_, periodIndex) => {
                                            let shouldDisplay = true;
                                            const period = filteredScheduleData[selectedGrade]?.[selectedClass]?.[day]?.[session]?.[periodIndex];

                                            if (selectedTeacher && period && parseInt(period.teacher_id) !== parseInt(selectedTeacher)) {
                                                shouldDisplay = false;
                                            }

                                            if (selectedSubject && period && parseInt(period.subject_Id) !== parseInt(selectedSubject)) {
                                                shouldDisplay = false;
                                            }

                                            if (selectedGrade && selectedClass && !period) {
                                                shouldDisplay = false;
                                            }

                                            if (!shouldDisplay) {
                                                return null;
                                            }

                                            periodCounter++;

                                            return (
                                                <tr key={`${day}-${session}-${periodIndex}`} className={rowClass}>
                                                    {sessionIndex === 0 && periodIndex === 0 && (
                                                        <td className="sticky-col col-1" rowSpan={totalDisplayedPeriods}>{day}</td>
                                                    )}
                                                    {periodIndex === 0 && <td className="sticky-col col-2" rowSpan={maxPeriods}>{session}</td>}
                                                    <td className="sticky-col col-3">Tiết {periodIndex + 1}</td>
                                                    {filteredClasses.map(({ grade, className }) => {
                                                        const period = filteredScheduleData[grade][className]?.[day]?.[session]?.[periodIndex];
                                                        return (
                                                            <td key={`${grade}-${className}-${day}-${session}-${periodIndex}`}>
                                                                {period
                                                                    ? `${getSubjectName(period.subject_Id)}${showTeacherName ? " - " + getTeacherName(period.teacher_id) : ""}`
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
