import React from "react";
import * as XLSX from "xlsx";
import { saveAs } from "file-saver";
import "./ExportSchedule.scss";

const ExportSchedule = ({ selectedGrade, selectedClass, filteredClasses, scheduleData, days, sessions, getSubjectName, getTeacherName }) => {

  const exportToExcel = () => {
    const worksheetData = [];

    // Tiêu đề chính
    worksheetData.push(["Thời Khóa Biểu"]);
    worksheetData.push([]); // Dòng trống

    // Hàng tiêu đề
    const headerRow = ["Thứ", "Buổi", "Tiết"];
    filteredClasses.forEach(({ grade, className }) => {
      headerRow.push(`Khối ${grade} - ${className}`);
    });
    worksheetData.push(headerRow);

    // Dữ liệu của bảng
    days.forEach((day) => {
      sessions.forEach((session) => {
        const maxPeriods = Math.max(
          ...filteredClasses.map(({ grade, className }) =>
            scheduleData[grade]?.[className]?.[day]?.[session]?.length || 0
          )
        );

        for (let periodIndex = 0; periodIndex < maxPeriods; periodIndex++) {
          const row = [];

          if (periodIndex === 0) {
            row.push(day);
          } else {
            row.push("");
          }

          if (periodIndex === 0) {
            row.push(session);
          } else {
            row.push("");
          }

          row.push(`Tiết ${periodIndex + 1}`);

          filteredClasses.forEach(({ grade, className }) => {
            const period = scheduleData[grade]?.[className]?.[day]?.[session]?.[periodIndex];
            row.push(period ? `${getSubjectName(period.subject_Id)} - ${getTeacherName(period.teacher_id)}` : "");
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
      { width: 10 }, // "Thứ" giữ nguyên
      { width: 10 }, // "Buổi" giữ nguyên
      { width: 10 }, // "Tiết" giữ nguyên
      ...filteredClasses.map(() => ({ width: 20 })) // Các lớp có độ rộng gấp đôi
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
