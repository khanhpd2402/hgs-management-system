import React, { useState, useMemo } from "react";
import { scheduleData, teacherData, subjectData } from "./data";
import "./Schedule.scss";
import ExportSchedule from "./ExportSchedule";
import { Calendar, Save, Trash2 } from "lucide-react";
import ImportSchedule from "./ImportSchedule";

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
    const dayMap = {
        "Monday": "Thứ 2",
        "Tuesday": "Thứ 3",
        "Wednesday": "Thứ 4",
        "Thursday": "Thứ 5",
        "Friday": "Thứ 6",
        "Saturday": "Thứ 7",
        "Sunday": "Chủ Nhật"
    };

    const sessionMap = {
        "Morning": "Sáng",
        "Afternoon": "Chiều"
    };

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

    // Thêm state để theo dõi việc kéo thả
    const [draggedItem, setDraggedItem] = useState(null);
    const [draggedOverItem, setDraggedOverItem] = useState(null);

    // Thêm state để theo dõi việc chỉnh sửa
    const [editingCell, setEditingCell] = useState(null);

    // Thêm state để quản lý popup
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedPeriodInfo, setSelectedPeriodInfo] = useState(null);
    const [selectedModalSubject, setSelectedModalSubject] = useState("");
    const [selectedModalTeacher, setSelectedModalTeacher] = useState("");

    // Thêm state để theo dõi vị trí kéo thả
    const [isDraggingOverTrash, setIsDraggingOverTrash] = useState(false);



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

    // Hàm xử lý khi bắt đầu kéo
    const handleDragStart = (day, session, periodIndex, grade, className, period) => {
        if (!period) return; // Không cho phép kéo ô trống
        setDraggedItem({
            day,
            session,
            periodIndex,
            grade,
            className,
            period
        });
    };

    // Hàm xử lý khi kéo qua một ô khác
    const handleDragOver = (e, day, session, periodIndex, grade, className) => {
        e.preventDefault();
        if (draggedItem && draggedItem.grade === grade && draggedItem.className === className) {
            setDraggedOverItem({ day, session, periodIndex });
        }
    };

    // Thêm hàm kiểm tra trùng lặp
    const checkDuplicate = (newScheduleData, grade, className, day, session, period) => {
        const existingPeriods = newScheduleData[grade][className][day][session] || [];

        // Kiểm tra trùng giáo viên trong cùng một khung giờ
        const hasTeacherDuplicate = existingPeriods.some(
            (existingPeriod) =>
                existingPeriod &&
                period &&
                existingPeriod.teacher_id === period.teacher_id
        );

        if (hasTeacherDuplicate) {
            return {
                isDuplicate: true,
                message: "Giáo viên này đã có tiết dạy trong khung giờ này!"
            };
        }

        return {
            isDuplicate: false,
            message: ""
        };
    };

    // Chỉnh sửa hàm handleDrop
    const handleDrop = (e, day, session, periodIndex, grade, className) => {
        e.preventDefault();

        if (!draggedItem || draggedItem.grade !== grade || draggedItem.className !== className) {
            return;
        }

        // Tạo bản sao của dữ liệu
        const newScheduleData = JSON.parse(JSON.stringify(originalScheduleData));

        // Kiểm tra trùng lặp trước khi thực hiện kéo thả
        const duplicateCheck = checkDuplicate(
            newScheduleData,
            grade,
            className,
            day,
            session,
            draggedItem.period
        );

        if (duplicateCheck.isDuplicate) {
            alert(duplicateCheck.message);
            setDraggedItem(null);
            setDraggedOverItem(null);
            return;
        }

        // Lấy tiết học ở vị trí đích
        const targetPeriod = newScheduleData[grade][className][day][session][periodIndex];

        // Hoán đổi vị trí giữa hai tiết học
        newScheduleData[grade][className][draggedItem.day][draggedItem.session][draggedItem.periodIndex] = targetPeriod;
        newScheduleData[grade][className][day][session][periodIndex] = draggedItem.period;

        // Cập nhật state
        setOriginalScheduleData(newScheduleData);
        setDraggedItem(null);
        setDraggedOverItem(null);
    };

    // Thêm hàm xử lý khi thay đổi môn học và giáo viên
    const handleSubjectChange = (day, session, periodIndex, grade, className, subjectId) => {
        const newScheduleData = JSON.parse(JSON.stringify(originalScheduleData));

        if (!newScheduleData[grade][className][day][session]) {
            newScheduleData[grade][className][day][session] = [];
        }

        // Lấy thông tin giáo viên hiện tại (nếu có)
        const currentTeacherId = newScheduleData[grade][className][day][session][periodIndex]?.teacher_id || "";

        // Tạo hoặc cập nhật period, giữ nguyên teacher_id
        if (!newScheduleData[grade][className][day][session][periodIndex]) {
            newScheduleData[grade][className][day][session][periodIndex] = {
                subject_Id: subjectId,
                teacher_id: currentTeacherId
            };
        } else {
            newScheduleData[grade][className][day][session][periodIndex] = {
                ...newScheduleData[grade][className][day][session][periodIndex],
                subject_Id: subjectId
            };
        }

        setOriginalScheduleData(newScheduleData);
    };

    const handleTeacherChange = (day, session, periodIndex, grade, className, teacherId) => {
        const newScheduleData = JSON.parse(JSON.stringify(originalScheduleData));

        if (!newScheduleData[grade][className][day][session]) {
            newScheduleData[grade][className][day][session] = [];
        }

        if (!newScheduleData[grade][className][day][session][periodIndex]) {
            newScheduleData[grade][className][day][session][periodIndex] = {
                subject_Id: "",
                teacher_id: teacherId
            };
        } else {
            newScheduleData[grade][className][day][session][periodIndex].teacher_id = teacherId;
        }

        setOriginalScheduleData(newScheduleData);
    };

    // Thêm hàm để mở popup khi click vào ô trống
    const handleEmptyCellClick = (day, session, periodIndex, grade, className) => {
        setSelectedPeriodInfo({ day, session, periodIndex, grade, className });
        setSelectedModalSubject("");
        setSelectedModalTeacher("");
        setIsModalOpen(true);
    };

    // Thêm hàm kiểm tra trùng lặp trong cùng hàng cho modal
    const checkDuplicateInRowForModal = (scheduleData, grade, className, day, session, periodIndex, teacherId) => {
        // Kiểm tra tất cả các lớp trong cùng khối
        const allClassesInGrade = Object.keys(scheduleData[grade]);

        for (const currentClassName of allClassesInGrade) {
            // Bỏ qua việc kiểm tra với chính lớp đang thêm
            if (currentClassName === className) continue;

            const currentPeriod = scheduleData[grade][currentClassName][day][session][periodIndex];

            // Kiểm tra nếu có cùng giáo viên trong cùng tiết
            if (currentPeriod && currentPeriod.teacher_id === teacherId) {
                return {
                    isDuplicate: true,
                    message: `Giáo viên này đã có tiết dạy ở lớp ${currentClassName} trong cùng thời điểm!`
                };
            }
        }

        return {
            isDuplicate: false,
            message: ""
        };
    };

    // Chỉnh sửa hàm handleModalConfirm
    const handleModalConfirm = () => {
        if (!selectedModalSubject) {
            alert("Vui lòng chọn môn học!");
            return;
        }
        if (!selectedModalTeacher) {
            alert("Vui lòng chọn giáo viên!");
            return;
        }

        const { day, session, periodIndex, grade, className } = selectedPeriodInfo;

        // Kiểm tra trùng lặp trước khi thêm tiết học mới
        const duplicateCheck = checkDuplicateInRowForModal(
            originalScheduleData,
            grade,
            className,
            day,
            session,
            periodIndex,
            selectedModalTeacher
        );

        if (duplicateCheck.isDuplicate) {
            alert(duplicateCheck.message);
            return;
        }

        const newScheduleData = JSON.parse(JSON.stringify(originalScheduleData));

        if (!newScheduleData[grade][className][day][session]) {
            newScheduleData[grade][className][day][session] = [];
        }

        newScheduleData[grade][className][day][session][periodIndex] = {
            subject_Id: selectedModalSubject,
            teacher_id: selectedModalTeacher
        };

        setOriginalScheduleData(newScheduleData);
        setIsModalOpen(false);
    };

    // Component Modal
    const ScheduleModal = ({ isOpen, onClose, onConfirm }) => {
        if (!isOpen) return null;

        return (
            <div className="modal-overlay">
                <div className="modal-content">
                    <h3>Thêm tiết học mới</h3>
                    <div className="modal-form">
                        <div className="form-group">
                            <label>Chọn môn học:</label>
                            <select
                                value={selectedModalSubject}
                                onChange={(e) => setSelectedModalSubject(e.target.value)}
                            >
                                <option value="">-- Chọn môn học --</option>
                                {subjectData.map((subject) => (
                                    <option key={subject.subject_Id} value={subject.subject_Id}>
                                        {subject.subject_name}
                                    </option>
                                ))}
                            </select>
                        </div>

                        {selectedModalSubject && (
                            <div className="form-group">
                                <label>Chọn giáo viên:</label>
                                <select
                                    value={selectedModalTeacher}
                                    onChange={(e) => setSelectedModalTeacher(e.target.value)}
                                >
                                    <option value="">-- Chọn giáo viên --</option>
                                    {teacherData.map((teacher) => (
                                        <option key={teacher.teacher_id} value={teacher.teacher_id}>
                                            {teacher.teacher_name}
                                        </option>
                                    ))}
                                </select>
                            </div>
                        )}
                    </div>
                    <div className="modal-actions">
                        <button onClick={onConfirm}>Xác nhận</button>
                        <button onClick={onClose}>Hủy</button>
                    </div>
                </div>
            </div>
        );
    };

    // Hàm xử lý khi kéo qua thùng rác
    const handleTrashDragOver = (e) => {
        e.preventDefault();
        if (draggedItem) {
            setIsDraggingOverTrash(true);
        }
    };

    // Hàm xử lý khi kéo ra khỏi thùng rác
    const handleTrashDragLeave = () => {
        setIsDraggingOverTrash(false);
    };

    // Hàm xử lý khi thả vào thùng rác
    const handleTrashDrop = (e) => {
        e.preventDefault();
        if (!draggedItem) return;

        const { day, session, periodIndex, grade, className } = draggedItem;
        const newScheduleData = JSON.parse(JSON.stringify(originalScheduleData));

        // Xóa tiết học
        newScheduleData[grade][className][day][session][periodIndex] = null;

        setOriginalScheduleData(newScheduleData);
        setDraggedItem(null);
        setIsDraggingOverTrash(false);
    };

    return (
        <div>
            <h2
                style={{
                    textAlign: "center",
                    fontSize: "28px",
                    fontWeight: "bold",
                    color: "#727CF5",
                    padding: "10px",
                    borderBottom: "3px solid #727CF5",
                    display: "inline-block",
                    marginBottom: "20px",
                }}
            >
                Thời Khóa Biểu
            </h2>

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





            <div className="filter-row-table" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <Calendar size={20} />
                    <span>Thời khóa biểu</span>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                    <div className="filter-column-table">
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
                        showTeacherName={showTeacherName}
                    />
                    <ImportSchedule />

                    {/* Nút Lưu */}
                    <button className="btn-save">
                        <Save size={16} /> Lưu
                    </button>

                    <button className="btn-delete">
                        <Trash2 size={16} /> Xóa
                    </button>

                </div>
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
                                                        <td className="sticky-col col-1" rowSpan={totalDisplayedPeriods}>
                                                            {dayMap[day] || day}
                                                        </td>)}
                                                    {periodIndex === 0 && <td className="sticky-col col-2" rowSpan={maxPeriods}>  {sessionMap[session] || session}</td>}
                                                    <td className="sticky-col col-3">Tiết {periodIndex + 1}</td>
                                                    {filteredClasses.map(({ grade, className }) => {
                                                        const period = filteredScheduleData[grade][className]?.[day]?.[session]?.[periodIndex];
                                                        return (
                                                            <td
                                                                key={`${grade}-${className}-${day}-${session}-${periodIndex}`}
                                                                draggable={!!period}
                                                                onDragStart={() => handleDragStart(day, session, periodIndex, grade, className, period)}
                                                                onDragOver={(e) => handleDragOver(e, day, session, periodIndex, grade, className)}
                                                                onDrop={(e) => handleDrop(e, day, session, periodIndex, grade, className)}
                                                                style={{
                                                                    cursor: period ? 'move' : 'default',
                                                                    backgroundColor:
                                                                        draggedOverItem?.day === day &&
                                                                            draggedOverItem?.session === session &&
                                                                            draggedOverItem?.periodIndex === periodIndex ?
                                                                            '#e0e0e0' : undefined
                                                                }}
                                                            >
                                                                {period ? (
                                                                    `${getSubjectName(period.subject_Id)}${showTeacherName ? " - " + getTeacherName(period.teacher_id) : ""}`
                                                                ) : (
                                                                    <div
                                                                        className="empty-cell"
                                                                        onClick={() => handleEmptyCellClick(day, session, periodIndex, grade, className)}
                                                                    >
                                                                        Click để thêm tiết học
                                                                    </div>
                                                                )}
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

            <ScheduleModal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                onConfirm={handleModalConfirm}
            />

            {/* Thêm thùng rác cố định ở góc màn hình */}
            <div
                className={`floating-trash ${isDraggingOverTrash ? 'trash-active' : ''} ${draggedItem ? 'trash-visible' : ''}`}
                onDragOver={handleTrashDragOver}
                onDragLeave={handleTrashDragLeave}
                onDrop={handleTrashDrop}
            >
                <Trash2 size={24} />
            </div>
        </div>
    );
};

export default ScheduleTable;
