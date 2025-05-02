import React, { useState, useEffect } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useAcademicYears } from '@/services/common/queries';
import { getSemesterByYear } from '../../../services/schedule/api';
import toast from 'react-hot-toast';
import { Button } from "@/components/ui/button";  // Add this import
import 'react-toastify/dist/ReactToastify.css';
import './TimetableManagement.scss';

// Hàm định dạng ngày
const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
};

// Hàm gọi API lấy danh sách thời khóa biểu
const fetchTimetables = async (semesterId) => {
    const response = await fetch(`https://localhost:8386/api/Timetables/semester/${semesterId}`, {
        headers: { 'Content-Type': 'application/json' },
    });
    if (!response.ok) throw new Error('Không thể lấy danh sách thời khóa biểu');
    return response.json();
};

// Component FilterSelect (tái sử dụng từ Schedule.jsx)
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

import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog";
import { format } from "date-fns";

const TimetableManagement = () => {
    const queryClient = useQueryClient();
    // State
    const [selectedYear, setSelectedYear] = useState('');
    const [selectedSemester, setSelectedSemester] = useState('');
    const [semesters, setSemesters] = useState([]);

    // Lấy danh sách năm học
    const { data: academicYears, isLoading: academicYearsLoading } = useAcademicYears();

    // Lấy danh sách thời khóa biểu dựa trên semesterId
    const { data: timetables = [], isLoading: timetablesLoading, error } = useQuery({
        queryKey: ['timetables', selectedSemester],
        queryFn: () => fetchTimetables(selectedSemester),
        enabled: !!selectedSemester, // Chỉ gọi API khi selectedSemester có giá trị
    });

    // Effect để lấy học kỳ khi năm học thay đổi
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

    // Effect để hiển thị lỗi từ API timetables
    useEffect(() => {
        if (error) {
            toast.error('Có lỗi xảy ra khi lấy danh sách thời khóa biểu');
        }
    }, [error]);

    const [openUpdateModal, setOpenUpdateModal] = useState(false);
    const [selectedTimetable, setSelectedTimetable] = useState(null);

    const handleUpdate = async (timetableData) => {
        try {
            const response = await fetch('https://localhost:8386/api/Timetables/info', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(timetableData)
            });

            if (!response.ok) {
                throw new Error('Cập nhật thất bại');
            }

            toast.success('Cập nhật thành công');
            setOpenUpdateModal(false);
            // Refresh data
            queryClient.invalidateQueries(['timetables', selectedSemester]);
        } catch (error) {
            toast.error('Lỗi khi cập nhật: ' + error.message);
        }
    };

    return (
        <div className="timetable-management-container p-4">
            {/* Xóa ToastContainer vì không cần thiết khi dùng react-hot-toast */}
            <div className="flex justify-between items-center mb-6">
                <h1 className="text-2xl font-bold text-gray-800">Quản lý thời khóa biểu</h1>
                <Button variant="outline" className="bg-blue-500 text-white hover:bg-blue-600">
                    Thêm mới
                </Button>
            </div>

            <div className="bg-white rounded-lg shadow-md p-6">
                <div className="grid grid-cols-2 gap-6 mb-6">
                    <FilterSelect
                        label="Năm học"
                        value={selectedYear}
                        onChange={(e) => setSelectedYear(parseInt(e.target.value))}
                        options={academicYears?.map(year => ({
                            value: year.academicYearID,
                            label: `${year.yearName} -- ${year.academicYearID}`
                        })) || []}
                        disabled={academicYearsLoading}
                    />
                    <FilterSelect
                        label="Học kỳ"
                        value={selectedSemester}
                        onChange={(e) => setSelectedSemester(parseInt(e.target.value))}
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
                        <table className="w-full border-collapse">
                            <thead>
                                <tr className="bg-gray-50">
                                    <th className="px-4 py-3 text-left text-sm font-medium text-gray-600 border-b">TKB ID</th>
                                    <th className="px-4 py-3 text-left text-sm font-medium text-gray-600 border-b">Kỳ ID</th>
                                    <th className="px-4 py-3 text-left text-sm font-medium text-gray-600 border-b">Ngày bắt đầu</th>
                                    <th className="px-4 py-3 text-left text-sm font-medium text-gray-600 border-b">Ngày kết thúc </th>
                                    <th className="px-4 py-3 text-left text-sm font-medium text-gray-600 border-b">Trạng thái</th>
                                    <th className="px-4 py-3 text-left text-sm font-medium text-gray-600 border-b">Hoạt động</th>
                                </tr>
                            </thead>
                            <tbody>
                                {timetables.length > 0 ? (
                                    timetables.map(timetable => (
                                        <tr key={timetable.timetableId} className="hover:bg-gray-50">
                                            <td className="px-4 py-3 text-sm text-gray-700 border-b">{timetable.timetableId}</td>
                                            <td className="px-4 py-3 text-sm text-gray-700 border-b">
                                                {timetable.semesterId} - {
                                                    semesters.find(sem => sem.semesterID === timetable.semesterId)?.semesterName || ''
                                                }
                                            </td>
                                            <td className="px-4 py-3 text-sm text-gray-700 border-b">{formatDate(timetable.effectiveDate)}</td>
                                            <td className="px-4 py-3 text-sm text-gray-700 border-b">{formatDate(timetable.endDate)}</td>
                                            <td className="px-4 py-3 text-sm text-gray-700 border-b">
                                                <span className={`px-2 py-1 rounded-full text-xs ${timetable.status === 'Active' ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'
                                                    }`}>
                                                    {timetable.status}
                                                </span>
                                            </td>
                                            <td className="px-4 py-3 text-sm border-b">
                                                <div className="flex gap-2">
                                                    <Button
                                                        variant="outline"
                                                        size="sm"
                                                        className="text-blue-500 hover:text-blue-700"
                                                        onClick={() => {
                                                            setSelectedTimetable(timetable);
                                                            setOpenUpdateModal(true);
                                                        }}
                                                    >
                                                        Sửa
                                                    </Button>
                                                </div>
                                            </td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan="6" className="px-4 py-8 text-center text-gray-500">
                                            Không có thời khóa biểu nào cho học kỳ này.
                                        </td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
            <Dialog open={openUpdateModal} onOpenChange={setOpenUpdateModal}>
                <DialogContent>
                    <DialogHeader>
                        <DialogTitle>Cập nhật thời khóa biểu</DialogTitle>
                    </DialogHeader>
                    <div className="grid gap-4 py-4">
                        <div className="grid grid-cols-1 gap-4">
                            <div className="grid gap-2">
                                <label>TKB ID</label>
                                <input
                                    type="text"
                                    value={selectedTimetable?.timetableId || ''}
                                    disabled
                                    className="border rounded p-2 bg-gray-100"
                                />
                            </div>
                            <div className="grid gap-2">
                                <label>Kỳ ID</label>
                                <input
                                    type="text"
                                    value={`${selectedTimetable?.semesterId || ''} - ${semesters.find(sem => sem.semesterID === selectedTimetable?.semesterId)?.semesterName || ''
                                        }`}
                                    disabled
                                    className="border rounded p-2 bg-gray-100"
                                />
                            </div>
                            <div className="grid gap-2">
                                <label>Ngày bắt đầu</label>
                                <input
                                    type="date"
                                    value={selectedTimetable?.effectiveDate ? format(new Date(selectedTimetable.effectiveDate), 'yyyy-MM-dd') : ''}
                                    onChange={(e) => setSelectedTimetable({
                                        ...selectedTimetable,
                                        effectiveDate: e.target.value
                                    })}
                                    className="border rounded p-2"
                                />
                            </div>
                            <div className="grid gap-2">
                                <label>Ngày kết thúc</label>
                                <input
                                    type="date"
                                    value={selectedTimetable?.endDate ? format(new Date(selectedTimetable.endDate), 'yyyy-MM-dd') : ''}
                                    onChange={(e) => setSelectedTimetable({
                                        ...selectedTimetable,
                                        endDate: e.target.value
                                    })}
                                    className="border rounded p-2"
                                />
                            </div>
                            <div className="grid gap-2">
                                <label>Trạng thái</label>
                                <select
                                    value={selectedTimetable?.status || ''}
                                    onChange={(e) => setSelectedTimetable({
                                        ...selectedTimetable,
                                        status: e.target.value
                                    })}
                                    className="border rounded p-2"
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
        </div>
    );
};

export default TimetableManagement;