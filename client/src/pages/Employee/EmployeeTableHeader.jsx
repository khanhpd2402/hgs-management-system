import ExcelImportModal from "@/components/excel/ExcelImportModal";
import ExportExcel from "@/components/excel/ExportExcel";
import PropTypes from "prop-types";
import EmployeeFilter from "./EmployeeFilter";

const EmployeeTableHeader = ({ type, setFilter }) => {
  return (
    <div className="mb-4 flex items-center justify-between">
      <h2 className="text-lg font-semibold">Danh sách cán bộ</h2>
      <div className="flex gap-2">
        <EmployeeFilter setFilter={setFilter} />
        <ExcelImportModal type={type} />
        <ExportExcel type={type} />
      </div>
    </div>
  );
};

EmployeeTableHeader.propTypes = {
  type: PropTypes.string.isRequired,
  data: PropTypes.string.isRequired,
};

export default EmployeeTableHeader;
