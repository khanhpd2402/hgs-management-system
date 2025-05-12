import React, { useState, useEffect } from 'react';
import axios from 'axios';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from "@/components/ui/table";
import ExportGradeToExcel from './ExportGradeToExcel.jsx';

// Replace with your actual token retrieval mechanism
const getAuthToken = () => {
    const token = localStorage.getItem('token') || 'your-auth-token-here';
    return token.replace(/"/g, "");
};

const GradeSummary = () => {
    const [academicYearId, setAcademicYearId] = useState('');
    const [semesterId, setSemesterId] = useState('');
    const [classId, setClassId] = useState('');
    const [academicYears, setAcademicYears] = useState([]);
    const [semesters, setSemesters] = useState([]);
    const [classes, setClasses] = useState([]);
    const [classSummary, setClassSummary] = useState(null);
    const [loading, setLoading] = useState(false);

    // Fetch academic years
    useEffect(() => {
        const fetchAcademicYears = async () => {
            try {
                const response = await axios.get('https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/AcademicYear', {
                    headers: {
                        Authorization: `Bearer ${getAuthToken()}`
                    }
                });
                setAcademicYears(response.data);
                if (response.data.length > 0) {
                    setAcademicYearId(response.data[0].academicYearID.toString());
                }
            } catch (error) {
                console.error('Lỗi khi tải danh sách năm học:', error);
            }
        };
        fetchAcademicYears();
    }, []);

    // Fetch semesters when academic year changes
    useEffect(() => {
        const fetchSemesters = async () => {
            if (academicYearId) {
                try {
                    const response = await axios.get(`https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Semester/by-academic-year/${academicYearId}`, {
                        headers: {
                            Authorization: `Bearer ${getAuthToken()}`
                        }
                    });
                    setSemesters(response.data);
                    if (response.data.length > 0) {
                        setSemesterId(response.data[0].semesterID.toString());
                    } else {
                        setSemesterId('');
                    }
                } catch (error) {
                    console.error('Lỗi khi tải danh sách học kỳ:', error);
                }
            }
        };
        fetchSemesters();
    }, [academicYearId]);

    // Fetch classes
    useEffect(() => {
        const fetchClasses = async () => {
            try {
                const response = await axios.get('https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Classes', {
                    headers: {
                        Authorization: `Bearer ${getAuthToken()}`
                    }
                });
                const activeClasses = response.data.filter(c => c.status === 'Hoạt động');
                setClasses(activeClasses);
                if (activeClasses.length > 0) {
                    setClassId(activeClasses[0].classId.toString());
                } else {
                    setClassId('');
                }
            } catch (error) {
                console.error('Lỗi khi tải danh sách lớp học:', error);
            }
        };
        fetchClasses();
    }, []);

    // Fetch class grade summary when class and semester change
    useEffect(() => {
        const fetchClassSummary = async () => {
            console.log("class", classId)
            console.log("semester", semesterId)
            if (classId && semesterId) {
                setLoading(true);
                try {
                    const response = await axios.get(`https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Grades/class-summary/${classId}/${semesterId}`, {
                        headers: {
                            Authorization: `Bearer ${getAuthToken()}`
                        }
                    });
                    setClassSummary(response.data);
                } catch (error) {
                    console.error('Lỗi khi tải tổng kết điểm lớp:', error);
                    setClassSummary(null);
                } finally {
                    setLoading(false);
                }
            } else {
                setClassSummary(null);
            }
        };
        fetchClassSummary();
    }, [classId, semesterId]);

    // Get unique subjects from class summary
    const getSubjects = () => {
        if (!classSummary || !classSummary.students || classSummary.students.length === 0) return [];
        const subjects = classSummary.students[0].subjectResults.map(subject => ({
            subjectId: subject.subjectId,
            subjectName: subject.subjectName
        }));
        return subjects.filter(subject => subject.subjectName !== 'Chào cờ');
    };

    // Derive semesterName and academicYearName
    const semesterName = classSummary?.semesterName || '';
    const academicYearName = classSummary?.academicYear || '';

    // Handle export for all students in the class
    const handleExportClassGrades = () => {
        if (classSummary && classSummary.students.length > 0) {
            printClassGradeReport(classSummary.students);
        } else {
            console.error('Không có dữ liệu tổng kết điểm cho lớp này.');
        }
    };

    // Print grade report for all students
    const printClassGradeReport = (students) => {
        const printContent = document.createElement('div');
        printContent.className = 'print-content';

        const className = classes.find(c => c.classId.toString() === classId)?.className || '';

        // Generate HTML for each student's grade report
        const reports = students.map((student, index) => `
            <div style="font-family: Times New Roman, serif; padding: 15px; width: 190mm; margin: auto; page-break-after: always;">
                <div style="text-align: center; margin-bottom: 15px;">
                    <div style="display: flex; justify-content: space-between; font-size: 13px;">
                        <div>
                            <p style="margin: 0;">SỞ GIÁO DỤC & ĐÀO TẠO Nam Định</p>
                            <p style="margin: 0;">TRƯỜNG THCS Hải Giang</p>
                            <p style="margin: 0;">---*---</p>
                        </div>
                        <div>
                            <p style="margin: 0;">CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</p>
                            <p style="margin: 0;">Độc lập - Tự do - Hạnh phúc</p>
                            <p style="margin: 0;">---o0o---</p>
                        </div>
                    </div>
                    <h2 style="margin: 15px 0; font-size: 16px;">PHIẾU ĐIỂM HỌC KỲ ${semesterName === "Học kỳ 1" ? "I" : "II"} - NĂM HỌC ${academicYearName}</h2>
                </div>

                <div style="margin-bottom: 15px; font-size: 13px;">
                    <p style="margin: 0;">Họ và tên học sinh: ${student.fullName || ''}    Lớp: ${className}</p>
                </div>

                <table style="width: 100%; border-collapse: collapse; margin-bottom: 15px; font-size: 13px;">
                    <thead>
                        <tr>
                            <th style="border: 1px solid black; padding: 6px;">STT</th>
                            <th style="border: 1px solid black; padding: 6px;">MÔN HỌC</th>
                            <th style="border: 1px solid black; padding: 6px;">TB HK1</th>
                            <th style="border: 1px solid black; padding: 6px;">GHI CHÚ</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${student.subjectResults.filter(subject => subject.subjectName !== 'Chào cờ').map((subject, index) => `
                            <tr>
                                <td style="border: 1px solid black; padding: 6px; text-align: center;">${index + 1}</td>
                                <td style="border: 1px solid black; padding: 6px;">${subject.subjectName}</td>
                                <td style="border: 1px solid black; padding: 6px; text-align: center;">${subject.finalScore || '-'}</td>
                                <td style="border: 1px solid black; padding: 6px;"></td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>

                <div style="text-align: right; margin-top: 20px; font-size: 13px;">
                    <p style="margin: 0;">Nam Định, ngày ... tháng ... năm ...</p>
                    <p style="margin: 40px 0 0 0;">Giáo viên quản nhiệm</p>
                    <p style="margin: 40px 0 0 0;">...</p>
                </div>
            </div>
        `).join('');

        printContent.innerHTML = reports;

        // Add print styles
        const style = document.createElement('style');
        style.textContent = `
            @media print {
                body * {
                    visibility: hidden;
                }
                .print-content, .print-content * {
                    visibility: visible;
                }
                .print-content {
                    position: absolute;
                    left: 0;
                    top: 0;
                }
                @page {
                    size: A4;
                    margin: 0;
                }
            }
        `;
        printContent.appendChild(style);

        // Append to body
        document.body.appendChild(printContent);

        // Print
        window.print();

        // Remove temporary element after printing
        document.body.removeChild(printContent);
    };

    return (
        <div className="container mx-auto p-4">
            <div className="flex gap-4 mb-6">
                <Select value={academicYearId} onValueChange={setAcademicYearId}>
                    <SelectTrigger className="w-[180px]">
                        <SelectValue placeholder="Chọn năm học" />
                    </SelectTrigger>
                    <SelectContent>
                        {academicYears.map(year => (
                            <SelectItem key={year.academicYearID} value={year.academicYearID.toString()}>
                                {year.yearName}
                            </SelectItem>
                        ))}
                    </SelectContent>
                </Select>

                <Select value={semesterId} onValueChange={setSemesterId} disabled={!academicYearId}>
                    <SelectTrigger className="w-[180px]">
                        <SelectValue placeholder="Chọn học kỳ" />
                    </SelectTrigger>
                    <SelectContent>
                        {semesters.map(semester => (
                            <SelectItem key={semester.semesterID} value={semester.semesterID.toString()}>
                                {semester.semesterName}
                            </SelectItem>
                        ))}
                    </SelectContent>
                </Select>

                <Select value={classId} onValueChange={setClassId} disabled={!semesterId}>
                    <SelectTrigger className="w-[180px]">
                        <SelectValue placeholder="Chọn lớp" />
                    </SelectTrigger>
                    <SelectContent>
                        {classes.map(cls => (
                            <SelectItem key={cls.classId} value={cls.classId.toString()}>
                                {cls.className}
                            </SelectItem>
                        ))}
                    </SelectContent>
                </Select>
            </div>

            {loading ? (
                <div className="flex items-center justify-center min-h-[200px]">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
                </div>
            ) : classSummary && classSummary.students && classSummary.students.length > 0 ? (
                <div className="bg-white rounded-lg shadow-md p-6">
                    <div className="mb-6">
                        <h2 className="text-2xl font-bold text-gray-800 mb-2">Tổng kết điểm lớp: {classSummary.className}</h2>
                        <div className="text-gray-600">
                            <p className="mb-1">Năm học: {academicYearName}</p>
                            <p>Học kỳ: {semesterName}</p>
                        </div>
                    </div>

                    <div className="overflow-x-auto">
                        <Table className="w-full border-collapse min-w-[800px]">
                            <TableHeader>
                                <TableRow className="bg-gray-50 hover:bg-gray-50">
                                    <TableHead className="w-[50px] py-4 px-4 text-left font-semibold text-gray-600 border-b border-gray-200 whitespace-nowrap">
                                        STT
                                    </TableHead>
                                    <TableHead className="w-[200px] py-4 px-4 text-left font-semibold text-gray-600 border-b border-gray-200 whitespace-nowrap">
                                        Họ và tên
                                    </TableHead>
                                    {getSubjects().map(subject => (
                                        <TableHead
                                            key={subject.subjectId}
                                            className="py-4 px-4 text-center font-semibold text-gray-600 border-b border-gray-200 whitespace-nowrap min-w-[100px]"
                                        >
                                            {subject.subjectName}
                                        </TableHead>
                                    ))}
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {classSummary.students.map((student, index) => (
                                    <TableRow
                                        key={student.studentId}
                                        className="hover:bg-gray-50 transition-colors"
                                    >
                                        <TableCell className="py-4 px-4 text-gray-500 border-b border-gray-200 whitespace-nowrap">
                                            {index + 1}
                                        </TableCell>
                                        <TableCell className="py-4 px-4 font-medium text-gray-900 border-b border-gray-200 whitespace-nowrap">
                                            {student.fullName}
                                        </TableCell>
                                        {getSubjects().map(subject => {
                                            const result = student.subjectResults.find(
                                                res => res.subjectId === subject.subjectId
                                            );
                                            const score = result?.finalScore;
                                            const isGradeTypeEvaluation = result?.gradeType === 'Đánh giá';
                                            
                                            // Xử lý điểm số có dấu phẩy
                                            const numericScore = typeof score === 'string' && !isGradeTypeEvaluation
                                                ? parseFloat(score.replace(',', '.'))
                                                : null;
                                            
                                            return (
                                                <TableCell
                                                    key={subject.subjectId}
                                                    className={`py-4 px-4 text-center border-b border-gray-200 whitespace-nowrap ${
                                                            isGradeTypeEvaluation
                                                                ? score === 'Đạt' 
                                                                    ? 'text-green-600 font-medium'
                                                                    : score === 'Không đạt'
                                                                        ? 'text-red-600 font-medium'
                                                                        : 'text-gray-400'
                                                                : numericScore >= 8
                                                                    ? 'text-green-600 font-medium'
                                                                    : numericScore >= 6.5
                                                                        ? 'text-blue-600 font-medium'
                                                                        : numericScore >= 5
                                                                            ? 'text-yellow-600 font-medium'
                                                                            : numericScore
                                                                                ? 'text-red-600 font-medium'
                                                                                : 'text-gray-400'
                                                        }`}
                                                >
                                                    {isGradeTypeEvaluation 
                                                        ? score || '-' 
                                                        : (score ? score.toString().replace('.', ',') : '-')}
                                                </TableCell>
                                            );
                                        })}
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </div>

                    <div className="mt-6 flex gap-4">
                        <button
                            onClick={handleExportClassGrades}
                            className="px-6 py-2.5 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors duration-200 flex items-center gap-2 disabled:opacity-50 disabled:cursor-not-allowed"
                            disabled={!classId || !semesterId || loading || !classSummary?.students.length}
                        >
                            <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                                <path fillRule="evenodd" d="M6 2a2 2 0 00-2 2v12a2 2 0 002 2h8a2 2 0 002-2V7.414A2 2 0 0015.414 6L12 2.586A2 2 0 0010.586 2H6zm5 6a1 1 0 10-2 0v3.586L7.707 10.293a1 1 0 10-1.414 1.414l3 3a1 1 0 001.414 0l3-3a1 1 0 00-1.414-1.414L11 11.586V8z" clipRule="evenodd" />
                            </svg>
                            In điểm cả lớp
                        </button>
                    </div>
                </div>
            ) : (
                <div className="text-center py-12 bg-gray-50 rounded-lg">
                    <svg xmlns="http://www.w3.org/2000/svg" className="mx-auto h-12 w-12 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
                    </svg>
                    <h3 className="mt-2 text-sm font-medium text-gray-900">Chưa có dữ liệu tổng kết</h3>
                    <p className="mt-1 text-sm text-gray-500">Vui lòng chọn lớp và học kỳ để xem bảng điểm</p>
                </div>
            )}
        </div>
    );
};

export default GradeSummary;