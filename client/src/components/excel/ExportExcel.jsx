import ExcelJS from "exceljs";
import { saveAs } from "file-saver";
import { Button } from "@/components/ui/button"; // ShadCN Button

const ExportExcel = ({ data, fileName = "data.xlsx" }) => {
  const handleExport = async () => {
    if (data.length === 0) {
      alert("Không có dữ liệu để xuất!");
      return;
    }

    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet("Sheet1");

    // Thêm tiêu đề cột
    const columns = Object.keys(data[0]).map((key) => ({
      header: key.toUpperCase(),
      key: key,
      width: 20,
    }));
    worksheet.columns = columns;

    // Thêm dữ liệu vào bảng
    data.forEach((row) => {
      worksheet.addRow(row);
    });

    // Thiết lập style cho header
    worksheet.getRow(1).eachCell((cell) => {
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
    saveAs(blob, fileName);
  };

  return (
    <Button onClick={handleExport} variant="outline">
      Export Excel
    </Button>
  );
};

export default ExportExcel;
