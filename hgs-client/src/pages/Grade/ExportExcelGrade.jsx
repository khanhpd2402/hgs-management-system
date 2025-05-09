import React from 'react';
import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';
import { Button } from "@/components/ui/button";

const ExportExcelGrade = ({ grades, selectedSubject, regularColumns, semesterId, classId, className }) => {
    const handleExport = async () => {
        const workbook = new ExcelJS.Workbook();
        const worksheet = workbook.addWorksheet('Bảng điểm');

        // Thiết lập tiêu đề chung
        worksheet.mergeCells('A1:K1');
        worksheet.getCell('A1').value = 'Trường THCS Hải Giang';
        worksheet.getCell('A1').alignment = { horizontal: 'center' };
        worksheet.getCell('A1').font = { bold: true, size: 14 };

        worksheet.mergeCells('A2:K2');
        worksheet.getCell('A2').value = `BẢNG KẾT QUẢ ĐÁNH GIÁ MÔN ${selectedSubject?.subjectName?.toUpperCase()} - LỚP ${className}`;
        worksheet.getCell('A2').alignment = { horizontal: 'center' };
        worksheet.getCell('A2').font = { bold: true, size: 13 };

        worksheet.mergeCells('A3:K3');
        worksheet.getCell('A3').value = `Học kỳ ${semesterId === '7' ? 'I' : 'II'} - Năm học 2024-2025`;
        worksheet.getCell('A3').alignment = { horizontal: 'center' };
        worksheet.getCell('A3').font = { bold: true };

        if (selectedSubject?.typeOfGrade === 'Nhận xét') {
            // Cấu trúc bảng cho môn học nhận xét
            const headers = [
                { header: 'STT', key: 'stt', width: 5 },
                { header: 'Mã học sinh', key: 'studentId', width: 12 },
                { header: 'Họ và tên', key: 'studentName', width: 25 },
                { header: 'Đánh giá', key: 'assessment', width: 40 }
            ];

            worksheet.columns = headers;
            worksheet.addRow(headers.map(h => h.header));

            // Thêm dữ liệu cho môn học nhận xét
            grades.forEach((student, index) => {
                worksheet.addRow({
                    stt: index + 1,
                    studentId: student.studentId,
                    studentName: student.studentName,
                    assessment: student.regularAssessments['TX1'] || '-'
                });
            });
        } else {
            // Cấu trúc bảng cho môn học có điểm số
            const mainHeaders = [
                { header: 'STT', key: 'stt', width: 5 },
                { header: 'Mã học sinh', key: 'studentId', width: 12 },
                { header: 'Họ và tên', key: 'studentName', width: 25 }
            ];

            // Thêm cột điểm thường xuyên
            regularColumns.forEach(col => {
                mainHeaders.push({ header: col, key: col, width: 8 });
            });

            // Thêm cột điểm giữa kỳ, cuối kỳ và tổng kết
            mainHeaders.push(
                { header: 'ĐĐG GK', key: 'GK', width: 8 },
                { header: 'ĐĐG CK', key: 'CK', width: 8 },
                { header: 'Điểm TK', key: 'finalGrade', width: 8 }
            );

            worksheet.columns = mainHeaders;
            worksheet.addRow(mainHeaders.map(h => h.header));

            // Thêm dữ liệu cho môn học có điểm số
            grades.forEach((student, index) => {
                const rowData = {
                    stt: index + 1,
                    studentId: student.studentId,
                    studentName: student.studentName
                };

                // Thêm điểm thường xuyên
                regularColumns.forEach(col => {
                    rowData[col] = student.regularAssessments[col] || '-';
                });

                // Thêm điểm giữa kỳ, cuối kỳ và tổng kết
                rowData.GK = student.GK || '-';
                rowData.CK = student.CK || '-';
                rowData.finalGrade = calculateFinalGrade(student);

                worksheet.addRow(rowData);
            });
        }

        // Style cho header
        const headerRow = worksheet.getRow(4);
        headerRow.font = { bold: true };
        headerRow.fill = {
            type: 'pattern',
            pattern: 'solid',
            fgColor: { argb: 'FFFFFF00' }
        };
        headerRow.alignment = { vertical: 'middle', horizontal: 'center' };

        // Style cho tất cả các ô dữ liệu
        worksheet.eachRow((row, rowNumber) => {
            if (rowNumber >= 4) { // Bỏ qua các hàng tiêu đề chung
                row.eachCell((cell) => {
                    cell.border = {
                        top: { style: 'thin' },
                        left: { style: 'thin' },
                        bottom: { style: 'thin' },
                        right: { style: 'thin' }
                    };
                    cell.alignment = { vertical: 'middle', horizontal: 'center' };
                });
            }
        });

        // Xuất file
        const buffer = await workbook.xlsx.writeBuffer();
        const blob = new Blob([buffer], {
            type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        });
        saveAs(blob, `Bangdiem_${selectedSubject?.subjectName}_Lop${className}_HK${semesterId === '7' ? 'I' : 'II'}.xlsx`);
    };

    // Hàm tính điểm tổng kết
    const calculateFinalGrade = (student) => {
        if (!student.regularAssessments && !student.GK && !student.CK) {
            return '-';
        }

        const txScores = Object.values(student.regularAssessments)
            .filter(score => score !== null && score !== undefined && score !== '')
            .map(score => parseFloat(score));

        const txAverage = txScores.length > 0
            ? txScores.reduce((sum, score) => sum + score, 0) / txScores.length
            : 0;

        const gkScore = student.GK ? parseFloat(student.GK) : 0;
        const ckScore = student.CK ? parseFloat(student.CK) : 0;

        const finalGrade = (txAverage + gkScore * 2 + ckScore * 3) / 6;
        return finalGrade.toFixed(1);
    };

    return (
        <Button onClick={handleExport} variant="outline">
            Xuất Excel
        </Button>
    );
};

export default ExportExcelGrade;
