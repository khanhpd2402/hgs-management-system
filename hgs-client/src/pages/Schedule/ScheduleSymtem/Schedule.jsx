import React, { useState, useRef, useEffect } from 'react';
import { useTimetableForPrincipal } from '../../../services/schedule/queries';
import { useTeachers } from '../../../services/teacher/queries';
import { useSubjects } from '@/services/common/queries';
import './Schedule.scss';
import { Calendar, Save, Trash2, Edit } from "lucide-react";
import ExportSchedule from './ExportSchedule';
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd';
import { useQueryClient } from '@tanstack/react-query';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog";
import { toast } from 'react-toastify';  // Thêm import này ở đầu file
import { useDeleteTimeTableDetail } from '@/services/schedule/mutation';

const Schedule = () => {
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


    const [selectedTeacherId, setSelectedTeacherId] = useState('');
    const [selectedSubjectId, setSelectedSubjectId] = useState('');
    const [selectedSchedule, setSelectedSchedule] = useState(null);
    const [showEditDialog, setShowEditDialog] = useState(false);
    // State declarations (moved selectedSemester here and removed duplicate)
    const [selectedSemester, setSelectedSemester] = useState(() => {
        const savedSemester = localStorage.getItem('selectedSemester');
        return savedSemester ? parseInt(savedSemester) : null;
    });
    const [selectedGrade, setSelectedGrade] = useState('');
    const [selectedClass, setSelectedClass] = useState('');
    const [selectedTeacher, setSelectedTeacher] = useState('');
    const [selectedSubject, setSelectedSubject] = useState('');
    const [selectedSession, setSelectedSession] = useState('');
    const [showTeacherName, setShowTeacherName] = useState(true);
    const [tempTeacher, setTempTeacher] = useState('');
    const [tempGrade, setTempGrade] = useState('');
    const [tempClass, setTempClass] = useState('');
    const [tempSubject, setTempSubject] = useState('');
    const [tempSession, setTempSession] = useState('');
    const [filteredSchedule, setFilteredSchedule] = useState(null);

    const topScrollRef = useRef(null);
    const bottomScrollRef = useRef(null);
    const queryClient = useQueryClient();

    const syncScroll = (sourceRef, targetRef) => {
        if (!sourceRef.current || !targetRef.current) return;
        targetRef.current.scrollLeft = sourceRef.current.scrollLeft;
    };




    const handleCellClick = (day, periodId, className) => {
        const scheduleToUse = filteredSchedule || scheduleData?.[0];
        if (!scheduleToUse?.details) return;

        const schedule = scheduleToUse.details.find(
            item =>
                item.dayOfWeek === day &&
                item.periodId === periodId &&
                item.className === className
        );

        setSelectedSchedule({
            day,
            periodId,
            className,
            currentSubject: schedule?.subjectId || '',
            currentTeacher: schedule?.teacherId || '',
            timetableDetailId: schedule?.timetableDetailId || null  // Thêm dòng này
        });
        setSelectedSubjectId(schedule?.subjectId || '');
        setSelectedTeacherId(schedule?.teacherId || '');
        setShowEditDialog(true);
    };

    const handleScheduleUpdate = () => {
        if (!selectedSchedule) return;

        const scheduleToUse = filteredSchedule || scheduleData?.[0];
        if (!scheduleToUse?.details) return;

        const updatedDetails = [...scheduleToUse.details];
        const scheduleIndex = updatedDetails.findIndex(
            item =>
                item.dayOfWeek === selectedSchedule.day &&
                item.periodId === selectedSchedule.periodId &&
                item.className === selectedSchedule.className
        );

        const selectedSubject = subjects.find(s => s.subjectId === parseInt(selectedSubjectId));
        const selectedTeacher = teachers.find(t => t.teacherId === parseInt(selectedTeacherId));

        if (scheduleIndex !== -1) {
            updatedDetails[scheduleIndex] = {
                ...updatedDetails[scheduleIndex],
                subjectId: selectedSubjectId,
                teacherId: selectedTeacherId,
                subjectName: selectedSubject?.subjectName || '',
                teacherName: selectedTeacher?.fullName || ''
            };
        } else {
            updatedDetails.push({
                dayOfWeek: selectedSchedule.day,
                periodId: selectedSchedule.periodId,
                className: selectedSchedule.className,
                subjectId: selectedSubjectId,
                teacherId: selectedTeacherId,
                subjectName: selectedSubject?.subjectName || '',
                teacherName: selectedTeacher?.fullName || ''
            });
        }

        if (filteredSchedule) {
            setFilteredSchedule({
                ...filteredSchedule,
                details: updatedDetails
            });
        } else {
            scheduleData[0].details = updatedDetails;
        }

        setShowEditDialog(false);
    };

    const deleteTimeTableDetailMutation = useDeleteTimeTableDetail();

    const handleDelete = async () => {
        if (!selectedSchedule) return;

        const scheduleToUse = filteredSchedule || scheduleData?.[0];
        if (!scheduleToUse?.details) return;

        const scheduleDetail = scheduleToUse.details.find(
            item =>
                item.dayOfWeek === selectedSchedule.day &&
                item.periodId === selectedSchedule.periodId &&
                item.className === selectedSchedule.className
        );

        if (!scheduleDetail) {
            toast.error('Không tìm thấy chi tiết thời khóa biểu');
            return;
        }

        try {
            await deleteTimeTableDetailMutation.mutateAsync(scheduleDetail.timetableDetailId);

            toast.success('Xóa thời khóa biểu thành công');

            const updatedDetails = scheduleToUse.details.filter(
                item => item.timetableDetailId !== scheduleDetail.timetableDetailId
            );

            if (filteredSchedule) {
                setFilteredSchedule({
                    ...filteredSchedule,
                    details: updatedDetails
                });
            } else {
                scheduleData[0].details = updatedDetails;
            }

            setShowEditDialog(false);
        } catch (error) {
            console.error('Lỗi khi xóa thời khóa biểu:', error);
            toast.error('Có lỗi xảy ra khi xóa thời khóa biểu');
        }
    };

    // Fetch semesters from localStorage
    const getSemesters = () => {
        try {
            const storedSemesters = localStorage.getItem('semesters');
            return storedSemesters ? JSON.parse(storedSemesters) : [];
        } catch (e) {
            console.error('Failed to parse semesters from localStorage:', e);
            return [];
        }
    };

    const semesters = getSemesters();
    const subjectData = [];
    const grades = ['6', '7', '8', '9'];

    // Set default semester if none selected
    useEffect(() => {
        if (!selectedSemester && semesters.length > 0) {
            setSelectedSemester(semesters[0].semesterID);
        }
    }, [selectedSemester, semesters]);

    // Save selected semester to localStorage and refetch data
    useEffect(() => {
        if (selectedSemester) {
            localStorage.setItem('selectedSemester', selectedSemester);
            queryClient.invalidateQueries(['timetable', selectedSemester]);
        }
    }, [selectedSemester, queryClient]);

    const handleSearch = () => {
        if (!scheduleData?.[0]?.details) return;

        setSelectedTeacher(tempTeacher);
        setSelectedGrade(tempGrade);
        setSelectedClass(tempClass);
        setSelectedSubject(tempSubject);
        setSelectedSession(tempSession);

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

    const handleReset = () => {
        setTempTeacher('');
        setTempGrade('');
        setTempClass('');
        setTempSubject('');
        setTempSession('');
        setSelectedTeacher('');
        setSelectedGrade('');
        setSelectedClass('');
        setSelectedSubject('');
        setSelectedSession('');
        setFilteredSchedule(null);
    };

    const getSchedule = (day, periodId, className, classIndex) => {
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
                <Draggable
                    key={`${className}-${day}-${periodId}`}
                    draggableId={`${className}-${day}-${periodId}`}
                    index={periodId}
                >
                    {(provided) => (
                        <div
                            className="schedule-cell"
                            ref={provided.innerRef}
                            {...provided.draggableProps}
                            {...provided.dragHandleProps}
                            onClick={(e) => {
                                e.stopPropagation();
                                handleCellClick(day, periodId, className);
                            }}
                        >
                            <div className="subject">{schedule.subjectName}</div>
                            {showTeacherName && <div className="teacher">{schedule.teacherName}</div>}
                        </div>
                    )}
                </Draggable>
            );
        }
        return <div className="schedule-cell"></div>;
    };

    const onDragEnd = (result, className) => {
        const { source, destination } = result;

        if (!destination || (source.droppableId === destination.droppableId && source.index === destination.index)) {
            return;
        }

        const scheduleToUse = filteredSchedule || scheduleData?.[0];
        if (!scheduleToUse?.details) return;

        const updatedDetails = [...scheduleToUse.details];
        const sourceItem = updatedDetails.find(
            item => item.dayOfWeek === daysOfWeek[source.droppableId.split('-')[1]] &&
                item.periodId === source.index &&
                item.className === className
        );
        const destItem = updatedDetails.find(
            item => item.dayOfWeek === daysOfWeek[destination.droppableId.split('-')[1]] &&
                item.periodId === destination.index &&
                item.className === className
        );

        if (sourceItem && destItem) {
            const sourcePeriodId = sourceItem.periodId;
            sourceItem.periodId = destItem.periodId;
            destItem.periodId = sourcePeriodId;
        } else if (sourceItem) {
            sourceItem.periodId = destination.index;
        }

        if (filteredSchedule) {
            setFilteredSchedule({
                ...filteredSchedule,
                details: updatedDetails
            });
        } else {
            scheduleData[0].details = updatedDetails;
        }
    };

    // Fetch timetable data with selected semester
    const { data: scheduleData, isLoading: scheduleLoading } = useTimetableForPrincipal(selectedSemester);
    const { data: teachersResponse = { teachers: [] }, isLoading: teachersLoading } = useTeachers();
    const { data: subjects = [], isLoading: subjectsLoading } = useSubjects();
    const teachers = Array.isArray(teachersResponse) ? teachersResponse : teachersResponse.teachers || [];

    // Get startDate and endDate for the selected semester
    const selectedSemesterData = semesters.find(s => s.semesterID === selectedSemester);

    if (scheduleLoading || teachersLoading || subjectsLoading) {
        return <div className="loading">Đang tải...</div>;
    }

    const currentSchedule = scheduleData?.[0];

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

    const getFilteredShifts = () => {
        if (!filteredSchedule?.selectedSession) return shifts;
        return shifts.filter(shift =>
            (filteredSchedule.selectedSession === 'Morning' && shift.name === 'Sáng') ||
            (filteredSchedule.selectedSession === 'Afternoon' && shift.name === 'Chiều')
        );
    };

    return (
        <div className="schedule-container">
            <div className="sticky-filter">
                <div className="filter-container">
                    {/* Phần filter giữ nguyên */}
                    <div className="filter-row">
                        <div className="filter-column">
                            <label>Học kỳ</label>
                            <select
                                value={selectedSemester || ''}
                                onChange={(e) => setSelectedSemester(parseInt(e.target.value))}
                                disabled={semesters.length === 0}
                            >
                                {semesters.length === 0 && <option value="">Không có học kỳ</option>}
                                {semesters.map(semester => (
                                    <option key={semester.semesterID} value={semester.semesterID}>
                                        {semester.semesterName}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div className="filter-column">
                            <label>Ngày áp dụng</label>
                            <input
                                type="text"
                                value={
                                    selectedSemesterData?.startDate && selectedSemesterData?.endDate
                                        ? `Từ ${formatDate(selectedSemesterData.startDate)} đến ${formatDate(selectedSemesterData.endDate)}`
                                        : "Đang tải..."
                                }
                                readOnly
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
                                    <option key={subject.subjectId} value={subject.subjectID}>
                                        {subject.subjectName}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </div>
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
                            schedule={filteredSchedule || scheduleData?.[0]}
                            showTeacherName={showTeacherName}
                        />
                        <button className="btn-save">
                            <Save size={16} /> Lưu
                        </button>
                        <button className="btn-delete">
                            <Trash2 size={16} /> Xóa
                        </button>
                    </div>
                </div>
            </div>
            <DragDropContext onDragEnd={(result) => {
                const className = result.source.droppableId.split('-')[0];
                onDragEnd(result, className);
            }}>
                <div className="table-container" ref={topScrollRef} onScroll={() => syncScroll(topScrollRef, bottomScrollRef)}>
                    <div className="timetable-table dummy-scroll" />
                    <br />
                    <br />
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
                                            <td className="sticky-col col-3">{getPeriodName(period)}</td>
                                            {(selectedClass ? [selectedClass] : classes).map((className, classIndex) => (
                                                <Droppable
                                                    key={`${className}-${dayIndex}`}
                                                    droppableId={`${className}-${dayIndex}`}
                                                >
                                                    {(provided) => (
                                                        <td
                                                            ref={provided.innerRef}
                                                            {...provided.draggableProps}
                                                        >
                                                            {getSchedule(day, period, className, classIndex)}
                                                            {provided.placeholder}
                                                        </td>
                                                    )}
                                                </Droppable>
                                            ))}
                                        </tr>
                                    ))
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            </DragDropContext>


            {/* Tách riêng phần Dialog ra khỏi phần filter */}
            <Dialog
                open={showEditDialog}
                onOpenChange={setShowEditDialog}
                className="schedule-edit-dialog"
            >
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle className="schedule-edit-title">
                            Chỉnh sửa thời khóa biểu
                        </DialogTitle>
                    </DialogHeader>

                    <div className="schedule-edit-info">
                        <div className="info-row">
                            <span className="info-label">Detail:</span>
                            <span className="info-value">{selectedSchedule?.timetableDetailId}</span>
                        </div>

                        <div className="info-row">
                            <span className="info-label">Thứ:</span>
                            <span className="info-value">{selectedSchedule?.day}</span>
                        </div>
                        <div className="info-row">
                            <span className="info-label">Tiết:</span>
                            <span className="info-value">{selectedSchedule?.periodId}</span>
                        </div>
                        <div className="info-row">
                            <span className="info-label">Lớp:</span>
                            <span className="info-value">{selectedSchedule?.className}</span>
                        </div>
                    </div>

                    <div className="schedule-edit-form">
                        <div className="form-group">
                            <label>Môn học:</label>
                            <select
                                value={selectedSubjectId}
                                onChange={(e) => setSelectedSubjectId(e.target.value)}
                                className="form-select"
                            >
                                <option value="">Chọn môn học</option>
                                {subjects.map(subject => (
                                    <option key={subject.subjectId} value={subject.subjectId}>
                                        {subject.subjectName}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="form-group">
                            <label>Giáo viên:</label>
                            <select
                                value={selectedTeacherId}
                                onChange={(e) => setSelectedTeacherId(e.target.value)}
                                className="form-select"
                            >
                                <option value="">Chọn giáo viên</option>
                                {teachers.map(teacher => (
                                    <option key={teacher.teacherId} value={teacher.teacherId}>
                                        {teacher.fullName}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="schedule-edit-actions">
                            <button
                                onClick={handleScheduleUpdate}
                                className="btn-save"
                            >
                                <Save size={16} />
                                Lưu thay đổi
                            </button>
                            <button
                                onClick={handleDelete}
                                className="btn-delete"
                            >
                                <Trash2 size={16} />
                                Xóa
                            </button>
                        </div>
                    </div>
                </DialogContent>
            </Dialog>

            {/* Phần còn lại của component */}
            <Dialog open={showEditDialog} onOpenChange={setShowEditDialog}>
                <DialogContent className="edit-schedule-dialog">
                    <DialogHeader>
                        <DialogTitle>Chỉnh sửa thời khóa biểu - {selectedSchedule?.className}</DialogTitle>
                        <div className="schedule-info">

                            <p>Detail id: {selectedSchedule?.timetableDetailId}</p>

                            <p>Tiết: {selectedSchedule?.periodId}</p>
                            <p>Thứ: {selectedSchedule?.day}</p>
                            <p>Lớp: {selectedSchedule?.className}</p>
                        </div>
                    </DialogHeader>
                    <div className="edit-schedule-form">
                        <div className="form-group">
                            <label>Môn học</label>
                            <select
                                value={selectedSubjectId}
                                onChange={(e) => setSelectedSubjectId(e.target.value)}
                            >
                                <option value="">Chọn môn học</option>
                                {subjects.map(subject => (
                                    <option key={subject.subjectId} value={subject.subjectId}>
                                        {subject.subjectName}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div className="form-group">
                            <label>Giáo viên</label>
                            <select
                                value={selectedTeacherId}
                                onChange={(e) => setSelectedTeacherId(e.target.value)}
                            >
                                <option value="">Chọn giáo viên</option>
                                {teachers.map(teacher => (
                                    <option key={teacher.teacherId} value={teacher.teacherId}>
                                        {teacher.fullName}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div className="form-actions">
                            <button onClick={handleScheduleUpdate} className="save-btn">
                                Lưu thay đổi
                            </button>
                            <button onClick={handleDelete} className="delete-btn">
                                Xóa môn học
                            </button>
                            <button onClick={() => setShowEditDialog(false)} className="cancel-btn">
                                Hủy
                            </button>
                        </div>
                    </div>
                </DialogContent>
            </Dialog>
        </div>
    );
};

export default Schedule;