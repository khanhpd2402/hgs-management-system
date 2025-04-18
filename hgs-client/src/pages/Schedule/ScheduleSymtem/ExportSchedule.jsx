import React from 'react';
import { FileDown } from 'lucide-react';
import * as XLSX from 'xlsx';

const ExportSchedule = ({ schedule, showTeacherName }) => {
  const handleExport = () => {
    try {
      if (!schedule?.details) return;

      // Define headerRows constant at the beginning
      const headerRows = 8;

      // Create the worksheet data array
      const wsData = [];

      // Add header information
      wsData.push(['Trường THCS Hải Giang']);
      wsData.push([`TKB SỐ ${schedule.timetableId || ''}`]);
      wsData.push(['THỜI KHÓA BIỂU']);
      wsData.push([schedule?.selectedSession === 'Afternoon' ? 'BUỔI CHIỀU' : 'BUỔI SÁNG']);
      wsData.push([`NĂM HỌC ${new Date(schedule.effectiveDate).getFullYear()}-${new Date(schedule.endDate).getFullYear()}`]);
      wsData.push([`Thực hiện từ ngày ${new Date(schedule.effectiveDate).toLocaleDateString('vi-VN')} đến ${new Date(schedule.endDate).toLocaleDateString('vi-VN')}`]);
      wsData.push(['HỌC KỲ ' + (schedule.semesterId === 1 ? 'I' : 'II')]);
      wsData.push([]); // Empty row for spacing

      // Get unique classes and sort them
      const classes = [...new Set(schedule.details.map(item => item.className))].sort();

      // Create grade headers
      const gradeHeaders = [];
      const classHeaders = [];

      // Group classes by grade
      const classesByGrade = classes.reduce((acc, className) => {
        const grade = className.charAt(0);
        if (!acc[grade]) acc[grade] = [];
        acc[grade].push(className);
        return acc;
      }, {});

      // Create headers
      Object.entries(classesByGrade).forEach(([grade, gradeClasses]) => {
        gradeHeaders.push(`Khối ${grade}`);
        gradeHeaders.push(...Array(gradeClasses.length - 1).fill(''));
        classHeaders.push(...gradeClasses);
      });

      // Add headers to worksheet
      wsData.push(['Lịch học', '', '', ...gradeHeaders]);
      wsData.push(['THỨ', 'BUỔI', 'TIẾT', ...classHeaders]);

      // Define days and shifts
      const daysOfWeek = ['Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'];
      const shifts = schedule?.selectedSession === 'Afternoon'
        ? [{ name: 'Chiều', periods: [6, 7, 8] }]
        : schedule?.selectedSession === 'Morning'
          ? [{ name: 'Sáng', periods: [1, 2, 3, 4, 5] }]
          : [
            { name: 'Sáng', periods: [1, 2, 3, 4, 5] },
            { name: 'Chiều', periods: [6, 7, 8] }
          ];

      // Create schedule rows
      let rowIndex = headerRows;
      daysOfWeek.forEach((day, dayIndex) => {
        shifts.forEach((shift, shiftIndex) => {
          const firstShiftRow = rowIndex;
          shift.periods.forEach((period, periodIndex) => {
            const row = [
              periodIndex === 0 && shiftIndex === 0 ? day : '', // Day column
              periodIndex === 0 ? shift.name : '',              // Shift column
              period                                            // Period column
            ];

            // Add data for each class
            classes.forEach(className => {
              const scheduleItem = schedule.details.find(
                item =>
                  item.dayOfWeek === day &&
                  item.periodId === period &&
                  item.className === className
              );

              if (scheduleItem) {
                row.push(showTeacherName
                  ? `${scheduleItem.subjectName}\n${scheduleItem.teacherName}`
                  : scheduleItem.subjectName);
              } else {
                row.push('');
              }
            });

            wsData.push(row);
            rowIndex++;
          });
        });
      });

      // Create worksheet first
      const ws = XLSX.utils.aoa_to_sheet(wsData);

      // Set column widths
      ws['!cols'] = [
        { wch: 10 },  // THỨ
        { wch: 10 },  // BUỔI
        { wch: 8 },   // TIẾT
        ...classes.map(() => ({ wch: 18 })) // Class columns
      ];

      // Set row heights
      ws['!rows'] = wsData.map((_, index) => ({
        hpt: index < headerRows ? 30 : 45
      }));

      // Initialize merges array
      ws['!merges'] = [];

      // Add header merges
      const totalColumns = 3 + classes.length;
      ws['!merges'].push(
        { s: { r: 0, c: 0 }, e: { r: 0, c: totalColumns - 1 } }, // School name
        { s: { r: 1, c: 0 }, e: { r: 1, c: totalColumns - 1 } }, // TKB number
        { s: { r: 2, c: 0 }, e: { r: 2, c: totalColumns - 1 } }, // Title
        { s: { r: 3, c: 0 }, e: { r: 3, c: totalColumns - 1 } }, // Session
        { s: { r: 4, c: 0 }, e: { r: 4, c: totalColumns - 1 } }, // School year
        { s: { r: 5, c: 0 }, e: { r: 5, c: totalColumns - 1 } }, // Date
        { s: { r: 6, c: 0 }, e: { r: 6, c: totalColumns - 1 } }, // Semester
        { s: { r: 7, c: 0 }, e: { r: 7, c: 2 } } // "Lịch học" merge
      );

      // Add grade header merges
      let currentCol = 3;
      Object.values(classesByGrade).forEach(gradeClasses => {
        ws['!merges'].push({
          s: { r: 7, c: currentCol },
          e: { r: 7, c: currentCol + gradeClasses.length - 1 }
        });
        currentCol += gradeClasses.length;
      });

      // Add day and shift merges
      daysOfWeek.forEach((_, dayIndex) => {
        const dayStartRow = headerRows + (dayIndex * shifts.reduce((acc, shift) => acc + shift.periods.length, 0));
        const dayEndRow = dayStartRow + shifts.reduce((acc, shift) => acc + shift.periods.length, 0) - 1;

        // Merge day cells
        ws['!merges'].push({
          s: { r: dayStartRow, c: 0 },
          e: { r: dayEndRow, c: 0 }
        });

        // Merge shift cells
        let currentRow = dayStartRow;
        shifts.forEach(shift => {
          ws['!merges'].push({
            s: { r: currentRow, c: 1 },
            e: { r: currentRow + shift.periods.length - 1, c: 1 }
          });
          currentRow += shift.periods.length;
        });
      });

      // Set cell styles
      const range = XLSX.utils.decode_range(ws['!ref']);
      for (let r = 0; r <= range.e.r; r++) {
        for (let c = 0; c <= range.e.c; c++) {
          const cellRef = XLSX.utils.encode_cell({ r, c });
          if (!ws[cellRef]) ws[cellRef] = { v: '' };
          ws[cellRef].s = {
            alignment: {
              vertical: 'center',
              horizontal: 'center',
              wrapText: true
            },
            font: {
              name: 'Arial',
              sz: 11
            },
            border: {
              top: { style: 'thin' },
              bottom: { style: 'thin' },
              left: { style: 'thin' },
              right: { style: 'thin' }
            }
          };
        }
      }

      // Create workbook and export
      const wb = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(wb, ws, 'Thời khóa biểu');
      XLSX.writeFile(wb, `ThoiKhoaBieu_${new Date().toISOString().slice(0, 10)}.xlsx`);
    } catch (error) {
      console.error('Export error:', error);
      alert('Có lỗi xảy ra khi xuất file. Vui lòng thử lại.');
    }
  };

  return (
    <button
      className="export-button"
      onClick={handleExport}
      disabled={!schedule?.details}
    >
      <FileDown size={16} /> Xuất Excel
    </button>
  );
};

export default ExportSchedule;
