import React, { useState, useRef, useEffect, memo } from 'react';
import { useTimetableForPrincipal } from '../../../services/schedule/queries';
import { useTeachers } from '../../../services/teacher/queries';
import { useSubjects } from '@/services/common/queries';
import { useAcademicYears } from '@/services/common/queries';
import './Schedule.scss';
import { Calendar, Save, Trash2, Plus } from 'lucide-react';
import ExportSchedule from './ExportSchedule';
import { DragDropContext, Droppable, Draggable } from '@hello-pangea/dnd';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { toast, ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { useDeleteTimeTableDetail } from '@/services/schedule/mutation';
import { getSemesterByYear } from '../../../services/schedule/api';
import { Link } from 'react-router-dom';


// Constants
const DAYS_OF_WEEK = ['Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy', 'Chủ Nhật'];
const SHIFTS = [
    { name: 'Sáng', periods: [1, 2, 3, 4, 5] },
    { name: 'Chiều', periods: [6, 7, 8] },
];
const GRADES = ['6', '7', '8', '9'];

// Fetch classes from API
const fetchClasses = async () => {
    const response = await fetch('https://localhost:8386/api/Classes', {
        headers: { 'Content-Type': 'application/json' },
    });
    if (!response.ok) throw new Error('Không thể lấy danh sách lớp');
    return response.json();
};

const FilterSection = memo(({
    selectedYear, setSelectedYear, selectedSemester, setSelectedSemester, semesters,
    tempTeacher, setTempTeacher, tempGrade, setTempGrade, tempClass, setTempClass,
    tempSubject, setTempSubject, tempSession, setTempSession,
    academicYears, teachers, subjects, getFilteredClasses, handleSearch, handleReset
}) => (
    <div className="sticky-filter">
        <div className="filter-container">
            <div className="filter-row">
                <FilterSelect label="Năm học" value={selectedYear} onChange={(e) => setSelectedYear(parseInt(e.target.value))} options={academicYears?.map(year => ({ value: year.academicYearID, label: year.yearName }))} />
                <FilterSelect label="Học kỳ" value={selectedSemester} onChange={(e) => setSelectedSemester(parseInt(e.target.value))} options={semesters.map(semester => ({ value: semester.semesterID, label: semester.semesterName }))} disabled={!selectedYear || !semesters.length} />
                <FilterInput label="Ngày áp dụng" value={semesters.find(s => s.semesterID === selectedSemester)?.startDate ?
                    `Từ ${formatDate(semesters.find(s => s.semesterID === selectedSemester).startDate)} đến ${formatDate(semesters.find(s => s.semesterID === selectedSemester).endDate)}` : 'Đang tải...'} readOnly />
                <FilterSelect label="Giáo viên" value={tempTeacher} onChange={(e) => setTempTeacher(e.target.value)} options={teachers.map(teacher => ({ value: teacher.teacherId, label: teacher.fullName }))} />
            </div>
            <div className="filter-row">
                <FilterSelect label="Khối" value={tempGrade} onChange={(e) => setTempGrade(e.target.value)} options={GRADES.map(grade => ({ value: grade, label: `Khối ${grade}` }))} />
                <FilterSelect label="Lớp" value={tempClass} onChange={(e) => setTempClass(e.target.value)} options={getFilteredClasses().map(cls => ({ value: cls.className, label: cls.className }))} disabled={!tempGrade} />
                <FilterSelect label="Môn học" value={tempSubject} onChange={(e) => setTempSubject(e.target.value)} options={subjects.map(subject => ({ value: subject.subjectID, label: subject.subjectName }))} />
                <FilterSelect label="Chọn buổi" value={tempSession} onChange={(e) => setTempSession(e.target.value)} options={[{ value: 'Morning', label: 'Sáng' }, { value: 'Afternoon', label: 'Chiều' }]} />
            </div>
            <div className="filter-row">
                <div className="filter-column search-button">
                    <button onClick={handleSearch}>Tìm kiếm</button>
                    <button onClick={handleReset} className="reset-button">Reset</button>
                </div>
            </div>
        </div>
    </div>
));

const ScheduleTable = memo(({
    selectedClass, getClassesByGrade, getUniqueClasses, getFilteredShifts,
    showTeacherName, setShowTeacherName, filteredSchedule, scheduleData,
    isViewMode, setIsViewMode, getSchedule
}) => (
    <div className="table-container">
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
                    {(selectedClass ? [selectedClass] : getUniqueClasses().map(cls => cls.className)).map(className => (
                        <th key={className}>{className}</th>
                    ))}
                </tr>
            </thead>
            <tbody>
                {DAYS_OF_WEEK.map((day, dayIndex) => {
                    const shiftsToShow = getFilteredShifts();
                    const totalPeriods = shiftsToShow.reduce((sum, shift) => sum + shift.periods.length, 0);
                    return shiftsToShow.map((shift, shiftIndex) =>
                        shift.periods.map((period, periodIndex) => (
                            <tr key={`${day}-${shift.name}-${period}`} className={dayIndex % 2 === 0 ? 'even-day' : 'odd-day'}>
                                {shiftIndex === 0 && periodIndex === 0 && (
                                    <td className="sticky-col col-1" rowSpan={totalPeriods}>{day}</td>
                                )}
                                {periodIndex === 0 && (
                                    <td className="sticky-col col-2" rowSpan={shift.periods.length}>{shift.name}</td>
                                )}
                                <td className="sticky-col col-3">{scheduleData?.[0]?.details.find(item => item.periodId === period)?.periodName || `Tiết ${period}`}</td>
                                {(selectedClass ? [selectedClass] : getUniqueClasses().map(cls => cls.className)).map((className, classIndex) => (
                                    <Droppable key={`${className}-${dayIndex}`} droppableId={`${className}-${dayIndex}`}>
                                        {(provided) => (
                                            <td ref={provided.innerRef} {...provided.droppableProps}>
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
));

const EditDialog = memo(({
    showEditDialog, setShowEditDialog, selectedSchedule, selectedSubjectId,
    setSelectedSubjectId, selectedTeacherId, setSelectedTeacherId,
    subjects, teachers, handleScheduleUpdate, handleDelete
}) => (
    <Dialog open={showEditDialog} onOpenChange={setShowEditDialog}>
        <DialogContent className="schedule-edit-content p-6">
            <DialogHeader className="schedule-edit-header mb-6">
                <DialogTitle className="schedule-edit-title text-2xl font-semibold text-center text-primary border-b pb-4">
                    Chỉnh sửa thời khóa biểu
                </DialogTitle>
            </DialogHeader>
            <div className="schedule-edit-info bg-gray-50 rounded-lg p-6 mb-6 space-y-4">
                <InfoRow label="Thứ" value={selectedSchedule?.day} />
                <InfoRow label="Tiết" value={selectedSchedule?.periodId} />
                <InfoRow label="Lớp" value={`${selectedSchedule?.className}---${selectedSchedule?.classId}`} />
            </div>
            <div className="schedule-edit-form space-y-6">
                <FilterSelect
                    label="Môn học"
                    value={selectedSubjectId}
                    onChange={(e) => setSelectedSubjectId(e.target.value)}
                    options={subjects.map(subject => ({ value: subject.subjectID, label: `${subject.subjectName} --${subject.subjectID}` }))}
                />
                <FilterSelect
                    label="Giáo viên"
                    value={selectedTeacherId}
                    onChange={(e) => setSelectedTeacherId(e.target.value)}
                    options={teachers.map(teacher => ({ value: teacher.teacherId, label: `${teacher.fullName}---${teacher.teacherId}` }))}
                />
                <div className="schedule-edit-actions flex gap-4 mt-8">
                    <button onClick={handleScheduleUpdate} className="btn-save flex-1 inline-flex items-center justify-center gap-2 px-4 py-2 bg-primary text-white rounded-md hover:bg-primary-dark transition-colors">
                        <Save size={16} /> Lưu thay đổi
                    </button>
                    <button onClick={handleDelete} className="btn-delete flex-1 inline-flex items-center justify-center gap-2 px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 transition-colors">
                        <Trash2 size={16} /> Xóa
                    </button>
                </div>
            </div>
        </DialogContent>
    </Dialog>
));

const AddDetailDialog = memo(({
    showAddDialog, setShowAddDialog, classes, days, periods, subjects, teachers,
    handleAddDetail
}) => {
    const [selectedClass, setSelectedClass] = useState('');
    const [selectedDay, setSelectedDay] = useState('');
    const [selectedPeriod, setSelectedPeriod] = useState('');
    const [selectedSubject, setSelectedSubject] = useState('');
    const [selectedTeacher, setSelectedTeacher] = useState('');

    const handleSubmit = () => {
        if (!selectedClass || !selectedDay || !selectedPeriod || !selectedSubject || !selectedTeacher) {
            toast.error('Vui lòng chọn đầy đủ thông tin');
            return;
        }
        const [className, classId] = selectedClass.split('---');
        handleAddDetail({
            classId,
            className,
            dayOfWeek: selectedDay,
            periodId: parseInt(selectedPeriod),
            subjectId: selectedSubject,
            teacherId: selectedTeacher
        });
        setShowAddDialog(false);
        setSelectedClass('');
        setSelectedDay('');
        setSelectedPeriod('');
        setSelectedSubject('');
        setSelectedTeacher('');
    };

    return (
        <Dialog open={showAddDialog} onOpenChange={setShowAddDialog}>
            <DialogContent className="add-dialog-content">
                <DialogHeader className="schedule-add-header mb-6">
                    <DialogTitle className="schedule-add-title text-2xl font-semibold text-center text-primary border-b pb-4">
                        Thêm tiết học mới
                    </DialogTitle>
                </DialogHeader>
                <div className="schedule-add-form space-y-6">
                    <FilterSelect
                        label="Lớp"
                        value={selectedClass}
                        onChange={(e) => setSelectedClass(e.target.value)}
                        options={classes.map(cls => ({
                            value: `${cls.className}---${cls.classId}`,
                            label: cls.className
                        }))}
                    />
                    <FilterSelect
                        label="Thứ"
                        value={selectedDay}
                        onChange={(e) => setSelectedDay(e.target.value)}
                        options={days.map(day => ({ value: day, label: day }))}
                    />
                    <FilterSelect
                        label="Tiết"
                        value={selectedPeriod}
                        onChange={(e) => setSelectedPeriod(e.target.value)}
                        options={periods.map(period => ({ value: period, label: `Tiết ${period}` }))}
                    />
                    <FilterSelect
                        label="Môn học"
                        value={selectedSubject}
                        onChange={(e) => setSelectedSubject(e.target.value)}
                        options={subjects.map(subject => ({ value: subject.subjectID, label: `${subject.subjectName} --${subject.subjectID}` }))}
                    />
                    <FilterSelect
                        label="Giáo viên"
                        value={selectedTeacher}
                        onChange={(e) => setSelectedTeacher(e.target.value)}
                        options={teachers.map(teacher => ({ value: teacher.teacherId, label: `${teacher.fullName}---${teacher.teacherId}` }))}
                    />
                    <div className="schedule-add-actions flex gap-4 mt-8">
                        <button onClick={handleSubmit} className="btn-save flex-1 inline-flex items-center justify-center gap-2 px-4 py-2 bg-primary text-white rounded-md hover:bg-primary-dark transition-colors">
                            <Save size={16} /> Thêm tiết học
                        </button>
                        <button onClick={() => setShowAddDialog(false)} className="btn-cancel flex-1 inline-flex items-center justify-center gap-2 px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 transition-colors">
                            Hủy
                        </button>
                    </div>
                </div>
            </DialogContent>
        </Dialog>
    );
});

const FilterSelect = ({ label, value, onChange, options, disabled }) => (
    <div className="filter-column">
        <label>{label}</label>
        <select value={value || ''} onChange={onChange} disabled={disabled}>
            <option value="">Chọn {label.toLowerCase()}</option>
            {options.map(option => (
                <option key={option.value} value={option.value}>{option.label}</option>
            ))}
        </select>
    </div>
);

const FilterInput = ({ label, value, readOnly }) => (
    <div className="filter-column">
        <label>{label}</label>
        <input type="text" value={value} readOnly={readOnly} />
    </div>
);

const InfoRow = ({ label, value }) => (
    <div className="info-row flex justify-between items-center py-3 border-b border-gray-200 last:border-0">
        <span className="info-label font-medium text-gray-600">{label}:</span>
        <span className="info-value font-medium text-gray-900">{value}</span>
    </div>
);

const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
};

const Schedule = () => {
    // State
    const [selectedYear, setSelectedYear] = useState('');
    const [selectedSemester, setSelectedSemester] = useState(() => localStorage.getItem('selectedSemester') ? parseInt(localStorage.getItem('selectedSemester')) : null);
    const [semesters, setSemesters] = useState([]);
    const [selectedTeacherId, setSelectedTeacherId] = useState('');
    const [selectedSubjectId, setSelectedSubjectId] = useState('');
    const [selectedSchedule, setSelectedSchedule] = useState(null);
    const [showEditDialog, setShowEditDialog] = useState(false);
    const [showAddDialog, setShowAddDialog] = useState(false);
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
    const [isViewMode, setIsViewMode] = useState(true);

    // Refs and Query Client
    const topScrollRef = useRef(null);
    const bottomScrollRef = useRef(null);
    const queryClient = useQueryClient();

    // Data Fetching
    const { data: scheduleData = [{ details: [] }], isLoading: scheduleLoading } = useTimetableForPrincipal(selectedSemester);
    const { data: teachersResponse = { teachers: [] }, isLoading: teachersLoading } = useTeachers();
    const { data: subjects = [], isLoading: subjectsLoading } = useSubjects();
    const { data: academicYears } = useAcademicYears();
    const { data: classes = [], isLoading: classesLoading } = useQuery({
        queryKey: ['classes'],
        queryFn: fetchClasses,
        enabled: true,
    });
    const teachers = Array.isArray(teachersResponse) ? teachersResponse : teachersResponse.teachers || [];
    const deleteTimeTableDetailMutation = useDeleteTimeTableDetail();

    // Utility Functions
    const syncScroll = (sourceRef, targetRef) => {
        if (sourceRef.current && targetRef.current) {
            targetRef.current.scrollLeft = sourceRef.current.scrollLeft;
        }
    };

    const getUniqueClasses = () => {
        if (!scheduleData?.[0]?.details) return [];
        const classMap = new Map();
        scheduleData[0].details.forEach(detail => {
            if (detail.className && detail.classId) {
                classMap.set(detail.className, {
                    className: detail.className,
                    classId: detail.classId
                });
            } else {
                console.warn('Invalid class data in scheduleData:', detail);
            }
        });
        return Array.from(classMap.values()).sort((a, b) => a.className.localeCompare(b.className));
    };

    const getFilteredClasses = () => {
        return tempGrade
            ? getUniqueClasses().filter(cls => cls.className.startsWith(tempGrade))
            : getUniqueClasses();
    };

    const getClassesByGrade = () => {
        return getUniqueClasses().reduce((acc, cls) => {
            const grade = cls.className.charAt(0);
            acc[grade] = acc[grade] || [];
            acc[grade].push(cls.className);
            return acc;
        }, {});
    };

    const getFilteredShifts = () => {
        if (!filteredSchedule?.selectedSession) return SHIFTS;
        return SHIFTS.filter(shift =>
            (filteredSchedule.selectedSession === 'Morning' && shift.name === 'Sáng') ||
            (filteredSchedule.selectedSession === 'Afternoon' && shift.name === 'Chiều')
        );
    };

    const getPeriods = () => {
        return SHIFTS.reduce((acc, shift) => [...acc, ...shift.periods], []);
    };

    // Handlers
    const handleCellClick = (day, periodId, className, classIndex) => {
        if (isViewMode) return;
        const scheduleToUse = filteredSchedule || scheduleData?.[0];
        const schedule = scheduleToUse.details.find(
            item => item.dayOfWeek === day && item.periodId === periodId && item.className === className
        );
        const classDetail = classes.find(cls => cls.className === className);
        const classId = classDetail?.classId || getUniqueClasses().find(cls => cls.className === className)?.classId;

        setSelectedSchedule({
            day,
            periodId,
            className,
            classId,
            currentSubject: schedule?.subjectId || '',
            currentTeacher: schedule?.teacherId || '',
            timetableDetailId: schedule?.timetableDetailId || null,
        });
        setSelectedSubjectId(schedule?.subjectId || '');
        setSelectedTeacherId(schedule?.teacherId || '');
        setShowEditDialog(true);
    };

    const handleScheduleUpdate = async () => {
        if (!selectedSchedule) {
            toast.error('Không có thời khóa biểu được chọn để cập nhật');
            return;
        }
        const scheduleToUse = filteredSchedule || scheduleData?.[0];

        if (!selectedSubjectId || !selectedTeacherId) {
            toast.error('Vui lòng chọn môn học và giáo viên');
            return;
        }

        const payload = {
            timetableId: scheduleToUse.timetableId || 0,
            classId: selectedSchedule.classId,
            subjectId: parseInt(selectedSubjectId) || 0,
            teacherId: parseInt(selectedTeacherId) || 0,
            dayOfWeek: selectedSchedule.day,
            periodId: selectedSchedule.periodId
        };

        try {
            const url = selectedSchedule.timetableDetailId ?
                'https://localhost:8386/api/Timetables/details' :
                'https://localhost:8386/api/Timetables/create-timetable-detail';
            const method = selectedSchedule.timetableDetailId ? 'PUT' : 'POST';

            const response = await fetch(url, {
                method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(selectedSchedule.timetableDetailId ? { timetableId: scheduleToUse.timetableId, details: [payload] } : payload),
            });

            if (!response.ok) throw new Error('Không thể cập nhật thời khóa biểu');

            toast.success(selectedSchedule.timetableDetailId ? 'Cập nhật thời khóa biểu thành công' : 'Tạo mới thời khóa biểu thành công');
            await queryClient.invalidateQueries(['timetable', selectedSemester]);

            if (filteredSchedule) handleSearch();
            setShowEditDialog(false);
        } catch (error) {
            console.error('Lỗi khi cập nhật thời khóa biểu:', error);
            toast.error('Có lỗi xảy ra khi cập nhật thời khóa biểu');
        }
    };

    const handleAddDetail = async (detail) => {
        const scheduleToUse = filteredSchedule || scheduleData?.[0];
        const payload = {
            timetableId: scheduleToUse.timetableId || 0,
            classId: detail.classId,
            subjectId: parseInt(detail.subjectId) || 0,
            teacherId: parseInt(detail.teacherId) || 0,
            dayOfWeek: detail.dayOfWeek,
            periodId: detail.periodId
        };

        try {
            const response = await fetch('https://localhost:8386/api/Timetables/create-timetable-detail', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload),
            });

            if (!response.ok) throw new Error('Không thể tạo tiết học');

            toast.success('Tạo tiết học thành công');
            await queryClient.invalidateQueries(['timetable', selectedSemester]);

            if (filteredSchedule) handleSearch();
        } catch (error) {
            console.error('Lỗi khi tạo tiết học:', error);
            toast.error('Có lỗi xảy ra khi tạo tiết học');
        }
    };

    const handleDelete = async () => {
        if (!selectedSchedule?.timetableDetailId) {
            toast.error('Không có thời khóa biểu để xóa');
            return;
        }

        try {
            await deleteTimeTableDetailMutation.mutateAsync(selectedSchedule.timetableDetailId);
            toast.success('Xóa thời khóa biểu thành công');

            const updatedDetails = (filteredSchedule || scheduleData[0]).details.filter(
                item => item.timetableDetailId !== selectedSchedule.timetableDetailId
            );

            if (filteredSchedule) {
                setFilteredSchedule({ ...filteredSchedule, details: updatedDetails });
            } else {
                queryClient.setQueryData(['timetable', selectedSemester], oldData =>
                    oldData?.[0] ? [{ ...oldData[0], details: updatedDetails }] : oldData
                );
            }

            setShowEditDialog(false);
        } catch (error) {
            console.error('Lỗi khi xóa thời khóa biểu:', error);
            toast.error('Có lỗi xảy ra khi xóa thời khóa biểu');
        }
    };

    const handleSearch = () => {
        if (!scheduleData?.[0]?.details) return;

        setSelectedTeacher(tempTeacher);
        setSelectedGrade(tempGrade);
        setSelectedClass(tempClass);
        setSelectedSubject(tempSubject);
        setSelectedSession(tempSession);

        let filtered = scheduleData[0].details;

        if (tempTeacher) filtered = filtered.filter(item => parseInt(item.teacherId) === parseInt(tempTeacher));
        if (tempGrade) filtered = filtered.filter(item => item.className.charAt(0) === tempGrade);
        if (tempClass) filtered = filtered.filter(item => item.className === tempClass);
        if (tempSubject) filtered = filtered.filter(item => parseInt(item.subjectId) === parseInt(tempSubject));
        if (tempSession) {
            const periods = tempSession === 'Morning' ? [1, 2, 3, 4, 5] : [6, 7, 8];
            filtered = filtered.filter(item => periods.includes(parseInt(item.periodId)));
        }

        setFilteredSchedule(filtered.length > 0 ? { ...scheduleData[0], details: filtered, selectedSession: tempSession } : null);
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
        if (!scheduleToUse?.details) return <div className="schedule-cell"></div>;

        const schedule = scheduleToUse.details.find(
            item => item.dayOfWeek === day && item.periodId === periodId && item.className === className
        );

        const classDetail = classes.find(cls => cls.className === className);
        const classId = classDetail?.classId || getUniqueClasses().find(cls => cls.className === className)?.classId || className;

        if (schedule) {
            return (
                <Draggable
                    key={`${className}-${day}-${periodId}`}
                    draggableId={`${className}-${day}-${periodId}`}
                    index={periodId}
                    isDragDisabled={isViewMode}
                >
                    {(provided) => (
                        <div
                            className="schedule-cell"
                            ref={provided.innerRef}
                            {...provided.draggableProps}
                            {...provided.dragHandleProps}
                            onClick={(e) => {
                                if (!isViewMode) {
                                    e.stopPropagation();
                                    handleCellClick(day, periodId, className, classIndex);
                                }
                            }}
                        >
                            <div className="subject">{schedule.subjectName}</div>
                            {showTeacherName && <div className="teacher">{schedule.teacherName}</div>}
                        </div>
                    )}
                </Draggable>
            );
        }

        if (!isViewMode) {
            return (
                <div
                    className="schedule-cell empty-cell"
                    onClick={() => handleCellClick(day, periodId, className, classIndex)}
                >
                    <button className="add-schedule-btn">+</button>
                </div>
            );
        }

        return <div className="schedule-cell empty-cell"></div>;
    };

    const onDragEnd = (result) => {
        if (!result.destination || (result.source.droppableId === result.destination.droppableId && result.source.index === result.destination.index)) {
            return;
        }

        const className = result.source.droppableId.split('-')[0];
        const scheduleToUse = filteredSchedule || scheduleData?.[0];
        if (!scheduleToUse?.details) return;

        const updatedDetails = [...scheduleToUse.details];
        const sourceItem = updatedDetails.find(
            item => item.dayOfWeek === DAYS_OF_WEEK[parseInt(result.source.droppableId.split('-')[1])] &&
                item.periodId === result.source.index && item.className === className
        );
        const destItem = updatedDetails.find(
            item => item.dayOfWeek === DAYS_OF_WEEK[parseInt(result.destination.droppableId.split('-')[1])] &&
                item.periodId === result.destination.index && item.className === className
        );

        if (sourceItem && destItem) {
            [sourceItem.periodId, destItem.periodId] = [destItem.periodId, sourceItem.periodId];
        } else if (sourceItem) {
            sourceItem.periodId = result.destination.index;
        }

        if (filteredSchedule) {
            setFilteredSchedule({ ...filteredSchedule, details: updatedDetails });
        } else {
            queryClient.setQueryData(['timetable', selectedSemester], oldData =>
                oldData?.[0] ? [{ ...oldData[0], details: updatedDetails }] : oldData
            );
        }
    };

    // Effects
    useEffect(() => {
        if (!selectedSemester && semesters.length > 0) {
            setSelectedSemester(semesters[0].semesterID);
        }
    }, [selectedSemester, semesters]);

    useEffect(() => {
        if (selectedSemester) {
            localStorage.setItem('selectedSemester', selectedSemester);
            queryClient.invalidateQueries(['timetable', selectedSemester]);
        }
    }, [selectedSemester, queryClient]);

    useEffect(() => {
        const handleYearChange = async () => {
            if (selectedYear) {
                try {
                    const semesterData = await getSemesterByYear(selectedYear);
                    setSemesters(semesterData || []);
                    setSelectedSemester('');
                } catch (error) {
                    console.error('Lỗi khi lấy dữ liệu học kỳ:', error);
                    toast.error('Không thể lấy danh sách học kỳ');
                }
            } else {
                setSemesters([]);
                setSelectedSemester('');
            }
        };
        handleYearChange();
    }, [selectedYear]);

    if (scheduleLoading || teachersLoading || subjectsLoading || classesLoading) {
        return <div className="loading">Đang tải...</div>;
    }

    return (
        <div className="schedule-container">
            <ToastContainer
                className="schedule-toast-container"
                position="top-right"
                autoClose={3000}
                hideProgressBar={false}
                newestOnTop={false}
                closeOnClick
                rtl={false}
                pauseOnFocusLoss
                draggable
                pauseOnHover
            />
            <FilterSection
                selectedYear={selectedYear}
                setSelectedYear={setSelectedYear}
                selectedSemester={selectedSemester}
                setSelectedSemester={setSelectedSemester}
                semesters={semesters}
                tempTeacher={tempTeacher}
                setTempTeacher={setTempTeacher}
                tempGrade={tempGrade}
                setTempGrade={setTempGrade}
                tempClass={tempClass}
                setTempClass={setTempClass}
                tempSubject={tempSubject}
                setTempSubject={setTempSubject}
                tempSession={tempSession}
                setTempSession={setTempSession}
                academicYears={academicYears}
                teachers={teachers}
                subjects={subjects}
                getFilteredClasses={getFilteredClasses}
                handleSearch={handleSearch}
                handleReset={handleReset}
            />
            <div className="filter-row-table" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <Calendar size={20} />
                    <span>Thời khóa biểu</span>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
                    <button onClick={() => setShowTeacherName(!showTeacherName)}>
                        {showTeacherName ? 'Ẩn tên giáo viên' : 'Hiển thị tên giáo viên'}
                    </button>
                    <ExportSchedule schedule={filteredSchedule || scheduleData?.[0]} showTeacherName={showTeacherName} />
                    <button className="btn-save" onClick={() => setIsViewMode(!isViewMode)}>
                        <Save size={16} /> {isViewMode ? 'Sửa' : 'Xem'}
                    </button>
                    {isViewMode == false && (
                        <button className="btn-add-detail" onClick={() => setShowAddDialog(true)}>
                            <Plus size={16} /> Thêm tiết học
                        </button>
                    )}

                    <Link to="/system/timetable-manager">
                        <button className="btn-schedule">
                            <Calendar size={16} /> Quản lý thời khóa biểu
                        </button>
                    </Link>

                </div>
            </div>
            <DragDropContext onDragEnd={onDragEnd}>
                <div className="table-container" ref={topScrollRef} onScroll={() => syncScroll(topScrollRef, bottomScrollRef)}>
                    <div className="timetable-table dummy-scroll" />
                    <br />
                    <br />
                    <ScheduleTable
                        selectedClass={selectedClass}
                        getClassesByGrade={getClassesByGrade}
                        getUniqueClasses={getUniqueClasses}
                        getFilteredShifts={getFilteredShifts}
                        showTeacherName={showTeacherName}
                        setShowTeacherName={setShowTeacherName}
                        filteredSchedule={filteredSchedule}
                        scheduleData={scheduleData}
                        isViewMode={isViewMode}
                        setIsViewMode={setIsViewMode}
                        getSchedule={getSchedule}
                    />
                </div>
            </DragDropContext>
            <EditDialog
                showEditDialog={showEditDialog}
                setShowEditDialog={setShowEditDialog}
                selectedSchedule={selectedSchedule}
                selectedSubjectId={selectedSubjectId}
                setSelectedSubjectId={setSelectedSubjectId}
                selectedTeacherId={selectedTeacherId}
                setSelectedTeacherId={setSelectedTeacherId}
                subjects={subjects}
                teachers={teachers}
                handleScheduleUpdate={handleScheduleUpdate}
                handleDelete={handleDelete}
            />
            <AddDetailDialog
                showAddDialog={showAddDialog}
                setShowAddDialog={setShowAddDialog}
                classes={classes}
                days={DAYS_OF_WEEK}
                periods={getPeriods()}
                subjects={subjects}
                teachers={teachers}
                handleAddDetail={handleAddDetail}
            />
        </div>
    );
};

export default Schedule;