import React from "react";
import * as XLSX from "xlsx";
import { saveAs } from "file-saver";
import { scheduleData, teacherData, subjectData } from "./data";

const getTeacherName = (teacher_id) => {
  const teacher = teacherData.find((t) => t.teacher_id === parseInt(teacher_id));
  return teacher ? teacher.teacher_name : "Unknown";
};

const getSubjectName = (subject_Id) => {
  const subject = subjectData.find((s) => s.subject_Id === parseInt(subject_Id));
  return subject ? subject.subject_name : "Unknown";
};

const ExportSchedule = () => {
  const exportToExcel = () => {
    const days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    const sessions = ["Morning", "Afternoon"];
    const grades = Object.keys(scheduleData);

    let worksheetData = [["Thứ", "Buổi", "Tiết", "Khối", "Lớp", "Môn học", "Giáo viên"]];

    grades.forEach((grade) => {
      Object.keys(scheduleData[grade]).forEach((className) => {
        days.forEach((day) => {
          sessions.forEach((session) => {
            (scheduleData[grade][className]?.[day]?.[session] || []).forEach((period, index) => {
              worksheetData.push([
                day,
                session,
                `Tiết ${index + 1}`,
                `Khối ${grade}`,
                className,
                getSubjectName(period.subject_Id),
                getTeacherName(period.teacher_id)
              ]);
            });
          });
        });
      });
    });

    const worksheet = XLSX.utils.aoa_to_sheet(worksheetData);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Schedule");

    const excelBuffer = XLSX.write(workbook, { bookType: "xlsx", type: "array" });
    const data = new Blob([excelBuffer], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

    saveAs(data, "ThoiKhoaBieu.xlsx");
  };

  return (
    <button onClick={exportToExcel} style={{ padding: "10px 15px", backgroundColor: "#4CAF50", color: "white", border: "none", borderRadius: "5px", cursor: "pointer" }}>
      Xuất Excel
    </button>
  );
};

export default ExportSchedule;
