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
                <div>Đang tải...</div>
            ) : classSummary && classSummary.students && classSummary.students.length > 0 ? (
                <div>
                    <div className="mb- Schmidt4">
                        <h2 className="text-lg font-semibold">Tổng kết điểm lớp: {classSummary.className}</h2>
                        <p>Năm học: {academicYearName}</p>
                        <p>Học kỳ: {semesterName}</p>
                    </div>

                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead className="w-[50px]">STT</TableHead>
                                <TableHead className="w-[200px]">Họ và tên</TableHead>
                                {getSubjects().map(subject => (
                                    <TableHead key={subject.subjectId} className="text-center">
                                        {subject.subjectName}
                                    </TableHead>
                                ))}
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {classSummary.students.map((student, index) => (
                                <TableRow key={student.studentId}>
                                    <TableCell>{index + 1}</TableCell>
                                    <TableCell>{student.fullName}</TableCell>
                                    {getSubjects().map(subject => {
                                        const result = student.subjectResults.find(
                                            res => res.subjectId === subject.subjectId
                                        );
                                        return (
                                            <TableCell key={subject.subjectId} className="text-center">
                                                {result?.finalScore || '-'}
                                            </TableCell>
                                        );
                                    })}
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                    <div className="mt-4">
                        <button
                            onClick={handleExportClassGrades}
                            className="px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600 transition-colors"
                            disabled={!classId || !semesterId || loading || !classSummary?.students.length}
                        >
                            In điểm cả lớp
                        </button>
                    </div>
                </div>
            ) : (
                <div>Chưa có dữ liệu tổng kết</div>
            )}
        </div>
    );
};

export default GradeSummary;