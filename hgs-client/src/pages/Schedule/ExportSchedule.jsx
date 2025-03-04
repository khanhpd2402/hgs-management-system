import React from 'react';
import * as XLSX from 'xlsx';
import { scheduleData, teacherData, subjectData } from './data'; // Đảm bảo đường dẫn này đúng

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

    let data = [["Thứ", "Buổi", "Tiết", ...grades.flatMap(grade => Object.keys(scheduleData[grade]))]];

    days.forEach((day, dayIndex) => {
      sessions.forEach((session) => {
        const maxPeriods = Math.max(
          ...grades.flatMap(grade =>
            Object.keys(scheduleData[grade]).map(className =>
              scheduleData[grade][className]?.[day]?.[session]?.length || 0
            )
          )
        );

        for (let periodIndex = 0; periodIndex < maxPeriods; periodIndex++) {
          let row = [];
          if (periodIndex === 0) {
            if (session === sessions[0]) {
              row.push(day);
            } else {
              row.push("");
            }
            row.push(session);
          } else {
            row.push("", "");
          }
          row.push(`Tiết ${periodIndex + 1}`);

          grades.forEach(grade => {
            Object.keys(scheduleData[grade]).forEach(className => {
              const period = scheduleData[grade][className]?.[day]?.[session]?.[periodIndex];
              row.push(period ? `${getSubjectName(period.subject_Id)} - ${getTeacherName(period.teacher_id)}` : "");
            });
          });

          data.push(row);
        }
      });
    });

    const ws = XLSX.utils.aoa_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Schedule");
    XLSX.writeFile(wb, "schedule.xlsx");
  };

  return (
    <button onClick={exportToExcel}>Export to Excel</button>
  );
};

export default ExportSchedule;