import React from 'react';

const ExportGradeToExcel = ({ gradeSummary, studentInfo, semesterName, academicYearName, gradeSummaries }) => {
    const printGradeReport = () => {
        if (!gradeSummary && !gradeSummaries) return;

        const printContent = document.createElement('div');
        printContent.className = 'print-content';

        let reports = '';

        if (gradeSummaries) {
            // Class-wide export
            const className = studentInfo?.className || '';
            reports = gradeSummaries.map((summary, index) => `
                <div style="font-family: Times New Roman, serif; padding: 15px; width: 190mm; margin: auto; page-break-after: always;">
                    <div style="text-align: center; margin-bottom: 15px;">
                        <div style="display: flex; justify-content: space-between; font-size: 13px;">
                            <div>
                                <p style="margin: 0;">SỞ GIÁO DỤC & ĐÀO TẠO Nam Định</p>
                                <p style="margin: 0;">TRƯỜNG THCS Hải Giang</p>
                                <p style="margin: 0;">---*---</p>
                            </div>
                            <div>
                                <p style="margin: 0;">CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</p>
                                <p style="margin: 0;">Độc lập - Tự do - Hạnh phúc</p>
                                <p style="margin: 0;">---o0o---</p>
                            </div>
                        </div>
                        <h2 style="margin: 15px 0; font-size: 16px;">PHIẾU ĐIỂM HỌC KỲ ${semesterName === "Học kỳ 1" ? "I" : "II"} - NĂM HỌC ${academicYearName}</h2>
                    </div>

                    <div style="margin-bottom: 15px; font-size: 13px;">
                        <p style="margin: 0;">Họ và tên học sinh: ${summary.studentName || ''}    Lớp: ${className}</p>
                    </div>

                    <table style="width: 100%; border-collapse: collapse; margin-bottom: 15px; font-size: 13px;">
                        <thead>
                            <tr>
                                <th style="border: 1px solid black; padding: 6px;">STT</th>
                                <th style="border: 1px solid black; padding: 6px;">MÔN HỌC</th>
                                <th style="border: 1px solid black; padding: 6px;">TB HK1</th>
                                <th style="border: 1px solid black; padding: 6px;">GHI CHÚ</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${summary.gradeSummaryEachSubjectNameDtos.map((subject, index) => `
                                <tr>
                                    <td style="border: 1px solid black; padding: 6px; text-align: center;">${index + 1}</td>
                                    <td style="border: 1px solid black; padding: 6px;">${subject.subjectName}</td>
                                    <td style="border: 1px solid black; padding: 6px; text-align: center;">${subject.semester1Average?.toFixed(1) || '-'}</td>
                                    <td style="border: 1px solid black; padding: 6px;"></td>
                                </tr>
                            `).join('')}
                            <tr>
                                <td colspan="2" style="border: 1px solid black; padding: 6px; text-align: center;">TB các môn</td>
                                <td style="border: 1px solid black; padding: 6px; text-align: center;">${summary.totalSemester1Average?.toFixed(1) || '-'}</td>
                                <td style="border: 1px solid black; padding: 6px;"></td>
                            </tr>
                        </tbody>
                    </table>

                    <div style="margin-bottom: 15px;">
                        <table style="width: 50%; border-collapse: collapse; font-size: 13px;">
                            <tr>
                                <td style="padding: 3px;">Xếp loại</td>
                                <td style="padding: 3px;">Học kỳ 1</td>
                                <td style="padding: 3px;">Ghi chú</td>
                            </tr>
                            <tr>
                                <td style="padding: 3px;">Học lực</td>
                                <td style="padding: 3px;">${summary.academicAbility || '-'}</td>
                                <td style="padding: 3px;"></td>
                            </tr>
                            <tr>
                                <td style="padding: 3px;">Hạnh kiểm</td>
                                <td style="padding: 3px;">${summary.conduct || '-'}</td>
                                <td style="padding: 3px;"></td>
                            </tr>
                            <tr>
                                <td style="padding: 3px;">Danh hiệu thi đua</td>
                                <td style="padding: 3px;">${summary.competitionTitle || '-'}</td>
                                <td style="padding: 3px;"></td>
                            </tr>
                        </table>
                    </div>

                    <div style="margin-bottom: 15px; font-size: 13px;">
                        <p style="margin: 0;">Nhận xét: ${summary.comment || ''}</p>
                    </div>

                    <div style="text-align: right; margin-top: 20px; font-size: 13px;">
                        <p style="margin: 0;">Nam Định, ngày ... tháng ... năm ...</p>
                        <p style="margin: 40px 0 0 0;">Giáo viên quản nhiệm</p>
                        <p style="margin: 40px 0 0 0;">${summary.teacherName || '...'}</p>
                    </div>
                </div>
            `).join('');
        } else {
            // Single student export
            reports = `
                <div style="font-family: Times New Roman, serif; padding: 15px; width: 190mm; margin: auto;">
                    <div style="text-align: center; margin-bottom: 15px;">
                        <div style="display: flex; justify-content: space-between; font-size: 13px;">
                            <div>
                                <p style="margin: 0;">SỞ GIÁO DỤC & ĐÀO TẠO Nam Định</p>
                                <p style="margin: 0;">TRƯỜNG THCS Hải Giang</p>
                                <p style="margin: 0;">---*---</p>
                            </div>
                            <div>
                                <p style="margin: 0;">CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</p>
                                <p style="margin: 0;">Độc lập - Tự do - Hạnh phúc</p>
                                <p style="margin: 0;">---o0o---</p>
                            </div>
                        </div>
                        <h2 style="margin: 15px 0; font-size: 16px;">PHIẾU ĐIỂM HỌC KỲ ${semesterName === "Học kỳ 1" ? "I" : "II"} - NĂM HỌC ${academicYearName}</h2>
                    </div>

                    <div style="margin-bottom: 15px; font-size: 13px;">
                        <p style="margin: 0;">Họ và tên học sinh: ${studentInfo?.studentName || ''}    Lớp: ${studentInfo?.className || ''}</p>
                    </div>

                    <table style="width: 100%; border-collapse: collapse; margin-bottom: 15px; font-size: 13px;">
                        <thead>
                            <tr>
                                <th style="border: 1px solid black; padding: 6px;">STT</th>
                                <th style="border: 1px solid black; padding: 6px;">MÔN HỌC</th>
                                <th style="border: 1px solid black; padding: 6px;">TB HK1</th>
                                <th style="border: 1px solid black; padding: 6px;">GHI CHÚ</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${gradeSummary.gradeSummaryEachSubjectNameDtos.map((subject, index) => `
                                <tr>
                                    <td style="border: 1px solid black; padding: 6px; text-align: center;">${index + 1}</td>
                                    <td style="border: 1px solid black; padding: 6px;">${subject.subjectName}</td>
                                    <td style="border: 1px solid black; padding: 6px; text-align: center;">${subject.semester1Average?.toFixed(1) || '-'}</td>
                                    <td style="border: 1px solid black; padding: 6px;"></td>
                                </tr>
                            `).join('')}
                            <tr>
                                <td colspan="2" style="border: 1px solid black; padding: 6px; text-align: center;">TB các môn</td>
                                <td style="border: 1px solid black; padding: 6px; text-align: center;">${gradeSummary.totalSemester1Average?.toFixed(1) || '-'}</td>
                                <td style="border: 1px solid black; padding: 6px;"></td>
                            </tr>
                        </tbody>
                    </table>

                    <div style="margin-bottom: 15px;">
                        <table style="width: 50%; border-collapse: collapse; font-size: 13px;">
                            <tr>
                                <td style="padding: 3px;">Xếp loại</td>
                                <td style="padding: 3px;">Học kỳ 1</td>
                                <td style="padding: 3px;">Ghi chú</td>
                            </tr>
                            <tr>
                                <td style="padding: 3px;">Học lực</td>
                                <td style="padding: 3px;">${gradeSummary.academicAbility || '-'}</td>
                                <td style="padding: 3px;"></td>
                            </tr>
                            <tr>
                                <td style="padding: 3px;">Hạnh kiểm</td>
                                <td style="padding: 3px;">${gradeSummary.conduct || '-'}</td>
                                <td style="padding: 3px;"></td>
                            </tr>
                            <tr>
                                <td style="padding: 3px;">Danh hiệu thi đua</td>
                                <td style="padding: 3px;">${gradeSummary.competitionTitle || '-'}</td>
                                <td style="padding: 3px;"></td>
                            </tr>
                        </table>
                    </div>

                    <div style="margin-bottom: 15px; font-size: 13px;">
                        <p style="margin: 0;">Nhận xét: ${gradeSummary.comment || ''}</p>
                    </div>

                    <div style="text-align: right; margin-top: 20px; font-size: 13px;">
                        <p style="margin: 0;">Nam Định, ngày ... tháng ... năm ...</p>
                        <p style="margin: 40px 0 0 0;">Giáo viên quản nhiệm</p>
                        <p style="margin: 40px 0 0 0;">${gradeSummary.teacherName || '...'}</p>
                    </div>
                </div>
            `;
        }

        printContent.innerHTML = reports;

        // Thêm style cho in ấn
        const style = document.createElement('style');
        style.textContent = `
            @media print {
                body * {
                    visibility: hidden;
                }
                .print-content, .print-content * {
                    visibility: visible;
                }
                .print-content {
                    position: absolute;
                    left: 0;
                    top: 0;
                }
                @page {
                    size: A4;
                    margin: 0;
                }
            }
        `;
        printContent.appendChild(style);

        // Thêm vào body
        document.body.appendChild(printContent);

        // In
        window.print();

        // Xóa phần tử tạm sau khi in
        document.body.removeChild(printContent);
    };

    return (
        <button
            onClick={printGradeReport}
            className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition-colors"
            disabled={!gradeSummary && !gradeSummaries}
        >
            In phiếu điểm
        </button>
    );
};

export default ExportGradeToExcel;