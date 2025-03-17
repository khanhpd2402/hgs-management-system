import { useState } from "react";
import ExcelImportModal from "@/components/excel/ExcelImportModal";
import ExportExcelByColumn from "@/components/excel/ExportExcelByColumn";
import PropTypes from "prop-types";
import StudentFilter from "./StudentFilter";
import { Button } from "@/components/ui/button";
import { Settings } from "lucide-react";
import ColumnConfigModal from "@/components/ColumnConfigModal";

const StudentTableHeader = ({
  type = "student",
  setFilter,
  setVisibleColumns,
  visibleColumns,
  columns,
}) => {
  const [isColumnConfigOpen, setIsColumnConfigOpen] = useState(false);
  return (
    <div className="mb-4 flex items-center justify-between">
      <h2 className="text-lg font-semibold">Danh sách học sinh</h2>
      <div className="flex gap-2">
        <StudentFilter setFilter={setFilter} />
        <ExcelImportModal type={type} />
        <ExportExcelByColumn type={type} />
        <Button
          variant="outline"
          onClick={() => setIsColumnConfigOpen(true)}
          className="flex items-center gap-1"
        >
          <Settings className="h-4 w-4" />
          Cấu hình cột hiển thị
        </Button>

        <ColumnConfigModal
          isOpen={isColumnConfigOpen}
          onClose={() => setIsColumnConfigOpen(false)}
          columns={columns}
          selectedColumns={visibleColumns}
          onSave={setVisibleColumns}
        />
      </div>
    </div>
  );
};

StudentTableHeader.propTypes = {
  type: PropTypes.string.isRequired,
  setFilter: PropTypes.func.isRequired,
  setVisibleColumns: PropTypes.func.isRequired,
  visibleColumns: PropTypes.array.isRequired,
  columns: PropTypes.array.isRequired,
};

export default StudentTableHeader;
