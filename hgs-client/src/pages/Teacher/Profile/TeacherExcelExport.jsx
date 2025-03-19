import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Download } from "lucide-react";
import ColumnConfigModal from "@/components/ColumnConfigModal";
import ExportExcel from "@/components/excel/ExportExcel";
import PropTypes from "prop-types";

export default function TeacherExcelExport({ data, columns }) {
  const [isColumnConfigOpen, setIsColumnConfigOpen] = useState(false);
  const [exportColumns, setExportColumns] = useState(
    columns.map((col) => col.id),
  );
  const [loading, setLoading] = useState(false);

  // Prepare data for Excel export
  const prepareExportData = () => {
    if (!data || data.length === 0) {
      return [];
    }

    // Export all data, ignoring pagination
    return data.map((teacher) => {
      const exportData = {};

      // Only include columns that are selected for export
      exportColumns.forEach((colId) => {
        // Map column IDs to the actual data properties
        switch (colId) {
          case "fullName":
            exportData["Họ tên cán bộ"] = teacher.fullName;
            break;
          case "phone":
            exportData["Số ĐTDD"] = teacher.phoneNumber;
            break;
          case "email":
            exportData["Địa chỉ Email"] = teacher.email;
            break;
          case "status":
            exportData["Trạng thái"] = teacher.employmentStatus;
            break;
          case "dob":
            exportData["Ngày sinh"] = teacher.dob;
            break;
          case "gender":
            exportData["Giới tính"] = teacher.gender;
            break;
          case "ethnicity":
            exportData["Dân tộc"] = teacher.ethnicity;
            break;
          case "position":
            exportData["Chức vụ"] = teacher.position;
            break;
          case "department":
            exportData["Tổ bộ môn"] = teacher.department;
            break;
          case "employmentType":
            exportData["Hình thức hợp đồng"] = teacher.employmentType;
            break;
        }
      });

      return exportData;
    });
  };

  const handleExportConfig = () => {
    setIsColumnConfigOpen(true);
  };

  const handleExportData = async (selectedColumns) => {
    setLoading(true);
    try {
      const exportData = prepareExportData();
      await ExportExcel.exportToExcel(exportData, "danh-sach-giao-vien");
    } catch (error) {
      console.error("Lỗi khi xuất Excel:", error);
      alert("Lỗi khi xuất Excel: " + (error.message || "Không xác định"));
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <Button
        variant="outline"
        onClick={handleExportConfig}
        className="flex items-center gap-1"
        disabled={loading}
      >
        <Download className="h-4 w-4" />
        {loading ? "Đang xuất..." : "Xuất Excel"}
      </Button>

      {/* Column Configuration Modal for Excel export */}
      <ColumnConfigModal
        isOpen={isColumnConfigOpen}
        onClose={() => setIsColumnConfigOpen(false)}
        columns={columns}
        selectedColumns={exportColumns}
        onSave={(selectedColumns) => {
          setExportColumns(selectedColumns);
          setIsColumnConfigOpen(false);
          handleExportData(selectedColumns);
        }}
      />
    </>
  );
}

TeacherExcelExport.propTypes = {
  data: PropTypes.array.isRequired,
  columns: PropTypes.array.isRequired,
};
