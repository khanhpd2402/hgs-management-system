import React, { useState } from 'react';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';
import './ListMarkTeacher.scss';


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
    <div>
      <div className="mark-report-container">
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

        <h3 className="text-lg font-semibold mb-2">Danh sách điểm học sinh</h3>
        <table className="w-full border-collapse">
          <thead>
            <tr>
              <th className="border p-2" rowSpan="2">Stt</th>
              <th className="border p-2" rowSpan="2">Tên học sinh</th>

              <th className="border p-2" colSpan="3">Điểm thường xuyên</th>
              <th className="border p-2" rowSpan="2">Điểm giữa kỳ</th>
              <th className="border p-2" rowSpan="2">Điểm cuối kỳ</th>
              <th className="border p-2" rowSpan="2">Nhận xét của giáo viên</th>
            </tr>
            <tr>
              <th className="border p-2">TX1</th>
              <th className="border p-2">TX2</th>
              <th className="border p-2">TX3</th>
            </tr>
          </thead>
          <tbody>
            {grades.length > 0 ? (
              // Group grades by studentId
              Object.values(grades.reduce((acc, grade) => {
                if (!acc[grade.studentId]) {
                  acc[grade.studentId] = {

                    studentId: grade.studentId,
                    studentName: grade.studentName,
                    TX1: null,
                    TX2: null,
                    TX3: null,
                    GK: null,
                    CK: null,
                    teacherComment: grade.teacherComment
                  };
                }

                // Map assessment types to corresponding fields
                switch (grade.assessmentType) {
                  case 'ĐĐG TX 1':
                    acc[grade.studentId].TX1 = grade.score;
                    break;
                  case 'ĐĐG TX 2':
                    acc[grade.studentId].TX2 = grade.score;
                    break;
                  case 'ĐĐG TX 3':
                    acc[grade.studentId].TX3 = grade.score;
                    break;
                  case 'ĐĐG GK':
                    acc[grade.studentId].GK = grade.score;
                    break;
                  case 'ĐĐG CK':
                    acc[grade.studentId].CK = grade.score;
                    break;
                }
                return acc;
              }, {})).map((student, index) => (
                <tr key={student.studentId}>
                  <td className="border p-2">{index + 1}</td>
                  <td className="border p-2">{student.studentName}</td>
                  <td className="border p-2">{student.TX1 !== null ? student.TX1 : 'Chưa có điểm'}</td>
                  <td className="border p-2">{student.TX2 !== null ? student.TX2 : 'Chưa có điểm'}</td>
                  <td className="border p-2">{student.TX3 !== null ? student.TX3 : 'Chưa có điểm'}</td>
                  <td className="border p-2">{student.GK !== null ? student.GK : 'Chưa có điểm'}</td>
                  <td className="border p-2">{student.CK !== null ? student.CK : 'Chưa có điểm'}</td>
                  <td className="border p-2">{student.teacherComment || 'Chưa có nhận xét'}</td>
                </tr>
              ))
            ) : (
              <tr>
                <td colSpan="7" className="border p-2 text-center">
                  Không có dữ liệu điểm.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}

export default ListMarkTeacher
