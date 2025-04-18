import React from 'react';
import * as XLSX from 'xlsx';
import { FileDown } from 'lucide-react';

const ExportSchedule = ({ schedule, showTeacherName }) => {
  const handleExport = () => {
    if (!schedule?.details) return;

    // Prepare data for export
    const exportData = [];

    // Add header information
    exportData.push(['THỜI KHÓA BIỂU']);
    exportData.push([`Học kỳ ${schedule.semesterId === 1 ? 'I' : 'II'}`]);
    exportData.push([`Thời gian áp dụng: Từ ${new Date(schedule.effectiveDate).toLocaleDateString('vi-VN')} đến ${new Date(schedule.endDate).toLocaleDateString('vi-VN')}`]);
    exportData.push([]); // Empty row for spacing

    // Get unique classes from filtered data
    const classes = [...new Set(schedule.details.map(item => item.className))].sort();

    // Define days and shifts
    const days = ['Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy', 'Chủ Nhật'];
    const shifts = [
      { name: 'Sáng', periods: [1, 2, 3, 4, 5] },
      { name: 'Chiều', periods: [6, 7, 8] }
    ];

    // Add table headers
    const headers = ['Thứ', 'Buổi', 'Tiết', ...classes];
    exportData.push(headers);

    // Add schedule data
    days.forEach(day => {
      shifts.forEach(shift => {
        shift.periods.forEach((period, periodIndex) => {
          const row = [];

          // Add day (only for first period of shift)
          row.push(periodIndex === 0 ? day : '');

          // Add shift name (only for first period)
          row.push(periodIndex === 0 ? shift.name : '');

          // Add period
          row.push(`Tiết ${period}`);

          // Add schedule for each class
          classes.forEach(className => {
            const scheduleItem = schedule.details.find(
              item =>
                item.dayOfWeek === day &&
                item.periodId === period &&
                item.className === className
            );

            if (scheduleItem) {
              const cellContent = showTeacherName
                ? `${scheduleItem.subjectName}\n${scheduleItem.teacherName}`
                : scheduleItem.subjectName;
              row.push(cellContent);
            } else {
              row.push('');
            }
          });

          exportData.push(row);
        });
      });
    });

    // Create workbook
    const wb = XLSX.utils.book_new();
    const ws = XLSX.utils.aoa_to_sheet(exportData);

    // Style the worksheet
    const colWidths = [
      { wch: 12 }, // Thứ
      { wch: 8 },  // Buổi
      { wch: 8 },  // Tiết
      ...classes.map(() => ({ wch: 25 })) // Class columns
    ];
    ws['!cols'] = colWidths;

    // Merge cells for header
    ws['!merges'] = [
      { s: { r: 0, c: 0 }, e: { r: 0, c: classes.length + 2 } }, // Title
      { s: { r: 1, c: 0 }, e: { r: 1, c: classes.length + 2 } }, // Semester
      { s: { r: 2, c: 0 }, e: { r: 2, c: classes.length + 2 } }, // Date range
    ];

    // Add the worksheet to workbook
    XLSX.utils.book_append_sheet(wb, ws, 'Thời khóa biểu');

    // Generate filename with current date
    const today = new Date();
    const filename = `ThoiKhoaBieu_${today.getDate()}_${today.getMonth() + 1}_${today.getFullYear()}.xlsx`;

    // Save file
    XLSX.writeFile(wb, filename);
  };

  return (
    <button
      className="btn-export"
      onClick={handleExport}
      disabled={!schedule?.details}
    >
      <FileDown size={16} /> Xuất Excel
    </button>
  );
};

export default ExportSchedule;
