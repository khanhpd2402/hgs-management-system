import React, { useState, useCallback, useMemo, useEffect } from 'react';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';
import toast from 'react-hot-toast';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { useAcademicYears, useSemestersByAcademicYear } from '../../../services/common/queries';
import './ListMarkTeacher.scss';

// Custom NumberInput component with simple input
const NumberInput = ({ value, onChange }) => {
  return (
    <div className="flex items-center w-20 mx-auto">
      <input
        id={`grade-input-${Math.random().toString(36).substring(2)}`}
        type="text"
        inputMode="decimal"
        className="w-full border rounded-lg p-2 text-sm text-center focus:ring-2 focus:ring-[#7DB6AD]"
        value={value ?? ''}
        onChange={(e) => onChange(e.target.value)}
        aria-label="Grade input"
      />
    </div>
  );
};

// Utility functions
const mapAssessmentType = (field) => {
  if (field.startsWith('TX')) return `ĐĐG TX ${field.replace('TX', '')}`;
  return { GK: 'ĐĐG GK', CK: 'ĐĐG CK' }[field] || '';
};

const getShortAssessmentName = (assessmentType) => {
  if (assessmentType.startsWith('ĐĐG TX')) return `TX${assessmentType.split(' ')[2]}`;
  return { 'ĐĐG GK': 'GK', 'ĐĐG CK': 'CK' }[assessmentType] || assessmentType;
};

const ListMarkTeacher = () => {
  // State
  const [academicYear, setAcademicYear] = useState('');
  const [semester, setSemester] = useState('');
  const [assignments, setAssignments] = useState([]);
  const [selectedAssignment, setSelectedAssignment] = useState('');
  const [grades, setGrades] = useState([]);
  const [isEditing, setIsEditing] = useState(false);
  const [editedGrades, setEditedGrades] = useState({});
  const [editingRows, setEditingRows] = useState({});
  const [loading, setLoading] = useState(false);

  // Token and teacher ID
  const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
  const decoded = useMemo(() => {
    try {
      return token ? jwtDecode(token) : {};
    } catch {
      toast.error('Phiên đăng nhập không hợp lệ.');
      return {};
    }
  }, [token]);
  const teacherId = decoded?.teacherId;

  // Queries
  const { data: academicYears, isLoading: academicYearsLoading } = useAcademicYears();
  const { data: semesters, isLoading: semestersLoading } = useSemestersByAcademicYear(academicYear);

  // Reset states on academic year change
  useEffect(() => {
    if (!academicYear) return;
    setSemester('');
    setAssignments([]);
    setSelectedAssignment('');
    setGrades([]);
  }, [academicYear]);

  // Reset states on semester change
  useEffect(() => {
    if (!semester) {
      setAssignments([]);
      setSelectedAssignment('');
      setGrades([]);
    }
  }, [semester]);

  // Get regular assessment types
  const regularAssessmentTypes = useMemo(() => {
    return Array.from(new Set(grades.map((g) => g.assessmentType)))
      .filter((type) => type.startsWith('ĐĐG TX'))
      .sort((a, b) => parseInt(a.split(' ')[2]) - parseInt(b.split(' ')[2]));
  }, [grades]);

  // Fetch assignments
  const fetchAssignments = useCallback(async (semesterId) => {
    if (!teacherId || !semesterId) return;
    setLoading(true);
    try {
      const response = await axios.get(
        `https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/TeachingAssignment/teacher/${teacherId}/semester/${semesterId}`,
        { headers: { Authorization: `Bearer ${token}` } }
      );
      setAssignments(response.data);
    } catch {
      toast.error('Không thể tải danh sách phân công giảng dạy.');
      setAssignments([]);
    } finally {
      setLoading(false);
    }
  }, [teacherId, token]);

  // Handle semester change
  const handleSemesterChange = useCallback((value) => {
    setSemester(value);
    fetchAssignments(value);
  }, [fetchAssignments]);

  // Validate search
  const validateSearch = useCallback(() => {
    if (!semester) {
      toast.error('Vui lòng chọn học kỳ.');
      return false;
    }
    if (!selectedAssignment) {
      toast.error('Vui lòng chọn lớp và môn học.');
      return false;
    }
    const assignment = assignments.find((a) => a.assignmentId === parseInt(selectedAssignment));
    if (!assignment) {
      toast.error('Phân công không hợp lệ.');
      return false;
    }
    return true;
  }, [assignments, selectedAssignment, semester]);

  // Fetch grades
  const fetchGrades = useCallback(async () => {
    if (!validateSearch()) return;
    setLoading(true);
    try {
      const assignment = assignments.find((a) => a.assignmentId === parseInt(selectedAssignment));
      const { subjectId, classId } = assignment;
      const response = await axios.get(
        `https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Grades/teacher`,
        {
          params: { teacherId, classId, subjectId, semesterId: semester },
          headers: { Authorization: `Bearer ${token}` },
        }
      );
      setGrades(response.data);
      toast.success('Tải danh sách điểm thành công.');
    } catch {
      toast.error('Không thể tải danh sách điểm.');
      setGrades([]);
    } finally {
      setLoading(false);
    }
  }, [assignments, selectedAssignment, semester, teacherId, token, validateSearch]);

  // Handle input change
  const handleInputChange = useCallback((studentId, field, value) => {
    setEditedGrades((prev) => ({
      ...prev,
      [studentId]: { ...prev[studentId], [field]: value },
    }));
  }, []);

  // Save grades
  const saveGrades = useCallback(
    async (gradesToSave, studentId = null) => {
      if (!gradesToSave || !Object.keys(gradesToSave).length) return;
      setLoading(true);
      try {
        const gradesPayload = {
          grades: Object.entries(gradesToSave).flatMap(([sId, fields]) =>
            Object.entries(fields)
              .filter(([_, value]) => value !== '')
              .map(([field, value]) => {
                const gradeInfo = grades.find(
                  (g) => g.studentId === parseInt(sId) && g.assessmentType === mapAssessmentType(field)
                );
                const formattedScore = parseFloat(value).toFixed(2);
                return {
                  gradeID: gradeInfo?.gradeId,
                  score: formattedScore,
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

        await fetchGrades();
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
        toast.success('Lưu điểm thành công.');
      } catch {
        toast.error('Không thể lưu điểm. Vui lòng thử lại.');
      } finally {
        setLoading(false);
      }
    },
    [grades, token, fetchGrades]
  );

  // Handle save all grades
  const handleSaveGrades = useCallback(() => saveGrades(editedGrades), [editedGrades, saveGrades]);

  // Handle save row
  const handleSaveRow = useCallback(
    (studentId) => saveGrades({ [studentId]: editedGrades[studentId] }, studentId),
    [editedGrades, saveGrades]
  );

  // Handle edit row
  const handleEditRow = useCallback(
    async (studentId) => {
      setLoading(true);
      try {
        const studentGrades = grades.filter((g) => g.studentId === studentId);
        const studentEditedGrades = editedGrades[studentId] || {};
        // Validate scores using editedGrades if available, otherwise fall back to grades
        const invalidScores = [];
        studentGrades.forEach((grade) => {
          const assessmentType = grade.assessmentType;
          const shortName = getShortAssessmentName(assessmentType);
          const score = studentEditedGrades[shortName] !== undefined ? String(studentEditedGrades[shortName]) : (grade.score ? String(grade.score) : '0.00');
          const parsedScore = parseFloat(score);

          if (isNaN(parsedScore)) {
            invalidScores.push(`Điểm ${assessmentType} phải là số hợp lệ.`);
          } else if (parsedScore < 1 || parsedScore > 10) {
            invalidScores.push(`Điểm ${assessmentType} phải nằm trong khoảng từ 1 đến 10.`);
          }
        });

        if (invalidScores.length > 0) {
          invalidScores.forEach((error) => toast.error(error));
          return;
        }

        const gradesPayload = {
          grades: studentGrades.map((grade) => {
            const assessmentType = grade.assessmentType;
            const shortName = getShortAssessmentName(assessmentType);
            const score = studentEditedGrades[shortName] !== undefined ? String(studentEditedGrades[shortName]) : (grade.score ? String(grade.score) : '0.00');
            return {
              gradeID: grade.gradeId,
              score: parseFloat(score).toFixed(2),
              teacherComment: 'Học sinh đi học và làm bài đầy đủ',
            };
          }),
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

        await fetchGrades();
        setEditingRows((prev) => ({ ...prev, [studentId]: true }));
        toast.success('Mở chế độ chỉnh sửa thành công.');
      } catch {
        toast.error('Không thể cập nhật điểm. Vui lòng thử lại.');
      } finally {
        setLoading(false);
      }
    },
    [grades, editedGrades, token, fetchGrades]
  );

  // Group grades by student
  const groupedGrades = useMemo(() => {
    return Object.values(
      grades.reduce((acc, { studentId, studentName, assessmentType, score, teacherComment }) => {
        acc[studentId] = acc[studentId] || {
          studentId,
          studentName,
          regularAssessments: {},
          GK: null,
          CK: null,
          teacherComment,
        };
        if (assessmentType.startsWith('ĐĐG TX')) {
          acc[studentId].regularAssessments[getShortAssessmentName(assessmentType)] = score ? parseFloat(score).toFixed(2) : null;
        } else if (assessmentType === 'ĐĐG GK') {
          acc[studentId].GK = score ? parseFloat(score).toFixed(2) : null;
        } else if (assessmentType === 'ĐĐG CK') {
          acc[studentId].CK = score ? parseFloat(score).toFixed(2) : null;
        }
        return acc;
      }, {})
    );
  }, [grades]);

  return (
    <div className="container mx-auto py-6">
      <div className="space-y-4">
        <div className="flex items-center justify-between border-b pb-4">
          <div>
            <h1 className="text-2xl font-bold">Quản lý điểm học sinh</h1>
            <p className="text-muted-foreground text-sm">Quản lý điểm số của học sinh theo học kỳ</p>
          </div>
        </div>

        <div className="grid gap-4 md:grid-cols-3">
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
              {semesters?.map((sem) => (
                <SelectItem key={sem.semesterID} value={sem.semesterID.toString()}>
                  {sem.semesterName}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
          <Select
            value={selectedAssignment}
            onValueChange={setSelectedAssignment}
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

        <div className="flex justify-end">
          <Button
            onClick={fetchGrades}
            className="bg-blue-600 hover:bg-blue-700 text-white"
            disabled={!selectedAssignment || loading}
          >
            {loading ? 'Đang tải...' : 'Tìm kiếm'}
          </Button>
        </div>

        <div className="placeholder  flex justify-between items-center">
          <h2 className="text-lg font-semibold">Danh sách điểm học sinh</h2>
          {grades.length > 0 && (
            <Button
              onClick={() => {
                if (isEditing) handleSaveGrades();
                else setIsEditing(true);
              }}
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
                          <NumberInput
                            value={
                              editedGrades[student.studentId]?.[shortName] ??
                              student.regularAssessments[shortName] ??
                              ''
                            }
                            onChange={(value) => handleInputChange(student.studentId, shortName, value)}
                          />
                        ) : (
                          student.regularAssessments[shortName] ?? 'Chưa có điểm'
                        )}
                      </TableCell>
                    );
                  })}
                  {['GK', 'CK'].map((field) => (
                    <TableCell key={field} className="border text-center">
                      {isEditing || editingRows[student.studentId] ? (
                        <NumberInput
                          value={editedGrades[student.studentId]?.[field] ?? student[field] ?? ''}
                          onChange={(value) => handleInputChange(student.studentId, field, value)}
                        />
                      ) : (
                        student[field] ?? 'Chưa có điểm'
                      )}
                    </TableCell>
                  ))}
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
                <TableCell colSpan={regularAssessmentTypes.length + 6} className="text-center">
                  Không có dữ liệu điểm.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    </div >
  );
};

export default ListMarkTeacher;