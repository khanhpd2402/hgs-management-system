import React, { useState } from 'react';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';

const ListMarkTeacher = () => {
  const [semester, setSemester] = useState('');
  const [assignments, setAssignments] = useState([]);
  const [selectedAssignment, setSelectedAssignment] = useState('');
  const [grades, setGrades] = useState([]);

  const token = localStorage.getItem('token')?.replace(/^"|"$/g, '');
  const decoded = token ? jwtDecode(token) : {};
  const teacherId = decoded?.teacherId;

  const handleSemesterChange = async (e) => {
    const selectedSemester = e.target.value;
    setSemester(selectedSemester);

    if (teacherId && selectedSemester) {
      const semesterId = selectedSemester === '1' ? 1 : 2;
      try {
        const response = await axios.get(
          `https://localhost:8386/api/TeachingAssignment/teacher/${teacherId}/semester/${semesterId}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        setAssignments(response.data);
        setSelectedAssignment('');
        setGrades([]); // reset lại điểm khi đổi học kỳ
      } catch (error) {
        console.error('Error fetching assignments:', error);
        setAssignments([]);
      }
    }
  };

  const handleAssignmentChange = (e) => {
    setSelectedAssignment(e.target.value);
  };

  const handleSearchGrades = async () => {
    const assignment = assignments.find(a => a.assignmentId === parseInt(selectedAssignment));
    if (assignment && semester) {
      const { subjectId, classId } = assignment;
      const semesterId = semester === '1' ? 1 : 2;

      try {
        const response = await axios.get(
          `https://localhost:8386/api/Grades/teacher`,
          {
            params: {
              teacherId: parseInt(teacherId),
              classId: parseInt(classId),
              subjectId: parseInt(subjectId),
              semesterId: parseInt(semesterId)
            },
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        setGrades(response.data);
      } catch (error) {
        console.error('Error fetching grades:', error);
        setGrades([]);
      }
    }
  };

  return (
    <div className="p-4 max-w-xl mx-auto">
      <h2 className="text-xl font-bold mb-4">Chọn học kỳ</h2>
      <select
        value={semester}
        onChange={handleSemesterChange}
        className="w-full p-2 border rounded mb-4"
      >
        <option value="">-- Chọn học kỳ --</option>
        <option value="1">Học kỳ 1</option>
        <option value="2">Học kỳ 2</option>
      </select>

      <h3 className="text-lg font-semibold mb-2">Chọn lớp và môn học</h3>
      <select
        value={selectedAssignment}
        onChange={handleAssignmentChange}
        className="w-full p-2 border rounded mb-4"
      >
        <option value="">-- Chọn lớp và môn học --</option>
        {assignments.map((a) => (
          <option key={a.assignmentId} value={a.assignmentId}>
            {a.subjectName}({a.subjectId}) - {a.className}({a.classId})
          </option>
        ))}
      </select>

      <button
        onClick={handleSearchGrades}
        className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded mb-6"
        disabled={!selectedAssignment}
      >
        Tìm kiếm
      </button>

      <h3 className="text-lg font-semibold mb-2">Danh sách điểm sinh viên</h3>
      <table className="w-full border-collapse">
        <thead>
          <tr>
            <th className="border p-2">Tên sinh viên</th>
            <th className="border p-2">Loại đánh giá</th>
            <th className="border p-2">Điểm</th>
            <th className="border p-2">Nhận xét của giáo viên</th>
          </tr>
        </thead>
        <tbody>
          {grades.length > 0 ? (
            grades.map((grade, index) => (
              <tr key={`${grade.studentId}-${grade.assessmentType}`}>
                <td className="border p-2">{grade.studentName}</td>
                <td className="border p-2">{grade.assessmentType}</td>
                <td className="border p-2">{grade.score !== null ? grade.score : 'Chưa có điểm'}</td>
                <td className="border p-2">{grade.teacherComment || 'Chưa có nhận xét'}</td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="4" className="border p-2 text-center">
                Không có dữ liệu điểm.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default ListMarkTeacher;
