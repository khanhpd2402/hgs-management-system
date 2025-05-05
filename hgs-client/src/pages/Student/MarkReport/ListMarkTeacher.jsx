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
import './ListMarkTeacher.scss';

// Hàm ánh xạ assessmentType
const mapAssessmentType = (field) => {
  if (field.startsWith('TX')) {
    const index = field.replace('TX', '');
    return `ĐĐG TX ${index}`;
  }
  return { GK: 'ĐĐG GK', CK: 'ĐĐG CK' }[field] || '';
};

// Hàm lấy tên ngắn cho đầu điểm (hiển thị trong bảng)
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
  const [isEditing, setIsEditing] = useState(false);
  const [editedGrades, setEditedGrades] = useState({});
  const [editingRows, setEditingRows] = useState({});
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
    setError('');
  }, [academicYear]);

  // Reset state khi học kỳ thay đổi
  useEffect(() => {
    if (!semester) {
      setAssignments([]);
      setSelectedAssignment('');
      setGrades([]);
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

  // Hàm xử lý thay đổi phân công
  const handleAssignmentChange = useCallback((value) => {
    setSelectedAssignment(value);
    setError('');
  }, []);

  // Hàm xác thực trước khi tìm kiếm điểm
  const validateSearch = useCallback(() => {
    if (!selectedAssignment) {
      setError('Vui lòng chọn lớp và môn học.');
      return false;
    }
    if (!semester) {
      setError('Vui lòng chọn học kỳ.');
      return false;
    }
    const assignment = assignments.find((a) => a.assignmentId === parseInt(selectedAssignment));
    if (!assignment) {
      setError('Phân công không hợp lệ.');
      return false;
    }
    return true;
  }, [assignments, selectedAssignment, semester]);

  // Hàm tìm kiếm điểm
  const handleSearchGrades = useCallback(async () => {
    if (!validateSearch()) return;

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
  }, [assignments, selectedAssignment, semester, teacherId, token, validateSearch]);

  // Hàm xử lý thay đổi input
  const handleInputChange = useCallback((studentId, field, value) => {
    // Chỉ validate khi có giá trị và là số
    if (value !== '') {
      const numericValue = Number(value);
      if (isNaN(numericValue) || numericValue < 0 || numericValue > 10) return;
    }

    setEditedGrades(prev => ({
      ...prev,
      [studentId]: { ...prev[studentId], [field]: value }
    }));
  }, []);

  // Hàm lưu điểm chung
  const saveGrades = useCallback(
    async (gradesToSave, studentId = null) => {
      if (!gradesToSave || !Object.keys(gradesToSave).length) return;

      setLoading(true);
      setError('');
      try {
        const gradesPayload = {
          grades: Object.entries(gradesToSave).flatMap(([sId, fields]) =>
            Object.entries(fields)
              .filter(([_, value]) => value !== '')
              .map(([field, value]) => {
                const gradeInfo = grades.find(
                  (g) => g.studentId === parseInt(sId) && g.assessmentType === mapAssessmentType(field)
                );
                return {
                  gradeID: gradeInfo?.gradeId,
                  score: value.toString(),
                  teacherComment: 'nhập điểm',
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

        await handleSearchGrades();
        if (studentId) {
          setEditingRows((prev) => ({ ...prev, [studentId]: false }));
          setEditedGrades((prev) => {
            const newGrades = { ...prev };
            delete newGrades[studentId];
            return newGrades;
          });
        } else {
          setIsEditing(false);
          setEditedGrades({});
        }
      } catch (error) {
        console.error('Error saving grades:', error);
        setError('Không thể lưu điểm. Vui lòng thử lại.');
      } finally {
        setLoading(false);
      }
    },
    [grades, token, handleSearchGrades]
  );

  // Hàm lưu điểm toàn bộ
  const handleSaveGrades = useCallback(() => saveGrades(editedGrades), [editedGrades, saveGrades]);

  // Hàm lưu điểm từng hàng
  const handleSaveRow = useCallback(
    (studentId) => saveGrades({ [studentId]: editedGrades[studentId] }, studentId),
    [editedGrades, saveGrades]
  );

  // Hàm chỉnh sửa hàng
  const handleEditRow = useCallback(
    async (studentId) => {
      setLoading(true);
      setError('');
      try {
        const studentGrades = grades.filter((g) => g.studentId === studentId);
        const gradesPayload = {
          grades: studentGrades.map((grade) => ({
            gradeID: grade.gradeId,
            score: grade.score ? grade.score.toString() : '0',
            teacherComment: 'nhập điểm',
          })),
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

        await handleSearchGrades();
        setEditingRows((prev) => ({ ...prev, [studentId]: true }));
      } catch (error) {
        console.error('Error updating grades:', error);
        setError('Không thể cập nhật điểm. Vui lòng thử lại.');
      } finally {
        setLoading(false);
      }
    },
    [grades, token, handleSearchGrades]
  );

  // Nhóm điểm theo học sinh
  const groupedGrades = useMemo(() => {
    const grouped = grades.reduce((acc, { studentId, studentName, assessmentType, score, teacherComment }) => {
      if (!acc[studentId]) {
        acc[studentId] = {
          studentId,
          studentName,
          regularAssessments: {},
          GK: null,
          CK: null,
          teacherComment,
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
            disabled={!selectedAssignment || loading}
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
          <h2 className="text-lg font-semibold">Danh sách điểm học sinh</h2>
          {grades.length > 0 && (
            <Button
              onClick={() => setIsEditing(!isEditing)}
              className={isEditing ? "bg-green-600 hover:bg-green-700" : "bg-blue-600 hover:bg-blue-700"}
              disabled={loading}
            >
              {isEditing ? 'Lưu điểm' : 'Nhập điểm'}
            </Button>
          )}
        </div>

        <Table>
          <TableHeader>
            <TableRow>
              <TableHead rowSpan="2" className="border text-center">Stt</TableHead>
              <TableHead rowSpan="2" className="border text-center">Tên học sinh</TableHead>
              <TableHead colSpan={regularAssessmentTypes.length} className="border text-center">
                Điểm thường xuyên
              </TableHead>
              <TableHead rowSpan="2" className="border text-center">Điểm giữa kỳ</TableHead>
              <TableHead rowSpan="2" className="border text-center">Điểm cuối kỳ</TableHead>
              <TableHead rowSpan="2" className="border text-center">Nhận xét</TableHead>
              <TableHead rowSpan="2" className="border text-center">Hành động</TableHead>
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
                    return (
                      <TableCell key={type} className="border text-center">
                        {isEditing || editingRows[student.studentId] ? (
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
                        ) : (
                          student.regularAssessments[shortName] !== null
                            ? student.regularAssessments[shortName]
                            : 'Chưa có điểm'
                        )}
                      </TableCell>
                    );
                  })}
                  {['GK', 'CK'].map((field) => (
                    <TableCell key={field} className="border text-center">
                      {isEditing || editingRows[student.studentId] ? (
                        <Input
                          type="text"
                          inputMode="decimal"
                          pattern="[0-9]*[.]?[0-9]*"
                          value={editedGrades[student.studentId]?.[field] ?? student[field] ?? ''}
                          onChange={(e) => handleInputChange(student.studentId, field, e.target.value)}
                          className="w-20 text-center focus:outline-none focus:ring-2 focus:ring-blue-500"
                          disabled={loading}
                        />
                      ) : (
                        student[field] !== null ? student[field] : 'Chưa có điểm'
                      )}
                    </TableCell>
                  ))}
                  <TableCell className="border">{student.teacherComment || 'Chưa có nhận xét'}</TableCell>
                  <TableCell className="border text-center">
                    <Button
                      onClick={() =>
                        editingRows[student.studentId]
                          ? handleSaveRow(student.studentId)
                          : handleEditRow(student.studentId)
                      }
                      className={
                        editingRows[student.studentId]
                          ? "bg-green-600 hover:bg-green-700"
                          : "bg-blue-600 hover:bg-blue-700"
                      }
                      disabled={loading}
                    >
                      {editingRows[student.studentId] ? 'Lưu' : 'Cập nhật'}
                    </Button>
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={regularAssessmentTypes.length + 6}
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
