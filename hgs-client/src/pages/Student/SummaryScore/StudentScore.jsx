import React, { useState, useEffect, useMemo } from 'react';
import { jwtDecode } from 'jwt-decode';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useAcademicYears } from '../../../services/common/queries';
import { getStudentNameAndClass, getSemesterByYear } from '../../../services/schedule/api';
import './StudentScore.scss';

// Hàm ánh xạ assessmentType
const mapAssessmentType = (field) => {
  if (field.startsWith('TX')) {
    const index = field.replace('TX', '');
    return `ĐĐG TX ${index}`;
  }
  return { GK: 'ĐĐG GK', CK: 'ĐĐG CK' }[field] || '';
};

// Hàm lấy tên ngắn cho đầu điểm
const getShortAssessmentName = (assessmentType) => {
  if (assessmentType.startsWith('ĐĐG TX')) {
    const index = assessmentType.split(' ')[2];
    return `TX${index}`;
  }
  return { 'ĐĐG GK': 'GK', 'ĐĐG CK': 'CK' }[assessmentType] || assessmentType;
};

// Hàm tính điểm trung bình
const calculateAverageScore = (regularAssessments, gk, ck) => {
  const txScores = Object.values(regularAssessments).filter(score => score !== null && !isNaN(score));
  const txCount = txScores.length;
  const txAverage = txCount > 0 ? txScores.reduce((sum, score) => sum + parseFloat(score), 0) / txCount : 0;

  let totalWeight = txCount;
  let weightedSum = txAverage * txCount;

  if (gk !== null && !isNaN(gk)) {
    weightedSum += parseFloat(gk) * 2;
    totalWeight += 2;
  }

  if (ck !== null && !isNaN(ck)) {
    weightedSum += parseFloat(ck) * 3;
    totalWeight += 3;
  }

  return totalWeight > 0 ? (weightedSum / totalWeight).toFixed(2) : 'Chưa đủ điểm';
};

const StudentScore = () => {
  const [studentId, setStudentId] = useState(null);
  const [academicYearId, setAcademicYearId] = useState('');
  const [semesterId, setSemesterId] = useState('');
  const [studentList, setStudentList] = useState([]);
  const [selectedStudent, setSelectedStudent] = useState(null);
  const [semesters, setSemesters] = useState([]);
  const [grades, setGrades] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const { data: academicYears, isLoading: academicYearsLoading } = useAcademicYears();

  // Lấy danh sách học kỳ khi năm học thay đổi
  useEffect(() => {
    const fetchSemesters = async () => {
      if (academicYearId) {
        try {
          const semesterData = await getSemesterByYear(academicYearId);
          setSemesters(semesterData || []);
          setSemesterId(semesterData[0]?.semesterID.toString() || '');
        } catch (error) {
          console.error('Lỗi khi lấy dữ liệu học kỳ:', error);
          setSemesters([]);
          setSemesterId('');
          setError('Không thể tải danh sách học kỳ.');
        }
      } else {
        setSemesters([]);
        setSemesterId('');
      }
    };
    fetchSemesters();
  }, [academicYearId]);

  // Lấy danh sách học sinh khi năm học thay đổi
  useEffect(() => {
    const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
    if (token && academicYearId) {
      const tokenParts = token.split('.');
      if (tokenParts.length === 3) {
        try {
          const payload = jwtDecode(token);
          const studentIdList = payload.studentIds.split(',');

          Promise.all(
            studentIdList.map(id =>
              getStudentNameAndClass(id, academicYearId)
            )
          ).then(students => {
            const formattedStudents = students.map(student => ({
              value: student.studentId.toString(),
              label: `${student.fullName} - Lớp ${student.className}`,
              studentInfo: student
            }));
            setStudentList(formattedStudents);
            setStudentId(formattedStudents[0]?.value || null);
            setSelectedStudent(formattedStudents[0]?.studentInfo || null);
          }).catch(error => {
            console.error('Error fetching student details:', error);
            setError('Không thể tải danh sách học sinh.');
          });
        } catch (e) {
          console.error('Invalid token:', e);
          setError('Phiên đăng nhập không hợp lệ.');
        }
      }
    }
  }, [academicYearId]);

  // Gọi API lấy danh sách điểm khi studentId hoặc semesterId thay đổi
  useEffect(() => {
    const fetchGrades = async () => {
      if (studentId && semesterId) {
        setLoading(true);
        setError('');
        try {
          const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
          const response = await fetch(
            `https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Grades/student?studentId=${studentId}&semesterId=${semesterId}`,
            {
              method: 'GET',
              headers: {
                'Accept': '*/*',
                'Authorization': `Bearer ${token}`
              }
            }
          );
          if (!response.ok) {
            throw new Error('Failed to fetch grades');
          }
          const data = await response.json();
          setGrades(data || []);
        } catch (error) {
          console.error('Error fetching grades:', error);
          setError('Không thể tải danh sách điểm.');
          setGrades([]);
        } finally {
          setLoading(false);
        }
      } else {
        setGrades([]);
      }
    };
    fetchGrades();
  }, [studentId, semesterId]);

  // Cập nhật thông tin học sinh khi chọn học sinh
  const handleStudentChange = (value) => {
    setStudentId(value);
    const selectedStudentInfo = studentList.find(student => student.value === value)?.studentInfo;
    setSelectedStudent(selectedStudentInfo);
  };

  // Lấy danh sách các đầu điểm thường xuyên
  const regularAssessmentTypes = useMemo(() => {
    const types = new Set(grades.map((g) => g.assessmentType));
    return Array.from(types)
      .filter((type) => type.startsWith('ĐĐG TX'))
      .sort((a, b) => {
        const indexA = parseInt(a.split(' ')[2]) || 0;
        const indexB = parseInt(b.split(' ')[2]) || 0;
        return indexA - indexB;
      });
  }, [grades]);

  // Nhóm điểm theo môn học
  const groupedGrades = useMemo(() => {
    const grouped = grades.reduce((acc, { subjectName, assessmentType, score, teacherComment }) => {
      if (!acc[subjectName]) {
        acc[subjectName] = {
          subjectName,
          regularAssessments: {},
          GK: null,
          CK: null,
          teacherComment,
        };
      }
      if (assessmentType.startsWith('ĐĐG TX')) {
        const shortName = getShortAssessmentName(assessmentType);
        acc[subjectName].regularAssessments[shortName] = score;
      } else if (assessmentType === 'ĐĐG GK') {
        acc[subjectName].GK = score;
      } else if (assessmentType === 'ĐĐG CK') {
        acc[subjectName].CK = score;
      }
      return acc;
    }, {});
    return Object.values(grouped);
  }, [grades]);

  return (
    <div className="container mx-auto py-6">
      <div className="space-y-4">
        <div className="flex items-center justify-between border-b pb-4">
          <div>
            <h1 className="text-2xl font-bold">Bảng điểm học sinh</h1>
            <p className="text-muted-foreground text-sm">
              Xem điểm số của học sinh theo học kỳ
            </p>
          </div>
        </div>

        <div className="grid gap-4 md:grid-cols-3">
          <div>
            <Select
              value={academicYearId}
              onValueChange={setAcademicYearId}
              disabled={loading || academicYearsLoading}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="-- Chọn năm học --" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="default" disabled>-- Chọn năm học --</SelectItem>
                {academicYears?.map((year) => (
                  <SelectItem key={year.academicYearID} value={year.academicYearID.toString()}>
                    {year.yearName}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <div>
            <Select
              value={semesterId}
              onValueChange={setSemesterId}
              disabled={loading || !academicYearId || !semesters.length}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="-- Chọn học kỳ --" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="default" disabled>-- Chọn học kỳ --</SelectItem>
                {semesters?.map((semester) => (
                  <SelectItem key={semester.semesterID} value={semester.semesterID.toString()}>
                    {semester.semesterName}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
          <div>
            <Select
              value={studentId}
              onValueChange={handleStudentChange}
              disabled={loading || !studentList.length}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="-- Chọn học sinh --" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="default" disabled>-- Chọn học sinh --</SelectItem>
                {studentList.map((student) => (
                  <SelectItem key={student.value} value={student.value}>
                    {student.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>

        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
            {error}
          </div>
        )}

        {selectedStudent && (
          <div className="flex justify-between items-center">
            <h2 className="text-lg font-semibold">
              Bảng Điểm - {selectedStudent.fullName} - Lớp {selectedStudent.className}
            </h2>
          </div>
        )}

        <Table>
          <TableHeader>
            <TableRow>
              <TableHead rowSpan="2" className="border text-center">Môn học</TableHead>
              <TableHead colSpan={regularAssessmentTypes.length} className="border text-center">
                Điểm thường xuyên
              </TableHead>
              <TableHead rowSpan="2" className="border text-center">Điểm giữa kỳ</TableHead>
              <TableHead rowSpan="2" className="border text-center">Điểm cuối kỳ</TableHead>
              <TableHead rowSpan="2" className="border text-center">Điểm trung bình</TableHead>
              <TableHead rowSpan="2" className="border text-center">Nhận xét</TableHead>
            </TableRow>
            <TableRow>
              {regularAssessmentTypes.map((type) => (
                <TableHead key={type} className="border text-center">
                  {getShortAssessmentName(type)}
                </TableHead>
              ))}
            </TableRow>
          </TableHeader>
          <TableBody>
            {groupedGrades.length > 0 ? (
              groupedGrades.map((subject, index) => (
                <TableRow key={`${subject.subjectName}-${index}`}>
                  <TableCell className="border">{subject.subjectName}</TableCell>
                  {regularAssessmentTypes.map((type) => {
                    const shortName = getShortAssessmentName(type);
                    return (
                      <TableCell key={type} className="border text-center">
                        {subject.regularAssessments[shortName] !== null
                          ? subject.regularAssessments[shortName]
                          : 'Chưa có điểm'}
                      </TableCell>
                    );
                  })}
                  <TableCell className="border text-center">
                    {subject.GK !== null ? subject.GK : 'Chưa có điểm'}
                  </TableCell>
                  <TableCell className="border text-center">
                    {subject.CK !== null ? subject.CK : 'Chưa có điểm'}
                  </TableCell>
                  <TableCell className="border text-center">
                    {calculateAverageScore(subject.regularAssessments, subject.GK, subject.CK)}
                  </TableCell>
                  <TableCell className="border">
                    {subject.teacherComment || 'Không có nhận xét'}
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={regularAssessmentTypes.length + 5}
                  className="text-center"
                >
                  Không có dữ liệu điểm.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
};

export default StudentScore;