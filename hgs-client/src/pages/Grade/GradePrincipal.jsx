import React, { useState, useEffect, useMemo } from 'react';
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
import ExportExcelGrade from './ExportExcelGrade';

const GradePrincipal = () => {
    const [academicYearId, setAcademicYearId] = useState('');
    const [semesterId, setSemesterId] = useState('');
    const [subjectId, setSubjectId] = useState('1');
    const [classId, setClassId] = useState('1');
    const [semesters, setSemesters] = useState([]);
    const [grades, setGrades] = useState([]);
    const [subjects, setSubjects] = useState([]);
    const [classes, setClasses] = useState([]);
    const [loading, setLoading] = useState(false);
    const [selectedSubject, setSelectedSubject] = useState(null);

    // Fetch semesters when clicking on semester select
    const handleSemesterSelectClick = async () => {
        const storedAcademicYearId = localStorage.getItem('selectedAcademicYearID');
        if (storedAcademicYearId) {
            setAcademicYearId(storedAcademicYearId);
            try {
                const response = await axios.get(`https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Semester/by-academic-year/${storedAcademicYearId}`);
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

    // Fetch subjects
    useEffect(() => {
        const fetchSubjects = async () => {
            try {
                const response = await axios.get('https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Subjects');
                setSubjects(response.data);
            } catch (error) {
                console.error('Lỗi khi tải danh sách môn học:', error);
            }
        };
        fetchSubjects();
    }, []);

    // Fetch classes
    useEffect(() => {
        const fetchClasses = async () => {
            try {
                const response = await axios.get('https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Classes');
                const activeClasses = response.data.filter(c =>
                    c.status.toLowerCase() === 'hoạt động' ||
                    c.status.toLowerCase() === 'Hoạt động'.toLowerCase()
                );
                setClasses(activeClasses);
            } catch (error) {
                console.error('Lỗi khi tải danh sách lớp học:', error);
            }
        };
        fetchClasses();
    }, []);

    // Fetch grades when filters change
    useEffect(() => {
        const fetchGrades = async () => {
            setLoading(true);
            try {
                const response = await axios.get(`https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Grades/school`, {
                    params: {
                        classId,
                        subjectId,
                        semesterId
                    }
                });
                setGrades(response.data);
            } catch (error) {
                console.error('Lỗi khi tải điểm:', error);
            } finally {
                setLoading(false);
            }
        };

        if (semesterId && subjectId && classId) {
            fetchGrades();
        }
    }, [semesterId, subjectId, classId]);

    // Fetch subject detail when subject changes
    useEffect(() => {
        const fetchSubjectDetail = async () => {
            try {
                const response = await axios.get(`https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Subjects/${subjectId}`);
                setSelectedSubject(response.data);
            } catch (error) {
                console.error('Lỗi khi tải thông tin môn học:', error);
            }
        };

        if (subjectId) {
            fetchSubjectDetail();
        }
    }, [subjectId]);

    // Group grades by student
    const groupedGrades = useMemo(() => {
        const grouped = grades.reduce((acc, { studentId, studentName, assessmentType, score }) => {
            if (!acc[studentId]) {
                acc[studentId] = {
                    studentId,
                    studentName,
                    regularAssessments: {},
                    GK: null,
                    CK: null,
                };
            }
            if (assessmentType.startsWith('ĐĐG TX')) {
                const shortName = assessmentType.replace('ĐĐG TX ', 'TX ');
                acc[studentId].regularAssessments[shortName] = score;
            } else if (assessmentType === 'ĐĐG GK') {
                acc[studentId].GK = score;
            } else if (assessmentType === 'ĐĐG CK') {
                acc[studentId].CK = score;
            }
            return acc;
        }, {});
        return Object.values(grouped);
    }, [grades]);

    // Get list of regular assessment columns
    const regularColumns = useMemo(() => {
        const columns = new Set();
        groupedGrades.forEach(student => {
            Object.keys(student.regularAssessments).forEach(col => columns.add(col));
        });
        return Array.from(columns).sort();
    }, [groupedGrades]);

    // Calculate final grade
    const calculateFinalGrade = (student) => {
        if (!student.regularAssessments && !student.GK && !student.CK) {
            return '-';
        }

        const txScores = Object.values(student.regularAssessments)
            .filter(score => score !== null && score !== undefined && score !== '')
            .map(score => parseFloat(score));

        const txAverage = txScores.length > 0
            ? txScores.reduce((sum, score) => sum + score, 0) / txScores.length
            : 0;

        const gkScore = student.GK ? parseFloat(student.GK) : 0;
        const ckScore = student.CK ? parseFloat(student.CK) : 0;

        const finalGrade = (txAverage + gkScore * 2 + ckScore * 3) / 6;
        return finalGrade.toFixed(1);
    };

    // Render table header based on subject type
    const renderTableHeader = () => {
        if (!selectedSubject) return null;

        if (selectedSubject.typeOfGrade === 'Nhận xét') {
            return (
                <TableHeader>
                    <TableRow>
                        <TableHead>STT</TableHead>
                        <TableHead>Mã học sinh</TableHead>
                        <TableHead>Tên học sinh</TableHead>
                        <TableHead>Đánh giá</TableHead>
                    </TableRow>
                </TableHeader>
            );
        }

        return (
            <TableHeader>
                <TableRow>
                    <TableHead rowSpan={2}>STT</TableHead>
                    <TableHead rowSpan={2}>Mã học sinh</TableHead>
                    <TableHead rowSpan={2}>Tên học sinh</TableHead>
                    <TableHead colSpan={regularColumns.length} className="text-center">
                        Điểm thường xuyên
                    </TableHead>
                    <TableHead rowSpan={2}>Điểm giữa kỳ</TableHead>
                    <TableHead rowSpan={2}>Điểm cuối kỳ</TableHead>
                    <TableHead rowSpan={2}>Điểm TK</TableHead>
                </TableRow>
                <TableRow>
                    {regularColumns.map(col => (
                        <TableHead key={col} className="text-center">{col}</TableHead>
                    ))}
                </TableRow>
            </TableHeader>
        );
    };

    // Render table body based on subject type
    const renderTableBody = () => {
        if (!selectedSubject) return null;

        if (selectedSubject.typeOfGrade === 'Nhận xét') {
            return (
                <TableBody>
                    {groupedGrades.map((student, index) => (
                        <TableRow key={student.studentId}>
                            <TableCell>{index + 1}</TableCell>
                            <TableCell>{student.studentId}</TableCell>
                            <TableCell>{student.studentName}</TableCell>
                            <TableCell>{student.regularAssessments['TX 1'] || '-'}</TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            );
        }

        return (
            <TableBody>
                {groupedGrades.map((student, index) => (
                    <TableRow key={student.studentId}>
                        <TableCell>{index + 1}</TableCell>
                        <TableCell>{student.studentId}</TableCell>
                        <TableCell>{student.studentName}</TableCell>
                        {regularColumns.map(col => (
                            <TableCell key={col} className="text-center">
                                {student.regularAssessments[col] || '-'}
                            </TableCell>
                        ))}
                        <TableCell className="text-center">{student.GK || '-'}</TableCell>
                        <TableCell className="text-center">{student.CK || '-'}</TableCell>
                        <TableCell className="text-center font-semibold">
                            {calculateFinalGrade(student)}
                        </TableCell>
                    </TableRow>
                ))}
            </TableBody>
        );
    };

    return (
        <div className="container mx-auto p-4">
            <div className="flex gap-4 mb-6">
                <Select value={semesterId} onValueChange={setSemesterId} onOpenChange={(open) => {
                    if (open) handleSemesterSelectClick();
                }}>
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

                <Select value={subjectId} onValueChange={setSubjectId}>
                    <SelectTrigger className="w-[180px]">
                        <SelectValue placeholder="Chọn môn học" />
                    </SelectTrigger>
                    <SelectContent>
                        {subjects.map(subject => (
                            <SelectItem key={subject.subjectID} value={subject.subjectID.toString()}>
                                {subject.subjectName}
                            </SelectItem>
                        ))}
                    </SelectContent>
                </Select>

                <Select value={classId} onValueChange={setClassId}>
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
                <ExportExcelGrade
                    grades={groupedGrades}
                    selectedSubject={selectedSubject}
                    regularColumns={regularColumns}
                    semesterId={semesterId}
                    classId={classId}
                    className={classes.find(cls => cls.classId.toString() === classId)?.className}
                />
            </div>

            {loading ? (
                <div>Đang tải...</div>
            ) : (
                <Table>
                    {renderTableHeader()}
                    {renderTableBody()}
                </Table>
            )}
        </div>
    );
};

export default GradePrincipal;