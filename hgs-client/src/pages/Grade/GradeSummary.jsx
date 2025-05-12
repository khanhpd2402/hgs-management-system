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
    // Lấy token và loại bỏ hết dấu nháy đơn nếu có
    const token = localStorage.getItem('token') || 'your-auth-token-here';
    return token.replace(/"/g, "");
};

const GradeSummary = () => {
    const [academicYearId, setAcademicYearId] = useState('');
    const [semesterId, setSemesterId] = useState('');
    const [classId, setClassId] = useState('');
    const [studentId, setStudentId] = useState('');
    const [academicYears, setAcademicYears] = useState([]);
    const [semesters, setSemesters] = useState([]);
    const [classes, setClasses] = useState([]);
    const [students, setStudents] = useState([]);
    const [gradeSummary, setGradeSummary] = useState(null);
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

    // Fetch students when class and academic year change
    useEffect(() => {
        const fetchStudents = async () => {
            if (classId && academicYearId) {
                setLoading(true);
                try {
                    const response = await axios.get('https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/StudentClass', {
                        headers: {
                            Authorization: `Bearer ${getAuthToken()}`,
                            Accept: 'application/json;odata.metadata=minimal;odata.streaming=true',
                        }
                    });
                    // Filter students by classId and academicYearId
                    const filteredStudents = response.data.filter(
                        student =>
                            student.classId === parseInt(classId) &&
                            student.academicYearId === parseInt(academicYearId)
                    );
                    console.log("Filtered students:", filteredStudents);
                    setStudents(filteredStudents);
                } catch (error) {
                    console.error('Lỗi khi tải danh sách học sinh:', error.response?.data || error.message);
                    setStudents([]);
                } finally {
                    setLoading(false);
                }
            } else {
                setStudents([]);
            }
        };
        fetchStudents();
    }, [classId, academicYearId]);

    // Fetch grade summary when student and semester change
    useEffect(() => {
        const fetchGradeSummary = async () => {
            if (studentId && semesterId) {
                setLoading(true);
                try {
                    const response = await axios.get(`https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Grades/student/${studentId}/grade-summary`, {
                        params: { semesterId },
                        headers: {
                            Authorization: `Bearer ${getAuthToken()}`
                        }
                    });
                    setGradeSummary(response.data);
                } catch (error) {
                    console.error('Lỗi khi tải tổng kết điểm:', error);
                } finally {
                    setLoading(false);
                }
            } else {
                setGradeSummary(null);
            }
        };
        fetchGradeSummary();
    }, [studentId, semesterId]);

    // Fetch grade summaries for all students in the class
    const fetchClassGradeSummaries = async () => {
        if (!classId || !semesterId || !students.length) return [];

        setLoading(true);
        const gradeSummaries = [];
        try {
            for (const student of students) {
                try {
                    const response = await axios.get(`https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Grades/student/${student.studentId}/grade-summary`, {
                        params: { semesterId },
                        headers: {
                            Authorization: `Bearer ${getAuthToken()}`
                        }
                    });
                    gradeSummaries.push({
                        ...response.data,
                        studentId: student.studentId,
                        studentName: student.studentName
                    });
                } catch (error) {
                    console.error(`Lỗi khi tải tổng kết điểm cho học sinh ${student.studentName}:`, error);
                }
            }
        } finally {
            setLoading(false);
        }
        return gradeSummaries;
    };

    // Handle export for all students in the class
    const handleExportClassGrades = async () => {
        const gradeSummaries = await fetchClassGradeSummaries();
        if (gradeSummaries.length > 0) {
            // Trigger print for all students
            printClassGradeReport(gradeSummaries);
        } else {
            console.error('Không có dữ liệu tổng kết điểm cho lớp này.');
        }
    };

    // Derive semesterName and academicYearName
    const semesterName = semesters.find(s => s.semesterID.toString() === semesterId)?.semesterName || '';
    const academicYearName = academicYears.find(y => y.academicYearID.toString() === academicYearId)?.yearName || '';

    // Print grade report for all students
    const printClassGradeReport = (gradeSummaries) => {
        const printContent = document.createElement('div');
        printContent.className = 'print-content';

        const className = classes.find(c => c.classId.toString() === classId)?.className || '';

        // Generate HTML for each student's grade report
        const reports = gradeSummaries.map((summary, index) => `
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
                    <p style="margin: 0;">Họ và tên học sinh: ${summary.studentName || ''}    Lớp: ${className}</p>
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
                        ${summary.gradeSummaryEachSubjectNameDtos.map((subject, index) => `
                            <tr>
                                <td style="border: 1px solid black; padding: 6px; text-align: center;">${index + 1}</td>
                                <td style="border: 1px solid black; padding: 6px;">${subject.subjectName}</td>
                                <td style="border: 1px solid black; padding: 6px; text-align: center;">${subject.semester1Average?.toFixed(1) || '-'}</td>
                                <td style="border: 1px solid black; padding: 6px;"></td>
                            </tr>
                        `).join('')}
                        <tr>
                            <td colspan="2" style="border: 1px solid black; padding: 6px; text-align: center;">TB các môn</td>
                            <td style="border: 1px solid black; padding: 6px; text-align: center;">${summary.totalSemester1Average?.toFixed(1) || '-'}</td>
                            <td style="border: 1px solid black; padding: 6px;"></td>
                        </tr>
                    </tbody>
                </table>

                <div style="margin-bottom: 15px;">
                    <table style="width: 50%; border-collapse: collapse; font-size: 13px;">
                        <tr>
                            <td style="padding: 3px;">Xếp loại</td>
                            <td style="padding: 3px;">Học kỳ 1</td>
                            <td style="padding: 3px;">Ghi chú</td>
                        </tr>
                        <tr>
                            <td style="padding: 3px;">Học lực</td>
                            <td style="padding: 3px;">${summary.academicAbility || '-'}</td>
                            <td style="padding: 3px;"></td>
                        </tr>
                        <tr>
                            <td style="padding: 3px;">Hạnh kiểm</td>
                            <td style="padding: 3px;">${summary.conduct || '-'}</td>
                            <td style="padding: 3px;"></td>
                        </tr>
                        <tr>
                            <td style="padding: 3px;">Danh hiệu thi đua</td>
                            <td style="padding: 3px;">${summary.competitionTitle || '-'}</td>
                            <td style="padding: 3px;"></td>
                        </tr>
                    </table>
                </div>

                <div style="margin-bottom: 15px; font-size: 13px;">
                    <p style="margin: 0;">Nhận xét: ${summary.comment || ''}</p>
                </div>

                <div style="text-align: right; margin-top: 20px; font-size: 13px;">
                    <p style="margin: 0;">Nam Định, ngày ... tháng ... năm ...</p>
                    <p style="margin: 40px 0 0 0;">Giáo viên quản nhiệm</p>
                    <p style="margin: 40px 0 0 0;">${summary.teacherName || '...'}</p>
                </div>
            </div>
        `).join('');

        printContent.innerHTML = reports;

        // Thêm style cho in ấn
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

        // Thêm vào body
        document.body.appendChild(printContent);

        // In
        window.print();

        // Xóa phần tử tạm sau khi in
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

                <Select value={studentId} onValueChange={setStudentId} disabled={!classId || loading}>
                    <SelectTrigger className="w-[180px]">
                        <SelectValue placeholder="Chọn học sinh" />
                    </SelectTrigger>
                    <SelectContent>
                        {loading ? (
                            <SelectItem value="loading" disabled>Đang tải...</SelectItem>
                        ) : students.length > 0 ? (
                            students.map(student => (
                                <SelectItem key={student.studentId} value={student.studentId.toString()}>
                                    {student.studentName}
                                </SelectItem>
                            ))
                        ) : (
                            <SelectItem value="no-students" disabled>Không có học sinh</SelectItem>
                        )}
                    </SelectContent>
                </Select>
            </div>

            {loading ? (
                <div>Đang tải...</div>
            ) : gradeSummary ? (
                <div>
                    <div className="mb-4">
                        <h2 className="text-lg font-semibold">
                            Tổng kết: {gradeSummary.gradeSummaryEachSubjectNameDtos[0]?.studentName}
                        </h2>
                        <p>Năm học: {academicYearName}</p>
                        <p>Học kỳ: {semesterName}</p>
                        <p>Học kỳ 1: {gradeSummary.totalSemester1Average?.toFixed(2) || '-'}</p>
                        <p>Học kỳ 2: {gradeSummary.totalSemester2Average?.toFixed(2) || '-'}</p>
                        <p>Cả năm: {gradeSummary.totalYearAverage?.toFixed(2) || '-'}</p>
                    </div>

                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Môn học</TableHead>
                                <TableHead>Học kỳ 1</TableHead>
                                <TableHead>Học kỳ 2</TableHead>
                                <TableHead>Cả năm</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {gradeSummary.gradeSummaryEachSubjectNameDtos.map((subject, index) => (
                                <TableRow key={index}>
                                    <TableCell>{subject.subjectName}</TableCell>
                                    <TableCell>{subject.semester1Average?.toFixed(2) || '-'}</TableCell>
                                    <TableCell>{subject.semester2Average?.toFixed(2) || '-'}</TableCell>
                                    <TableCell>{subject.yearAverage?.toFixed(2) || '-'}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                    <div className="mt-4 flex gap-4">
                        <ExportGradeToExcel
                            gradeSummary={gradeSummary}
                            studentInfo={{
                                studentName: gradeSummary.gradeSummaryEachSubjectNameDtos[0]?.studentName,
                                className: classes.find(c => c.classId.toString() === classId)?.className,
                            }}
                            semesterName={semesterName}
                            academicYearName={academicYearName}
                        />
                        <button
                            onClick={handleExportClassGrades}
                            className="px-4 py-2 bg-green-500 text-white rounded hover:bg-green-600 transition-colors"
                            disabled={!classId || !semesterId || loading || students.length === 0}
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