import React, { useState, useCallback, useMemo, useEffect } from 'react';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useAcademicYears, useSemestersByAcademicYear } from '../../../services/common/queries';
import toast from 'react-hot-toast';
import './ListMarkTeacher.scss';
import ExportExcelGrade from '../../Grade/ExportExcelGrade';
import ImportGrade from './ImportGrade';

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

const ListMarkTeacher = () => {
  const [academicYear, setAcademicYear] = useState('');
  const [semester, setSemester] = useState('');
  const [assignments, setAssignments] = useState([]);
  const [selectedAssignment, setSelectedAssignment] = useState('');
  const [grades, setGrades] = useState([]);
  const [subjectInfo, setSubjectInfo] = useState(null);
  const [isEditing, setIsEditing] = useState(false);
  const [editedGrades, setEditedGrades] = useState({});
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Lấy và kiểm tra token
  const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
  const decoded = useMemo(() => {
    try {
      return token ? jwtDecode(token) : {};
    } catch (e) {
      console.error('Invalid token:', e);
      return {};
    }
  }, [token]);
  const teacherId = decoded?.teacherId;

  // Queries cho năm học và học kỳ
  const { data: academicYears, isLoading: academicYearsLoading } = useAcademicYears();
  const { data: semesters, isLoading: semestersLoading } = useSemestersByAcademicYear(academicYear);

  // Reset state khi năm học thay đổi
  useEffect(() => {
    if (!academicYear) return;
    setSemester('');
    setAssignments([]);
    setSelectedAssignment('');
    setGrades([]);
    setSubjectInfo(null);
    setError('');
  }, [academicYear]);

  // Reset state khi học kỳ thay đổi
  useEffect(() => {
    if (!semester) {
      setAssignments([]);
      setSelectedAssignment('');
      setGrades([]);
      setSubjectInfo(null);
      setError('');
    }
  }, [semester]);

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

  // Hàm xử lý thay đổi học kỳ và lấy phân công
  const handleSemesterChange = useCallback(
    async (value) => {
      setSemester(value);
      if (!teacherId || !value) return;

      setLoading(true);
      try {
        const response = await axios.get(
          `https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/TeachingAssignment/teacher/${teacherId}/semester/${value}`,
          { headers: { Authorization: `Bearer ${token}` } }
        );
        setAssignments(response.data);
      } catch (error) {
        console.error('Error fetching assignments:', error);
        setError('Không thể tải danh sách phân công giảng dạy.');
        setAssignments([]);
      } finally {
        setLoading(false);
      }
    },
    [teacherId, token]
  );

  // Hàm xử lý thay đổi phân công và gọi API Subjects
  const handleAssignmentChange = useCallback(async (value) => {
    setSelectedAssignment(value);
    setError('');
    setSubjectInfo(null);

    if (value) {
      const assignment = assignments.find((a) => a.assignmentId === parseInt(value));
      if (assignment) {
        try {
          setLoading(true);
          const response = await axios.get(
            `https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Subjects/${assignment.subjectId}`,
            { headers: { Authorization: `Bearer ${token}` } }
          );
          setSubjectInfo(response.data);
        } catch (error) {
          console.error('Error fetching subject info:', error);
          setError('Không thể tải thông tin môn học.');
        } finally {
          setLoading(false);
        }
      }
    }
  }, [assignments, token]);

  // Hàm tìm kiếm điểm
  const handleSearchGrades = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const assignment = assignments.find((a) => a.assignmentId === parseInt(selectedAssignment));
      const { subjectId, classId } = assignment;
      const response = await axios.get(`https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Grades/teacher`, {
        params: { teacherId, classId, subjectId, semesterId: semester },
        headers: { Authorization: `Bearer ${token}` },
      });
      setGrades(response.data);
    } catch (error) {
      console.error('Error fetching grades:', error);
      setError('Không thể tải danh sách điểm.');
      setGrades([]);
    } finally {
      setLoading(false);
    }
  }, [assignments, selectedAssignment, semester, teacherId, token]);

  // Hàm xử lý thay đổi input
  const handleInputChange = useCallback((studentId, field, value) => {
    setEditedGrades(prev => ({
      ...prev,
      [studentId]: { ...prev[studentId], [field]: value }
    }));
  }, []);

  // Hàm kiểm tra điểm hợp lệ
  const validateGrades = useCallback((gradesToValidate) => {
    if (!gradesToValidate || !Object.keys(gradesToValidate).length) return true;

    if (subjectInfo?.typeOfGrade !== 'Nhận xét') {
      for (const [studentId, fields] of Object.entries(gradesToValidate)) {
        for (const [field, value] of Object.entries(fields)) {
          if (value === '') continue;

          // Kiểm tra giá trị là số hợp lệ
          if (!/^\d*\.?\d*$/.test(value)) {
            toast.error(`Điểm phải là số hợp lệ`);
            return false;
          }

          // Kiểm tra giá trị nằm trong khoảng 0-10
          const numValue = parseFloat(value);
          if (isNaN(numValue) || numValue < 0 || numValue > 10) {
            toast.error(`Điểm phải nằm trong khoảng từ 0 đến 10`);
            return false;
          }
        }
      }
    }
    return true;
  }, [subjectInfo]);

  // Hàm lưu điểm
  const saveGrades = useCallback(
    async (gradesToSave) => {
      if (!gradesToSave || !Object.keys(gradesToSave).length) return;

      // Kiểm tra điểm trước khi lưu
      if (!validateGrades(gradesToSave)) {
        return;
      }

      setLoading(true);
      setError('');
      try {
        const gradesPayload = {
          grades: Object.entries(gradesToSave).flatMap(([sId, fields]) =>
            Object.entries(fields)
              .filter(([key, value]) => value !== '')
              .map(([field, value]) => {
                const gradeInfo = grades.find(
                  (g) => g.studentId === parseInt(sId) && g.assessmentType === mapAssessmentType(field)
                );
                const scoreValue = subjectInfo?.typeOfGrade === 'Nhận xét'
                  ? (value === 'Đạt' ? 'Đ' : value === 'Không Đạt' ? 'KĐ' : value)
                  : value;
                return {
                  gradeID: gradeInfo?.gradeId,
                  score: scoreValue.toString(),
                  teacherComment: 'Học sinh đi học và làm bài đầy đủ',
                };
              })
          ),
        };

        await axios.put(
          `https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Grades/update-multiple-scores`,
          gradesPayload,
          {
            headers: {
              Authorization: `Bearer ${token}`,
              'Content-Type': 'application/json;odata.metadata=minimal;odata.streaming=true',
              accept: '*/*',
            },
          }
        );

        toast.success('Lưu điểm thành công!');
        await handleSearchGrades();
        setIsEditing(false);
        setEditedGrades({});
      } catch (error) {
        console.error('Error saving grades:', error);
        toast.error('Không thể lưu điểm. Vui lòng thử lại.');
      } finally {
        setLoading(false);
      }
    },
    [grades, token, handleSearchGrades, subjectInfo, validateGrades]
  );

  // Hàm lưu điểm toàn bộ
  const handleSaveGrades = useCallback(() => saveGrades(editedGrades), [editedGrades, saveGrades]);

  // Nhóm điểm theo học sinh
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
        const shortName = getShortAssessmentName(assessmentType);
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

  // Get selected assignment details for display
  const selectedAssignmentDetails = useMemo(() => {
    return assignments.find((a) => a.assignmentId === parseInt(selectedAssignment));
  }, [assignments, selectedAssignment]);

  return (
    <div className="container mx-auto py-6">
      <div className="space-y-4">
        <div className="flex items-center justify-between border-b pb-4">
          <div>
            <h1 className="text-2xl font-bold">Quản lý điểm học sinh</h1>
            <p className="text-muted-foreground text-sm">
              Quản lý điểm số của học sinh theo học kỳ
            </p>
          </div>
        </div>

        <div className="grid gap-4 md:grid-cols-3">
          <div>
            <Select
              value={academicYear}
              onValueChange={setAcademicYear}
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
              value={semester}
              onValueChange={handleSemesterChange}
              disabled={loading || semestersLoading || !academicYear}
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
              value={selectedAssignment}
              onValueChange={handleAssignmentChange}
              disabled={loading || !semester}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="-- Chọn lớp và môn học --" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="default" disabled>-- Chọn lớp và môn học --</SelectItem>
                {assignments.map((a) => (
                  <SelectItem key={a.assignmentId} value={a.assignmentId.toString()}>
                    {a.subjectName} - {a.className}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>

        <div className="flex justify-end">
          <Button
            onClick={handleSearchGrades}
            className="bg-blue-600 hover:bg-blue-700 text-white"
            disabled={loading}
          >
            {loading ? 'Đang tải...' : 'Tìm kiếm'}
          </Button>
        </div>

        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
            {error}
          </div>
        )}

        <div className="flex justify-between items-center">
          <h2 className="text-lg font-semibold">
            Danh sách điểm học sinh {selectedAssignmentDetails && subjectInfo ? (
              `- Lớp: ${selectedAssignmentDetails.className} - Môn: ${selectedAssignmentDetails.subjectName} - Loại điểm: ${subjectInfo.typeOfGrade}`
            ) : ''}
          </h2>
          <div className="flex gap-2">
            {grades.length > 0 && (
              <>
                <Button
                  onClick={() => {
                    if (isEditing) handleSaveGrades();
                    setIsEditing(!isEditing);
                  }}
                  className={isEditing ? "bg-green-600 hover:bg-green-700" : "bg-blue-600 hover:bg-blue-700"}
                  disabled={loading}
                >
                  {isEditing ? 'Lưu điểm' : 'Nhập điểm'}
                </Button>
                <ExportExcelGrade
                  grades={groupedGrades}
                  selectedSubject={subjectInfo}
                  regularColumns={regularAssessmentTypes.map(type => getShortAssessmentName(type))}
                  semesterId={semester}
                  classId={selectedAssignmentDetails?.classId}
                  className={selectedAssignmentDetails?.className}
                />
                <ImportGrade
                  classId={selectedAssignmentDetails?.classId}
                  subjectId={selectedAssignmentDetails?.subjectId}
                  semesterId={semester}
                />
              </>
            )}
          </div>
        </div>

        <Table>
          <TableHeader>
            <TableRow>
              <TableHead rowSpan="2" className="border text-center">Stt</TableHead>
              <TableHead rowSpan="2" className="border text-center">Tên học sinh</TableHead>
              <TableHead colSpan={regularAssessmentTypes.length} className="border text-center">
                {subjectInfo?.typeOfGrade === 'Nhận xét' ? 'Đánh giá' : 'Điểm thường xuyên'}
              </TableHead>
              {subjectInfo?.typeOfGrade !== 'Nhận xét' && (
                <>
                  <TableHead rowSpan="2" className="border text-center">Điểm giữa kỳ</TableHead>
                  <TableHead rowSpan="2" className="border text-center">Điểm cuối kỳ</TableHead>
                </>
              )}
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
              groupedGrades.map((student, index) => (
                <TableRow key={student.studentId}>
                  <TableCell className="border text-center">{index + 1}</TableCell>
                  <TableCell className="border">{student.studentName}</TableCell>
                  {regularAssessmentTypes.map((type) => {
                    const shortName = getShortAssessmentName(type);
                    const displayValue = student.regularAssessments[shortName] === 'Đ'
                      ? 'Đạt'
                      : student.regularAssessments[shortName] === 'KĐ'
                        ? 'Không Đạt'
                        : student.regularAssessments[shortName];
                    return (
                      <TableCell key={type} className="border text-center">
                        {isEditing ? (
                          subjectInfo?.typeOfGrade === 'Nhận xét' ? (
                            <Select
                              value={
                                editedGrades[student.studentId]?.[shortName] ??
                                (student.regularAssessments[shortName] === 'Đ' ? 'Đạt' :
                                  student.regularAssessments[shortName] === 'KĐ' ? 'Không Đạt' : '') ??
                                ''
                              }
                              onValueChange={(value) => handleInputChange(student.studentId, shortName, value)}
                              disabled={loading}
                            >
                              <SelectTrigger className="w-32">
                                <SelectValue placeholder="-- Chọn --" />
                              </SelectTrigger>
                              <SelectContent>
                                <SelectItem value="Đạt">Đạt</SelectItem>
                                <SelectItem value="Không Đạt">Không Đạt</SelectItem>
                              </SelectContent>
                            </Select>
                          ) : (
                            <Input
                              type="text"
                              inputMode="decimal"
                              pattern="[0-9]*[.]?[0-9]*"
                              value={
                                editedGrades[student.studentId]?.[shortName] ??
                                student.regularAssessments[shortName] ??
                                ''
                              }
                              onChange={(e) => handleInputChange(student.studentId, shortName, e.target.value)}
                              className="w-20 text-center focus:outline-none focus:ring-2 focus:ring-blue-500"
                              disabled={loading}
                            />
                          )
                        ) : (
                          displayValue !== null && displayValue !== undefined
                            ? displayValue
                            : 'Chưa có điểm'
                        )}
                      </TableCell>
                    );
                  })}
                  {subjectInfo?.typeOfGrade !== 'Nhận xét' && (
                    <>
                      {['GK', 'CK'].map((field) => {
                        const displayValue = student[field] === 'Đ'
                          ? 'Đạt'
                          : student[field] === 'KĐ'
                            ? 'Không Đạt'
                            : student[field];
                        return (
                          <TableCell key={field} className="border text-center">
                            {isEditing ? (
                              subjectInfo?.typeOfGrade === 'Nhận xét' ? (
                                <Select
                                  value={
                                    editedGrades[student.studentId]?.[field] ??
                                    (student[field] === 'Đ' ? 'Đạt' :
                                      student[field] === 'KĐ' ? 'Không Đạt' : '') ??
                                    ''
                                  }
                                  onValueChange={(value) => handleInputChange(student.studentId, field, value)}
                                  disabled={loading}
                                >
                                  <SelectTrigger className="w-32">
                                    <SelectValue placeholder="-- Chọn --" />
                                  </SelectTrigger>
                                  <SelectContent>
                                    <SelectItem value="Đạt">Đạt</SelectItem>
                                    <SelectItem value="Không Đạt">Không Đạt</SelectItem>
                                  </SelectContent>
                                </Select>
                              ) : (
                                <Input
                                  type="text"
                                  inputMode="decimal"
                                  pattern="[0-9]*[.]?[0-9]*"
                                  value={editedGrades[student.studentId]?.[field] ?? student[field] ?? ''}
                                  onChange={(e) => handleInputChange(student.studentId, field, e.target.value)}
                                  className="w-20 text-center focus:outline-none focus:ring-2 focus:ring-blue-500"
                                  disabled={loading}
                                />
                              )
                            ) : (
                              displayValue !== null && displayValue !== undefined
                                ? displayValue
                                : 'Chưa có điểm'
                            )}
                          </TableCell>
                        );
                      })}
                    </>
                  )}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={regularAssessmentTypes.length + (subjectInfo?.typeOfGrade !== 'Nhận xét' ? 4 : 2)}
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

export default ListMarkTeacher;