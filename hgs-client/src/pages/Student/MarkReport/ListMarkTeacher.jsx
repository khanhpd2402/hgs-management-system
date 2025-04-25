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

  const [isEditing, setIsEditing] = useState(false);
  const [editedGrades, setEditedGrades] = useState({});
  const [editingRows, setEditingRows] = useState({});

  const handleInputChange = (studentId, field, value) => {
    setEditedGrades(prev => ({
      ...prev,
      [studentId]: {
        ...prev[studentId],
        [field]: value
      }
    }));
  };

  const handleSaveGrades = async () => {
    try {
      // Tạo mảng các điểm cần cập nhật
      const updatedGrades = Object.entries(editedGrades).flatMap(([studentId, fields]) => {
        return Object.entries(fields).map(([field, value]) => ({
          studentId: parseInt(studentId),
          score: parseFloat(value),
          assessmentType: field === 'TX1' ? 'ĐĐG TX 1' :
            field === 'TX2' ? 'ĐĐG TX 2' :
              field === 'TX3' ? 'ĐĐG TX 3' :
                field === 'GK' ? 'ĐĐG GK' :
                  field === 'CK' ? 'ĐĐG CK' : '',
          teacherId: parseInt(teacherId),
          classId: parseInt(selectedAssignment.classId),
          subjectId: parseInt(selectedAssignment.subjectId),
          semesterId: semester === '1' ? 1 : 2
        }));
      });

      await axios.post(
        'https://localhost:8386/api/Grades/update-multiple',
        updatedGrades,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      // Sau khi lưu thành công, cập nhật lại dữ liệu
      handleSearchGrades();
      setIsEditing(false);
      setEditedGrades({});
    } catch (error) {
      console.error('Error saving grades:', error);
    }
  };

  const handleEditRow = async (studentId) => {
    try {
      // Lấy thông tin điểm của học sinh từ grades
      const studentGrades = grades.filter(g => g.studentId === studentId);

      // Tạo payload theo cấu trúc API mới
      const gradesPayload = {
        grades: studentGrades.map(grade => ({
          gradeID: grade.gradeId,
          score: grade.score ? grade.score.toString() : "0", // Add null check and default value
          teacherComment: "nhập điểm"
        }))
      };
      console.log("gradesPayload", gradesPayload)
      console.log("grade", grades)

      await axios.post(
        'https://localhost:8386/api/Grades/update-multiple-scores',
        gradesPayload,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      // Cập nhật trạng thái sau khi lưu thành công
      handleSearchGrades();
      setEditingRows(prev => ({
        ...prev,
        [studentId]: true
      }));
    } catch (error) {
      console.error('Error updating grades:', error);
    }
  };

  const handleSaveRow = async (studentId) => {
    try {
      const studentGrades = editedGrades[studentId];
      if (!studentGrades) return;

      const gradesPayload = {
        grades: Object.entries(studentGrades).map(([field, value]) => ({
          gradeID: grades.find(g =>
            g.studentId === parseInt(studentId) &&
            g.assessmentType === (
              field === 'TX1' ? 'ĐĐG TX 1' :
                field === 'TX2' ? 'ĐĐG TX 2' :
                  field === 'TX3' ? 'ĐĐG TX 3' :
                    field === 'GK' ? 'ĐĐG GK' :
                      field === 'CK' ? 'ĐĐG CK' : ''
            )
          )?.gradeId,
          score: value.toString(),
          teacherComment: "nhập điểm"
        }))
      };

      await axios.post(
        'https://localhost:8386/api/Grades/update-multiple-scores',
        gradesPayload,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      handleSearchGrades();
      setEditingRows(prev => ({
        ...prev,
        [studentId]: false
      }));
      setEditedGrades(prev => {
        const newGrades = { ...prev };
        delete newGrades[studentId];
        return newGrades;
      });
    } catch (error) {
      console.error('Error saving row:', error);
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

        <div className="flex justify-between items-center mb-4">
          <h3 className="text-lg font-semibold">Danh sách điểm học sinh</h3>
          {grades.length > 0 && (
            <div>
              {isEditing ? (
                <button
                  onClick={handleSaveGrades}
                  className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded mr-2"
                >
                  Lưu điểm
                </button>
              ) : (
                <button
                  onClick={() => setIsEditing(true)}
                  className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded"
                >
                  Nhập điểm
                </button>
              )}
            </div>
          )}
        </div>

        <table className="w-full border-collapse">
          <thead>
            <tr>
              <th className="border p-2" rowSpan="2">Stt</th>
              <th className="border p-2" rowSpan="2">Tên học sinh</th>

              <th className="border p-2" colSpan="3">Điểm thường xuyên</th>
              <th className="border p-2" rowSpan="2">Điểm giữa kỳ</th>
              <th className="border p-2" rowSpan="2">Điểm cuối kỳ</th>
              <th className="border p-2" rowSpan="2">Nhận xét của giáo viên</th>
              <th className="border p-2" rowSpan="2">Hành động</th>
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
                  <td className="border p-2">
                    {isEditing ? (
                      <input
                        type="number"
                        step="0.1"
                        min="0"
                        max="10"
                        value={editedGrades[student.studentId]?.TX1 ?? student.TX1 ?? ''}
                        onChange={(e) => handleInputChange(student.studentId, 'TX1', e.target.value)}
                        className="w-20 p-1 border rounded"
                      />
                    ) : (
                      student.TX1 !== null ? student.TX1 : 'Chưa có điểm'
                    )}
                  </td>
                  <td className="border p-2">
                    {isEditing ? (
                      <input
                        type="number"
                        step="0.1"
                        min="0"
                        max="10"
                        value={editedGrades[student.studentId]?.TX2 ?? student.TX2 ?? ''}
                        onChange={(e) => handleInputChange(student.studentId, 'TX2', e.target.value)}
                        className="w-20 p-1 border rounded"
                      />
                    ) : (
                      student.TX2 !== null ? student.TX2 : 'Chưa có điểm'
                    )}
                  </td>
                  <td className="border p-2">
                    {isEditing ? (
                      <input
                        type="number"
                        step="0.1"
                        min="0"
                        max="10"
                        value={editedGrades[student.studentId]?.TX3 ?? student.TX3 ?? ''}
                        onChange={(e) => handleInputChange(student.studentId, 'TX3', e.target.value)}
                        className="w-20 p-1 border rounded"
                      />
                    ) : (
                      student.TX3 !== null ? student.TX3 : 'Chưa có điểm'
                    )}
                  </td>
                  <td className="border p-2">
                    {isEditing ? (
                      <input
                        type="number"
                        step="0.1"
                        min="0"
                        max="10"
                        value={editedGrades[student.studentId]?.GK ?? student.GK ?? ''}
                        onChange={(e) => handleInputChange(student.studentId, 'GK', e.target.value)}
                        className="w-20 p-1 border rounded"
                      />
                    ) : (
                      student.GK !== null ? student.GK : 'Chưa có điểm'
                    )}
                  </td>
                  <td className="border p-2">
                    {isEditing ? (
                      <input
                        type="number"
                        step="0.1"
                        min="0"
                        max="10"
                        value={editedGrades[student.studentId]?.CK ?? student.CK ?? ''}
                        onChange={(e) => handleInputChange(student.studentId, 'CK', e.target.value)}
                        className="w-20 p-1 border rounded"
                      />
                    ) : (
                      student.CK !== null ? student.CK : 'Chưa có điểm'
                    )}
                  </td>
                  <td className="border p-2">{student.teacherComment || 'Chưa có nhận xét'}</td>
                  <td className="border p-2">
                    {editingRows[student.studentId] ? (
                      <button
                        onClick={() => handleSaveRow(student.studentId)}
                        className="bg-green-600 hover:bg-green-700 text-white px-3 py-1 rounded text-sm"
                      >
                        Lưu
                      </button>
                    ) : (
                      <button
                        onClick={() => handleEditRow(student.studentId)}
                        className="bg-blue-600 hover:bg-blue-700 text-white px-3 py-1 rounded text-sm"
                      >
                        Cập nhật
                      </button>
                    )}
                  </td>
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
