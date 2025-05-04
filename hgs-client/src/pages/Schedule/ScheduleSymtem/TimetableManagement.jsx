import React, { useState, useEffect, useCallback } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { useSubjects, useAcademicYears } from '@/services/common/queries';
import { getSemesterByYear } from '../../../services/schedule/api';
import toast from 'react-hot-toast';
import { Button } from "@/components/ui/button";
import './TimetableManagement.scss';
import { useUpdateTimetableInfo, useCreateTimetable } from '../../../services/schedule/mutation';
import { useTimetables, useGetClasses } from '../../../services/schedule/queries';
import { Link } from 'react-router-dom';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog";
import { format } from "date-fns";
import { ArrowLeft, Upload } from 'lucide-react';
import ImportSchedule from './ImportSchedule';

// Utility to format date for display
const formatDate = (dateString) => {
    return dateString ? new Date(dateString).toLocaleDateString('vi-VN', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
    }) : '';
};

// FilterSelect Component
const FilterSelect = ({ label, value, onChange, options, disabled }) => (
    <div className="filter-column">
        <label className="text-sm font-medium text-gray-700">{label}</label>
        <select
            value={value || ''}
            onChange={onChange}
            disabled={disabled}
            className="mt-1 block w-full border border-gray-300 rounded-md p-2 text-sm focus:ring-blue-500 focus:border-blue-500 disabled:bg-gray-100"
        >
            <option value="">Chọn {label.toLowerCase()}</option>
            {options.map(option => (
                <option key={option.value} value={option.value}>{option.label}</option>
            ))}
        </select>
    </div>
);

const TimetableManagement = () => {
    const queryClient = useQueryClient();
    const [selectedYear, setSelectedYear] = useState('');
    const [selectedSemester, setSelectedSemester] = useState('');
    const [semesters, setSemesters] = useState([]);
    const [openUpdateModal, setOpenUpdateModal] = useState(false);
    const [openCreateModal, setOpenCreateModal] = useState(false);
    const [openImportModal, setOpenImportModal] = useState(false);
    const [selectedTimetable, setSelectedTimetable] = useState(null);
    const [createYear, setCreateYear] = useState('');
    const [createSemesters, setCreateSemesters] = useState([]);

    const [newTimetable, setNewTimetable] = useState({
        semesterId: 0,
        effectiveDate: '',
        endDate: '',
        status: 'Không hoạt động',
        details: []
    });

    const { data: academicYears = [], isLoading: academicYearsLoading } = useAcademicYears();
    const { data: timetables = [], isLoading: timetablesLoading, error } = useTimetables(selectedSemester);
    const { data: subjects = [], isLoading: subjectsLoading } = useSubjects();
    const { data: classes = [], isLoading: classesLoading } = useGetClasses();
    const updateTimetableMutation = useUpdateTimetableInfo();
    const createTimetableMutation = useCreateTimetable();

    // Fetch semesters based on selected year
    const fetchSemesters = useCallback(async (year, setSemesterState) => {
        if (!year) {
            setSemesterState([]);
            return;
        }
        try {
            const semesterData = await getSemesterByYear(year);
            setSemesterState(semesterData || []);
        } catch (error) {
            console.error('Error fetching semesters:', error);
            toast.error('Không thể lấy danh sách học kỳ');
            setSemesterState([]);
        }
    }, []);

    // Handle year change for filter
    useEffect(() => {
        fetchSemesters(selectedYear, setSemesters);
        setSelectedSemester('');
    }, [selectedYear, fetchSemesters]);

    // Handle year change for create modal
    useEffect(() => {
        fetchSemesters(createYear, setCreateSemesters);
        setNewTimetable(prev => ({ ...prev, semesterId: 0 }));
    }, [createYear, fetchSemesters]);

    // Display error from timetables API
    useEffect(() => {
        if (error) {
            toast.error('Có lỗi xảy ra khi lấy danh sách thời khóa biểu');
        }
    }, [error]);

    // Initialize timetable details when classes and subjects are loaded
    useEffect(() => {
        if (classes.length > 0 && subjects.length > 0 && !subjectsLoading && !classesLoading) {
            const chaoCoSubject = subjects.find(subject => subject.subjectName === 'Chào cờ');
            const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
            let teacherId = 1; // default value

            if (token) {
                const tokenParts = token.split('.');
                if (tokenParts.length === 3) {
                    const payload = JSON.parse(atob(tokenParts[1]));
                    teacherId = Number(payload.teacherId);
                }
            }

            if (chaoCoSubject) {
                const newDetails = classes.map(cls => ({
                    classId: cls.classId,
                    subjectId: chaoCoSubject.subjectID,
                    teacherId: teacherId,
                    dayOfWeek: 'Thứ Hai',
                    periodId: 1
                }));
                setNewTimetable(prev => ({ ...prev, details: newDetails }));
            } else {
                toast.error('Không tìm thấy môn học Chào cờ');
            }
        }
    }, [classes, subjects, subjectsLoading, classesLoading]);

    // Handle timetable update
    const handleUpdate = async (timetableData) => {
        try {
            await updateTimetableMutation.mutateAsync(timetableData);
            queryClient.invalidateQueries(['timetables', selectedSemester]);
            setOpenUpdateModal(false);
            toast.success('Cập nhật thời khóa biểu thành công');
        } catch (error) {
            console.error('Error updating timetable:', error);
            toast.error('Cập nhật thất bại');
        }
    };

    // Handle timetable creation
    const handleCreate = async () => {
        // Validate required fields
        if (!newTimetable.semesterId || !newTimetable.effectiveDate || !newTimetable.endDate) {
            toast.error('Vui lòng điền đầy đủ thông tin bắt buộc');
            return;
        }
        if (newTimetable.details.length === 0) {
            toast.error('Danh sách chi tiết thời khóa biểu không được rỗng');
            return;
        }

        // Validate details
        const invalidDetails = newTimetable.details.some(detail =>
            !detail.classId ||
            !detail.subjectId ||
            !detail.teacherId ||
            !detail.dayOfWeek ||
            !detail.periodId
        );

        if (invalidDetails) {
            toast.error('Chi tiết thời khóa biểu có thông tin không hợp lệ');
            return;
        }

        // Validate dates
        const startDate = new Date(newTimetable.effectiveDate);
        const endDate = new Date(newTimetable.endDate);
        if (startDate >= endDate) {
            toast.error('Ngày bắt đầu phải nhỏ hơn ngày kết thúc');
            return;
        }

        // Ensure numeric IDs and format dates
        const payload = {
            ...newTimetable,
            semesterId: Number(newTimetable.semesterId),
            effectiveDate: format(new Date(newTimetable.effectiveDate), 'yyyy-MM-dd'),
            endDate: format(new Date(newTimetable.endDate), 'yyyy-MM-dd'),
            status: newTimetable.status || 'Không hoạt động',
            details: newTimetable.details.map(detail => ({
                classId: Number(detail.classId),
                subjectId: Number(detail.subjectId),
                teacherId: Number(detail.teacherId),
                dayOfWeek: detail.dayOfWeek,
                periodId: Number(detail.periodId)
            }))
        };

        try {
            await createTimetableMutation.mutateAsync(payload);
            queryClient.invalidateQueries(['timetables', selectedSemester]);
            setOpenCreateModal(false);
            setNewTimetable({
                semesterId: 0,
                effectiveDate: '',
                endDate: '',
                status: 'Không hoạt động',
                details: classes.map(cls => ({
                    classId: cls.classId,
                    subjectId: subjects.find(subject => subject.subjectName === 'Chào cờ')?.subjectID || 0,
                    teacherId: 64,
                    dayOfWeek: 'Thứ Hai',
                    periodId: 1
                }))
            });
            setCreateYear('');
            toast.success('Tạo thời khóa biểu thành công');
        } catch (error) {
            console.error('Error creating timetable:', error);
            const errorMessage = error.response?.data || 'Tạo thời khóa biểu thất bại';
            toast.error(errorMessage);

            // Log detailed error information
            if (error.response) {
                console.error('Error response:', {
                    data: error.response.data,
                    status: error.response.status,
                    headers: error.response.headers
                });
            }
        }
    };

    return (
        <div className="timetable-management-container p-6">
            <div className="flex justify-between items-center mb-8">
                <div className="flex items-center gap-4">
                    <Button
                        variant="outline"
                        asChild
                        className="border-gray-300 text-gray-700 hover:bg-gray-100"
                    >
                        <Link to="/system/schedule" className="flex items-center gap-2">
                            <ArrowLeft size={16} />
                            Trở về
                        </Link>
                    </Button>
                    <h1 className="text-3xl font-bold text-gray-800">Quản lý thời khóa biểu</h1>
                </div>
                <div className="flex gap-2">
                    <Button
                        className="bg-blue-500 text-white hover:bg-blue-600 flex items-center gap-2"
                        onClick={() => setOpenImportModal(true)}
                    >
                        <Upload size={16} />
                        Import
                    </Button>
                    <Button
                        className="bg-blue-500 text-white hover:bg-blue-600"
                        onClick={() => setOpenCreateModal(true)}
                    >
                        Thêm mới
                    </Button>
                </div>
            </div>

            <div className="bg-white rounded-lg shadow-md p-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
                    <FilterSelect
                        label="Năm học"
                        value={selectedYear}
                        onChange={(e) => setSelectedYear(Number(e.target.value))}
                        options={academicYears.map(year => ({
                            value: year.academicYearID,
                            label: `${year.yearName} -- ${year.academicYearID}`
                        }))}
                        disabled={academicYearsLoading}
                    />
                    <FilterSelect
                        label="Học kỳ"
                        value={selectedSemester}
                        onChange={(e) => setSelectedSemester(Number(e.target.value))}
                        options={semesters.map(semester => ({
                            value: semester.semesterID,
                            label: `${semester.semesterName} -- ${semester.semesterID}`
                        }))}
                        disabled={!selectedYear || !semesters.length}
                    />
                </div>

                {timetablesLoading && (
                    <div className="flex justify-center items-center h-32">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
                    </div>
                )}

                {!timetablesLoading && selectedSemester && (
                    <div className="overflow-x-auto">
                        <table className="w-full border-collapse text-sm">
                            <thead>
                                <tr className="bg-gray-100 text-gray-700">
                                    <th className="px-4 py-3 text-left font-semibold border-b">STT</th>
                                    <th className="px-4 py-3 text-left font-semibold border-b">TKB ID</th>
                                    <th className="px-4 py-3 text-left font-semibold border-b">Học kỳ</th>
                                    <th className="px-4 py-3 text-left font-semibold border-b">Ngày bắt đầu</th>
                                    <th className="px-4 py-3 text-left font-semibold border-b">Ngày kết thúc</th>
                                    <th className="px-4 py-3 text-left font-semibold border-b">Trạng thái</th>
                                    <th className="px-4 py-3 text-left font-semibold border-b">Hành động</th>
                                </tr>
                            </thead>
                            <tbody>
                                {timetables.length > 0 ? (
                                    timetables.map((timetable, index) => (
                                        <tr key={timetable.timetableId} className="hover:bg-gray-50 transition-colors">
                                            <td className="px-4 py-3 text-gray-700 border-b">{index + 1}</td>
                                            <td className="px-4 py-3 text-gray-700 border-b">{timetable.timetableId}</td>
                                            <td className="px-4 py-3 text-gray-700 border-b">
                                                {timetable.semesterId} - {
                                                    semesters.find(sem => sem.semesterID === timetable.semesterId)?.semesterName || ''
                                                }
                                            </td>
                                            <td className="px-4 py-3 text-gray-700 border-b">{formatDate(timetable.effectiveDate)}</td>
                                            <td className="px-4 py-3 text-gray-700 border-b">{formatDate(timetable.endDate)}</td>
                                            <td className="px-4 py-3 text-gray-700 border-b">
                                                <span className={`px-2 py-1 rounded-full text-xs ${timetable.status === 'Hoạt động'
                                                    ? 'bg-green-100 text-green-800'
                                                    : 'bg-gray-100 text-gray-800'
                                                    }`}>
                                                    {timetable.status}
                                                </span>
                                            </td>
                                            <td className="px-4 py-3 border-b">
                                                <Button
                                                    variant="outline"
                                                    size="sm"
                                                    className="text-blue-500 hover:text-blue-700 border-blue-500"
                                                    onClick={() => {
                                                        setSelectedTimetable(timetable);
                                                        setOpenUpdateModal(true);
                                                    }}
                                                >
                                                    Sửa
                                                </Button>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan="7" className="px-4 py-8 text-center text-gray-500">
                                            Không có thời khóa biểu nào cho học kỳ này.
                                        </td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>

            {/* Update Modal */}
            <Dialog open={openUpdateModal} onOpenChange={setOpenUpdateModal}>
                <DialogContent className="sm:max-w-md">
                    <DialogHeader>
                        <DialogTitle className="text-xl font-semibold">Cập nhật thời khóa biểu</DialogTitle>
                    </DialogHeader>
                    <div className="grid gap-4 py-4">
                        <div className="grid grid-cols-1 gap-4">
                            <div className="grid gap-2">
                                <label className="text-sm font-medium text-gray-700">TKB ID</label>
                                <input
                                    type="text"
                                    value={selectedTimetable?.timetableId || ''}
                                    disabled
                                    className="border rounded p-2 bg-gray-100 text-sm"
                                />
                            </div>
                            <div className="grid gap-2">
                                <label className="text-sm font-medium text-gray-700">Học kỳ</label>
                                <input
                                    type="text"
                                    value={`${selectedTimetable?.semesterId || ''} - ${semesters.find(sem => sem.semesterID === selectedTimetable?.semesterId)?.semesterName || ''}`}
                                    disabled
                                    className="border rounded p-2 bg-gray-100 text-sm"
                                />
                            </div>
                            <div className="grid gap-2">
                                <label className="text-sm font-medium text-gray-700">Ngày bắt đầu</label>
                                <input
                                    type="date"
                                    value={selectedTimetable?.effectiveDate ? format(new Date(selectedTimetable.effectiveDate), 'yyyy-MM-dd') : ''}
                                    onChange={(e) => setSelectedTimetable({
                                        ...selectedTimetable,
                                        effectiveDate: e.target.value
                                    })}
                                    className="border rounded p-2 text-sm"
                                />
                            </div>
                            <div className="grid gap-2">
                                <label className="text-sm font-medium text-gray-700">Ngày kết thúc</label>
                                <input
                                    type="date"
                                    value={selectedTimetable?.endDate ? format(new Date(selectedTimetable.endDate), 'yyyy-MM-dd') : ''}
                                    onChange={(e) => setSelectedTimetable({
                                        ...selectedTimetable,
                                        endDate: e.target.value
                                    })}
                                    className="border rounded p-2 text-sm"
                                />
                            </div>
                            <div className="grid gap-2">
                                <label className="text-sm font-medium text-gray-700">Trạng thái</label>
                                <select
                                    value={selectedTimetable?.status || ''}
                                    onChange={(e) => setSelectedTimetable({
                                        ...selectedTimetable,
                                        status: e.target.value
                                    })}
                                    className="border rounded p-2 text-sm"
                                >
                                    <option value="Hoạt động">Hoạt động</option>
                                    <option value="Không hoạt động">Không hoạt động</option>
                                </select>
                            </div>
                        </div>
                        <div className="flex justify-end gap-2">
                            <Button
                                variant="outline"
                                onClick={() => setOpenUpdateModal(false)}
                            >
                                Hủy
                            </Button>
                            <Button
                                onClick={() => handleUpdate(selectedTimetable)}
                                className="bg-blue-500 text-white hover:bg-blue-600"
                            >
                                Cập nhật
                            </Button>
                        </div>
                    </div>
                </DialogContent>
            </Dialog>

            {/* Create Modal */}
            <Dialog open={openCreateModal} onOpenChange={setOpenCreateModal}>
                <DialogContent className="sm:max-w-md">
                    <DialogHeader>
                        <DialogTitle className="text-xl font-semibold">Tạo thời khóa biểu mới</DialogTitle>
                    </DialogHeader>
                    <div className="grid gap-4 py-4">
                        <div className="grid grid-cols-1 gap-4">
                            <div className="grid gap-2">
                                <label className="text-sm font-medium text-gray-700">Năm học</label>
                                <select
                                    value={createYear}
                                    onChange={(e) => setCreateYear(Number(e.target.value))}
                                    className="border rounded p-2 text-sm"
                                >
                                    <option value="">Chọn năm học</option>
                                    {academicYears.map(year => (
                                        <option key={year.academicYearID} value={year.academicYearID}>
                                            {year.yearName} -- {year.academicYearID}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div className="grid gap-2">
                                <label className="text-sm font-medium text-gray-700">Học kỳ</label>
                                <select
                                    value={newTimetable.semesterId}
                                    onChange={(e) => setNewTimetable({ ...newTimetable, semesterId: Number(e.target.value) })}
                                    className="border rounded p-2 text-sm"
                                    disabled={!createYear || !createSemesters.length}
                                >
                                    <option value={0}>Chọn học kỳ</option>
                                    {createSemesters.map(semester => (
                                        <option key={semester.semesterID} value={semester.semesterID}>
                                            {semester.semesterName} -- {semester.semesterID}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div className="grid gap-2">
                                <label className="text-sm font-medium text-gray-700">Ngày bắt đầu</label>
                                <input
                                    type="date"
                                    value={newTimetable.effectiveDate}
                                    onChange={(e) => setNewTimetable({ ...newTimetable, effectiveDate: e.target.value })}
                                    className="border rounded p-2 text-sm"
                                />
                            </div>
                            <div className="grid gap-2">
                                <label className="text-sm font-medium text-gray-700">Ngày kết thúc</label>
                                <input
                                    type="date"
                                    value={newTimetable.endDate}
                                    onChange={(e) => setNewTimetable({ ...newTimetable, endDate: e.target.value })}
                                    className="border rounded p-2 text-sm"
                                />
                            </div>
                            <div className="grid gap-2">
                                <label className="text-sm font-medium text-gray-700">Trạng thái</label>
                                <select
                                    value={newTimetable.status}
                                    onChange={(e) => setNewTimetable({ ...newTimetable, status: e.target.value })}
                                    className="border rounded p-2 text-sm"
                                >
                                    <option value="Hoạt động">Hoạt động</option>
                                    <option value="Không hoạt động">Không hoạt động</option>
                                </select>
                            </div>
                        </div>
                        <div className="flex justify-end gap-2">
                            <Button
                                variant="outline"
                                onClick={() => setOpenCreateModal(false)}
                            >
                                Hủy
                            </Button>
                            <Button
                                onClick={handleCreate}
                                className="bg-blue-500 text-white hover:bg-blue-600"
                            >
                                Tạo
                            </Button>
                        </div>
                    </div>
                </DialogContent>
            </Dialog>

            {/* Import Modal */}
            <Dialog open={openImportModal} onOpenChange={setOpenImportModal}>
                <DialogContent className="sm:max-w-md">
                    <ImportSchedule onClose={() => setOpenImportModal(false)} />
                </DialogContent>
            </Dialog>
        </div>
    );
};

export default TimetableManagement;