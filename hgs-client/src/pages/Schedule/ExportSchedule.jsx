import React from "react";
import * as XLSX from "xlsx";
import { saveAs } from "file-saver";
import "./ExportSchedule.scss";

const dayMap = {
  "Monday": "Thứ 2",
  "Tuesday": "Thứ 3",
  "Wednesday": "Thứ 4",
  "Thursday": "Thứ 5",
  "Friday": "Thứ 6",
  "Saturday": "Thứ 7",
  "Sunday": "Chủ Nhật"
};

const sessionMap = {
  "Morning": "Sáng",
  "Afternoon": "Chiều"
};

const ExportSchedule = ({
  selectedGrade,
  selectedClass,
  filteredClasses,
  scheduleData,
  days,
  sessions,
  getSubjectName,
  getTeacherName,
  showTeacherName
}) => {
  const exportToExcel = () => {
    const worksheetData = [];

    // Tiêu đề chính
    worksheetData.push(["Trường THCS Hải Giang"]);
    worksheetData.push([]);

    // Hàng tiêu đề
    const headerRow = ["Thứ", "Buổi", "Tiết"];
    filteredClasses.forEach(({ grade, className }) => {
      headerRow.push(`Khối ${grade} - ${className}`);
    });
    worksheetData.push(headerRow);

    // Dữ liệu của bảng
    days.forEach((day) => {
      let firstRowOfDay = true;
      sessions.forEach((session) => {
        const maxPeriods = Math.max(
          ...filteredClasses.map(({ grade, className }) =>
            scheduleData[grade]?.[className]?.[day]?.[session]?.length || 0
          )
        );

        let firstRowOfSession = true;
        for (let periodIndex = 0; periodIndex < maxPeriods; periodIndex++) {
          const row = [];

          if (firstRowOfDay) {
            row.push(dayMap[day]);
            firstRowOfDay = false;
          } else {
            row.push("");
          }

          if (firstRowOfSession) {
            row.push(sessionMap[session]);
            firstRowOfSession = false;
          } else {
            row.push("");
          }

          row.push(`Tiết ${periodIndex + 1}`);

          filteredClasses.forEach(({ grade, className }) => {
            const period = scheduleData[grade]?.[className]?.[day]?.[session]?.[periodIndex];
            if (period) {
              const subjectName = getSubjectName(period.subject_Id);
              const teacherName = showTeacherName ? ` - ${getTeacherName(period.teacher_id)}` : "";
              row.push(`${subjectName}${teacherName}`);
            } else {
              row.push("");
            }
          });

          worksheetData.push(row);
        }
      });
    });

    // Tạo file Excel
    const worksheet = XLSX.utils.aoa_to_sheet(worksheetData);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Thoi_Khoa_Bieu");

    // Thiết lập độ rộng cho từng cột
    worksheet["!cols"] = [
      { width: 10 },
      { width: 10 },
      { width: 10 },
      ...filteredClasses.map(() => ({ width: 25 }))
    ];

    // Xuất file
    const excelBuffer = XLSX.write(workbook, { bookType: "xlsx", type: "array" });
    const data = new Blob([excelBuffer], { type: "application/octet-stream" });
    saveAs(data, "Thoi_Khoa_Bieu.xlsx");
  };

  return (
    <button onClick={exportToExcel} className="export-button">
      Xuất Excel
    </button>
  );
};

export default ExportSchedule;