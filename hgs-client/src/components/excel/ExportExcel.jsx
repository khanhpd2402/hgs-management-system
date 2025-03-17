import ExcelJS from "exceljs";
import { saveAs } from "file-saver";
import { Button } from "@/components/ui/button"; // ShadCN Button
import PropTypes from "prop-types";
import { useState } from "react";

const ExportExcel = ({ fileName = "data.xlsx", allData, visibleColumns }) => {
  const [loading, setLoading] = useState(false);

  const handleExport = async () => {
    if (!allData || allData.length === 0) {
      alert("Không có dữ liệu để xuất!");
      return;
    }

    try {
      setLoading(true);
      const workbook = new ExcelJS.Workbook();
      const worksheet = workbook.addWorksheet("Danh sách giáo viên");

      // Lọc cột, bỏ qua cột "actions"
      const filteredColumns = visibleColumns.filter(
        ({ id }) => id !== "actions",
      );

      worksheet.columns = filteredColumns.map(({ id, label }) => ({
        header: label,
        key: id,
        width: 20,
      }));

      // Thêm dữ liệu vào bảng
      allData.forEach((row) => {
        const filteredRow = {};
        filteredColumns.forEach(({ id }) => {
          filteredRow[id] = row[id] || "";
        });
        worksheet.addRow(filteredRow);
      });

      // Thiết lập style cho header
      const headerRow = worksheet.getRow(1);
      headerRow.eachCell((cell) => {
        cell.font = { bold: true };
        cell.alignment = { horizontal: "center" };
        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: { argb: "FFFF00" },
        };
      });

      // Xuất file
      const buffer = await workbook.xlsx.writeBuffer();
      const blob = new Blob([buffer], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });

      try {
        saveAs(blob, fileName);
      } catch (saveError) {
        console.error("Lỗi khi tải file:", saveError);
        alert("Không thể tải file, vui lòng thử lại.");
      }
    } catch (error) {
      console.error("Lỗi khi xuất Excel:", error);
      alert("Lỗi khi xuất Excel: " + (error.message || "Không xác định"));
    } finally {
      setLoading(false);
    }
  };

  return (
    <Button onClick={handleExport} variant="outline" disabled={loading}>
      {loading ? "Đang xuất..." : "Xuất Excel"}
    </Button>
  );
};

ExportExcel.propTypes = {
  fileName: PropTypes.string,
  allData: PropTypes.array.isRequired,
  visibleColumns: PropTypes.arrayOf(
    PropTypes.shape({
      id: PropTypes.string.isRequired,
      label: PropTypes.string.isRequired,
    }),
  ).isRequired,
};

export default ExportExcel;
